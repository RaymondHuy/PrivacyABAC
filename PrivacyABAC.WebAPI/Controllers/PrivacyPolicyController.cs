using PrivacyABAC.WebAPI.Commands;
using PrivacyABAC.WebAPI.Utilities;
using PrivacyABAC.WebAPI.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrivacyABAC.Core.Service;
using PrivacyABAC.DbInterfaces.Repository;
using PrivacyABAC.DbInterfaces.Model;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Cors;

namespace PrivacyABAC.WebAPI.Controllers
{
    [EnableCors("CorsPolicy")]
    public class PrivacyPolicyController : Controller
    {
        private readonly SecurityService _securityService;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IResourceRepository _resourceRepository;
        private readonly IPrivacyDomainRepository _privacyDomainRepository;
        private readonly ConditionalExpressionService _conditionalExpressionService;
        private readonly IPrivacyPolicyRepository _privacyPolicyRepository;
        private readonly PrivacyService _privacyService;
        private readonly ILogger<PrivacyPolicyController> _logger;

        public PrivacyPolicyController(
            SecurityService securityService,
            ISubjectRepository subjectRepository,
            IResourceRepository resourceRepository,
            ConditionalExpressionService conditionalExpressionService,
            IPrivacyPolicyRepository privacyPolicyRepository,
            PrivacyService privacyService,
            IPrivacyDomainRepository privacyDomainRepository,
            ILogger<PrivacyPolicyController> logger)
        {
            _securityService = securityService;
            _subjectRepository = subjectRepository;
            _resourceRepository = resourceRepository;
            _conditionalExpressionService = conditionalExpressionService;
            _privacyPolicyRepository = privacyPolicyRepository;
            _privacyService = privacyService;
            _privacyDomainRepository = privacyDomainRepository;
            _logger = logger;
        }

        [HttpPost]
        [Route("api/Privacy/Check")]
        public string Check([FromBody]PrivacyCheckingCommand command)
        {
            _logger.LogInformation(DateTime.Now.Millisecond.ToString());
            var subject = _subjectRepository.GetUniqueUser("_id", command.UserID);
            var environment = string.IsNullOrEmpty(command.Environment) || command.Environment == "{}" ? null : JObject.Parse(command.Environment);
            var resource = _resourceRepository.GetCollectionDataWithCustomFilter(command.ResourceName, null);
            var action = command.Action;
            var result = _securityService.ExecuteProcess(subject, resource, action, command.ResourceName, environment);
            //if (result.Effect == EffectResult.Deny)
            //    return "Deny";
            //if (result.Effect == EffectResult.NotApplicable)
            //    return "Not Applicable";
            _logger.LogInformation(DateTime.Now.Millisecond.ToString());
            return result.Data == null ? "" : result.Data.ToString();
        }


        [HttpPost]
        [Route("api/PrivacyPolicy")]
        public void Create([FromBody]PrivacyPolicyInsertCommand command)
        {
            bool IsResourceRequired = false;

            if (command.Target.Contains("\"Resource."))
                IsResourceRequired = true;

            var fieldRules = new List<FieldRule>();
            var target = _conditionalExpressionService.Parse(command.Target);
            foreach (var rule in command.Rules)
            {
                var condition = _conditionalExpressionService.Parse(rule.Condition);
                var fieldRule = new FieldRule()
                {
                    Identifer = rule.RuleID,
                    FieldEffects = rule.FieldEffects,
                    Condition = condition
                };
                fieldRules.Add(fieldRule);

                if (!IsResourceRequired)
                    IsResourceRequired = rule.Condition.Contains("\"Resource.");
            }

            var policy = new PrivacyPolicy()
            {
                CollectionName = command.CollectionName,
                Description = command.Description,
                Id = command.PolicyID,
                Rules = fieldRules,
                IsAttributeResourceRequired = IsResourceRequired,
                Target = target
            };
            _privacyPolicyRepository.Add(policy);
        }
        [HttpPost]
        [Route("api/SubPrivacyPolicy")]
        public void Create([FromBody]SubPrivacyPolicyInsertCommand command)
        {
            bool IsResourceRequired = true;

            var fieldRules = new List<FieldRule>();
            foreach (var rule in command.Rules)
            {
                var condition = _conditionalExpressionService.Parse(rule.Condition);
                var fieldRule = new FieldRule()
                {
                    Identifer = rule.RuleID,
                    FieldEffects = rule.FieldEffects,
                    Condition = condition
                };
                fieldRules.Add(fieldRule);

                if (!IsResourceRequired)
                    IsResourceRequired = rule.Condition.Contains("\"Resource.");
            }

            var policy = new PrivacyPolicy()
            {
                CollectionName = command.CollectionName,
                Description = command.Description,
                Id = command.PolicyID,
                Rules = fieldRules,
                IsAttributeResourceRequired = IsResourceRequired
            };
            _privacyPolicyRepository.Add(policy);

            var priorty = new PriorityFunction() { Name = command.PolicyID, Priority = command.Priority };
            _privacyDomainRepository.AddPriorityFunctions(command.DomainName, priorty);

        }

        [HttpPost]
        [Route("api/Privacy/Review")]
        public IEnumerable<PrivacyPolicyViewModel> Review([FromBody]PolicyReviewCommand command)
        {
            JObject user = string.IsNullOrEmpty(command.UserJsonData) ? new JObject() : JObject.Parse(command.UserJsonData);
            JObject resource = string.IsNullOrEmpty(command.ResourceJsonData) ? new JObject() : JObject.Parse(command.ResourceJsonData);
            JObject environment = string.IsNullOrEmpty(command.EnvironmentJsonData) ? new JObject() : JObject.Parse(command.EnvironmentJsonData);

            var relativePolicies = _privacyService.Review(user, resource, environment);

            var result = new List<PrivacyPolicyViewModel>();
            foreach (var policy in relativePolicies)
            {
                result.Add(new PrivacyPolicyViewModel()
                {
                    CollectionName = policy.CollectionName,
                    Description = policy.Description,
                    PolicyId = policy.Id,
                    Target = FunctionUtility.Convert(policy.Target)
                });
            }
            return result;
        }

        [HttpGet]
        [Route("api/PrivacyPolicy")]
        public IEnumerable<PrivacyPolicyViewModel> PrivacyPolicy()
        {
            var policies = _privacyPolicyRepository.GetAll();
            var result = new List<PrivacyPolicyViewModel>();
            foreach (var policy in policies)
            {
                result.Add(new PrivacyPolicyViewModel()
                {
                    CollectionName = policy.CollectionName,
                    Description = policy.Description,
                    PolicyId = policy.Id,
                    Target = FunctionUtility.Convert(policy.Target)
                });
            }
            return result;
        }

        [HttpDelete]
        [Route("api/PrivacyPolicy")]
        public void PrivacyPolicy(string policyID)
        {
            _privacyPolicyRepository.Delete(policyID);
        }

        [HttpGet]
        [Route("api/ArrayFields")]
        public IEnumerable<string> ArrayFields()
        {
            return _resourceRepository.GetArrayFieldName();
        }
    }
}
