# NeoNexus — Windows

This folder contains the Windows-specific source and build files for NeoNexus.

Layout (brief)
- native/                - Native C/C++ code (DLLs, platform code)
- win/NeoNexus.WinDemo/  - Managed Windows UI & services (C# WinForms demo)
- installer/             - Packaging / installer script templates (WiX, Inno)
- third_party/           - Pointers to native libs (libusb, libcurl)
- build/                 - Build helper scripts
- tests/                 - Native and managed test stubs

Quick build (Windows, dev):
1. Build native DLL:
   - mkdir Windows/native/build && cd Windows/native/build
   - cmake .. -G "Visual Studio 17 2022" -A x64
   - cmake --build . --config Release
   - Copy resulting NeoNexusNative.dll to Windows/win/NeoNexus.WinDemo/bin/Debug/net7.0-windows (or publish folder)

2. Build managed app:
   - cd Windows/win/NeoNexus.WinDemo
   - dotnet build
   - Ensure NeoNexusNative.dll is next to the .exe or in PATH
   - dotnet run

Notes:
- The native project exposes a small C ABI (NN_GetVersion, NN_ListPkgFiles, NN_FreeString).
- The managed app demonstrates P/Invoke to that native DLL and includes VitaDB/CBPS fetchers.
- Replace or extend pkg_parser.cpp and UsbHelpers with real implementations as you go.
- For production builds target both x86 and x64 and bundle the correct native binaries per-arch.

License: Follow root repo license (Apache-2.0).
