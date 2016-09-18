using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public class Encryption
    {
        public bool Cancel { set; get; }

        public string Uncode(string key, string text)
        {
            Cancel = false;

            byte[] userKey = Encoding.Default.GetBytes(key);

            int keyInt =0;
            for (int i = 0; i < userKey.Length; i++)
            {
                keyInt += (int)userKey[i];
            }

            //конвертация ключа
            //keyInt = BitConverter.ToInt32(userKey, 0);
            byte keyByte = (byte)keyInt;

            //конвертация строки в байты
            byte[] textBytes = Encoding.Default.GetBytes(text);

            byte[] uncodeBytes = new byte[textBytes.Length];

            for (int k = 0; k < textBytes.Length; k++)
            {
                uncodeBytes[k] = (byte) (textBytes[k] ^ keyByte);
                if(Cancel == true)
                 break;
            }

            if (Cancel == true)
                return null;

            return Encoding.Default.GetString(uncodeBytes);

        }
    }
}
