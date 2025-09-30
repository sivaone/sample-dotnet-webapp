var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

// Intentional to test deployments
app.MapOpenApi();

app.UseHttpsRedirection();
app.MapControllers();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "API V1");
});

// TODO: Test rulesets, remove later
var num = 100;

app.Run();

public partial class Program
{
}