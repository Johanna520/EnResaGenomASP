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
    //4.3 jag skapar en ny klass som tar in implementerar IHostedService
    public class MyBackgroundProcess : IHostedService
    {
        //4.3 Klassens logger ser ut som indexcshtml.cs
        public readonly ILogger<MyBackgroundProcess> _logger;



        // 4.4 för att kunna använda MyBackgroundProcessOptions tjänster lägger vjag till en privat tjänst som kan
        //referera till potions objektet, lik _logger, objektet döper jag till _options.
        private readonly MyBackgroundProcessOptions _options;

        public MyBackgroundProcess (ILogger<MyBackgroundProcess> logger,
            //4.4 Jag anväder   IOptions<MyBackgroundProcessOptions> Options) i kosntruktorn för att få tag på tjänsten.
            IOptions<MyBackgroundProcessOptions> Options)
        {
            _logger = logger;

            // 4.4 i options.value finns options objekt 
            _options = Options.Value;
        }
        //4.3 Task.Run() startar ett parallelt arbete resultat returnerar jag direkt. 
        //Som inargument skickar jag med metoden Repeat
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(Repeat);

        }
      

        //4.3 Metoden Repeat() är en async metod som returnerar och använder await task.Delay() i en oändlig loop
        //för att för att regelbunder logga något ex var 10 sek.
        async void Repeat()
        {
            while (true)
            {
                //4.3 await Task.Delay(10000);
                //_logger.LogInformation("Ten seconds has passed");

                // 4.4 jag använder _options.Delay för att kalla på task.delay
                await Task.Delay(_options.Delay * 1000);
                _logger.LogInformation(_options.Delay + " second(s) has passed");
            }
        }


        //4.3 StopAsync kan endast returnera Task.CompletedTask.
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
   

}
