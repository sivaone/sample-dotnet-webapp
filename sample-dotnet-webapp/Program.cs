var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

// Intentional to test deployments
app.MapOpenApi();

// app.UseHttpsRedirection();
app.MapControllers();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "API V1");
});


app.Run();

public partial class Program
{
}