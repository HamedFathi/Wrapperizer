using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wrapperizer.Core.Abstraction;
using Wrapperizer.Core.Abstraction.Specifications;

namespace Wrapperizer.Sample.Api.Queries
{
    public sealed class PastDateError : DomainError
    {
        public override string Message => "The specified date is past.";
    }

    public class NotPastSpecification : Specification<GetWeatherForecast>
    {
        public override DomainError Error => new PastDateError();

        public override Expression<Func<GetWeatherForecast, bool>> ToExpression()
        {
            return g => g.DateTime >= DateTime.Now;
        }
    }

    public sealed class GetWeatherForecast : IQuery<IReadOnlyCollection<WeatherForecast>>
    {
        public DateTime DateTime { get; set; }

        public sealed class
            GetWeatherForecastHandler : IQueryHandler<GetWeatherForecast, IReadOnlyCollection<WeatherForecast>>
        {
            private readonly ICrudRepository<WeatherForecast> _repository;

            public GetWeatherForecastHandler(
                ICrudRepository<WeatherForecast> repository) =>
                _repository = repository;

            public Task<IReadOnlyCollection<WeatherForecast>> Handle(
                GetWeatherForecast request, CancellationToken cancellationToken)
                => _repository.FindBy(_ => true);
        }
    }
}
