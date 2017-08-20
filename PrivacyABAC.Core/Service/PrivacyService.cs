using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PrivacyABAC.DbInterfaces.Model;
using PrivacyABAC.DbInterfaces.Repository;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PrivacyABAC.Core.Extension;
using PrivacyABAC.Core.Model;

namespace PrivacyABAC.Core.Service
{
    public class PrivacyService
    {
        private readonly ConditionalExpressionService _expressionService;
        private readonly IPrivacyDomainRepository _privacyDomainRepository;
        private readonly IPrivacyPolicyRepository _privacyPolicyRepository;
        private readonly ILogger<PrivacyService> _logger;

        private IDictionary<string, string> _collectionPrivacyRules;

        public PrivacyService(
            ConditionalExpressionService expressionService,
            IPrivacyDomainRepository privacyDomainRepository,
            IPrivacyPolicyRepository privacyPolicyRepository,
            ILogger<PrivacyService> logger)
        {
            _expressionService = expressionService;
            _privacyDomainRepository = privacyDomainRepository;
            _privacyPolicyRepository = privacyPolicyRepository;
            _logger = logger;
        }

        public ResponseContext ExecuteProcess(Subject subject, Resource resource, string action, EnvironmentObject environment)
        {
            environment.Data.AddAnnotation(action);

            _collectionPrivacyRules = GetFieldCollectionRules(subject, resource, action, environment);
            var privacyRecords = new JArray();
            int count = 0;
            if (resource.Data.Length > 1000)
            {
                Parallel.ForEach(resource.Data, record =>
                {
                    var privacyFields = GetPrivacyRecordField(subject, record, resource.Name, environment);
                    if (privacyFields.Count > 0)
                    {
                        var privacyRecord = PrivacyProcessing(record, privacyFields, subject, environment);
                        lock (privacyRecords)
                            privacyRecords.Add(privacyRecord);
                        ++count;
                    }
                });
            }
            else
            {
                foreach (var record in resource.Data)
                {
                    var privacyFields = GetPrivacyRecordField(subject, record, resource.Name, environment);
                    if (privacyFields.Count > 0)
                    {
                        var privacyRecord = PrivacyProcessing(record, privacyFields, subject, environment);
                        privacyRecords.Add(privacyRecord);
                        ++count;
                    }
                }
            }
            if (privacyRecords.Count == 0)
                return new ResponseContext(AccessControlEffect.Permit, privacyRecords, "No privacy rules is satisfied");

            return new ResponseContext(AccessControlEffect.Permit, privacyRecords);
        }

        public ICollection<PrivacyPolicy> Review(JObject user, JObject resource, JObject environment)
        {
            var policies = _privacyPolicyRepository.GetAll();
            var result = new List<PrivacyPolicy>();
            foreach (var policy in policies)
            {
                if (_expressionService.IsPrivacyPolicyRelateToContext(policy, user, resource, environment))
                    result.Add(policy);
            }
            return result;
        }

        private IDictionary<string, string> GetFieldCollectionRules(Subject subject, Resource resource, string action, EnvironmentObject environment)
        {
            var policies = _privacyPolicyRepository.GetPolicies(resource.Name, false);
            var targetPolicies = new List<PrivacyPolicy>();
            foreach (var policy in policies)
            {
                bool isTarget = _expressionService.Evaluate(policy.Target, subject.Data, null, environment.Data);
                if (isTarget)
                    targetPolicies.Add(policy);
            }
            var fieldCollectionRules = new Dictionary<string, string>();
            foreach (var policy in targetPolicies)
            {
                foreach (var collectionField in policy.Rules)
                {
                    bool isApplied = _expressionService.Evaluate(collectionField.Condition, subject.Data, null, environment.Data);
                    if (isApplied)
                    {
                        InsertPrivacyRule(fieldCollectionRules, collectionField.FieldEffects);
                    }
                }
            }
            return fieldCollectionRules;
        }

        private void InsertPrivacyRule(IDictionary<string, string> privacyRules, ICollection<FieldEffect> bonusFields)
        {
            foreach (var field in bonusFields)
            {
                if (!privacyRules.Keys.Contains(field.Name))
                {
                    privacyRules.Add(field.Name, field.FunctionApply);
                }
                else if (field.FunctionApply.Equals("Optional") || field.FunctionApply.Equals(privacyRules[field.Name]))
                {
                    continue;
                }
                else if (privacyRules[field.Name].Equals("Optional") || privacyRules[field.Name].Equals("DefaultDomainPrivacy.Show"))
                {
                    privacyRules[field.Name] = field.FunctionApply;
                }
                else if (field.FunctionApply.Equals("DefaultDomainPrivacy.Hide"))
                {
                    privacyRules[field.Name] = field.FunctionApply;
                }
                else if (field.FunctionApply.Equals("DefaultDomainPrivacy.Show"))
                {
                    continue;
                }
                else
                {
                    privacyRules[field.Name] = _privacyDomainRepository.ComparePrivacyFunction(privacyRules[field.Name], field.FunctionApply);
                }
            }
        }

