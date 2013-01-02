using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sonosify
{
    class Program
    {
        static void Main(string[] args)
        {
            SonosLocator locator = new SonosLocator();
            locator.CreateSonosListener();

            foreach (var device in locator.Devices)
            {
                Console.WriteLine(device.Location);
            }

            Console.ReadLine();
        }
    }
}
