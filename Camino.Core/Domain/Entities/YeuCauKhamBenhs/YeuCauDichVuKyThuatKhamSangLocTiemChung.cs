using Camino.Core.Domain.Entities.KetQuaSinhHieus;
using Camino.Core.Domain.Entities.NhanViens;
using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.PhongBenhViens;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauDichVuKyThuatKhamSangLocTiemChung : BaseEntity
    {        
        public string ThongTinKhamSangLocTiemChungTemplate { get; set; }
        public string ThongTinKhamSangLocTiemChungData { get; set; }
        public bool BenhNhanDeNghi { get; set; }
        public string LyDoDeNghi { get; set; }
        public LoaiKetLuanKhamSangLocTiemChung? KetLuan { get; set; }
        public string GhiChuKetLuan { get; set; }
        public int? SoNgayHenTiemMuiTiepTheo { get; set; }
        public string GhiChuHenTiemMuiTiepTheo { get; set; }
        public long? NhanVienHoanThanhKhamSangLocId { get; set; }
        public DateTime? ThoiDiemHoanThanhKhamSangLoc { get; set; }
        public long? NoiTheoDoiSauTiemId { get; set; }
        public long? NhanVienTheoDoiSauTiemId { get; set; }
        public DateTime? ThoiDiemTheoDoiSauTiem { get; set; }
        public string GhiChuTheoDoiSauTiem { get; set; }
        public LoaiPhanUngSauTiem? LoaiPhanUngSauTiem { get; set; }
        public string ThongTinPhanUngSauTiem { get; set; }

        public virtual YeuCauDichVuKyThuat YeuCauDichVuKyThuat { get; set; }
        public virtual NhanVien NhanVienHoanThanhKhamSangLoc { get; set; }
        public virtual NhanVien NhanVienTheoDoiSauTiem { get; set; }
        public virtual PhongBenhVien NoiTheoDoiSauTiem { get; set; }

        private ICollection<KetQuaSinhHieu> _ketQuaSinhHieus;
        public virtual ICollection<KetQuaSinhHieu> KetQuaSinhHieus
        {
            get => _ketQuaSinhHieus ?? (_ketQuaSinhHieus = new List<KetQuaSinhHieu>());
            protected set => _ketQuaSinhHieus = value;
        }

        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuats;
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuats
        {
            get => _yeuCauDichVuKyThuats ?? (_yeuCauDichVuKyThuats = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuats = value;
        }
    }
}
