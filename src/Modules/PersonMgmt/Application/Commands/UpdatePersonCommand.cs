using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Specifications;
namespace PersonMgmt.Application.Commands;
public class UpdatePersonCommand : IRequest<Result<PersonResponse>>
{
    public UpdatePersonCommand(Guid personId, UpdatePersonRequest request)
    {
        PersonId = personId;
        Request = request;
    }
    public Guid PersonId { get; set; }
    public UpdatePersonRequest Request { get; set; }
    public class Handler : IRequestHandler<UpdatePersonCommand, Result<PersonResponse>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<Person>
            _personRepository;
        public Handler(IRepository<Person>
            personRepository, IMapper mapper, ILogger<Handler> logger)
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
                var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
                if (person == null || person.IsDeleted)
                {
                    _logger.LogWarning("Person not found or is deleted with ID: {PersonId}", request.PersonId);
                    return Result<PersonResponse>.Failure("Person not found or has been deleted");
                }
                if (!string.IsNullOrEmpty(request.Request.Email) &&
                    request.Request.Email != person.Email)
                {
                    var isEmailUnique = await _personRepository.IsUniqueAsync(
                        new PersonByEmailSpecification(request.PersonId, request.Request.Email),
                        cancellationToken);
                    if (!isEmailUnique) return Result<PersonResponse>.Failure("Email already exists");
                }
                person.UpdatePersonalInfo(
                    request.Request.Email ?? person.Email,
                    request.Request.PhoneNumber ?? person.PhoneNumber,
                    request.Request.DepartmentId ?? person.DepartmentId,
                    request.Request.ProfilePhotoUrl ?? person.ProfilePhotoUrl);
                await _personRepository.UpdateAsync(person, cancellationToken);
                await _personRepository.SaveChangesAsync(cancellationToken);
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