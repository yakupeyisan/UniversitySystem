using Core.Application.Abstractions;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace PersonMgmt.Application.Commands;

public class RemoveRestrictionCommand : IRequest<Result<Unit>>
{
    public RemoveRestrictionCommand(Guid personId, Guid restrictionId)
    {
        PersonId = personId;
        RestrictionId = restrictionId;
    }

    public Guid PersonId { get; set; }
    public Guid RestrictionId { get; set; }

    public class Handler : IRequestHandler<RemoveRestrictionCommand, Result<Unit>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<Handler> _logger;
        private readonly IPersonRepository _personRepository;

        public Handler(IPersonRepository personRepository, ICurrentUserService currentUserService,
            ILogger<Handler> logger)
        {
            _personRepository = personRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(RemoveRestrictionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Removing restriction {RestrictionId} from person {PersonId} by user {UserId}",
                    request.RestrictionId,
                    request.PersonId,
                    _currentUserService.UserId);
                var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
                if (person == null)
                {
                    _logger.LogWarning("Person not found with ID: {PersonId}", request.PersonId);
                    return Result<Unit>.Failure("Person not found");
                }

                person.RemoveRestriction(request.RestrictionId, _currentUserService.UserId);
                await _personRepository.UpdateAsync(person, cancellationToken);
                await _personRepository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                    "Restriction {RestrictionId} removed successfully from person {PersonId}",
                    request.RestrictionId,
                    request.PersonId);
                return Result<Unit>.Success(Unit.Value, "Restriction removed successfully");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Restriction not found: {RestrictionId}", request.RestrictionId);
                return Result<Unit>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing restriction from person {PersonId}", request.PersonId);
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}