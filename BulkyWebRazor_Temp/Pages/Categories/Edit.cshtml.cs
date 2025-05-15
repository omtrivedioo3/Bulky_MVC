using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public Category? Category { get; set; }
        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet(int? id)
        {
            if (id != null || id != 0)
            {
                Category = _db.Categoies.Find(id);
                //    return NotFound();
            }
            // find - it work with primary key only
           
        }
        public IActionResult OnPost() {
            if (ModelState.IsValid)
            {
                _db.Categoies.Update(Category);
                _db.SaveChanges();
                TempData["success"] = "Item edited successfully";

                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
