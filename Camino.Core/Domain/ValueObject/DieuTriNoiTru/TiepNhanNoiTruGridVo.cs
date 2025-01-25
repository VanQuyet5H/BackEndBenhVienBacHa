using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class TiepNhanNoiTruGridVo : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public string MaTiepNhan { get; set; }
        public long BenhNhanId { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        
        //Cập nhật 03/06/2022
        public string HoTenTheoBenhNhanId { get; set; }

        public string GioiTinh { get; set; }
        public long KhoaNhapVienId { get; set; }
        public string KhoaNhapVien { get; set; }
        public DateTime? ThoiGianTiepNhan { get; set; }
        public string ThoiGianTiepNhanDisplay =>
            ThoiGianTiepNhan != null ? ThoiGianTiepNhan.Value.ApplyFormatDateTimeSACH() : null;
        public string SoBenhAn { get; set; }
        public string NoiChiDinh { get; set; }
        public string ChanDoan { get; set; }
        public long? ChanDoanNhapVienId { get; set; }
        public string DoiTuong => MucHuong != null ? "BHYT ("+MucHuong+"%)" : "Viện phí";
        public int? MucHuong { get; set; }
        public bool CapCuu { get; set; }
        public Enums.TrangThaiBenhAn TrangThai { get; set; }
        public string TenTrangThai => TrangThai.GetDescription();
        public DateTime? ThoiGianNhapVien { get; set; }
        public string ThoiGianNhapVienDisplay => ThoiGianNhapVien?.ApplyFormatDateTimeSACH();

        public bool DaTiepNhan { get; set; }

        //cập nhật 11/07/2022: fix grid load chậm
        public long? NoiTruBenhAnId { get; set; }
        public bool? CoBHYT { get; set; }
        public int? BHYTMucHuong { get; set; }
        public bool? BenhAnCapCuu { get; set; }
    }

    public class TiepNhanNoiTruTimKiemNangCaoVo
    {
        public string SearchString { get; set; }
        public long? KhoaPhongId { get; set; }
        public TiepNhanNoiTruTimKiemTrangThaiVo TrangThai { get; set; }
        public TiepNhanNoiTruTimKiemTuNgayDenNgayVo TuNgayDenNgay { get; set; }
    }

    public class TiepNhanNoiTruTimKiemTrangThaiVo
    {
        public bool ChoQuyetToan { get; set; }
        public bool ChuaTaoBenhAn { get; set; }
        public bool DaTaoBenhAn { get; set; }
    }

    public class TiepNhanNoiTruTimKiemTuNgayDenNgayVo
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
    }

    public class TiepNhanNoiTruSoDoGiuongGridVo : GridItem
    {
        public TiepNhanNoiTruSoDoGiuongGridVo()
        {
            GiuongBenhs = new List<TiepNhanNoiTruGiuongVo>();
        }
        public string Tang { get; set; }
        public string Phong { get; set; }
        public bool CoTheBaoPhong => GiuongBenhs.All(x => x.IsEmpty)

                                     // 18/11/2021: bổ sung trường hợp chỉ định lại giường đang nằm của bệnh nhân hiện tại
                                     // trường hợp này là trong phòng có người bệnh sử dụng và người bệnh đó chính là người bệnh đang chọn
                                     || (GiuongBenhs.SelectMany(x => x.BenhNhans.Select(y => y.BenhNhanId)).Distinct().Count() == 1 && GiuongBenhs.Any(x => x.BenhNhans.Any(y => y.LaGiuongDangSuDung == true)));
        public bool DisabledPhong => GiuongBenhs.Any(x => x.BenhNhans.Any(y => y.BaoPhong == true
                                                                               // 18/11/2021: bổ sung trường hợp chỉ định lại giường đang nằm của bệnh nhân hiện tại
                                                                               && y.LaGiuongDangSuDung != true));
        public List<TiepNhanNoiTruGiuongVo> GiuongBenhs { get; set; }

        public List<TiepNhanNoiTruGiuongVo> GiuongBenhDisplays
        {
            get
            {
                var thongTinBaoPhong = GiuongBenhs.FirstOrDefault(x => x.BenhNhans.Any(y => y.BaoPhong == true));
                if (thongTinBaoPhong != null)
                {
                    var giuongBenhs = new List<TiepNhanNoiTruGiuongVo>();
                    foreach (var item in GiuongBenhs)
                    {
                        var itemClone = item.Clone();
                        //if (itemClone.GiuongId != thongTinBaoPhong.GiuongId)
                        //{
                        //    itemClone.BenhNhans = new List<TiepNhanNoiTruBenhNhanTrenGiuongVo>();
                        //}
                        giuongBenhs.Add(itemClone);
                        //giuongBenhs.Add(thongTinBaoPhong);
                    }
                    return giuongBenhs;
                }
                else
                {
                    return GiuongBenhs;
                }
            }
        }
    }

    public class TiepNhanNoiTruGiuongVo
    {
        public TiepNhanNoiTruGiuongVo()
        {
            BenhNhans = new List<TiepNhanNoiTruBenhNhanTrenGiuongVo>();
        }
        public string TenGiuong { get; set; }
        public long GiuongId { get; set; }
        public int SoLuongBenhNhan => BenhNhans.Count;
        public int SoLuongBenhNhanToiDa { get; set; }
        public bool IsEmpty => SoLuongBenhNhan == 0;
        public bool IsHalf => (SoLuongBenhNhan > 0 && SoLuongBenhNhan < SoLuongBenhNhanToiDa)
                              // 18/11/2021: bổ sung trường hợp chỉ định lại giường đang nằm của bệnh nhân hiện tại
                              || BenhNhans.Any(x => x.LaGiuongDangSuDung == true);
        public bool IsFull => SoLuongBenhNhanToiDa <= SoLuongBenhNhan
                              // 18/11/2021: bổ sung trường hợp chỉ định lại giường đang nằm của bệnh nhân hiện tại
                              && BenhNhans.All(x => x.LaGiuongDangSuDung != true);

        public List<TiepNhanNoiTruBenhNhanTrenGiuongVo> BenhNhans { get; set; }

        #region clone
        public TiepNhanNoiTruGiuongVo Clone()
        {
            return (TiepNhanNoiTruGiuongVo)this.MemberwiseClone();
        }
        #endregion
    }

    public class TiepNhanNoiTruBenhNhanTrenGiuongVo
    {
        public string DichVuGiuong { get; set; }
        public string MaGiuong { get; set; }
        public string Phong { get; set; }
        public string Tang { get; set; }
        public string SoBenhAn { get; set; }
        public string TenBenhNhan { get; set; }
        public string NgayVao { get; set; }
        public string NgayRa { get; set; }
        public bool? BaoPhong { get; set; }
        public string CoBaoPhong => BaoPhong == true ? "Có" : "Không";

        // 18/11/2021: bổ sung trường hợp chỉ định lại giường đang nằm của bệnh nhân hiện tại
        public bool? LaGiuongDangSuDung { get; set; }
        public long? BenhNhanId { get; set; }
    }

    public class TiepNhanNoiTruSoDoGiuongTimKiemNangCaoVo
    {
        public long KhoaPhongId { get; set; }
        public long? PhongBenhVienId { get; set; }
        public bool GiuongTrong { get; set; }
        public bool GiuongDaCoBenhNhan { get; set; }
        public DateTime? ThoiGianNhan { get; set; }
        public DateTime? ThoiGianTra { get; set; }
        public long? YeuCauDichVuGiuongBenhVienId { get; set; }
        public long? YeuCauTiepNhanNoiTruId { get; set; }
    }


    public class TiepNhanNoiTruLichSuChuyenDoiTuongGridVo : GridItem
    {
        public string DoiTuong { get; set; }
        public string SoTheBaoHiem { get; set; }
        public string DiaChiThe { get; set; }
        public string MucHuong { get; set; }
        public DateTime? TuNgay { get; set; }
        public string TuNgayDisplay => TuNgay?.ApplyFormatDate();
        public DateTime? DenNgay { get; set; }
        public string DenNgayDisplay => DenNgay?.ApplyFormatDate();
        public string NoiDangKyBaoHiem { get; set; }
        public DateTime? NgayNhap { get; set; }
        public string NgayNhapDisplay => NgayNhap?.ApplyFormatDate();
        public DateTime? ThoiGianMienDongChiTra { get; set; }
        public string ThoiGianMienDongChiTraDisplay => ThoiGianMienDongChiTra?.ApplyFormatDate();
        public bool? GiaHanThe { get; set; }
        public bool? DaHuy { get; set; }
        public string TinhTrang =>
            string.IsNullOrEmpty(SoTheBaoHiem) ? null : (DaHuy == true ? "Đã hủy" : "Đang sử dụng");
    }

    public class LookupItemNoiTruCongTyBHTNVo : LookupItemVo
    {
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
    }

    public class ThongTinNhapVienTheoKhoaVo
    {
        public ThongTinNhapVienTheoKhoaVo()
        {
            YeuCauTiepNhanIds = new List<long>();
        }
        public long YeuCauNhapVienId { get; set; }
        public List<long> YeuCauTiepNhanIds { get; set; }
    }
}
