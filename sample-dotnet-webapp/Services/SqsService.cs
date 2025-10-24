using Amazon.SQS;
using Amazon.SQS.Model;

namespace sample_dotnet_webapp.Services;

public class SqsService(IAmazonSQS sqs, IConfiguration configuration)
{
    private readonly string _queueUrl = 
        configuration["AWS:SQS:QueueUrl"] ?? throw new ArgumentException("SQS QueueUrl is not configured.");

    public virtual async Task SendMessageAsync(string messageBody)
    {
        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = _queueUrl,
            MessageBody = messageBody
        };

        var response = await sqs.SendMessageAsync(sendMessageRequest);
        Console.WriteLine($"Message sent to SQS queue. MessageId: {response.MessageId}");
    }
}