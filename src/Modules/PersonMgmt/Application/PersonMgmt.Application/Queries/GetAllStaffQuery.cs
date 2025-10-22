using Core.Domain.Results;
using MediatR;
using PersonMgmt.Application.DTOs;

namespace PersonMgmt.Application.Queries;

/// <summary>
/// Tüm personelleri getirme query
/// 
/// Kullanım:
/// var query = new GetAllStaffQuery();
/// var result = await _mediator.Send(query);
/// </summary>
public class GetAllStaffQuery : IRequest<Result<IEnumerable<PersonResponse>>>
{
}