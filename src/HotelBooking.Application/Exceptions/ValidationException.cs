namespace HotelBooking.Application.Exceptions;

public class ValidationException : AppException
{
    public override int StatusCode => 400;
    public override string ErrorCode => "VALIDATION_ERROR";

    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("Виникли помилки валідації.")
    {
        Errors = errors;
    }

    public ValidationException(string propertyName, string errorMessage)
        : base("Виникли помилки валідації.")
    {
        Errors = new Dictionary<string, string[]>
        {
            { propertyName, [errorMessage] }
        };
    }
}