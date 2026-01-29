# 🎯 ChatRobor 项目完整文档总览

## 📋 项目简介

**ChatRobor** 是一个基于 ASP.NET Core MVC 8.0 的现代化 AI 聊天应用，集成 DeepSeek 大模型，提供用户认证、聊天管理、历史记录和个性化设置等完整功能。

### 核心特性

- ✅ **AI 聊天**：与 DeepSeek 大模型进行实时对话
- ✅ **会话管理**：创建、删除、搜索多个聊天会话
- ✅ **历史记录**：完整保存和查看聊天历史
- ✅ **用户认证**：安全的登录注册系统
- ✅ **角色管理**：基于角色的权限控制
- ✅ **个性化设置**：AI 模型参数、主题、语言等
- ✅ **管理面板**：用户和角色管理

## 📚 文档导航

### 快速开始

1. **[QUICKSTART.md](./QUICKSTART.md)** ⭐ 开始这里！
   - 5 分钟快速开始
   - 功能导览
   - 常见问题解答
   - 故障排除

2. **[README.md](./README.md)** 📖
   - 项目概述
   - 功能列表
   - 技术栈
   - 基本安装步骤

### 深入学习

3. **[DEVELOPMENT.md](./DEVELOPMENT.md)** 💻
   - 项目初始化
   - 开发环境配置
   - 常见开发任务
   - 项目结构说明

4. **[ARCHITECTURE.md](./ARCHITECTURE.md)** 🏗️
   - 系统架构
   - 组件详解
   - 数据库设计
   - 安全架构
   - 扩展性考虑

### 部署指南

5. **[DEPLOYMENT.md](./DEPLOYMENT.md)** 🚀
   - 5 种部署方式
   - 逐步部署说明
   - 故障排查
   - 生产环境建议

## 🗂️ 项目结构

```
ChatRobor/
│
├── 📄 配置文件
│   ├── Program.cs                   # 应用入口和配置
│   ├── ChatRobor.csproj            # 项目文件
│   ├── appsettings.json            # 生产配置
│   └── appsettings.Development.json # 开发配置
│
├── 📁 Models/                       # 数据模型
│   ├── ApplicationUser.cs           # 用户模型
│   ├── ChatConversation.cs         # 聊天会话
│   ├── ChatMessage.cs              # 聊天消息
│   └── UserPreference.cs           # 用户偏好
│
├── 📁 Controllers/                  # 业务控制器
│   ├── HomeController.cs           # 主页
│   ├── AccountController.cs        # 登录注册
│   ├── ChatController.cs           # 聊天功能
│   ├── SettingsController.cs       # 用户设置
│   └── AdminController.cs          # 管理面板
│
├── 📁 Services/                     # 业务服务
│   ├── DeepSeekService.cs          # DeepSeek API 集成
│   ├── ChatService.cs              # 聊天业务逻辑
│   └── UserPreferenceService.cs    # 偏好管理
│
├── 📁 Data/                         # 数据访问
│   └── ApplicationDbContext.cs      # EF Core 数据库上下文
│
├── 📁 Views/                        # Razor 视图
│   ├── Shared/
│   │   ├── _Layout.cshtml          # 主布局
│   │   └── _ViewStart.cshtml       # 视图启动
│   ├── Chat/
│   │   ├── Index.cshtml            # 会话列表
│   │   └── Conversation.cshtml     # 聊天窗口
│   ├── Account/
│   │   ├── Login.cshtml            # 登录页
│   │   ├── Register.cshtml         # 注册页
│   │   └── ...
│   ├── Settings/
│   │   └── Index.cshtml            # 用户设置
│   ├── Admin/
│   │   ├── Users.cshtml            # 用户管理
│   │   └── Roles.cshtml            # 角色管理
│   └── Home/
│       ├── Index.cshtml            # 首页
│       ├── Privacy.cshtml          # 隐私页
│       └── Error.cshtml            # 错误页
│
├── 📁 wwwroot/                      # 静态文件
│   ├── css/
│   │   └── site.css               # 全局样式
│   └── js/
│       └── site.js                # 全局脚本
│
└── 📄 文档
    ├── README.md                   # 项目说明
    ├── QUICKSTART.md               # 快速开始
    ├── DEVELOPMENT.md              # 开发指南
    ├── ARCHITECTURE.md             # 架构文档
    ├── DEPLOYMENT.md               # 部署指南
    ├── check-env.sh/bat            # 环境检查脚本
    └── Dockerfile                  # Docker 配置
```

## 🚀 快速开始（3 步）

### 1️⃣ 克隆项目
```bash
git clone <repository-url>
cd ChatRobor
```

### 2️⃣ 配置文件
编辑 `appsettings.json`：
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ChatRobor;Trusted_Connection=true;"
  },
  "DeepSeek": {
    "ApiKey": "sk_live_YOUR_API_KEY"
  }
}
```

### 3️⃣ 启动应用
```bash
dotnet restore
dotnet ef database update
dotnet run
```

访问 `https://localhost:5001`

## 💾 数据库设计

### 核心表结构

```
AspNetUsers (Identity)
  ├── 用户认证信息
  └── 扩展字段：FullName, Avatar, Theme 等

ChatConversations
  ├── Id (PK)
  ├── UserId (FK)
  ├── Title
  └── Messages[]

ChatMessages
  ├── Id (PK)
  ├── ConversationId (FK)
  ├── Content
  └── Role (user/assistant)

UserPreferences
  ├── Id (PK)
  ├── UserId (FK, Unique)
  └── Model, Temperature, MaxTokens...
```

