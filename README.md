# ABN Capture

Fast, open-source screen capture and recording for Windows.  
Бърза програма с отворен код за снимане и запис на екрана в Windows.  
Быстрая программа с открытым исходным кодом для снимков и записи экрана в Windows.

<!-- Add a screenshot or GIF here, e.g.: -->
<!-- ![ABN Capture](docs/screenshot.png) -->

<details open>
<summary><h2>🇬🇧 English</h2></summary>

ABN Capture lives in your system tray. Press a hotkey and the screen freezes, so you can pick exactly what you want to capture, then grab it in one click.

### Features

- **Frozen-screen selection** – the screen is captured first, dimmed, and you select the area on the frozen image. Nothing moves while you choose.
- **Floating toolbar** – a rounded bar at the bottom of the screen with Selection and Screen modes and a big round capture button.
- **Resizable selection** – drag to create, drag the round corner handles to resize, drag inside to move. The pixel size is shown live.
- **Direct mode** – optionally skip the toolbar and capture immediately after you release the mouse.
- **Screen recording** – record the screen to video (powered by Media Foundation).
- **Save and/or copy** – save PNG files to a folder of your choice, copy to the clipboard, or both.
- **Tray app** – no big window in the way; everything is available from the tray icon.
- **Custom hotkeys** – change the capture hotkeys in Settings.
- **Start with Windows** – optional.

### Default hotkeys

| Action | Hotkey |
|---|---|
| Select area | `Ctrl+Shift+S` |
| Full screen | `Ctrl+Shift+F` |

While the overlay is open: `Enter` captures, `Esc` cancels.

### Download

