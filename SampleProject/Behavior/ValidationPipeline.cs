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
    public class ValidationPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TResponse : ResultBase, new()
        // We need a common parent between Result and Result<T>,
        // and we need the new() constraint because we're going to instantiate the TResponse
        // The new() constraint does require a parameterless constructor in ResultBase,
        // otherwise the pipeline is not called
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

                // Instantiate the TResponse
                var f = new TResponse();
                // Add error, this is a bit hacky as I didn't have an easy way to add an error to ResultBase
                f.WithReason(error);
                // Returns non-null and valid result of type TResponse
                return f;
            }

            return await next();
        }
    }


}