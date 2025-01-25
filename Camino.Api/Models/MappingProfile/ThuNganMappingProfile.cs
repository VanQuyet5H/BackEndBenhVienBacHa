using AutoMapper;
using Camino.Api.Models.ThongTinBenhNhan;
using Camino.Api.Models.Thuoc;
using Camino.Core.Domain.ValueObject.DanhSachBenhNhanChoThuNgan;
using Camino.Core.Domain.ValueObject.QuayThuoc;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class ThuNganMappingProfile : Profile
    {
        public ThuNganMappingProfile()
        {
            var index = 1;
            CreateMap<DanhSachBenhNhanChoThuNganGridVo, ThuTienExportExcel>()
                .AfterMap((s, d) =>
                {
                    d.STT = index + 1;
                    d.SoTienBNPhaiTT = s.SoTienBNPhaiTT.ToString();
                    d.SoTienBNDaTT = s.SoTienBNDaTT.ToString();
                    d.Status = s.ChuaThu == true ? "Chưa thanh toán xong" : "Đã thu";
                });

            CreateMap<DanhSachLichSuThuNganGridVo, LichSuThuTienExportExcel>()
             .AfterMap((s, d) =>
             {
                 d.STT = index + 1;
                 d.SoTienThu = s.SoTienThu.GetValueOrDefault().ToString();
                 d.TienMat = s.TienMat.GetValueOrDefault().ToString();
                 d.ChuyenKhoan = s.ChuyenKhoan.GetValueOrDefault().ToString();
                 d.Pos = s.Pos.GetValueOrDefault().ToString();

             });


            CreateMap<ThongTinHuyPhieuViewModel, ThongTinHuyPhieuVo>();
            CreateMap<ThongTinHuyPhieuXuatTrongNgayViewModel, ThongTinHuyPhieuXuatTrongNgayVo>();
        }
    }
}
