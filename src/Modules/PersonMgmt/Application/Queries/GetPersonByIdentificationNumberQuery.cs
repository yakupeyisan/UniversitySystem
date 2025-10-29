using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Specifications;

namespace PersonMgmt.Application.Queries;

public class GetPersonByIdentificationNumberQuery : IRequest<Result<PersonResponse>>
{
    public GetPersonByIdentificationNumberQuery(string identificationNumber)
    {
        if (string.IsNullOrWhiteSpace(identificationNumber))
            throw new ArgumentException("Identification number cannot be empty", nameof(identificationNumber));
        IdentificationNumber = identificationNumber.Trim();
    }

    public string IdentificationNumber { get; set; }

    public class Handler : IRequestHandler<GetPersonByIdentificationNumberQuery, Result<PersonResponse>>
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
            GetPersonByIdentificationNumberQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching person by identification number: {IdentificationNumber}",
                    request.IdentificationNumber);
                var person = await _personRepository.GetAsync(
                    new PersonByIdentificationNumberSpecification(request.IdentificationNumber),
                    cancellationToken);
                if (person == null)
                {
                    _logger.LogWarning(
                        "Person with identification number {IdentificationNumber} not found",
                        request.IdentificationNumber);
                    return Result<PersonResponse>.Failure(
                        $"Person with identification number {request.IdentificationNumber} not found");
                }

                var response = _mapper.Map<PersonResponse>(person);
                _logger.LogInformation(
                    "Successfully retrieved person by identification number: {IdentificationNumber}",
                    request.IdentificationNumber);
                return Result<PersonResponse>.Success(response, "Person retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching person by identification number: {IdentificationNumber}",
                    request.IdentificationNumber);
                return Result<PersonResponse>.Failure(ex.Message);
            }
        }
    }
}