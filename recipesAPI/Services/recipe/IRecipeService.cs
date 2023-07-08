using RecipesAPI;

namespace recipesAPI.Data
{
    public interface IRecipeService
    {
        Task<List<Recipe>> UpdateAndAdd(List<Recipe> requests);
        Task<List<Recipe>> GetAll();
    }
}

