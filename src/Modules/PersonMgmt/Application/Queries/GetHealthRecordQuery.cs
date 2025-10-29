using AutoMapper;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;

namespace PersonMgmt.Application.Queries;

public class GetHealthRecordQuery : IRequest<Result<PersonResponse>>
{
    public GetHealthRecordQuery(Guid personId)
    {
        if (personId == Guid.Empty)
            throw new ArgumentException("Person ID cannot be empty", nameof(personId));
        PersonId = personId;
    }

    public Guid PersonId { get; set; }

    public class Handler : IRequestHandler<GetHealthRecordQuery, Result<PersonResponse>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;
        private readonly IPersonRepository _personRepository;

        public Handler(IPersonRepository personRepository, IMapper mapper, ILogger<Handler> logger)
        {
            _personRepository = personRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PersonResponse>> Handle(
            GetHealthRecordQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching health record for person: {PersonId}", request.PersonId);
                var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
                if (person == null)
                {
                    _logger.LogWarning("Person with ID {PersonId} not found", request.PersonId);
                    return Result<PersonResponse>.Failure(
                        $"Person with ID {request.PersonId} not found");
                }

                if (person.HealthRecord == null)
                {
                    _logger.LogWarning(
                        "Health record for person {PersonId} does not exist",
                        request.PersonId);
                    return Result<PersonResponse>.Failure(
                        $"Health record for person {request.PersonId} does not exist");
                }

                var response = _mapper.Map<PersonResponse>(person);
                _logger.LogInformation(
                    "Successfully retrieved health record for person: {PersonId}",
                    request.PersonId);
                return Result<PersonResponse>.Success(response, "Health record retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching health record for person: {PersonId}",
                    request.PersonId);
                return Result<PersonResponse>.Failure(ex.Message);
            }
        }
    }
}