using Core.Domain.Results;
using MediatR;
using PersonMgmt.Application.DTOs;

namespace PersonMgmt.Application.Queries;

/// <summary>
/// Departmana göre kişileri getirme query
/// 
/// Kullanım:
/// var query = new GetPersonsByDepartmentQuery(departmentId);
/// var result = await _mediator.Send(query);
/// </summary>
public class GetPersonsByDepartmentQuery : IRequest<Result<IEnumerable<PersonResponse>>>
{
    /// <summary>
    /// Departman ID
    /// </summary>
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public GetPersonsByDepartmentQuery(Guid departmentId)
    {
        DepartmentId = departmentId;
    }
}