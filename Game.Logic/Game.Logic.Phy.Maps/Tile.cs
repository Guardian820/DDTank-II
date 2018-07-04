using System;
using System.Drawing;
using System.IO;

namespace Game.Logic.Phy.Maps
{
    public class Tile
    {
        private byte[] _data;
        private int _width;
        private int _height;
        private Rectangle _rect;
        private int _bw;
        private int _bh;
        private bool _digable;

        public Rectangle Bound
        {
            get
            {
                return this._rect;
            }
        }

        public byte[] Data
        {
            get
            {
                return this._data;
            }
        }

        public int Width
        {
            get
            {
                return this._width;
            }
        }

        public int Height
        {
            get
            {
                return this._height;
            }
        }

        public Tile(byte[] data, int width, int height, bool digable)
        {
            this._data = data;
            this._width = width;
            this._height = height;
            this._digable = digable;
            this._bw = this._width / 8 + 1;
            this._bh = this._height;
            this._rect = new Rectangle(0, 0, this._width, this._height);
            GC.AddMemoryPressure((long)data.Length);
        }

        public Tile(Bitmap bitmap, bool digable)
        {
            this._width = bitmap.Width;
            this._height = bitmap.Height;
            this._bw = this._width / 8 + 1;
            this._bh = this._height;
            this._data = new byte[this._bw * this._bh];
            this._digable = digable;
            for (int y = 0; y < bitmap.Height; ++y)
            {
                for (int x = 0; x < bitmap.Width; ++x)
                {
                    byte num = (int)bitmap.GetPixel(x, y).A <= 100 ? (byte)0 : (byte)1;
                    this._data[y * this._bw + x / 8] |= (byte)((uint)num << 7 - x % 8);
                }
            }
            this._rect = new Rectangle(0, 0, this._width, this._height);
            GC.AddMemoryPressure((long)this._data.Length);
        }

        public Tile(string file, bool digable)
        {
            BinaryReader binaryReader = new BinaryReader((Stream)File.Open(file, FileMode.Open));
            this._width = binaryReader.ReadInt32();
            this._height = binaryReader.ReadInt32();
            this._bw = this._width / 8 + 1;
            this._bh = this._height;
            this._data = binaryReader.ReadBytes(this._bw * this._bh);
            this._digable = digable;
            this._rect = new Rectangle(0, 0, this._width, this._height);
            binaryReader.Close();
            GC.AddMemoryPressure((long)this._data.Length);
        }

        public void Dig(int cx, int cy, Tile surface, Tile border)
        {
            if (!this._digable || surface == null)
                return;
            this.Remove(cx - surface.Width / 2, cy - surface.Height / 2, surface);
            if (border == null)
                return;
            this.Add(cx - border.Width / 2, cy - border.Height / 2, surface);
        }

        protected void Add(int x, int y, Tile tile)
        {
        }

        protected void Remove(int x, int y, Tile tile)
        {
            byte[] numArray = tile._data;
            Rectangle bound = tile.Bound;
            bound.Offset(x, y);
            bound.Intersect(this._rect);
            if (bound.Width == 0 || bound.Height == 0)
                return;
            bound.Offset(-x, -y);
            int num1 = bound.X / 8;
            int num2 = (bound.X + x) / 8;
            int y1 = bound.Y;
            int num3 = bound.Width / 8 + 1;
            int height = bound.Height;
            if (bound.X == 0)
            {
                if (num3 + num2 < this._bw)
                {
                    int num4 = num3 + 1;
                    num3 = num4 > tile._bw ? tile._bw : num4;
                }
                int num5 = (bound.X + x) % 8;
                int num6 = 0;
                for (int index1 = 0; index1 < height; ++index1)
                {
                    num6 = 0;
                    int num4 = 0;
                    for (int index2 = 0; index2 < num3; ++index2)
                    {
                        int index3 = (index1 + y + y1) * this._bw + index2 + num2;
                        int index4 = (index1 + y1) * tile._bw + index2 + num1;
                        int num7 = (int)numArray[index4];
                        int num8 = num7 >> num5;
                        int num9 = (int)this._data[index3];
                        int num10 = num9 & ~(num9 & num8);
                        if (num4 != 0)
                            num10 &= ~(num10 & num4);
                        this._data[index3] = (byte)num10;
                        num4 = num7 << 8 - num5;
                    }
                }
            }
            else
            {
                int num4 = bound.X % 8;
                int num5 = 0;
                int num6 = 0;
                for (int index1 = 0; index1 < height; ++index1)
                {
                    num5 = 0;
                    num6 = 0;
                    for (int index2 = 0; index2 < num3; ++index2)
                    {
                        int index3 = (index1 + y + y1) * this._bw + index2 + num2;
                        int index4 = (index1 + y1) * tile._bw + index2 + num1;
                        int num7 = (int)numArray[index4] << num4;
                        int num8 = index2 >= num3 - 1 ? 0 : (int)numArray[index4 + 1] >> 8 - num4;
                        int num9 = (int)this._data[index3];
                        int num10 = num9 & ~(num9 & num7);
                        if (num8 != 0)
                            num10 &= ~(num10 & num8);
                        this._data[index3] = (byte)num10;
                    }
                }
            }
        }

        public bool IsEmpty(int x, int y)
        {
            if (x < 0 || x >= this._width || (y < 0 || y >= this._height))
                return true;
            byte num = (byte)(1 << 7 - x % 8);
            return ((int)this._data[y * this._bw + x / 8] & (int)num) == 0;
        }

        public bool IsYLineEmtpy(int x, int y, int h)
        {
            if (x < 0 || x >= this._width)
                return true;
            y = y < 0 ? 0 : y;
            h = y + h > this._height ? this._height - y : h;
            for (int index = 0; index < h; ++index)
            {
                if (!this.IsEmpty(x, y + index))
                    return false;
            }
            return true;
        }

        public bool IsRectangleEmptyQuick(Rectangle rect)
        {
            rect.Intersect(this._rect);
            return this.IsEmpty(rect.Right, rect.Bottom) && this.IsEmpty(rect.Left, rect.Bottom) && (this.IsEmpty(rect.Right, rect.Top) && this.IsEmpty(rect.Left, rect.Top));
        }

        public Point FindNotEmptyPoint(int x, int y, int h)
        {
            if (x < 0 || x >= this._width)
                return new Point(-1, -1);
            y = y < 0 ? 0 : y;
            h = y + h > this._height ? this._height - y : h;
            for (int index = 0; index < h; ++index)
            {
                if (!this.IsEmpty(x, y + index))
                    return new Point(x, y + index);
            }
            return new Point(-1, -1);
        }

        public Bitmap ToBitmap()
        {
            Bitmap bitmap = new Bitmap(this._width, this._height);
            for (int y = 0; y < this._height; ++y)
            {
                for (int x = 0; x < this._width; ++x)
                {
                    if (this.IsEmpty(x, y))
                        bitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));
                    else
                        bitmap.SetPixel(x, y, Color.FromArgb((int)byte.MaxValue, 0, 0, 0));
                }
            }
            return bitmap;
        }

        public Tile Clone()
        {
            return new Tile(this._data.Clone() as byte[], this._width, this._height, this._digable);
        }
    }
}
