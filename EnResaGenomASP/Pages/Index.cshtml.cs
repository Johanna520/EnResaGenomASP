using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnResaGenomASP.Pages
{
    //IndexModel-klassen är en sk. service som ansvarar för just denna sida i vår hemsida 
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
           
           
        }
      

        public void OnGet()
        {
            //LogInformation är till för informativa meddelanden och inte i debuggsyfte. därför
            //använder vi .LogDebug istället. Men då loggnivån i appsettings.Development.json är instället
            //per default på "Information" måste man ändra detta till "debug" innan .LogDebug kan användas. 

            _logger.LogDebug("en http request för index.html");
        }
    }
}
