using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace OAuth2.App_Start
{
    public class HomeController : Controller
    {
        // GET api/values
        public IEnumerable<string> Index()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
