using Amazon.SQS;
using sample_dotnet_webapp.Services;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Only add SystemsManager config if not running in test environment
var isTestEnv = builder.Environment.EnvironmentName == "Test" ||
                Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == "Test" ||
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Test";
if (!isTestEnv)
{
    config.AddSystemsManager("/sample-dotnet-webapp/dev", false, TimeSpan.FromMinutes(5));
}

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddAWSService<IAmazonSQS>();
builder.Services.AddSingleton<SqsService>();

builder.Logging.SetMinimumLevel(LogLevel.Debug);

var sqsQueueUrl = Environment.GetEnvironmentVariable("SQS_QUEUE_URL");
Console.WriteLine($"SQS_QUEUE_URL: {sqsQueueUrl}");

var app = builder.Build();

// Intentional to test deployments
app.MapOpenApi();

// app.UseHttpsRedirection();
app.MapControllers();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "API V1");
});



app.MapHealthChecks("/health");
app.Run();

public partial class Program
{
}