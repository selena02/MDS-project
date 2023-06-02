using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MDS.Models
{
    public class RecipeIngredient
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdRecipeIngredient { get; set; }

        public int IdRecipe { get; set; }

        public int IdIngredient { get; set; }

        public virtual Ingredient? Ingredient { get; set; }

        public virtual Recipe? Recipe { get; set; }

        [Required(ErrorMessage = "Quantity required")]
        public int Quantity { get; set; }
    }
}
