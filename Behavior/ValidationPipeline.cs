using System;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using FluentResultsMediatr.Model;
using FluentValidation;
using MediatR;
using Microsoft.VisualBasic;

namespace FluentResultsMediatr.Behavior
{
    public class ValidationPipeline<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TResponse : ResultBase
    {
        private readonly IValidator<TRequest> _compositeValidator;

        public ValidationPipeline(IValidator<TRequest> compositeValidator)
        {
            _compositeValidator = compositeValidator;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {

            var result = await _compositeValidator.ValidateAsync(request, cancellationToken);

            if (!result.IsValid)
            {
                Error error = new Error();
                var responseType = typeof(TResponse);
                Type[] typeParameters = responseType.GetGenericArguments();

                foreach (var validationFailure in result.Errors)
                {
                    error.Reasons.Add(new Error(validationFailure.ErrorMessage));
                }

                // Hard coding the type parameter does convert to TResponse correctly
                var conversion = Result.Fail<WeatherForecast>(error) as TResponse;
                return conversion;
            }

            return await next();
        }
    }


}