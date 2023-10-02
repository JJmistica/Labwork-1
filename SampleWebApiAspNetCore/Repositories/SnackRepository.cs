using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Models;

namespace SampleWebApiAspNetCore.Repositories
{
    public interface SnackRepository
    {
        SnackEntity GetSingle(int id);
        void Add(SnackEntity item);
        void Delete(int id);
        SnackEntity Update(int id, SnackEntity item);
        IQueryable<SnackEntity> GetAll(QueryParameters queryParameters);
        ICollection<SnackEntity> GetRandomMeal();
        int Count();
        bool Save();
    }
}
