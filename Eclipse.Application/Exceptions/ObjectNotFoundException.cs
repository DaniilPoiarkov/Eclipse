namespace Eclipse.Application.Exceptions;

internal class ObjectNotFoundException : Exception
{
    internal ObjectNotFoundException(string name) : base($"Object {name} not found")
    {
    }
}
