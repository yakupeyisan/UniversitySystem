using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;
using PersonMgmt.Domain.Interfaces;
namespace PersonMgmt.Application.Commands;
public class EnrollStudentCommand : IRequest<Result<Unit>>
{
    public Guid PersonId { get; set; }
    public EnrollStudentRequest Request { get; set; }
    public EnrollStudentCommand(Guid personId, EnrollStudentRequest request)
    {
        PersonId = personId;
        Request = request;
    }
    public class Handler : IRequestHandler<EnrollStudentCommand, Result<Unit>>
    {
        private readonly IPersonRepository _personRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;
        public Handler(IPersonRepository personRepository, IMapper mapper, ILogger<Handler> logger)
        {
            _personRepository = personRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<Result<Unit>> Handle(
            EnrollStudentCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Enrolling student for person with ID: {PersonId}", request.PersonId);
                var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
                if (person == null)
                {
                    _logger.LogWarning("Person not found with ID: {PersonId}", request.PersonId);
                    return Result<Unit>.Failure("Person not found");
                }
                var isStudentNumberUnique = await _personRepository.IsStudentNumberUniqueAsync(
                    request.Request.StudentNumber, cancellationToken);
                if (!isStudentNumberUnique)
                {
                    _logger.LogWarning("Student number already exists: {StudentNumber}",
                        request.Request.StudentNumber);
                    return Result<Unit>.Failure("Student number already exists");
                }
                if (person.Student != null)
                {
                    return Result<Unit>.Failure("Person is already enrolled as a student");
                }
                if (person.Staff != null)
                {
                    return Result<Unit>.Failure("Person is already registered as staff - cannot enroll as student");
                }
                var educationLevel = (EducationLevel)request.Request.EducationLevel;
                person.EnrollAsStudent(
                    studentNumber: request.Request.StudentNumber,
                    educationLevel: educationLevel,
                    enrollmentDate: request.Request.EnrollmentDate,
                    advisorId: null);
                await _personRepository.UpdateAsync(person, cancellationToken);
                _logger.LogInformation("Student enrolled successfully for person with ID: {PersonId}", person.Id);
                return Result<Unit>.Success(Unit.Value, "Student enrolled successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enrolling student for person with ID: {PersonId}", request.PersonId);
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}