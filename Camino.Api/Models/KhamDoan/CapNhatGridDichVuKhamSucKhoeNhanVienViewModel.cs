using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;

namespace Camino.Api.Models.KhamDoan
{
    public class CapNhatGridDichVuKhamSucKhoeNhanVienViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        //public Enums.NhomDichVuChiDinhKhamSucKhoe LoaiDichVu { get; set; }
        public bool LaDichVuKham { get; set; }
        public long YeuCauDichVuBenhVienId { get; set; }
        public decimal? DonGia { get; set; }
        public int? SoLan { get; set; }
        public long? NoiThucHienId { get; set; }
        public bool IsUpdateDonGia { get; set; }
        public bool IsUpdateSoLan { get; set; }
        public bool IsUpdateNoiThucHien { get; set; }

        //Cập nhật 31/03/2022
        public bool? XoaDichVuDaChiDinhTrongGoiChung { get; set; }
        public long? GoiKhamSucKhoeChungDichVuKhamBenhNhanVienId { get; set; }
        public long? GoiKhamSucKhoeChungDichVuKyThuatNhanVienId { get; set; }
    }
}
