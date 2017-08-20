using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using PrivacyABAC.Functions;
using Microsoft.AspNetCore.Cors;

namespace PrivacyABAC.WebAPI.Controllers
{
    [EnableCors("CorsPolicy")]
    public class UserFunctionController : Controller
    {
        [HttpGet]
        [Route("api/function")]
        public IEnumerable<string> Get()
        {
            var function = UserDefinedFunctionFactory.GetInstance();
            return function.GetAllFunctionNames();
        }
    }
}
