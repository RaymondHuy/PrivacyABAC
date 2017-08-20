using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PrivacyABAC.DbInterfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivacyABAC.WebAPI.Controllers
{
    [EnableCors("CorsPolicy")]
    public class AccountController : Controller
    {
        private readonly ISubjectRepository _subjectRepository;

        public AccountController(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        [HttpGet]
        [Route("api/accounts")]
        public string Get()
        {
            return _subjectRepository.GetAllUsers().ToString();
        }
    }
}
