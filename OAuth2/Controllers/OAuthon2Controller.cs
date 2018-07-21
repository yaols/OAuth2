using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OAuth2.Controllers
{
    public class OAuthon2Controller : Controller
    {


        private const string HOST_ADDRESS = "http://localhost:60903";

        // GET: OAuthon2
        public string Index()
        {
            string clientId = "xishuai";
            string url = $"{HOST_ADDRESS}/authorize?grant_type=authorization_code&response_type=code&client_id={clientId}&redirect_uri={HttpUtility.UrlEncode($"{HOST_ADDRESS}/api/authorization_code")}";
            return url;
        }


        public string BaseString()
        {
            string clientId = "xishuai";
            string clientSecret = "123";
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(clientId + ":" + clientSecret));
        }

    }
}