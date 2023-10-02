using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Repositories;

namespace SampleWebApiAspNetCore.Services
{
    public class SeedDataService : ISeedDataService
    {
        public void Initialize(SnackDbContext context)
        {
            context.SnackItems.Add(new SnackEntity() { Calories = 400, Type = "Fries", Name = "French Fries", Created = DateTime.Now });
            context.SnackItems.Add(new SnackEntity() { Calories = 149, Type = "Chips", Name = "Potato Chips", Created = DateTime.Now });
            context.SnackItems.Add(new SnackEntity() { Calories = 155, Type = "Corn", Name = "Popcorn", Created = DateTime.Now });
            context.SnackItems.Add(new SnackEntity() { Calories = 134, Type = "Wheat", Name = "Bread", Created = DateTime.Now });

            context.SaveChanges();
        }
    }
}
