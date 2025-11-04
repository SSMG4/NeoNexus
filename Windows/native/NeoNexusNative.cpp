#include "NeoNexusNative.h"
#include "src/pkg_parser.h"
#include <sstream>
#include <string>
#include <windows.h>
#include <combaseapi.h>
#include <vector>
#include <nlohmann/json.hpp> // optional: if you don't have this, we fallback to manual JSON

// If you have nlohmann/json available via vcpkg, you can enable the #define below.
// Otherwise the code falls back to a manual JSON output.
#if 0
using json = nlohmann::json;
#endif

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
        // naive JSON escaping for path; replace " with \"
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
