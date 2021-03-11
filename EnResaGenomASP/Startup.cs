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
            //här får jag tag på mitt värde och skriver ut värdet i output, 
            Configuration = configuration;
            Debug.WriteLine(Configuration["MyProperty"]);
            //Genom AsEnumerable-metoden i en forech-loop kan vi skriva ut en lista med alla instäööningar som laddats in.
            //foreach-metoden skiver ut samtliga key och value-par
            foreach (var prop in configuration.AsEnumerable())
            {
                Debug.WriteLine( $"key [{prop.Key}] \n value [{prop.Value}]\"" );
            }

        }

        public IConfiguration Configuration { get;}
        // This method gets called by the runtime. Use this method to add services to the container.

        //ConfigureServices kallas i byggfasen .Build() i Program.cs
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddHostedService<MyBackgroundProcess>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        //Configure kallas på när webHost kör igång i Program.cs (.Run()) 
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
