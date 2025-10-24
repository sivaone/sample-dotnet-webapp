using Amazon.SQS;
using sample_dotnet_webapp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddAWSService<IAmazonSQS>();
builder.Services.AddSingleton<SqsService>();

builder.Logging.SetMinimumLevel(LogLevel.Debug);

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