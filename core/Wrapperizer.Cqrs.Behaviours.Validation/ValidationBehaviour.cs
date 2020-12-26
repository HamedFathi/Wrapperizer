using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Wrapperizer.Domain.Abstractions;

namespace Wrapperizer.Cqrs.Behaviours.Validation
{
    public sealed class ValidationBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, ViewResult<TResponse>>
    {
        private readonly ILogger<ValidationBehaviour<TRequest, TResponse>> _logger;

        public ValidationBehaviour(ILogger<ValidationBehaviour<TRequest,TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<ViewResult<TResponse>> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<ViewResult<TResponse>> next)
        {
            if (next == null)
                throw new ArgumentNullException(nameof(next));
            
            var validationResults = new List<ValidationResult>();
            if (request is IValidatableObject validatableObject)
            {
                validationResults.AddRange(
                    validatableObject.Validate(
                        new ValidationContext(request)).ToList()
                );
            }

            if (!validationResults.Any()) return await next();
            
            // ToDO: raise domain events as well
            // var validationErrorHappened = new ValidationErrorHappened<TRequest>(validationResults);
            // _domainEventManager.Publish(validationErrorHappened);

            var viewResult = ViewResult.Fail();
            validationResults.ForEach(v =>
            {
                viewResult.Fail(v.ErrorMessage);
            });

            return viewResult;
        }
    }
}
