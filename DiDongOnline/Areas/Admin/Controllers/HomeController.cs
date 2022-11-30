using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DiDongOnline.Models;

namespace DiDongOnline.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        dbDiDongOnlineDataContext db = new dbDiDongOnlineDataContext();
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection f)
        {

            var sTenDN = f["UserName"];
            var sMatKhau = f["Password"];
            NHANVIEN ad = db.NHANVIENs.SingleOrDefault(n => n.TenDN == sTenDN && n.MatKhau == sMatKhau);
            if (ad != null)
            {
                Session["TenDN"] = ad;
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }


            return View();
        }

        public ActionResult DangXuat()
        {
            Session["TenDN"] = null;
            return RedirectToAction("Login", "Home");
        }
    }
}