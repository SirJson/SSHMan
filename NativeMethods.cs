using System;
using System.Runtime.InteropServices;

namespace SSHMan {
    internal static class NativeMethods {
        internal class Win32ShowCmd {
            public const int ForceMinimize = 11;
            public const int Hide = 0;
            public const int Maximize = 3;
            public const int Minimize = 6;
            public const int Restore = 9;
            public const int Show = 5;
            public const int ShowDefault = 10;
            public const int ShowMaximized = 3;
            public const int ShowMinimized = 2;
            public const int ShoMinNoActive = 7;
            public const int ShowNa = 8;
            public const int ShowNoActivate = 4;
            public const int ShowNormal = 1;
        }

        [DllImport ("kernel32.dll")]
        [
            return :MarshalAs (UnmanagedType.Bool)
        ]
        public static extern bool AllocConsole ();

        [DllImport ("user32.DLL")]
        public static extern bool SetForegroundWindow (IntPtr hWnd);

        [DllImport ("user32.dll")]
        public static extern bool ShowWindow (IntPtr handle, int nCmdShow);

        [DllImport ("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow (string lpClassName, string lpWindowName);

    }
}