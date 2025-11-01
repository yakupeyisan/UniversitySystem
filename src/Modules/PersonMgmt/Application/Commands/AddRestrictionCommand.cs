using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;
namespace PersonMgmt.Application.Commands;
public class AddRestrictionCommand : IRequest<Result<Unit>>
{
    public AddRestrictionCommand(Guid personId, Guid appliedBy, AddRestrictionRequest request)
    {
        PersonId = personId;
        AppliedBy = appliedBy;
        Request = request;
    }
    public Guid PersonId { get; set; }
    public Guid AppliedBy { get; set; }
    public AddRestrictionRequest Request { get; set; }
    public class Handler : IRequestHandler<AddRestrictionCommand, Result<Unit>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<Person>
            _personRepository;
        public Handler(IRepository<Person>
            personRepository, IMapper mapper, ILogger<Handler> logger)
        {
            _personRepository = personRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<Result<Unit>> Handle(
            AddRestrictionCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Adding restriction to person with ID: {PersonId} by {AppliedBy}",
                    request.PersonId, request.AppliedBy);
                var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
                if (person == null)
                {
                    _logger.LogWarning("Person not found with ID: {PersonId}", request.PersonId);
                    return Result<Unit>.Failure("Person not found");
                }
                if (request.Request.EndDate.HasValue &&
                    request.Request.EndDate <= request.Request.StartDate)
                {
                    _logger.LogWarning("EndDate must be after StartDate");
                    return Result<Unit>.Failure("End date must be after start date");
                }
                var restrictionType = (RestrictionType)request.Request.RestrictionType;
                var restrictionLevel = (RestrictionLevel)request.Request.RestrictionLevel;
                person.AddRestriction(
                    restrictionType,
                    restrictionLevel,
                    request.AppliedBy,
                    request.Request.StartDate,
                    request.Request.EndDate,
                    request.Request.Reason,
                    request.Request.Severity
                );
                await _personRepository.UpdateAsync(person, cancellationToken);
                await _personRepository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                    "Restriction of type {RestrictionType} added to person with ID {PersonId}",
                    restrictionType, request.PersonId);
                return Result<Unit>.Success(Unit.Value, "Restriction added successfully");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Validation error while adding restriction");
                return Result<Unit>.Failure($"Validation error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding restriction to person with ID: {PersonId}",
                    request.PersonId);
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}