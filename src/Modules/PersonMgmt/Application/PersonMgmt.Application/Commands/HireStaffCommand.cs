using MediatR;
using PersonMgmt.Application.DTOs;

/// <summary>
/// Kişiyi personel olarak işe alma command
/// 
/// Kullanım:
/// var command = new HireStaffCommand(personId, new HireStaffRequest { ... });
/// await _mediator.Send(command);
/// </summary>
public class HireStaffCommand : IRequest<Unit>
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
}