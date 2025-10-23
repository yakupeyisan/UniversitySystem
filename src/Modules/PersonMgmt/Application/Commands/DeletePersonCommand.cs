using Core.Application.Abstractions;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Domain.Aggregates;

namespace PersonMgmt.Application.Commands;

/// <summary>
/// Kişi silme command (soft delete)
/// 
/// Kullanım:
/// var command = new DeletePersonCommand(personId);
/// var result = await _mediator.Send(command);
/// </summary>
public class DeletePersonCommand : IRequest<Result<Unit>>
{
    /// <summary>
    /// Kişi ID
    /// </summary>
    public Guid PersonId { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public DeletePersonCommand(Guid personId)
    {
        PersonId = personId;
    }

    /// <summary>
    /// DeletePersonCommand Handler
    /// </summary>
    public class Handler : IRequestHandler<DeletePersonCommand, Result<Unit>>
    {
        public readonly IRepository<Person> _personRepository;
        public readonly ICurrentUserService _currentUserService;
        public readonly ILogger<Handler> _logger;

        public Handler(IRepository<Person> personRepository, ICurrentUserService currentUserService, ILogger<Handler> logger)
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

                // Kişiyi ID'ye göre getir
                var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
                if (person == null)
                {
                    _logger.LogWarning("Person not found with ID: {PersonId}", request.PersonId);
                    return Result<Unit>.Failure($"Person with ID {request.PersonId} not found");
                }

                // Soft delete yap
                person.Delete(_currentUserService.UserId);

                // Repository'ye kaydet (UpdateAsync)
                await _personRepository.UpdateAsync(person, cancellationToken);

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