using MassTransit;
using MTPing.MessageContracts;

const string queueName = "test"; 

var busControl = Bus.Factory.CreateUsingRabbitMq( cfg => 
{
    cfg.Host("localhost", "/");
    cfg.ReceiveEndpoint(queueName, e =>
        {
            e.Bind<PingMessage>();
            e.Consumer(() => new PingConsumer());
        });
    
});

await busControl.StartAsync();
Console.WriteLine($"MTPong ready. Listening on queue \"{queueName}\", press a key to exit");
await Task.Run(() => Console.ReadKey());
await busControl.StopAsync();

public class PingConsumer : IConsumer<PingMessage>
{
    public async Task Consume(ConsumeContext<PingMessage> ctx)
    {
        Console.WriteLine($"Received msg with sequence number {ctx.Message.SequenceNumber}.");
    }
}
