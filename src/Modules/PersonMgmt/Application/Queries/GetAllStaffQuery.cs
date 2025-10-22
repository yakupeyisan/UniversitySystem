using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;

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
    /// <summary>
    /// GetAllStaffQuery Handler
    /// </summary>
    public class Handler : IRequestHandler<GetAllStaffQuery, Result<IEnumerable<PersonResponse>>>
    {
        public readonly IRepository<Person> _personRepository;
        public readonly IMapper _mapper;
        public readonly ILogger<Handler> _logger;

        public Handler(IRepository<Person> personRepository, IMapper mapper, ILogger<Handler> logger)
        {
            _personRepository = personRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<PersonResponse>>> Handle(GetAllStaffQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching all staff members");

                // Repository'den staff'leri getir (sadece Staff'leri)
                var staffPersons = await _personRepository.GetAllAsync(cancellationToken);
                var staffOnly = staffPersons.Where(p => p.Staff != null && !p.Staff.IsDeleted).ToList();

                // Response'a map et
                var responses = _mapper.Map<IEnumerable<PersonResponse>>(staffOnly);

                _logger.LogInformation("Retrieved {Count} staff members successfully", staffOnly.Count);

                return Result<IEnumerable<PersonResponse>>.Success(responses, "Staff members retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching staff members");
                return Result<IEnumerable<PersonResponse>>.Failure(ex.Message);
            }
        }
    }
}