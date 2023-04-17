using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using BulkyWeb.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class ProductReporsitory : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductReporsitory( ApplicationDbContext db) : base(db)
        {
            this._db = db;
        }
        public void Update(Product obj)
        {
            var objFromDb = _db.Products.FirstOrDefault(u=>u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.ISBN = obj.ISBN;
                objFromDb.Price = obj.Price;
                objFromDb.Description = obj.Description;
                objFromDb.Price50 = obj.Price50;
                objFromDb.Price100 = obj.Price100;
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.Author = obj.Author;
                objFromDb.ListPrice = obj.ListPrice;
                if(objFromDb.ImageUrl != null) { 
                    objFromDb.ImageUrl = obj.ImageUrl;
                }
            }


        }
    }
}
