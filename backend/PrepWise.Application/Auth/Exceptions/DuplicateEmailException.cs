namespace PrepWise.Application.Auth.Exceptions;

public class DuplicateEmailException : Exception
{
    public DuplicateEmailException(string email) : base($"A user with email '{email}' already exists.")
    {
    }
}
