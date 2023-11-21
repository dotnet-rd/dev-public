using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class YourApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IApiCallStrategyFactory _apiCallStrategyFactory;

        public YourApiController(IConfiguration configuration, IApiCallStrategyFactory apiCallStrategyFactory)
        {
            _configuration = configuration;
            _apiCallStrategyFactory = apiCallStrategyFactory;
        }

        [HttpPost("CallApi")]
        public IActionResult CallApi([FromBody] ApiRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var countryCode = request.CountryCode;
            var apiName = request.ApiName;
            var status = request.Status;
            var requestData = request.Data;

            var strategy = _apiCallStrategyFactory.GetStrategy(countryCode, apiName, status);

            if (strategy != null)
            {
                var apiCaller = new ApiCaller<string, string>(strategy);

                // Execute API actions based on the provided data
                var result = apiCaller.ExecuteApiActions(requestData);

                return Ok(result);
            }
            else
            {
                return NotFound("No configuration found for the provided country code, API name, or status");
            }
        }
    }

    public class ApiRequestModel
    {
        public string CountryCode { get; set; }
        public string ApiName { get; set; }
        public string Status { get; set; }
        public string Data { get; set; }
        // Other properties as per your request structure
    }

    public interface IApiCallStrategy<TInput, TOutput>
    {
        TOutput Execute(TInput data);
    }

    public interface IApiCallStrategyFactory
    {
        IApiCallStrategy<string, string> GetStrategy(string countryCode, string apiName, string status);
    }

    public class ApiCaller<TInput, TOutput>
    {
        private readonly IApiCallStrategy<TInput, TOutput> apiCallStrategy;

        public ApiCaller(IApiCallStrategy<TInput, TOutput> strategy)
        {
            apiCallStrategy = strategy;
        }

        public TOutput ExecuteApiActions(TInput data)
        {
            return apiCallStrategy.Execute(data);
        }
    }

    public class ApiCallStrategyFactory : IApiCallStrategyFactory
    {
        public IApiCallStrategy<string, string> GetStrategy(string countryCode, string apiName, string status)
        {
            if (countryCode == "US")
            {
                if (apiName == "API1")
                {
                    return new USApi1CallStrategy(status);
                }
                // Add other API strategies for US...
            }
            else if (countryCode == "UK")
            {
                if (apiName == "API1")
                {
                    return new UKApi1CallStrategy(status);
                }
                // Add other API strategies for UK...
            }

            return null; // Return null for unknown configurations
        }
    }

    public class USApi1CallStrategy : IApiCallStrategy<string, string>
    {
        private readonly string _status;

        public USApi1CallStrategy(string status)
        {
            _status = status;
        }

        public string Execute(string data)
        {
            // Implement status-based logic here
            if (_status == "activate")
            {
                Console.WriteLine($"Called Activate method for US API1, Data: {data}");
                return "Activation successful";
            }
            else if (_status == "block")
            {
                Console.WriteLine($"Called Block method for US API1, Data: {data}");
                return "Blocked successfully";
            }
            else if (_status == "unblock")
            {
                Console.WriteLine($"Called Unblock method for US API1, Data: {data}");
                return "Unblocked successfully";
            }
            else
            {
                throw new ArgumentException($"Unknown status: {_status}");
            }
        }
    }

    // Other concrete strategy classes...

    // YourApiController remains the same
}
