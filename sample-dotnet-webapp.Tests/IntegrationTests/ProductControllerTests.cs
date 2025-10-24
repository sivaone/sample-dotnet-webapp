using System.Net;
using System.Net.Http.Json;
using Amazon.SQS;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using sample_dotnet_webapp.Services;

namespace sample_dotnet_webapp.Tests.IntegrationTests;

public class ProductControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public ProductControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private HttpClient CreateHttpsClient() => _factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        BaseAddress = new Uri("https://localhost") // avoid HTTPS redirect from app.UseHttpsRedirection()
    });

    public record Product(int Id, string Name, string Description, decimal Price);

    private static WebApplicationFactory<Program> CreateFactoryWithFakeSqs(WebApplicationFactory<Program> factory)
    {
        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var sqsMock = new Mock<IAmazonSQS>();
                sqsMock.Setup(s => s.SendMessageAsync(
                    It.IsAny<Amazon.SQS.Model.SendMessageRequest>(),
                    It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new Amazon.SQS.Model.SendMessageResponse { MessageId = "test-message-id" });
                services.AddSingleton(sqsMock.Object);
                var config = new ConfigurationBuilder().AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {"AWS:SQS:QueueUrl", "http://localhost:4566/000000000000/test-queue"}
                    }!).Build();
                services.AddSingleton<IConfiguration>(config);
                services.AddSingleton<SqsService>();
            });
        });
    }

    [Fact]
    public async Task Create_Returns201_With_Location_And_Body()
    {
        var factoryWithFakeSqs = CreateFactoryWithFakeSqs(_factory);
        var client = factoryWithFakeSqs.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var request = new
        {
            name = "Coffee Mug",
            description = "Great white mug",
            price = 9.99m
        };

        var response = await client.PostAsJsonAsync("/api/products", request);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Headers.Location.ShouldNotBeNull();

        var product = await response.Content.ReadFromJsonAsync<Product>();
        product.ShouldNotBeNull();
        product.Id.ShouldBeGreaterThan(0);
        product.Name.ShouldBe(request.name);
        product.Description.ShouldBe(request.description);
        product.Price.ShouldBe(request.price);

        // Location should point to the created product
        response.Headers.Location!.AbsolutePath.ShouldBe($"/api/products/{product.Id}");
    }

    [Fact]
    public async Task GetById_Returns_Created_Product()
    {
        var factoryWithFakeSqs = CreateFactoryWithFakeSqs(_factory);
        var client = factoryWithFakeSqs.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        // First create a product
        var createResponse = await client.PostAsJsonAsync("/api/products", new
        {
            name = "Notebook",
            description = "A ruled notebook",
            price = 4.50m
        });
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<Product>();
        created.ShouldNotBeNull();

        // Then get it by id
        var getResponse = await client.GetAsync($"/api/products/{created.Id}");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var fetched = await getResponse.Content.ReadFromJsonAsync<Product>();
        fetched.ShouldBeEquivalentTo(created);
    }

    [Fact]
    public async Task GetById_Unknown_Returns404()
    {
        var factoryWithFakeSqs = CreateFactoryWithFakeSqs(_factory);
        var client = factoryWithFakeSqs.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var response = await client.GetAsync("/api/products/999999");
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_DuplicateId_Returns409()
    {
        var factoryWithFakeSqs = CreateFactoryWithFakeSqs(_factory);
        var client = factoryWithFakeSqs.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        // Create with explicit id unlikely to collide across runs
        var first = await client.PostAsJsonAsync("/api/products", 
            new { id = 123456, name = "Pen", description = "Blue pen", price = 1.25m });
        first.StatusCode.ShouldBeOneOf(HttpStatusCode.Created, HttpStatusCode.Conflict);

        // Try to create again with the same id
        var second = await client.PostAsJsonAsync("/api/products", 
            new { id = 123456, name = "Pen 2", description = "Blue pen 2", price = 1.30m });
        second.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
