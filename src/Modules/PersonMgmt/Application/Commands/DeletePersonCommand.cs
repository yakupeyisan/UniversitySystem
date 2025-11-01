using Core.Application.Abstractions;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Domain.Aggregates;
namespace PersonMgmt.Application.Commands;
public class DeletePersonCommand : IRequest<Result<Unit>>
{
    public DeletePersonCommand(Guid personId)
    {
        PersonId = personId;
    }
    public Guid PersonId { get; set; }
    public class Handler : IRequestHandler<DeletePersonCommand, Result<Unit>>
    {
        public readonly ICurrentUserService _currentUserService;
        public readonly ILogger<Handler> _logger;
        public readonly IRepository<Person>
            _personRepository;
        public Handler(IRepository<Person>
                personRepository, ICurrentUserService currentUserService,
            ILogger<Handler> logger)
        {
            _personRepository = personRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }
        public async Task<Result<Unit>> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting person with ID: {PersonId}", request.PersonId);
                var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
                if (person == null)
                {
                    _logger.LogWarning("Person not found with ID: {PersonId}", request.PersonId);
                    return Result<Unit>.Failure($"Person with ID {request.PersonId} not found");
                }
                person.Delete(_currentUserService.UserId);
                await _personRepository.UpdateAsync(person, cancellationToken);
                await _personRepository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Person deleted successfully with ID: {PersonId}", person.Id);
                return Result<Unit>.Success(Unit.Value, "Person deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting person with ID: {PersonId}", request.PersonId);
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}