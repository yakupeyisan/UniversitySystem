using FluentValidation;
using MediatR;

namespace Core.Application.Behaviors;

/// <summary>
/// ValidationBehavior - MediatR Pipeline
/// 
/// Sorumluluğu:
/// - Tüm Command/Query'ler execute edilmeden ÖNCE validate et
/// - ValidationException throw et (hata varsa)
/// 
/// Avantajlar:
/// - Cross-cutting concern'ı merkezi bir yerde handle etme
/// - Her command/query handler'da manual validation yazmasına gerek yok
/// - DRY prensibine uygun
/// 
/// Örnek:
/// var command = new CreatePersonCommand(...);  // Invalid
/// await mediator.Send(command);  // ValidationException thrown
/// 
/// Kullanım (Program.cs):
/// services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Pipeline'ı handle et
    /// </summary>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Validator'lar varsa çalıştır
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            // Tüm validators'ları çalıştır
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            // Hatalar varsa ValidationException throw et
            var failures = validationResults
                .Where(r => r.Errors.Count != 0)
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Count != 0)
                throw new ValidationException(failures);
        }

        // Validation başarılı, handler'ı çalıştır
        return await next();
    }
}