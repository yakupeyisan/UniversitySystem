using Core.Application.Abstractions;
using Core.Domain;
using Core.Domain.Events;
using Core.Domain.Specifications;
using Shared.Infrastructure.Persistence.Configurations.PersonMgmt;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonMgmt.Domain.Aggregates;
using System;
using System.Linq.Expressions;

namespace Shared.Infrastructure.Persistence.Contexts;
public class AppDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IPublisher _mediator;
    private readonly IDateTime _dateTime;
    public AppDbContext(DbContextOptions<AppDbContext> options, 
        ICurrentUserService currentUserService, 
        IPublisher mediator, IDateTime dateTime) : base(options)
    {
        _currentUserService = currentUserService;
        _mediator = mediator;
        _dateTime = dateTime;
    }
    public DbSet<Person> Persons { get; set; } = null!;
    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<Address> Addresses { get; set; } = null!;
    public DbSet<Staff> Staff { get; set; } = null!;
    public DbSet<HealthRecord> HealthRecords { get; set; } = null!;
    public DbSet<PersonRestriction> PersonRestrictions { get; set; } = null!;
    public DbSet<EmergencyContact> EmergencyContacts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Ignore<DomainEvent>();
        modelBuilder.ApplyConfiguration(new PersonConfiguration());
        modelBuilder.ApplyConfiguration(new AddressConfiguration());
        modelBuilder.ApplyConfiguration(new StudentConfiguration());
        modelBuilder.ApplyConfiguration(new StaffConfiguration());
        modelBuilder.ApplyConfiguration(new HealthRecordConfiguration());
        modelBuilder.ApplyConfiguration(new EmergencyContactConfiguration());
        modelBuilder.ApplyConfiguration(new PersonRestrictionConfiguration());

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(GetSoftDeleteFilter(entityType.ClrType));
            }
        }
    }

    private static LambdaExpression GetSoftDeleteFilter(Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "e");
        var property = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
        var condition = Expression.Equal(property, Expression.Constant(false));
        return Expression.Lambda(condition, parameter);
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = _currentUserService.UserId;
                    entry.Entity.CreatedAt = _dateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedBy = _currentUserService.UserId;
                    entry.Entity.UpdatedAt = _dateTime.UtcNow;
                    break;

                case EntityState.Deleted:
                    if (entry.Entity is ISoftDelete softDeleteEntity)
                    {
                        entry.State = EntityState.Modified;
                        softDeleteEntity.Delete(_currentUserService.UserId);

                    }
                    break;
            }
        }

        // Domain events will be dispatched here
        await DispatchDomainEventsAsync(cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }
    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEntities = ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        // Domain event dispatching will be implemented with MediatR
        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }
    }


}