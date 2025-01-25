using System;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.TiemChung
{
    public class TiemChungThongTinPhanUngSauTiem
    {
        public bool? SungDauTaiChoTiem { get; set; }
        public bool? SotLonHonBang39DoPUTT { get; set; }
        public bool? TrieuChungKhacPUTT { get; set; }
        public string MoTaPhanUngPUTT { get; set; }

        public bool? SotLonHonBang39DoTBN { get; set; }
        public bool? SocTrongVong72Gio { get; set; }
        public bool? BenhNaoTrongVong7Ngay { get; set; }
        public bool? ApXeTaiChoTiem { get; set; }
        public bool? NhungConCoGiatTrong3Ngay { get; set; }
        public bool? TrieuChungKhacTBN { get; set; }
        public string MoTaPhanUngTBN { get; set; }
        public string TienSuBenhTat { get; set; }
        public bool? XuLyPhanUngSauTiem { get; set; }
        public NoiXuTriTheoDoiTiemVacxin[] NoiXuTri { get; set; }
        public string NoiXuTriDisplay { get; set; }
        public string NoiXuTriKhac { get; set; }
        public long? NguoiXuTriId { get; set; }
        public string NguoiXuTriDisplay { get; set; }
        public TinhTrangHienTaiTheoDoiTiemVacxin? TinhTrangHienTai { get; set; }
        public string TinhTrangHienTaiDisplay { get; set; }
        public string MoTaTinhTrangHienTai { get; set; }
        public DateTime? NgayTuVong { get; set; }
        public long? NguoiBaoCaoId { get; set; }
        public string NguoiBaoCaoDisplay { get; set; }

        public string MoTaPhanUngPUK { get; set; }
    }
}