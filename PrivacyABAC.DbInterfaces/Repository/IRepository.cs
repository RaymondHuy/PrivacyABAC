using PrivacyABAC.DbInterfaces.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.DbInterfaces.Repository
{
    public interface IRepository<T> where T : IEntityBase
    {
        IEnumerable<T> GetAll();

        T GetById(string id);

        void Add(T entity);

        void Update(T entity);

        void Delete(string id);
    }
}
