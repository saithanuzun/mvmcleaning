using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace mvmclean.backend.Infrastructure.Persistence;

public class MVMdbContextFactory : IDesignTimeDbContextFactory<MVMdbContext>
{
    public MVMdbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MVMdbContext>();

        optionsBuilder.UseNpgsql(
            "User ID=postgres;Password=password123;Server=localhost;Port=5432;Database=MVMTest;Pooling=true",
            o => o.EnableRetryOnFailure()
        );

        return new MVMdbContext(optionsBuilder.Options);
    }
}
