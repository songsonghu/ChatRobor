# ChatRobor 架构文档

## 系统架构概览

```
┌─────────────────────────────────────────────────────┐
│                    Client Layer                      │
│              (HTML5, CSS3, Vanilla JS)              │
└────────────────────┬────────────────────────────────┘
                     │ HTTP/HTTPS
┌────────────────────▼────────────────────────────────┐
│              ASP.NET Core MVC Layer                 │
│  ┌──────────────┬──────────────┬──────────────┐   │
│  │  Controllers │    Models    │    Views     │   │
│  └──────────────┴──────────────┴──────────────┘   │
└────────────────────┬────────────────────────────────┘
                     │
     ┌───────────────┼───────────────┐
     │               │               │
┌────▼────────┐ ┌───▼────────┐ ┌────▼──────────┐
│  Services   │ │   Data     │ │ External API │
│  Layer      │ │  Layer     │ │              │
└────────────┘ └────────────┘ └────┬─────────┘
     │               │              │
     └───────────────┼──────────────┘
                     │
         ┌───────────┴──────────┐
         │                      │
    ┌────▼─────────┐   ┌───────▼──────┐
    │  SQL Server  │   │ DeepSeek API │
    │  Database    │   │              │
    └──────────────┘   └────────────────┘
```

## 主要组件

### 1. 控制器层 (Controllers)

#### HomeController
- 主页展示
- 未登录用户的落地页

#### AccountController
- 用户注册
- 用户登录
- 用户登出
- 密码管理

#### ChatController
- 获取对话列表
- 创建新对话
- 发送消息
- 删除对话
- 更新对话标题

#### SettingsController
- 用户个人资料修改
- 对话偏好设置
- 密码修改

#### AdminController
- 用户管理
- 角色管理
- 权限分配

### 2. 模型层 (Models)

```
ApplicationUser
├── Id (string)
├── Email (string)
├── PasswordHash (string)
├── FullName (string)
├── Avatar (string)
├── CreatedAt (DateTime)
├── LastLogin (DateTime)
├── IsActive (bool)
├── Theme (string)
├── Language (string)
└── Navigation Properties
    ├── Conversations
    └── Preferences

ChatConversation
├── Id (int)
├── UserId (string) [FK]
├── Title (string)
├── CreatedAt (DateTime)
├── UpdatedAt (DateTime)
├── IsDeleted (bool)
└── Navigation Properties
    ├── User (ApplicationUser)
    └── Messages

ChatMessage
├── Id (int)
├── ConversationId (int) [FK]
├── Content (string)
├── Role (string) [user/assistant]
├── CreatedAt (DateTime)
├── TokenCount (int?)
└── Navigation Properties
    └── Conversation

UserPreference
├── Id (int)
├── UserId (string) [FK]
├── Model (string)
├── Temperature (float)
├── MaxTokens (int)
├── ShowTimestamp (bool)
├── EnableNotifications (bool)
├── UpdatedAt (DateTime)
└── Navigation Properties
    └── User
```

### 3. 服务层 (Services)

#### DeepSeekService
职责：与 DeepSeek API 交互

```csharp
public interface IDeepSeekService
{
    Task<string> SendMessageAsync(
        string message, 
        float temperature, 
        int maxTokens, 
        List<(string role, string content)> history
    );
}
```

特性：
- HTTP 客户端配置
- API 认证
- 错误处理和日志
- 消息历史管理

#### ChatService
职责：聊天数据管理和业务逻辑

```csharp
public interface IChatService
{
    Task<ChatConversation> CreateConversationAsync(string userId);
    Task<ChatMessage> AddMessageAsync(int conversationId, string content, string role);
    Task<IEnumerable<ChatConversation>> GetUserConversationsAsync(string userId);
    Task<ChatConversation> GetConversationAsync(int id, string userId);
    Task<IEnumerable<ChatMessage>> GetConversationMessagesAsync(int conversationId);
    Task DeleteConversationAsync(int conversationId, string userId);
    Task UpdateConversationTitleAsync(int conversationId, string title, string userId);
}
```

特性：
- 用户隔离（只获取用户自己的对话）
- 软删除支持
- 自动时间戳管理
- 事务性操作

#### UserPreferenceService
职责：用户偏好管理

```csharp
public interface IUserPreferenceService
{
    Task<UserPreference> GetUserPreferenceAsync(string userId);
    Task UpdateUserPreferenceAsync(string userId, UserPreference preference);
}
```

特性：
- 用户偏好的 CRUD 操作
- 默认值初始化
- 偏好缓存

### 4. 数据访问层 (Data)

#### ApplicationDbContext
- EF Core DbContext
- 数据库映射配置
- 导航属性定义
- 索引定义
- 级联删除规则

### 5. 身份认证与授权

使用 ASP.NET Core Identity：
- 用户注册和登录
- 密码哈希和验证
- 基于角色的授权 (RBAC)
- Claims 支持

