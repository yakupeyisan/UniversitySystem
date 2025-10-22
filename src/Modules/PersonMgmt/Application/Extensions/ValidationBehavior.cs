using FluentValidation;
using MediatR;

namespace PersonMgmt.Application.Extensions;

/// <summary>
/// MediatR Pipeline Behavior - Validation
/// 
/// Tüm command'lar bu behavior'dan geçer
/// ve otomatik olarak validate edilir
/// 
/// Eğer validation başarısız olursa
/// ValidationException throw edilir
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Validators
    /// </summary>
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Constructor
    /// </summary>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Handle - Validation logic
    /// </summary>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Validator'ları al
        var context = new ValidationContext<TRequest>(request);

        // Tüm validator'ları çalıştır
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Hataları topla
        var failures = validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .ToList();

        // Hata varsa exception throw et
        if (failures.Any())
        {
            throw new ValidationException(failures);
        }

        // Devam et
        return await next();
    }
}