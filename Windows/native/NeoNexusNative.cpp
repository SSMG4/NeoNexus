#include "NeoNexusNative.h"
#include "src/pkg_parser.h"
#include <sstream>
#include <string>
#include <windows.h>
#include <combaseapi.h>
#include <vector>

// Note: removed nlohmann/json.hpp dependency so CI doesn't require that external lib.
// We build JSON manually in this file to avoid pulling native dependencies.

const char* NN_GetVersion() {
    return "NeoNexus.Native v0.2-windows";
}

char* NN_ListPkgFiles(const char* pkgPath) {
    if (!pkgPath) {
        const char* s = "[]";
        char* out = (char*)CoTaskMemAlloc(3);
        memcpy(out, s, 3);
        out[2] = '\0';
        return out;
    }

    std::vector<PkgEntry> entries = ParsePkgFile(pkgPath);

    // Build JSON manually (array of objects)
    std::ostringstream ss;
    ss << "[";
    bool first = true;
    for (const auto &e : entries) {
        if (!first) ss << ",";
        ss << "{";
        // naive JSON escaping for path; replace backslash and quote
        std::string p = e.path;
        std::string escaped;
        escaped.reserve(p.size());
        for (char c : p) {
            if (c == '\\') escaped += "\\\\";
            else if (c == '"') escaped += "\\\"";
            else escaped += c;
        }
        ss << "\"path\":\"" << escaped << "\",\"size\":" << e.size;
        ss << "}";
        first = false;
    }
    ss << "]";

    std::string s = ss.str();
    char* out = (char*)CoTaskMemAlloc(s.size() + 1);
    if (!out) return nullptr;
    memcpy(out, s.c_str(), s.size());
    out[s.size()] = '\0';
    return out;
}

void NN_FreeString(char* s) {
    if (s) CoTaskMemFree(s);
}
