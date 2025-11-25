using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemoWeb1.Models;

namespace DemoWeb1.Controllers
{
    
    public class ProductController : Controller
    {
        ShopDunkDBEntities db = new Models.ShopDunkDBEntities();
        // GET: Product
        public ActionResult Index()
        {
            return View(db.Products.ToList());
        }
        public ActionResult CategoriesList()
        {
            return View(db.Categories.ToList());
        }
     public ActionResult Create()
        {
            Product pro = new Product();
            return View(pro);
        }
  
    }
}