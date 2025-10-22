using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;

namespace PersonMgmt.Application.Queries;

/// <summary>
/// ID'ye göre kişi getirme query
/// 
/// Kullanım:
/// var query = new GetPersonQuery(personId);
/// var result = await _mediator.Send(query);
/// </summary>
public class GetPersonQuery : IRequest<Result<PersonResponse>>
{
    /// <summary>
    /// Kişi ID
    /// </summary>
    public Guid PersonId { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public GetPersonQuery(Guid personId)
    {
        PersonId = personId;
    }

    /// <summary>
    /// GetPersonQuery Handler
    /// </summary>
    public class Handler : IRequestHandler<GetPersonQuery, Result<PersonResponse>>
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

        public async Task<Result<PersonResponse>> Handle(GetPersonQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching person with ID: {PersonId}", request.PersonId);

                // Repository'den kişiyi getir
                var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
                if (person == null)
                {
                    _logger.LogWarning("Person not found with ID: {PersonId}", request.PersonId);
                    return Result<PersonResponse>.Failure($"Person with ID {request.PersonId} not found");
                }

                // Response'a map et
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