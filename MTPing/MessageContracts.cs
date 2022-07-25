namespace MTPing.MessageContracts;
public record PingMessage
{
    public string? Title { get; set; }
    public int SequenceNumber { get; set;}
}