using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DiDongOnline.Models;
using PagedList;
using PagedList.Mvc;

namespace DiDongOnline.Controllers
{
    public class DiDongOnlineController : Controller
    {
        dbDiDongOnlineDataContext db = new dbDiDongOnlineDataContext();
        // GET: DiDongOnline
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult HeaderPartial()
        {
            return PartialView();
        }
        public ActionResult FooterPartial()
        {
            return PartialView();
        }
        public ActionResult ProductsTypePartial()
        {
            var listLoaiSanPham = from lsp in db.LOAISANPHAMs select lsp;
            return PartialView(listLoaiSanPham);
        }
        public ActionResult ProductsType(int id, int? page)
        {
            ViewBag.MaLoaiSP = id;
            int iSize = 6;
            int iPageNum = (page ?? 1);
            var sp = from s in db.SANPHAMs where s.MaLoaiSP == id select s;
            return View(sp.ToPagedList(iPageNum, iSize));
        }
        public ActionResult LoginLogout()
        {
            return PartialView("LoginLogoutPartial");
        }
        public ActionResult SaleProductsPartial()
        {
            return PartialView();
        }
        private List<SANPHAM> SellingProducts(int count)
        {
            return db.SANPHAMs.OrderByDescending(a => a.SoLuongBan).Take(count).ToList();
        }
        public ActionResult SellingProducts()
        {
            var listSellingProducts = SellingProducts(4);
            return PartialView(listSellingProducts);
        }
        private List<SANPHAM> NewProducts(int count)
        {
            return db.SANPHAMs.OrderByDescending(a => a.NgayCapNhat).Take(count).ToList();
        }
        public ActionResult NewProducts()
        {
            var listNewProducts = NewProducts(4);
            return PartialView(listNewProducts);
        }
        public ActionResult AllProducts( int? page)
        {
            int iSize = 8;
            int iPageNum = (page ?? 1);;
            var lstAllProducts = NewProducts(int.MaxValue);
            return View(lstAllProducts.ToPagedList(iPageNum, iSize));
        }
        [HttpGet]
        public ActionResult Search(int? page, string sTuKhoa)
        {
            if (sTuKhoa == "")
            {
                return View("Index");
            }
            ViewBag.TuKhoa = sTuKhoa;
            int iSize = 8;
            int iPageNum = (page ?? 1);
            var lstSp = db.SANPHAMs.Where(n => n.TenSanPham.ToUpper().Contains(sTuKhoa.ToUpper()));
            return View(lstSp.OrderBy(n => n.TenSanPham).ToPagedList(iPageNum, iSize));
        }
        [HttpPost]
        public ActionResult LayTuKhoa(string sTuKhoa)
        {
            return RedirectToAction("Search", new { @sTuKhoa = sTuKhoa });
        }
        public ActionResult ProductsDetail(int id)
        {
            var sp = from s in db.SANPHAMs where s.MaSanPham == id select s;
            return View(sp.Single());
        }
    }
}