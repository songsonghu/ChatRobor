using ChatRobor.Models;
using ChatRobor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChatRobor.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;
        private readonly IDeepSeekService _deepSeekService;
        private readonly IUserPreferenceService _userPreferenceService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ChatController> _logger;

        public ChatController(
            IChatService chatService,
            IDeepSeekService deepSeekService,
            IUserPreferenceService userPreferenceService,
            UserManager<ApplicationUser> userManager,
            ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _deepSeekService = deepSeekService;
            _userPreferenceService = userPreferenceService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var conversations = await _chatService.GetUserConversationsAsync(userId);
            return View(conversations);
        }

        public async Task<IActionResult> Conversation(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var conversation = await _chatService.GetConversationAsync(id, userId);
            if (conversation == null)
                return NotFound();

            var preference = await _userPreferenceService.GetUserPreferenceAsync(userId);
            ViewBag.Preference = preference;

            return View(conversation);
        }

        [HttpPost]
        public async Task<IActionResult> CreateConversation()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var conversation = await _chatService.CreateConversationAsync(userId);
            return RedirectToAction(nameof(Conversation), new { id = conversation.Id });
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int conversationId, string message)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var conversation = await _chatService.GetConversationAsync(conversationId, userId);
            if (conversation == null)
                return NotFound();

            try
            {
                // Save user message
                await _chatService.AddMessageAsync(conversationId, message, "user");

                // Get user preferences
                var preference = await _userPreferenceService.GetUserPreferenceAsync(userId);

                // Get conversation history
                var messages = await _chatService.GetConversationMessagesAsync(conversationId);
                var history = messages.Where(m => m.Role != "user" || m.Content != message)
                    .Select(m => (m.Role, m.Content))
                    .ToList();

                // Get response from DeepSeek
                var response = await _deepSeekService.SendMessageAsync(
                    message,
                    preference.Temperature,
                    preference.MaxTokens,
                    history
                );

                // Save assistant message
                await _chatService.AddMessageAsync(conversationId, response, "assistant");

                return Json(new { success = true, message = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message");
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConversation(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await _chatService.DeleteConversationAsync(id, userId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTitle(int id, string title)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await _chatService.UpdateConversationTitleAsync(id, title, userId);
            return Json(new { success = true });
        }
    }
}
