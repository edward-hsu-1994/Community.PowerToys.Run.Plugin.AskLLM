using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Community.PowerToys.Run.Plugin.AskLLM;

public class ClipboardHelper
{
    [DllImport("user32.dll")] // Import OpenClipboard function from user32.dll
    public static extern bool OpenClipboard(IntPtr hWndNewOwner);

    [DllImport("user32.dll")] // Import CloseClipboard function from user32.dll
    public static extern bool CloseClipboard();

    [DllImport("user32.dll")] // Import EmptyClipboard function from user32.dll
    public static extern bool EmptyClipboard();

    [DllImport("user32.dll")] // Import GetClipboardData function from user32.dll
    public static extern IntPtr GetClipboardData(uint uFormat);

    [DllImport("user32.dll")] // Import SetClipboardData function from user32.dll
    public static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

    [DllImport("user32.dll")] // Import EnumClipboardFormats function from user32.dll
    public static extern uint EnumClipboardFormats(uint format);

    [DllImport("kernel32.dll")] // Import GlobalAlloc function from kernel32.dll
    public static extern IntPtr GlobalAlloc(uint uFlags, IntPtr dwBytes);

    [DllImport("kernel32.dll")] // Import GlobalLock function from kernel32.dll
    public static extern IntPtr GlobalLock(IntPtr hMem);

    [DllImport("kernel32.dll")] // Import GlobalUnlock function from kernel32.dll
    public static extern bool GlobalUnlock(IntPtr hMem);

    private const uint GMEM_MOVEABLE = 0x0002; // Constant for moveable memory

    public static Dictionary<uint, IntPtr> BackupClipboard()
    {
        var clipboardData = new Dictionary<uint, IntPtr>();

        if (OpenClipboard(IntPtr.Zero)) // Open the clipboard to access its data
        {
            try
            {
                uint format = 0;

                // Loop through all available clipboard formats
                while ((format = EnumClipboardFormats(format)) != 0)
                {
                    IntPtr hData = GetClipboardData(format); // Get handle to clipboard data in this format
                    if (hData != IntPtr.Zero)
                    {
                        IntPtr hCopy = CopyGlobalMemory(hData); // Make a copy of the global memory block
                        if (hCopy != IntPtr.Zero)
                        {
                            clipboardData[format] = hCopy; // Store the copied data in dictionary
                        }
                    }
                }
            }
            finally
            {
                CloseClipboard(); // Ensure to close the clipboard after processing
            }
        }

        return clipboardData; // Return the dictionary containing backed up clipboard data
    }

    public static void RestoreClipboard(Dictionary<uint, IntPtr> clipboardData)
    {
        if (OpenClipboard(IntPtr.Zero)) // Open the clipboard for setting data
        {
            try
            {
                EmptyClipboard(); // Clear any existing clipboard contents

                foreach (var kvp in clipboardData) // Iterate through backed up clipboard data
                {
                    SetClipboardData(kvp.Key, kvp.Value); // Restore each data item back to clipboard
                }
            }
            finally
            {
                CloseClipboard(); // Make sure to close the clipboard after restoring
            }
        }
    }

    public static IntPtr CopyGlobalMemory(IntPtr hMem)
    {
        IntPtr sourcePtr = GlobalLock(hMem); // Lock global memory block and get pointer to that memory
        if (sourcePtr == IntPtr.Zero) return IntPtr.Zero; // Return null pointer if locking fails

        try
        {
            // Read size of the memory block from the last 4 bytes of this memory block
            IntPtr size = Marshal.ReadIntPtr(hMem, -4);

            // Allocate new global memory block with same size as source data
            IntPtr hCopy = GlobalAlloc(GMEM_MOVEABLE, size);
            if (hCopy != IntPtr.Zero)
            {
                IntPtr destPtr = GlobalLock(hCopy); // Lock newly allocated memory and get pointer to it
                if (destPtr != IntPtr.Zero)
                {
                    try
                    {
                        // Copy byte-by-byte data from source to destination
                        for (long i = 0; i < (long)size; i++)
                        {
                            Marshal.WriteByte(destPtr, (int)i, Marshal.ReadByte(sourcePtr, (int)i));
                        }
                    }
                    finally
                    {
                        GlobalUnlock(hCopy); // Unlock destination memory after copying data
                    }
                }
            }

            return hCopy; // Return pointer to copied global memory
        }
        finally
        {
            GlobalUnlock(hMem); // Ensure source memory is unlocked finally, whatever happens
        }
    }
}