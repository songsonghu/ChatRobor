#!/bin/bash

# ChatRobor 环境检查脚本

echo "========================================"
echo "ChatRobor 环境检查"
echo "========================================"
echo ""

# 检查 .NET SDK
echo "✓ 检查 .NET SDK..."
if command -v dotnet &> /dev/null; then
    DOTNET_VERSION=$(dotnet --version)
    echo "  ✓ .NET SDK 已安装: $DOTNET_VERSION"
else
    echo "  ✗ 未找到 .NET SDK"
    echo "  请访问: https://dotnet.microsoft.com/download"
    exit 1
fi

# 检查 SQL Server LocalDB
echo ""
echo "✓ 检查 SQL Server LocalDB..."
if command -v sqllocaldb &> /dev/null; then
    echo "  ✓ SQL Server LocalDB 已安装"
else
    echo "  ✗ 未找到 SQL Server LocalDB"
    echo "  请访问: https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb"
    exit 1
fi

# 检查 Git
echo ""
echo "✓ 检查 Git..."
if command -v git &> /dev/null; then
    GIT_VERSION=$(git --version)
    echo "  ✓ Git 已安装: $GIT_VERSION"
else
    echo "  ⚠ Git 未安装（可选，但推荐）"
fi

# 检查项目文件
echo ""
echo "✓ 检查项目文件..."
if [ -f "ChatRobor.csproj" ]; then
    echo "  ✓ 项目文件存在"
else
    echo "  ✗ 项目文件不存在"
    exit 1
fi

if [ -f "appsettings.json" ]; then
    echo "  ✓ 配置文件存在"
else
    echo "  ✗ 配置文件不存在"
    exit 1
fi

# 检查目录结构
echo ""
echo "✓ 检查目录结构..."
REQUIRED_DIRS=("Models" "Controllers" "Services" "Data" "Views" "wwwroot")
for dir in "${REQUIRED_DIRS[@]}"; do
    if [ -d "$dir" ]; then
        echo "  ✓ $dir 目录存在"
    else
        echo "  ✗ $dir 目录不存在"
        exit 1
    fi
done

# 检查配置
echo ""
echo "✓ 检查配置..."
if grep -q "DefaultConnection" appsettings.json; then
    echo "  ✓ 数据库连接字符串已配置"
else
    echo "  ⚠ 数据库连接字符串未配置"
fi

if grep -q "DeepSeek" appsettings.json; then
    echo "  ✓ DeepSeek 配置已存在"
    if grep -q "your-deepseek-api-key-here" appsettings.json; then
        echo "  ⚠ DeepSeek API 密钥未配置，请修改 appsettings.json"
    else
        echo "  ✓ DeepSeek API 密钥已配置"
    fi
else
    echo "  ✗ DeepSeek 配置不存在"
fi

echo ""
echo "========================================"
echo "✓ 环境检查完成！"
echo "========================================"
echo ""
echo "接下来的步骤:"
echo "1. 配置 appsettings.json（数据库和 DeepSeek API 密钥）"
echo "2. 运行: dotnet restore"
echo "3. 运行: dotnet ef database update"
echo "4. 运行: dotnet run"
echo ""
