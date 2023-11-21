using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class YourApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IApiStrategyFactory _apiStrategyFactory;

        public YourApiController(IConfiguration configuration, IApiStrategyFactory apiStrategyFactory)
        {
            _configuration = configuration;
            _apiStrategyFactory = apiStrategyFactory;
        }

        [HttpPost("CallApi")]
        public IActionResult CallApi([FromBody] ApiRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var countryCode = request.CountryCode;
            var status = request.Status;
            var requestData = request.Data;

            var endpointStrategy = _apiStrategyFactory.GetStrategy(countryCode);
            if (endpointStrategy != null)
            {
                var apiCaller = new ApiCaller<string>(endpointStrategy);

                // Execute API call based on the provided status and data
                apiCaller.ExecuteApiCall(status, requestData);

                return Ok("API call executed successfully"); // You might return other responses as per your application logic
            }
            else
            {
                return NotFound("No configuration found for the provided country code");
            }
        }
    }

    // YourRequestModel structure to accept data in the API request body
    public class ApiRequestModel
    {
        public string CountryCode { get; set; }
        public string Status { get; set; }
        public string Data { get; set; }
        // Other properties as per your request structure
    }

    // Define API configurations for different countries
    public class CountryApiConfiguration
    {
        public string AuthenticationEndpoint { get; set; }
        // Other API configuration properties
    }

    // Strategy interface for API calls
    public interface IApiCallStrategy<T>
    {
        void ExecuteApiCall(string status, T data);
    }

    // Concrete strategy for a specific endpoint
    public class EndpointApiCallStrategy : IApiCallStrategy<string>
    {
        private readonly string _endpointUrl;
        private readonly string _authEndpoint;

        public EndpointApiCallStrategy(string endpointUrl, string authEndpoint)
        {
            _endpointUrl = endpointUrl;
            _authEndpoint = authEndpoint;
        }

        public void ExecuteApiCall(string status, string data)
        {
            var authToken = GetAuthToken(_authEndpoint); // Obtain token from authentication endpoint

            // Perform API call using the determined endpoint URL, token, and data
            Console.WriteLine($"Called API, Status: {status}, Endpoint URL: {_endpointUrl}, Token: {authToken}, Data: {data}");
        }

        private string GetAuthToken(string authEndpoint)
        {
            // Implement logic to obtain authentication token from the provided endpoint
            Console.WriteLine($"Obtaining token from Authentication Endpoint: {authEndpoint}");
            return "sample_auth_token";
        }
    }

    // Context class representing API calling based on strategy
    public class ApiCaller<T>
    {
        private readonly IApiCallStrategy<T> apiCallStrategy;

        public ApiCaller(IApiCallStrategy<T> strategy)
        {
            apiCallStrategy = strategy;
        }

        public void ExecuteApiCall(string status, T data)
        {
            apiCallStrategy?.ExecuteApiCall(status, data);
        }
    }

    // Strategy Factory interface
    public interface IApiStrategyFactory
    {
        IApiCallStrategy<string> GetStrategy(string countryCode);
    }

    // Concrete factory implementing the creation of strategies
    public class ApiStrategyFactory : IApiStrategyFactory
    {
        private readonly Dictionary<string, IApiCallStrategy<string>> _strategies;

        public ApiStrategyFactory()
        {
            _strategies = new Dictionary<string, IApiCallStrategy<string>>
            {
                { "US", new EndpointApiCallStrategy("Endpoint URL for US", "US Authentication Endpoint") },
                { "UK", new EndpointApiCallStrategy("Endpoint URL for UK", "UK Authentication Endpoint") },
                // Add more country codes and corresponding strategies as needed
            };
        }

        public IApiCallStrategy<string> GetStrategy(string countryCode)
        {
            if (_strategies.TryGetValue(countryCode, out var strategy))
            {
                return strategy;
            }

            return null;
        }
    }
}
