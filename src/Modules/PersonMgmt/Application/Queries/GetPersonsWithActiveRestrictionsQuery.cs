using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;

namespace PersonMgmt.Application.Queries;

/// <summary>
/// Aktif kısıtlamalarla kişileri getirme query
/// 
/// Kullanım:
/// var query = new GetPersonsWithActiveRestrictionsQuery();
/// var result = await _mediator.Send(query);
/// </summary>
public class GetPersonsWithActiveRestrictionsQuery : IRequest<Result<IEnumerable<PersonResponse>>>
{
    /// <summary>
    /// GetPersonsWithActiveRestrictionsQuery Handler
    /// </summary>
    public class Handler : IRequestHandler<GetPersonsWithActiveRestrictionsQuery, Result<IEnumerable<PersonResponse>>>
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

        public async Task<Result<IEnumerable<PersonResponse>>> Handle(GetPersonsWithActiveRestrictionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching persons with active restrictions");

                // Repository'den tüm kişileri getir
                var allPersons = await _personRepository.GetAllAsync(cancellationToken);

                // ✅ GetActiveRestrictions() kullanarak aktif kısıtlaması olanları filtrele
                var personsWithRestrictions = allPersons
                    .Where(p => p.GetActiveRestrictions().Any())
                    .ToList();

                // Response'a map et
                var responses = _mapper.Map<IEnumerable<PersonResponse>>(personsWithRestrictions);

                _logger.LogInformation("Retrieved {Count} persons with active restrictions successfully", personsWithRestrictions.Count);

                return Result<IEnumerable<PersonResponse>>.Success(responses, "Persons with active restrictions retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching persons with active restrictions");
                return Result<IEnumerable<PersonResponse>>.Failure(ex.Message);
            }
        }
    }
}