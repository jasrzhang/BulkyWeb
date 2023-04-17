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
            _db.Products.Update(obj);
        }
    }
}
