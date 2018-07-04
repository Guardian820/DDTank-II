using System;
using System.IO;
using System.Text;
using zlib;
namespace Game.Base
{
	public class Marshal
	{
		public static string ConvertToString(byte[] cstyle)
		{
			if (cstyle == null)
			{
				return null;
			}
			for (int i = 0; i < cstyle.Length; i++)
			{
				if (cstyle[i] == 0)
				{
					return Encoding.Default.GetString(cstyle, 0, i);
				}
			}
			return Encoding.Default.GetString(cstyle);
		}
		public static int ConvertToInt32(byte[] val)
		{
			return Marshal.ConvertToInt32(val, 0);
		}
		public static int ConvertToInt32(byte[] val, int startIndex)
		{
			return Marshal.ConvertToInt32(val[startIndex], val[startIndex + 1], val[startIndex + 2], val[startIndex + 3]);
		}
		public static int ConvertToInt32(byte v1, byte v2, byte v3, byte v4)
		{
			return (int)v1 << 24 | (int)v2 << 16 | (int)v3 << 8 | (int)v4;
		}
		public static uint ConvertToUInt32(byte[] val)
		{
			return Marshal.ConvertToUInt32(val, 0);
		}
		public static uint ConvertToUInt32(byte[] val, int startIndex)
		{
			return Marshal.ConvertToUInt32(val[startIndex], val[startIndex + 1], val[startIndex + 2], val[startIndex + 3]);
		}
		public static uint ConvertToUInt32(byte v1, byte v2, byte v3, byte v4)
		{
			return (uint)((int)v1 << 24 | (int)v2 << 16 | (int)v3 << 8 | (int)v4);
		}
		public static short ConvertToInt16(byte[] val)
		{
			return Marshal.ConvertToInt16(val, 0);
		}
		public static short ConvertToInt16(byte[] val, int startIndex)
		{
			return Marshal.ConvertToInt16(val[startIndex], val[startIndex + 1]);
		}
		public static short ConvertToInt16(byte v1, byte v2)
		{
			return (short)((int)v1 << 8 | (int)v2);
		}
		public static ushort ConvertToUInt16(byte[] val)
		{
			return Marshal.ConvertToUInt16(val, 0);
		}
		public static ushort ConvertToUInt16(byte[] val, int startIndex)
		{
			return Marshal.ConvertToUInt16(val[startIndex], val[startIndex + 1]);
		}
		public static ushort ConvertToUInt16(byte v1, byte v2)
		{
			return (ushort)((int)v2 | (int)v1 << 8);
		}
		public static string ToHexDump(string description, byte[] dump)
		{
			return Marshal.ToHexDump(description, dump, 0, dump.Length);
		}
		public static string ToHexDump(string description, byte[] dump, int start, int count)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (description != null)
			{
				stringBuilder.Append(description).Append("\n");
			}
			int num = start + count;
			for (int i = start; i < num; i += 16)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				StringBuilder stringBuilder3 = new StringBuilder();
				stringBuilder3.Append(i.ToString("X4"));
				stringBuilder3.Append(": ");
				for (int j = 0; j < 16; j++)
				{
					if (j + i < num)
					{
						byte b = dump[j + i];
						stringBuilder3.Append(dump[j + i].ToString("X2"));
						stringBuilder3.Append(" ");
						if (b >= 32 && b <= 127)
						{
							stringBuilder2.Append((char)b);
						}
						else
						{
							stringBuilder2.Append(".");
						}
					}
					else
					{
						stringBuilder3.Append("   ");
						stringBuilder2.Append(" ");
					}
				}
				stringBuilder3.Append("  ");
				stringBuilder3.Append(stringBuilder2.ToString());
				stringBuilder3.Append('\n');
				stringBuilder.Append(stringBuilder3.ToString());
			}
			return stringBuilder.ToString();
		}
		public static byte[] Compress(byte[] src)
		{
			return Marshal.Compress(src, 0, src.Length);
		}
		public static byte[] Compress(byte[] src, int offset, int length)
		{
			MemoryStream memoryStream = new MemoryStream();
			Stream stream = new ZOutputStream(memoryStream, 9);
			stream.Write(src, offset, length);
			stream.Close();
			return memoryStream.ToArray();
		}
		public static byte[] Uncompress(byte[] src)
		{
			MemoryStream memoryStream = new MemoryStream();
			Stream stream = new ZOutputStream(memoryStream);
			stream.Write(src, 0, src.Length);
			stream.Close();
			return memoryStream.ToArray();
		}
	}
}
