using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;

namespace PersonMgmt.Application.Commands;

/// <summary>
/// Kişiyi öğrenci olarak kaydetme command
/// 
/// Kullanım:
/// var command = new EnrollStudentCommand(personId, new EnrollStudentRequest { ... });
/// var result = await _mediator.Send(command);
/// </summary>
public class EnrollStudentCommand : IRequest<Result<Unit>>
{
    /// <summary>
    /// Kişi ID
    /// </summary>
    public Guid PersonId { get; set; }

    /// <summary>
    /// Request verisi
    /// </summary>
    public EnrollStudentRequest Request { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public EnrollStudentCommand(Guid personId, EnrollStudentRequest request)
    {
        PersonId = personId;
        Request = request;
    }

    /// <summary>
    /// EnrollStudentCommand Handler
    /// </summary>
    public class Handler : IRequestHandler<EnrollStudentCommand, Result<Unit>>
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

        public async Task<Result<Unit>> Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Enrolling student for person with ID: {PersonId}", request.PersonId);

                // Kişiyi ID'ye göre getir
                var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
                if (person == null)
                {
                    _logger.LogWarning("Person not found with ID: {PersonId}", request.PersonId);
                    return Result<Unit>.Failure($"Person with ID {request.PersonId} not found");
                }

                // ✅ EducationLevel byte'ı enum'a dönüştür
                var educationLevel = (EducationLevel)request.Request.EducationLevel;

                // Öğrenciyi kayıt et
                person.EnrollAsStudent(
                    studentNumber: request.Request.StudentNumber,
                    educationLevel: educationLevel,
                    enrollmentDate: request.Request.EnrollmentDate,
                    advisorId: null);  // Advisor bilgisi request'te yok

                // Repository'ye kaydet (UpdateAsync)
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