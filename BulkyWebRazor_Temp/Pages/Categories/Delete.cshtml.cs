using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public Category? Category { get; set; }
        public DeleteModel(ApplicationDbContext db)
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
        public IActionResult OnPost()
        {

            Category? obj = _db.Categoies.Find(Category.Id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Categoies.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = "Item deleted successfully";
            return RedirectToPage("Index");
            //return Page();
        }
    }
}
