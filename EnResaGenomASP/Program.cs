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
            //i webHost kallar jag sedan på Run(); Run() kommer att lyssna efter http anrop från klienten. 
            webHost.Run();          
        }
    
    }
}

//jag skapar en ny klass som tar in implementerar IHostedService
public class MyBackgroundProcess : IHostedService
{

   //Klassens logger ser ut som indexcshtml.cs
    public readonly ILogger<MyBackgroundProcess> _logger;

    public MyBackgroundProcess (ILogger<MyBackgroundProcess> logger)
    {
        _logger = logger;
    }
    //Task.Run() startar ett parallelt arbete Dess resultat returnerar jag direkt. 
    //Som inargument skickar jag med metoden Repeat
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.Run(Repeat);
    }

    //Metoden Repeat() är en async metod som returnerar och använder await task.Delay() i en oändlig loop
    //för att för att regelbunder logga något var 10 sek.
     async void Repeat()
    {
        while(true)
        {
            await Task.Delay(10000);
            _logger.LogInformation("Ten seconds has passed");
        }
    }


    //StopAsync kan endast returnera Task.CompletedTask.
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
