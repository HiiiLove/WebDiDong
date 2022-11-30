using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DiDongOnline.Models;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.IO;
using Common;
using Facebook;
using System.Configuration;

namespace DiDongOnline.Controllers
{
    public class UserController : Controller
    {
        dbDiDongOnlineDataContext db = new dbDiDongOnlineDataContext();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var sTenDN = collection["TenDN"];
            var sMatKhau = collection["MatKhau"];
            if (String.IsNullOrEmpty(sTenDN))
            {
                ViewData["Err1"] = "Bạn chưa nhập tên đăng nhập ";
            }
            else if (String.IsNullOrEmpty(sMatKhau))
            {
                ViewData["Err2"] = "Phäi nhâp mật khẩu";
            }
            else
            {
                KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(n => n.TaiKhoan == sTenDN && n.MatKhau == sMatKhau);
                if (kh != null)
                {
                    Session["TaiKhoan"] = kh;
                    return RedirectToAction("Index", "DiDongOnline");
                }
                else
                {
                    ViewBag.ThongBao = "Sai tk hoặc mk";
                }
            }
            return View();
        }
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("Facebookcallback");
                return uriBuilder.Uri;
            }
        }
        public ActionResult LoginFacebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email",
            });
            return Redirect(loginUrl.AbsoluteUri);
        }

        public ActionResult Facebookcallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code,
            });

            var accessToken = result.access_token;
            if (!string.IsNullOrEmpty(accessToken))
            {
                fb.AccessToken = accessToken;
                dynamic me = fb.Get("me?fields=first_name,middle_name,last_name,id,email");
                string email = me.email;
                string userName = me.eamil;
                string firstName = me.first_name;
                string middleName = me.middle_name;
                string lastName = me.last_name;

                var kh = new KHACHHANG();
                kh.Email = email;
                kh.TaiKhoan = email;
                kh.HoTen = firstName + " " + middleName + " " + lastName;
                kh.NgaySinh = DateTime.Now;
                var resultInsert = new User().Insert(kh);
                if (resultInsert > 0)
                {
                    Session["TaiKhoan"] = kh;
                    return RedirectToAction("Index", "DiDongOnline");

                }

            }
            return RedirectToAction("Index", "DiDongOnline");
        }
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(FormCollection collection, KHACHHANG kh)
        {

            var sHoTen = collection["HoTen"];
            var sTenDN = collection["TenDN"];
            var sMatKhau = collection["MatKhau"];
            var sMatKhauNhapLai = collection["MatKhauNL"];
            var sDiaChi = collection["DiaChi"];
            var sEmail = collection["Email"];
            var sDienThoai = collection["DienThoai"];
            var dNgaySinh = String.Format("{0:MM/dd/yyyy}", collection["NgaySinh"]);
            if (String.IsNullOrEmpty(sHoTen))
            {
                ViewData["err1"] = "Họ tên không được rỗng";
            }
            else if (String.IsNullOrEmpty(sTenDN))
            {
                ViewData["err1"] = "Tên đăng nhập không được rỗng";
            }
            else if (String.IsNullOrEmpty(sMatKhau))
            {
                ViewData["err1"] = "Phải nhập mật khẩu";
            }
            else if (String.IsNullOrEmpty(sMatKhauNhapLai))
            {
                ViewData["err1"] = "Phải nhập lại mật khẩu";
            }
            else if (String.IsNullOrEmpty(sMatKhauNhapLai))
            {
                ViewData["err1"] = "Mật khẩu không khớp";
            }
            else if (String.IsNullOrEmpty(sEmail))
            {
                ViewData["err1"] = "Email không được rỗng";
            }
            else if (String.IsNullOrEmpty(sDienThoai))
            {
                ViewData["err1"] = "Số diện thoại không được rỗng";
            }
            else if (db.KHACHHANGs.SingleOrDefault(n => n.TaiKhoan == sTenDN) != null)
            {
                ViewData["err1"] = "Tên đăng nhập đã tồn tại ";
            }
            else if (db.KHACHHANGs.SingleOrDefault(n => n.Email == sEmail) != null)
            {
                ViewData["err1"] = "Tên đăng nhập đã tồn tại ";
            }
            else
            {
                kh.HoTen = sHoTen;
                kh.TaiKhoan = sTenDN;
                kh.MatKhau = sMatKhau;
                kh.Email = sEmail;
                kh.DiaChi = sDiaChi;
                kh.DienThoai = sDienThoai;
                kh.NgaySinh = DateTime.Parse(dNgaySinh);
                
                db.KHACHHANGs.InsertOnSubmit(kh);
                db.SubmitChanges();

                return RedirectToAction("DangNhap");
            }   
            return this.DangKy();
        }
        public string RandomMK()
        {
            int Numrd;
            string Numrd_str;
            Random rd = new Random();
            Numrd_str = rd.Next(10000, 1000000).ToString();


            return Numrd_str;
        }
        public ActionResult DangXuat()
        {
            Session["TaiKhoan"] = null;
            return RedirectToAction("Index", "DiDongOnline");
        }
        //public ActionResult QuenMatKhau( string email)
        //{
        //    string email1 = email;
        //    var s = db.KHACHHANGs.FirstOrDefault(n => n.Email == email1);
        //        if(s != null)
        //    {

        //        string mk = RandomMK();
        //            string content = System.IO.File.ReadAllText(Server.MapPath("~/assets/client/template/newpassword.html"));
        //            content = content.Replace("{{pass}}", mk);
        //            content = content.Replace("{{pass}}",email1);
        //            new MailHelper().SendMail(email1, "Lấy lại maatk khẩu");

        //        s.MatKhau = mk;
        //        db.SubmitChanges();
        //        //mail.Send(message);
        //        ViewBag.ThongBao = "Đã gửi maill thành công.Vui lòng kiểm tra mail của bạn";
        //        return View("DangNhap");
        //    }
        //    else
        //    {
        //        ViewBag.ThongBao = "Địa chỉ Email của bạn không chính xác";
        //        return View("DangNhap");
        //    }
        //}
        [HttpPost]
        public ActionResult QuenMatKhau(string email)
        {
            string email1 = email;
            var s = db.KHACHHANGs.FirstOrDefault(n => n.Email == email1);
            if (s != null)
            {

                var mail = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("huyph11211@gmail.com", "itunes5s"),
                    EnableSsl = true
                };
                var message = new MailMessage();
                message.From = new MailAddress("huyph11211@gmail.com");
                message.ReplyToList.Add("huyph11211@gmail.com");
                message.To.Add(new MailAddress(s.Email));
                message.Subject = "Thông báo về việc thay đổi maatk khẩu của Home SHOP";
                string mk = RandomMK();
                message.Body = "Mật khẩu mới của bạn là:" + mk;
                s.MatKhau = mk;
                db.SubmitChanges();
                mail.Send(message);
                ViewBag.ThongBao = "Đã gửi maill thành công.Vui lòng kiểm tra mail của bạn";
                return View("DangNhap");
            }
            else
            {
                ViewBag.ThongBao = "Địa chỉ Email của bạn không chính xác";
                return View("DangNhap");
            }
        }
        [HttpGet]
        public ActionResult ChangeInformation(int id)
        {
            var kh = db.KHACHHANGs.SingleOrDefault(k => k.MaKH == id);
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(kh);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ChangeInformation(FormCollection f)
        {
            var kh = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == int.Parse(f["iMaKH"]));
            if (ModelState.IsValid)
            {
                kh.TaiKhoan = f["sTaikhoan"];
                kh.HoTen = f["sHoTen"];
                kh.MatKhau = f["sMatkhau"];
                kh.Email = f["sEmail"];
                kh.DiaChi = f["sDiaChi"];
                kh.DienThoai = f["sDienThoai"];
                kh.NgaySinh = Convert.ToDateTime(f["dNgaySinh"]);
                db.SubmitChanges();
                return RedirectToAction("Index","DiDongOnline");
            }
            return View(kh);
        }
    }
}