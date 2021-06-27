using GT.WebServices.API.Application.Middleware;
using GT.WebServices.API.Application.Security;
using GT.WebServices.API.Core;
using GT.WebServices.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace GT.WebServices.API
{
   public class Startup
   {
      public Startup(IConfiguration configuration)
      {
         Configuration = configuration;
      }

      public IConfiguration Configuration { get; }

      // This method gets called by the runtime. Use this method to add services to the container.
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddApplicationInsightsTelemetry();
         services.Configure<AdsConfigurationOptions>(Configuration.GetSection(AdsConfigurationOptions.Name));
         services.AddAutoMapper(Assembly.GetExecutingAssembly());

         services.AddScoped<ITerminalConfiguration, TerminalConfiguration>();
         services.AddScoped<IEmployeeDataService, EmployeeDataService>();

         services.AddSingleton<IJwtTokenService, JwtTokenService>();

         services.AddControllers(options =>
         {
            options.OutputFormatters.RemoveType(typeof(SystemTextJsonOutputFormatter));
            options.ReturnHttpNotAcceptable = true;

         })
         .AddXmlSerializerFormatters()
         .AddNewtonsoftJson();
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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

         app.UseMiddleware<TerminalConfigurationMiddleware>();

         app.UseHttpsRedirection();
         app.UseRouting();
         //app.UseAuthorization();

         app.UseEndpoints(endpoints =>
         {
            endpoints.MapControllers();
         });
      }
   }
}
