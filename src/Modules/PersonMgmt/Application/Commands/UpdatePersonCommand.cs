using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Interfaces;

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
    public Guid PersonId { get; set; }
    public UpdatePersonRequest Request { get; set; }

    public UpdatePersonCommand(Guid personId, UpdatePersonRequest request)
    {
        PersonId = personId;
        Request = request;
    }

    public class Handler : IRequestHandler<UpdatePersonCommand, Result<PersonResponse>>
    {
        private readonly IPersonRepository _personRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(IPersonRepository personRepository, IMapper mapper, ILogger<Handler> logger)
        {
            _personRepository = personRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PersonResponse>> Handle(
            UpdatePersonCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating person with ID: {PersonId}", request.PersonId);

                // Kişiyi ID'ye göre getir
                var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);

                // ✅ FIX 1: SoftDelete check - kişi var mı ve silinmemiş mi?
                if (person == null || person.IsDeleted)
                {
                    _logger.LogWarning("Person not found or is deleted with ID: {PersonId}", request.PersonId);
                    return Result<PersonResponse>.Failure("Person not found or has been deleted");
                }

                // ✅ FIX 2: Email uniqueness check (değiştirilmiş mi?)
                if (!string.IsNullOrEmpty(request.Request.Email) &&
                    request.Request.Email != person.Email)
                {
                    var isEmailUnique = await _personRepository.IsEmailUniqueAsync(
                        request.Request.Email,
                        excludeId: request.PersonId,
                        cancellationToken: cancellationToken);

                    if (!isEmailUnique)
                    {
                        return Result<PersonResponse>.Failure("Email already exists");
                    }
                }

                // Kişi bilgilerini güncelle
                person.UpdatePersonalInfo(
                    email: request.Request.Email ?? person.Email,
                    phoneNumber: request.Request.PhoneNumber ?? person.PhoneNumber,
                    departmentId: request.Request.DepartmentId ?? person.DepartmentId,
                    profilePhotoUrl: request.Request.ProfilePhotoUrl ?? person.ProfilePhotoUrl);

                // Repository'de güncelle
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