using MDS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MDS.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
       
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<RecipeIngredient>()
               .HasKey(ab => new {
                   ab.IdRecipeIngredient,
                   ab.IdIngredient,
                   ab.IdRecipe
               });
            modelBuilder.Entity<RecipeIngredient>()
             .HasOne(ab => ab.Recipe)
             .WithMany(ab => ab.RecipeIngredients)
             .HasForeignKey(ab => ab.IdRecipe);
            modelBuilder.Entity<RecipeIngredient>()
            .HasOne(ab => ab.Ingredient)
            .WithMany(ab => ab.RecipeIngredients)
            .HasForeignKey(ab => ab.IdIngredient);
            modelBuilder.Entity<RecipeIngredient>()
               .HasOne(ab => ab.Ingredient)
               .WithMany(ab => ab.RecipeIngredients)
               .HasForeignKey(ab => ab.IdIngredient);

            modelBuilder.Entity<RecipeIngredient>()
            .HasOne(ab => ab.Recipe)
            .WithMany(ab => ab.RecipeIngredients)
            .HasForeignKey(ab => ab.IdRecipe);
        }
    }
    
}