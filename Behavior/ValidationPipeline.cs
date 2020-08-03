﻿using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using FluentValidation;
using MediatR;

namespace FluentResultsMediatr.Behavior
{
    public class ValidationPipeline<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TResponse : Result
        where TRequest : IRequest<Result>
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

                foreach (var validationFailure in result.Errors)
                {
                    error.Reasons.Add(new Error(validationFailure.ErrorMessage));
                }

                return Result.Fail(error) as TResponse;
            }

            return await next();
        }
    }


}