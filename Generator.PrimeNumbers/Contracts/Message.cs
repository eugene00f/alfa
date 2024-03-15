namespace PrimeNumbersGenerator.Contracts;

public class Message
{
    public string Nickname { get; set; } = "Eugene00f";
    public int Number { get; set; }
    public DateTime GeneratedTime { get; set; }
    public DateTime PublishedTime { get; set; } = DateTime.UtcNow;

}