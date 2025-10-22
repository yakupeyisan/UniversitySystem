using Core.Domain.Results;
using MediatR;
using PersonMgmt.Application.DTOs;

namespace PersonMgmt.Application.Queries;

/// <summary>
/// ID'ye göre kişi getirme query
/// 
/// Kullanım:
/// var query = new GetPersonQuery(personId);
/// var result = await _mediator.Send(query);
/// </summary>
public class GetPersonQuery : IRequest<Result<PersonResponse>>
{
    /// <summary>
    /// Kişi ID
    /// </summary>
    public Guid PersonId { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public GetPersonQuery(Guid personId)
    {
        PersonId = personId;
    }
}