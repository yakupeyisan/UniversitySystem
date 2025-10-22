using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;

/// <summary>
/// Kişiye kısıtlama ekleme command
/// 
/// Kullanım:
/// var command = new AddRestrictionCommand(personId, new AddRestrictionRequest { ... });
/// await _mediator.Send(command);
/// </summary>
public class AddRestrictionCommand : IRequest<Result<Unit>>
{
    /// <summary>
    /// Kişi ID
    /// </summary>
    public Guid PersonId { get; set; }

    /// <summary>
    /// Kısıtlamayı uygulayan (Admin/User ID)
    /// </summary>
    public Guid AppliedBy { get; set; }

    /// <summary>
    /// Request verisi
    /// </summary>
    public AddRestrictionRequest Request { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public AddRestrictionCommand(Guid personId, Guid appliedBy, AddRestrictionRequest request)
    {
        PersonId = personId;
        AppliedBy = appliedBy;
        Request = request;
    }
}
