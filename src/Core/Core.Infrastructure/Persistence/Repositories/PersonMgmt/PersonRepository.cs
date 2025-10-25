using Core.Domain.Specifications;
using Core.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Interfaces;
using PersonMgmt.Domain.Specifications;

namespace Core.Infrastructure.Persistence.Repositories.PersonMgmt;
public class PersonRepository : GenericRepository<Person>, IPersonRepository
{
    private readonly AppDbContext _context;

    public PersonRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    #region Identification & Email Lookups

    public async Task<Person?> GetByIdentificationNumberAsync(
        string identificationNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.Persons
            .Where(p => !p.IsDeleted)
            .Include(p => p.Student)
            .Include(p => p.Staff)
            .Include(p => p.HealthRecord)
            .FirstOrDefaultAsync(
                p => p.IdentificationNumber == identificationNumber,
                cancellationToken);
    }

    public async Task<Person?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await _context.Persons
            .Where(p => !p.IsDeleted)
            .Include(p => p.Student)
            .Include(p => p.Staff)
            .Include(p => p.HealthRecord)
            .FirstOrDefaultAsync(
                p => p.Email == email,
                cancellationToken);
    }

    public async Task<Person?> GetByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.Persons
            .Where(p => !p.IsDeleted)
            .Include(p => p.Student)
            .Include(p => p.Staff)
            .Include(p => p.HealthRecord)
            .FirstOrDefaultAsync(
                p => p.PhoneNumber == phoneNumber,
                cancellationToken);
    }

    #endregion

    #region Search Methods

    public async Task<ICollection<Person>> SearchByNameAsync(
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default)
    {
        // ✅ Use specification pattern
        var spec = new PersonsByNameSearchSpecification(firstName, lastName);
        var query = ApplySpecification(spec);

        return await query.ToListAsync(cancellationToken);
    }

    #endregion

    #region Student Methods

    public async Task<Student?> GetStudentByStudentNumberAsync(
        string studentNumber,
        CancellationToken cancellationToken = default)
    {
        var person = await _context.Persons
            .Where(p => !p.IsDeleted)
            .Include(p => p.Student)
            .FirstOrDefaultAsync(
                p => p.Student != null && p.Student.StudentNumber == studentNumber,
                cancellationToken);

        return person?.Student;
    }

    public async Task<ICollection<Student>> GetStudentsByProgramAsync(
        Guid programId,
        CancellationToken cancellationToken = default)
    {
        // ✅ Use specification pattern
        var spec = new StudentsByProgramSpecification(programId);
        var query = ApplySpecification(spec);

        return await query
            .Select(p => p.Student!)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<Student>> GetStudentsByGpaRangeAsync(
        double minGpa,
        double maxGpa,
        CancellationToken cancellationToken = default)
    {
        // ✅ Use specification pattern
        var spec = new StudentsByGpaRangeSpecification(minGpa, maxGpa);
        var query = ApplySpecification(spec);

        return await query
            .Select(p => p.Student!)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Staff Methods

    public async Task<ICollection<Staff>> GetStaffByDepartmentAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default)
    {
        // ✅ Use specification pattern
        var spec = new StaffByDepartmentSpecification(departmentId);
        var query = ApplySpecification(spec);

        return await query
            .Select(p => p.Staff!)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<Staff>> GetStaffByPositionAsync(
        string position,
        CancellationToken cancellationToken = default)
    {
        // ✅ Use specification pattern
        var spec = new StaffByPositionSpecification(position);
        var query = ApplySpecification(spec);

        return await query
            .Select(p => p.Staff!)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Restriction Methods

    public async Task<ICollection<PersonRestriction>> GetActiveRestrictionsAsync(
        Guid personId,
        CancellationToken cancellationToken = default)
    {
        // ✅ Query active restrictions directly
        return await _context.Persons
            .Where(p => p.Id == personId && !p.IsDeleted)
            .SelectMany(p => p.Restrictions
                .Where(r => !r.IsDeleted &&
                           r.IsActive &&
                           r.StartDate <= DateTime.UtcNow &&
                           (r.EndDate == null || r.EndDate >= DateTime.UtcNow)))
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Health Record Methods

    public async Task<ICollection<HealthRecord>> GetHealthRecordsByDateRangeAsync(
        Guid personId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _context.Persons
            .Where(p => p.Id == personId && !p.IsDeleted)
            .Where(p => p.HealthRecord != null)
            .Select(p => p.HealthRecord!)
            .Where(h => h.CreatedAt >= startDate && h.CreatedAt <= endDate)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Uniqueness Validation

    public async Task<bool> IsIdentificationNumberUniqueAsync(
        string identificationNumber,
        CancellationToken cancellationToken = default)
    {
        return !await _context.Persons
            .Where(p => !p.IsDeleted)
            .AnyAsync(
                p => p.IdentificationNumber == identificationNumber,
                cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(
        string email,
        Guid? excludeId = null, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.Persons
            .Where(p => !p.IsDeleted)
            .Where(p => p.Email == email);

        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> IsEmployeeNumberUniqueAsync(
        string employeeNumber,
        CancellationToken cancellationToken = default)
    {
        return !await _context.Persons
            .Where(p => !p.IsDeleted)
            .Include(p => p.Staff)
            .AnyAsync(
                p => p.Staff != null && p.Staff.EmployeeNumber == employeeNumber,
                cancellationToken);
    }

    public async Task<bool> IsStudentNumberUniqueAsync(
        string studentNumber,
        CancellationToken cancellationToken = default)
    {
        return !await _context.Persons
            .Where(p => !p.IsDeleted)
            .Include(p => p.Student)
            .AnyAsync(
                p => p.Student != null && p.Student.StudentNumber == studentNumber,
                cancellationToken);
    }

    #endregion

    #region Unit of Work Pattern

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region Helper Method

    /// <summary>
    /// Helper method to apply specification (from GenericRepository).
    /// Used by derived repository methods.
    /// </summary>
    private IQueryable<Person> ApplySpecification(ISpecification<Person> spec)
    {
        var query = _context.Set<Person>().AsQueryable();

        // Apply SplitQuery if enabled
        if (spec.IsSplitQuery)
            query = query.AsSplitQuery();

        // Apply criteria
        if (spec.Criteria != null)
            query = query.Where(spec.Criteria);

        // Apply expression-based includes
        if (spec.Includes != null && spec.Includes.Any())
        {
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
        }

        // Apply string-based includes
        if (spec.IncludeStrings != null && spec.IncludeStrings.Any())
        {
            query = spec.IncludeStrings.Aggregate(query, (current, includeString) => current.Include(includeString));
        }

        // Apply ordering
        if (spec.OrderBys != null && spec.OrderBys.Any())
        {
            foreach (var (orderExpression, isDescending) in spec.OrderBys)
            {
                query = isDescending
                    ? query.OrderByDescending(orderExpression)
                    : query.OrderBy(orderExpression);
            }
        }

        // Apply paging
        if (spec.IsPagingEnabled)
        {
            if (spec.Skip.HasValue)
                query = query.Skip(spec.Skip.Value);

            if (spec.Take.HasValue)
                query = query.Take(spec.Take.Value);
        }

        return query;
    }

    #endregion
}


