namespace Chatly.Interfaces.Services;

public interface IPasswordFormatValidator
{
    bool IsValid(string? password, out List<string> errors);
}