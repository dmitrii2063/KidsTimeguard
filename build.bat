@echo off
title Сборка установщика Shutdown Scheduler
echo ========================================
echo   Сборка установщика для приложения
echo ========================================
echo.

:: Установка переменных
set INNO_COMPILER="C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
set SCRIPT_FILE="ShutdownScheduler.iss"
set OUTPUT_DIR="installer"

:: Проверка наличия Inno Setup
if not exist %INNO_COMPILER% (
    echo Ошибка: Inno Setup не найден!
    echo Установите Inno Setup 6 с официального сайта
    pause
    exit /b 1
)

:: Создание директории для выходных файлов
if not exist %OUTPUT_DIR% mkdir %OUTPUT_DIR%

:: Сборка установщика
echo Компиляция скрипта установщика...
%INNO_COMPILER% %SCRIPT_FILE%

if %errorlevel% == 0 (
    echo.
    echo ========================================
    echo   Установщик успешно создан!
    echo   Файл: installer\ShutdownScheduler_Setup.exe
    echo ========================================
) else (
    echo.
    echo Ошибка при создании установщика!
    echo Код ошибки: %errorlevel%
)

echo.
pause