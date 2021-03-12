using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace EnResaGenomASP
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //h�r f�r jag tag p� mitt v�rde och skriver ut v�rdet i output, 
            Configuration = configuration;
            Debug.WriteLine(Configuration["MyProperty"]);
            //Genom AsEnumerable-metoden i en forech-loop kan vi skriva ut en lista med alla inst���ningar som laddats in.
            //foreach-metoden skiver ut samtliga key och value-par
            foreach (var prop in configuration.AsEnumerable())
            {
                Debug.WriteLine( $"key [{prop.Key}] \n value [{prop.Value}]\"" );
            }

        }

        public IConfiguration Configuration { get;}
        // This method gets called by the runtime. Use this method to add services to the container.


        //ConfigureServices kallas i byggfasen .Build() i Program.cs
        //H�r l�ggs alla services klasser till och st�lls in.
        //Om en sida beh�ver en HttpClient f�r ex OpenWeather eller DatabaseContext f�r att hitta ino fr�n en databas
        //beh�ver jag l�gga in de klasserna som egna Services, f�r att Razor Pages tj�nsterna ska kunna anv�nda dem.
        //h�r st�ller man till exempel �ven om vissa Razer Pages tj�nster endast �r tillg�ngliga f�r inloggade anv�ndare.

        public void ConfigureServices(IServiceCollection services)
        {
            //addRazorPages() hittar och l�gger in samtliga Razor Pages automatiskt
            services.AddRazorPages();
            //addHostServices anv�nds n�r man vill l�gga till en klass som �rver av IHostedService, denna tj�nst startar
            //n�r vi k�r Run() i Program.cs

            //h�r registrerar vi tj�nsten MyBackgroundProcess, som finns i Program.cs med hj�lp av addHostService.
            services.AddHostedService<MyBackgroundProcess>();
            services.Configure<MyBackgroundProcessOptions>(Configuration.GetSection("MyBackgroundProcess"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        //Configure kallas p� n�r webHost k�r ig�ng i Program.cs (.Run()) 
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
