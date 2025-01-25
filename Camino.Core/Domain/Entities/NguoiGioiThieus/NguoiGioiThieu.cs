using System.Collections.Generic;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using System.ComponentModel.DataAnnotations.Schema;

namespace Camino.Core.Domain.Entities.NguoiGioiThieus
{
    public class NguoiGioiThieu : BaseEntity
    {
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public long NhanVienQuanLyId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoDienThoaiDisplay { get; set; }

        public virtual NhanViens.NhanVien NhanVien { get; set; }

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhanNguoiGioiThieus;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhanNguoiGioiThieus
        {
            get => _yeuCauTiepNhanNguoiGioiThieus ?? (_yeuCauTiepNhanNguoiGioiThieus = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhanNguoiGioiThieus = value;
        }
    }
}
