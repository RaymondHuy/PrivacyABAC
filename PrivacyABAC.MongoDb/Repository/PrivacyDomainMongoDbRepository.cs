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
        public PrivacyDomainMongoDbRepository(MongoDbContextProvider mongoDbContextProvider)
            : base(mongoDbContextProvider)
        {

        }

        public string ComparePrivacyFunction(string firstPrivacyFunction, string secondPrivacyFunction)
        {
            string domainName = firstPrivacyFunction.Split('.')[0];
            string firstPrivacyFunctionName = firstPrivacyFunction.Split('.')[1];
            string secondPrivacyFunctionName = secondPrivacyFunction.Split('.')[1];

            var privacyCollection = dbContext.GetCollection<PrivacyDomain>("PrivacyDomain");
            var privacyDomain = privacyCollection.Find(f => f.DomainName.Equals(domainName)).FirstOrDefault();
            int priority1 = privacyDomain.Functions.Where(f => f.Name.Equals(firstPrivacyFunctionName)).FirstOrDefault().Priority;
            int priority2 = privacyDomain.Functions.Where(f => f.Name.Equals(secondPrivacyFunctionName)).FirstOrDefault().Priority;

            if (priority1 > priority2)
                return firstPrivacyFunction;
            else return secondPrivacyFunction;
        }
    }
}
