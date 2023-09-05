namespace Eclipse.Core.Validation;

public class ValidationContext
{
    public IServiceProvider ServiceProvider { get; }

    public ValidationContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
}
