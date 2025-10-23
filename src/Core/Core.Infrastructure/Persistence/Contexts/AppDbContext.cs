using Core.Domain.Specifications;
using Core.Infrastructure.Persistence.Configurations.PersonMgmt;
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
/// 
/// GÜNCELLENMIŞ: PersonMgmt Configuration'ları entegre edildi
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
    /// ✅ GÜNCELLENMIŞ: Tüm PersonMgmt configurations entegre edildi
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ==================== PersonMgmt Configurations ====================

        /// <summary>
        /// Person Configuration
        /// - Aggregate Root'un mapping'i
        /// - ValueObjects: PersonName, Address
        /// - Child entities relationships
        /// </summary>
        modelBuilder.ApplyConfiguration(new PersonConfiguration());

        /// <summary>
        /// Student Configuration
        /// - Shared Primary Key Pattern (Student.Id = Person.Id)
        /// - One-to-One Relationship
        /// - Enum conversions: EducationLevel, StudentStatus
        /// </summary>
        modelBuilder.ApplyConfiguration(new StudentConfiguration());

        /// <summary>
        /// Staff Configuration
        /// - Shared Primary Key Pattern (Staff.Id = Person.Id)
        /// - One-to-One Relationship
        /// - ValueObjects: Address, EmergencyContact
        /// - Enum conversion: AcademicTitle
        /// </summary>
        modelBuilder.ApplyConfiguration(new StaffConfiguration());

        /// <summary>
        /// HealthRecord Configuration
        /// - Independent Primary Key (HealthRecord.Id != Person.Id)
        /// - One-to-One Relationship (conceptual)
        /// - Hassas Tıbbi Veriler
        /// </summary>
        modelBuilder.ApplyConfiguration(new HealthRecordConfiguration());

        /// <summary>
        /// PersonRestriction Configuration
        /// - Independent Primary Key (PersonRestriction.Id != Person.Id)
        /// - One-to-Many Relationship
        /// - Enum conversions: RestrictionType, RestrictionLevel
        /// </summary>
        modelBuilder.ApplyConfiguration(new PersonRestrictionConfiguration());

        // ==================== Global Configurations ====================

        /// <summary>
        /// Soft Delete Query Filter
        /// - Tüm queries'de IsDeleted = false filtrelenmesi için
        /// - ISoftDelete interface'ini implement eden entities'e uygulanır
        /// </summary>
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

        // ==================== Entity-specific Global Configs ====================

        // ✅ Person Aggregate Constraints
        modelBuilder.Entity<Person>(entity =>
        {
            // Student ve Staff arasında ONE-TO-ONE constraint
            // Bir kişi hem student hem staff olamaz (logik)
            // Bu enforced edilemez SQL'de ama domain logic'de yapılır
        });

        // ✅ Student Specific Constraints
        modelBuilder.Entity<Student>(entity =>
        {
            // StudentNumber unique constraint zaten configuration'da var
            // CGPA ve SGPA range validation domain layer'da yapılır (0.0 - 4.0)
        });

        // ✅ Staff Specific Constraints
        modelBuilder.Entity<Staff>(entity =>
        {
            // EmployeeNumber unique constraint zaten configuration'da var
            // HireDate > DateTime.MinValue validation domain layer'da yapılır
        });

        // ✅ HealthRecord Specific Constraints
        modelBuilder.Entity<HealthRecord>(entity =>
        {
            // Medical data privacy considerations
            // Encryption at-rest should be configured at connection string level
        });

        // ✅ PersonRestriction Specific Constraints
        modelBuilder.Entity<PersonRestriction>(entity =>
        {
            // StartDate < EndDate (if EndDate != null) validation domain layer'da yapılır
            // Severity range (1-10) validation domain layer'da yapılır
        });
    }
}