using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
namespace DistrSystems2
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length==0)
            {
                new Bank().Run();
            }
   
            else
            {
                switch (args[0].ToLower()){
                    case "c":
                        new Client().Run();
                        break;
                    case "s":
                        new Bank().Run();
                        break;
                    default:
                        new Bank().Run();
                        break;
                }
            }
        }
    }
}
