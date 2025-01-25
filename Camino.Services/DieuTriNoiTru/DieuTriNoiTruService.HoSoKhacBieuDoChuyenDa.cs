using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public NoiTruHoSoKhac GetThongTinHoSoKhacBieuDoChuyenDa(long yeuCauTiepNhanId)
        {
            return _noiTruHoSoKhacRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                        p.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.BieuDoChuyenDa)
                                                            .Include(p => p.NhanVienThucHien).ThenInclude(p => p.User)
                                                            .Include(p => p.NoiThucHien)
                                                            .FirstOrDefault();
        }

        public async Task<string> InBieuDoChuyenDa(long yeuCauTiepNhanId, bool isInFilePDF = true)
        { 
            var template = _templateRepository.TableNoTracking.FirstOrDefault(p => p.Name.Equals(isInFilePDF ? "HoSoKhacBieuDoChuyenDaPDF" : "HoSoKhacBieuDoChuyenDa"));

            var yeuCauTiepNhan = await BaseRepository.TableNoTracking.Where(p => p.Id.Equals(yeuCauTiepNhanId))
                                                                     .Include(p => p.NoiTruBenhAn)
                                                                     .Include(p => p.NoiTruHoSoKhacs)
                                                                     .FirstOrDefaultAsync();

            var noiTruHoSoKhac = yeuCauTiepNhan.NoiTruHoSoKhacs.Where(p => p.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.BieuDoChuyenDa)
                                                               .FirstOrDefault();

            if (noiTruHoSoKhac == null || noiTruHoSoKhac.ThongTinHoSo == null)
            {
                var defaultData = new HoSoKhacGiayInBieuDoChuyenDa
                {
                    HoTen = yeuCauTiepNhan.HoTen,
                    Para = "",
                    SoNhapVien = yeuCauTiepNhan.MaYeuCauTiepNhan,
                    NgayVaoVienValue = yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien,
                    //NgayGhiBieuDo = thongTin.NgayGhiBieuDo,
                    OiDaVo = ""
                };

                return TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, defaultData);
            }

            var thongTin = JsonConvert.DeserializeObject<HoSoKhacBieuDoChuyenDaVo>(noiTruHoSoKhac.ThongTinHoSo);
            var loaiBenhAn = GetThongTinLoaiBenhAn(yeuCauTiepNhanId);
            var thongTinBenhAn = new ThongTinBenhAn();

            switch(loaiBenhAn)
            {
                case (int)LoaiBenhAn.SanKhoaMo:
                    thongTinBenhAn = GetThongTinBenhAnSanKhoaMoThuong(yeuCauTiepNhanId);
                    break;
                case (int)LoaiBenhAn.SanKhoaThuong:
                    thongTinBenhAn = GetThongTinBenhAnSK(yeuCauTiepNhanId);
                    break;
            }

            var data = new HoSoKhacGiayInBieuDoChuyenDa
            {
                HoTen = yeuCauTiepNhan.HoTen,
                Para = "",
                SoNhapVien = yeuCauTiepNhan.MaYeuCauTiepNhan,
                NgayVaoVienValue = yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien,
                NgayGhiBieuDoValue = thongTin.NgayGhiBieuDo,
                OiDaVo = thongTinBenhAn.NgayVoNuocOi?.ApplyFormatDateTimeSACH()
            };

            if(thongTin.TienThaiPara1 != null || thongTin.TienThaiPara2 != null || thongTin.TienThaiPara3 != null || thongTin.TienThaiPara4 != null)
            {
                data.Para = $"{thongTin.TienThaiPara1?.ToString() ?? "&nbsp;&nbsp;"} - {thongTin.TienThaiPara2?.ToString() ?? "&nbsp;&nbsp;"} - {thongTin.TienThaiPara3.ToString() ?? "&nbsp;&nbsp;"} - {thongTin.TienThaiPara4?.ToString() ?? "&nbsp;&nbsp;"}";
            }

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);

            return content;
        }
    }
}
