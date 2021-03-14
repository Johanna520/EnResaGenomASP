using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace EnResaGenomASP
{
    public class Program
    {
        public static void Main(string[] args)
        //H�r startar programmet - Main
        {
            //1. loggar en string i vs output konsollen
            Debug.WriteLine("HELLO, lets start debug!");
            //1. skriver ut en string i konsollen som startar upp
            Console.WriteLine("HELLO, lets start!");

            //2. h�r skriver jag om creatHostBuilder s� allt sker i Main-metoden ist�llet.

            //2. jag skapar en variabel: HotstBuilder och l�gger DefaultBuilder d�r i. 
            IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args);
            //2. jag kallar p� ConfigureWebHostDefaults, och sparar ner resultatet i webHost. 
            hostBuilder.ConfigureWebHostDefaults(webBuilder =>
            {//2. h�r ber�ttar vi att att man ska anv�nda startup-classen och dess metoder. 
                webBuilder.UseStartup<Startup>();
            });
            //2. Jag kallar p� Build() i variabeln webHost
            IHost webHost = hostBuilder.Build();
            //2. i webHost kallar jag sedan p� Run(); Run() kommer att lyssna efter http anrop fr�n klienten. 
            webHost.Run();          
        }
    
    }
}

