using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace PersonMgmt.Application.Commands;

public class TerminateStaffCommand : IRequest<Result<Unit>>
{
    public TerminateStaffCommand(Guid personId, DateTime terminationDate, string? reason = null)
    {
        PersonId = personId;
        TerminationDate = terminationDate;
        Reason = reason;
    }

    public Guid PersonId { get; set; }
    public DateTime TerminationDate { get; set; }
    public string? Reason { get; set; }

    public class Handler : IRequestHandler<TerminateStaffCommand, Result<Unit>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IPersonRepository _personRepository;

        public Handler(IPersonRepository personRepository, ILogger<Handler> logger)
        {
            _personRepository = personRepository;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(TerminateStaffCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Terminating staff for person {PersonId} with termination date {TerminationDate}",
                    request.PersonId,
                    request.TerminationDate);
                var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
                if (person == null)
                {
                    _logger.LogWarning("Person not found with ID: {PersonId}", request.PersonId);
                    return Result<Unit>.Failure("Person not found");
                }

                if (person.Staff == null)
                {
                    _logger.LogWarning("Person {PersonId} is not registered as staff", request.PersonId);
                    return Result<Unit>.Failure("Person is not registered as staff");
                }

                if (request.TerminationDate > DateTime.UtcNow)
                {
                    _logger.LogWarning("Termination date cannot be in the future: {TerminationDate}",
                        request.TerminationDate);
                    return Result<Unit>.Failure("Termination date cannot be in the future");
                }

                person.Staff.Terminate(request.TerminationDate);
                await _personRepository.UpdateAsync(person, cancellationToken);
                _logger.LogInformation(
                    "Staff terminated successfully for person {PersonId} on date {TerminationDate}. Reason: {Reason}",
                    request.PersonId,
                    request.TerminationDate,
                    request.Reason ?? "Not specified");
                return Result<Unit>.Success(Unit.Value, "Staff terminated successfully");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Termination validation error: {Message}", ex.Message);
                return Result<Unit>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error terminating staff for person {PersonId}", request.PersonId);
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}