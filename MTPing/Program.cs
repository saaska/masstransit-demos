using MassTransit;
using System.Diagnostics;
using MTPing.MessageContracts;

const int N = 1000;

var busControl = Bus.Factory.CreateUsingRabbitMq( cfg => 
    {
        cfg.Host("localhost", "/");
    });
var endpoint = busControl.GetSendEndpoint(new Uri($"exchange:MTPing.MessageContracts:PingMessage")).Result;

var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));
await busControl.StartAsync(source.Token);
Console.WriteLine($"MTPing: About to send {N} messages to queue");

var stopwatch = new Stopwatch();

stopwatch.Start();
var res = await SendPingsAsync(endpoint, N);
stopwatch.Stop();

await busControl.StopAsync();
var mps = (int)(1000.0 * N / stopwatch.ElapsedMilliseconds);
Console.WriteLine(res ? $"MTPing: Sent {N} pings in {stopwatch.ElapsedMilliseconds}ms, @{mps} msg/sec" 
                      : "MTPing: Failure");


async Task<bool> SendPingsAsync(ISendEndpoint sendEndpoint, int N)
{
    try
    {
        for (var i = 0; i < N; i++)
        {
            await sendEndpoint.Send<PingMessage>(new PingMessage { Title = "msg", SequenceNumber = i });
        }
    }
    catch
    {
        return false;
    }
    return true;
}
