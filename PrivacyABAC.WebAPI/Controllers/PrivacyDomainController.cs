using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PrivacyABAC.WebAPI.Commands;
using PrivacyABAC.DbInterfaces.Repository;
using PrivacyABAC.Domains.Common;
using PrivacyABAC.DbInterfaces.Model;
using Microsoft.AspNetCore.Cors;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PrivacyABAC.WebAPI.Controllers
{
    [EnableCors("CorsPolicy")]
    public class PrivacyDomainController : Controller
    {
        private readonly IPrivacyDomainRepository _privacyDomainRepository;

        public PrivacyDomainController(IPrivacyDomainRepository privacyDomainRepository)
        {
            _privacyDomainRepository = privacyDomainRepository;
        }


        [HttpGet]
        [Route("api/PrivacyFunctions")]
        public IEnumerable<string> GetPrivacyFunctions()
        {
            return _privacyDomainRepository.GetAllPrivacyFunctionName();
        }

        [HttpGet]
        [Route("api/UnregisteredPrivacyFunctions")]
        public IEnumerable<string> GetUnRegisteredPrivacyFunctions()
        {
            var plugin = PrivacyDomainPluginFactory.GetInstance();
            return plugin.GetAllDomainType();
        }

        [HttpGet]
        [Route("api/PrivacyFunction")]
        public IEnumerable<string> GetPrivacyFunction(string name)
        {
            return _privacyDomainRepository.GetPrivacyFunctionNames(name);
        }

        [HttpGet]
        [Route("api/PrivacyDomain")]
        public IEnumerable<string> GetAllPrivacyDomain()
        {
            var container = PrivacyDomainPluginFactory.GetInstance();

            return container.GetAllDomainType();
        }
        [HttpGet]
        [Route("api/PrivacyDomainField")]
        public IEnumerable<PrivacyDomain> GetPrivacyDomainFields()
        {
            return _privacyDomainRepository.GetAll();
        }

        [HttpPost]
        [Route("api/DomainField")]
        public void UpdateDomainField([FromBody]PrivacyDomainFieldUpdateCommand command)
        {
            _privacyDomainRepository.UpdateDomainField(command.DomainName, command.FieldName);
        }

        [HttpPost]
        [Route("api/PriorityFunctions")]
        public void UpdatePriorityFunctions([FromBody]UpdatePriorityFunctionCommand command)
        {
            _privacyDomainRepository.UpdatePriorityFunctions(command.DomainName, command.PriorityFunctions);
        }


        [HttpPost]
        [Route("api/PrivacyDomain")]
        public void PrivacyDomain([FromBody]string name)
        {
            _privacyDomainRepository.Add(
                new PrivacyDomain()
                {
                    DomainName = name,
                    Functions = new PriorityFunction[0],
                    Fields = new string[0],
                    IsArrayFieldDomain = false
                });
            return;
        }

        [HttpPost]
        [Route("api/PrivacyDomainFunction")]
        public void PrivacyDomain([FromBody]InsertPriorityFunctionCommand command)
        {
            _privacyDomainRepository.AddPriorityFunctions(command.DomainName, command.Priority);
        }
    }
}
