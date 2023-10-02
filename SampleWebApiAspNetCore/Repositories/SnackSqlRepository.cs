using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Helpers;
using SampleWebApiAspNetCore.Models;
using System.Linq.Dynamic.Core;

namespace SampleWebApiAspNetCore.Repositories
{
    public class SnackSqlRepository : SnackRepository
    {
        private readonly SnackDbContext _snackDbContext;

        public SnackSqlRepository(SnackDbContext foodDbContext)
        {
            _snackDbContext = foodDbContext;
        }

        public SnackEntity GetSingle(int id)
        {
            return _snackDbContext.SnackItems.FirstOrDefault(x => x.Id == id);
        }

        public void Add(SnackEntity item)
        {
            _snackDbContext.SnackItems.Add(item);
        }

        public void Delete(int id)
        {
            SnackEntity snackItem = GetSingle(id);
            _snackDbContext.SnackItems.Remove(snackItem);
        }

        public SnackEntity Update(int id, SnackEntity item)
        {
            _snackDbContext.SnackItems.Update(item);
            return item;
        }

        public IQueryable<SnackEntity> GetAll(QueryParameters queryParameters)
        {
            IQueryable<SnackEntity> _allItems = _snackDbContext.SnackItems.OrderBy(queryParameters.OrderBy,
              queryParameters.IsDescending());

            if (queryParameters.HasQuery())
            {
                _allItems = _allItems
                    .Where(x => x.Calories.ToString().Contains(queryParameters.Query.ToLowerInvariant())
                    || x.Name.ToLowerInvariant().Contains(queryParameters.Query.ToLowerInvariant()));
            }

            return _allItems
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        public int Count()
        {
            return _snackDbContext.SnackItems.Count();
        }

        public bool Save()
        {
            return (_snackDbContext.SaveChanges() >= 0);
        }

        public ICollection<SnackEntity> GetRandomMeal()
        {
            List<SnackEntity> toReturn = new List<SnackEntity>();

            toReturn.Add(GetRandomItem("Starter"));
            toReturn.Add(GetRandomItem("Main"));
            toReturn.Add(GetRandomItem("Dessert"));

            return toReturn;
        }

        private SnackEntity GetRandomItem(string type)
        {
            return _snackDbContext.SnackItems
                .Where(x => x.Type == type)
                .OrderBy(o => Guid.NewGuid())
                .FirstOrDefault();
        }
    }
}
