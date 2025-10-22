using Core.Domain.Results;
using MediatR;
using PersonMgmt.Application.DTOs;

namespace PersonMgmt.Application.Queries;

/// <summary>
/// Aktif kısıtlamalarla kişileri getirme query
/// 
/// Kullanım:
/// var query = new GetPersonsWithActiveRestrictionsQuery();
/// var result = await _mediator.Send(query);
/// </summary>
public class GetPersonsWithActiveRestrictionsQuery : IRequest<Result<IEnumerable<PersonResponse>>>
{
}