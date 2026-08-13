# GeistStudio

A simple **GeistScript Code Editor for Windows**.

![Platform](https://img.shields.io/badge/platform-Windows-0078D6?logo=windows&logoColor=white)
![Language](https://img.shields.io/badge/language-C%23-239120?logo=c-sharp&logoColor=white)
[![License: GPL v3](https://img.shields.io/badge/License-GPL%20v3-yellow.svg)](https://www.gnu.org/licenses/gpl-3.0)
![Status](https://img.shields.io/badge/status-active-brightgreen)

## About

**GeistStudio** is a lightweight Windows Forms application for writing and running **GeistScript** code. The goal is a straightforward, self-built editor with the essential features needed for everyday work with GeistScript — including an integrated terminal and customizable settings.

## Features

- Code editor made for my own programming language GeistScript
- Integrated terminal for running code directly
- Customizable settings (`Settings`)
- Backend module for processing/executing GeistScript
- Custom menu rendering (`GeistStudioMenuRenderer`)
- All Versions window (`Information`)

## Project Structure

```
GeistStudio/
├── Backend/                      # Backend logic for GeistScript
├── Properties/                   # Project settings (assembly info etc.)
├── Form1.cs / Form1.Designer.cs  # Main editor window
├── Terminal.cs / .Designer.cs    # Integrated terminal
├── Information.cs / .Designer.cs # All Versions Window
├── Settings.cs / .Designer.cs    # Settings dialog
├── GeistStudioMenuRenderer.cs    # Custom rendering for the menu
├── UtilFunctions.cs              # Helper functions
├── GeistStudioData.json          # Configuration/data file
├── Program.cs                    # Application entry point
├── GeistStudio.csproj            # Project file
└── GeistStudio.slnx              # Solution file
```

## Requirements

- Windows operating system
- [.NET](https://dotnet.microsoft.com/) (V 4.7.2)
- Visual Studio 2022 or newer (recommended, for `.slnx` support)

## Installation & Running

1. Clone the repository:
   ```bash
   git clone https://github.com/Geistvk/GeistStudio.git
   ```
2. Navigate into the folder:
   ```bash
   cd GeistStudio
   ```
3. Open the solution in Visual Studio:
   ```
   GeistStudio.slnx
   ```
4. Build and run the project (`F5` or "Start" in Visual Studio).

Alternatively, using the .NET CLI:

```bash
dotnet build
dotnet run --project GeistStudio.csproj
```

Or the Programm can be build using `g++`:
```bash
git clone https://github.com/Geistvk/GeistStudio.git
cd GeistStudio
g++ GeistStudio.cpp -o GeistStudio.exe
```

## Usage

1. Launch GeistStudio.
2. Create a new GeistScript file or open an existing one.
3. Write your code in the editor.
4. Use the integrated terminal to run/test the code.
5. Adjust editor settings via the Settings menu if needed.

## Contributing

Contributions are welcome!

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/my-feature`)
3. Commit your changes (`git commit -m "Add: my feature"`)
4. Push the branch (`git push origin feature/my-feature`)
5. Open a Pull Request

## License

This project is licensed under the [GNU General Public License v3.0](LICENSE).

## Author

Developed by [**Geistvk**](https://github.com/Geistvk)
