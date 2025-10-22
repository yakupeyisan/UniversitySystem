using MediatR;
using PersonMgmt.Application.DTOs;

/// <summary>
/// Kişi güncelleme command
/// 
/// Kullanım:
/// var command = new UpdatePersonCommand(personId, new UpdatePersonRequest { ... });
/// await _mediator.Send(command);
/// </summary>
public class UpdatePersonCommand : IRequest<Unit>
{
    /// <summary>
    /// Kişi ID
    /// </summary>
    public Guid PersonId { get; set; }

    /// <summary>
    /// Request verisi
    /// </summary>
    public UpdatePersonRequest Request { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public UpdatePersonCommand(Guid personId, UpdatePersonRequest request)
    {
        PersonId = personId;
        Request = request;
    }
}