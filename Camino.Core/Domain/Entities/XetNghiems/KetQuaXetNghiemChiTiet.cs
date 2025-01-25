using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.DichVuXetNghiems;
using Camino.Core.Domain.Entities.MauMayXetNghiems;
using Camino.Core.Domain.Entities.MayXetNghiems;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.XetNghiems
{
    public class KetQuaXetNghiemChiTiet : BaseEntity
    {
        public string BarCodeID { get; set; }
        public int BarCodeNumber { get; set; }

        public long PhienXetNghiemChiTietId { get; set; }
        public long YeuCauDichVuKyThuatId { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }
        public long NhomDichVuBenhVienId { get; set; }

        public int LanThucHien { get; set; }

        public long DichVuXetNghiemId { get; set; }
        public long? DichVuXetNghiemChaId { get; set; }

        public string DichVuXetNghiemMa { get; set; }
        public string DichVuXetNghiemTen { get; set; }

        public int CapDichVu { get; set; }
        public string DonVi { get; set; }
        public int? SoThuTu { get; set; }

        public long? DichVuXetNghiemKetNoiChiSoId { get; set; }

        public string MaChiSo { get; set; }
        public double? TiLe { get; set; }

        public long? MauMayXetNghiemId { get; set; }
        public long? MayXetNghiemId { get; set; }

        public string GiaTriTuMay { get; set; }
        public string GiaTriNhapTay { get; set; }
        public string GiaTriDuyet { get; set; }
        public string GiaTriCu { get; set; }

        public long? NhanVienNhapTayId { get; set; }

        public string GiaTriMin { get; set; }
        public string GiaTriMax { get; set; }
        public string GiaTriNguyHiemMin { get; set; }
        public string GiaTriNguyHiemMax { get; set; }

        public bool? GiaTriKhacThuong { get; set; }
        public bool? GiaTriNguyHiem { get; set; }
        public bool? ToDamGiaTri { get; set; }
        public bool? DaDuyet { get; set; }
        public DateTime? ThoiDiemGuiYeuCau { get; set; }
        public DateTime? ThoiDiemNhanKetQua { get; set; }
        public DateTime? ThoiDiemDuyetKetQua { get; set; }

        public long? NhanVienDuyetId { get; set; }

        public string Rack { get; set; }
        public string Comment { get; set; }
        public string StripType { get; set; }
        public string LotId { get; set; }

        public virtual PhienXetNghiemChiTiet PhienXetNghiemChiTiet { get; set; }
        public virtual YeuCauDichVuKyThuat YeuCauDichVuKyThuat { get; set; }
        public virtual DichVuKyThuatBenhVien DichVuKyThuatBenhVien { get; set; }
        public virtual NhomDichVuBenhVien.NhomDichVuBenhVien NhomDichVuBenhVien { get; set; }
        public virtual DichVuXetNghiem DichVuXetNghiem { get; set; }
        public virtual DichVuXetNghiem DichVuXetNghiemCha { get; set; }
        public virtual DichVuXetNghiemKetNoiChiSo DichVuXetNghiemKetNoiChiSo { get; set; }
        public virtual MauMayXetNghiem MauMayXetNghiem { get; set; }
        public virtual MayXetNghiem MayXetNghiem { get; set; }

        public virtual NhanVien NhanVienNhapTay { get; set; }
        public virtual NhanVien NhanVienDuyet { get; set; }
       

    }
    public class ListInKetQua { 
        public long IdYeuCauDichVuKyThuat { get; set; }
        public long Id { get; set; }
    }
}
