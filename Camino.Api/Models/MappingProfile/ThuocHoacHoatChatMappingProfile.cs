using AutoMapper;
using Camino.Api.Models.Thuoc;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Thuoc;

namespace Camino.Api.Models.MappingProfile
{
    public class ThuocHoacHoatChatMappingProfile : Profile
    {
        public ThuocHoacHoatChatMappingProfile()
        {
            CreateMap<ThuocHoacHoatChat, ThuocHoacHoatChatViewModel>()
                .AfterMap((source, dest) =>
                {
                    dest.DuongDung = source.DuongDung != null ? source.DuongDung?.Ma + " - " + source.DuongDung?.Ten : "";
                    dest.NhomThuoc = source.NhomThuoc?.Ten;
                });

            CreateMap<ThuocHoacHoatChatViewModel, ThuocHoacHoatChat>()
                .ForMember(d => d.DuongDung, o => o.Ignore())
                .ForMember(d => d.NhomThuoc, o => o.Ignore());

            CreateMap<ThuocHoacHoatChatGridVo, ThuocHoacHoatChatExportExcel>()
                .AfterMap((source, dest) =>
                {
                    dest.SttHoatChat = source.STTHoatChat.ToString();
                    dest.SttThuoc = source.STTThuoc.ToString();
                    dest.HoiChan = source.HoiChan != true ? "Không" : "Có";
                    dest.TyLeBaoHiemThanhToan = source.TyLeBaoHiemThanhToan.ToString();
                    dest.CoDieuKienThanhToan = source.CoDieuKienThanhToan != true ? "Không" : "Có";
                    dest.BenhVienHang1 = ProcessResultForHangBenhVien(source.BenhVienHang1);
                    dest.BenhVienHang2 = ProcessResultForHangBenhVien(source.BenhVienHang2);
                    dest.BenhVienHang3 = ProcessResultForHangBenhVien(source.BenhVienHang3);
                    dest.BenhVienHang4 = ProcessResultForHangBenhVien(source.BenhVienHang4);
                });
        }

        private string ProcessResultForHangBenhVien(bool? hangBenhVien)
        {
            if (hangBenhVien == null)
            {
                return string.Empty;
            }

            return hangBenhVien == true ? "Có" : "Không";
        }
    }
}
