using FluentResults;
using FluentValidation;
using MediatR;

namespace ProductReviewer.Application.Common.Behaviours
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>>
            where TRequest : IRequest<Result<TResponse>>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<Result<TResponse>> Handle(
            TRequest request,
            RequestHandlerDelegate<Result<TResponse>> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(e => e != null)
                .ToList();

            if (failures.Any())
            {
                var errors = failures.Select(f => new FluentResults.Error(f.ErrorMessage)).ToList();
                return Result.Fail<TResponse>(errors);
            }

            return await next();
        }
    }

}
