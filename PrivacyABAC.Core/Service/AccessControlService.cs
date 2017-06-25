using Newtonsoft.Json.Linq;
using PrivacyABAC.Core.Model;
using PrivacyABAC.DbInterfaces.Model;
using PrivacyABAC.DbInterfaces.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PrivacyABAC.Core.Service
{
    public class AccessControlService
    {
        private readonly IAccessControlPolicyRepository _accessControlPolicyRepository;
        private readonly ConditionalExpressionService _expressionService;

        public AccessControlService(
            IAccessControlPolicyRepository accessControlPolicyRepository,
            ConditionalExpressionService expressionService)
        {
            _accessControlPolicyRepository = accessControlPolicyRepository;
            _expressionService = expressionService;
        }

        ResponseContext ExecuteProcess(Subject subject, Resource resource, string action, EnvironmentObject environment)
        {
            environment.Data.AddAnnotation(action);

            AccessControlEffect effect = CollectionAccessControlProcess(subject, resource, action, environment);
            if (effect == AccessControlEffect.Deny)
                return new ResponseContext(AccessControlEffect.Deny, null);
            else if (effect == AccessControlEffect.Permit)
                return new ResponseContext(AccessControlEffect.Permit, resource.Data);

            var accessControlRecordPolicies = _accessControlPolicyRepository.Get(resource.Name, action, true);

            if (accessControlRecordPolicies.Count == 0)
                return new ResponseContext(AccessControlEffect.Deny, null);

            string policyCombining = _accessControlPolicyRepository.GetPolicyCombining(accessControlRecordPolicies);

            ICollection<JObject> _resource = new List<JObject>();
            if (resource.Data.Length > 1000)
            {
                Parallel.ForEach(resource.Data, record =>
                {
                    if (RowAccessControlProcess(subject, record, environment, policyCombining, accessControlRecordPolicies) != null)
                    {
                        lock (_resource)
                            _resource.Add(record);
                    }
                });
            }
            else
            {
                foreach (var record in resource.Data)
                {
                    if (RowAccessControlProcess(subject, record, environment, policyCombining, accessControlRecordPolicies) != null)
                        _resource.Add(record);
                }
            }
            if (_resource.Count == 0)
                return new ResponseContext(AccessControlEffect.Deny, null);

            return new ResponseContext(AccessControlEffect.Permit, _resource);

        }

        private AccessControlEffect CollectionAccessControlProcess(Subject subject, Resource resource, string action, EnvironmentObject environment)
        {
            AccessControlEffect result = AccessControlEffect.NotApplicable;

            ICollection<AccessControlPolicy> collectionPolicies = _accessControlPolicyRepository.Get(resource.Name, action, false);

            if (collectionPolicies.Count == 0)
                return AccessControlEffect.NotApplicable;

            string policyCombining = _accessControlPolicyRepository.GetPolicyCombining(collectionPolicies);

            var targetPolicies = new List<AccessControlPolicy>();
            foreach (var policy in collectionPolicies)
            {
                bool isTarget = _expressionService.Evaluate(policy.Target, subject.Data, null, environment.Data);
                if (isTarget)
                    targetPolicies.Add(policy);
            }

            foreach (var policy in targetPolicies)
            {
                string policyEffect = String.Empty;

                foreach (var rule in policy.Rules)
                {
                    bool isApplied = _expressionService.Evaluate(rule.Condition, subject.Data, null, environment.Data);
                    if (isApplied && rule.Effect.Equals("Permit") && policy.RuleCombining.Equals("permit-overrides"))
                    {
                        policyEffect = "Permit";
                        break;
                    }
                    if (isApplied && rule.Effect.Equals("Deny") && policy.RuleCombining.Equals("deny-overrides"))
                    {
                        policyEffect = "Deny";
                        break;
                    }
                }
                if (policyEffect.Equals("Permit") && policyCombining.Equals("permit-overrides"))
                {
                    result = AccessControlEffect.Permit;
                    break;
                }
                else if (policyEffect.Equals("Deny") && policyCombining.Equals("deny-overrides"))
                {
                    result = AccessControlEffect.Deny;
                    break;
                }
            }
            return result;
        }


        private JObject RowAccessControlProcess(Subject subject, JObject resource, EnvironmentObject environment, string policyCombining, ICollection<AccessControlPolicy> policies)
        {
            JObject result = null;
            var targetPolicy = new List<AccessControlPolicy>();
            foreach (var policy in policies)
            {
                bool isTarget = _expressionService.Evaluate(policy.Target, subject.Data, resource, environment.Data);
                if (isTarget)
                    targetPolicy.Add(policy);
            }
            foreach (var policy in targetPolicy)
            {
                string policyEffect = String.Empty;

                foreach (var rule in policy.Rules)
                {
                    bool isApplied = _expressionService.Evaluate(rule.Condition, subject.Data, resource, environment.Data);
                    if (isApplied && rule.Effect.Equals("Permit") && policy.RuleCombining.Equals("permit-overrides"))
                    {
                        policyEffect = "Permit";
                        break;
                    }
                    if (isApplied && rule.Effect.Equals("Deny") && policy.RuleCombining.Equals("deny-overrides"))
                    {
                        policyEffect = "Deny";
                        break;
                    }
                }
                if (policyEffect.Equals("Permit") && policyCombining.Equals("permit-overrides"))
                {
                    result = resource;
                    break;
                }
                else if (policyEffect.Equals("Deny") && policyCombining.Equals("deny-overrides"))
                {
                    result = null;
                    break;
                }
            }
            return result;
        }
    }
}
