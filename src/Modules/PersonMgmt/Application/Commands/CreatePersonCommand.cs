using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;
using PersonMgmt.Domain.Interfaces;
namespace PersonMgmt.Application.Commands;
public class CreatePersonCommand : IRequest<Result<PersonResponse>>
{
    public CreatePersonRequest Request { get; set; }
    public CreatePersonCommand(CreatePersonRequest request)
    {
        Request = request;
    }
    public class Handler : IRequestHandler<CreatePersonCommand, Result<PersonResponse>>
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
            CreatePersonCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating new person: {FirstName} {LastName}",
                    request.Request.FirstName, request.Request.LastName);
                var isIdentificationNumberUnique = await _personRepository.IsIdentificationNumberUniqueAsync(
                    request.Request.IdentificationNumber,
                    cancellationToken: cancellationToken);
                if (!isIdentificationNumberUnique)
                {
                    _logger.LogWarning("National ID already exists: {IdentificationNumber}", request.Request.IdentificationNumber);
                    return Result<PersonResponse>.Failure("National ID already exists");
                }
                var isEmailUnique = await _personRepository.IsEmailUniqueAsync(
                    request.Request.Email,
                    cancellationToken: cancellationToken);
                if (!isEmailUnique)
                {
                    _logger.LogWarning("Email already exists: {Email}", request.Request.Email);
                    return Result<PersonResponse>.Failure("Email already exists");
                }
                var gender = (Gender)request.Request.Gender;
                var person = Person.Create(
                    firstName: request.Request.FirstName,
                    lastName: request.Request.LastName,
                    identificationNumber: request.Request.IdentificationNumber,
                    birthDate: request.Request.BirthDate,
                    gender: gender,
                    email: request.Request.Email,
                    phoneNumber: request.Request.PhoneNumber,
                    departmentId: request.Request.DepartmentId,
                    profilePhotoUrl: request.Request.ProfilePhotoUrl);
                await _personRepository.AddAsync(person, cancellationToken);
                var response = _mapper.Map<PersonResponse>(person);
                _logger.LogInformation("Person created successfully with ID: {PersonId}", person.Id);
                return Result<PersonResponse>.Success(response, "Person created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating person");
                return Result<PersonResponse>.Failure(ex.Message);
            }
        }
    }
}