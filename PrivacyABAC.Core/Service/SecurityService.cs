using Newtonsoft.Json.Linq;
using PrivacyABAC.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.Core.Service
{
    public class SecurityService
    {
        private readonly AccessControlService _accessControlService;
        private readonly PrivacyService _privacyService;

        public SecurityService(
            AccessControlService accessControlService,
            PrivacyService privacyService)
        {
            _accessControlService = accessControlService;
            _privacyService = privacyService;
        }

        public ResponseContext ExecuteProcess(JObject user, JObject[] resource, string action, string collectionName, JObject environment)
        {
            var subject = new Subject(user);
            var data = new Resource(resource, collectionName);
            var env = new EnvironmentObject(environment);
            var accessControlResult = _accessControlService.ExecuteProcess(subject, data, action, env);
            if (accessControlResult.Effect == AccessControlEffect.Permit)
            {
                var privacyResult = _privacyService.ExecuteProcess(subject, data, action, env);
                return privacyResult;
            }
            return null;
        }
    }
}
