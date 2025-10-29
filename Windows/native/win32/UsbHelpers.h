#pragma once

// Simple Win32 USB helper stubs.
// Replace with libusb or WinUSB implementations as needed.

bool Usb_IsDeviceConnected();
const char* Usb_GetLastError();
