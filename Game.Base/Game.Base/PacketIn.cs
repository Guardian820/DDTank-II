using System;
using System.Text;
using System.Threading;

namespace Game.Base
{
    public class PacketIn
    {
        public static int[] SEND_KEY = new int[8]
    {
      174,
      191,
      86,
      120,
      171,
      205,
      239,
      241
    };
        public volatile bool isSended = true;
        public volatile int m_sended = 0;
        public volatile int packetNum = 0;
        protected byte[] m_buffer;
        protected int m_length;
        protected int m_offset;

        public byte[] Buffer
        {
            get
            {
                return this.m_buffer;
            }
        }

        public int Length
        {
            get
            {
                return this.m_length;
            }
        }

        public int Offset
        {
            get
            {
                return this.m_offset;
            }
            set
            {
                this.m_offset = value;
            }
        }

        public int DataLeft
        {
            get
            {
                return this.m_length - this.m_offset;
            }
        }

        static PacketIn()
        {
        }

        public PacketIn(byte[] buf, int len)
        {
            this.m_buffer = buf;
            this.m_length = len;
            this.m_offset = 0;
        }

        public void Skip(int num)
        {
            this.m_offset += num;
        }

        public virtual bool ReadBoolean()
        {
            return (int)this.m_buffer[this.m_offset++] != 0;
        }

        public virtual byte ReadByte()
        {
            return this.m_buffer[this.m_offset++];
        }

        public virtual short ReadShort()
        {
            return Marshal.ConvertToInt16(this.ReadByte(), this.ReadByte());
        }

        public virtual short ReadShortLowEndian()
        {
            return Marshal.ConvertToInt16(this.ReadByte(), this.ReadByte());
        }

        public virtual int ReadInt()
        {
            return Marshal.ConvertToInt32(this.ReadByte(), this.ReadByte(), this.ReadByte(), this.ReadByte());
        }

        public virtual float ReadFloat()
        {
            byte[] numArray = new byte[4];
            for (int index = 0; index < numArray.Length; ++index)
                numArray[index] = this.ReadByte();
            return BitConverter.ToSingle(numArray, 0);
        }

        public virtual double ReadDouble()
        {
            byte[] numArray = new byte[8];
            for (int index = 0; index < numArray.Length; ++index)
                numArray[index] = this.ReadByte();
            return BitConverter.ToDouble(numArray, 0);
        }

        public virtual string ReadString()
        {
            short num = this.ReadShort();
            string @string = Encoding.UTF8.GetString(this.m_buffer, this.m_offset, (int)num);
            this.m_offset += (int)num;
            return @string.Replace("\0", "");
        }

        public virtual byte[] ReadBytes(int maxLen)
        {
            byte[] numArray = new byte[maxLen];
            Array.Copy((Array)this.m_buffer, this.m_offset, (Array)numArray, 0, maxLen);
            this.m_offset += maxLen;
            return numArray;
        }

        public virtual byte[] ReadBytes()
        {
            return this.ReadBytes(this.m_length - this.m_offset);
        }

        public DateTime ReadDateTime()
        {
            return new DateTime((int)this.ReadShort(), (int)this.ReadByte(), (int)this.ReadByte(), (int)this.ReadByte(), (int)this.ReadByte(), (int)this.ReadByte());
        }

        public virtual int CopyTo(byte[] dst, int dstOffset, int offset)
        {
            int count = this.m_length - offset < dst.Length - dstOffset ? this.m_length - offset : dst.Length - dstOffset;
            if (count > 0)
                System.Buffer.BlockCopy((Array)this.m_buffer, offset, (Array)dst, dstOffset, count);
            return count;
        }

        public virtual int CopyTo(byte[] dst, int dstOffset, int offset, int key)
        {
            int num = this.m_length - offset < dst.Length - dstOffset ? this.m_length - offset : dst.Length - dstOffset;
            if (num > 0)
            {
                key = (key & 16711680) >> 16;
                for (int index = 0; index < num; ++index)
                    dst[dstOffset + index] = (byte)((uint)this.m_buffer[offset + index] ^ (uint)key);
            }
            return num;
        }

