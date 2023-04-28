using System.ComponentModel.DataAnnotations;

namespace MDS.Models
{
    public class RecipeIngredient
    {
        [Key]
        public int IdRecipeIngredient { get; set; }

        public int IdRecipe { get; set; }

        public int IdIngredient { get; set; }

        public virtual Ingredient? Ingredient { get; set; }

        public virtual Recipe? Recipe { get; set; }

        [Required(ErrorMessage = "Quantity required")]
        public int Quantity { get; set; }
    }
}
