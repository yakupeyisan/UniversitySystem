using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;
using PersonMgmt.Domain.Specifications;

namespace PersonMgmt.Application.Commands;

public class HireStaffCommand : IRequest<Result<Unit>>
{
    public HireStaffCommand(Guid personId, HireStaffRequest request)
    {
        PersonId = personId;
        Request = request;
    }

    public Guid PersonId { get; set; }
    public HireStaffRequest Request { get; set; }

    public class Handler : IRequestHandler<HireStaffCommand, Result<Unit>>
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
                var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
                if (person == null)
                {
                    _logger.LogWarning("Person not found with ID: {PersonId}", request.PersonId);
                    return Result<Unit>.Failure("Person not found");
                }

                var isEmployeeNumberUnique = await _personRepository.IsUniqueAsync(
                    new PersonByEmployeeNumberSpecification(request.Request.EmployeeNumber),
                    cancellationToken);
                if (!isEmployeeNumberUnique)
                {
                    _logger.LogWarning("Employee number already exists: {EmployeeNumber}",
                        request.Request.EmployeeNumber);
                    return Result<Unit>.Failure("Employee number already exists");
                }

                if (person.Staff != null)
                {
                    _logger.LogWarning("Person with ID {PersonId} is already registered as staff",
                        request.PersonId);
                    return Result<Unit>.Failure("Person is already registered as staff");
                }

                if (person.Student != null)
                {
                    _logger.LogWarning("Person with ID {PersonId} is already enrolled as student",
                        request.PersonId);
                    return Result<Unit>.Failure("Person is already enrolled as student and cannot be hired as staff");
                }

                var academicTitle = Enum.Parse<AcademicTitle>(request.Request.Position);
                person.HireAsStaff(
                    request.Request.EmployeeNumber,
                    academicTitle,
                    request.Request.HireDate
                );
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