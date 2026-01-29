# ChatRobor 开发指南

## 项目初始化与运行

### 1. 安装依赖

```bash
dotnet restore
```

### 2. 配置数据库

#### 选项 A: 使用 LocalDB（推荐开发）

编辑 `appsettings.json`：

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ChatRobor;Trusted_Connection=true;"
}
```

#### 选项 B: 使用 SQL Server

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=ChatRobor;User Id=sa;Password=YOUR_PASSWORD;"
}
```

### 3. 配置 DeepSeek API

编辑 `appsettings.json`，添加 DeepSeek API 密钥：

```json
"DeepSeek": {
  "ApiUrl": "https://api.deepseek.com/chat/completions",
  "ApiKey": "sk_live_your_api_key_here",
  "Model": "deepseek-chat"
}
```

### 4. 数据库迁移

```bash
# 创建初始迁移（如果需要）
dotnet ef migrations add InitialCreate

# 应用迁移到数据库
dotnet ef database update
```

### 5. 运行应用

```bash
dotnet run
```

应用将在 `https://localhost:5001` 启动

## 常见任务

### 添加新的数据库模型

1. 在 `Models` 文件夹中创建新类
2. 在 `Data/ApplicationDbContext.cs` 中添加 `DbSet<YourModel>`
3. 创建迁移：`dotnet ef migrations add AddYourModel`
4. 更新数据库：`dotnet ef database update`

### 添加新的控制器

1. 在 `Controllers` 文件夹中创建新的控制器类
2. 继承 `Controller` 基类
3. 添加路由属性和 Action 方法
4. 在 `Views` 文件夹中创建对应的视图文件夹和 `.cshtml` 文件

### 添加新的服务

1. 在 `Services` 文件夹中创建接口
2. 实现接口类
3. 在 `Program.cs` 中注册服务：`builder.Services.AddScoped<IYourService, YourService>();`

### 修改样式

编辑 `wwwroot/css/site.css` 或在视图中添加 `<style>` 标签

### 添加前端脚本

在 `wwwroot/js/site.js` 中添加或创建新的 JavaScript 文件

## 项目结构说明

### Controllers（控制器）

- **HomeController**: 主页面
- **AccountController**: 登录、注册、登出
- **ChatController**: 聊天功能（创建会话、发送消息等）
- **SettingsController**: 用户设置
- **AdminController**: 管理面板（用户、角色管理）

### Models（模型）

- **ApplicationUser**: 扩展的用户模型
- **ChatConversation**: 聊天会话
- **ChatMessage**: 聊天消息
- **UserPreference**: 用户偏好设置

### Services（服务）

- **DeepSeekService**: 与 DeepSeek API 交互
- **ChatService**: 聊天相关业务逻辑
- **UserPreferenceService**: 用户偏好管理

### Data（数据访问）

- **ApplicationDbContext**: EF Core 数据库上下文

### Views（视图）

按控制器名称组织的 Razor 视图文件

## 部署

### 本地发布

```bash
dotnet publish -c Release -o ./publish
```

### Azure 部署

1. 创建 Azure App Service
2. 配置应用设置（连接字符串、API 密钥）
3. 部署发布的文件

### Docker 部署

创建 `Dockerfile`：

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ChatRobor.csproj", "."]
RUN dotnet restore "ChatRobor.csproj"
COPY . .
RUN dotnet build "ChatRobor.csproj" -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/build .
EXPOSE 80 443
ENTRYPOINT ["dotnet", "ChatRobor.dll"]
```

## 调试

### 启用详细日志

编辑 `appsettings.Development.json`：

```json
"Logging": {
  "LogLevel": {
    "Default": "Debug",
    "Microsoft.AspNetCore": "Information"
  }
}
```

### 使用 Visual Studio 调试

1. 在代码中设置断点
2. 按 F5 启动调试
3. 应用会在断点处暂停

### 查看数据库

使用 SQL Server Management Studio 或 Azure Data Studio 连接到 LocalDB：
- Server: `(localdb)\mssqllocaldb`
- Database: `ChatRobor`

## 常见问题

### Q: 如何重置数据库？

A: 
```bash
dotnet ef database drop
dotnet ef database update
```

### Q: 如何添加新用户角色？

A: 在应用运行时，进入管理面板 `/Admin/Roles` 创建新角色

### Q: DeepSeek API 返回 401 错误

A: 检查 `appsettings.json` 中的 API 密钥是否正确且未过期

### Q: 本地数据库无法连接

A: 
1. 确保 SQL Server LocalDB 已安装
2. 运行 `sqllocaldb info` 查看可用实例
3. 尝试重启 LocalDB: `sqllocaldb stop` 和 `sqllocaldb start`

## 性能优化建议

1. 使用异步操作（async/await）
2. 添加数据库索引
3. 实现缓存（如 Redis）
4. 使用 CDN 分发静态文件
5. 启用 GZIP 压缩

## 安全建议

1. 定期更新依赖包
2. 使用 HTTPS
3. 实现速率限制
4. 验证用户输入
5. 使用 CSRF 保护令牌
6. 实施日志和监控

## 参考资源

- [ASP.NET Core 官方文档](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core 文档](https://docs.microsoft.com/ef/core)
- [ASP.NET Identity 文档](https://docs.microsoft.com/aspnet/core/security/authentication/identity)
- [DeepSeek API 文档](https://platform.deepseek.com)
