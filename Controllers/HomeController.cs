using DemoWeb1.Models;
using DemoWeb1.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoWeb1.Controllers
{
    public class HomeController : Controller
    {
        private ShopDunkDBEntities db = new ShopDunkDBEntities();
        private List<string> homepageProductNames = new List<string> {
            "Iphone 17 ",
            "Iphone 17 Pro ",
            "Iphone 17 Pro Max ",
            "Iphone Air "
        };

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }


        public ActionResult Trangchu()
        {
            var viewModel = new HomepageViewModel();


            viewModel.iPhones = db.Products
                                  .Where(p => homepageProductNames.Contains(p.NamePro))
                                  .ToList();


            viewModel.iPads = db.Products
                                 .Where(p => p.CategoryId == 2)
                                 .OrderByDescending(p => p.IDProduct)
                                 .Take(4)
                                 .ToList();

            viewModel.Macs = db.Products
                               .Where(p => p.CategoryId == 3)
                               .OrderByDescending(p => p.IDProduct)
                               .Take(4)
                               .ToList();

            return View(viewModel);
        }

        public ActionResult DangKy()
        {
            return View();
        }

        public ActionResult DestopComputer()
        {
            return View();
        }

        public ActionResult GioHang()
        {
            List<CartItem> cart = (List<CartItem>)Session["Cart"];
            if (cart == null)
            {
                cart = new List<CartItem>();
            }
            ViewBag.TongTien = cart.Sum(item => item.TotalPrice);
            ViewBag.TongSoLuong = cart.Sum(item => item.Quantity);
            return View(cart);
        }

        public ActionResult DangNhap()
        {
            return View();
        }

        public ActionResult QuanLyDanhMuc()
        {
            return View();
        }

        public ActionResult ChiTietSanPham(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.SanphamLienQuan = db.Products
        .Where(p => p.CategoryId == product.CategoryId && p.IDProduct != id)
        .Take(4)
        .ToList();
            return View(product);
        }

        public ActionResult Index()
        {
     
            return View();
        }

        public ActionResult DanhSachSanPham(int categoryID)
        {
            var productList = db.Products
                .Where(p => p.CategoryId == categoryID)
                .Where(p => !homepageProductNames.Contains(p.NamePro))
                .ToList();

            return View(productList);
        }

        [HttpPost]
        public ActionResult AddToCart(int IDProduct, int quantity)
        {
            List<CartItem> cart = (List<CartItem>)Session["Cart"];
            if (cart == null)
            {
                cart = new List<CartItem>();
            }
            CartItem item = cart.FirstOrDefault(c => c.IDProduct == IDProduct);

            if (item != null)
            {
                item.Quantity += quantity;
            }
            else
            {
                Product product = db.Products.Find(IDProduct);
                if (product == null)
                {
                    return HttpNotFound();
                }
                CartItem newItem = new CartItem();
                newItem.IDProduct = IDProduct;
                newItem.NamePro = product.NamePro;
                newItem.Image = product.Image;
                newItem.Price = product.Price;
                newItem.Quantity = quantity;
                cart.Add(newItem);
            }
            Session["Cart"] = cart;
            return RedirectToAction("GioHang");
        }
        [HttpGet]
        [Authorize]
        public ActionResult ThanhToan()
        {
            List<CartItem> cart = (List<CartItem>)Session["Cart"];
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("GioHang");
            }
            ViewBag.TongTien = cart.Sum(item => item.TotalPrice);
            return View(cart);
        }
        public ActionResult RemoveFromCart(int id) 
        {
            List<CartItem> cart = (List<CartItem>)Session["Cart"];
            if (cart != null)
            {
                CartItem itemToRemove = cart.FirstOrDefault(c => c.IDProduct == id);
                if (itemToRemove != null)
                {
                    cart.Remove(itemToRemove);
                }
                Session["Cart"] = cart;
            }
            return RedirectToAction("GioHang");
        }
        [HttpGet]
        public ActionResult Search(string query)
        {
            List<Product> results;
            if (!string.IsNullOrEmpty(query))
            {
                results = db.Products
                            .Where(p => p.NamePro.Contains(query))
                            .ToList();
            }
            else
            {
                results = new List<Product>();
            }
            ViewBag.SearchQuery = query;
            return View("SearchResult", results);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult XuLyThanhToan(FormCollection form)
        {
            List<CartItem> cart = (List<CartItem>)Session["Cart"];
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("GioHang");
            }
            string customerName = form["CustomerName"];
            string customerPhone = form["CustomerPhone"];
            string customerAddress = form["CustomerAddress"];
            string currentUserId = User.Identity.GetUserId();
            OrderPro newOrder = new OrderPro();
            newOrder.DateOrder = DateTime.Now;
            newOrder.IDCus = currentUserId;
            newOrder.CustomerName = customerName;
            newOrder.CustomerPhone = customerPhone; 
            newOrder.TotalAmount = cart.Sum(item => item.TotalPrice);
            newOrder.AddressDelivery = customerAddress;
            newOrder.Status = "Đang chờ xử lý"; 
            db.OrderProes.Add(newOrder);
            db.SaveChanges();

            int newOrderId = newOrder.ID;

            foreach (var item in cart)
            {
                var productInDb = db.Products.Find(item.IDProduct);

                if (productInDb == null)
                {
                    continue;
                }
                OrderDetail detail = new OrderDetail();
                detail.IDOrder = newOrderId;
                detail.IDProduct = item.IDProduct;
                detail.Quantity = item.Quantity;
                detail.UnitPrice = item.Price;

                db.OrderDetails.Add(detail);
            }
            db.SaveChanges();
            Session["Cart"] = null;
            return RedirectToAction("DatHangThanhCong");
        }
        [Authorize]
        public ActionResult DatHangThanhCong()
        {
            return View();
        }
    }
}