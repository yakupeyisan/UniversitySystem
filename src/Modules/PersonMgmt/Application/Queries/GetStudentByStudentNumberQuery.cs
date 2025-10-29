using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;

namespace PersonMgmt.Application.Queries;

public class GetStudentByStudentNumberQuery : IRequest<Result<PersonResponse>>
{
    public GetStudentByStudentNumberQuery(string studentNumber)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
            throw new ArgumentException("Student number cannot be empty", nameof(studentNumber));
        StudentNumber = studentNumber.Trim();
    }

    public string StudentNumber { get; set; }

    public class Handler : IRequestHandler<GetStudentByStudentNumberQuery, Result<PersonResponse>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<Person> _personRepository;

        public Handler(IRepository<Person> personRepository, IMapper mapper, ILogger<Handler> logger)
        {
            _personRepository = personRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PersonResponse>> Handle(
            GetStudentByStudentNumberQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching person by student number: {StudentNumber}",
                    request.StudentNumber);
                var student = await _personRepository.GetStudentByStudentNumberAsync(
                    request.StudentNumber,
                    cancellationToken);
                if (student == null)
                    return Result<PersonResponse>.Failure(
                        $"Person with student number {request.StudentNumber} not found");
                var response = _mapper.Map<PersonResponse>(student);
                _logger.LogInformation(
                    "Successfully retrieved person by student number: {StudentNumber}",
                    request.StudentNumber);
                return Result<PersonResponse>.Success(response, "Person retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching person by student number: {StudentNumber}",
                    request.StudentNumber);
                return Result<PersonResponse>.Failure(ex.Message);
            }
        }
    }
}