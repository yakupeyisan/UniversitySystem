using MediatR;
using Microsoft.AspNetCore.Mvc;
using PersonMgmt.Application.Commands;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Application.Queries;
using Core.Domain.Pagination;

namespace API.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class PersonsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PersonsController> _logger;
    public PersonsController(IMediator mediator, ILogger<PersonsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PersonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPerson(Guid id)
    {
        _logger.LogInformation("Getting person with ID: {PersonId}", id);
        var query = new GetPersonQuery(id);
        var result = await _mediator.Send(query);
        if (!result.IsSuccess)
            return NotFound(result);
        return Ok(result);
    }
    [HttpGet]
    [ProducesResponseType(typeof(PagedList<PersonResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPersons(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Getting all persons - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
        var pagedRequest = new PagedRequest { PageNumber = pageNumber, PageSize = pageSize };
        var query = new GetAllPersonsQuery(pagedRequest);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    [HttpPost]
    [ProducesResponseType(typeof(PersonResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePerson([FromBody] CreatePersonRequest request)
    {
        _logger.LogInformation("Creating new person: {FirstName} {LastName}", request.FirstName, request.LastName);
        var command = new CreatePersonCommand(request);
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetPerson), new { id = result.Value!.Id }, result);
    }
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(PersonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePerson(Guid id, [FromBody] UpdatePersonRequest request)
    {
        _logger.LogInformation("Updating person with ID: {PersonId}", id);
        var command = new UpdatePersonCommand(id, request);
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePerson(Guid id)
    {
        _logger.LogInformation("Deleting person with ID: {PersonId}", id);
        var currentUserId = Guid.NewGuid();
        var command = new DeletePersonCommand(id);
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return NotFound(result);
        return NoContent();
    }
    [HttpGet("search/email")]
    [ProducesResponseType(typeof(PersonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEmail([FromQuery] string email)
    {
        _logger.LogInformation("Getting person by email: {Email}", email);
        var query = new GetPersonByEmailQuery(email);
        var result = await _mediator.Send(query);
        if (!result.IsSuccess)
            return NotFound(result);
        return Ok(result);
    }
    [HttpGet("search/identification/{identificationNumber}")]
    [ProducesResponseType(typeof(PersonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdentificationNumber(string identificationNumber)
    {
        _logger.LogInformation("Getting person by identification number: {IdNumber}", identificationNumber);
        var query = new GetPersonByIdentificationNumberQuery(identificationNumber);
        var result = await _mediator.Send(query);
        if (!result.IsSuccess)
            return NotFound(result);
        return Ok(result);
    }

    [HttpGet("students")]
    [ProducesResponseType(typeof(PagedList<PersonResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllStudents(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Getting all students");
        var pagedRequest = new PagedRequest { PageNumber = pageNumber, PageSize = pageSize };
        var query = new GetAllStudentsQuery(pagedRequest);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    [HttpGet("students/{studentNumber}")]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentByNumber(string studentNumber)
    {
        _logger.LogInformation("Getting student by number: {StudentNumber}", studentNumber);
        var query = new GetStudentByStudentNumberQuery(studentNumber);
        var result = await _mediator.Send(query);
        if (!result.IsSuccess)
            return NotFound(result);
        return Ok(result);
    }

    [HttpGet("staff")]
    [ProducesResponseType(typeof(PagedList<PersonResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllStaff(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Getting all staff members");
        var pagedRequest = new PagedRequest { PageNumber = pageNumber, PageSize = pageSize };
        var query = new GetAllStaffQuery(pagedRequest);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    [HttpGet("staff/{employeeNumber}")]
    [ProducesResponseType(typeof(StaffResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStaffByNumber(string employeeNumber)
    {
        _logger.LogInformation("Getting staff by number: {EmployeeNumber}", employeeNumber);
        var query = new GetStaffByNumberQuery(employeeNumber);
        var result = await _mediator.Send(query);
        if (!result.IsSuccess)
            return NotFound(result);
        return Ok(result);
    }

    [HttpGet("{personId}/restrictions")]
    [ProducesResponseType(typeof(IEnumerable<RestrictionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPersonRestrictions(Guid personId)
    {
        _logger.LogInformation("Getting restrictions for person: {PersonId}", personId);
        var query = new GetPersonRestrictionsQuery(personId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    [HttpPost("{personId}/restrictions")]
    [ProducesResponseType(typeof(RestrictionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddRestriction(Guid personId, [FromBody] AddRestrictionRequest request)
    {
        _logger.LogInformation("Adding restriction to person: {PersonId}", personId);
        var appliedBy = Guid.NewGuid();
        var command = new AddRestrictionCommand(personId, appliedBy, request);
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetPersonRestrictions), new { personId }, result);
    }
    [HttpGet("{personId}/health-record")]
    [ProducesResponseType(typeof(HealthRecordResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHealthRecord(Guid personId)
    {
        _logger.LogInformation("Getting health record for person: {PersonId}", personId);
        var query = new GetHealthRecordQuery(personId);
        var result = await _mediator.Send(query);
        if (!result.IsSuccess)
            return NotFound(result);
        return Ok(result);
    }
}