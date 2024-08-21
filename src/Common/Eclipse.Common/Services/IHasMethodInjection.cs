namespace Eclipse.Common.Services;

public interface IHasMethodInjection<T>
{
    void Set(T instance);
}
