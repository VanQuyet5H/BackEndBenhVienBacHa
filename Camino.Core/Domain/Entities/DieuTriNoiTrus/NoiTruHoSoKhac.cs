using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.DieuTriNoiTrus
{
    public class NoiTruHoSoKhac : BaseEntity
    {
        public long YeuCauTiepNhanId { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long NhanVienThucHienId { get; set; }
        public DateTime ThoiDiemThucHien { get; set; }
        public long NoiThucHienId { get; set; }

        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual NhanVien NhanVienThucHien { get; set; }
        public virtual PhongBenhVien NoiThucHien { get; set; }

        private ICollection<NoiTruHoSoKhacFileDinhKem> _noiTruHoSoKhacFileDinhKems;
        public virtual ICollection<NoiTruHoSoKhacFileDinhKem> NoiTruHoSoKhacFileDinhKems
        {
            get => _noiTruHoSoKhacFileDinhKems ?? (_noiTruHoSoKhacFileDinhKems = new List<NoiTruHoSoKhacFileDinhKem>());
            protected set => _noiTruHoSoKhacFileDinhKems = value;
        }
    }
}
