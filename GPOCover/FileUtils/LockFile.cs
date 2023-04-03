using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPOCover.FileUtils;

internal class LockFile
{
}

/**
 * Code from:
 * https://social.msdn.microsoft.com/Forums/vstudio/en-US/7217a8d3-d36d-43c9-ad4f-ad638a9ac1de/lockfileex-in-c-application?forum=csharpgeneral
 */

public struct OVERLAPPED
{

    /// ULONG_PTR->unsigned int 
    public uint Internal;

    /// ULONG_PTR->unsigned int 
    public uint InternalHigh;

    /// Anonymous_7416d31a_1ce9_4e50_b1e1_0f2ad25c0196 
    public Anonymous_7416d31a_1ce9_4e50_b1e1_0f2ad25c0196 Union1;

    /// HANDLE->void* 
    public System.IntPtr hEvent;
}

[System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit)]
public struct Anonymous_7416d31a_1ce9_4e50_b1e1_0f2ad25c0196
{

    /// Anonymous_ac6e4301_4438_458f_96dd_e86faeeca2a6 
    [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
    public Anonymous_ac6e4301_4438_458f_96dd_e86faeeca2a6 Struct1;

    /// PVOID->void* 
    [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
    public System.IntPtr Pointer;
}

[System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
public struct Anonymous_ac6e4301_4438_458f_96dd_e86faeeca2a6
{

    /// DWORD->unsigned int 
    public uint Offset;

    /// DWORD->unsigned int 
    public uint OffsetHigh;
}

public partial class NativeMethods
{

    /// Return Type: BOOL->int 
    ///hFile: HANDLE->void* 
    ///dwFlags: DWORD->unsigned int 
    ///dwReserved: DWORD->unsigned int 
    ///nNumberOfBytesToLockLow: DWORD->unsigned int 
    ///nNumberOfBytesToLockHigh: DWORD->unsigned int 
    ///lpOverlapped: LPOVERLAPPED->_OVERLAPPED* 
    [System.Runtime.InteropServices.DllImportAttribute("kernel32.dll", EntryPoint = "LockFileEx")]
    [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
    public static extern bool LockFileEx([System.Runtime.InteropServices.InAttribute()] System.IntPtr hFile,
        uint dwFlags,
        uint dwReserved,
        uint nNumberOfBytesToLockLow,
        uint nNumberOfBytesToLockHigh,
        ref OVERLAPPED lpOverlapped);

}
