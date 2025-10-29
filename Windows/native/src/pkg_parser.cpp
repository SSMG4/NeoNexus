#include "pkg_parser.h"
#include <vector>
#include <string>

// Stubbed parser: returns a few demo entries.
std::vector<PkgEntry> ParsePkgFile(const char* path) {
    std::vector<PkgEntry> out;
    out.push_back({ "eboot.bin", 123456 });
    out.push_back({ "sce_sys/icon0.png", 2048 });
    out.push_back({ "README.txt", 512 });
    return out;
}
