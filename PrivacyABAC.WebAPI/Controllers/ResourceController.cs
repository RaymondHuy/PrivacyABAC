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
    public class ResourceController : Controller
    {
        private readonly IResourceRepository _resourceRepository;

        public ResourceController(IResourceRepository resourceRepository)
        {
            _resourceRepository = resourceRepository;
        }

        [HttpGet]
        [Route("api/structure")]
        public string GetCollectionStructure(string collectionName)
        {
            
            return _resourceRepository.GetJsonStructureOfCollection(collectionName);
        }

        [HttpGet]
        [Route("api/SubStructure")]
        public string GetFieldStructure(string fieldName)
        {
            return _resourceRepository.GetJsonStructureOfEmbeddedArray(fieldName);
        }

        [HttpGet]
        [Route("api/collections")]
        public IEnumerable<string> GetAllCollections()
        {
            return _resourceRepository.GetAllCollectionNames();
        }

        [HttpGet]
        [Route("api/subject/fields")]
        public string GetSubjectFields()
        {
            return _resourceRepository.GetJsonStructureOfCollection("User");
        }
    }
}
