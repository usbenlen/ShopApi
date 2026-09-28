namespace Shop.Api.Exceptions;

public sealed class ValidationAppException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationAppException(IDictionary<string, string[]> errors) : base("One or more validation errors occurred")
    {
        Errors = new Dictionary<string, string[]>(errors);
    }
}