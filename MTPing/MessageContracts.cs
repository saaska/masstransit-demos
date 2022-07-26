namespace MTPing.MessageContracts;
public record PingMessage
{
    public string? Title { get; set; }
    public int SequenceNumber { get; set;}
}

public record PongMessage
{
    public string? Title { get; set; }
    public int SequenceNumber { get; set; }
    public DateTime? PingSentTime { get; set; }
    public TimeSpan? PingTimeInTransit { get; set; }
}
