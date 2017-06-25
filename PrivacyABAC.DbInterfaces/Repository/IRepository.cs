using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.DbInterfaces.Repository
{
    public interface IRepository<T> where T : class
    {
        T[] GetAll();

        void Create(T entity);

        void Update(T entity, string id);

        void Delete(string id);
    }
}
