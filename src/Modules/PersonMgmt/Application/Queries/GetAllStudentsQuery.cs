using Core.Domain.Results;
using MediatR;
using PersonMgmt.Application.DTOs;

namespace PersonMgmt.Application.Queries;

/// <summary>
/// Tüm öğrencileri getirme query
/// 
/// Kullanım:
/// var query = new GetAllStudentsQuery();
/// var result = await _mediator.Send(query);
/// </summary>
public class GetAllStudentsQuery : IRequest<Result<IEnumerable<PersonResponse>>>
{
}