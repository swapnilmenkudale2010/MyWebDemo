using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Helpers;
using SampleWebApiAspNetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace SampleWebApiAspNetCore.Repositories
{
    public class EfFoodRepository : IFoodRepository
    {
        private readonly FoodDbContext _foodDbContext;

        public EfFoodRepository(FoodDbContext foodDbContext)
        {
            _foodDbContext = foodDbContext;
        }

        public FoodItem GetSingle(int id)
        {
            return _foodDbContext.FoodItems.FirstOrDefault(x => x.Id == id);
        }

        public void Add(FoodItem item)
        {
            _foodDbContext.FoodItems.Add(item);
        }

        public void Delete(int id)
        {
            FoodItem foodItem = GetSingle(id);
            _foodDbContext.FoodItems.Remove(foodItem);
        }

        public FoodItem Update(int id, FoodItem item)
        {
            _foodDbContext.FoodItems.Update(item);
            return item;
        }

        public IQueryable<FoodItem> GetAll(QueryParameters queryParameters)
        {
            IQueryable<FoodItem> _allItems = _foodDbContext.FoodItems.OrderBy(queryParameters.OrderBy,
              queryParameters.IsDescending());

            if (queryParameters.HasQuery())
            {
                _allItems = _allItems
#pragma warning disable CA1305 // Specify IFormatProvider
#pragma warning disable CA1307 // Specify StringComparison
#pragma warning disable CA1308 // Normalize strings to uppercase
                    .Where(x => x.Calories.ToString().Contains(queryParameters.Query.ToLowerInvariant())
#pragma warning restore CA1308 // Normalize strings to uppercase
#pragma warning restore CA1307 // Specify StringComparison
#pragma warning restore CA1305 // Specify IFormatProvider
#pragma warning disable CA1308 // Normalize strings to uppercase
#pragma warning disable CA1307 // Specify StringComparison
                    || x.Name.ToLowerInvariant().Contains(queryParameters.Query.ToLowerInvariant()));
#pragma warning restore CA1307 // Specify StringComparison
#pragma warning restore CA1308 // Normalize strings to uppercase

            }

            return _allItems
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        public int Count()
        {
            return _foodDbContext.FoodItems.Count();
        }

        public bool Save()
        {
            return (_foodDbContext.SaveChanges() >= 0);
        }

        public ICollection<FoodItem> GetRandomMeal()
        {
            List<FoodItem> toReturn = new List<FoodItem>
            {
                GetRandomItem("Starter"),
                GetRandomItem("Main"),
                GetRandomItem("Dessert")
            };

            return toReturn;
        }

        private FoodItem GetRandomItem(string type)
        {
            return _foodDbContext.FoodItems
                .Where(x => x.Type == type)
                .OrderBy(o => Guid.NewGuid())
                .FirstOrDefault();
        }
    }
}
