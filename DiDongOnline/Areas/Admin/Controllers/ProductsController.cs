using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DiDongOnline.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace DiDongOnline.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        dbDiDongOnlineDataContext db = new dbDiDongOnlineDataContext();
        // GET: Admin/Products
        public ActionResult Index(int? page, string sTuKhoa)
        {
            int iPageNum = (page ?? 1);
            int iPageSize = 7;
            if (!String.IsNullOrEmpty(sTuKhoa))
            {
                var lstSp = db.SANPHAMs.Where(n => n.TenSanPham.ToUpper().Contains(sTuKhoa.ToUpper())).OrderByDescending(s => s.NgayCapNhat).ToList();
                return View(lstSp.ToPagedList(iPageNum, iPageSize));
            }
            var listSPMoi = db.SANPHAMs.OrderByDescending(s => s.NgayCapNhat);
            return View(listSPMoi.ToPagedList(iPageNum, iPageSize));
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.MaLoaiSP = new SelectList(db.LOAISANPHAMs.ToList().OrderBy(n => n.TenLoaiSP), "MaLoaiSP", "TenLoaiSP");
            ViewBag.MaNSX = new SelectList(db.NHASANXUATs.ToList().OrderBy(n => n.TenNSX), "MaNSX", "TenNSX");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(SANPHAM sp, FormCollection f, HttpPostedFileBase fFileUpload)
        {
            ViewBag.MaLoaiSP = new SelectList(db.LOAISANPHAMs.ToList().OrderBy(n => n.TenLoaiSP), "MaLoaiSP", "TenLoaiSP");
            ViewBag.MaNSX = new SelectList(db.NHASANXUATs.ToList().OrderBy(n => n.TenNSX), "MaNSX", "TenNSX");
            if (fFileUpload == null)
            {
                ViewBag.ThongBao = "Hãy chọn ảnh bìa.";
                ViewBag.TenSanPham = f["sTenSanPham"];
                ViewBag.MoTa = f["sMoTa"];
                ViewBag.SoLuong = int.Parse(f["iSoLuong"]);
                ViewBag.GiaBan = decimal.Parse(f["mGiaBan"]);
                ViewBag.MaLoaiSP = new SelectList(db.LOAISANPHAMs.ToList().OrderBy(n => n.TenLoaiSP), "MaLoaiSP", "TenLoaiSP", int.Parse(f["MaLoaiSP"]));
                ViewBag.MaNSX = new SelectList(db.NHASANXUATs.ToList().OrderBy(n => n.TenNSX), "MaNSX", "TenNSX", int.Parse(f["MaNSX"]));
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var sFileName = Path.GetFileName(fFileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("/images"), sFileName);
                    if (!System.IO.File.Exists(path))
                    {
                        fFileUpload.SaveAs(path);
                    }
                    sp.TenSanPham = f["sTenSanPham"];
                    sp.MoTa = f["sMoTa"].Replace("<p>", "").Replace("</p>", "/n");
                    sp.AnhBia = sFileName;
                    sp.NgayCapNhat = Convert.ToDateTime(f["dNgayCapNhat"]);
                    sp.SoLuongBan = int.Parse(f["iSoLuong"]);
                    sp.GiaBan = decimal.Parse(f["mGiaBan"]);
                    sp.MaLoaiSP = int.Parse(f["MaLoaiSP"]);
                    sp.MaNSX = int.Parse(f["MaNSX"]);
                    db.SANPHAMs.InsertOnSubmit(sp);
                    db.SubmitChanges();
                    return RedirectToAction("Index");
                }
                return View();
            }
        }
        public ActionResult Details(int id)
        {
            var sp = db.SANPHAMs.SingleOrDefault(n => n.MaSanPham == id);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);

        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var sp = db.SANPHAMs.SingleOrDefault(n => n.MaSanPham == id);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id, FormCollection f)
        {
            var sp = db.SANPHAMs.SingleOrDefault(n => n.MaSanPham == id);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            var ctdh = db.CHITIETDATHANGs.Where(ct => ct.MaSanPham == id);
            if (ctdh.Count() != 0)
            {
                ViewBag.ThongBao = "Sách này đang có trong bảng Chi tiết đặt hàng <br>" + "Nếu muốn xóa thì phải xóa hết mã sách trong bảng Chi tiết đặt hàng";
                return View(sp);
            }
            db.SANPHAMs.DeleteOnSubmit(sp);
            db.SubmitChanges();
            return RedirectToAction("Index");

        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var sp = db.SANPHAMs.SingleOrDefault(n => n.MaSanPham == id);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaLoaiSP = new SelectList(db.LOAISANPHAMs.ToList().OrderBy(n => n.TenLoaiSP), "MaLoaiSP", "TenLoaiSP", sp.MaLoaiSP);
            ViewBag.MaNSX = new SelectList(db.NHASANXUATs.ToList().OrderBy(n => n.TenNSX), "MaNSX", "TenNSX", sp.MaNSX);
            return View(sp);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection f, HttpPostedFileBase fFileUpload)
        {
            var sp = db.SANPHAMs.SingleOrDefault(n => n.MaSanPham == int.Parse(f["iMaSanPham"]));
            ViewBag.MaLoaiSP = new SelectList(db.LOAISANPHAMs.ToList().OrderBy(n => n.TenLoaiSP), "MaLoaiSP", "TenLoaiSP", sp.MaLoaiSP);
            ViewBag.MaNSX = new SelectList(db.NHASANXUATs.ToList().OrderBy(n => n.TenNSX), "MaNSX", "TenNSX", sp.MaNSX);
            if (ModelState.IsValid)
            {
                if (fFileUpload != null)
                {
                    var sFileName = Path.GetFileName(fFileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("/images"), sFileName);
                    if (!System.IO.File.Exists(path))
                    {
                        fFileUpload.SaveAs(path);
                    }
                    sp.AnhBia = sFileName;
                }
                sp.TenSanPham = f["sTenSanPham"];
                sp.MoTa = f["sMoTa"].Replace("<p>", "").Replace("</p>", "/n");
                sp.NgayCapNhat = Convert.ToDateTime(f["dNgayCapNhat"]);
                sp.SoLuongBan = int.Parse(f["iSoLuong"]);
                sp.GiaBan = decimal.Parse(f["mGiaBan"]);
                sp.MaLoaiSP = int.Parse(f["MaLoaiSP"]);
                sp.MaNSX = int.Parse(f["MaNSX"]);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return View(sp);
        }
    }
}