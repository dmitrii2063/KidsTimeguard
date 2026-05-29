# Kids Computer Time Guard

Ограничение использования ПК ребёнком в Windows 11: выключение вечером, блокировка до утра.

## Сборка

```powershell
cd KidsComputerTimeGuard
.\build.bat
```

или:

```powershell
dotnet build -c Release
dotnet publish -c Release -r win-x64 --self-contained false
```

## Установщик Inno Setup

1. Выполните `build.bat` (папка `bin\Release\net8.0-windows\win-x64\publish` должна существовать).
2. Откройте `KidsComputerTimeGuard.iss` в **Inno Setup Compiler**.
3. Соберите — установщик появится в `installer\KidsComputerTimeGuard_Setup.exe`.

## Режимы

| Режим | Запуск |
|--------|--------|
| Родитель | `--parent` + пароль |
| Ребёнок | без аргументов или `--child` |

## Первый запуск

1. Ярлык **«Родитель»** от имени администратора.
2. Задать пароль, время выключения/разблокировки, **Применить**.
3. Подтвердить автозапуск в Планировщике заданий.

Настройки: `%ProgramData%\KidsComputerTimeGuard\appsettings.json` (защита ACL).