Get the latest `ABNCapture.exe` from the [Releases](https://github.com/abn-bg-online/ABN-Capture/releases/latest) page. It is a self-contained build, so you do not need to install .NET.

> The executable is not code-signed yet, so Windows SmartScreen may show "Windows protected your PC". Click **More info → Run anyway**.

### Requirements

- Windows 10 or Windows 11 (x64)
- Windows 10 **N / KN** editions need the *Media Feature Pack* for recording to work

### Where things are stored

- Screenshots: `Pictures\ABN Capture` (change it in Settings)
- Settings: `%APPDATA%\ABNCapture\settings.json`

### Build from source

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```powershell
git clone https://github.com/<your-username>/ABN-Capture.git
cd ABN-Capture
dotnet run -p:Platform=x64
```

Create a single-file release build:

```powershell
dotnet publish -c Release -p:Platform=x64 -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:IncludeAllContentForSelfExtract=true
```

The output is in `bin\x64\Release\net10.0-windows\win-x64\publish\`.

### Project structure

```
Assets/      App icon
Models/      AppSettings
Services/    Hotkey handling
Themes/      Shared WPF styles
Views/       Windows: main, settings, capture overlay, toolbar
```

### Built with

- [WPF](https://learn.microsoft.com/dotnet/desktop/wpf/) on .NET 10
- [ScreenRecorderLib](https://github.com/sskodje/ScreenRecorderLib) – screen recording (MIT)
- [H.NotifyIcon](https://github.com/HavenDV/H.NotifyIcon) – system tray icon (MIT)

### Contributing

Issues and pull requests are welcome. For larger changes, please open an issue first to discuss what you would like to change.

</details>

<details>
<summary><h2>🇧🇬 Български</h2></summary>

ABN Capture живее в системната област (tray). Натискаш клавишна комбинация и екранът замръзва, за да избереш точно какво искаш да заснемеш, а след това го заснемаш с един клик.

### Функции

- **Селекция върху замръзнал екран** – екранът първо се заснема и затъмнява, а ти избираш областта върху замръзналата картина. Нищо не се движи, докато избираш.
- **Плаваща лента** – заоблена лента най-долу на екрана с режими Selection и Screen и голям кръгъл бутон за заснемане.
- **Селекция с преоразмеряване** – влачиш, за да създадеш област, влачиш кръглите дръжки по ъглите, за да я преоразмериш, и влачиш отвътре, за да я преместиш. Размерът в пиксели се показва на живо.
- **Директен режим** – по желание пропускаш лентата и заснемаш веднага щом пуснеш мишката.
- **Запис на екрана** – записвай екрана във видео (чрез Media Foundation).
- **Запазване и/или копиране** – записвай PNG файлове в папка по избор, копирай в клипборда или и двете.
- **Tray приложение** – няма голям прозорец, който да пречи; всичко е достъпно от иконата в tray.
- **Собствени хоткеи** – клавишните комбинации за заснемане се сменят от Settings.
- **Стартиране с Windows** – по желание.

### Клавишни комбинации по подразбиране

| Действие | Комбинация |
|---|---|
| Избор на област | `Ctrl+Shift+S` |
| Цял екран | `Ctrl+Shift+F` |

Докато екранът със селекцията е отворен: `Enter` заснема, `Esc` отказва.

### Изтегляне

Свали последния `ABNCapture.exe` от страницата [Releases](https://github.com/abn-bg-online/ABN-Capture/releases/latest). Това е самостоятелна версия, така че не е нужно да инсталираш .NET.

> Изпълнимият файл още не е подписан, затова Windows SmartScreen може да покаже предупреждение „Windows protected your PC". Натисни **More info → Run anyway** (бутоните може да са преведени на езика на твоя Windows).

### Изисквания

- Windows 10 или Windows 11 (x64)
- При Windows 10 **N / KN** за записа е нужен *Media Feature Pack*

### Къде се пази всичко

- Снимки: `Pictures\ABN Capture` (сменя се от Settings)
- Настройки: `%APPDATA%\ABNCapture\settings.json`

### Компилиране от изходния код

Нужен е [.NET 10 SDK](https://dotnet.microsoft.com/download).

```powershell
git clone https://github.com/<your-username>/ABN-Capture.git
cd ABN-Capture
dotnet run -p:Platform=x64
```

Готова версия в един файл:

```powershell
dotnet publish -c Release -p:Platform=x64 -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:IncludeAllContentForSelfExtract=true
```

Резултатът е в `bin\x64\Release\net10.0-windows\win-x64\publish\`.

### Структура на проекта

```
Assets/      Икона на приложението
Models/      AppSettings
Services/    Работа с хоткеи
Themes/      Общи WPF стилове
Views/       Прозорци: главен, настройки, екран за селекция, лента
```

### Направено с

- [WPF](https://learn.microsoft.com/dotnet/desktop/wpf/) върху .NET 10
- [ScreenRecorderLib](https://github.com/sskodje/ScreenRecorderLib) – запис на екрана (MIT)
- [H.NotifyIcon](https://github.com/HavenDV/H.NotifyIcon) – икона в системната област (MIT)

### Принос

Докладите за проблеми (issues) и pull request-ите са добре дошли. При по-големи промени първо отвори issue, за да обсъдим какво искаш да промениш.

</details>

<details>
<summary><h2>🇷🇺 Русский</h2></summary>

ABN Capture живёт в системном трее. Нажимаешь горячую клавишу — экран замирает, ты выбираешь именно то, что нужно снять, и делаешь снимок одним кликом.

### Возможности

- **Выделение на замороженном экране** – сначала делается снимок экрана, он затемняется, и ты выделяешь область на «замороженной» картинке. Пока выбираешь, ничего не двигается.
- **Плавающая панель** – скруглённая панель внизу экрана с режимами Selection и Screen и большой круглой кнопкой съёмки.
- **Изменяемое выделение** – тяни, чтобы создать область; круглые маркеры по углам меняют размер; перетаскивание внутри перемещает область. Размер в пикселях показывается в реальном времени.
- **Прямой режим** – при желании можно пропустить панель и снимать сразу после отпускания кнопки мыши.
- **Запись экрана** – запись экрана в видео (на базе Media Foundation).
- **Сохранение и/или копирование** – сохраняй PNG в выбранную папку, копируй в буфер обмена или делай и то, и другое.
- **Приложение в трее** – никакого большого окна, всё доступно из значка в трее.
- **Свои горячие клавиши** – сочетания для съёмки меняются в настройках.
- **Запуск вместе с Windows** – по желанию.

### Горячие клавиши по умолчанию

| Действие | Сочетание |
|---|---|
| Выделить область | `Ctrl+Shift+S` |
| Весь экран | `Ctrl+Shift+F` |

Пока открыт экран выделения: `Enter` — снять, `Esc` — отмена.

### Скачать

Скачай последний `ABNCapture.exe` на странице [Releases](https://github.com/abn-bg-online/ABN-Capture/releases/latest). Это автономная сборка, устанавливать .NET не нужно.

> Исполняемый файл пока не подписан, поэтому Windows SmartScreen может показать предупреждение «Windows защитила ваш компьютер». Нажми **Подробнее → Выполнить в любом случае**.

### Требования

- Windows 10 или Windows 11 (x64)
- Для Windows 10 **N / KN** для записи нужен *Media Feature Pack*

### Где всё хранится

- Снимки: `Pictures\ABN Capture` (меняется в настройках)
- Настройки: `%APPDATA%\ABNCapture\settings.json`

### Сборка из исходников

Нужен [.NET 10 SDK](https://dotnet.microsoft.com/download).

```powershell
git clone https://github.com/<your-username>/ABN-Capture.git
cd ABN-Capture
dotnet run -p:Platform=x64
```

Готовая сборка в одном файле:

```powershell
dotnet publish -c Release -p:Platform=x64 -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:IncludeAllContentForSelfExtract=true
```

Результат лежит в `bin\x64\Release\net10.0-windows\win-x64\publish\`.

### Структура проекта

```
Assets/      Значок приложения
Models/      AppSettings
Services/    Работа с горячими клавишами
Themes/      Общие стили WPF
Views/       Окна: главное, настройки, экран выделения, панель
```

### Использовано

- [WPF](https://learn.microsoft.com/dotnet/desktop/wpf/) на .NET 10
- [ScreenRecorderLib](https://github.com/sskodje/ScreenRecorderLib) – запись экрана (MIT)
- [H.NotifyIcon](https://github.com/HavenDV/H.NotifyIcon) – значок в системном трее (MIT)

### Вклад в проект

Issues и pull request'ы приветствуются. Для крупных изменений сначала открой issue, чтобы обсудить, что ты хочешь изменить.

</details>
