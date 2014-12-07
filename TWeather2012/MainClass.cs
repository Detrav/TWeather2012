using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;

namespace TWeather2012
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainClass : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        TSnowFlakes snow;
        //SpriteBatch spriteBatch;
        private Margins marg;
        TBorders borders;
        RECT[] rects;
        System.Windows.Forms.NotifyIcon NI;
        ContextMenuStrip cms;

        public struct Margins
        {
            public int Left, Right, Top, Bottom;
        }



        [DllImport("user32.dll", SetLastError = true)]
        private static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        [DllImport("dwmapi.dll")]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMargins);
        [DllImport("user32.dll")]
        public static extern void SetWindowPos(uint Hwnd, int Level, int X, int Y, int W, int H, uint Flags);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);


        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int WS_EX_TRANSPARENT = 0x20;
        public const int LWA_ALPHA = 0x2;
        public const int LWA_COLORKEY = 0x1;
        public static long WS_CHILD = 0x40000000; //child window
        public static long WS_BORDER = 0x00800000L; //window with border
        public static long WS_DLGFRAME = 0x00400000; //window with double border but no title
        public static long WS_CAPTION = WS_BORDER | WS_DLGFRAME; //window with a title bar 
        public static long WS_SYSMENU = 0x00080000; //window menu
        public static long WS_THICKFRAME = 0x00040000L;
        public static long WS_MAXIMIZE = 0x01000000;
        public static long WS_POPUP = 0x80000000L;
        public static long WS_MAXIMIZEBOX = 0x00010000;
        public static long WS_MINIMIZE = 0x20000000;
        public static long WS_MINIMIZEBOX = 0x00020000;
        public static long WS_EX_APPWINDOW = 0x00040000, WS_EX_TOOLWINDOW = 0x00000080;



        public MainClass()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //snow = new TSnowFlakes(graphics, new Rectangle(0, 0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height));
            snow = new TSnowFlakes(graphics, new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height));
            borders = new TBorders(500, 1f);
            rects = new RECT[100];
            NI = new System.Windows.Forms.NotifyIcon();
            cms = new ContextMenuStrip();
            cms.Items.Add("Exit");
            cms.Items[0].Click += MainClass_Click;
        }

        void MainClass_Click(object sender, EventArgs e)
        {
            this.Exit();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            //spriteBatch = new SpriteBatch(GraphicsDevice);

            marg = new Margins();
            marg.Left = 0;
            marg.Top = 0;
            //marg.Right = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //marg.Bottom = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            marg.Right = Screen.PrimaryScreen.Bounds.Width;
            marg.Bottom = Screen.PrimaryScreen.Bounds.Height;
            graphics.PreferredBackBufferHeight = Screen.PrimaryScreen.Bounds.Height;
            graphics.PreferredBackBufferWidth = Screen.PrimaryScreen.Bounds.Width;
            graphics.ApplyChanges();
            SetWindowLong(this.Window.Handle, GWL_STYLE, (IntPtr)(unchecked((int)0x80000000) | WS_BORDER | WS_SYSMENU));
            SetWindowLong(this.Window.Handle, GWL_EXSTYLE, (IntPtr)(((GetWindowLong(this.Window.Handle, GWL_EXSTYLE) ^ WS_EX_LAYERED ^ WS_EX_TRANSPARENT) | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW));
            SetWindowPos((uint)this.Window.Handle, -1, 0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 0);
            SetLayeredWindowAttributes(this.Window.Handle, 0, 255, LWA_ALPHA);
            DwmExtendFrameIntoClientArea(this.Window.Handle, ref marg);

            snow.LoadContent(Content);
            // TODO: use this.Content to load your game content here

            /*
            MemoryStream ms = new MemoryStream();
            Texture2D temp = Content.Load<Texture2D>("snowflake");
            temp.SaveAsPng(ms, temp.Width, temp.Height);
            ms.Seek(0, SeekOrigin.Begin);
            NI.Icon = System.Drawing.Icon.FromHandle((new System.Drawing.Bitmap(System.Drawing.Bitmap.FromStream(ms))).GetHicon());
            ms.Close();
            NI.Visible = true;
            NI.MouseClick += NI_MouseClick;
             Версия 2012 года, к сожалению с MonoGame не работает
             */
            System.Drawing.Bitmap tmp = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile(@"Content/snowflake.png");
            NI.Icon = System.Drawing.Icon.FromHandle(tmp.GetHicon());
            NI.Visible = true;
            NI.MouseClick += NI_MouseClick;
        }

        void NI_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            cms.Show(Cursor.Position);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            NI.Visible = false;
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            //    this.Exit();

            // TODO: Add your update logic here
            RECT temp;
            if (GetWindowRect(this.Window.Handle, out temp))
                if (temp.Top != 0 || temp.Left != 0)
                    SetWindowPos((uint)this.Window.Handle, -1, 0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 0);

            int size = borders.Update((float)gameTime.ElapsedGameTime.TotalSeconds, ref rects);
            snow.Tick();
            snow.Update((float)gameTime.ElapsedGameTime.TotalSeconds, ref rects, size);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(0, 0, 0, 0));

            // TODO: Add your drawing code here
            snow.Render();
            base.Draw(gameTime);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;        // x position of upper-left corner
        public int Top;         // y position of upper-left corner
        public int Right;       // x position of lower-right corner
        public int Bottom;      // y position of lower-right corner
    }
}
