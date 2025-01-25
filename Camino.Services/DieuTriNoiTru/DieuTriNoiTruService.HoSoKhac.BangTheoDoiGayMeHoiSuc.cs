using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public NoiTruHoSoKhac GetThongTinHoSoKhacBangTheoDoiGayMeHoiSuc(long yeuCauTiepNhanId, long? hoSoKhacId)
        {
            return _noiTruHoSoKhacRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                        (hoSoKhacId == null || hoSoKhacId == 0 || p.Id == hoSoKhacId.GetValueOrDefault()) &&
                                                                        p.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.BangTheoDoiGayMeHoiSuc)
                                                            .Include(p => p.NoiTruHoSoKhacFileDinhKems)
                                                            .Include(p => p.NhanVienThucHien).ThenInclude(p => p.User)
                                                            .Include(p => p.NoiThucHien)
                                                            .LastOrDefault();
        }

        public async Task<GridDataSource> GetDanhSachBangTheoDoiGayMeHoiSuc(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            long.TryParse(queryInfo.AdditionalSearchString, out long yeuCauTiepNhanId);

            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                             p.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.BangTheoDoiGayMeHoiSuc)
                                                                 .Select(p => new DanhSachBangTheoDoiGayMeHoiSucGridVo
                                                                 {
                                                                     Id = p.Id,
                                                                     ThongTinHoSo = p.ThongTinHoSo
                                                                     //NgayThucHien = p.ThoiDiemThucHien
                                                                 });

            var queryTask = query.Skip(queryInfo.Skip)
                                 .Take(queryInfo.Take)
                                 .ToArrayAsync();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();

            await Task.WhenAll(countTask, queryTask);

            foreach (var item in queryTask.Result)
            {
                var thongTinHoSo = JsonConvert.DeserializeObject<HoSoKhacBangTheoDoiGayMeHoiSucVo>(item.ThongTinHoSo);

                item.NgayThucHien = thongTinHoSo.NgayThucHien.GetValueOrDefault();
                item.LoaiMo = thongTinHoSo.LoaiMo;
                item.NguoiMo = thongTinHoSo.NguoiMo;
            }

            var result = queryTask.Result.OrderBy(p => p.NgayThucHien).ToArray();

            return new GridDataSource
            {
                Data = result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPagesDanhSachBangTheoDoiGayMeHoiSuc(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            long.TryParse(queryInfo.AdditionalSearchString, out long yeuCauTiepNhanId);

            var query = await _noiTruHoSoKhacRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                             p.LoaiHoSoDieuTriNoiTru == LoaiHoSoDieuTriNoiTru.BangTheoDoiGayMeHoiSuc)
                                                                       .CountAsync();

            return new GridDataSource
            {
                TotalRowCount = query
            };
        }

        public async Task<string> InBangTheoDoiGayMeHoiSuc(long yeuCauTiepNhanId, long hoSoKhacId, bool isInFilePDF = true)
        {
            var today = DateTime.Now;

            var template = _templateRepository.TableNoTracking.FirstOrDefault(p => p.Name.Equals(isInFilePDF ? "BangTheoDoiGayMeHoiSucPDF" : "BangTheoDoiGayMeHoiSuc"));

            var yeuCauTiepNhan = await BaseRepository.TableNoTracking.Where(p => p.Id.Equals(yeuCauTiepNhanId))
                                                                     .Include(p => p.NoiTruBenhAn)
                                                                     .Include(p => p.NoiTruHoSoKhacs).ThenInclude(p => p.NhanVienThucHien).ThenInclude(p => p.User)
                                                                     .FirstOrDefaultAsync();

            var noiTruHoSoKhac = yeuCauTiepNhan.NoiTruHoSoKhacs.Where(p => p.Id == hoSoKhacId)
                                                               .FirstOrDefault();

            if (noiTruHoSoKhac == null || noiTruHoSoKhac.ThongTinHoSo == null)
            {
                var defaultData = new
                {
                    //MaSoTTVPhi = thongTin.MaSoThongTinVienPhi,
                    HoTen = yeuCauTiepNhan.HoTen,
                    Tuoi = yeuCauTiepNhan.NamSinh != null ? (today.Year - yeuCauTiepNhan.NamSinh.Value).ToString() : "&nbsp;",
                    GioiTinh = yeuCauTiepNhan.GioiTinh?.GetDescription(),
                    //ChanDoan = thongTin.ChanDoan,
                    //Ngay = thongTin.NgayThucHien?.Day,
                    //Thang = thongTin.NgayThucHien?.Month,
                    //Nam = thongTin.NgayThucHien?.ToString("yy"),
                    //TienMe = thongTin.TienMe,
                    //TacDung = thongTin.TacDung,
                    //Nang = thongTin.Nang,
                    //Cao = thongTin.Cao,
                    //LoaiMo = thongTin.LoaiMo,
                    //TuTheMo = thongTin.TuTheMo,
                    //NguoiGayMe = thongTin.NguoiGayMe,
                    //NguoiMo = thongTin.NguoiMo,
                    //PhuongPhapVoCam = thongTin.PhuongPhapVoCam,
                    NhomMau = yeuCauTiepNhan.NhomMau?.GetDescription(),
                    //NguoiThuCheo = thongTin.NguoiThuCheo,
                };

                return TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, defaultData);
            }

            var thongTin = JsonConvert.DeserializeObject<HoSoKhacBangTheoDoiGayMeHoiSucVo>(noiTruHoSoKhac.ThongTinHoSo);

            var data = new HoSoKhacBangInTheoDoiGayMeHoiSucVo
            {
                MaSoTTVPhi = thongTin.MaSoThongTinVienPhi,
                HoTen = yeuCauTiepNhan.HoTen,
                Tuoi = yeuCauTiepNhan.NamSinh != null ? (today.Year - yeuCauTiepNhan.NamSinh.Value).ToString() : "&nbsp;",
                GioiTinh = yeuCauTiepNhan.GioiTinh?.GetDescription(),
                ChanDoan = thongTin.ChanDoan,
                Ngay = thongTin.NgayThucHien?.Day,
                Thang = thongTin.NgayThucHien?.Month,
                Nam = thongTin.NgayThucHien?.ToString("yy"),
                TienMe = thongTin.TienMe,
                TacDung = thongTin.TacDung,
                Nang = thongTin.Nang,
                Cao = thongTin.Cao,
                LoaiMo = thongTin.LoaiMo,
                TuTheMo = thongTin.TuTheMo,
                NguoiGayMe = thongTin.NguoiGayMe,
                NguoiMo = thongTin.NguoiMo,
                PhuongPhapVoCam = thongTin.PhuongPhapVoCam,
                NhomMau = yeuCauTiepNhan.NhomMau?.GetDescription(),
                NguoiThuCheo = thongTin.NguoiThuCheo,
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);

            return content;
        }
    }
}