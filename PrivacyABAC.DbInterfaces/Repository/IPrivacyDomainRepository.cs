using PrivacyABAC.DbInterfaces.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.DbInterfaces.Repository
{
    public interface IPrivacyDomainRepository : IRepository<PrivacyDomain>
    {
        string ComparePrivacyFunction(string firstPrivacyFunction, string secondPrivacyFunction);
        void AddPriorityFunctions(string domainName, PriorityFunction priority);
        void UpdatePriorityFunctions(string domainName, ICollection<PriorityFunction> priorityFunctions);
        void UpdateDomainField(string domainName, string fieldName);
        IEnumerable<string> GetPrivacyFunctionNames(string name);
        IEnumerable<string> GetAllPrivacyFunctionName();
    }
}
