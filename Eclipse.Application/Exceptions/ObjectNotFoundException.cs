namespace Eclipse.Application.Exceptions;

public class ObjectNotFoundException : ApplicationException
{
    internal ObjectNotFoundException(string name)
        : base($"Object {name} not found") { }
}
