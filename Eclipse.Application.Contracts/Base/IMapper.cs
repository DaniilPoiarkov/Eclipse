namespace Eclipse.Application.Contracts.Base;

/// <summary>
/// Simple class which contains logic to map one object to another
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TDto"></typeparam>
public interface IMapper<TEntity, TDto>
{
    /// <summary>
    /// Map value to specified data transfer object
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    TDto Map(TEntity value);
}
