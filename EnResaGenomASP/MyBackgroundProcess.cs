using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Options;

namespace EnResaGenomASP
{
    //jag skapar en ny klass som tar in implementerar IHostedService
    public class MyBackgroundProcess : IHostedService
    {
        //Klassens logger ser ut som indexcshtml.cs
        public readonly ILogger<MyBackgroundProcess> _logger;



        // för att kunna använda MyBackgroundProcessOptions tjänster lägger vjag till en privat tjänst som kan
        //referera till potions objektet, lik _logger, objektet döper jag till _options.
        private readonly MyBackgroundProcessOptions _options;

        public MyBackgroundProcess (ILogger<MyBackgroundProcess> logger,
            //jag anväder   IOptions<MyBackgroundProcessOptions> Options) i kosntruktorn för att få tag på tjänsten.
            IOptions<MyBackgroundProcessOptions> Options)
        {
            _logger = logger;

            // i options.value finns options objekt 
            _options = Options.Value;
        }
        //Task.Run() startar ett parallelt arbete Dess resultat returnerar jag direkt. 
        //Som inargument skickar jag med metoden Repeat
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(Repeat);

        }

        //Metoden Repeat() är en async metod som returnerar och använder await task.Delay() i en oändlig loop
        //för att för att regelbunder logga något ex var 10 sek.
        async void Repeat()
        {
            while (true)
            {
                //await Task.Delay(10000);
                //_logger.LogInformation("Ten seconds has passed");

                // jag använder _options.Delay för att kalla på task.delay
                await Task.Delay(_options.Delay * 1000);
                _logger.LogInformation(_options.Delay + " second(s) has passed");
            }
        }


        //StopAsync kan endast returnera Task.CompletedTask.
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
   

}
