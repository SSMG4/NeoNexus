#pragma once

#ifdef __cplusplus
extern "C" {
#endif

// Returns a static string (do NOT free)
__declspec(dllexport) const char* NN_GetVersion();

// Returns an allocated C-string (CoTaskMemAlloc). Caller (managed) must free via NN_FreeString.
__declspec(dllexport) char* NN_ListPkgFiles(const char* pkgPath);

// Free string allocated by NN_ListPkgFiles
__declspec(dllexport) void NN_FreeString(char* s);

#ifdef __cplusplus
}
#endif
