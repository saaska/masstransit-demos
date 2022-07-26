using MassTransit;
using System.Diagnostics;
using MTPing.MessageContracts;

const int N = 20;
const int TIMEOUT_MS = 4000;

const string queueName = "pong"; 

var busControl = Bus.Factory.CreateUsingRabbitMq( cfg => 
    {
        cfg.Host("localhost", "/");
        cfg.ReceiveEndpoint(queueName, e =>
            {
                e.Bind<PongMessage>();
                e.Consumer(() => new PongConsumer());
            });
    });
// EndpointConvention.Map<PingMessage>(new Uri("exchange:MTPing.MessageContracts:PingMessage"));
// EndpointConvention.Map<PongMessage>(new Uri("exchange:MTPong.MessageContracts:PongMessage"));

var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));
await busControl.StartAsync(source.Token);
Console.WriteLine($"MTPing: About to send {N} messages to queue");

var stopwatch = new Stopwatch();

stopwatch.Start();
var res = await SendPingsAsync(busControl, N);
stopwatch.Stop();

await busControl.StopAsync();
var mps = (int)(1000.0 * N / stopwatch.ElapsedMilliseconds);
Console.WriteLine(res ? $"MTPing: Sent {N} pings in {stopwatch.ElapsedMilliseconds}ms, @{mps} msg/sec" 
                      : "MTPing: Failure");

await Task.Delay(TIMEOUT_MS);

async Task<bool> SendPingsAsync(IBus _bus, int N)
{
    try
    {
        for (var i = 0; i < N; i++)
        {
            await _bus.Send(new PingMessage
            {
                Title = "msg", SequenceNumber = i
            });
            Console.WriteLine($"Ping, seq={i}");
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return false;
    }
    
    return true;
}

public class PongConsumer : IConsumer<PongMessage>
{
    public PongConsumer()
    {
    }
    public async Task Consume(ConsumeContext<PongMessage> ctx)
    {
        var dt = "";
        
        dt += ctx.Message.PingTimeInTransit != null 
            ? $", forward time {ctx.Message.PingTimeInTransit?.TotalMilliseconds:0.#}ms" : "";
        dt += ctx.Message.PingSentTime != null
            ? $", roundtrip time = {(DateTime.Now - ctx.Message.PingSentTime).Value.TotalMilliseconds:0#}" : "";
        
        await Task.Run( () => Console.WriteLine($"Pong, seq={ctx.Message.SequenceNumber}{dt}"));
    }
}