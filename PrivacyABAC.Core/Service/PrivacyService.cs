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
using PrivacyABAC.Domains.Common;
using System.Diagnostics;
using System.IO;

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
            var recordPrivacyPolicies = _privacyPolicyRepository.GetPolicies(resource.Name, true).ToArray();
            var privacyRecords = new JArray();
            if (resource.Data.Length > 1000)
            {
                Parallel.ForEach(resource.Data, record =>
                {
                    //var privacyFields = GetPrivacyRecordField(subject, record, resource.Name, environment);
                    //if (_collectionPrivacyRules.Count > 0)
                    //{
                        PrivacyProcessing(record, _collectionPrivacyRules, subject, environment, recordPrivacyPolicies);
                    //}
                });
            }
            else
            {
                foreach (var record in resource.Data)
                {
                    //var privacyFields = GetPrivacyRecordField(subject, record, resource.Name, environment);
                    //var privacyFields = _collectionPrivacyRules.ToDictionary(entry => entry.Key, entry => entry.Value);
                    //if (privacyFields.Count > 0)
                    //{
                        PrivacyProcessing(record, _collectionPrivacyRules, subject, environment, recordPrivacyPolicies);
                        //privacyRecords.Add(privacyRecord);
                    //}
                }
            }
            //if (privacyRecords.Count == 0)
            //    return new ResponseContext(AccessControlEffect.Permit, null, "No privacy rules is satisfied");

            return new ResponseContext(AccessControlEffect.Permit, resource.Data);
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
            IDictionary<string, string> recordPrivacyRules = _collectionPrivacyRules.ToDictionary(entry => entry.Key, entry => entry.Value);
            //return recordPrivacyRules;
            var policies = _privacyPolicyRepository.GetPolicies(collectionName, true);
            var targetPolicies = new List<PrivacyPolicy>();
            foreach (var policy in policies)
            {
                bool isTarget = _expressionService.Evaluate(policy.Target, user.Data, record, environment.Data);
                if (isTarget)
                    targetPolicies.Add(policy);
            }
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

        private void PrivacyProcessing(JObject record, IDictionary<string, string> collectionPrivacyRules, Subject subject, EnvironmentObject environment, PrivacyPolicy[] privacyRecordRules = null)
        {
            var privacyField = collectionPrivacyRules.ToDictionary(m => m.Key, m => m.Value);
            CombinePrivacyRuleOfCollectionAndRecord(record, subject, environment, privacyRecordRules, privacyField);

            foreach (var fieldName in privacyField.Keys)
            {
                if (fieldName == "_id") continue;

                if (privacyField[fieldName] != "Optional")
                {
                    if (record.SelectToken(fieldName) == null) continue;

                    string json = record.SelectToken(fieldName).ToString();
                    try
                    {
                        var token = JToken.Parse(json);
                        if (token is JArray)
                        {
                            var arr = JArray.Parse(record.SelectToken(fieldName).ToString());
                            //privacyRecord[fieldName] = RecursivePrivacyProcess(privacyField[fieldName], arr, subject, environment);
                        }
                        //else privacyRecord.AddNewField(fieldName, record, privacyField[fieldName]);
                        else ApplyPrivacyFunction(record, privacyField[fieldName], fieldName);
                    }
                    catch (Exception)
                    {
                        ApplyPrivacyFunction(record, privacyField[fieldName], fieldName);
                        //privacyRecord.AddNewField(fieldName, record, privacyField[fieldName]);
                    }
                }
            }
        }

        private void CombinePrivacyRuleOfCollectionAndRecord(JObject record, Subject subject, EnvironmentObject environment, PrivacyPolicy[] privacyRecordRules, Dictionary<string, string> privacyField)
        {
            if (privacyRecordRules != null)
            {
                int count = privacyRecordRules.Count();
                for (int i = 0; i < count; i++)
                {
                    bool isTargetRelative = _expressionService.Evaluate(privacyRecordRules[i].Target, subject.Data, record, environment.Data);
                    if (isTargetRelative)
                    {
                        for (int j = 0; j < privacyRecordRules[i].Rules.Count(); j++)
                        {
                            var condition = privacyRecordRules[i].Rules.ElementAt(j).Condition;
                            bool isRuleRelative = _expressionService.Evaluate(condition, subject.Data, record, environment.Data);
                            if (isRuleRelative)
                            {
                                InsertPrivacyRule(privacyField, privacyRecordRules[i].Rules.ElementAt(j).FieldEffects);
                            }
                        }
                    }
                }
            }
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
                //result.Add(PrivacyProcessing(record, fieldCollectionRules, subject, environment));
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

        private void ApplyPrivacyFunction(JObject record, string privacyFunction, string fieldPath)
        {
            string[] tokens = fieldPath.Split('.');
            JToken temp = record;
            for (int idx = 0; idx < tokens.Length - 1; idx++)
            {
                temp = temp[tokens[idx]];
            }

            string className = privacyFunction.Split('.')[0];
            string functionName = privacyFunction.Split('.')[1];
            var privacyDomainFactory = PrivacyDomainPluginFactory.GetInstance();
            string param = record.SelectToken(fieldPath).ToString();
            string result = privacyDomainFactory.ExecuteFunction(className, functionName, new string[] { param });

            temp[tokens.Last()] = result;
        }
    }
}
