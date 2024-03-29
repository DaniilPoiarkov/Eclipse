﻿using Eclipse.Common.Specifications;

using System.Linq.Expressions;

namespace Eclipse.Common.Tests.Specifications.Utils;

internal sealed class XGreaterThanSpecification : Specification<TestObject>
{
    private readonly int _x;

    public XGreaterThanSpecification(int x)
    {
        _x = x;
    }

    public override Expression<Func<TestObject, bool>> IsSatisfied()
    {
        return testObject => testObject.X > _x;
    }
}
