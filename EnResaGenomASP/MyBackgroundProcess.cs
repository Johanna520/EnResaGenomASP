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
        private readonly MyBackgroundProcessOptions _options;

        public MyBackgroundProcess (ILogger<MyBackgroundProcess> logger, 
            IOptions<MyBackgroundProcessOptions> Options)
        {
            _logger = logger;
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
