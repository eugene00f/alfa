namespace ProcessorPrimeNumbers.Contracts;

public class Message
{
    public string? Nickname { get; set; }
    public int Number { get; set; }
    public DateTime GeneratedTime { get; set; }
    public DateTime PublishedTime { get; set; }

}