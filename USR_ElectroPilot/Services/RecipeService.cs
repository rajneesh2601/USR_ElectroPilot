using System.Collections.Generic;
using USR_ElectroPilot.Data;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public class RecipeService
    {
        private readonly RecipeRepository _recipeRepository = new RecipeRepository();

        public List<RecipeModel> GetRecipes()
        {
            DatabaseHelper.InitializeDatabase();
            return _recipeRepository.GetAll();
        }

        public int AddRecipe(RecipeModel recipe)
        {
            DatabaseHelper.InitializeDatabase();
            return _recipeRepository.Add(recipe);
        }
    }
}
