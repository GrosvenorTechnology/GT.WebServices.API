using GT.WebServices.API.Application.Middleware;
using GT.WebServices.API.Application.Security;
using GT.WebServices.API.Core;
using GT.WebServices.API.Services;
using Microsoft.AspNetCore.Mvc.Formatters;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddApplicationInsightsTelemetry();
builder.Services.Configure<AdsConfigurationOptions>(builder.Configuration.GetSection(AdsConfigurationOptions.Name));

builder.Services.AddScoped<ITerminalConfiguration, TerminalConfiguration>();
builder.Services.AddScoped<EmployeeDataService, EmployeeDataService>();
builder.Services.AddScoped<JobCategoryDataService, JobCategoryDataService>();
builder.Services.AddScoped<JobCodeDataService, JobCodeDataService>();
builder.Services.AddScoped<ScheduleDataService, ScheduleDataService>();

builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddSingleton<IDataCollectionService, DataCollectionService>();

builder.Services.AddControllers(options =>
    {
        options.OutputFormatters.RemoveType(typeof(SystemTextJsonOutputFormatter));
        options.ReturnHttpNotAcceptable = true;

    })
    .AddXmlSerializerFormatters()
    .AddNewtonsoftJson();

var app = builder.Build();

app.UseDeveloperExceptionPage();

app.UseMiddleware<TerminalConfigurationMiddleware>();

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
