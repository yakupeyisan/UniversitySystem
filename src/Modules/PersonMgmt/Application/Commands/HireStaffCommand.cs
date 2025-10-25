using AutoMapper;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Enums;
using PersonMgmt.Domain.Interfaces;
namespace PersonMgmt.Application.Commands;


public class HireStaffCommand : IRequest<Result<Unit>>
{
    public Guid PersonId { get; set; }
    public HireStaffRequest Request { get; set; }

    public HireStaffCommand(Guid personId, HireStaffRequest request)
    {
        PersonId = personId;
        Request = request;
    }

    public class Handler : IRequestHandler<HireStaffCommand, Result<Unit>>
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
            HireStaffCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Hiring staff for person with ID: {PersonId}, EmployeeNumber: {EmployeeNumber}",
                    request.PersonId,
                    request.Request.EmployeeNumber);

                // Get person by ID
                var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
                if (person == null)
                {
                    _logger.LogWarning("Person not found with ID: {PersonId}", request.PersonId);
                    return Result<Unit>.Failure("Person not found");
                }

                // Check if employee number is unique
                var isEmployeeNumberUnique = await _personRepository.IsEmployeeNumberUniqueAsync(
                    request.Request.EmployeeNumber,
                    cancellationToken: cancellationToken);

                if (!isEmployeeNumberUnique)
                {
                    _logger.LogWarning("Employee number already exists: {EmployeeNumber}",
                        request.Request.EmployeeNumber);
                    return Result<Unit>.Failure("Employee number already exists");
                }

                // Check if person is already staff
                if (person.Staff != null)
                {
                    _logger.LogWarning("Person with ID {PersonId} is already registered as staff",
                        request.PersonId);
                    return Result<Unit>.Failure("Person is already registered as staff");
                }

                // Check if person is student - cannot be both
                if (person.Student != null)
                {
                    _logger.LogWarning("Person with ID {PersonId} is already enrolled as student",
                        request.PersonId);
                    return Result<Unit>.Failure("Person is already enrolled as student and cannot be hired as staff");
                }

                // Parse AcademicTitle enum
                var academicTitle = Enum.Parse<AcademicTitle>(request.Request.Position);

                // Hire as staff
                person.HireAsStaff(
                    employeeNumber: request.Request.EmployeeNumber,
                    academicTitle: academicTitle,
                    hireDate: request.Request.HireDate
                );

                // Save changes
                await _personRepository.UpdateAsync(person, cancellationToken);
                await _personRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Person with ID {PersonId} hired as staff successfully",
                    request.PersonId);

                return Result<Unit>.Success(Unit.Value, "Staff hired successfully");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Validation error while hiring staff");
                return Result<Unit>.Failure($"Validation error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error hiring staff for person with ID: {PersonId}", request.PersonId);
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}


