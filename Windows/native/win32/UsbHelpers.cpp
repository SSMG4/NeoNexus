#include "UsbHelpers.h"
#include <windows.h>
#include <setupapi.h>
#include <devguid.h>
#include <regstr.h>
#include <string>

#pragma comment(lib, "setupapi.lib")

static std::string g_lastError;

bool Usb_IsDeviceConnected() {
    HDEVINFO hDevInfo = SetupDiGetClassDevsA(NULL, "USB", NULL, DIGCF_PRESENT | DIGCF_ALLCLASSES);
    if (hDevInfo == INVALID_HANDLE_VALUE) {
        g_lastError = "SetupDiGetClassDevsA failed";
        return false;
    }

    SP_DEVINFO_DATA DeviceInfoData;
    DeviceInfoData.cbSize = sizeof(SP_DEVINFO_DATA);
    bool found = false;
    for (DWORD i = 0; SetupDiEnumDeviceInfo(hDevInfo, i, &DeviceInfoData); ++i) {
        CHAR buf[1024];
        if (SetupDiGetDeviceRegistryPropertyA(hDevInfo, &DeviceInfoData, SPDRP_DEVICEDESC, NULL, (PBYTE)buf, sizeof(buf), NULL)) {
            std::string desc(buf);
            // crude heuristic: treat anything with "USB" or "VIA" in description as connected
            for (auto &c : desc) c = (char)toupper(c);
            if (desc.find("USB") != std::string::npos) {
                found = true;
                break;
            }
        }
    }

    SetupDiDestroyDeviceInfoList(hDevInfo);
    if (!found) g_lastError = "No USB device found (checked device descriptions)";
    return found;
}

const char* Usb_GetLastError() {
    if (g_lastError.empty()) return "No error";
    return g_lastError.c_str();
}
