using Microsoft.AspNetCore.Mvc;

using recipesAPI.Services.RecipeService;

namespace recipesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipeController(IRecipeService recipeService)
        {
            this._recipeService = recipeService;
        }
        
        [HttpPut]
        public async Task<ActionResult<List<Recipe>>> UpdateAndAdd(List<Recipe> requests)
        {
            return Ok(await this._recipeService.UpdateAndAdd(requests));
        }
        
        [HttpGet]
        public async Task<ActionResult<List<Recipe>>> GetAll()
        {
            return Ok(await this._recipeService.GetAll());
        }

    }
}