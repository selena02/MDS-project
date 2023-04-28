using MDS.Data;
using MDS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

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
        [AllowAnonymous]
        public IActionResult Index()
        {
            var recipes = db.Recipes;
            int _perPage = 3;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
   
            int totalItems = recipes.Count();
            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

          
            var offset = 0;

  
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

       
            var paginatedRecipes = recipes.Skip(offset).Take(_perPage);


         

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);

          
            ViewBag.Recipes = paginatedRecipes;
          
          
            ViewBag.Recipes =recipes;
            return View();
        }

        public IActionResult Show(int id)
        {
            Recipe recipe = db.Recipes.Include("Comments").Include("Comments.User")
                                         .Where(art => art.IdRecipe == id)
                                         .First();
            ViewBag.Recipe = recipe;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"].ToString();
            } 
            return View(recipe);
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
                Recipe rec = db.Recipes.Include("Comments").Include("User").Include("Comments.User")
                                         .Where(art => art.IdRecipe == comment.IdRecipe)
                                         .First();
          
                return View(rec);
            }
        }
        public IActionResult New()
        {
            Recipe recipe = new Recipe();
            return View(recipe);
        }
        [HttpPost]
        public IActionResult New(Recipe recipe)
        {
           
            if (ModelState.IsValid)
            {
                db.Recipes.Add(recipe);
                db.SaveChanges();
                TempData["message"] = "Post loaded";
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
                    recipe.DescriptionRecipe= requestRecipe.DescriptionRecipe;
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
        // [HttpPost]
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

