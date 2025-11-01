using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Specifications;
namespace PersonMgmt.Application.Queries;
public class GetStaffByNumberQuery : IRequest<Result<PersonResponse>>
{
    public GetStaffByNumberQuery(string employeeNumber)
    {
        if (string.IsNullOrWhiteSpace(employeeNumber))
            throw new ArgumentException("Employee number cannot be empty", nameof(employeeNumber));
        EmployeeNumber = employeeNumber.Trim();
    }
    public string EmployeeNumber { get; set; }
    public class Handler : IRequestHandler<GetStaffByNumberQuery, Result<PersonResponse>>
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
            GetStaffByNumberQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching person by employee number: {EmployeeNumber}",
                    request.EmployeeNumber);
                var persons = await _personRepository.GetAllAsync(
                    new PersonByEmployeeNumberSpecification(request.EmployeeNumber),
                    cancellationToken);
                if (persons == null || persons.Count() == 0)
                {
                    _logger.LogWarning(
                        "Person with employee number {EmployeeNumber} not found",
                        request.EmployeeNumber);
                    return Result<PersonResponse>.Failure(
                        $"Person with employee number {request.EmployeeNumber} not found");
                }
                var person = persons.FirstOrDefault();
                if (person == null)
                    return Result<PersonResponse>.Failure(
                        $"Person with employee number {request.EmployeeNumber} not found");
                var response = _mapper.Map<PersonResponse>(person);
                _logger.LogInformation(
                    "Successfully retrieved person by employee number: {EmployeeNumber}",
                    request.EmployeeNumber);
                return Result<PersonResponse>.Success(response, "Person retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching person by employee number: {EmployeeNumber}",
                    request.EmployeeNumber);
                return Result<PersonResponse>.Failure(ex.Message);
            }
        }
    }
}