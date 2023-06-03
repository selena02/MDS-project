using MDS.Data;
using MDS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MDS.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CommentsController(
       ApplicationDbContext context,
       UserManager<ApplicationUser> userManager,
       RoleManager<IdentityRole> roleManager
       )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);
            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Comments.Remove(comm);
                db.SaveChanges();
                TempData["message"] = "Comment deleted sucesfully";
                return Redirect("/Recipes/Show/" + comm.IdRecipe);
            }
            else
            {
                TempData["message"] = "Comment cannot be deleted";
                return RedirectToAction("Index", "Recipes");

            }
        }
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {
            Comment comm = db.Comments.Where(com => com.IdComment == id)
                                      .First();
            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(comm);
            }
            else
            {
                TempData["message"] = "Comment cannot be edited";
                return RedirectToAction("Index", "Articles");

            }
        }
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Comment requestComment)
        {
            requestComment.IdComment = id;
            Comment comment = db.Comments.Where(com => com.IdComment == id)
                                         .First();
            if (ModelState.IsValid)
            {

                if (comment.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    comment.Content = requestComment.Content;
                    db.SaveChanges();
                    return Redirect("/Recipes/Show/" + comment.IdRecipe);
                }
                else
                {
                    TempData["message"] = "Comment cannot be edited";
                    return RedirectToAction("Index", "Recipes");
                }
            }
            else
                return View(requestComment);
        }
    }
}
