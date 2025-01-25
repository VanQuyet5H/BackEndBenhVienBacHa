using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class LuuThongTinChuyenGoiMoi
    {
        public long YeuCauGoiDichVuId { get; set; }
        public string TenGoiMoi { get; set; }
        public string MaGoiMoi { get; set; }
        public List<DichVuTrongGoiMarketingModel> DichVuTrongGoiMarketingModels { get; set; }     
    }

    public class DichVuTrongGoiMarketingModel
    {      
        public long DichVuBenhVienId { get; set; }      
        public EnumDichVuTongHop Nhom { get; set; }
        public string NhomDisplay => Nhom.GetDescription();
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string LoaiGia { get; set; }
        public long NhomGiaDichVuId { get; set; }    

        public int SoLuong { get; set; }
        public decimal DonGiaBenhVien { get; set; }

        public decimal DonGiaTruocChietKhau { get; set; }
        public decimal DonGiaSauChietKhau { get; set; }

        public decimal ThanhTienBenhVien => DonGiaBenhVien * SoLuong;
        public decimal ThanhTienTruocChietKhau => DonGiaTruocChietKhau * SoLuong;
        public decimal ThanhTienSauChietKhau => DonGiaSauChietKhau * SoLuong;
    }

    public class GoiMarketingJson
    {
        public long YeuCauGoiDichVuId { get; set; }       
    }
}
