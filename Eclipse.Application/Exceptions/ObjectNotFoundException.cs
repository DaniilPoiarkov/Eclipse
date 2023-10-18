namespace Eclipse.Application.Exceptions;

public class ObjectNotFoundException : ApplicationException
{
    public ObjectNotFoundException(string name)
        : base($"Object {name} not found") { }
}
