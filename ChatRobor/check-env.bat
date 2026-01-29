@REM ChatRobor 环境检查脚本 (Windows)

@echo off
color 0A
echo.
echo ========================================
echo ChatRobor 环境检查
echo ========================================
echo.

REM 检查 .NET SDK
echo 检查 .NET SDK...
dotnet --version >nul 2>&1
if %errorlevel% equ 0 (
    for /f "tokens=*" %%i in ('dotnet --version') do set DOTNET_VERSION=%%i
    echo   [OK] .NET SDK 已安装: %DOTNET_VERSION%
) else (
    echo   [错误] 未找到 .NET SDK
    echo   请访问: https://dotnet.microsoft.com/download
    exit /b 1
)

REM 检查 SQL Server LocalDB
echo.
echo 检查 SQL Server LocalDB...
sqllocaldb v 2>&1 >nul
if %errorlevel% equ 0 (
    echo   [OK] SQL Server LocalDB 已安装
) else (
    echo   [错误] 未找到 SQL Server LocalDB
    echo   请访问: https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb
    exit /b 1
)

REM 检查项目文件
echo.
echo 检查项目文件...
if exist ChatRobor.csproj (
    echo   [OK] 项目文件存在
) else (
    echo   [错误] 项目文件不存在
    exit /b 1
)

if exist appsettings.json (
    echo   [OK] 配置文件存在
) else (
    echo   [错误] 配置文件不存在
    exit /b 1
)

REM 检查目录结构
echo.
echo 检查目录结构...
set REQUIRED_DIRS=Models Controllers Services Data Views wwwroot
for %%D in (%REQUIRED_DIRS%) do (
    if exist %%D (
        echo   [OK] %%D 目录存在
    ) else (
        echo   [错误] %%D 目录不存在
        exit /b 1
    )
)

echo.
echo ========================================
echo [OK] 环境检查完成！
echo ========================================
echo.
echo 接下来的步骤:
echo 1. 配置 appsettings.json（数据库和 DeepSeek API 密钥）
echo 2. 运行: dotnet restore
echo 3. 运行: dotnet ef database update
echo 4. 运行: dotnet run
echo.
pause
