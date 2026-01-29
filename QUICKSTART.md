# ChatRobor 快速入门指南

## ⚡ 5 分钟快速开始

### 第 1 步：克隆项目

```bash
git clone <repository-url>
cd ChatRobor
```

### 第 2 步：编辑配置文件

编辑 `appsettings.json`，配置以下内容：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ChatRobor;Trusted_Connection=true;"
  },
  "DeepSeek": {
    "ApiUrl": "https://api.deepseek.com/chat/completions",
    "ApiKey": "sk_live_YOUR_API_KEY",
    "Model": "deepseek-chat"
  }
}
```

**注意**：从 [DeepSeek 官网](https://platform.deepseek.com) 获取您的 API 密钥

### 第 3 步：安装和初始化

```bash
# 恢复 NuGet 包
dotnet restore

# 应用数据库迁移
dotnet ef database update

# 运行应用
dotnet run
```

### 第 4 步：访问应用

1. 打开浏览器访问 `https://localhost:5001`
2. 点击 "Sign Up" 创建账户
3. 使用邮箱和密码登录
4. 开始使用聊天功能！

## 功能导览

### 1️⃣ 聊天 (`/Chat`)

- **创建新会话**：点击左侧栏的"+ New Chat"按钮
- **发送消息**：在输入框中输入内容后按 Enter 或点击发送按钮
- **查看历史**：左侧栏显示所有历史会话
- **删除会话**：点击会话旁的删除按钮

### 2️⃣ 设置 (`/Settings`)

访问用户设置页面自定义：

- **Profile**：修改姓名和电话号码
- **Preferences**：
  - 选择 AI 模型（DeepSeek Chat / Coder）
  - 调整温度（0.0 - 2.0，默认 0.7）
  - 设置最大 Token 数（默认 2048）
  - 启用/禁用时间戳和通知
- **Security**：修改账户密码

### 3️⃣ 管理面板 (`/Admin`)

**仅 Admin 用户可访问**

- **Users**：管理所有用户，停用/激活账户
- **Roles**：创建自定义角色

## 常见操作

### 修改 AI 模型参数

```
设置 → Preferences → 调整参数 → 保存
```

参数说明：
- **Temperature**: 控制输出的随机性
  - 0 = 最确定性（重复）
  - 1 = 平衡
  - 2 = 最随机（创意）
  
- **Max Tokens**: 单条回复的最大长度
  - 建议范围：1024 - 4096

### 创建新会话

方法 1（推荐）：
```
点击左侧栏 "+ New Chat" 按钮
```

方法 2：
```
聊天页面顶部 → 创建新会话
```

### 导出对话记录

当前版本支持手动复制。建议功能：
- 右键选择文本复制
- 手动保存到文件
- 或使用浏览器的打印功能

## 模板提示词

在聊天中使用这些提示获得最佳效果：

### 编程助手
```
你是一个资深的编程专家。请帮我用 C# 实现 [功能描述]
```

### 文案撰写
```
请帮我撰写一篇关于 [主题] 的 [文案类型]
```

### 数据分析
```
我有一些数据：[数据]。请分析这些数据的趋势和洞察
```

### 学习辅导
```
请详细解释 [概念] 的含义，并举例说明
```

## 故障排除

### ❌ 无法登录

**问题**：登录失败或密码错误
**解决**：
1. 确保邮箱和密码正确
2. 检查账号是否被停用
3. 重新注册新账号

### ❌ 无法发送消息

**问题**：发送消息后没有回复
**解决**：
1. 检查 DeepSeek API 密钥是否正确
2. 检查网络连接
3. 查看浏览器控制台（F12）的错误信息
4. 检查 API 请求额度

### ❌ 数据库错误

**问题**："连接字符串错误"
**解决**：
```bash
# 确保 LocalDB 运行
sqllocaldb start

# 检查数据库
sqlcmd -S (localdb)\mssqllocaldb
```

### ❌ DeepSeek API 错误 (401/403)

**问题**：API 返回未授权错误
**解决**：
1. 验证 API 密钥是否正确
2. 检查 API 密钥是否已过期
3. 访问 [DeepSeek 控制面板](https://platform.deepseek.com) 生成新密钥

### ❌ 页面显示空白

**问题**：应用打开后什么都看不到
**解决**：
1. 按 F12 打开开发者工具查看错误
2. 检查浏览器控制台的 JavaScript 错误
3. 尝试清除浏览器缓存
4. 使用无痕模式重新测试

## 开发者命令速查

```bash
# 运行应用
dotnet run

# 构建项目
dotnet build

# 发布项目
dotnet publish -c Release

# 创建迁移
dotnet ef migrations add MigrationName

# 应用迁移
dotnet ef database update

# 回滚迁移
dotnet ef database update PreviousMigration

# 查看数据库
sqlcmd -S (localdb)\mssqllocaldb -d ChatRobor
```

## 项目文件说明

| 文件/文件夹 | 用途 |
|-----------|------|
| `Program.cs` | 应用配置和启动 |
| `appsettings.json` | 数据库和 API 配置 |
| `Models/` | 数据模型定义 |
| `Controllers/` | 业务逻辑控制器 |
| `Services/` | 业务服务类 |
| `Views/` | HTML 视图模板 |
| `wwwroot/` | 静态文件（CSS、JS、图片） |
| `Data/` | 数据库上下文 |

## 下一步

- 📖 阅读 [DEVELOPMENT.md](./DEVELOPMENT.md) 了解开发指南
- 🏗️ 阅读 [ARCHITECTURE.md](./ARCHITECTURE.md) 了解系统架构
- 🔗 查看 [DeepSeek API 文档](https://platform.deepseek.com/docs)
- 💬 查看 [ASP.NET Core 文档](https://docs.microsoft.com/aspnet/core)

## 需要帮助？

1. 检查本文档
2. 查看项目中的注释代码
3. 提交 Issue
4. 查看官方文档

祝您使用愉快！🎉
