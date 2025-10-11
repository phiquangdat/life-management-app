#!/usr/bin/env bash
set -euo pipefail

# Reproducible build and launch script for MacCatalyst debug (maccatalyst-arm64)
# Behaves like VS Code's ".NET MAUI" launch configuration
# Usage: ./script.sh

export COPYFILE_DISABLE=1
export DOTNET_CLI_TELEMETRY_OPTOUT=1

PROJECT_DIR="$(cd "$(dirname "$0")" && pwd)"
cd "$PROJECT_DIR"

echo "🚀 Starting .NET MAUI build and launch process..."

# Change to the NoteTakingApp directory where the project file is located
cd NoteTakingApp

# Pre-launch task: Restore and build (equivalent to "maui: Build" preLaunchTask)
echo "📦 Restoring packages..."
dotnet restore

echo "🔨 Building project..."
# Build parameters aligned with VS Code's Maui task
dotnet build -t:Build \
  -p:Configuration=Debug \
  -f net9.0-maccatalyst \
  -r maccatalyst-arm64 \
  -p:CustomAfterMicrosoftCSharpTargets="/Users/$USER/.vscode/extensions/ms-dotnettools.dotnet-maui-1.10.18-darwin-arm64/dist/resources/Custom.After.Microsoft.CSharp.targets" \
  -p:MauiVSCodeBuildOutputFile="/tmp/maui-vsc-build.json" \
  -p:MauiTargetProject="$PROJECT_DIR/NoteTakingApp/NoteTakingApp.csproj" \
  -p:XamlTools="/Users/$USER/.vscode/extensions/ms-dotnettools.csharp-2.93.22-darwin-arm64/.xamlTools" \
  NoteTakingApp.csproj

if [ $? -eq 0 ]; then
    echo "✅ Build completed successfully!"
    
    # Launch the app (equivalent to VS Code's "launch" request)
    echo "🚀 Launching .NET MAUI app..."
    dotnet run \
      -p NoteTakingApp.csproj \
      -f net9.0-maccatalyst \
      -r maccatalyst-arm64 \
      -c Debug \
      --no-build
    
    echo "🏁 App execution finished."
else
    echo "❌ Build failed. App will not be launched."
    exit 1
fi
