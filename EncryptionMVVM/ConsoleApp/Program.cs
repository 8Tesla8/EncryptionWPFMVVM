using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Security;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Security.Encryption rc4 = new Security.Encryption();


            Console.WriteLine("enter key");
            string strKey = Console.ReadLine();

            //rc4.Key(strKey);

            Console.WriteLine("enter string");
            string str = Console.ReadLine();

            string uncode = rc4.Uncode(strKey, str);
            Console.WriteLine(uncode);

            string uncode1 = rc4.Uncode(strKey, uncode);
            Console.WriteLine(uncode1);

            Console.ReadKey();
        }
    }
}
