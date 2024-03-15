using MediatR;
using ProcessorPrimeNumbers.Contracts;

namespace ProcessorPrimeNumbers.Adapters.Commands;

public class SavePrimeNumber: IRequest
{
    public Message Message { get; set; }

    public SavePrimeNumber(Message message)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }
}
