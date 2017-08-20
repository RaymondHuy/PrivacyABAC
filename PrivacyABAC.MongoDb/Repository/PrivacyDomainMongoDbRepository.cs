using PrivacyABAC.DbInterfaces.Model;
using PrivacyABAC.DbInterfaces.Repository;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.MongoDb.Repository
{
    public class PrivacyDomainMongoDbRepository : MongoDbRepositoryBase<PrivacyDomain>, IPrivacyDomainRepository
    {
        private readonly IMongoCollection<PrivacyDomain> _mongoCollection;
        private IEnumerable<PrivacyDomain> _privacyDomains;

        public PrivacyDomainMongoDbRepository(MongoDbContextProvider mongoDbContextProvider)
            : base(mongoDbContextProvider)
        {
            _mongoCollection = dbContext.GetCollection<PrivacyDomain>("PrivacyDomain");
            _privacyDomains = _mongoCollection.Find(_ => true).ToList();
        }

        public void AddPriorityFunctions(string domainName, PriorityFunction priority)
        {
            var filter = Builders<PrivacyDomain>.Filter.Eq("domain_name", domainName);
            var update = Builders<PrivacyDomain>.Update.Push("hierarchy", priority);
            dbContext.GetCollection<PrivacyDomain>("PrivacyDomain").UpdateOne(filter, update);
        }

        public string ComparePrivacyFunction(string firstPrivacyFunction, string secondPrivacyFunction)
        {
            string domainName = firstPrivacyFunction.Split('.')[0];
            string firstPrivacyFunctionName = firstPrivacyFunction.Split('.')[1];
            string secondPrivacyFunctionName = secondPrivacyFunction.Split('.')[1];

            var privacyDomain = _privacyDomains.Where(f => f.DomainName.Equals(domainName)).FirstOrDefault();
            int priority1 = privacyDomain.Functions.Where(f => f.Name.Equals(firstPrivacyFunctionName)).FirstOrDefault().Priority;
            int priority2 = privacyDomain.Functions.Where(f => f.Name.Equals(secondPrivacyFunctionName)).FirstOrDefault().Priority;

            if (priority1 > priority2)
                return firstPrivacyFunction;
            else return secondPrivacyFunction;
        }

        public IEnumerable<string> GetAllPrivacyFunctionName()
        {
            var result = new List<string>();
            foreach (var domain in _privacyDomains)
            {
                foreach (var function in domain.Functions)
                {
                    result.Add(domain.DomainName + "." + function.Name);
                }
            }
            return result;
        }

        public IEnumerable<string> GetPrivacyFunctionNames(string fieldName)
        {
            var result = new List<string>();
            var builder = Builders<PrivacyDomain>.Filter;
            var filter = builder.In("fields", fieldName);
            result.Add("Optional");
            foreach (var domain in _privacyDomains)
            {
                if (domain.Fields.Contains(fieldName))
                    foreach (var function in domain.Functions)
                    {
                        result.Add(domain.DomainName + "." + function.Name);
                    }
            }
            result.Add("DefaultDomainPrivacy.Show");
            result.Add("DefaultDomainPrivacy.Hide");
            return result;
        }

        public void UpdateDomainField(string domainName, string fieldName)
        {
            var filter = Builders<PrivacyDomain>.Filter.Eq("domain_name", domainName);
            var update = Builders<PrivacyDomain>.Update.AddToSet("fields", fieldName);
            _mongoCollection.UpdateOne(filter, update);
        }

        public void UpdatePriorityFunctions(string domainName, ICollection<PriorityFunction> priorityFunctions)
        {
            var filter = Builders<PrivacyDomain>.Filter.Eq("domain_name", domainName);
            var update = Builders<PrivacyDomain>.Update.Set("hierarchy", priorityFunctions);
            _mongoCollection.UpdateOne(filter, update);
        }
    }
}
