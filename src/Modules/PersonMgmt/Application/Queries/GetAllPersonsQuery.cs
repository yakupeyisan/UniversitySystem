using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;

namespace PersonMgmt.Application.Queries;

/// <summary>
/// Tüm kişileri getirme query
/// 
/// Kullanım:
/// var query = new GetAllPersonsQuery();
/// var result = await _mediator.Send(query);
/// </summary>
public class GetAllPersonsQuery : IRequest<Result<IEnumerable<PersonResponse>>>
{
    /// <summary>
    /// GetAllPersonsQuery Handler
    /// </summary>
    public class Handler : IRequestHandler<GetAllPersonsQuery, Result<IEnumerable<PersonResponse>>>
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

        public async Task<Result<IEnumerable<PersonResponse>>> Handle(GetAllPersonsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching all persons");

                // Repository'den kişileri getir
                var persons = await _personRepository.GetAllAsync(cancellationToken);

                // Response'a map et
                var responses = _mapper.Map<IEnumerable<PersonResponse>>(persons);

                _logger.LogInformation("Retrieved {Count} persons successfully", persons.Count());

                return Result<IEnumerable<PersonResponse>>.Success(responses, "Persons retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all persons");
                return Result<IEnumerable<PersonResponse>>.Failure(ex.Message);
            }
        }
    }
}