using Academic.Application.Commands.Courses;
using Academic.Application.DTOs;
using Academic.Application.Queries.Courses;
using Core.Domain.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AcademicController : ControllerBase
{
    private readonly ILogger<AcademicController> _logger;
    private readonly IMediator _mediator;

    public AcademicController(IMediator mediator, ILogger<AcademicController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("courses")]
    [ProducesResponseType(typeof(PagedList<CourseListResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCourses(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Getting all courses - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
        try
        {
            var pagedRequest = new PagedRequest { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetAllCoursesQuery(pagedRequest);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all courses");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpGet("courses/{id}")]
    [ProducesResponseType(typeof(CourseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourse(Guid id)
    {
        _logger.LogInformation("Getting course with ID: {CourseId}", id);
        try
        {
            var query = new GetCourseQuery(id);
            var result = await _mediator.Send(query);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting course with ID: {CourseId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpGet("courses/code/{courseCode}")]
    [ProducesResponseType(typeof(CourseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourseByCourseCode(string courseCode)
    {
        _logger.LogInformation("Getting course with code: {CourseCode}", courseCode);
        try
        {
            var query = new GetCourseByCourseCodeQuery(courseCode);
            var result = await _mediator.Send(query);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting course with code: {CourseCode}", courseCode);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpGet("courses/department/{departmentId}")]
    [ProducesResponseType(typeof(PagedList<CourseListResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCoursesByDepartment(
        Guid departmentId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Getting courses for department: {DepartmentId}", departmentId);
        try
        {
            var pagedRequest = new PagedRequest { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetCoursesByDepartmentQuery(departmentId, pagedRequest);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting courses for department: {DepartmentId}", departmentId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpGet("courses/available")]
    [ProducesResponseType(typeof(IEnumerable<CourseListResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableCourses()
    {
        _logger.LogInformation("Getting available courses");
        try
        {
            var query = new GetAvailableCoursesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available courses");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpPost("courses")]
    [ProducesResponseType(typeof(CourseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequest request)
    {
        _logger.LogInformation("Creating new course: {CourseName}", request.Name);
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var command = new CreateCourseCommand(request);
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetCourse), new { id = result.Value.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating course: {CourseName}", request.Name);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpGet("students/{studentId}/courses")]
    [ProducesResponseType(typeof(StudentCoursesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentCourses(
        Guid studentId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Getting courses for student: {StudentId}", studentId);
        try
        {
            var pagedRequest = new PagedRequest { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetStudentCoursesQuery(studentId, pagedRequest);
            var result = await _mediator.Send(query);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting courses for student: {StudentId}", studentId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpPost("enrollments/enroll")]
    [ProducesResponseType(typeof(CourseRegistrationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EnrollStudent([FromBody] EnrollStudentRequest request)
    {
        _logger.LogInformation("Enrolling student {StudentId} in course {CourseId}",
            request.StudentId, request.CourseId);
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var command = new EnrollStudentCommand(request);
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetStudentCourses), new { studentId = request.StudentId }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enrolling student {StudentId} in course {CourseId}",
                request.StudentId, request.CourseId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpDelete("enrollments/drop-course")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DropCourse(DropCourseRequest dropCourseRequest)
    {
        _logger.LogInformation("Dropping course enrollment:course-> {CourseId} student:{StudentId} reson: {Reason}  ",
            dropCourseRequest.CourseId, dropCourseRequest.StudentId, dropCourseRequest.Reason);
        try
        {
            var command = new DropCourseCommand(dropCourseRequest);
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error dropping course enrollment: course-> {CourseId} student:{StudentId} reson: {Reason}  ",
                dropCourseRequest.CourseId, dropCourseRequest.StudentId, dropCourseRequest.Reason);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpGet("students/{studentId}/exams")]
    [ProducesResponseType(typeof(IEnumerable<ExamResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentExams(Guid studentId)
    {
        _logger.LogInformation("Getting exams for student: {StudentId}", studentId);
        try
        {
            var query = new GetExamsByStudentQuery(studentId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting exams for student: {StudentId}", studentId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpGet("courses/{courseId}/exams")]
    [ProducesResponseType(typeof(IEnumerable<ExamResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseExams(Guid courseId)
    {
        _logger.LogInformation("Getting exams for course: {CourseId}", courseId);
        try
        {
            var query = new GetExamsByCourseQuery(courseId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting exams for course: {CourseId}", courseId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpGet("exams/date-range")]
    [ProducesResponseType(typeof(IEnumerable<ExamResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExamsByDateRange(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate)
    {
        _logger.LogInformation("Getting exams for date range: {StartDate} to {EndDate}", startDate, endDate);
        try
        {
            var query = new GetExamsByDateRangeQuery(startDate, endDate);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting exams for date range");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpGet("students/{studentId}/grades")]
    [ProducesResponseType(typeof(StudentGradesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentGrades(Guid studentId)
    {
        _logger.LogInformation("Getting grades for student: {StudentId}", studentId);
        try
        {
            var query = new GetStudentGradesQuery(studentId);
            var result = await _mediator.Send(query);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting grades for student: {StudentId}", studentId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpGet("students/{studentId}/grades/semester/{semester}")]
    [ProducesResponseType(typeof(IEnumerable<GradeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentGradesBySemester(Guid studentId, string semester)
    {
        _logger.LogInformation("Getting grades for student {StudentId} in semester {Semester}", studentId, semester);
        try
        {
            var query = new GetStudentGradesBySemesterQuery(studentId, semester);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting grades for student {StudentId} in semester {Semester}",
                studentId, semester);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpGet("students/{studentId}/grades/course/{courseId}")]
    [ProducesResponseType(typeof(GradeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentGradeForCourse(Guid studentId, Guid courseId)
    {
        _logger.LogInformation("Getting grade for student {StudentId} in course {CourseId}",
            studentId, courseId);
        try
        {
            var query = new GetStudentGradeForCourseQuery(studentId, courseId);
            var result = await _mediator.Send(query);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting grade for student {StudentId} in course {CourseId}",
                studentId, courseId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpPut("grades/update")]
    [ProducesResponseType(typeof(GradeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateGrade([FromBody] UpdateGradeRequest request)
    {
        _logger.LogInformation("Updating grade: {GradeId}", request.GradeId);
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var command = new UpdateGradeCommand(request);
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating grade: {GradeId}", request.GradeId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpPost("grade-objections")]
    [ProducesResponseType(typeof(GradeObjectionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitGradeObjection([FromBody] SubmitGradeObjectionRequest request)
    {
        _logger.LogInformation("Submitting grade objection for student {StudentId}", request.StudentId);
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var command = new SubmitGradeObjectionCommand(request);
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetStudentGrades), new { studentId = request.StudentId }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting grade objection");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpGet("students/{studentId}/grade-objections")]
    [ProducesResponseType(typeof(IEnumerable<GradeObjectionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentGradeObjections(Guid studentId)
    {
        _logger.LogInformation("Getting grade objections for student: {StudentId}", studentId);
        try
        {
            var query = new GetStudentGradeObjectionsQuery(studentId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting grade objections for student: {StudentId}", studentId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpGet("grade-objections/pending")]
    [ProducesResponseType(typeof(IEnumerable<GradeObjectionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingGradeObjections()
    {
        _logger.LogInformation("Getting pending grade objections");
        try
        {
            var query = new GetPendingGradeObjectionsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending grade objections");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpPut("grade-objections/approve")]
    [ProducesResponseType(typeof(GradeObjectionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveGradeObjection([FromBody] ApproveGradeObjectionRequest request)
    {
        _logger.LogInformation("Approving grade objection: {ObjectionId}", request.ObjectionId);
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var command = new ApproveGradeObjectionCommand(request);
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving grade objection: {ObjectionId}", request.ObjectionId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpPut("grade-objections/{objectionId}/reject")]
    [ProducesResponseType(typeof(GradeObjectionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectGradeObjection(
        Guid objectionId,
        [FromBody] RejectGradeObjectionRequest request)
    {
        _logger.LogInformation("Rejecting grade objection: {ObjectionId}", objectionId);
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var command = new RejectGradeObjectionCommand(objectionId, request);
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting grade objection: {ObjectionId}", objectionId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpPost("waiting-list/join")]
    [ProducesResponseType(typeof(CourseWaitingListEntryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> JoinWaitingList([FromBody] JoinWaitingListRequest request)
    {
        _logger.LogInformation("Student {StudentId} joining waiting list for course {CourseId}",
            request.StudentId, request.CourseId);
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var command = new JoinWaitingListCommand(request);
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetCourseWaitingList),
                new { courseId = request.CourseId }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining waiting list for course");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpGet("courses/{courseId}/waiting-list")]
    [ProducesResponseType(typeof(IEnumerable<CourseWaitingListEntryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseWaitingList(Guid courseId)
    {
        _logger.LogInformation("Getting waiting list for course: {CourseId}", courseId);
        try
        {
            var query = new GetCourseWaitingListQuery(courseId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting waiting list for course: {CourseId}", courseId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpGet("students/{studentId}/waiting-list-positions")]
    [ProducesResponseType(typeof(IEnumerable<CourseWaitingListEntryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentWaitingListPositions(Guid studentId)
    {
        _logger.LogInformation("Getting waiting list positions for student: {StudentId}", studentId);
        try
        {
            var query = new GetStudentWaitingListPositionsQuery(studentId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting waiting list positions for student: {StudentId}", studentId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
}