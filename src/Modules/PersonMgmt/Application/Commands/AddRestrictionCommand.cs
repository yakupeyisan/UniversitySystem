using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;

namespace PersonMgmt.Application.Commands;

/// <summary>
/// Kişiye kısıtlama ekleme command
/// 
/// Kullanım:
/// var command = new AddRestrictionCommand(personId, appliedBy, new AddRestrictionRequest { ... });
/// var result = await _mediator.Send(command);
/// </summary>
public class AddRestrictionCommand : IRequest<Result<Unit>>
{
    /// <summary>
    /// Kişi ID
    /// </summary>
    public Guid PersonId { get; set; }

    /// <summary>
    /// Kısıtlamayı uygulayan (Admin/User ID)
    /// </summary>
    public Guid AppliedBy { get; set; }

    /// <summary>
    /// Request verisi
    /// </summary>
    public AddRestrictionRequest Request { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public AddRestrictionCommand(Guid personId, Guid appliedBy, AddRestrictionRequest request)
    {
        PersonId = personId;
        AppliedBy = appliedBy;
        Request = request;
    }

    /// <summary>
    /// AddRestrictionCommand Handler
    /// </summary>
    public class Handler : IRequestHandler<AddRestrictionCommand, Result<Unit>>
    {
        public readonly IRepository<Person> _personRepository;
        public readonly IMapper _mapper;
        public readonly ILogger<Handler> _logger;

        public Handler(IRepository<Person> personRepository, IMapper mapper, ILogger<Handler> logger)
        {
            _personRepository = personRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(AddRestrictionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Adding restriction to person with ID: {PersonId} by {AppliedBy}",
                    request.PersonId, request.AppliedBy);

                // Kişiyi ID'ye göre getir
                var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
                if (person == null)
                {
                    _logger.LogWarning("Person not found with ID: {PersonId}", request.PersonId);
                    return Result<Unit>.Failure($"Person with ID {request.PersonId} not found");
                }

                // ✅ byte'ları enum'a dönüştür
                var restrictionType = (RestrictionType)request.Request.RestrictionType;
                var restrictionLevel = (RestrictionLevel)request.Request.RestrictionLevel;

                // Kısıtlamayı ekle
                person.AddRestriction(
                    restrictionType: restrictionType,
                    restrictionLevel: restrictionLevel,
                    appliedBy: request.AppliedBy,
                    startDate: request.Request.StartDate,
                    endDate: request.Request.EndDate,
                    reason: request.Request.Reason,
                    severity: request.Request.Severity);

                // Repository'ye kaydet (UpdateAsync)
                await _personRepository.UpdateAsync(person, cancellationToken);

                _logger.LogInformation("Restriction added successfully to person with ID: {PersonId}", person.Id);

                return Result<Unit>.Success(Unit.Value, "Restriction added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding restriction to person with ID: {PersonId}", request.PersonId);
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}