using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnResaGenomASP
{// Jag bygger en ny klass som ska reflektera e bit av json strukturen och låter denna klassen vara tllgänglig 
 // som en service i resten av programmet. 
  
    public class MyBackgroundProcessOptions
    {
        //jag lägger till en property som kallas Delay
        public int Delay { get; set; }
    }
}
