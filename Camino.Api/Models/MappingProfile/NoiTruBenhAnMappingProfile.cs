using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Models.MappingProfile
{
    public class NoiTruBenhAnMappingProfile : Profile
    {
        public NoiTruBenhAnMappingProfile()
        {
            #region export

            CreateMap<TiepNhanNoiTruGridVo, DanhSachTiepNhanNoiTruExportExcel>().IgnoreAllNonExisting();


            #endregion

            #region xem bệnh án

            CreateMap<NoiTruBenhAnViewModel, NoiTruBenhAn>().IgnoreAllNonExisting();
            CreateMap<NoiTruBenhAn, NoiTruBenhAnViewModel>().IgnoreAllNonExisting();

            CreateMap<NoiTruBenhAnThongTinHanhChinhViewModel, Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan>().IgnoreAllNonExisting();
            CreateMap<Core.Domain.Entities.YeuCauTiepNhans.YeuCauTiepNhan, NoiTruBenhAnThongTinHanhChinhViewModel>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.MaBenhNhan = s.BenhNhan != null ? s.BenhNhan.MaBN : null;
                    //d.DoiTuong = s.CoBHYT == true ? "BHYT" : "Viện phí";
                    d.BHYTMucHuong
                        = //s.YeuCauTiepNhanTheBHYTs.Any() ? s.YeuCauTiepNhanTheBHYTs.OrderByDescending(x => x.Id).Select(x => x.MucHuong).First() : (int?) null;

                        //BVHD-3754
                        //s.YeuCauTiepNhanTheBHYTs.Any(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15))) 
                        //    ? s.YeuCauTiepNhanTheBHYTs.Where(a => a.NgayHieuLuc.Date <= DateTime.Now.Date && (a.NgayHetHan == null || a.NgayHetHan.Value.Date >= DateTime.Now.Date || (a.DuocGiaHanThe == true && (DateTime.Now.Date - a.NgayHetHan.Value.Date).Days <= 15)))
                        //                              .OrderByDescending(a => a.MucHuong).ThenBy(a => a.NgayHieuLuc)
                        //                              .Select(a => a.MucHuong).First() : (int?) null;
                        s.CoBHYT == true ? s.BHYTMucHuong : (int?)null;

                    d.DanToc = s.DanToc?.Ten;
                    d.NgheNghiep = s.NgheNghiep?.Ten;
                    d.ThongTinNhapVien = new NoiTruBenhAnYeuCauNhapVienViewModel()
                    {
                        KhoaNhapVien = s.YeuCauNhapVien?.KhoaPhongNhapVien?.Ten,
                        KhoaNhapVienId = s.YeuCauNhapVien?.KhoaPhongNhapVienId,
                        ChanDoanNhapVien = s.YeuCauNhapVien != null ? 
                            (string.IsNullOrEmpty(s.YeuCauNhapVien.ChanDoanNhapVienGhiChu) ? s.YeuCauNhapVien?.ChanDoanNhapVienICD?.Ma + " - " + s.YeuCauNhapVien?.ChanDoanNhapVienICD?.TenTiengViet : s.YeuCauNhapVien?.ChanDoanNhapVienGhiChu) : null,
                        TrangThaiDieuTri = s.NoiTruBenhAn == null ? Enums.TrangThaiDieuTri.ChoNhapVien : (s.NoiTruBenhAn.TinhTrangRaVien == null ? Enums.TrangThaiDieuTri.DangDieuTri : s.NoiTruBenhAn.ChuyenDenBenhVienId != null ? Enums.TrangThaiDieuTri.ChuyenVien : Enums.TrangThaiDieuTri.DaRaVien),
                        NoiChiDinh = s.YeuCauNhapVien?.NoiChiDinh?.Ten,
                        ChanDoanKemTheo = s.YeuCauNhapVien != null && s.YeuCauNhapVien.YeuCauNhapVienChanDoanKemTheos.Any() ? string.Join(", ", s.YeuCauNhapVien.YeuCauNhapVienChanDoanKemTheos.Select(a => string.IsNullOrEmpty(a.GhiChu) ? (a.ICD.Ma + " - " + a.ICD.TenTiengViet) : a.GhiChu)) : null,
                        NguoiTiepNhan = s.NhanVienTiepNhan?.User.HoTen,
                        BacSiChiDinh = s.YeuCauNhapVien?.BacSiChiDinh?.User.HoTen,
                        LyDoNhapVien = s.YeuCauNhapVien?.LyDoNhapVien
                    };
                });
            #endregion
        }
    }
}
