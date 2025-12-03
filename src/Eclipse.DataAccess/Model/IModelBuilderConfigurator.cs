using Microsoft.EntityFrameworkCore;

namespace Eclipse.DataAccess.Model;

internal interface IModelBuilderConfigurator
{
    void Configure(ModelBuilder modelBuilder);
}
