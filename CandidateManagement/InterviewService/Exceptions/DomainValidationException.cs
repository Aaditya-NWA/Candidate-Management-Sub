using System.Diagnostics.CodeAnalysis;

namespace InterviewService.Exceptions;

public class DomainValidationException : Exception
{
    [ExcludeFromCodeCoverage]
    public DomainValidationException(string message) : base(message)
    {
    }
}
