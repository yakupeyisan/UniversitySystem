using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;

namespace PersonMgmt.Application.Commands;

/// <summary>
/// Kişi güncelleme command
/// 
/// Kullanım:
/// var command = new UpdatePersonCommand(personId, new UpdatePersonRequest { ... });
/// var result = await _mediator.Send(command);
/// </summary>
public class UpdatePersonCommand : IRequest<Result<PersonResponse>>
{
    /// <summary>
    /// Kişi ID
    /// </summary>
    public Guid PersonId { get; set; }

    /// <summary>
    /// Request verisi
    /// </summary>
    public UpdatePersonRequest Request { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public UpdatePersonCommand(Guid personId, UpdatePersonRequest request)
    {
        PersonId = personId;
        Request = request;
    }

    /// <summary>
    /// UpdatePersonCommand Handler
    /// </summary>
    public class Handler : IRequestHandler<UpdatePersonCommand, Result<PersonResponse>>
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

        public async Task<Result<PersonResponse>> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating person with ID: {PersonId}", request.PersonId);

                // Kişiyi ID'ye göre getir
                var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
                if (person == null)
                {
                    _logger.LogWarning("Person not found with ID: {PersonId}", request.PersonId);
                    return Result<PersonResponse>.Failure($"Person with ID {request.PersonId} not found");
                }

                // ✅ UpdatePersonalInfo() - email, phoneNumber, departmentId, profilePhotoUrl
                person.UpdatePersonalInfo(
                    email: request.Request.Email,
                    phoneNumber: request.Request.PhoneNumber,
                    departmentId: request.Request.DepartmentId,
                    profilePhotoUrl: request.Request.ProfilePhotoUrl);

                // Repository'ye kaydet
                await _personRepository.UpdateAsync(person, cancellationToken);

                // Response'a map et
                var response = _mapper.Map<PersonResponse>(person);

                _logger.LogInformation("Person updated successfully with ID: {PersonId}", person.Id);

                return Result<PersonResponse>.Success(response, "Person updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating person with ID: {PersonId}", request.PersonId);
                return Result<PersonResponse>.Failure(ex.Message);
            }
        }
    }
}