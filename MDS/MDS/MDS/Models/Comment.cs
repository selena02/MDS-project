using System.ComponentModel.DataAnnotations;

namespace MDS.Models
{
    public class Comment
    {
        [Key]
        public int IdComment { get; set; }

        [Required(ErrorMessage = "Content required")]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public int IdRecipe { get; set; } 

        public virtual Recipe? Recipe { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

    }
}
