using AutoMapper;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Interfaces;
namespace PersonMgmt.Application.Queries;

public class GetPersonQuery : IRequest<Result<PersonResponse>>
{
    public Guid PersonId { get; set; }

    public GetPersonQuery(Guid personId)
    {
        PersonId = personId;
    }

    public class Handler : IRequestHandler<GetPersonQuery, Result<PersonResponse>>
    {
        private readonly IPersonRepository _personRepository;  // ✅ FIXED: IRepository<Person> → IPersonRepository
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(IPersonRepository personRepository, IMapper mapper, ILogger<Handler> logger)
        {
            _personRepository = personRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PersonResponse>> Handle(
            GetPersonQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching person with ID: {PersonId}", request.PersonId);

                var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
                if (person == null)
                {
                    _logger.LogWarning("Person not found with ID: {PersonId}", request.PersonId);
                    return Result<PersonResponse>.Failure($"Person with ID {request.PersonId} not found");
                }

                var response = _mapper.Map<PersonResponse>(person);
                _logger.LogInformation("Retrieved person successfully with ID: {PersonId}", person.Id);
                return Result<PersonResponse>.Success(response, "Person retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching person with ID: {PersonId}", request.PersonId);
                return Result<PersonResponse>.Failure(ex.Message);
            }
        }
    }
}


