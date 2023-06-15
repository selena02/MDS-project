using MDS.Data;
using MDS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MDS.Controllers
{

    public class RecipeIngredientsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RecipeIngredientsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public IActionResult Show(int id)
        {
            RecipeIngredient recipeIngredient = db.RecipeIngredients.Where(art => art.IdRecipeIngredient == id)
                                         .First();
            ViewBag.RecipeIngredient = recipeIngredient;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"].ToString();
            } 
            return View(recipeIngredient);
        }
        [HttpPost]
        public IActionResult New()
        {
            RecipeIngredient recipeIngredient = new RecipeIngredient();
            return View(recipeIngredient);
        }
        [HttpPost]
        public IActionResult New(RecipeIngredient recipeIngredient)
        {

            if (ModelState.IsValid)
            {
                db.RecipeIngredients.Add(recipeIngredient);
                db.SaveChanges();
                //return RedirectToAction("Show", "Recipes", null);
                //return RedirectToAction("/Recipes/Show/@ViewBag.IdReteta");
                 return RedirectToAction("Show", "Recipes", new { id = ViewBag.IdReteta });
               // return Redirect("/Recipes/Show/" + ViewBag.IdReteta);

            }
            else
            {
                return View(recipeIngredient);
            }
        }


        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {

            RecipeIngredient recipeIngredient = db.RecipeIngredients.Where(art => art.IdRecipeIngredient == id)
                                         .First();
            ViewBag.RecipeIngredient = recipeIngredient;
            if (User.IsInRole("Admin"))
            {
                return View(recipeIngredient);
            }
            else
            {
                TempData["message"] = "Ingredient cannot be edited";
                return RedirectToAction("/Recipes/Show/@ViewBag.IdReteta");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, RecipeIngredient requestRecipeIngredient)
        {
            RecipeIngredient recipeIngredient = db.RecipeIngredients.Find(id);

            if (ModelState.IsValid)
            {
                if (User.IsInRole("Admin"))
                {
                    recipeIngredient.IdRecipe = ViewBag.IdReteta;
                    recipeIngredient.IdIngredient = ViewBag.IdIngredient;
                    recipeIngredient.Quantity = requestRecipeIngredient.Quantity; 
                    db.SaveChanges();
                    TempData["message"] = "Ingredient edited sucesfully";
                    return RedirectToAction("/Recipes/Show/@ViewBag.IdReteta");
                }
                else
                {
                    TempData["message"] = "Ingredient cannot be edited";
                    return RedirectToAction("/Recipes/Show/@ViewBag.IdReteta");

                }
            }
            else
            {
                return View(requestRecipeIngredient);
            }
        }
        // [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            RecipeIngredient recipeIngredient = db.RecipeIngredients.Where(art => art.IdRecipeIngredient == id)
                                         .First();

            if (User.IsInRole("Admin"))
            {
                db.RecipeIngredients.Remove(recipeIngredient);
                db.SaveChanges();
                TempData["message"] = "Ingredient deleted sucesfully";
                return RedirectToAction("/Recipes/Show/@ViewBag.IdReteta");
            }
            else
            {
                TempData["message"] = "Ingredient cannot be deleted";
                return RedirectToAction("/Recipes/Show/@ViewBag.IdReteta");

            }
        }


    }
}
