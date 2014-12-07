using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TWeather2012
{
    class TBorders
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetTopWindow(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetWindow(IntPtr hWnd, GetWindow_Cmd uCmd);
        enum GetWindow_Cmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        class WindowBorder
        {
            public bool Enable;
            public IntPtr hwnd;
            public WINDOWPLACEMENT place;
            public long style;
            public WindowBorder()
            {
                Enable = false;
                hwnd = IntPtr.Zero;
                place = new WINDOWPLACEMENT();
                style = 0;
            }
        }
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public RECT rcNormalPosition;
        }
        const UInt32 SW_HIDE = 0;
        const UInt32 SW_SHOWNORMAL = 1;
        const UInt32 SW_NORMAL = 1;
        const UInt32 SW_SHOWMINIMIZED = 2;
        const UInt32 SW_SHOWMAXIMIZED = 3;
        const UInt32 SW_MAXIMIZE = 3;
        const UInt32 SW_SHOWNOACTIVATE = 4;
        const UInt32 SW_SHOW = 5;
        const UInt32 SW_MINIMIZE = 6;
        const UInt32 SW_SHOWMINNOACTIVE = 7;
        const UInt32 SW_SHOWNA = 8;
        const UInt32 SW_RESTORE = 9;

        public static class WindowStyles
        {

            public static readonly long

            WS_BORDER = 0x00800000,

            WS_CAPTION = 0x00C00000,

            WS_CHILD = 0x40000000,

            WS_CHILDWINDOW = 0x40000000,

            WS_CLIPCHILDREN = 0x02000000,

            WS_CLIPSIBLINGS = 0x04000000,

            WS_DISABLED = 0x08000000,

            WS_DLGFRAME = 0x00400000,

            WS_GROUP = 0x00020000,

            WS_HSCROLL = 0x00100000,

            WS_ICONIC = 0x20000000,

            WS_MAXIMIZE = 0x01000000,

            WS_MAXIMIZEBOX = 0x00010000,

            WS_MINIMIZE = 0x20000000,

            WS_MINIMIZEBOX = 0x00020000,

            WS_OVERLAPPED = 0x00000000,

            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,

            WS_POPUP = unchecked((int)0x80000000),

            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,

            WS_SIZEBOX = 0x00040000,

            WS_SYSMENU = 0x00080000,

            WS_TABSTOP = 0x00010000,

            WS_THICKFRAME = 0x00040000,

            WS_TILED = 0x00000000,

            WS_TILEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,

            WS_VISIBLE = 0x10000000,

            WS_VSCROLL = 0x00200000;

        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern long GetWindowLong(IntPtr hWnd, int nIndex);

        WindowBorder[] Borders;
        int max_size;
        int size; int pos;
        float timer1;
        float max_timer1;

        public TBorders(int _max_size, float time)
        {
            max_size = _max_size;
            Borders = new WindowBorder[max_size];
            for (int i = 0; i < Borders.Count(); i++)
            {
                Borders[i] = new WindowBorder();
            }
            max_timer1 = time;
            timer1 = 0;
        }

        public int Update(float gameTime, ref RECT[] rects)
        {

            timer1 -= gameTime;
            if (timer1 <= 0)
                Force_Update();
            return Force_Constantly(ref rects);
        }


        private int Force_Constantly(ref RECT[] rects)
        {
            int j = 0;
            for (int i = 0; i < size; i++)
            {
                Borders[i].place.length = Marshal.SizeOf(Borders[i].place);
                if (!GetWindowPlacement(Borders[i].hwnd, ref Borders[i].place))
                    continue;
                if (Borders[i].place.showCmd == 3)
                    return j;
                Borders[i].style = GetWindowLong(Borders[i].hwnd, -16);
                Borders[i].Enable = (((Borders[i].style) & (WindowStyles.WS_VISIBLE | WindowStyles.WS_SIZEBOX)) == (WindowStyles.WS_VISIBLE | WindowStyles.WS_SIZEBOX) && (Borders[i].place.showCmd == 1 || Borders[i].place.showCmd == 3));
                Borders[i].style = GetWindowLong(Borders[i].hwnd, -20);
                Borders[i].Enable = Borders[i].Enable && (((Borders[i].style) & (0x00000020L)) == (0));
                if (Borders[i].Enable)
                {
                    rects[j].Left = Borders[i].place.rcNormalPosition.Left;
                    rects[j].Top = Borders[i].place.rcNormalPosition.Top;
                    rects[j].Right = Borders[i].place.rcNormalPosition.Right;
                    rects[j].Bottom = Borders[i].place.rcNormalPosition.Bottom;
                    j++;
                }
            }
            return j;
        }

        private void Force_Update()
        {
            IntPtr hwnd = GetTopWindow(IntPtr.Zero);
            int i = 0;
            while (hwnd != IntPtr.Zero && i < max_size)
            {
                Borders[i].hwnd = hwnd;
                hwnd = GetWindow(hwnd, GetWindow_Cmd.GW_HWNDNEXT);
                i++;
            }
            size = i;
            timer1 = max_timer1;
        }
    }
}
