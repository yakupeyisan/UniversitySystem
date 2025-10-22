using Core.Domain.Results;
using MediatR;
using PersonMgmt.Application.DTOs;

namespace PersonMgmt.Application.Queries;

/// <summary>
/// Tüm kişileri getirme query
/// 
/// Kullanım:
/// var query = new GetAllPersonsQuery();
/// var result = await _mediator.Send(query);
/// </summary>
public class GetAllPersonsQuery : IRequest<Result<IEnumerable<PersonResponse>>>
{
}