# ChatRobor 部署指南

## 部署方式对比

| 方式 | 难度 | 成本 | 维护 | 推荐场景 |
|------|------|------|------|---------|
| 本地运行 | ⭐ | 免费 | 低 | 开发、测试 |
| Windows 服务器 | ⭐⭐ | ₩₩ | 中 | 企业内部 |
| Azure App Service | ⭐⭐⭐ | ₩₩₩ | 低 | 生产环境 |
| Docker + 容器 | ⭐⭐⭐ | ₩ | 低 | 云环境、DevOps |
| 虚拟服务器 (VPS) | ⭐⭐⭐⭐ | ₩₩ | 高 | 自托管 |

## 1. 本地部署（开发）

### 步骤

```bash
# 克隆或进入项目目录
cd ChatRobor

# 恢复包
dotnet restore

# 应用迁移
dotnet ef database update

# 运行应用
dotnet run
```

访问：`https://localhost:5001`

## 2. Windows 服务器部署

### 先决条件

- Windows Server 2016 或更新版本
- .NET 8.0 Runtime
- SQL Server 2019 或更新版本
- IIS（可选）

### 部署步骤

#### A. 发布应用

```bash
dotnet publish -c Release -o ./publish
```

#### B. 复制到服务器

```powershell
# 使用 SCP 或其他工具上传 publish 文件夹到服务器
scp -r ./publish user@server:/path/to/app
```

#### C. 配置服务器

1. **安装 .NET Runtime**
   ```powershell
   # 访问 https://dotnet.microsoft.com/download
   # 下载并安装 Runtime
   ```

2. **配置 SQL Server**
   ```sql
   -- 创建数据库
   CREATE DATABASE ChatRobor;
   
   -- 创建登录
   CREATE LOGIN ChatRobor_User WITH PASSWORD = 'Strong@Password123';
   USE ChatRobor;
   CREATE USER ChatRobor_User FOR LOGIN ChatRobor_User;
   ALTER ROLE db_owner ADD MEMBER ChatRobor_User;
   ```

3. **更新连接字符串**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=SERVER_NAME;Database=ChatRobor;User Id=ChatRobor_User;Password=Strong@Password123;"
     }
   }
   ```

4. **应用迁移**
   ```powershell
   cd /path/to/app
   dotnet ef database update
   ```

#### D. 配置 IIS（可选）

1. **安装 ASP.NET Core 托管包**
   - 访问：https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-aspnetcore-8.0-windows-hosting-bundle-installer

2. **创建应用池**
   - 名称：ChatRobor
   - .NET 版本：无托管代码
   - 管道模式：集成

3. **创建网站**
   - 物理路径：`/path/to/publish`
   - 应用池：ChatRobor
   - 绑定：https（配置 SSL 证书）

4. **启动应用**
   ```powershell
   # 或在 IIS 中启动网站
   net start w3svc
   ```

#### E. 作为 Windows 服务运行

创建服务安装脚本 `install-service.ps1`：

```powershell
# 以管理员身份运行

$appPath = "C:\path\to\app\ChatRobor.dll"
$serviceName = "ChatRobor"
$displayName = "ChatRobor Chat Application"

# 创建服务
New-Service -Name $serviceName -DisplayName $displayName `
    -BinaryPathName "C:\Program Files\dotnet\dotnet.exe $appPath" `
    -StartupType Automatic

# 启动服务
Start-Service -Name $serviceName
```

运行：
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
.\install-service.ps1
```

## 3. Azure 部署

### 先决条件

- Azure 账户（免费试用或付费）
- Azure CLI 或 Visual Studio

### 快速部署

#### 使用 Azure CLI

```bash
# 登录 Azure
az login

# 创建资源组
az group create --name ChatRobor-RG --location eastasia

# 创建 App Service 计划
az appservice plan create --name ChatRobor-Plan --resource-group ChatRobor-RG --sku FREE

# 创建 Web 应用
az webapp create --resource-group ChatRobor-RG --plan ChatRobor-Plan --name chatrObor-app

# 创建 SQL Server 和数据库
az sql server create --name chatrObor-sql --resource-group ChatRobor-RG \
    --admin-user sqladmin --admin-password 'Strong@Password123'

az sql db create --resource-group ChatRobor-RG --server chatrObor-sql \
    --name ChatRobor --edition Basic

# 配置应用设置
az webapp config appsettings set --resource-group ChatRobor-RG --name chatrObor-app \
    --settings "ConnectionStrings__DefaultConnection=Server=chatrObor-sql.database.windows.net;Database=ChatRobor;User Id=sqladmin;Password=Strong@Password123;" \
    "DeepSeek__ApiKey=your_api_key"

# 部署
az webapp deployment source config-zip --resource-group ChatRobor-RG --name chatrObor-app --src publish.zip
```

#### 使用 Visual Studio

