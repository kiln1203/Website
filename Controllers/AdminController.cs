using DemoWeb1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace DemoWeb1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ShopDunkDBEntities db = new ShopDunkDBEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult QuanLyDonHang()
        {
            var orders = db.OrderProes.Include(o => o.Customer).OrderByDescending(o => o.DateOrder).ToList();
            return View(orders);
        }
        public ActionResult ThongKeDoanhThu()
        {
            ViewBag.SelectedMonth = DateTime.Now.Month;
            ViewBag.SelectedYear = DateTime.Now.Year;
            ViewBag.Years = Enumerable.Range(DateTime.Now.Year - 5, 6).Select(y => new SelectListItem { Value = y.ToString(), Text = y.ToString() });
            ViewBag.Months = Enumerable.Range(1, 12).Select(m => new SelectListItem { Value = m.ToString(), Text = $"Tháng {m}" });

            ViewBag.TotalRevenue = 0M;
            ViewBag.TotalOrders = 0;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThongKeDoanhThu(int month, int year)
        {
            var orders = db.OrderProes
                .Where(o => o.DateOrder.HasValue &&
                            o.DateOrder.Value.Year == year &&
                            o.DateOrder.Value.Month == month &&
                            o.Status != "Đã hủy") 
                .ToList();

            decimal totalRevenue = orders.Sum(o => o.TotalAmount ?? 0);
            int totalOrders = orders.Count;

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.SelectedMonth = month;
            ViewBag.SelectedYear = year;
            ViewBag.Years = Enumerable.Range(DateTime.Now.Year - 5, 6).Select(y => new SelectListItem { Value = y.ToString(), Text = y.ToString() });
            ViewBag.Months = Enumerable.Range(1, 12).Select(m => new SelectListItem { Value = m.ToString(), Text = $"Tháng {m}" });

            return View();
        }

        // GET: /Admin/ThongKeTonKho
        public ActionResult ThongKeTonKho()
        {
            var products = db.Products.OrderBy(p => p.SoLuongTon).ToList();
            return View(products);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
