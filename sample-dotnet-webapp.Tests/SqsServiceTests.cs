using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using Moq;
using sample_dotnet_webapp.Services;
using Xunit;

public class SqsServiceTests
{
    [Fact]
    public async Task SendMessageAsync_CallsSqsWithCorrectParameters()
    {
        // Arrange
        var queueUrl = "http://localhost:4566/000000000000/test-queue";
        var messageBody = "test-message";

        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["AWS:SQS:QueueUrl"]).Returns(queueUrl);

        var sqsMock = new Mock<IAmazonSQS>();
        sqsMock.Setup(s => s.SendMessageAsync(
            It.Is<SendMessageRequest>(r => r.QueueUrl == queueUrl && r.MessageBody == messageBody),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendMessageResponse { MessageId = "msg-123" })
            .Verifiable();

        var service = new SqsService(sqsMock.Object, configMock.Object);

        // Act
        await service.SendMessageAsync(messageBody);

        // Assert
        sqsMock.Verify();
    }
}