1. 右键项目 → 发布
2. 选择 Azure 作为目标
3. 按照向导完成部署

## 4. Docker 部署

### Dockerfile

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
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "ChatRobor.dll"]
```

### Docker Compose

创建 `docker-compose.yml`：

```yaml
version: '3.8'

services:
  app:
    build: .
    ports:
      - "5000:80"
      - "5001:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=db;Database=ChatRobor;User Id=sa;Password=Strong@Password123;
      - DeepSeek__ApiKey=your_api_key
    depends_on:
      - db
    volumes:
      - ./certs:/app/certs

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    ports:
      - "1433:1433"
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=Strong@Password123
      - MSSQL_PID=Express
    volumes:
      - sqldata:/var/opt/mssql

volumes:
  sqldata:
```

### 构建和运行

```bash
# 构建镜像
docker build -t chatrobor:latest .

# 使用 Docker Compose
docker-compose up -d

# 查看日志
docker-compose logs -f app

# 停止
docker-compose down
```

## 5. VPS 部署（CentOS/Ubuntu）

### 步骤

1. **安装依赖**
   ```bash
   # Ubuntu
   sudo apt-get update
   sudo apt-get install dotnet-runtime-8.0
   sudo apt-get install mssql-server
   
   # CentOS
   sudo yum install dotnet-runtime-8.0
   sudo yum install mssql-server
   ```

2. **上传应用**
   ```bash
   scp -r ./publish user@server:/var/www/chatrobor
   ```

3. **创建 Systemd 服务**
   ```bash
   sudo nano /etc/systemd/system/chatrobor.service
   ```
   
   ```ini
   [Unit]
   Description=ChatRobor Application
   After=network.target
   
   [Service]
   Type=notify
   User=www-data
   WorkingDirectory=/var/www/chatrobor
   ExecStart=/usr/bin/dotnet /var/www/chatrobor/ChatRobor.dll
   Restart=on-failure
   RestartSec=10
   StandardOutput=journal
   StandardError=journal
   
   [Install]
   WantedBy=multi-user.target
   ```

4. **启动服务**
   ```bash
   sudo systemctl daemon-reload
   sudo systemctl enable chatrobor
   sudo systemctl start chatrobor
   sudo systemctl status chatrobor
   ```

5. **配置 Nginx 反向代理**
   ```bash
   sudo nano /etc/nginx/sites-available/chatrobor
   ```
   
   ```nginx
   server {
       listen 80;
       server_name your-domain.com;
       
       location / {
           proxy_pass http://localhost:5000;
           proxy_http_version 1.1;
           proxy_set_header Upgrade $http_upgrade;
           proxy_set_header Connection keep-alive;
           proxy_set_header Host $host;
           proxy_cache_bypass $http_upgrade;
       }
   }
   ```

6. **启用 SSL（Let's Encrypt）**
   ```bash
   sudo apt-get install certbot python3-certbot-nginx
   sudo certbot --nginx -d your-domain.com
   ```

## 部署检查清单

- [ ] 配置了 `appsettings.json` 中的所有必需设置
- [ ] 设置了强数据库密码
- [ ] 设置了 DeepSeek API 密钥
- [ ] 配置了 HTTPS/SSL 证书
- [ ] 设置了防火墙规则
- [ ] 配置了备份策略
- [ ] 设置了日志和监控
- [ ] 测试了数据库连接
- [ ] 测试了 API 调用
- [ ] 进行了负载测试

## 生产环境建议

1. **安全性**
   - 使用强密码
   - 启用 HTTPS
   - 配置 WAF（Web 应用防火墙）
   - 定期更新依赖包

2. **性能**
   - 启用缓存
   - 配置 CDN
   - 使用负载均衡
   - 优化数据库查询

3. **监控**
   - 设置日志聚合
   - 配置告警
   - 监控应用性能
   - 跟踪错误率

4. **备份**
   - 定期备份数据库
   - 备份应用配置
   - 测试恢复流程
   - 异地冗余备份

5. **更新**
   - 定期更新运行时
   - 及时更新依赖包
   - 计划维护窗口
   - 版本管理

## 故障排查

### 应用无法启动

```bash
# 查看日志
journalctl -u chatrobor -n 50

# 检查端口
netstat -tlnp | grep dotnet

# 手动运行测试
dotnet ChatRobor.dll --urls "http://localhost:5000"
```

### 数据库连接失败

```bash
# 测试连接
sqlcmd -S server_name -U user -P password -Q "SELECT 1"

# 检查防火墙
sudo ufw status
sudo ufw allow 1433
```

### 高 CPU/内存使用

```bash
# 检查进程
top -p $(pgrep -f dotnet)

# 调整应用设置
# 在 appsettings.json 中减少线程池大小
```

## 获取帮助

- 查看应用日志
- 检查系统日志
- 参考 [.NET 文档](https://docs.microsoft.com/dotnet)
- 提交 Issue
