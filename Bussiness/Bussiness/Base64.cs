using System;
namespace Bussiness
{
    public class Base64
    {
        private static readonly string BASE64_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
        public static string encodeByteArray(byte[] param1)
        {
            string text = "";
            byte[] array = new byte[4];
            for (int i = 0; i < param1.Length; i += 4)
            {
                byte[] array2 = new byte[3];
                for (int j = 0; j < param1.Length; j++)
                {
                    if (j < 3)
                    {
                        if (j + i > param1.Length)
                        {
                            break;
                        }
                        array2[j] = param1[j + i];
                    }
                }
                array[0] = (byte)((array2[0] & 252) >> 2);
                array[1] = (byte)((int)(array2[0] & 3) << 4 | array2[1] >> 4);
                array[2] = (byte)((int)(array2[1] & 15) << 2 | array2[2] >> 6);
                array[3] = (byte)(array2[2] & 63); //_loc_3[3] = (byte)(_loc_4[2] & 63);
                for (int k = array2.Length; k < 3; k++)
                {
                    array[k + 1] = 64;
                }
                for (int l = 0; l < array.Length; l++)
                {
                    text += Base64.BASE64_CHARS.Substring((int)array[l], 1);
                }
            }
            text = text.Substring(0, param1.Length - 1);
            return text + "=";
        }
        public static byte[] decodeToByteArray2(string param1)
        {
            byte[] array = new byte[param1.Length];
            byte[] array2 = new byte[4];
            for (int i = 0; i < param1.Length; i += 4)
            {
                int num = 0;
                int num2;
                do
                {
                    num2 = i + num;
                    if (num < 4)
                    {
                        array2[num] = (byte)Base64.BASE64_CHARS.IndexOf(param1.Substring(num2, 1));
                    }
                    num++;
                }
                while (num2 < param1.Length);
                int num3 = 0;
                while (num3 < array2.Length && array2[num3] != 64)
                {
                    array[i + num3] = array2[num3];
                    num3++;
                }
            }
            return array;
        }
        public static byte[] decodeToByteArray(string param1)
        {
            byte[] array = new byte[param1.Length];
            byte[] array2 = new byte[4];
            byte[] array3 = new byte[3];
            for (int i = 0; i < param1.Length; i += 4)
            {
                int num = 0;
                int num2;
                do
                {
                    num2 = i + num;
                    if (num < 4)
                    {
                        array2[num] = (byte)Base64.BASE64_CHARS.IndexOf(param1.Substring(num2, 1));
                    }
                    num++;
                }
                while (num2 < param1.Length);
                array3[0] = (byte)(((int)array2[0] << 2) + ((array2[1] & 48) >> 4));
                array3[1] = (byte)(((int)(array2[1] & 15) << 4) + ((array2[2] & 60) >> 2));
                array3[2] = (byte)(((int)(array2[2] & 3) << 6) + (int)array2[3]);
                int num3 = 0;
                while (num3 < array3.Length && array2[num3 + 1] != 64)
                {
                    array[i + num3] = array3[num3];
                    num3++;
                }
            }
            return array;
        }
    }
}
