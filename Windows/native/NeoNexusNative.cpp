#include "NeoNexusNative.h"
#include <windows.h>
#include <combaseapi.h>
#include <string>
#include <vector>
#include <sstream>

// Demo native library for NeoNexus on Windows.
// Real implementation should parse PKG files, talk to libusb/WinUSB, etc.

const char* NN_GetVersion() {
    return "NeoNexus.Native v0.1-windows";
}

char* NN_ListPkgFiles(const char* pkgPath) {
    // Demo: return static JSON regardless of input
    std::stringstream ss;
    ss << "[\"eboot.bin\",\"sce_sys/icon0.png\",\"README.txt\"]";
    std::string s = ss.str();
    size_t len = s.size();
    char* out = (char*)CoTaskMemAlloc(len + 1);
    if (!out) return nullptr;
    memcpy(out, s.c_str(), len);
    out[len] = '\0';
    return out;
}

void NN_FreeString(char* s) {
    if (s) CoTaskMemFree(s);
}
