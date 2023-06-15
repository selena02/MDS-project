using MDS.Data;
using MDS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;

namespace MDS.Controllers
{
    public class RecipesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RecipesController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var _perPage = 3;
            var recipes = db.Recipes;
            int totalItems;
            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
            var offset = 0;

            // Checkbox selection
            var selectedIngredients = HttpContext.Request.Query["selectedIngredients"].ToArray();
            ViewBag.AllIngredients = db.Ingredients;

            if (selectedIngredients.Length > 0)
            {
                var selectedIngredientsList = selectedIngredients.Select(int.Parse).ToList();

                var recipeIds = db.RecipeIngredients
                    .Where(ri => selectedIngredientsList.Contains(ri.IdIngredient))
                    .Select(ri => ri.IdRecipe)
                    .Distinct()
                    .ToList();

                if (recipeIds.Count > 0)
                {
                    // Apply pagination to the filtered recipes
                    totalItems = recipeIds.Count;
                    offset = (currentPage - 1) * _perPage;
                    if (offset < 0)
                    {
                        offset = 0; // Ensure the offset is not negative
                    }
                    var paginatedRecipeIds = recipeIds.Skip(offset).Take(_perPage);

                    var paginatedRecipes = db.Recipes
                        .Where(r => paginatedRecipeIds.Contains(r.IdRecipe))
                        .ToList();

                    ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);
                    return View(paginatedRecipes);
                }
                else
                {
                    // Handle the case when no recipes match the selected ingredients
                    ViewBag.lastPage = 0;
                    return View(Enumerable.Empty<Recipe>());
                }
            }
            else
            {
                // Apply pagination to all recipes
                totalItems = recipes.Count();
                offset = (currentPage - 1) * _perPage;
                if (offset < 0)
                {
                    offset = 0; // Ensure the offset is not negative
                }
                var paginatedRecipes = recipes.Skip(offset).Take(_perPage);

                ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);
                return View(paginatedRecipes);
            }
        }





        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("User"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }
        public IActionResult Show(int id)
        {
            Recipe recipe = db.Recipes.Include("Comments").Include("Comments.User")
                                          .Where(art => art.IdRecipe == id)
                                          .First();

            ViewBag.Recipe = recipe;

            var lista = db.Comments.Include(c => c.User).Where(a => a.IdRecipe == id);
            ViewBag.Comm = lista;
            ViewBag.HasComm = lista.Any();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"].ToString();
            }
            SetAccessRights();


            Recipe recipe1 = db.Recipes.Include(r => r.Comments)
                                      .ThenInclude(c => c.User)
                                      .Include(r => r.RecipeIngredients)
                                      .ThenInclude(ri => ri.Ingredient)
                                      .FirstOrDefault(r => r.IdRecipe == id);


            if (recipe1 != null)
            {
                ViewBag.Recipe1 = recipe1;
                ViewBag.RecipeIngredients = recipe1.RecipeIngredients?.ToList() ?? new List<RecipeIngredient>();

                if (TempData.ContainsKey("message"))
                {
                    ViewBag.Msg = TempData["message"].ToString();
                }

                SetAccessRights();

                return View(recipe);
            }
            else
            {
                return NotFound();
            }
        }



        [HttpPost]
        public IActionResult Show([FromForm] Comment comment)
        {
            comment.Date = DateTime.Now;
            comment.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return Redirect("/Recipes/Show/" + comment.IdRecipe);
            }
            else
            {
                Recipe rec = db.Recipes.Include(r => r.Comments)
                                       .FirstOrDefault(r => r.IdRecipe == comment.IdRecipe);

                if (rec != null)
                {
                    ViewBag.Comm = rec.Comments;
                    ViewBag.HasComm = rec.Comments.Any();
                    return View(rec);
                }
                else
                {
                    return NotFound();
                }
            }
        }




        [HttpPost]
        public IActionResult AddIngredient(int recipeId, string ingredientName, string ingredientUnit, int quantity)
        {
            Recipe recipe = db.Recipes.Include(r => r.RecipeIngredients)
                                      .FirstOrDefault(r => r.IdRecipe == recipeId);

            if (recipe != null)
            {
                if (recipe.RecipeIngredients == null)
                {
                    recipe.RecipeIngredients = new List<RecipeIngredient>();
                }

                Ingredient ingredient = new Ingredient
                {
                    NameIngredient = ingredientName,
                    UnitIngredient = ingredientUnit
                };

                RecipeIngredient recipeIngredient = new RecipeIngredient
                {
                    Ingredient = ingredient,
                    Recipe = recipe,
                    Quantity = quantity
                };

                recipe.RecipeIngredients.Add(recipeIngredient);
                db.SaveChanges();

                // redirectioneaza catre show-ul retetei respective
                return RedirectToAction("Show", new { id = recipeId });
            }
            else
            {
                return NotFound();
            }
        }


        public IActionResult New()
        {
            Recipe recipe = new Recipe();
            return View(recipe);
        }
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult New(Recipe recipe)
        {

            if (ModelState.IsValid)
            {
                db.Recipes.Add(recipe);
                db.SaveChanges();
                TempData["message"] = "Recipe loaded";
                return RedirectToAction("Index");
            }
            else
            {
                return View(recipe);
            }
        }


        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {

            Recipe recipe = db.Recipes.Where(art => art.IdRecipe == id)
                                         .First();
            ViewBag.Recipe = recipe;
            if (User.IsInRole("Admin"))
            {
                return View(recipe);
            }
            else
            {
                TempData["message"] = "Recipe cannot be edited";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Recipe requestRecipe)
        {
            Recipe recipe = db.Recipes.Find(id);

            if (ModelState.IsValid)
            {
                if (User.IsInRole("Admin"))
                {
                    recipe.NameRecipe = requestRecipe.NameRecipe;
                    recipe.DescriptionRecipe = requestRecipe.DescriptionRecipe;
                    recipe.PhotoLink = requestRecipe.PhotoLink;
                    recipe.NrPortions = requestRecipe.NrPortions;
                    recipe.NrCalories = requestRecipe.NrCalories;
                    db.SaveChanges();
                    TempData["message"] = "Recipe edited sucesfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Recipe cannot be edited";
                    return RedirectToAction("Index");

                }
            }
            else
            {
                return View(requestRecipe);
            }
        }
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            Recipe recipe = db.Recipes.Include("Comments")
                                         .Where(art => art.IdRecipe == id)
                                         .First();

            if (User.IsInRole("Admin"))
            {
                db.Recipes.Remove(recipe);
                db.SaveChanges();
                TempData["message"] = "Recipe deleted sucesfully";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Recipe cannot be deleted";
                return RedirectToAction("Index");

            }
        }


    }


}

