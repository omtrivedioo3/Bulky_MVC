using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Data;
using Bulky.DataAcess.Repository.IRepository;
using Bulky.Models;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAcess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        // : base(db) pass whatever get in db to base class
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product Product)
        {
            // update all directly
            //_db.Products.Update(Product);

            // update individual properties 
            var objFromDb = _db.Products.FirstOrDefault(u => u.Id == Product.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = Product.Title;
                objFromDb.Author = Product.Author;
                objFromDb.ISBN = Product.ISBN;
                objFromDb.ListPrice = Product.ListPrice;
                objFromDb.Price = Product.Price;
                objFromDb.Price50 = Product.Price50;
                objFromDb.Price100 = Product.Price100;
                objFromDb.CategoryId = Product.CategoryId;
                if (Product.EmageUrl != null)
                {
                    objFromDb.EmageUrl = Product.EmageUrl;
                }
            }

            }
    }
   
}
