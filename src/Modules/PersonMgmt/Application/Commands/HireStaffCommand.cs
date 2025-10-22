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
/// Kişiyi personel olarak işe alma command
/// 
/// Kullanım:
/// var command = new HireStaffCommand(personId, new HireStaffRequest { ... });
/// var result = await _mediator.Send(command);
/// </summary>
public class HireStaffCommand : IRequest<Result<Unit>>
{
    /// <summary>
    /// Kişi ID
    /// </summary>
    public Guid PersonId { get; set; }

    /// <summary>
    /// Request verisi
    /// </summary>
    public HireStaffRequest Request { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public HireStaffCommand(Guid personId, HireStaffRequest request)
    {
        PersonId = personId;
        Request = request;
    }

    /// <summary>
    /// HireStaffCommand Handler
    /// </summary>
    public class Handler : IRequestHandler<HireStaffCommand, Result<Unit>>
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

        public async Task<Result<Unit>> Handle(HireStaffCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Hiring staff for person with ID: {PersonId}", request.PersonId);

                // Kişiyi ID'ye göre getir
                var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
                if (person == null)
                {
                    _logger.LogWarning("Person not found with ID: {PersonId}", request.PersonId);
                    return Result<Unit>.Failure($"Person with ID {request.PersonId} not found");
                }

                // ✅ Position string'ini AcademicTitle enum'a dönüştür
                // Position, enum name olarak gelmesi bekleniyor (e.g., "Professor", "Lecturer")
                if (!Enum.TryParse<AcademicTitle>(request.Request.Position, out var academicTitle))
                {
                    _logger.LogWarning("Invalid academic title: {Position}", request.Request.Position);
                    // Default değer: Assistant
                    academicTitle = AcademicTitle.Assistant;
                }

                // Personeli işe al
                person.HireAsStaff(
                    employeeNumber: request.Request.EmployeeNumber,
                    academicTitle: academicTitle,
                    hireDate: request.Request.HireDate);

                // Repository'ye kaydet (UpdateAsync)
                await _personRepository.UpdateAsync(person, cancellationToken);

                _logger.LogInformation("Staff hired successfully for person with ID: {PersonId}", person.Id);

                return Result<Unit>.Success(Unit.Value, "Staff hired successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error hiring staff for person with ID: {PersonId}", request.PersonId);
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}