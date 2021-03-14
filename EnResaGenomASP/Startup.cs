using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text;
using System;

namespace EnResaGenomASP
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //3. h�r f�r jag tag p� mitt v�rde och skriver ut v�rdet i output, 
            Configuration = configuration;
            Debug.WriteLine(Configuration["MyProperty"]);
            //3. Genom AsEnumerable-metoden i en forech-loop kan vi skriva ut en lista med alla inst���ningar som laddats in.
            //3. foreach-metoden skiver ut samtliga key och value-par
            foreach (var prop in configuration.AsEnumerable())
            {
                Debug.WriteLine( $"key [{prop.Key}] \n value [{prop.Value}]\"" );
            }

        }

        public IConfiguration Configuration { get;}
        // This method gets called by the runtime. Use this method to add services to the container.


        //4. ConfigureServices kallas i byggfasen .Build() i Program.cs
        //H�r l�ggs alla services klasser till och st�lls in.
        //Om en sida beh�ver en HttpClient f�r ex OpenWeather eller DatabaseContext f�r att hitta ino fr�n en databas
        //beh�ver jag l�gga in de klasserna som egna Services, f�r att Razor Pages tj�nsterna ska kunna anv�nda dem.
        //h�r st�ller man till exempel �ven om vissa Razer Pages tj�nster endast �r tillg�ngliga f�r inloggade anv�ndare.

        public void ConfigureServices(IServiceCollection services)
        {
            //5.4 Vi ska l�gga in en varning om Cookie, genonm https://docs.microsoft.com/en-us/aspnet/core/security/gdpr?view=aspnetcore-5.0
            // bygger vi denna varning. 
            services.Configure<CookiePolicyOptions>( options =>
                {
                // denna lambda best�mmer hurvida anv�ndaren samtycker till icke-v�sentlig cookies beh�vs f�r en given beg�ran
                options.CheckConsentNeeded = context => true;

                    // denna kod kr�ver:  using Microsoft.AspNetCore.Http;
                 options.MinimumSameSitePolicy = SameSiteMode.None;

                });
            //4. addRazorPages() hittar och l�gger in samtliga Razor Pages automatiskt
            services.AddRazorPages();
            //4. addHostServices anv�nds n�r man vill l�gga till en klass som �rver av IHostedService, denna tj�nst startar
            //n�r vi k�r Run() i Program.cs

            //4.3 h�r registrerar vi tj�nsten MyBackgroundProcess, som finns i Program.cs med hj�lp av addHostService.
            services.AddHostedService<MyBackgroundProcess>();

       

            //4.4 Vi kopplar ihop klassen MyBackgroundProcessOptions med json och g�r den tillg�nglig som en Servise. 
            //detta g�r jag genom att kallas p� .Configure metoden och skickar med klassnamnet samt r�tt
            // sektion fr�n json filen, detta g�rs genom Configuration.GetSection("MyBackgroundProcess")
            services.Configure<MyBackgroundProcessOptions>(Configuration.GetSection("MyBackgroundProcess"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        //4. Configure kallas p� n�r webHost k�r ig�ng i Program.cs (.Run()) 

        //5. SIsta biten av startup.cs k�rs direkt efter att vikallat p� .Run() i Program.cs. 
        // H�r inne stller vi in vilka sk. Middleware som ska anv�ndas. 
        // vi st�ller in alla trappsteg p� resan fr�n ett http paketet anl�nder 
        // - webHost fr�n en klient - vi n�r r�tt Razor Page f�r �ndam�let. D�refter sker 
        // hela trappan bakl�nges: Razor Pages skapar ett hmlt svar som skickas upp genom samma Middleware steg
        // tills en http 200 OK genereras och skickas tillbaka till klienten. (se bild i pp f�rel�sning1 ASP.Core Net)
        // Alla steg fr�n ExceptionHandler till Authorization m.m sker h�r. 
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //5. f�r varje app.Use...() finns tillh�rande inst�llningar som alla st�r besrkivna p� msdn. (https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-5.0#built-in-middleware)
            // det �r ocks� viktigt att ha kolla p� vilken ordning metoderna kallas p�. 
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //5.3 jag stoppar in metoden InspectHttpRequeastAndResponse fr�n private async Task InspectHttpRequeastAndRespons f�r att kunna analysera 
            // kommande http request och utge�nde http response. F�r att analysera dessa debuggar jag dessa inuti metoden. 

            // 5.3 i app.Use() anv�nder vi f�r att l�gga in metoden i piplinen. 
          
            app.Use(InspectHttpRequeastAndResponse);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            //5.4 anv�nds f�r varning om Cookie 
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthorization();

            // 5. endpoint h�nvisar till slutet av v�r hemsida, det kan vara MVC eller en Raxor Page.
            // Koden nedan ser till att r�tt url eller index.html g�r till r�tt Endpoint
            app.UseEndpoints(endpoints =>
            {
                // 5.1 om vi kommenterar ut/ta bort  endpoints.MapRazorPages(); hittar vi inte index.html l�ngre och Error.cshtml skrivs ut p� sidan.
                endpoints.MapRazorPages();

                //5.2 Efter att skapat asynk Task WriteHTMLResponse() skapar jag en endpoints.MapGet() och som inargument get
                // skriver jag en egen url och kallar p� WriteHTMLResponse() metoden. P� webbplatsen skriver jag sedan ut: 
                // webrooten/JohannasSida
                endpoints.MapGet("/JohannasSida", WriteHTMLResponse);
            });
        }
        //5.2 Jag skapar en async Task f�r att manuellt st�lla in koppling mellan inkommande url och sidor.
     
        private async Task WriteHTMLResponse(HttpContext context)
        {
            var message = Encoding.ASCII.GetBytes("<p>Johannas html!</p>");
            await context.Response.Body.WriteAsync(message, 0, message.Length);
        }

        //5.3 H�r skriver jag min eget "Middleware trappsteg". Detta g�rs genom att kalla p� app.Use()
        //denna metod l�ter oss sedan l�gga in egna instruktioner om vad soma ska ske n�r
        //ett http request kommer. 
        private async Task InspectHttpRequeastAndResponse(HttpContext context, Func<Task> nextMiddleware)
        {
           
            //h�r tittar vi p� context.Request. Vad skrivs ut i debug: 
            Debug.WriteLine("H�r kommer en http request fr�n : " + context.Request.Host); //localhost: 5001
            Debug.WriteLine("f�r att f� tag p�: " + context.Request.Path); // /

            //F�rst sker task InspectHttpRequeastAndResponse(). d�refter f�r att kunna referera tll metoden i app.Use()
            // skriver jag; 
            await nextMiddleware();
            //...f�r att kunna leverera meddelandet vidare till n�sta steg i piplinen, m�ste vi kalla p� steget direkt och v�nta 
            // p� att resten av piplinen jobbat klart. 


            // h�r tittar vi p� context.Response. Vad skrivs ut i debug: 
            Debug.WriteLine("Servern s�ger: " + context.Response.StatusCode); // 200
        }
    }
}
