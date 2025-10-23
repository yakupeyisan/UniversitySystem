using Core.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using PersonMgmt.Domain.Aggregates;

namespace Core.Infrastructure.Persistence.Contexts;


/// <summary>
/// Application DbContext - TÜM Modüller için centralized
/// 
/// Sorumluluğu:
/// - Tüm modüllerin entities'lerini yönetmek
/// - Database mapping'ini tanımlamak
/// - Migrations'ı handle etmek
/// 
/// Kullanım:
/// services.AddDbContext<AppDbContext>(options =>
///     options.UseSqlServer(connectionString));
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Constructor
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // ==================== PersonMgmt Aggregates ====================

    /// <summary>
    /// Kişi (Person) aggregate root
    /// </summary>
    public DbSet<Person> Persons { get; set; } = null!;

    /// <summary>
    /// Öğrenci (Student) entity
    /// </summary>
    public DbSet<Student> Students { get; set; } = null!;

    /// <summary>
    /// Personel (Staff) entity
    /// </summary>
    public DbSet<Staff> Staff { get; set; } = null!;

    /// <summary>
    /// Sağlık kaydı (HealthRecord) entity
    /// </summary>
    public DbSet<HealthRecord> HealthRecords { get; set; } = null!;

    /// <summary>
    /// Kişi kısıtlaması (PersonRestriction) entity
    /// </summary>
    public DbSet<PersonRestriction> PersonRestrictions { get; set; } = null!;

    // ==================== TODO: Diğer Modüller ====================
    // Academic, VirtualPOS, AccessControl, Cafeteria, EventTicketing, 
    // Library, Parking, Payroll, Research, Wallet, vb. modüllerin 
    // aggregates'leri buraya eklenecek
    // Her modül için Pattern aynı:
    // public DbSet<[ModuleAggregate]> [ModuleAggregates] { get; set; } = null!;

    /// <summary>
    /// OnModelCreating - Entity configurations ve constraints
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ==================== PersonMgmt Configurations ====================

        // Person Configuration
        modelBuilder.ApplyConfiguration(new PersonConfiguration());

        // Student Configuration
        modelBuilder.ApplyConfiguration(new StudentConfiguration());

        // Staff Configuration
        modelBuilder.ApplyConfiguration(new StaffConfiguration());

        // HealthRecord Configuration
        modelBuilder.ApplyConfiguration(new HealthRecordConfiguration());

        // PersonRestriction Configuration
        modelBuilder.ApplyConfiguration(new PersonRestrictionConfiguration());

        // ==================== Global Configurations ====================

        // Soft Delete Query Filter
        // Tüm queries'de IsDeleted = false filtrelenmesi için
        var deletableEntities = modelBuilder.Model
            .GetEntityTypes()
            .Where(t => typeof(ISoftDelete).IsAssignableFrom(t.ClrType));

        foreach (var entity in deletableEntities)
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(entity.ClrType);
            var property = System.Linq.Expressions.Expression.Property(parameter, "IsDeleted");
            var filter = System.Linq.Expressions.Expression.Lambda(
                System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false)),
                parameter
            );

            modelBuilder.Entity(entity.ClrType).HasQueryFilter(filter);
        }
    }
}