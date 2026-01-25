using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using WeatherApi.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(15));

var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .Or<TimeoutRejectedException>()
    .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
    .CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 5,
        durationOfBreak: TimeSpan.FromSeconds(30)
    );

builder.Services.AddHttpClient<WeatherApiService>()
    .AddPolicyHandler((serviceProvider, request) =>
    {
      var logger = serviceProvider.GetRequiredService<ILogger<WeatherApiService>>();
      return HttpPolicyExtensions
          .HandleTransientHttpError()
          .Or<TimeoutRejectedException>()
          .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
          .WaitAndRetryAsync(3,
              (retryAttempt, response, context) =>
              {
                if (response.Result?.StatusCode == HttpStatusCode.TooManyRequests &&
                      response.Result.Headers.TryGetValues("Retry-After", out var values) &&
                      int.TryParse(values.FirstOrDefault(), out var delaySeconds))
                {
                  return TimeSpan.FromSeconds(delaySeconds);
                }
                return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
              },
              (response, timespan, retryAttempt, context) =>
              {
                logger.LogWarning("Request to {RequestUri} failed with {StatusCode}. Delaying for {Timespan}, then making retry {RetryAttempt}.", request.RequestUri, response.Result?.StatusCode, timespan, retryAttempt);
                return Task.CompletedTask;
              });
    })
    .AddPolicyHandler(circuitBreakerPolicy)
    .AddPolicyHandler(timeoutPolicy);

builder.Services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
