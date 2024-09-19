using Microsoft.EntityFrameworkCore;

namespace Eclipse.DataAccess.Model;

public interface IModelBuilderConfigurator
{
    void Configure(ModelBuilder modelBuilder);
}
