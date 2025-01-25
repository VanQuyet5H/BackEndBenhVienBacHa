using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Core.Domain.Entities.DieuTriNoiTrus
{
    public class NoiTruPhieuDieuTriChiTietYLenh : BaseEntity
    {
        //public long NoiTruPhieuDieuTriId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long? NoiTruChiDinhDuocPhamId { get; set; }
        public long? YeuCauVatTuBenhVienId { get; set; }
        public long? YeuCauTruyenMauId { get; set; }
        public string MoTaYLenh { get; set; }
        public int GioYLenh { get; set; }
        public long NhanVienChiDinhId { get; set; }
        public long NoiChiDinhId { get; set; }
        public bool? XacNhanThucHien { get; set; }
        public DateTime? ThoiDiemXacNhanThucHien { get; set; }
        public long? NhanVienXacNhanThucHienId { get; set; }
        public string LyDoKhongThucHien { get; set; }
        public DateTime? ThoiDiemCapNhat { get; set; }
        public long? NhanVienCapNhatId { get; set; }

        //BVHD-3312
        public long NoiTruBenhAnId { get; set; }
        public DateTime NgayDieuTri { get; set; }

        //BVHD-3575
        public long? YeuCauKhamBenhId { get; set; }

        //public virtual NoiTruPhieuDieuTri NoiTruPhieuDieuTri { get; set; }
        public virtual YeuCauDichVuKyThuat YeuCauDichVuKyThuat { get; set; }
        public virtual NoiTruChiDinhDuocPham NoiTruChiDinhDuocPham { get; set; }
        public virtual YeuCauVatTuBenhVien YeuCauVatTuBenhVien { get; set; }
        public virtual YeuCauTruyenMau YeuCauTruyenMau { get; set; }
        public virtual NhanVien NhanVienChiDinh { get; set; }
        public virtual PhongBenhVien NoiChiDinh { get; set; }
        public virtual NhanVien NhanVienCapNhat { get; set; }
        public virtual NhanVien NhanVienXacNhanThucHien { get; set; }

        //BVHD-3312
        public virtual NoiTruBenhAn NoiTruBenhAn { get; set; }

        //BVHD-3575
        public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }

        #region clone
        public NoiTruPhieuDieuTriChiTietYLenh Clone()
        {
            return (NoiTruPhieuDieuTriChiTietYLenh)this.MemberwiseClone();
        }
        #endregion
    }
}
