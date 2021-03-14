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
            //3. här får jag tag på mitt värde och skriver ut värdet i output, 
            Configuration = configuration;
            Debug.WriteLine(Configuration["MyProperty"]);
            //3. Genom AsEnumerable-metoden i en forech-loop kan vi skriva ut en lista med alla instäööningar som laddats in.
            //3. foreach-metoden skiver ut samtliga key och value-par
            foreach (var prop in configuration.AsEnumerable())
            {
                Debug.WriteLine( $"key [{prop.Key}] \n value [{prop.Value}]\"" );
            }

        }

        public IConfiguration Configuration { get;}
        // This method gets called by the runtime. Use this method to add services to the container.


        //4. ConfigureServices kallas i byggfasen .Build() i Program.cs
        //Här läggs alla services klasser till och ställs in.
        //Om en sida behöver en HttpClient för ex OpenWeather eller DatabaseContext för att hitta ino från en databas
        //behöver jag lägga in de klasserna som egna Services, för att Razor Pages tjänsterna ska kunna använda dem.
        //här ställer man till exempel även om vissa Razer Pages tjänster endast är tillgängliga för inloggade användare.

        public void ConfigureServices(IServiceCollection services)
        {
            //5.4 Vi ska lägga in en varning om Cookie, genonm https://docs.microsoft.com/en-us/aspnet/core/security/gdpr?view=aspnetcore-5.0
            // bygger vi denna varning. 
            services.Configure<CookiePolicyOptions>( options =>
                {
                // denna lambda bestämmer hurvida användaren samtycker till icke-väsentlig cookies behövs för en given begäran
                options.CheckConsentNeeded = context => true;

                    // denna kod kräver:  using Microsoft.AspNetCore.Http;
                 options.MinimumSameSitePolicy = SameSiteMode.None;

                });
            //4. addRazorPages() hittar och lägger in samtliga Razor Pages automatiskt
            services.AddRazorPages();
            //4. addHostServices används när man vill lägga till en klass som ärver av IHostedService, denna tjänst startar
            //när vi kör Run() i Program.cs

            //4.3 här registrerar vi tjänsten MyBackgroundProcess, som finns i Program.cs med hjälp av addHostService.
            services.AddHostedService<MyBackgroundProcess>();

       

            //4.4 Vi kopplar ihop klassen MyBackgroundProcessOptions med json och gör den tillgänglig som en Servise. 
            //detta gör jag genom att kallas på .Configure metoden och skickar med klassnamnet samt rätt
            // sektion från json filen, detta görs genom Configuration.GetSection("MyBackgroundProcess")
            services.Configure<MyBackgroundProcessOptions>(Configuration.GetSection("MyBackgroundProcess"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        //4. Configure kallas på när webHost kör igång i Program.cs (.Run()) 

        //5. SIsta biten av startup.cs körs direkt efter att vikallat på .Run() i Program.cs. 
        // Här inne stller vi in vilka sk. Middleware som ska användas. 
        // vi ställer in alla trappsteg på resan från ett http paketet anländer 
        // - webHost från en klient - vi når rätt Razor Page för ändamålet. Därefter sker 
        // hela trappan baklänges: Razor Pages skapar ett hmlt svar som skickas upp genom samma Middleware steg
        // tills en http 200 OK genereras och skickas tillbaka till klienten. (se bild i pp föreläsning1 ASP.Core Net)
        // Alla steg från ExceptionHandler till Authorization m.m sker här. 
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //5. för varje app.Use...() finns tillhörande inställningar som alla står besrkivna på msdn. (https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-5.0#built-in-middleware)
            // det är också viktigt att ha kolla på vilken ordning metoderna kallas på. 
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

            //5.3 jag stoppar in metoden InspectHttpRequeastAndResponse från private async Task InspectHttpRequeastAndRespons för att kunna analysera 
            // kommande http request och utgeånde http response. För att analysera dessa debuggar jag dessa inuti metoden. 

            // 5.3 i app.Use() använder vi för att lägga in metoden i piplinen. 
          
            app.Use(InspectHttpRequeastAndResponse);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            //5.4 används för varning om Cookie 
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthorization();

            // 5. endpoint hänvisar till slutet av vår hemsida, det kan vara MVC eller en Raxor Page.
            // Koden nedan ser till att rätt url eller index.html går till rätt Endpoint
            app.UseEndpoints(endpoints =>
            {
                // 5.1 om vi kommenterar ut/ta bort  endpoints.MapRazorPages(); hittar vi inte index.html längre och Error.cshtml skrivs ut på sidan.
                endpoints.MapRazorPages();

                //5.2 Efter att skapat asynk Task WriteHTMLResponse() skapar jag en endpoints.MapGet() och som inargument get
                // skriver jag en egen url och kallar på WriteHTMLResponse() metoden. På webbplatsen skriver jag sedan ut: 
                // webrooten/JohannasSida
                endpoints.MapGet("/JohannasSida", WriteHTMLResponse);
            });
        }
        //5.2 Jag skapar en async Task för att manuellt ställa in koppling mellan inkommande url och sidor.
     
        private async Task WriteHTMLResponse(HttpContext context)
        {
            var message = Encoding.ASCII.GetBytes("<p>Johannas html!</p>");
            await context.Response.Body.WriteAsync(message, 0, message.Length);
        }

        //5.3 Här skriver jag min eget "Middleware trappsteg". Detta görs genom att kalla på app.Use()
        //denna metod låter oss sedan lägga in egna instruktioner om vad soma ska ske när
        //ett http request kommer. 
        private async Task InspectHttpRequeastAndResponse(HttpContext context, Func<Task> nextMiddleware)
        {
           
            //här tittar vi på context.Request. Vad skrivs ut i debug: 
            Debug.WriteLine("Här kommer en http request från : " + context.Request.Host); //localhost: 5001
            Debug.WriteLine("för att få tag på: " + context.Request.Path); // /

            //Först sker task InspectHttpRequeastAndResponse(). därefter för att kunna referera tll metoden i app.Use()
            // skriver jag; 
            await nextMiddleware();
            //...för att kunna leverera meddelandet vidare till nästa steg i piplinen, måste vi kalla på steget direkt och vänta 
            // på att resten av piplinen jobbat klart. 


            // här tittar vi på context.Response. Vad skrivs ut i debug: 
            Debug.WriteLine("Servern säger: " + context.Response.StatusCode); // 200
        }
    }
}
