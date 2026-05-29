@echo off
chcp 65001 >nul
cd /d "%~dp0"

echo === dotnet build ===
dotnet build -c Release
if errorlevel 1 exit /b 1

echo.
echo === dotnet publish (для Inno Setup) ===
dotnet publish -c Release -r win-x64 --self-contained false
if errorlevel 1 exit /b 1

echo.
echo Готово:
echo   EXE: bin\Release\net8.0-windows\win-x64\publish\KidsComputerTimeGuard.exe
echo.
echo Следующий шаг — откройте KidsComputerTimeGuard.iss в Inno Setup Compiler.
pause