        public virtual int CopyTo3(byte[] dst, int dstOffset, int offset, byte[] key, ref int packetArrangeSend)
        {
            int num1 = this.m_length - offset < dst.Length - dstOffset ? this.m_length - offset : dst.Length - dstOffset;
            string str = string.Empty;
            Console.WriteLine("Game.Base.Packet.Length" + (object)num1 + ".DDTOffset" + (string)(object)dstOffset + ".Offset" + (string)(object)offset);
            if (num1 > 0)
            {
                int num2 = 0;
                num2 = this.m_sended + dstOffset;
                int num3;
                if (this.isSended)
                {
                    this.packetNum = Interlocked.Increment(ref packetArrangeSend);
                    packetArrangeSend = this.packetNum;
                    this.m_sended = 0;
                    this.isSended = false;
                    num3 = this.m_sended + dstOffset;
                }
                else
                    num3 = 16869;
                if (this.packetNum != packetArrangeSend)
                    return 0;
                for (int index1 = 0; index1 < num1; ++index1)
                {
                    int index2 = offset + index1;
                    while (num3 > 16869)
                        num3 -= 16869;
                    if (this.m_sended == 0)
                    {
                        dst[dstOffset] = (byte)((uint)this.m_buffer[index2] ^ (uint)key[this.m_sended % 8]);
                    }
                    else
                    {
                        key[this.m_sended % 8] = (byte)((int)key[this.m_sended % 8] + (int)dst[num3 - 1] ^ this.m_sended);
                        dst[dstOffset + index1] = (byte)(((uint)this.m_buffer[index2] ^ (uint)key[this.m_sended % 8]) + (uint)dst[num3 - 1]);
                    }
                    ++this.m_sended;
                    ++num3;
                }
            }
            return num1;
        }

        public virtual int CopyFrom(byte[] src, int srcOffset, int offset, int count)
        {
            if (count >= this.m_buffer.Length || count - srcOffset >= src.Length)
                return -1;
            System.Buffer.BlockCopy((Array)src, srcOffset, (Array)this.m_buffer, offset, count);
            return count;
        }

        public virtual int CopyFrom(byte[] src, int srcOffset, int offset, int count, int key)
        {
            if (count >= this.m_buffer.Length || count - srcOffset >= src.Length)
                return -1;
            key = (key & 16711680) >> 16;
            for (int index = 0; index < count; ++index)
                this.m_buffer[offset + index] = (byte)((uint)src[srcOffset + index] ^ (uint)key);
            return count;
        }

        public virtual int[] CopyFrom3(byte[] src, int srcOffset, int offset, int count, byte[] key)
        {
            int[] numArray = new int[count];
            for (int index = 0; index < count; ++index)
                this.m_buffer[index] = src[index];
            if (count < this.m_buffer.Length && count - srcOffset < src.Length)
            {
                this.m_buffer[0] = (byte)((uint)src[srcOffset] ^ (uint)key[0]);
                for (int index = 1; index < count; ++index)
                {
                    key[index % 8] = (byte)((int)key[index % 8] + (int)src[srcOffset + index - 1] ^ index);
                    this.m_buffer[index] = (byte)((uint)src[srcOffset + index] - (uint)src[srcOffset + index - 1] ^ (uint)key[index % 8]);
                }
            }
            return numArray;
        }

        public virtual void WriteBoolean(bool val)
        {
            this.m_buffer[this.m_offset++] = val ? (byte)1 : (byte)0;
            this.m_length = this.m_offset > this.m_length ? this.m_offset : this.m_length;
        }

        public virtual void WriteByte(byte val)
        {
            this.m_buffer[this.m_offset++] = val;
            this.m_length = this.m_offset > this.m_length ? this.m_offset : this.m_length;
        }

        public virtual void Write(byte[] src)
        {
            this.Write(src, 0, src.Length);
        }

