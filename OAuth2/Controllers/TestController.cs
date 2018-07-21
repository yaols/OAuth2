using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace OAuth2.Controllers
{
    public class TestController : Controller
    {

        private const string HOST_ADDRESS = "http://localhost:60903";
        private IDisposable _webApp;
        private static HttpClient _httpClient;


        public TestController()
        {
            //_webApp = WebApp.Start<Startup>(HOST_ADDRESS);
            Console.WriteLine("Web API started!");
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(HOST_ADDRESS);
            Console.WriteLine("HttpClient started!");

        }

        private static async Task<TokenResponse> GetToken(string grantType, string refreshToken = null, string userName = null, string password = null, string authorizationCode = null)
        {
            var clientId = "xishuai";
            var clientSecret = "123";
            var parameters = new Dictionary<string, string>();
            parameters.Add("grant_type", grantType);

            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                parameters.Add("username", userName);
                parameters.Add("password", password);
            }
            if (!string.IsNullOrEmpty(authorizationCode))
            {
                parameters.Add("code", authorizationCode);
                parameters.Add("redirect_uri", $"{HOST_ADDRESS}/api/authorization_code"); //和获取 authorization_code 的 redirect_uri 必须一致，不然会报错
            }
            if (!string.IsNullOrEmpty(refreshToken))
            {
                parameters.Add("refresh_token", refreshToken);
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(clientId + ":" + clientSecret)));

            var response = await _httpClient.PostAsync("/token", new FormUrlEncodedContent(parameters));
            var responseValue = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine((await response.Content.ReadAsAsync<HttpError>()).ExceptionMessage);
                return null;
            }
            return await response.Content.ReadAsAsync<TokenResponse>();
        }

        private  async Task<string> GetAuthorizationCode()
        {
            var clientId = "xishuai";

            var response = await _httpClient.GetAsync($"/authorize?grant_type=authorization_code&response_type=code&client_id={clientId}&redirect_uri={HttpUtility.UrlEncode("http://localhost:60903/Test/AuthorizationTest")}");
            var authorizationCode = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine((await response.Content.ReadAsAsync<HttpError>()).ExceptionMessage);
                return null;
            }
            return authorizationCode;
        }



        public async Task<ActionResult> AuthorizationTest(string code)
        {
            var authorizationCode = GetAuthorizationCode().Result; //获取 authorization_code
            var tokenResponse = GetToken("authorization_code", null, null, null, code).Result; //根据 authorization_code 获取 access_token
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            var response = await _httpClient.GetAsync($"/api/values");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine((await response.Content.ReadAsAsync<HttpError>()).ExceptionMessage);
            }
            Console.WriteLine(await response.Content.ReadAsStringAsync());
   
            var tokenResponseTwo = GetToken("refresh_token", tokenResponse.RefreshToken).Result; //根据 refresh_token 获取 access_token
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponseTwo.AccessToken);
            var responseTwo = await _httpClient.GetAsync($"/api/values");

            return View();
        }
    }
}