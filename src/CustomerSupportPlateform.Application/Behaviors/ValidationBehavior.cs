
using ConduitR.Abstractions;
using FluentValidation;
using FluentValidation.Results;

namespace CustomerSupportPlateform.Application.Behaviors;



public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest :notnull, IRequest<TResponse> 
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async ValueTask<TResponse> Handle(TRequest request, 
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        if (_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        IEnumerable<ValidationResult> validationResults = _validators.Select(x => x.Validate(context));
        var validationResultWithFailures = validationResults.SelectMany(x => x.Errors).Where(f=>f!=null);

        if (validationResultWithFailures.Any())
            throw new ValidationException(validationResultWithFailures);
        
        return await next();    

    }
}