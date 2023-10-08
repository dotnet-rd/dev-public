

using ConsumeWebApisUsingHttpClient.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ConsumeWebApisUsingHttpClient.Service
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsumeWebApisUsingHttpClient.Common
    {
        public class Helpers
        {
            public static FormUrlEncodedContent CreateFormUrlEncodedContent(Dictionary<string, string> keyValuePairs)
            {
                FormUrlEncodedContent formUrlContent;

                try
                {

                    var keyValueList = new List<KeyValuePair<string, string>>();

                    foreach (var kv in keyValuePairs)
                    {
                        keyValueList.Add(new KeyValuePair<string, string>(kv.Key, kv.Value));
                    }

                    formUrlContent = new FormUrlEncodedContent(keyValueList);

                    return formUrlContent;

                }
                catch (Exception)
                {

                    throw;
                }


            }
        }
    }

    public class ConsumeRestServices
    {

        public HttpResponseMessage PostFormUrlEncodingContents(Dictionary<string, string> formEncodingContent, string baseUri, string requestUri)
        {
            try
            {
                HttpResponseMessage responseMessage;

                // form FormUrlEncodedContent
                var requestBody = Helpers.CreateFormUrlEncodedContent(formEncodingContent);

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    responseMessage = client.PostAsync(baseUri + requestUri, requestBody).Result;
                }

                return responseMessage;

            }
            catch (Exception)
            {

                throw;
            }

        }
        //OR
        public HttpResponseMessage PostFormUrlEncodingStringContent(Dictionary<string, string> formEncodingContent, string baseUri, string requestUri)
        {
            try
            {
                HttpResponseMessage responseMessage;

                using (HttpClient client = new HttpClient())
                {

                    client.BaseAddress = new Uri(baseUri);
                    string requestContent = string.Empty;
                    var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
                    foreach (var content in formEncodingContent)
                    {
                        requestContent = requestContent + string.Format("{0}={1},", content.Key, Uri.EscapeDataString(content.Value));

                    }


                    request.Content = new StringContent(requestContent.TrimEnd(','), Encoding.UTF8, "application/x-www-form-urlencoded");

                    responseMessage = client.SendAsync(request).Result;
                    return responseMessage;

                }
            }
            catch (Exception)
            {

                throw;
            }


        }
        public HttpResponseMessage GetMethodOne(string baseUrl, string requestUri, string accessToken)
        {

            HttpResponseMessage responseMessage;
            try
            {
                using (var client = new HttpClient())
                {

                    HttpRequestMessage requestMsg = new HttpRequestMessage();

                    requestMsg.Headers.Accept.Clear();
                    requestMsg.Method = HttpMethod.Get;
                    requestMsg.RequestUri = new Uri(baseUrl + requestUri);
                    requestMsg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    requestMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    responseMessage = client.SendAsync(requestMsg).Result;

                    return responseMessage;
                }
            }
            catch (Exception)
            {

                throw;
            }


        }

        public HttpResponseMessage GetMethodTwo(string baseUrl, string requestUri, string accessToken)
        {

            HttpResponseMessage responseMessage;
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    responseMessage = client.GetAsync(baseUrl + requestUri).Result;

                    return responseMessage;
                }
            }
            catch (Exception)
            {

                throw;
            }


        }

        public HttpResponseMessage PostMethodOne(string baseUrl, string requestUri, string requestBody, string accessToken)
        {

            HttpResponseMessage responseMessage;
            try
            {
                using (var client = new HttpClient())
                {
                    Uri uri = new Uri(baseUrl + requestUri);

                    client.DefaultRequestHeaders.Accept.Clear();

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    responseMessage = client.PostAsync(uri, new StringContent(requestBody, Encoding.UTF8, "application/json")).Result;

                    return responseMessage;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public HttpResponseMessage PostMethodTwo(string baseUrl, string requestUri, string requestBody, string accessToken)
        {

            HttpResponseMessage responseMessage;
            try
            {
                using (var client = new HttpClient())
                {

                    HttpRequestMessage requestMsg = new HttpRequestMessage();

                    requestMsg.Headers.Accept.Clear();
                    requestMsg.RequestUri = new Uri(baseUrl + requestUri);
                    requestMsg.Method = HttpMethod.Post;
                    requestMsg.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                    requestMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    responseMessage = client.SendAsync(requestMsg).Result;

                    return responseMessage;
                }
            }
            catch (Exception)
            {

                throw;
            }


        }

        public HttpResponseMessage PutMethodOne(string baseUrl, string requestUri, string requestBody, string accessToken)
        {

            HttpResponseMessage responseMessage;
            try
            {
                using (var client = new HttpClient())
                {
                    Uri uri = new Uri(baseUrl + requestUri);

                    client.DefaultRequestHeaders.Accept.Clear();

                    // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    responseMessage = client.PutAsync(uri, new StringContent(requestBody, Encoding.UTF8, "application/json")).Result;

                    return responseMessage;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public HttpResponseMessage PutMethodTwo(string baseUrl, string requestUri, string requestBody, string accessToken)
        {
            HttpResponseMessage responseMessage;
            try
            {
                using (var client = new HttpClient())
                {

                    HttpRequestMessage requestMsg = new HttpRequestMessage();

                    requestMsg.Headers.Accept.Clear();
                    requestMsg.RequestUri = new Uri(baseUrl + requestUri);
                    requestMsg.Method = HttpMethod.Post;
                    requestMsg.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                    requestMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    responseMessage = client.SendAsync(requestMsg).Result;

                    return responseMessage;
                }
            }
            catch (Exception)
            {

                throw;
            }


        }

        public HttpResponseMessage DeleteOne(string baseUrl, string requestUri, int id, string accessToken)
        {

            HttpResponseMessage responseMessage;
            try
            {
                using (var client = new HttpClient())
                {

                    HttpRequestMessage requestMsg = new HttpRequestMessage();

                    requestMsg.Headers.Accept.Clear();
                    requestMsg.RequestUri = new Uri(baseUrl + requestUri + id);
                    requestMsg.Method = HttpMethod.Delete;

                    requestMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    responseMessage = client.SendAsync(requestMsg).Result;

                    return responseMessage;
                }
            }
            catch (Exception)
            {

                throw;
            }


        }

        public HttpResponseMessage DeleteTwo(string baseUrl, string requestUri, int id, string accessToken)
        {

            HttpResponseMessage responseMessage;
            try
            {
                using (var client = new HttpClient())
                {
                    Uri uri = new Uri(baseUrl + requestUri + id);

                    client.DefaultRequestHeaders.Accept.Clear();

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    responseMessage = client.DeleteAsync(uri).Result;

                    return responseMessage;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
