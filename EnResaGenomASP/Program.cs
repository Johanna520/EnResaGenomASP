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
        //Här startar programmet - Main
        {
            //1. loggar en string i vs output konsollen
            Debug.WriteLine("HELLO, lets start debug!");
            //1. skriver ut en string i konsollen som startar upp
            Console.WriteLine("HELLO, lets start!");

            //2. här skriver jag om creatHostBuilder så allt sker i Main-metoden istället.

            //2. jag skapar en variabel: HotstBuilder och lägger DefaultBuilder där i. 
            IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args);
            //2. jag kallar på ConfigureWebHostDefaults, och sparar ner resultatet i webHost. 
            hostBuilder.ConfigureWebHostDefaults(webBuilder =>
            {//2. här berättar vi att att man ska använda startup-classen och dess metoder. 
                webBuilder.UseStartup<Startup>();
            });
            //2. Jag kallar på Build() i variabeln webHost
            IHost webHost = hostBuilder.Build();
            //2. i webHost kallar jag sedan på Run(); Run() kommer att lyssna efter http anrop från klienten. 
            webHost.Run();          
        }
    
    }
}