## 🔐 安全特性

- ✅ ASP.NET Core Identity 认证
- ✅ 密码加密存储 (bcrypt)
- ✅ 基于角色的授权 (RBAC)
- ✅ HTTPS 强制使用
- ✅ CSRF 令牌保护
- ✅ XSS 防护
- ✅ 用户数据隔离
- ✅ SQL 注入防护 (参数化查询)

## 🎯 API 端点速查

### 认证
- `POST /Account/Register` - 注册
- `POST /Account/Login` - 登录
- `POST /Account/Logout` - 登出

### 聊天
- `GET /Chat/Index` - 会话列表
- `GET /Chat/Conversation/{id}` - 会话详情
- `POST /Chat/CreateConversation` - 创建会话
- `POST /Chat/SendMessage` - 发送消息
- `POST /Chat/DeleteConversation/{id}` - 删除会话

### 设置
- `GET /Settings/Index` - 设置页面
- `POST /Settings/UpdateProfile` - 更新资料
- `POST /Settings/UpdatePreferences` - 更新偏好
- `POST /Settings/ChangePassword` - 修改密码

### 管理
- `GET /Admin/Users` - 用户管理
- `GET /Admin/Roles` - 角色管理
- `POST /Admin/CreateRole` - 创建角色
- `POST /Admin/AssignRole` - 分配角色

## 🔧 技术栈

| 层级 | 技术 |
|------|------|
| **框架** | ASP.NET Core 8.0 MVC |
| **数据库** | SQL Server + EF Core 8.0 |
| **认证** | ASP.NET Core Identity |
| **前端** | HTML5, CSS3, Vanilla JavaScript |
| **API** | DeepSeek REST API |
| **容器** | Docker (可选) |

## 📋 配置说明

### appsettings.json

```json
{
  "Logging": { /* 日志配置 */ },
  "AllowedHosts": "*",
  
  "ConnectionStrings": {
    "DefaultConnection": "SQL Server 连接字符串"
  },
  
  "DeepSeek": {
    "ApiUrl": "https://api.deepseek.com/chat/completions",
    "ApiKey": "your-api-key",
    "Model": "deepseek-chat"
  }
}
```

### 环境变量（生产环境推荐）

```bash
# .env 文件或系统环境变量
ConnectionStrings__DefaultConnection=...
DeepSeek__ApiKey=...
ASPNETCORE_ENVIRONMENT=Production
```

## 🛠️ 常用命令

```bash
# 开发
dotnet run                          # 运行应用
dotnet watch run                    # 监视模式

# 数据库
dotnet ef migrations add MigrationName   # 创建迁移
dotnet ef database update                # 应用迁移
dotnet ef database drop                  # 删除数据库

# 构建
dotnet build                        # 构建
dotnet publish -c Release           # 发布

# 测试
dotnet test                         # 运行测试
```

## 📦 部署选项

| 选项 | 时间 | 成本 | 推荐度 |
|------|------|------|--------|
| 本地开发 | 5分钟 | 💰💰💰 | ⭐⭐⭐ |
| Windows 服务器 | 1小时 | 💰💰 | ⭐⭐ |
| Azure App Service | 15分钟 | 💰💰💰 | ⭐⭐⭐⭐ |
| Docker | 30分钟 | 💰 | ⭐⭐⭐⭐⭐ |
| VPS/自托管 | 2小时 | 💰💰 | ⭐⭐ |

详见 [DEPLOYMENT.md](./DEPLOYMENT.md)

## 🎓 学习路径

### 初级开发者
1. 阅读 [QUICKSTART.md](./QUICKSTART.md)
2. 本地运行应用
3. 尝试修改 UI 样式
4. 查看数据库表结构

### 中级开发者
1. 阅读 [ARCHITECTURE.md](./ARCHITECTURE.md)
2. 学习 EF Core 迁移
3. 实现新的 API 端点
4. 添加新的数据模型

### 高级开发者
1. 自定义认证流程
2. 实现缓存策略
3. 性能优化
4. 容器化部署

## 🐛 常见问题

### Q: 如何更改数据库？
A: 编辑 `appsettings.json` 中的 `ConnectionStrings`

### Q: DeepSeek API 返回 401？
A: 检查 API 密钥是否正确，访问 https://platform.deepseek.com

### Q: 如何添加新用户角色？
A: 进入管理面板 `/Admin/Roles`

### Q: 数据库无法连接？
A: 运行 `sqllocaldb start` 启动 LocalDB

### Q: 如何部署到生产环境？
A: 参考 [DEPLOYMENT.md](./DEPLOYMENT.md)

## 📞 获取支持

1. 📖 查看相关文档
2. 🔍 搜索 [GitHub Issues](https://github.com/songsonghu/ChatRobor/issues)
3. 💬 提交新 Issue
4. 🤝 提交 Pull Request

## 📝 许可证

MIT License - 详见 LICENSE 文件

## 🙏 致谢

感谢所有贡献者和用户的支持！

---

**最后更新**: 2024年1月  
**维护者**: ChatRobor 开发团队  
**官方网站**: [链接](http://example.com)  
**文档版本**: 1.0.0

## 下一步

- 👉 开始 [QUICKSTART.md](./QUICKSTART.md)
- 📚 深入 [ARCHITECTURE.md](./ARCHITECTURE.md)
- 🚀 部署 [DEPLOYMENT.md](./DEPLOYMENT.md)
- 💻 开发 [DEVELOPMENT.md](./DEVELOPMENT.md)
