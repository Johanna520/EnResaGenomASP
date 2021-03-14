using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnResaGenomASP
{// 4.4 Jag bygger en ny klass som ska reflektera e bit av json strukturen och låter denna klassen vara tllgänglig 
 // som en service i resten av programmet. 
  
    public class MyBackgroundProcessOptions
    {
        //4.4 jag lägger till en property som kallas Delay
        public int Delay { get; set; }
    }
}
