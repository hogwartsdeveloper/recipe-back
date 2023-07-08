namespace recipesAPI.Services.RecipeService
{
    public interface IRecipeService
    {
        Task<List<Recipe>> UpdateAndAdd(List<Recipe> requests);
        Task<List<Recipe>> GetAll();
    }
}

