using MassTransit;
using MTPing.MessageContracts;

const string queueName = "ping"; 

var busControl = Bus.Factory.CreateUsingRabbitMq( cfg => 
    {
        cfg.Host("localhost", "/");
        cfg.ReceiveEndpoint(queueName, e =>
            {
                e.Bind<PingMessage>();
                e.Consumer(() => new PingConsumer());
            });
    });
EndpointConvention.Map<PongMessage>(new Uri("exchange:MTPing.MessageContracts:PongMessage"));

var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));
await busControl.StartAsync(source.Token);
Console.WriteLine($"MTPong ready. Listening on queue \"{queueName}\", press a key to exit");
await Task.Run(() => Console.ReadKey());
await busControl.StopAsync();

public class PingConsumer : IConsumer<PingMessage>
{
    public async Task Consume(ConsumeContext<PingMessage> ctx)
    {
        var dt = DateTime.UtcNow - ctx.SentTime;
        await ctx.Send<PongMessage>(new PongMessage
        {
            SequenceNumber = ctx.Message.SequenceNumber,
            Title = ctx.Message.Title,
            PingSentTime = ctx.SentTime, 
            PingTimeInTransit = dt
        });
        var elapsed = dt != null? $", time in transit = {dt?.TotalMilliseconds:0.#}ms" : "";
        await Task.Run(() => Console.WriteLine($"Got ping msg with sequence number {ctx.Message.SequenceNumber}{elapsed}."));
    }
}
