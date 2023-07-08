using RecipesAPI;

namespace recipesAPI.Data
{
    public class RecipeService : IRecipeService
    {
        private readonly DataContext _context;

        public RecipeService(DataContext context)
        {
            this._context = context;
        }
        
        public async Task<List<Recipe>> UpdateAndAdd(List<Recipe> requests)
        {
            foreach (var item in requests)
            {
                var find = await this._context.Recipes.FindAsync(item.Id);
                if (find is null)
                {
                    await this._context.Recipes.AddAsync(item);
                }
                else
                {
                    find.Description = item.Description;
                    find.Name = item.Name;
                    find.imagePath = item.imagePath;
                    find.Ingredients = item.Ingredients;
                }

                await this._context.SaveChangesAsync();
            }

            return await this._context.Recipes.ToListAsync();
        }

        public async Task<List<Recipe>> GetAll()
        {
            return await this._context.Recipes.ToListAsync();
        }
    }
}