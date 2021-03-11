using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EnResaGenomASP
{
    public class Program
    {
        public static void Main(string[] args)
        //H�r startar programmet - Main
        {
            //loggar en strin i vs output konsollen
            Debug.WriteLine("HELLO, lets start debug!");
            //skriver ut en string i konsollen som startar upp
            Console.WriteLine("HELLO, lets start!");

            //h�r skriver jag om creatHostBuilder s� allt sker i Main-metoden ist�llet.

            //jag skapar en variabel: HotstBuilder och l�gger DefaultBuilder d�r i. 
            IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args);
            //jag kallar p� ConfigureWebHostDefaults, och sparar ner resultatet i webHost. 
            hostBuilder.ConfigureWebHostDefaults(webBuilder =>
            {//h�r ber�ttar vi att att man ska anv�nda startup-classen och dess metoder. 
                webBuilder.UseStartup<Startup>();
            });
            //Jag kallar p� Build() i variabeln webHost
            IHost webHost = hostBuilder.Build();
            //i webHost kallar jag sedan p� Run();
            webHost.Run();          
        }
    
    }
}
