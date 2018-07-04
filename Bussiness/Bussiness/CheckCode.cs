using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
namespace Bussiness
{
	public class CheckCode
	{
		private enum RandomStringMode
		{
			LowerLetter,
			UpperLetter,
			Letter,
			Digital,
			Mix
		}
		public static ThreadSafeRandom rand = new ThreadSafeRandom();
		private static Color[] c = new Color[]
		{
			Color.BlueViolet,
			Color.Red,
			Color.DarkBlue,
			Color.Green,
			Color.Orange,
			Color.Brown,
			Color.DarkCyan,
			Color.Purple
		};
		private static string[] font = new string[]
		{
			"Verdana",
			"Terminal",
			"Comic Sans MS",
			"Arial",
			"Tekton Pro"
		};
		private static char[] digitals = new char[]
		{
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9'
		};
		private static char[] lowerLetters = new char[]
		{
			'a',
			'b',
			'c',
			'd',
			'e',
			'f',
			'h',
			'k',
			'm',
			'n',
			'p',
			'q',
			'r',
			's',
			't',
			'u',
			'v',
			'w',
			'x',
			'y',
			'z'
		};
		private static char[] upperLetters = new char[]
		{
			'A',
			'B',
			'C',
			'D',
			'E',
			'F',
			'G',
			'H',
			'K',
			'M',
			'N',
			'P',
			'Q',
			'R',
			'S',
			'T',
			'U',
			'V',
			'W',
			'X',
			'Y',
			'Z'
		};
		private static char[] letters = new char[]
		{
			'a',
			'b',
			'c',
			'd',
			'e',
			'f',
			'g',
			'h',
			'i',
			'j',
			'k',
			'l',
			'm',
			'n',
			'p',
			'q',
			'r',
			's',
			't',
			'u',
			'v',
			'w',
			'x',
			'y',
			'z',
			'A',
			'B',
			'C',
			'D',
			'E',
			'F',
			'G',
			'H',
			'I',
			'J',
			'K',
			'L',
			'M',
			'N',
			'P',
			'Q',
			'R',
			'S',
			'T',
			'U',
			'V',
			'W',
			'X',
			'Y',
			'Z'
		};
		private static char[] mix = new char[]
		{
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9',
			'a',
			'b',
			'c',
			'd',
			'e',
			'f',
			'h',
			'k',
			'm',
			'n',
			'p',
			'q',
			'r',
			's',
			't',
			'u',
			'v',
			'w',
			'x',
			'y',
			'z',
			'A',
			'B',
			'C',
			'D',
			'E',
			'F',
			'G',
			'H',
			'K',
			'M',
			'N',
			'P',
			'Q',
			'R',
			'S',
			'T',
			'U',
			'V',
			'W',
			'X',
			'Y',
			'Z'
		};
		public static byte[] CreateImage(string randomcode)
		{
			int num = 30;
			int width = randomcode.Length * 30;
			Bitmap bitmap = new Bitmap(width, 36);
			Graphics graphics = Graphics.FromImage(bitmap);
			byte[] result;
			try
			{
				graphics.Clear(Color.Transparent);
				int num2 = CheckCode.rand.Next(7);
				Brush brush = new SolidBrush(CheckCode.c[num2]);
				for (int i = 0; i < 1; i++)
				{
					int num3 = CheckCode.rand.Next(bitmap.Width / 2);
					int num4 = CheckCode.rand.Next(bitmap.Width * 3 / 4, bitmap.Width);
					int num5 = CheckCode.rand.Next(bitmap.Height);
					int num6 = CheckCode.rand.Next(bitmap.Height);
					graphics.DrawBezier(new Pen(CheckCode.c[num2], 2f), (float)num3, (float)num5, (float)((num3 + num4) / 4), 0f, (float)((num3 + num4) * 3 / 4), (float)bitmap.Height, (float)num4, (float)num6);
				}
				char[] array = randomcode.ToCharArray();
				StringFormat stringFormat = new StringFormat(StringFormatFlags.NoClip);
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Center;
				for (int j = 0; j < array.Length; j++)
				{
					int num7 = CheckCode.rand.Next(5);
					Font font = new Font(CheckCode.font[num7], 22f, FontStyle.Bold);
					Point point = new Point(16, 16);
					float num8 = (float)ThreadSafeRandom.NextStatic(-num, num);
					graphics.TranslateTransform((float)point.X, (float)point.Y);
					graphics.RotateTransform(num8);
					graphics.DrawString(array[j].ToString(), font, brush, 1f, 1f, stringFormat);
					graphics.RotateTransform(-num8);
					graphics.TranslateTransform(2f, (float)(-(float)point.Y));
				}
				MemoryStream memoryStream = new MemoryStream();
				bitmap.Save(memoryStream, ImageFormat.Png);
				result = memoryStream.ToArray();
			}
			finally
			{
				graphics.Dispose();
				bitmap.Dispose();
			}
			return result;
		}
		private static string GenerateRandomString(int length, CheckCode.RandomStringMode mode)
		{
			string text = string.Empty;
			if (length == 0)
			{
				return text;
			}
			switch (mode)
			{
			case CheckCode.RandomStringMode.LowerLetter:
				for (int i = 0; i < length; i++)
				{
					text += CheckCode.lowerLetters[CheckCode.rand.Next(0, CheckCode.lowerLetters.Length)];
				}
				break;

			case CheckCode.RandomStringMode.UpperLetter:
				for (int j = 0; j < length; j++)
				{
					text += CheckCode.upperLetters[CheckCode.rand.Next(0, CheckCode.upperLetters.Length)];
				}
				break;

			case CheckCode.RandomStringMode.Letter:
				for (int k = 0; k < length; k++)
				{
					text += CheckCode.letters[CheckCode.rand.Next(0, CheckCode.letters.Length)];
				}
				break;

			case CheckCode.RandomStringMode.Digital:
				for (int l = 0; l < length; l++)
				{
					text += CheckCode.digitals[CheckCode.rand.Next(0, CheckCode.digitals.Length)];
				}
				break;

			default:
				for (int m = 0; m < length; m++)
				{
					text += CheckCode.mix[CheckCode.rand.Next(0, CheckCode.mix.Length)];
				}
				break;
			}
			return text;
		}
		public static string GenerateCheckCode()
		{
			return CheckCode.GenerateRandomString(4, CheckCode.RandomStringMode.Mix);
		}
	}
}
