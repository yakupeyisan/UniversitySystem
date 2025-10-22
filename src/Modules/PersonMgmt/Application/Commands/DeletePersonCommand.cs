using MediatR;

/// <summary>
/// Kişi silme command (soft delete)
/// 
/// Kullanım:
/// var command = new DeletePersonCommand(personId);
/// await _mediator.Send(command);
/// </summary>
public class DeletePersonCommand : IRequest<Unit>
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
}