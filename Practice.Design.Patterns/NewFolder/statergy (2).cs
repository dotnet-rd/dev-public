using System;
using System.Collections.Generic;

namespace Strategy.RealWorld
{
    // Define API configurations for different countries
    public class CountryApiConfiguration
    {
        public Dictionary<string, string> AuthenticationEndpoints { get; set; }
        // Other API configuration properties
    }

    // Strategy interface for API calls
    public interface IApiCallStrategy<T>
    {
        void ExecuteApiCall(string status, T data);
    }

    // Concrete strategy for a specific endpoint
    public class Endpoint1ApiCallStrategy : IApiCallStrategy<string>
    {
        private readonly string _endpointUrl;
        private readonly string _authEndpoint;

        public Endpoint1ApiCallStrategy(string endpointUrl, string authEndpoint)
        {
            _endpointUrl = endpointUrl;
            _authEndpoint = authEndpoint;
        }

        public void ExecuteApiCall(string status, string data)
        {
            var authToken = GetAuthToken(_authEndpoint); // Obtain token from authentication endpoint

            // Perform API call using the determined endpoint URL, token, and data
            Console.WriteLine($"Called API for Endpoint 1, Status: {status}, Endpoint URL: {_endpointUrl}, Token: {authToken}, Data: {data}");
        }

        private string GetAuthToken(string authEndpoint)
        {
            // Implement logic to obtain authentication token from the provided endpoint
            Console.WriteLine($"Obtaining token from Authentication Endpoint: {authEndpoint}");
            return "sample_auth_token";
        }
    }

    // Concrete strategy for another specific endpoint
    public class Endpoint2ApiCallStrategy : IApiCallStrategy<string>
    {
        private readonly string _endpointUrl;
        private readonly string _authEndpoint;

        public Endpoint2ApiCallStrategy(string endpointUrl, string authEndpoint)
        {
            _endpointUrl = endpointUrl;
            _authEndpoint = authEndpoint;
        }

        public void ExecuteApiCall(string status, string data)
        {
            var authToken = GetAuthToken(_authEndpoint); // Obtain token from authentication endpoint

            // Perform API call using the determined endpoint URL, token, and data
            Console.WriteLine($"Called API for Endpoint 2, Status: {status}, Endpoint URL: {_endpointUrl}, Token: {authToken}, Data: {data}");
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
        private IApiCallStrategy<T> apiCallStrategy;

        public void SetApiCallStrategy(IApiCallStrategy<T> strategy)
        {
            apiCallStrategy = strategy;
        }

        public void ExecuteApiCall(string status, T data)
        {
            apiCallStrategy?.ExecuteApiCall(status, data);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            // Create configurations for different endpoints with authentication endpoints
            var endpoint1Config = new CountryApiConfiguration
            {
                AuthenticationEndpoints = new Dictionary<string, string>
                {
                    { "US", "US Authentication Endpoint" },
                }
            };

            var endpoint2Config = new CountryApiConfiguration
            {
                AuthenticationEndpoints = new Dictionary<string, string>
                {
                    { "UK", "UK Authentication Endpoint" },
                }
            };

            // Create an API caller with a strategy determined by the country code
            var apiCaller = new ApiCaller<string>();

            string countryCode = "US"; // Example country code from the API request body
            string status = "Active"; // Example status from the API request body
            string requestData = "Sample Request Data"; // Example request data

            if (countryCode == "US")
            {
                var strategy = new Endpoint1ApiCallStrategy("Endpoint1 URL", endpoint1Config.AuthenticationEndpoints["US"]);
                apiCaller.SetApiCallStrategy(strategy);
            }
            else if (countryCode == "UK")
            {
                var strategy = new Endpoint2ApiCallStrategy("Endpoint2 URL", endpoint2Config.AuthenticationEndpoints["UK"]);
                apiCaller.SetApiCallStrategy(strategy);
            }
            else
            {
                Console.WriteLine("No strategy found for the provided country code");
            }

            // Execute API call based on the provided status and data
            apiCaller.ExecuteApiCall(status, requestData);

            Console.ReadKey();
        }
    }
}
