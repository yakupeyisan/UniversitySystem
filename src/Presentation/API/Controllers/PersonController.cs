using System.Security.Claims;
using Core.Application.Abstractions.Pagination;
using Core.Domain.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonMgmt.Application.Commands;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Application.Queries;

namespace API.Controllers;

/// <summary>
/// Person Management Controller
/// 
/// Endpoints:
/// - POST   /api/persons              → Create person
/// - GET    /api/persons/{id}         → Get person by ID
/// - GET    /api/persons              → Get all persons (paginated)
/// - PUT    /api/persons/{id}         → Update person
/// - POST   /api/persons/{id}/enroll-student    → Enroll as student
/// - POST   /api/persons/{id}/hire-staff        → Hire as staff
/// - POST   /api/persons/{id}/add-restriction   → Add restriction
/// - DELETE /api/persons/{id}         → Delete person (soft delete)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PersonController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PersonController> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    public PersonController(IMediator mediator, ILogger<PersonController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    // ==================== BASIC CRUD ====================

    /// <summary>
    /// Yeni kişi oluştur
    /// </summary>
    /// <param name="request">Kişi oluşturma bilgileri</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>Oluşturulan kişi bilgileri</returns>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<PersonResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<PersonResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<PersonResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<PersonResponse>>> CreatePerson(
        [FromBody] CreatePersonRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating person: {FirstName} {LastName}", request.FirstName, request.LastName);

        var command = new CreatePersonCommand(request);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetPerson), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// ID'ye göre kişi getir
    /// </summary>
    /// <param name="id">Kişi ID'si</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>Kişi bilgileri</returns>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<PersonResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<PersonResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<PersonResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<PersonResponse>>> GetPerson(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetPersonQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Tüm kişileri getir (pagination ve filter desteği)
    /// </summary>
    /// <param name="pageNumber">Sayfa numarası (default: 1)</param>
    /// <param name="pageSize">Sayfa boyutu (default: 20)</param>
    /// <param name="filter">Dinamik filter (format: field|operator|value;field2|operator2|value2)</param>
    /// <param name="sortBy">Sıralama alanı</param>
    /// <param name="sortDirection">Sıralama yönü (asc/desc)</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>Kişi listesi (paginated)</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<PagedList<PersonResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<PagedList<PersonResponse>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<PagedList<PersonResponse>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<PagedList<PersonResponse>>>> GetAllPersons(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? filter = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null,
        CancellationToken cancellationToken = default)
    {
        var pagedRequest = new PagedRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy ?? "CreatedAt",
            SortDirection = sortDirection ?? "desc"
        };

        var query = new GetAllPersonsQuery(pagedRequest, filter);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Kişi bilgisini güncelle
    /// </summary>
    /// <param name="id">Kişi ID'si</param>
    /// <param name="request">Güncellenecek bilgiler</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>Güncellenmiş kişi bilgileri</returns>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result<PersonResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<PersonResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<PersonResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<PersonResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<PersonResponse>>> UpdatePerson(
        [FromRoute] Guid id,
        [FromBody] UpdatePersonRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdatePersonCommand(id, request);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Kişiyi sil (soft delete)
    /// </summary>
    /// <param name="id">Kişi ID'si</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>Silme işlemi sonucu</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<Unit>>> DeletePerson(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeletePersonCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    // ==================== STUDENT OPERATIONS ====================

    /// <summary>
    /// Kişiyi öğrenci olarak kaydet
    /// </summary>
    /// <param name="id">Kişi ID'si</param>
    /// <param name="request">Öğrenci kayıt bilgileri</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>İşlem sonucu</returns>
    [HttpPost("{id:guid}/enroll-student")]
    [Authorize(Roles = "Admin,Registrar")]
    [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<Unit>>> EnrollStudent(
        [FromRoute] Guid id,
        [FromBody] EnrollStudentRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Enrolling person {PersonId} as student with number {StudentNumber}", id, request.StudentNumber);

        var command = new EnrollStudentCommand(id, request);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Tüm öğrencileri getir (paginated)
    /// </summary>
    /// <param name="pageNumber">Sayfa numarası</param>
    /// <param name="pageSize">Sayfa boyutu</param>
    /// <param name="filter">Dinamik filter</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>Öğrenci listesi</returns>
    [HttpGet("students/all")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<PagedList<PersonResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<PagedList<PersonResponse>>>> GetAllStudents(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var pagedRequest = new PagedRequest { PageNumber = pageNumber, PageSize = pageSize };
        var query = new GetAllStudentsQuery(pagedRequest, filter);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    // ==================== STAFF OPERATIONS ====================

    /// <summary>
    /// Kişiyi personel olarak işe al
    /// </summary>
    /// <param name="id">Kişi ID'si</param>
    /// <param name="request">İşe alma bilgileri</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>İşlem sonucu</returns>
    [HttpPost("{id:guid}/hire-staff")]
    [Authorize(Roles = "Admin,HR")]
    [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<Unit>>> HireStaff(
        [FromRoute] Guid id,
        [FromBody] HireStaffRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hiring person {PersonId} as staff with number {EmployeeNumber}", id, request.EmployeeNumber);

        var command = new HireStaffCommand(id, request);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Tüm personelleri getir (paginated)
    /// </summary>
    /// <param name="pageNumber">Sayfa numarası</param>
    /// <param name="pageSize">Sayfa boyutu</param>
    /// <param name="filter">Dinamik filter</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>Personel listesi</returns>
    [HttpGet("staff/all")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<PagedList<PersonResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<PagedList<PersonResponse>>>> GetAllStaff(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var pagedRequest = new PagedRequest { PageNumber = pageNumber, PageSize = pageSize };
        var query = new GetAllStaffQuery(pagedRequest, filter);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    // ==================== RESTRICTION OPERATIONS ====================

    /// <summary>
    /// Kişiye kısıtlama ekle
    /// </summary>
    /// <param name="id">Kişi ID'si</param>
    /// <param name="request">Kısıtlama bilgileri</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>İşlem sonucu</returns>
    [HttpPost("{id:guid}/add-restriction")]
    [Authorize(Roles = "Admin,Disciplinary")]
    [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Result<Unit>>> AddRestriction(
        [FromRoute] Guid id,
        [FromBody] AddRestrictionRequest request,
        CancellationToken cancellationToken)
    {
        var appliedByString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString();
        var appliedBy = Guid.TryParse(appliedByString, out var parsedGuid) ? parsedGuid : Guid.Empty;

        _logger.LogInformation("Adding restriction to person {PersonId}", id);

        var command = new AddRestrictionCommand(id, appliedBy, request);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Aktif kısıtlamalarla kişileri getir (paginated)
    /// </summary>
    /// <param name="pageNumber">Sayfa numarası</param>
    /// <param name="pageSize">Sayfa boyutu</param>
    /// <param name="filter">Dinamik filter</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>Kısıtlamalarla kişilerin listesi</returns>
    [HttpGet("restrictions/active")]
    [Authorize(Roles = "Admin,Disciplinary")]
    [ProducesResponseType(typeof(Result<PagedList<PersonResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<PagedList<PersonResponse>>>> GetPersonsWithActiveRestrictions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var pagedRequest = new PagedRequest { PageNumber = pageNumber, PageSize = pageSize };
        var query = new GetPersonsWithActiveRestrictionsQuery(pagedRequest, filter);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    // ==================== DEPARTMENT OPERATIONS ====================

    /// <summary>
    /// Departmandaki kişileri getir (paginated)
    /// </summary>
    /// <param name="departmentId">Departman ID'si</param>
    /// <param name="pageNumber">Sayfa numarası</param>
    /// <param name="pageSize">Sayfa boyutu</param>
    /// <param name="filter">Dinamik filter</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>Departmandaki kişilerin listesi</returns>
    [HttpGet("department/{departmentId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<PagedList<PersonResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<PagedList<PersonResponse>>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<PagedList<PersonResponse>>>> GetByDepartment(
        [FromRoute] Guid departmentId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var pagedRequest = new PagedRequest { PageNumber = pageNumber, PageSize = pageSize };
        var query = new GetPersonsByDepartmentQuery(departmentId, pagedRequest, filter);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }
}


