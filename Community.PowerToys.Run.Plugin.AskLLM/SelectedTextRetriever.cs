using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace Community.PowerToys.Run.Plugin.AskLLM;

// Class to retrieve selected text from the current application.
public class SelectedTextRetriever
{
    // Method to get the currently selected text in the active window.
    public static string GetSelectedText(IntPtr hWm)
    {
        ClipboardHelper.ClearClipboard();
        Thread.Sleep(100); // Wait for a short period to ensure the Copy operation is completed.

        var currentFocusWindow = GetForegroundWindow();

        SetForegroundWindow(hWm);
        
        try
        {
            // Simulate Ctrl+C (Copy operation) to copy the selected text to the clipboard.
            SimulateCtrlC();

            Thread.Sleep(100); // Wait for a short period to ensure the Copy operation is completed.

            // Initialize an empty string variable to store the retrieved text.
            string selectedText = string.Empty;

            // Check if it's possible to open the clipboard.
            if (ClipboardHelper.OpenClipboard(IntPtr.Zero))
            {
                try
                {
                    // Get the handle of the data associated with CF_UNICODETEXT format (Unicode text).
                    IntPtr hData = ClipboardHelper.GetClipboardData(13); // CF_UNICODETEXT

                    // Check if there is valid data in the clipboard that can be retrieved.
                    if (hData != IntPtr.Zero)
                    {
                        // Lock the global memory object to access it for reading.
                        IntPtr pData = ClipboardHelper.GlobalLock(hData);

                        // Check if the memory lock was successful.
                        if (pData != IntPtr.Zero)
                        {
                            // Convert the pointer to a Unicode string and assign it to selectedText.
                            selectedText = Marshal.PtrToStringUni(pData);
                            // Unlock the global memory object since its contents have been retrieved successfully.
                            ClipboardHelper.GlobalUnlock(hData);
                        }
                    }
                }
                finally
                {
                    // Close the clipboard after all operations are complete.
                    ClipboardHelper.CloseClipboard();
                }
            }

            // Return the retrieved text, or an empty string if no valid text was found in the clipboard.
            return selectedText ?? string.Empty;
        }
        finally
        {            
            ClipboardHelper.ClearClipboard();
            SetForegroundWindow(currentFocusWindow);
        }
    }

    // Method to simulate pressing and releasing the Ctrl+C keyboard shortcut.
    private static void SimulateCtrlC()
    {
        const int VK_CONTROL = 0x11; // Virtual-Key code for Control key.
        const int VK_C = 0x43;       // Virtual-Key code for 'C' key.
        const uint KEYEVENTF_KEYUP = 0x0002; // Flag indicating the key-up event.

        // Generate and simulate keyboard events to press and release Ctrl+C:
        keybd_event((byte)VK_CONTROL, 0, 0, IntPtr.Zero); // Key down for Control
        keybd_event((byte)VK_C, 0, 0,IntPtr.Zero);      // Key down for 'C'
        
        // Simulate that user releases the keys: 
        keybd_event((byte)VK_C, 0, KEYEVENTF_KEYUP, IntPtr.Zero);    // Key up for 'C'  
        keybd_event((byte)VK_CONTROL, 0, KEYEVENTF_KEYUP, IntPtr.Zero); // Key up for Control
    }

    // Import the keybd_event function from user32.dll to simulate keyboard input events.
    [DllImport("user32.dll")]
    static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);
    
    
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();
    
    [DllImport("user32.dll")]
    static extern bool SetForegroundWindow(IntPtr hWnd);
}