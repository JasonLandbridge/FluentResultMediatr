using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using FluentResultsMediatr.Model;
using FluentValidation;
using MediatR;

namespace FluentResultsMediatr.Queries
{
    /// <summary>
    /// This works!
    /// </summary>
    public class GetWeatherForecastByIdNoTypedResultQuery: IRequest<Result>
    {
        public int Id { get; }

        public GetWeatherForecastByIdNoTypedResultQuery(int id)
        {
            Id = id;
        }
    }
    
    public class GetWeatherForecastByIdNoTypedResultQueryValidator : AbstractValidator<GetWeatherForecastByIdNoTypedResultQuery>
    {
        public GetWeatherForecastByIdNoTypedResultQueryValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
    
    public class GetWeatherForecastByIdNoTypedResultQueryHandler : IRequestHandler<GetWeatherForecastByIdNoTypedResultQuery, Result>
    {
        public async Task<Result> Handle(GetWeatherForecastByIdNoTypedResultQuery request, CancellationToken cancellationToken)
        {
           
            var weatherForecastDb = new List<WeatherForecast>();

            for (int i = 1; i < 11; i++)
            {
                weatherForecastDb.Add(new WeatherForecast
                {
                    Id = i,
                    Date = new DateTime().AddDays(i).AddYears(i * 10),
                    TemperatureC = i * 6,
                    Summary = $"This is a weather forecast for a city called {i}"
                });
            }

            WeatherForecast dbResult = weatherForecastDb.FirstOrDefault(x => x.Id == request.Id);
            if (dbResult != null)
            {
                return Result.Ok(dbResult);
            }

            return Result.Fail($"Could not find a weather forecast with id {request.Id}");
        }
    }
}

