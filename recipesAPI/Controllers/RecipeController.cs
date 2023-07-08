using Microsoft.AspNetCore.Mvc;
using RecipesAPI;

namespace recipesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeController : ControllerBase
    {
        private readonly DataContext _context;

        public RecipeController(DataContext context)
        {
            this._context = context;
        }
        
        [HttpPut]
        public async Task<ActionResult<List<Recipe>>> UpdateAndAdd(List<Recipe> requests)
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

            return Ok(await this._context.Recipes.ToListAsync());
        }
        
        [HttpGet]
        public async Task<ActionResult<List<Recipe>>> GetAll()
        {
            return Ok(await this._context.Recipes.ToListAsync());
        }

    }
}