using Core.Domain.Results;
using MediatR;
using PersonMgmt.Application.DTOs;

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
}