角色：
- **Admin**: 管理员（用户和角色管理）
- **User**: 普通用户（聊天功能）

### 6. 数据库设计

#### 表结构

```sql
-- 用户表（由 Identity 管理）
AspNetUsers

-- 角色表（由 Identity 管理）
AspNetRoles

-- 用户角色关系表（由 Identity 管理）
AspNetUserRoles

-- 聊天会话表
ChatConversations
  ├── Id (PK)
  ├── UserId (FK -> AspNetUsers.Id)
  ├── Title
  ├── CreatedAt (索引)
  ├── UpdatedAt (索引)
  └── IsDeleted

-- 聊天消息表
ChatMessages
  ├── Id (PK)
  ├── ConversationId (FK -> ChatConversations.Id)
  ├── Content
  ├── Role
  ├── CreatedAt
  └── TokenCount

-- 用户偏好表
UserPreferences
  ├── Id (PK)
  ├── UserId (FK -> AspNetUsers.Id, Unique)
  ├── Model
  ├── Temperature
  ├── MaxTokens
  ├── ShowTimestamp
  ├── EnableNotifications
  └── UpdatedAt
```

#### 索引策略

```sql
-- 性能优化索引
CREATE INDEX IX_ChatConversations_UserId ON ChatConversations(UserId);
CREATE INDEX IX_ChatMessages_ConversationId ON ChatMessages(ConversationId);
CREATE UNIQUE INDEX IX_UserPreferences_UserId ON UserPreferences(UserId);
```

### 7. 请求/响应流程

#### 聊天流程

```
User Input
    │
    ▼
ChatController.SendMessage()
    │
    ├─▶ Validate User & Conversation
    │
    ├─▶ IChatService.AddMessageAsync() [User Message]
    │
    ├─▶ IUserPreferenceService.GetUserPreferenceAsync()
    │
    ├─▶ Get Conversation History
    │
    ├─▶ IDeepSeekService.SendMessageAsync()
    │   │
    │   ├─▶ HTTP POST to DeepSeek API
    │   │
    │   └─▶ Parse Response
    │
    ├─▶ IChatService.AddMessageAsync() [Assistant Response]
    │
    └─▶ Return JSON Response to Client
         │
         └─▶ Update UI with Messages
```

## 安全架构

### 认证

- ASP.NET Core Identity 提供用户认证
- 密码通过 bcrypt 加盐加密
- Claims-based 授权

### 授权

- [Authorize] 属性用于方法级别保护
- 角色检查：[Authorize(Roles = "Admin")]
- 自定义授权策略支持

### 数据保护

- HTTPS 强制使用
- CSRF 令牌验证
- 用户数据隔离（用户只能访问自己的数据）
- SQL 参数化查询（EF Core 自动）
- XSS 防护（Razor 默认 HTML 编码）

## 扩展性考虑

### 可水平扩展

- 无状态设计（session 可选）
- 支持负载均衡
- 数据库连接池

### 可垂直扩展

- 异步操作
- 缓存支持（Redis）
- 数据库优化

### 可维护性

- 清晰的分层架构
- 依赖注入
- 接口驱动设计
- 单一职责原则

## 性能考虑

1. **数据库查询优化**
   - 使用索引
   - 延迟加载（Include）
   - 分页查询

2. **缓存策略**
   - 用户偏好缓存
   - 会话列表缓存
   - API 响应缓存

3. **前端优化**
   - 静态文件压缩
   - 按需加载
   - 本地存储支持

## 监控与日志

- 内置 ASP.NET Core 日志
- 错误记录
- API 响应时间跟踪
- DeepSeek API 调用日志

## 部署拓扑

```
Internet
    │
    ▼
Load Balancer (可选)
    │
    ├─▶ Web Server Instance 1
    │   └─▶ ASP.NET Core App
    │
    ├─▶ Web Server Instance 2
    │   └─▶ ASP.NET Core App
    │
    └─▶ Web Server Instance N
        └─▶ ASP.NET Core App
            │
            └─▶ SQL Server (共享数据库)
                
External:
    └─▶ DeepSeek API
```

## 配置管理

```
appsettings.json (生产配置)
    ├── Logging
    ├── ConnectionStrings
    ├── DeepSeek API 设置
    └── ...

appsettings.Development.json (开发配置)
    ├── 详细日志
    ├── 开发数据库
    └── ...

User Secrets (敏感信息)
    └── API 密钥、数据库密码等
```

## API 接口规范

### 请求格式
- Content-Type: application/json 或 application/x-www-form-urlencoded
- 使用标准 HTTP 方法（GET、POST、PUT、DELETE）

### 响应格式
```json
{
    "success": true/false,
    "data": {...},
    "error": "error message",
    "message": "success message"
}
```

### 错误处理
- 400 Bad Request: 请求验证失败
- 401 Unauthorized: 未认证
- 403 Forbidden: 无权限
- 404 Not Found: 资源不存在
- 500 Internal Server Error: 服务器错误
