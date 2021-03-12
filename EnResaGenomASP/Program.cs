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
            //i webHost kallar jag sedan p� Run(); Run() kommer att lyssna efter http anrop fr�n klienten. 
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

    //Metoden Repeat() �r en async metod som returnerar och anv�nder await task.Delay() i en o�ndlig loop
    //f�r att f�r att regelbunder logga n�got var 10 sek.
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
