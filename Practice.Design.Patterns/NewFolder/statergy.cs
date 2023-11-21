using System;
using System.Collections.Generic;

namespace Strategy.RealWorld
{
	// Define API configurations for different countries
	public class CountryApiConfiguration
	{
		public string ApiUrl { get; set; }
		// Other API configuration properties
	}

	// Strategy interface for API calls
	public interface IApiCallStrategy<T>
	{
		void ExecuteApiCall(List<T> data);
	}

	// Different strategies for API calls
	public class USApiCallStrategy : IApiCallStrategy<string>
	{
		private readonly CountryApiConfiguration _config;

		public USApiCallStrategy(CountryApiConfiguration config)
		{
			_config = config;
		}

		public void ExecuteApiCall(List<string> data)
		{
			// Implement logic to call US API using '_config' and 'data'
			Console.WriteLine("Called US API");
		}
	}

	public class UKApiCallStrategy : IApiCallStrategy<string>
	{
		private readonly CountryApiConfiguration _config;

		public UKApiCallStrategy(CountryApiConfiguration config)
		{
			_config = config;
		}

		public void ExecuteApiCall(List<string> data)
		{
			// Implement logic to call UK API using '_config' and 'data'
			Console.WriteLine("Called UK API");
		}
	}

	public class OtherApiCallStrategy : IApiCallStrategy<string>
	{
		private readonly CountryApiConfiguration _config;

		public OtherApiCallStrategy(CountryApiConfiguration config)
		{
			_config = config;
		}

		public void ExecuteApiCall(List<string> data)
		{
			// Implement logic to call another API using '_config' and 'data'
			Console.WriteLine("Called another API");
		}
	}

	// Context class representing API calling based on strategy
	public class ApiCaller<T>
	{
		private readonly List<T> data = new List<T>();
		private IApiCallStrategy<T> apiCallStrategy;

		public void SetApiCallStrategy(IApiCallStrategy<T> strategy)
		{
			apiCallStrategy = strategy;
		}

		public void AddData(T item)
		{
			data.Add(item);
		}

		public void ExecuteApiCall()
		{
			apiCallStrategy?.ExecuteApiCall(data);
		}
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			// Create configurations for different countries
			var usConfig = new CountryApiConfiguration { ApiUrl = "US API URL" };
			var ukConfig = new CountryApiConfiguration { ApiUrl = "UK API URL" };
			var otherConfig = new CountryApiConfiguration { ApiUrl = "Another API URL" };

			// Create an API caller for different countries with respective configurations

			var usApiCaller = new ApiCaller<string>();
			usApiCaller.SetApiCallStrategy(new USApiCallStrategy(usConfig));

			var ukApiCaller = new ApiCaller<string>();
			ukApiCaller.SetApiCallStrategy(new UKApiCallStrategy(ukConfig));

			var otherApiCaller = new ApiCaller<string>();
			otherApiCaller.SetApiCallStrategy(new OtherApiCallStrategy(otherConfig));

			// Add data and execute API calls based on strategy

			usApiCaller.AddData("US data");
			usApiCaller.ExecuteApiCall(); // Calls US API

			ukApiCaller.AddData("UK data");
			ukApiCaller.ExecuteApiCall(); // Calls UK API

			otherApiCaller.AddData("Other data");
			otherApiCaller.ExecuteApiCall(); // Calls another API

			Console.ReadKey();
		}
	}
}
