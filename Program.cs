using Hangfire;
using Hangfire.MySql.Core;
using Hangfire_background_jobs.IServices;
using Hangfire_background_jobs.Services;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http;
using System.Security.Authentication;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


//Register hangfire services
builder.Services.AddHangfire(configuration =>
{
    configuration.UseStorage(
        new MySqlStorage(
            "server=localhost;port=3306;uid=root;pwd=sobiazafar@2023;database=hangfire ;Allow User Variables=True",
            new MySqlStorageOptions
            {
                TablePrefix = "Hangfire"
            }
        )
    );
});


//You have the option to implement factory class either by using Named clients or typed clients. 
// Register Poly Polices // AddHttpClient is used to add httpclient by named client

builder.Services.AddHttpClient("csharpcorner")
        //SetHandlerLifetime used to set lifetime of above added httpclient
        .SetHandlerLifetime(TimeSpan.FromMinutes(5))
        // important step  ->this line use to set retry polciy on above created httpclient
        .AddPolicyHandler(GetRetryPolicy());

builder.Services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
//GetRetryPolicy is defined to return retry poly poly policy such as 404 ,500 ,
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        // HttpRequestException, 5XX and 408  
        .HandleTransientHttpError()
        // 404  
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .OrResult(msg=>msg.StatusCode== System.Net.HttpStatusCode.InternalServerError)
        // Retry two times after delay  
        .WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromSeconds(5))
        // time stamp policy     
        ;
}


//ssl connection disable

HttpClientHandler clientHandler = new HttpClientHandler();
clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
HttpClient cleint =new HttpClient(clientHandler) ;
// bulkhead
// MaxParallelism is set to 3 which means the max number of concurrent actions handled through this policy.  

//MaxQueuingAction is set to 5 which means the max number of actions that may be queued. Waiting for execution slot.
var bulk = Policy.BulkheadAsync<HttpResponseMessage>(3, 5, x =>
{
    Console.WriteLine("rejected" + x.OperationKey);
    return Task.CompletedTask;
});
//Register Polly using typed cleint
builder.Services.AddHttpClient<IWeatherService, WeatherService>(httpleint =>
{
    httpleint.BaseAddress = new Uri("https://localhost:7230");
    httpleint.DefaultRequestHeaders.Add("Accept", "application/json");
}).ConfigurePrimaryHttpMessageHandler(() => clientHandler)
.AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(7, _ => TimeSpan.FromSeconds(2)))
.AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(5, TimeSpan.FromSeconds(10)))
.AddPolicyHandler(policy => bulk);

// ...



builder.Services.AddHangfireServer();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseHangfireDashboard();

app.UseAuthorization();

app.MapControllers();

app.Run();
