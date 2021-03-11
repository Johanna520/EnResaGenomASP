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
        //Här startar programmet - Main
        {
            //loggar en strin i vs output konsollen
            Debug.WriteLine("HELLO, lets start debug!");
            //skriver ut en string i konsollen som startar upp
            Console.WriteLine("HELLO, lets start!");

            //här skriver jag om creatHostBuilder så allt sker i Main-metoden istället.

            //jag skapar en variabel: HotstBuilder och lägger DefaultBuilder där i. 
            IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args);
            //jag kallar på ConfigureWebHostDefaults, och sparar ner resultatet i webHost. 
            hostBuilder.ConfigureWebHostDefaults(webBuilder =>
            {//här berättar vi att att man ska använda startup-classen och dess metoder. 
                webBuilder.UseStartup<Startup>();
            });
            //Jag kallar på Build() i variabeln webHost
            IHost webHost = hostBuilder.Build();
            //i webHost kallar jag sedan på Run();
            webHost.Run();          
        }
    
    }
}
