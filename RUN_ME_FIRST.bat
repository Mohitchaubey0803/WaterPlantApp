@echo off
color 0B
echo ============================================
echo   AquaPure Water Plant - First Time Setup
echo ============================================
echo.
echo Step 1: Restoring NuGet packages...
dotnet restore WaterPlantApp.csproj
if %errorlevel% neq 0 (
    echo ERROR: dotnet restore failed.
    echo Make sure .NET 8 SDK is installed: https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)
echo [OK] Packages restored.
echo.
echo Step 2: Building project...
dotnet build WaterPlantApp.csproj -c Release --no-restore
if %errorlevel% neq 0 (
    echo ERROR: Build failed. Check the errors above.
    pause
    exit /b 1
)
echo [OK] Build successful.
echo.
echo ============================================
echo   BEFORE RUNNING:
echo   1. Open SQL Server Management Studio
echo   2. Run: Database_Schema.sql
echo   3. Edit appsettings.json if needed
echo      (change Server=.\SQLEXPRESS if required)
echo ============================================
echo.
echo Step 3: Starting Web Application...
echo.
echo  Website: http://localhost:5000
echo  Stores:  http://localhost:5000/Store
echo  QR Code: http://localhost:5000/Store/QRPage
echo  Admin:   http://localhost:5000/Store/Manage
echo.
echo Press Ctrl+C to stop the server.
echo.
dotnet run --project WaterPlantApp.csproj --launch-profile http
pause
