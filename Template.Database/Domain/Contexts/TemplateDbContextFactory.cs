using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Template.Database.Domain.Contexts
{
    public class TemplateDbContextFactory : IDesignTimeDbContextFactory<TemplateDbContext>
    {
        public TemplateDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TemplateDbContext>();

            optionsBuilder.UseSqlServer(
                "Data Source=localhost;Initial Catalog=TemplateDb;Integrated Security=True;Encrypt=False;Trust Server Certificate=True");

            return new TemplateDbContext(optionsBuilder.Options);
        }
    }
}
