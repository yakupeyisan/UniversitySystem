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
                _logger.LogInformation("Hiring staff for person with ID: {PersonId}", request.PersonId);
                var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
                if (person == null)
                {
                    _logger.LogWarning("Person not found with ID: {PersonId}", request.PersonId);
                    return Result<Unit>.Failure("Person not found");
                }
                var isEmployeeNumberUnique = await _personRepository.IsEmployeeNumberUniqueAsync(
                    request.Request.EmployeeNumber,
                    cancellationToken: cancellationToken);
                if (!isEmployeeNumberUnique)
                {
                    _logger.LogWarning("Employee number already exists: {EmployeeNumber}",
                        request.Request.EmployeeNumber);
                    return Result<Unit>.Failure("Employee number already exists");
                }
                if (person.Staff != null)
                {
                    return Result<Unit>.Failure("Person is already registered as staff");
                }
                if (person.Student != null)
                {
                    return Result<Unit>.Failure("Person is already enrolled as student - cannot hire as staff");
                }
                if (!Enum.TryParse<AcademicTitle>(request.Request.Position, out var academicTitle))
                {
                    _logger.LogWarning("Invalid academic title: {Position}", request.Request.Position);
                    return Result<Unit>.Failure("Invalid academic title");
                }
                person.HireAsStaff(
                    employeeNumber: request.Request.EmployeeNumber,
                    academicTitle: academicTitle,
                    hireDate: request.Request.HireDate);
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