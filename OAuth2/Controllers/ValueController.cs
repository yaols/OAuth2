using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace OAuth2.Controllers
{
    public class ValueController : ApiController
    {
        // GET api/values
        [Authorize]
        [HttpGet]
        public IEnumerable<string> Index()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        [Route("api/authorization_code")]
        public HttpResponseMessage Get(string code)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(code, Encoding.UTF8, "text/plain")
            };
        }

        [HttpGet]
        [Route("api/access_token")]
        public HttpResponseMessage GetToken()
        {
            var url = Request.RequestUri;
            return new HttpResponseMessage()
            {
                Content = new StringContent("", Encoding.UTF8, "text/plain")
            };
        }

    }
}
