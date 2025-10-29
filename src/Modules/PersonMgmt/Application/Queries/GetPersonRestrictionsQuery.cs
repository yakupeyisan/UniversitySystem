using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;

namespace PersonMgmt.Application.Queries;

public class GetPersonRestrictionsQuery : IRequest<Result<PersonResponse>>
{
    public GetPersonRestrictionsQuery(Guid personId, bool onlyActive = true)
    {
        if (personId == Guid.Empty)
            throw new ArgumentException("Person ID cannot be empty", nameof(personId));
        PersonId = personId;
        OnlyActive = onlyActive;
    }

    public Guid PersonId { get; set; }
    public bool OnlyActive { get; set; }

    public class Handler : IRequestHandler<GetPersonRestrictionsQuery, Result<PersonResponse>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IPersonRepository _personRepository;

        public Handler(IPersonRepository personRepository, ILogger<Handler> logger)
        {
            _personRepository = personRepository;
            _logger = logger;
        }

        public async Task<Result<PersonResponse>> Handle(
            GetPersonRestrictionsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching restrictions for person: {PersonId}, OnlyActive: {OnlyActive}",
                    request.PersonId,
                    request.OnlyActive);
                var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
                if (person == null)
                {
                    _logger.LogWarning("Person with ID {PersonId} not found", request.PersonId);
                    return Result<PersonResponse>.Failure(
                        $"Person with ID {request.PersonId} not found");
                }

                var restrictions = request.OnlyActive
                    ? person.GetActiveRestrictions().ToList()
                    : person.Restrictions.Where(r => !r.IsDeleted).ToList();
                if (restrictions.Count == 0)
                {
                    var message = request.OnlyActive
                        ? $"Person {request.PersonId} has no active restrictions"
                        : $"Person {request.PersonId} has no restrictions";
                    _logger.LogInformation(message);
                }

                _logger.LogInformation(
                    "Successfully retrieved {RestrictionCount} restrictions for person: {PersonId}",
                    restrictions.Count,
                    request.PersonId);
                var response = new PersonResponse
                {
                    Id = person.Id,
                    FirstName = person.Name.FirstName,
                    LastName = person.Name.LastName,
                    Email = person.Email,
                    PhoneNumber = person.PhoneNumber,
                    IdentificationNumber = person.IdentificationNumber
                };
                return Result<PersonResponse>.Success(
                    response,
                    $"Retrieved {restrictions.Count} restriction(s) for person");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching restrictions for person: {PersonId}",
                    request.PersonId);
                return Result<PersonResponse>.Failure(ex.Message);
            }
        }
    }
}