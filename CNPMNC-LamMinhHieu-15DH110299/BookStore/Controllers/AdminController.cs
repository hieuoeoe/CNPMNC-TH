using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStore.Models;
using System.IO;
using PagedList;
using PagedList.Mvc;
namespace BookStore.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Sach(int ?page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            // return View(db.SACHes.ToList());
            return View(db.SACHes.ToList().OrderBy(n => n.Masach).ToPagedList(pageNumber, pageSize));
        }

        dbQLBansachDataContext db = new dbQLBansachDataContext();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection colection)
        {
            var tendn = colection["username"];
            var mk = colection["password"];
            if (String.IsNullOrEmpty(tendn) && String.IsNullOrEmpty(mk))
            {
                ViewData["Loi1"] = "Phải nhập username";
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập username";
            }
            else if (String.IsNullOrEmpty(mk))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                Admin ad = db.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == mk);
                if (ad != null)
                {
                    //ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }



            return View();
        }
        [HttpGet]
        public ActionResult Themmoisach()
        {
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Themmoisach(SACH sach, HttpPostedFileBase fileupload)
        {
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            //kiem tra duong dan file
            if (fileupload == null)
            {
                ViewBag.ThongBao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            //them vao CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    //luu ten file
                    var fileName = Path.GetFileName(fileupload.FileName);
                    //luu duong dan file
                    var path = Path.Combine(Server.MapPath("~/images"), fileName);
                    //kiem tra hinh anh ton tai
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tổn tại";
                    }
                    else
                    {
                        //luu hinh anh vao duong dan
                        fileupload.SaveAs(path);
                    }

                    sach.Hinhminhhoa = fileName;
                    db.SACHes.InsertOnSubmit(sach);
                    db.SubmitChanges();
                }
                return RedirectToAction("Sach");
            }
        }

        public ActionResult Chitietsach(int id)
        {
            //Lay dt oi tuong sach theo masach
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            { Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }

        public ActionResult Xoasach(int id)
        {
            //Lay ra doi tuong can xoa theo ma
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        [HttpPost,ActionName("Xoasach")]
        public ActionResult Xacnhanxoa(int id)
        {
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.SACHes.DeleteOnSubmit(sach);
            db.SubmitChanges();
            return RedirectToAction("Sach");
        }

        //ChinhSuaSach
        [HttpGet]
        public ActionResult Suasach(int id)
        {
            //Lấy ra đối tượng sách cần sửa theo mã
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.MaCD), "MaCD", "MaCD");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.MaNXB), "MaNXB", "MaNXB");
            return View(sach);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Suasach(SACH sach, HttpPostedFileBase fileUpload)
        {
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChude");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            var s = db.SACHes.SingleOrDefault(n => n.Masach == sach.Masach);
            //Kiểm tra đường dẫn file
            //if (fileUpload == null)
            //{
            //    s.Tensach = sach.Tensach;
            //    s.Mota = sach.Mota;
            //    s.Dongia = sach.Dongia;
            //    s.MaCD = sach.MaCD;
            //    s.MaNXB = sach.MaNXB;
            //    s.Ngaycapnhat = sach.Ngaycapnhat;
            //    s.Soluongban = sach.Soluongban;
            //    db.SubmitChanges();
            //    return View(sach);
            //}
            //Thêm vào CSDL
            //else
            //{
            if (ModelState.IsValid)
            {
                //Lưu tên file, lưu ý bổ sung thư viên System.IO;
                //var fileName = Path.GetFileName(fileUpload.FileName);
                ////Lưu đường dẫn của file
                //var path = Path.Combine(Server.MapPath("~/images"), fileName);
                //// Kiễm tra hình ảnh tồn tại
                //if (System.IO.File.Exists(path))
                //{
                //    ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                //}
                //else
                //{
                //    //Lưu hình ảnh vào đường dẫn
                //    fileUpload.SaveAs(path);
                //}
                //sach.Hinhminhhoa = fileName;
                // Lưu vào CSDL
                //var s = db.SACHes.SingleOrDefault(n => n.Masach == sach.Masach);
                s.Tensach = sach.Tensach;
                s.Mota = sach.Mota;
                s.Dongia = sach.Dongia;
                s.MaCD = sach.MaCD;
                s.MaNXB = sach.MaNXB;
                s.Ngaycapnhat = sach.Ngaycapnhat;
                //db.SACHes.InsertOnSubmit(s);
                db.SubmitChanges();
            }
            return RedirectToAction("Sach");
            //}
        }

       
    }
}