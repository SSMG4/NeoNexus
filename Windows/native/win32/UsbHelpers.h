#pragma once

#include <stdbool.h>

#ifdef __cplusplus
extern "C" {
#endif

// Returns true if any USB device is detected (best-effort).
bool Usb_IsDeviceConnected();

// Returns a textual last-error (static string).
const char* Usb_GetLastError();

#ifdef __cplusplus
}
#endif
