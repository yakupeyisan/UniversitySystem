using Core.Domain.Specifications;
using Core.Infrastructure.Persistence.Configurations.PersonMgmt;
using Microsoft.EntityFrameworkCore;
using PersonMgmt.Domain.Aggregates;
namespace Core.Infrastructure.Persistence.Contexts;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Person> Persons { get; set; } = null!;
    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<Address> Addresses { get; set; } = null!;
    public DbSet<Staff> Staff { get; set; } = null!;
    public DbSet<HealthRecord> HealthRecords { get; set; } = null!;
    public DbSet<PersonRestriction> PersonRestrictions { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new PersonConfiguration());
        modelBuilder.ApplyConfiguration(new AddressConfiguration());
        modelBuilder.ApplyConfiguration(new StudentConfiguration());
        modelBuilder.ApplyConfiguration(new StaffConfiguration());
        modelBuilder.ApplyConfiguration(new HealthRecordConfiguration());
        modelBuilder.ApplyConfiguration(new EmergencyContactConfiguration());
        modelBuilder.ApplyConfiguration(new PersonRestrictionConfiguration());
        var deletableEntities = modelBuilder.Model
    .GetEntityTypes()
    .Where(t => typeof(ISoftDelete).IsAssignableFrom(t.ClrType));
        foreach (var entity in deletableEntities)
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(entity.ClrType);
            var property = System.Linq.Expressions.Expression.Property(parameter, "IsDeleted");
            var filter = System.Linq.Expressions.Expression.Lambda(
                System.Linq.Expressions.Expression.Equal(
                    property,
                    System.Linq.Expressions.Expression.Constant(false)
                ),
                parameter
            );
            modelBuilder.Entity(entity.ClrType).HasQueryFilter(filter);
        }
        modelBuilder.Entity<Person>(entity =>
        {
        });
        modelBuilder.Entity<Student>(entity =>
        {
        });
        modelBuilder.Entity<Staff>(entity =>
        {
        });
        modelBuilder.Entity<HealthRecord>(entity =>
        {
        });
        modelBuilder.Entity<PersonRestriction>(entity =>
        {
        });
    }
}