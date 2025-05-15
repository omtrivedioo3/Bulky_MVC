
using Bulky.DataAccess.Data;
using Bulky.DataAcess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        // create model object 
        private readonly IUnitOfWork _unitOfWork;
        // inbuild functionality to get wwwroot folder access
        private readonly IWebHostEnvironment _webhostEnvironment;
        // Dependency injection
        public ProductController(IUnitOfWork db, IWebHostEnvironment webhostEnvironment)
        {
            _unitOfWork = db;
            _webhostEnvironment = webhostEnvironment;
        }
        public IActionResult Index()
        {
            // connect model with view
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
           
            return View(objProductList);
        }
        // Upsert --> update+ insert(create)
        public IActionResult Upsert(int? id)
        {
            // get category list for dropdown , we want to get specific column from category table  to show category list in product form dropdown 
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll()
            .Select(i => new SelectListItem  // Projection in EF
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            //ViewBag.CategoryList = CategoryList;
            //ViewData["CategoryList"] = CategoryList;
            //OR belowe viewmodel
            ProductVM productVM = new() 
            {
                Product = new Product(),
                CategoryList = CategoryList
            };
            if(id== null || id == 0)
            {
                // create product
                return View(productVM);
            }
            else
            {
                // update product
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
            return View(productVM);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM obj , IFormFile? imageurl)
        {       
           
            // inbuild error define 
            if (ModelState.IsValid)
            {
                // add file into wwwroot folder
                string wwwRootPath = _webhostEnvironment.WebRootPath;
                if (imageurl != null)
                {
                    // generate unique name for image
                       string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageurl.FileName);
                    string upload = Path.Combine(wwwRootPath, @"images\product");
                    // if we want to update emage then first delete old image and then add new image
                    if (!string.IsNullOrEmpty(obj.Product.EmageUrl))
                    {
                        // delete old image
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.EmageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(upload, fileName), FileMode.Create))
                    {
                        imageurl.CopyTo(fileStream);
                    }
                    obj.Product.EmageUrl = @"\images\product\"+ fileName;
                }
                if(obj.Product.Id == 0)
                {
                    // create product
                    _unitOfWork.Product.Add(obj.Product);
                }
                else
                {
                    // update product
                    _unitOfWork.Product.Update(obj.Product);
                }
                _unitOfWork.Save();
                TempData["success"] = "Item created successfully";
                return RedirectToAction("Index", "Product");
            }
            else
            {
                 obj.CategoryList = _unitOfWork.Category.GetAll()
                .Select(i => new SelectListItem  // Projection in EF
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                return View(obj);
            }
        }
        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product? ProductFromDb = _unitOfWork.Product.Get(u => u.Id == id);
        //    if (ProductFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(ProductFromDb);
        //}

        //// both delete action have same parameter thts why we have to change name but explicitly define original name for url
        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePOST(int? id)
        //{
        //    Product? obj = _unitOfWork.Product.Get(u => u.Id == id);
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }
        //    _unitOfWork.Product.Remove(obj);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Item deleted successfully";
        //    return RedirectToAction("Index", "Product");
        //}

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

            return Json(new { data = objProductList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            // delete image from wwwroot folder
            var oldImagePath = Path.Combine(_webhostEnvironment.WebRootPath, productToBeDeleted.EmageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();
          
            return Json(new { success = true, message = "Delete successful" });
        }
        #endregion  
    }
}
