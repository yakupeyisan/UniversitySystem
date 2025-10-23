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
/// Kişiye kısıtlama ekleme command
/// 
/// Kullanım:
/// var command = new AddRestrictionCommand(personId, appliedBy, new AddRestrictionRequest { ... });
/// var result = await _mediator.Send(command);
/// </summary>
public class AddRestrictionCommand : IRequest<Result<Unit>>
{
    public Guid PersonId { get; set; }
    public Guid AppliedBy { get; set; }
    public AddRestrictionRequest Request { get; set; }

    public AddRestrictionCommand(Guid personId, Guid appliedBy, AddRestrictionRequest request)
    {
        PersonId = personId;
        AppliedBy = appliedBy;
        Request = request;
    }

    public class Handler : IRequestHandler<AddRestrictionCommand, Result<Unit>>
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

        public async Task<Result<Unit>> Handle(
            AddRestrictionCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Adding restriction to person with ID: {PersonId} by {AppliedBy}",
                    request.PersonId, request.AppliedBy);

                // Kişiyi ID'ye göre getir
                var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
                if (person == null)
                {
                    _logger.LogWarning("Person not found with ID: {PersonId}", request.PersonId);
                    return Result<Unit>.Failure("Person not found");
                }

                // ✅ FIX 1: EndDate validation - StartDate'den sonra olmalı
                if (request.Request.EndDate.HasValue &&
                    request.Request.EndDate <= request.Request.StartDate)
                {
                    _logger.LogWarning("EndDate must be after StartDate");
                    return Result<Unit>.Failure("End date must be after start date");
                }

                // byte'ları enum'a dönüştür
                var restrictionType = (RestrictionType)request.Request.RestrictionType;
                var restrictionLevel = (RestrictionLevel)request.Request.RestrictionLevel;

                // Kısıtlamayı ekle
                person.AddRestriction(
                    restrictionType: restrictionType,
                    restrictionLevel: restrictionLevel,
                    appliedBy: request.AppliedBy,
                    startDate: request.Request.StartDate,
                    endDate: request.Request.EndDate,
                    reason: request.Request.Reason,
                    severity: request.Request.Severity);

                // Repository'de güncelle
                await _personRepository.UpdateAsync(person, cancellationToken);

                // ✅ FIX 2: SaveChangesAsync() - CRITICAL!
                await _personRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Restriction added successfully to person with ID: {PersonId}", person.Id);

                return Result<Unit>.Success(Unit.Value, "Restriction added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding restriction to person with ID: {PersonId}", request.PersonId);
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}


