﻿using System.Linq.Expressions;

namespace Eclipse.Domain.Shared.Specifications;

/// <summary>
/// Use specifications to avoid writing same linq queries
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Specification<T>
{
    public abstract Expression<Func<T, bool>> IsSatisfied();

    public static implicit operator Expression<Func<T, bool>>(Specification<T> specification) => specification.IsSatisfied();

    public static implicit operator Func<T, bool>(Specification<T> specification) => specification.IsSatisfied().Compile();
}
