﻿namespace Eclipse.Infrastructure.Exceptions;

public class EclipseValidationException : EclipseException
{
    public EclipseValidationException(string? message = null, string? code = null)
        : base(message, code) { }
}
