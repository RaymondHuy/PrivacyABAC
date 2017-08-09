using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PrivacyABAC.Core.JsonAC;
using System.Reflection;
using PrivacyABAC.Core.JsonAC.UserDefinedFunction;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PrivacyABAC.WebAPI.Controllers
{
    public class UserFunctionController : Controller
    {
        // GET: api/values
        [HttpGet]
        [Route("api/function")]
        public IEnumerable<string> Get()
        {
            var function = UserDefinedFunctionPluginFactory.GetInstance();
            return function.GetAllRegisteredFunction().Where(f=>f != "And" && f != "Or" && f != "Not");
        }
    }
}
