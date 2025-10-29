using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;
using PersonMgmt.Domain.Specifications;

namespace PersonMgmt.Application.Commands;

public class CreatePersonCommand : IRequest<Result<PersonResponse>>
{
    public CreatePersonCommand(CreatePersonRequest request)
    {
        Request = request;
    }

    public CreatePersonRequest Request { get; set; }

    public class Handler : IRequestHandler<CreatePersonCommand, Result<PersonResponse>>
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
            CreatePersonCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating new person: {FirstName} {LastName}",
                    request.Request.FirstName, request.Request.LastName);
                var isIdentificationNumberUnique = await _personRepository.IsUniqueAsync(
                    new PersonByIdentificationNumberSpecification(request.Request.IdentificationNumber),
                    cancellationToken);
                if (!isIdentificationNumberUnique)
                {
                    _logger.LogWarning("National ID already exists: {IdentificationNumber}",
                        request.Request.IdentificationNumber);
                    return Result<PersonResponse>.Failure("National ID already exists");
                }

                var isEmailUnique = await _personRepository.IsUniqueAsync(
                    new PersonByEmailSpecification(request.Request.Email),
                    cancellationToken);
                if (!isEmailUnique)
                {
                    _logger.LogWarning("Email already exists: {Email}", request.Request.Email);
                    return Result<PersonResponse>.Failure("Email already exists");
                }

                var gender = (Gender)request.Request.Gender;
                var person = Person.Create(
                    request.Request.FirstName,
                    request.Request.LastName,
                    request.Request.IdentificationNumber,
                    request.Request.BirthDate,
                    gender,
                    request.Request.Email,
                    request.Request.PhoneNumber,
                    request.Request.DepartmentId,
                    request.Request.ProfilePhotoUrl
                );
                await _personRepository.AddAsync(person, cancellationToken);
                await _personRepository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Person created successfully with ID: {PersonId}", person.Id);
                var response = _mapper.Map<PersonResponse>(person);
                return Result<PersonResponse>.Success(response, "Person created successfully");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Validation error while creating person");
                return Result<PersonResponse>.Failure($"Validation error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating person");
                return Result<PersonResponse>.Failure(ex.Message);
            }
        }
    }
}