        private IDictionary<string, string> GetPrivacyRecordField(Subject user, JObject record, string collectionName, EnvironmentObject environment)
        {
            var policies = _privacyPolicyRepository.GetPolicies(collectionName, true);
            var targetPolicies = new List<PrivacyPolicy>();
            foreach (var policy in policies)
            {
                bool isTarget = _expressionService.Evaluate(policy.Target, user.Data, record, environment.Data);
                if (isTarget)
                    targetPolicies.Add(policy);
            }
            IDictionary<string, string> recordPrivacyRules = _collectionPrivacyRules.ToDictionary(entry => entry.Key, entry => entry.Value);
            //Privacy checking
            foreach (var policy in targetPolicies)
            {
                foreach (var rule in policy.Rules)
                {
                    bool isRuleApplied = _expressionService.Evaluate(rule.Condition, user.Data, record, environment.Data);
                    if (isRuleApplied)
                    {
                        CombinePrivacyFields(recordPrivacyRules, rule.FieldEffects);
                    }
                }
            }
            return recordPrivacyRules;
        }

        private JObject PrivacyProcessing(JObject record, IDictionary<string, string> privacyField, Subject subject, EnvironmentObject environment)
        {
            var privacyRecord = new JObject();

            foreach (var fieldName in privacyField.Keys)
            {
                if (fieldName == "_id") continue;
                if (privacyField[fieldName] != "Optional")
                {
                    string json = record.SelectToken(fieldName).ToString();
                    try
                    {
                        var token = JToken.Parse(json);
                        if (token is JArray)
                        {
                            var arr = JArray.Parse(record.SelectToken(fieldName).ToString());
                            privacyRecord[fieldName] = RecursivePrivacyProcess(privacyField[fieldName], arr, subject, environment);
                        }
                        else privacyRecord.AddNewField(fieldName, record, privacyField[fieldName]);
                    }
                    catch (Exception)
                    {
                        privacyRecord.AddNewField(fieldName, record, privacyField[fieldName]);
                    }
                }
                else privacyRecord[fieldName] = "";
            }
            return privacyRecord;
        }

        private JArray RecursivePrivacyProcess(string policyName, JArray nestedArrayResource, Subject subject, EnvironmentObject environment)
        {
            var policyID = policyName.Split('.')[1];
            var policy = _privacyPolicyRepository.GetById(policyID);
            var result = new JArray();
            foreach (var token in nestedArrayResource)
            {
                var record = (JObject)token;
                var fieldCollectionRules = new Dictionary<string, string>();
                foreach (var rule in policy.Rules)
                {
                    bool isRuleApplied = _expressionService.Evaluate(rule.Condition, subject.Data, record, environment.Data);
                    if (isRuleApplied)
                    {
                        foreach (var fieldEffect in rule.FieldEffects)
                        {
                            if (!fieldCollectionRules.ContainsKey(fieldEffect.Name))
                                fieldCollectionRules.Add(fieldEffect.Name, fieldEffect.FunctionApply);
                            else fieldCollectionRules[fieldEffect.Name] = fieldEffect.FunctionApply;
                        }
                    }
                }
                result.Add(PrivacyProcessing(record, fieldCollectionRules, subject, environment));
            }
            return result;
        }

        private void CombinePrivacyFields(IDictionary<string, string> privacyRules, ICollection<FieldEffect> bonusFields)
        {
            foreach (FieldEffect field in bonusFields)
            {
                if (!privacyRules.Keys.Contains(field.Name))
                {
                    privacyRules.Add(field.Name, field.FunctionApply);
                }
                else if (field.FunctionApply.Equals("Optional") || field.FunctionApply.Equals(privacyRules[field.Name]))
                {
                    continue;
                }
                else if (privacyRules[field.Name].Equals("Optional") || privacyRules[field.Name].Equals("DefaultDomainPrivacy.Show"))
                {
                    privacyRules[field.Name] = field.FunctionApply;
                }
                else if (field.FunctionApply.Equals("DefaultDomainPrivacy.Hide"))
                {
                    privacyRules[field.Name] = field.FunctionApply;
                }
                else if (field.FunctionApply.Equals("DefaultDomainPrivacy.Show"))
                {
                    continue;
                }
                else
                {
                    privacyRules[field.Name] = _privacyDomainRepository.ComparePrivacyFunction(privacyRules[field.Name], field.FunctionApply);
                }
            }
        }
    }
}
