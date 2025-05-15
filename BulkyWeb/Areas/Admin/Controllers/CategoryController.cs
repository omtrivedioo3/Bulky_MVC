using Bulky.DataAccess.Data;
using Bulky.DataAcess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        // create model object 
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork db)
        {
            _unitOfWork = db;
        }
        public IActionResult Index()
        {
            // connect model with view
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            // custom error define
            if (obj.Name.ToLower() == obj.DisplayOrder.ToString().ToLower())
            {
                ModelState.AddModelError("name", "The DisplayOrder cannot be exactly match the Name.");
            }
            // inbuild error define 
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Item created successfully";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }


        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            // find - it work with primary key only
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            // FirstOrDefault- it work with all column
            //Category? categoryFromDb = _db.Categories.FirstOrDefault(u=> u.Id==id);
            // adv filter query
            //Category? categoryFromDb = _db.Categories.Where(u=>u.Id==id).FirstOrDefault(u=> u.Id==id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {

            // inbuild error define 
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Item edited successfully";

                return RedirectToAction("Index", "Category");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        // both delete action have same parameter thts why we have to change name but explicitly define original name for url
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? obj = _unitOfWork.Category.Get(u => u.Id == id);
            if (id == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Item deleted successfully";
            return RedirectToAction("Index", "Category");
        }
    }
}
