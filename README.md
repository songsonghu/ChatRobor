# ChatRobor - DeepSeek Chat Web Application

一个基于 ASP.NET Core MVC 的现代化聊天应用，集成 DeepSeek AI 大模型，支持用户认证、角色管理、聊天历史记录等功能。

## 功能特性

### 🔐 用户认证与授权
- 用户注册和登录
- 安全的密码管理
- 基于角色的访问控制 (RBAC)
- 用户账户状态管理

### 💬 聊天功能
- 实时与 DeepSeek AI 对话
- 支持多个独立的聊天会话
- 保存完整的聊天历史记录
- 自动为新会话生成标题
- 消息时间戳显示

### ⚙️ 用户设置
- 个人资料管理
- AI 模型选择（DeepSeek Chat / DeepSeek Coder）
- 对话参数自定义（温度、最大 Token 等）
- 显示偏好设置
- 密码修改

### 👥 角色管理与管理面板
- 创建自定义角色
- 为用户分配角色
- 用户管理（列表、停用/激活）
- 基于角色的权限控制

## 技术栈

- **框架**: ASP.NET Core 8.0
- **架构**: MVC（Model-View-Controller）
- **数据库**: SQL Server + Entity Framework Core 8.0
- **认证**: ASP.NET Identity
- **前端**: HTML5、CSS3、Vanilla JavaScript
- **API 集成**: DeepSeek API

## 快速开始

### 环境要求
- .NET 8.0 SDK 或更高版本
- SQL Server 2019 或更高版本（或 LocalDB）
- 有效的 DeepSeek API 密钥

### 安装步骤

1. **配置数据库连接**
   编辑 `appsettings.json`，设置数据库连接字符串

2. **配置 DeepSeek API**
   在 `appsettings.json` 中添加您的 API 密钥：
   ```json
   "DeepSeek": {
     "ApiUrl": "https://api.deepseek.com/chat/completions",
     "ApiKey": "your-api-key-here",
     "Model": "deepseek-chat"
   }
   ```

3. **还原依赖包**
   ```bash
   dotnet restore
   ```

4. **应用数据库迁移**
   ```bash
   dotnet ef database update
   ```

5. **运行应用**
   ```bash
   dotnet run
   ```

6. **首次使用**
   - 访问 `https://localhost:5001`
   - 创建账户并登录
   - 开始使用聊天功能

## 项目结构

```
ChatRobor/
├── Models/              # 数据模型
├── Controllers/         # 控制器
├── Services/            # 业务逻辑
├── Data/               # 数据访问
├── Views/              # Razor 视图
├── wwwroot/            # 静态文件
├── Program.cs          # 应用入口
└── appsettings.json    # 配置文件
```

## 数据模型

- **ApplicationUser**: 应用用户（继承自 IdentityUser）
- **ChatConversation**: 聊天会话
- **ChatMessage**: 聊天消息
- **UserPreference**: 用户偏好设置

## 许可证

MIT License
