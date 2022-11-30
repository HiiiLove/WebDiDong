using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DiDongOnline.Models;

namespace DiDongOnline.Models
{
    public class User
    {
        dbDiDongOnlineDataContext db = new dbDiDongOnlineDataContext();
        public long Insert(KHACHHANG entity)
        {
            var kh = db.KHACHHANGs.SingleOrDefault(x => x.TaiKhoan == entity.TaiKhoan);
            if (kh == null)
            {
                db.KHACHHANGs.InsertOnSubmit(entity);
                db.SubmitChanges();
                return entity.MaKH;
            }
            else

            {
                return kh.MaKH;
            }

        }
    }
}