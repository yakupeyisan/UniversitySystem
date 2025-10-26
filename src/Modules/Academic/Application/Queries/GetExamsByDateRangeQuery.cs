using Academic.Application.DTOs;
using Academic.Domain.Interfaces;
using AutoMapper;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Academic.Application.Queries.Courses;

public class GetExamsByDateRangeQuery : IRequest<Result<IEnumerable<ExamResponse>>>
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public GetExamsByDateRangeQuery(DateOnly startDate, DateOnly endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date cannot be greater than end date");
        StartDate = startDate;
        EndDate = endDate;
    }

    public class Handler : IRequestHandler<GetExamsByDateRangeQuery, Result<IEnumerable<ExamResponse>>>
    {
        private readonly IExamRepository _examRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(IExamRepository examRepository, IMapper mapper, ILogger<Handler> logger)
        {
            _examRepository = examRepository ?? throw new ArgumentNullException(nameof(examRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<IEnumerable<ExamResponse>>> Handle(
            GetExamsByDateRangeQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching exams between {StartDate} and {EndDate}",
                    request.StartDate, request.EndDate);

                var exams = await _examRepository.GetByDateRangeAsync(
                    request.StartDate, request.EndDate, cancellationToken);

                var responses = _mapper.Map<IEnumerable<ExamResponse>>(exams);

                _logger.LogInformation("Retrieved {Count} exams for date range", exams.Count());

                return Result<IEnumerable<ExamResponse>>.Success(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching exams by date range");
                return Result<IEnumerable<ExamResponse>>.Failure(ex.Message);
            }
        }
    }
}