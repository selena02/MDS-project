using System.ComponentModel.DataAnnotations;

namespace MDS.Models
{
    public class Recipe
    {
        [Key]
        public int IdRecipe { get; set; }

        [Required(ErrorMessage = "Name required")]
        public string NameRecipe { get; set; }

        [Required(ErrorMessage = "Photo required")]
        public string PhotoLink { get; set; }

        [Required(ErrorMessage = "Description required")]
        public string DescriptionRecipe { get; set; }

        [Required(ErrorMessage = "Number of portions required")]
        public int NrPortions { get; set; }

        [Required(ErrorMessage = "Number of calories required")]
        public int NrCalories { get; set; }

        public virtual ICollection <Comment> Comments { get; set; }

        public virtual ICollection<RecipeIngredient> RecipesIngredient { get; set; }
    }
}
