using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWeather2012
{
    class TSnowFlakes
    {
        Particle[] particles;
        Particle[] sparts;
        Random rand;
        SpriteBatch sb;
        GraphicsDeviceManager graphics;
        Texture2D snowflake;
        Vector2 sc;
        Rectangle window;
        Rectangle[] recters;
        int size;

        Texture2D ClearedTexture;
        Color TransparentColor;


        public TSnowFlakes(GraphicsDeviceManager _graphics, Rectangle _wndsize, int n = 1000)
        {
            graphics = _graphics;
            particles = new Particle[n];
            sparts = new Particle[n];
            for (int i = 0; i < n; i++)
                particles[i].Enabled = 0;
            rand = new Random();
            window = _wndsize;
            size = 0;
            TransparentColor = new Color(0, 0, 0, 0);
            recters = new Rectangle[100];
        }

        public void LoadContent(ContentManager Content, string _snowflake = "snowflake")
        {
            sb = new SpriteBatch(graphics.GraphicsDevice);
            snowflake = Content.Load<Texture2D>(_snowflake);
            sc = new Vector2((float)(snowflake.Width) / 2f, (float)(snowflake.Height) / 2f);
            ClearedTexture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            ClearedTexture.SetData(new Color[] { Color.Black });
        }

        public void Clear()
        {
            for (int i = 0; i < particles.Count(); i++)
                particles[i].Enabled = 0;
        }

        public void Tick(int n = 1)
        {
            for (int i = 0, j = 0; i < particles.Count() && j < n; i++)
                if (particles[i].Enabled == 0)
                {
                    particles[i].pos.X = rand.Next(window.Width);
                    particles[i].pos.Y = -sc.Y;
                    particles[i].spd.X = (float)(rand.Next(500) - 250) / 500f;
                    particles[i].spd.Y = (float)(rand.Next(250) + 250) / 500f;
                    particles[i].scale = Math.Abs(particles[i].spd.Y) / 10;
                    particles[i].Enabled = 1;
                    particles[i].c = Color.White;
                    j++;
                }
        }

        public void Update(float dt, ref RECT[] rects, int _size = 0)
        {
            size = _size;
            for (int i = 0; i < particles.Count(); i++)
            {
                if (particles[i].Enabled == 1)
                {
                    particles[i].pos += particles[i].spd;//проверка на края
                    if (particles[i].pos.Y > window.Height + sc.Y) { particles[i].Enabled = 0; continue; }
                    if (particles[i].pos.X < -sc.X) { particles[i].pos.X = window.Width + sc.X * particles[i].scale; continue; }
                    if (particles[i].pos.X > window.Width + sc.X) { particles[i].pos.X = -sc.X * particles[i].scale; continue; }
                    for (int j = 0; j < size; j++)
                    {
                        if (particles[i].pos.Y < rects[j].Top) continue;
                        if (particles[i].pos.Y > rects[j].Bottom) continue;
                        if (particles[i].pos.X < rects[j].Left) continue;
                        if (particles[i].pos.X > rects[j].Right) continue;
                        if (Add(particles[i])) { particles[i].Enabled = 0; }
                        else particles[i].Enabled = 2;
                        break;
                    }
                }
                if (particles[i].Enabled == 2)
                {
                    particles[i].c.A = (byte)((double)particles[i].c.A * 0.9);
                    if (particles[i].c.A < 10)
                        particles[i].Enabled = 0;
                }
            }

            for (int j = 0; j < size; j++)
            {
                recters[j].Y = rects[j].Top;
                recters[j].X = rects[j].Left;
                recters[j].Width = rects[j].Right - rects[j].Left;
                recters[j].Height = rects[j].Bottom - rects[j].Top;
            }
            Vector2 temp;
            for (int i = 0; i < sparts.Count(); i++)
            {
                if (sparts[i].Enabled == 1)
                {
                    temp = sparts[i].pos + sparts[i].spd;
                    sparts[i].time -= dt;
                    if (sparts[i].time <= 0) { sparts[i].Enabled = 0; continue; }
                    bool flag = false;
                    int num = -1;
                    for (int j = 0; j < size; j++)
                    {
                        if (sparts[i].pos.Y < rects[j].Top) continue;
                        if (sparts[i].pos.Y > rects[j].Bottom) continue;
                        if (sparts[i].pos.X < rects[j].Left) continue;
                        if (sparts[i].pos.X > rects[j].Right) continue;
                        num = j;
                        break;
                    }
                    if (num < 0)
                        for (int j = 0; j < size; j++)
                        {
                            if (sparts[i].pos.X < rects[j].Left) continue;
                            if (sparts[i].pos.X > rects[j].Right) continue;
                            if (sparts[i].pos.Y < rects[j].Top)
                            {
                                if (temp.Y > rects[j].Top)
                                {
                                    sparts[i].pos.Y = recters[j].Top - 0.1f;
                                    flag = true;
                                    break;
                                }
                            }
                        }
                    else
                    {
                        for (int j = num; j >= 0; j--)
                        {
                            if (sparts[i].pos.X < rects[j].Left) continue;
                            if (sparts[i].pos.X > rects[j].Right) continue;
                            if (sparts[i].pos.Y < rects[j].Top)
                            {
                                if (temp.Y > rects[j].Top)
                                {
                                    sparts[i].pos.Y = recters[j].Top - 0.1f;
                                    flag = true;
                                    break;
                                }
                            }
                            else
                                if (sparts[i].pos.Y < rects[j].Bottom)
                                    if (temp.Y > rects[j].Bottom)
                                    {
                                        sparts[i].pos.Y = recters[j].Bottom - 0.1f;
                                        flag = true;
                                        break;
                                    }
                        }
                    }
                    if (!flag)
                    {
                        sparts[i].pos += sparts[i].spd;
                        sparts[i].spd *= 1.0098f;
                        if (sparts[i].pos.Y > window.Height + sc.Y) { sparts[i].Enabled = 0; continue; }
                    }
                }
            }
        }

        public void Render()
        {
            sb.Begin();
            foreach (var part in particles)
                if (part.Enabled > 0)
                    sb.Draw(snowflake, part.pos - sc * part.scale, null, part.c, 0f, Vector2.Zero, part.scale, SpriteEffects.None, 0f);
            //     sb.Draw(st, part.pos, null, Color.White, 0f, Vector2.Zero, part.scale, SpriteEffects.None, 0f);
            //sb.Draw(dummyTexture, testrect, Color.Black);
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
            for (int j = 0; j < size; j++)
                sb.Draw(ClearedTexture, recters[j], TransparentColor);
            sb.End();
            sb.Begin();
            foreach (var part in sparts)
                if (part.Enabled > 0)
                    sb.Draw(snowflake, part.pos - sc * part.scale, null, part.c, 0f, Vector2.Zero, part.scale, SpriteEffects.None, 0f);
            //     sb.Draw(st, part.pos, null, Color.White, 0f, Vector2.Zero, part.scale, SpriteEffects.None, 0f);
            //sb.Draw(dummyTexture, testrect, Color.Black);
            sb.End();
        }

        private bool Add(Particle p)
        {
            for (int i = 0, j = 0; i < sparts.Count(); i++)
                if (sparts[i].Enabled == 0)
                {
                    sparts[i].pos.X = p.pos.X - p.spd.X * 1.1f;
                    sparts[i].pos.Y = p.pos.Y - p.spd.Y * 1.1f;
                    sparts[i].scale = p.scale;
                    sparts[i].c = Color.White;
                    sparts[i].Enabled = 1;
                    sparts[i].spd.X = 0;
                    sparts[i].spd.Y = 1f + 0.5f * p.scale;
                    sparts[i].time = 10f;
                    return true;
                }
            return false;
        }

    }

    struct Particle
    {
        public byte Enabled;
        public Vector2 pos;
        public Vector2 spd;
        public float scale;
        public Color c;
        public float time;
    }
}
