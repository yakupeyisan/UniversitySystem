using MediatR;
using PersonMgmt.Application.DTOs;

/// <summary>
/// Kişiyi öğrenci olarak kaydetme command
/// 
/// Kullanım:
/// var command = new EnrollStudentCommand(personId, new EnrollStudentRequest { ... });
/// await _mediator.Send(command);
/// </summary>
public class EnrollStudentCommand : IRequest<Unit>
{
    /// <summary>
    /// Kişi ID
    /// </summary>
    public Guid PersonId { get; set; }

    /// <summary>
    /// Request verisi
    /// </summary>
    public EnrollStudentRequest Request { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public EnrollStudentCommand(Guid personId, EnrollStudentRequest request)
    {
        PersonId = personId;
        Request = request;
    }
}