        public virtual void Write(byte[] src, int offset, int len)
        {
            Array.Copy((Array)src, offset, (Array)this.m_buffer, this.m_offset, len);
            this.m_offset += len;
            this.m_length = this.m_offset > this.m_length ? this.m_offset : this.m_length;
        }

        public virtual void WriteShort(short val)
        {
            this.WriteByte((byte)((uint)val >> 8));
            this.WriteByte((byte)((uint)val & (uint)byte.MaxValue));
        }

        public virtual void WriteShortLowEndian(short val)
        {
            this.WriteByte((byte)((uint)val & (uint)byte.MaxValue));
            this.WriteByte((byte)((uint)val >> 8));
        }

        public virtual void WriteInt(int val)
        {
            this.WriteByte((byte)(val >> 24));
            this.WriteByte((byte)(val >> 16 & (int)byte.MaxValue));
            this.WriteByte((byte)((val & (int)ushort.MaxValue) >> 8));
            this.WriteByte((byte)(val & (int)ushort.MaxValue & (int)byte.MaxValue));
        }

        public virtual void WriteFloat(float val)
        {
            this.Write(BitConverter.GetBytes(val));
        }

        public virtual void WriteDouble(double val)
        {
            this.Write(BitConverter.GetBytes(val));
        }

        public virtual void Fill(byte val, int num)
        {
            for (int index = 0; index < num; ++index)
                this.WriteByte(val);
        }

        public virtual void WriteLong(long val)
        {
            long num = val;
            int val1 = (int)num;
            string str1 = Convert.ToString(num, 2);
            string str2 = str1.Length <= 32 ? "" : str1.Substring(0, str1.Length - 32);
            int val2 = 0;
            for (int index = 0; index < str2.Length; ++index)
            {
                string str3 = str2.Substring(str2.Length - (index + 1));
                if (!(str3 == "0"))
                {
                    if (str3 == "1")
                        val2 += 1 << index;
                    else
                        break;
                }
            }
            this.WriteInt(val2);
            this.WriteInt(val1);
        }

        public virtual void WriteString(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                this.WriteShort((short)(bytes.Length + 1));
                this.Write(bytes, 0, bytes.Length);
                this.WriteByte((byte)0);
            }
            else
            {
                this.WriteShort((short)1);
                this.WriteByte((byte)0);
            }
        }

        public virtual void WriteString(string str, int maxlen)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            int len = bytes.Length < maxlen ? bytes.Length : maxlen;
            this.WriteShort((short)len);
            this.Write(bytes, 0, len);
        }

        public void WriteDateTime(DateTime date)
        {
            this.WriteShort((short)date.Year);
            this.WriteByte((byte)date.Month);
            this.WriteByte((byte)date.Day);
            this.WriteByte((byte)date.Hour);
            this.WriteByte((byte)date.Minute);
            this.WriteByte((byte)date.Second);
        }

        public virtual int CopyTo(byte[] dst, int dstOffset, int offset, ref byte[] key)
        {
            int num = this.m_length - offset < dst.Length - dstOffset ? this.m_length - offset : dst.Length - dstOffset;
            if (this.m_length - offset >= dst.Length - dstOffset)
                Console.WriteLine("Loi gi o day ? (" + (object)this.m_length + "," + (string)(object)offset + ")=" + (string)(object)(this.m_length - offset) + "->(" + (string)(object)dst.Length + "," + (string)(object)dstOffset + ")=" + (string)(object)(dst.Length - dstOffset));
            if (num > 0)
            {
                for (int index = 0; index < num; ++index)
                {
                    if (index == 0)
                    {
                        dst[dstOffset] = (byte)((uint)this.m_buffer[offset + index] ^ (uint)key[index % 8]);
                    }
                    else
                    {
                        key[index % 8] = (byte)((int)key[index % 8] + (int)dst[dstOffset + index - 1] ^ index);
                        dst[dstOffset + index] = (byte)(((uint)this.m_buffer[offset + index] ^ (uint)key[index % 8]) + (uint)dst[dstOffset + index - 1]);
                    }
                }
            }
            return num;
        }
    }
}
