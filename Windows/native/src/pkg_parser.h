#pragma once
#include <vector>
#include <string>

// Very small PKG parser API stub.
// Replace with real parsing code (read PKG headers, list contents, metadata, etc.)

struct PkgEntry {
    std::string path;
    uint64_t size;
};

std::vector<PkgEntry> ParsePkgFile(const char* path);
