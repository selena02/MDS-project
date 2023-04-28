using System.ComponentModel.DataAnnotations;

namespace MDS.Models
{
    public class Ingredient
    {

        [Key]
        public int IdIngredient { get; set; }

        [Required(ErrorMessage = "Name required")]
        public string NameIngredient { get; set; }

        [Required(ErrorMessage = "Unit required")]
        public string UnitIngredient { get; set; }

        public virtual ICollection <RecipeIngredient> RecipeIngredients { get; set; }
    }
}
