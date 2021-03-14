using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text;

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
            // det �r ocks� viktigt att ha kolla p� vilken orning metoderna kallas p�. 
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // 5. endpoint h�nvisar till slutet av v�r hemsida, det kan vara MVC eller en Raxor Page.
            // Koden nedan ser till att r�tt url eller index.html g�r till r�tt Endpoint
            app.UseEndpoints(endpoints =>
            {
                // 5.1 om vi kommenterar ut/ta bort  endpoints.MapRazorPages(); hittar vi inte index.html l�ngre och ett error-meddelande skrivs ut p� sidan.
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
    }
}
