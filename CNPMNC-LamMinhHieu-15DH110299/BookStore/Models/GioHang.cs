using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Models
{
    public class GioHang
    {
        dbQLBansachDataContext data = new dbQLBansachDataContext();
        public int iMasach { get; set; }
        public string sTensach { get; set; }
        public string sHinhminhhoa { get; set; }
        public Double dDongia { get; set; }
        public int iSoluong { get; set; }
        public Double dThanhtien
        {
            get
            {
                return iSoluong * dDongia;
            }
        }

        public GioHang(int Masach)
        {
            iMasach = Masach;
            SACH sach = data.SACHes.Single(n => n.Masach == iMasach);
            sTensach = sach.Tensach;
            sHinhminhhoa = sach.Hinhminhhoa;
            dDongia = double.Parse(sach.Dongia.ToString());
            iSoluong = 1;
        }
    }
}