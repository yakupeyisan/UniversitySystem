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

/// <summary>
/// Kişi oluşturma command
/// 
/// Kullanım:
/// var command = new CreatePersonCommand(new CreatePersonRequest { ... });
/// var result = await _mediator.Send(command);
/// </summary>
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

                // ✅ FIX 1: Duplicate NationalId check
                var isNationalIdUnique = await _personRepository.IsNationalIdUniqueAsync(
                    request.Request.NationalId,
                    cancellationToken: cancellationToken);

                if (!isNationalIdUnique)
                {
                    _logger.LogWarning("National ID already exists: {NationalId}", request.Request.NationalId);
                    return Result<PersonResponse>.Failure("National ID already exists");
                }

                // ✅ FIX 2: Email uniqueness check (bonus)
                var isEmailUnique = await _personRepository.IsEmailUniqueAsync(
                    request.Request.Email,
                    cancellationToken: cancellationToken);

                if (!isEmailUnique)
                {
                    _logger.LogWarning("Email already exists: {Email}", request.Request.Email);
                    return Result<PersonResponse>.Failure("Email already exists");
                }

                // Gender byte'ı enum'a dönüştür
                var gender = (Gender)request.Request.Gender;

                // Domain entity'yi request'ten oluştur
                var person = Person.Create(
                    firstName: request.Request.FirstName,
                    lastName: request.Request.LastName,
                    nationalId: request.Request.NationalId,
                    birthDate: request.Request.BirthDate,
                    gender: gender,
                    email: request.Request.Email,
                    phoneNumber: request.Request.PhoneNumber,
                    departmentId: request.Request.DepartmentId,
                    profilePhotoUrl: request.Request.ProfilePhotoUrl);

                // Repository'ye ekle
                await _personRepository.AddAsync(person, cancellationToken);

                // ✅ FIX 3: SaveChangesAsync() - CRITICAL!
                await _personRepository.SaveChangesAsync(cancellationToken);

                // Response'a map et
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
