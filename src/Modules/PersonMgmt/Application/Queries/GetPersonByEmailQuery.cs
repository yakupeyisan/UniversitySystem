using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Specifications;

namespace PersonMgmt.Application.Queries;

public class GetPersonByEmailQuery : IRequest<Result<PersonResponse>>
{
    public GetPersonByEmailQuery(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        Email = email.Trim().ToLower();
    }

    public string Email { get; set; }

    public class Handler : IRequestHandler<GetPersonByEmailQuery, Result<PersonResponse>>
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

        public async Task<Result<PersonResponse>> Handle(
            GetPersonByEmailQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching person by email: {Email}", request.Email);
                var person =
                    await _personRepository.GetAsync(new PersonByEmailSpecification(request.Email), cancellationToken);
                if (person == null)
                {
                    _logger.LogWarning("Person with email {Email} not found", request.Email);
                    return Result<PersonResponse>.Failure($"Person with email {request.Email} not found");
                }

                var response = _mapper.Map<PersonResponse>(person);
                _logger.LogInformation("Successfully retrieved person by email: {Email}", request.Email);
                return Result<PersonResponse>.Success(response, "Person retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching person by email: {Email}", request.Email);
                return Result<PersonResponse>.Failure(ex.Message);
            }
        }
    }
}