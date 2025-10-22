using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;

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
    /// <summary>
    /// Request verisi
    /// </summary>
    public CreatePersonRequest Request { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public CreatePersonCommand(CreatePersonRequest request)
    {
        Request = request;
    }

    /// <summary>
    /// CreatePersonCommand Handler
    /// </summary>
    public class Handler : IRequestHandler<CreatePersonCommand, Result<PersonResponse>>
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

        public async Task<Result<PersonResponse>> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating new person: {FirstName} {LastName}",
                    request.Request.FirstName, request.Request.LastName);

                // Gender byte'ı enum'a dönüştür
                var gender = (Gender)request.Request.Gender;

                // Domain entity'yi request'ten oluştur
                var person = Person.Create(
                    firstName: request.Request.FirstName,
                    lastName: request.Request.LastName,
                    nationalId: request.Request.NationalId,
                    birthDate: request.Request.BirthDate,  // ✅ BirthDate (DateOfBirth değil)
                    gender: gender,
                    email: request.Request.Email,
                    phoneNumber: request.Request.PhoneNumber,
                    departmentId: request.Request.DepartmentId,
                    profilePhotoUrl: request.Request.ProfilePhotoUrl);

                // Repository'ye kaydet
                await _personRepository.AddAsync(person, cancellationToken);

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