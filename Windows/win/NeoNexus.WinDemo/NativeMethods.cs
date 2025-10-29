using System;
using System.Runtime.InteropServices;

internal static class NativeMethods
{
    [DllImport("NeoNexusNative.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr NN_GetVersion();

    [DllImport("NeoNexusNative.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr NN_ListPkgFiles(string pkgPath);

    [DllImport("NeoNexusNative.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void NN_FreeString(IntPtr s);

    public static string GetVersion()
    {
        var p = NN_GetVersion();
        return Marshal.PtrToStringAnsi(p) ?? "<null>";
    }

    public static string ListPkgFiles(string pkgPath)
    {
        var p = NN_ListPkgFiles(pkgPath);
        if (p == IntPtr.Zero) return "null";
        try
        {
            return Marshal.PtrToStringAnsi(p) ?? "null";
        }
        finally
        {
            NN_FreeString(p);
        }
    }
}
