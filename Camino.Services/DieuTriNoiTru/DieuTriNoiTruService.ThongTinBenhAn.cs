using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;
using System.Linq.Dynamic.Core;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        #region Danh sáchnộ dung mẫu lời dặn bác sĩ 

        public async Task<GridDataSource> GetDataForGridNoiDungMauLoiDanBacSiAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var loaiBenhAn = long.Parse(queryInfo.AdditionalSearchString);
            var query = _noiDungMauLoiDanBacSiRepository.TableNoTracking
                .Where(x => x.LoaiBenhAn == loaiBenhAn)
                .Select(s => new NoiDungMauLoiDanBacSiGridVo()
                {
                    Id = s.Id,
                    Ma = s.Ma,
                    Ten = s.Ten,
                    LoaiBenhAn = s.LoaiBenhAn
                }).ApplyLike(queryInfo.SearchTerms, g => g.Ma, g => g.Ten);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridNoiDungMauLoiDanBacSiAsync(QueryInfo queryInfo)
        {
            var loaiBenhAn = long.Parse(queryInfo.AdditionalSearchString);
            var query = _noiDungMauLoiDanBacSiRepository.TableNoTracking
               .Where(x => x.LoaiBenhAn == loaiBenhAn)
                .Select(s => new NoiDungMauGridVo()
                {
                    Id = s.Id,
                    Ma = s.Ma,
                    Ten = s.Ten
                }).ApplyLike(queryInfo.SearchTerms, g => g.Ma, g => g.Ten);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        #endregion

        #region Lấy loại thông tin bệnh án

        public int GetThongTinLoaiBenhAn(long yeuCauTiepNhanId)
        {
            var noiTruBenhAn = _noiTruBenhAnRepository.TableNoTracking.Where(cc => cc.Id == yeuCauTiepNhanId).FirstOrDefault();
            return (int)noiTruBenhAn.LoaiBenhAn;
        }

        #endregion

        #region bệnh sơ sinh

        public ThongTinBenhAn GetThongTinBenhAnTreSoSinh(long yeuCauTiepNhanId)
        {
            var thongTinBenhAnNoiKhoaNhi = new ThongTinBenhAn();
            var noiTruBenhAn = BaseRepository.Table.Where(cc => cc.Id == yeuCauTiepNhanId)
                                                .Select(cc => cc.NoiTruBenhAn)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauTiepNhan).ThenInclude(cc => cc.YeuCauDichVuKyThuats)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauDichVuKyThuats)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(cc => cc.Icdchinh)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhChuanDoans).ThenInclude(c => c.ChuanDoan)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhChanDoanPhanBiets).ThenInclude(c => c.ICD)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                .ThenInclude(cc => cc.YeuCauKhamBenhICDKhacs).ThenInclude(c => c.ICD)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.BenhNhan)
                                                .FirstOrDefault();

            thongTinBenhAnNoiKhoaNhi.NamSinh = noiTruBenhAn.YeuCauTiepNhan.NamSinh;

            if (noiTruBenhAn != null && noiTruBenhAn.ThongTinBenhAn != null)
            {
                thongTinBenhAnNoiKhoaNhi = JsonConvert.DeserializeObject<ThongTinBenhAn>(noiTruBenhAn.ThongTinBenhAn);

            }
            else
            {
                var thongTinKhamBenh = JsonConvert.SerializeObject(thongTinBenhAnNoiKhoaNhi);
                noiTruBenhAn.ThongTinBenhAn = thongTinKhamBenh;

                _noiTruBenhAnRepository.Update(noiTruBenhAn);
            }

            if (thongTinBenhAnNoiKhoaNhi.ICDChinh != null)
            {
                var icd = _icdRepository.TableNoTracking.Where(cc => cc.Id == thongTinBenhAnNoiKhoaNhi.ICDChinh.Value).FirstOrDefault();
                thongTinBenhAnNoiKhoaNhi.TenICDChinh = icd.Ma + " - " + thongTinBenhAnNoiKhoaNhi.TenICDChinh;
            }

            thongTinBenhAnNoiKhoaNhi.DacDiemTreSoSinhSauSinhs = new List<DacDiemTreSoSinhSauSinh>
            {
                new DacDiemTreSoSinhSauSinh {Diem = 0 ,SuGianNoLongNguc ="Điều hòa",CoKeoCoLienSuon="Không",CoKeoMuiUc="Không",DapCanhMui= "Không",Reni="Không"},
                new DacDiemTreSoSinhSauSinh {Diem = 1 ,SuGianNoLongNguc ="Xê dịch nhịp thở với di động bụng",CoKeoCoLienSuon="Có ít",CoKeoMuiUc="Có ít",DapCanhMui= "Nhẹ",Reni="Nghe bằng ống nghe"},
                new DacDiemTreSoSinhSauSinh {Diem = 2 ,SuGianNoLongNguc ="Không di động ngực bụng",CoKeoCoLienSuon="Thấy rõ",CoKeoMuiUc="Tháy rõ",DapCanhMui= "Rõ",Reni="Tai thường nghe được"}
            };

            return thongTinBenhAnNoiKhoaNhi;
        }

        public void LuuHoacCapNhatThongTinBenhAnTreSoSinh(ThongTinBenhAn thongTinBenhAnNoiKhoaNhi)
        {
            var noiTruBenhAn = _noiTruBenhAnRepository.TableNoTracking.Where(cc => cc.Id == thongTinBenhAnNoiKhoaNhi.YeuCauTiepNhanId).FirstOrDefault();
            if (noiTruBenhAn != null)
            {
                var thongTinKhamBenh = JsonConvert.SerializeObject(thongTinBenhAnNoiKhoaNhi);
                noiTruBenhAn.ThongTinBenhAn = thongTinKhamBenh;
                _noiTruBenhAnRepository.Update(noiTruBenhAn);
            }
        }

        #endregion

        #region Lấy thông tin bệnh án nội khoa và khoa nhi

        public ThongTinBenhAn GetThongTinBenhAnNoiKhoaNhi(long yeuCauTiepNhanId)
        {
            var model = new ThongTinBenhAn();
            var noiTruBenhAn = BaseRepository.Table.Where(cc => cc.Id == yeuCauTiepNhanId)
                                                .Select(cc => cc.NoiTruBenhAn)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauTiepNhan).ThenInclude(cc => cc.YeuCauDichVuKyThuats)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauDichVuKyThuats)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(cc => cc.Icdchinh)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhChuanDoans).ThenInclude(c => c.ChuanDoan)
                                                   .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhChanDoanPhanBiets).ThenInclude(c => c.ICD)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                .ThenInclude(cc => cc.YeuCauKhamBenhICDKhacs).ThenInclude(c => c.ICD)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.BenhNhan).ThenInclude(cc => cc.BenhNhanTienSuBenhs)
                                                .FirstOrDefault();
            model.NamSinh = noiTruBenhAn.YeuCauTiepNhan.NamSinh;
            if (noiTruBenhAn != null && noiTruBenhAn.ThongTinBenhAn != null)
            {
                model = JsonConvert.DeserializeObject<ThongTinBenhAn>(noiTruBenhAn.ThongTinBenhAn);
            }
            else
            {
                PrepareThongTinBenhAn(model, noiTruBenhAn);

                var thongTinKhamBenh = JsonConvert.SerializeObject(model);
                noiTruBenhAn.ThongTinBenhAn = thongTinKhamBenh;

                _noiTruBenhAnRepository.Update(noiTruBenhAn);
            }



            return model;
        }
        public void LuuHoacCapNhatThongTinBenhAnNoiKhoaNhi(ThongTinBenhAn thongTinBenhAnNoiKhoaNhi)
        {
            var noiTruBenhAn = _noiTruBenhAnRepository.TableNoTracking.Where(cc => cc.Id == thongTinBenhAnNoiKhoaNhi.YeuCauTiepNhanId).FirstOrDefault();
            if (noiTruBenhAn != null)
            {
                var thongTinKhamBenh = JsonConvert.SerializeObject(thongTinBenhAnNoiKhoaNhi);
                noiTruBenhAn.ThongTinBenhAn = thongTinKhamBenh;
                _noiTruBenhAnRepository.Update(noiTruBenhAn);
            }
        }

        #endregion

        #region Lấy thông tin bệnh án phụ khoa

        public ThongTinBenhAn GetThongTinBenhAnPhuKhoa(long yeuCauTiepNhanId)
        {
            var model = new ThongTinBenhAn();

            var noiTruBenhAn = BaseRepository.Table.Where(cc => cc.Id == yeuCauTiepNhanId)
                                                .Select(cc => cc.NoiTruBenhAn)
                                                .Include(y => y.YeuCauTiepNhan)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauTiepNhan).ThenInclude(cc => cc.YeuCauDichVuKyThuats)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauDichVuKyThuats)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(cc => cc.Icdchinh)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhChuanDoans).ThenInclude(c => c.ChuanDoan)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhChanDoanPhanBiets).ThenInclude(c => c.ICD)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                .ThenInclude(cc => cc.YeuCauKhamBenhICDKhacs).ThenInclude(c => c.ICD)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.BenhNhan).ThenInclude(cc => cc.BenhNhanTienSuBenhs)
                                                .FirstOrDefault();

            if (noiTruBenhAn != null && noiTruBenhAn.ThongTinBenhAn != null)
            {
                model = JsonConvert.DeserializeObject<ThongTinBenhAn>(noiTruBenhAn.ThongTinBenhAn);
            }
            else
            {
                PrepareThongTinBenhAn(model, noiTruBenhAn);
                var rangHamMat = !string.IsNullOrEmpty(model.RangHamMat) ? "\nRăng - Hàm - Mặt: " + model.RangHamMat : ("");
                var tietNieu = !string.IsNullOrEmpty(model.ThanKinh) ? "\nNội tiết-dinh dưỡng: " + model.NoiTiet : ("");
                var sanPhuKhoa = !string.IsNullOrEmpty(model.SanPhuKhoa) ? "\nSản phụ khoa: " + model.SanPhuKhoa : ("");
                var daLieu = !string.IsNullOrEmpty(model.DaLieu) ? "\nDa liễu: " + model.DaLieu : ("");

                model.Khac = rangHamMat + tietNieu + sanPhuKhoa + daLieu;

                var thongTinKhamBenh = JsonConvert.SerializeObject(model);
                noiTruBenhAn.ThongTinBenhAn = thongTinKhamBenh;

                _noiTruBenhAnRepository.Update(noiTruBenhAn);
            }

            model.NamSinh = noiTruBenhAn.YeuCauTiepNhan.NamSinh;
            return model;
        }

        public void LuuHoacCapNhatThongTinBenhAnPhuKhoa(ThongTinBenhAn thongTinBenhAnPhuKhoa)
        {
            var noiTruBenhAn = _noiTruBenhAnRepository.TableNoTracking.Where(cc => cc.Id == thongTinBenhAnPhuKhoa.YeuCauTiepNhanId).FirstOrDefault();
            if (noiTruBenhAn != null)
            {
                var thongTinKhamBenh = JsonConvert.SerializeObject(thongTinBenhAnPhuKhoa);
                noiTruBenhAn.ThongTinBenhAn = thongTinKhamBenh;
                _noiTruBenhAnRepository.Update(noiTruBenhAn);
            }
        }

        #endregion

        #region Lấy thông tin bệnh án ngoại khoa

        public ThongTinBenhAn GetThongTinBenhAnNgoaiKhoa(long yeuCauTiepNhanId)
        {
            var model = new ThongTinBenhAn();

            var noiTruBenhAn = BaseRepository.Table.Where(cc => cc.Id == yeuCauTiepNhanId)
                                                .Select(cc => cc.NoiTruBenhAn)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauTiepNhan).ThenInclude(cc => cc.YeuCauDichVuKyThuats)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauDichVuKyThuats)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(cc => cc.Icdchinh)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhChanDoanPhanBiets).ThenInclude(c => c.ICD)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                .ThenInclude(cc => cc.YeuCauKhamBenhICDKhacs).ThenInclude(c => c.ICD)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhBoPhanTonThuongs)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.BenhNhan).ThenInclude(cc => cc.BenhNhanTienSuBenhs)
                                                .FirstOrDefault();
            model.NamSinh = noiTruBenhAn.YeuCauTiepNhan.NamSinh;

            if (noiTruBenhAn != null && noiTruBenhAn.ThongTinBenhAn != null)
            {
                model = JsonConvert.DeserializeObject<ThongTinBenhAn>(noiTruBenhAn.ThongTinBenhAn);
            }
            else
            {
                PrepareThongTinBenhAn(model, noiTruBenhAn);

                var thongTinKhamBenh = JsonConvert.SerializeObject(model);
                noiTruBenhAn.ThongTinBenhAn = thongTinKhamBenh;

                _noiTruBenhAnRepository.Update(noiTruBenhAn);
            }



            return model;
        }

        public void LuuHoacCapNhatThongTinBenhAnNgoaiKhoa(ThongTinBenhAn thongTinBenhAnNgoaiKhoa)
        {
            var noiTruBenhAn = _noiTruBenhAnRepository.TableNoTracking.Where(cc => cc.Id == thongTinBenhAnNgoaiKhoa.YeuCauTiepNhanId).FirstOrDefault();
            if (noiTruBenhAn != null)
            {
                var thongTinKhamBenh = JsonConvert.SerializeObject(thongTinBenhAnNgoaiKhoa);
                noiTruBenhAn.ThongTinBenhAn = thongTinKhamBenh;
                _noiTruBenhAnRepository.Update(noiTruBenhAn);
            }
        }

        #endregion

        #region Lấy thông tin bệnh án sản khoa

        public ThongTinBenhAn GetThongTinBenhAnSK(long yeuCauTiepNhanId)
        {
            var model = new ThongTinBenhAn();

            var noiTruBenhAn = BaseRepository.Table.Where(cc => cc.Id == yeuCauTiepNhanId)
                                                .Select(cc => cc.NoiTruBenhAn).Include(y => y.YeuCauTiepNhan)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauTiepNhan).ThenInclude(cc => cc.YeuCauDichVuKyThuats)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauDichVuKyThuats)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(cc => cc.Icdchinh)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhChuanDoans).ThenInclude(c => c.ChuanDoan)
                                                   .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhChanDoanPhanBiets).ThenInclude(c => c.ICD)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                .ThenInclude(cc => cc.YeuCauKhamBenhICDKhacs).ThenInclude(c => c.ICD)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.BenhNhan).ThenInclude(cc => cc.BenhNhanTienSuBenhs)
                                                .FirstOrDefault();

            if (noiTruBenhAn != null && !string.IsNullOrEmpty(noiTruBenhAn.ThongTinBenhAn))
            {
                model = JsonConvert.DeserializeObject<ThongTinBenhAn>(noiTruBenhAn.ThongTinBenhAn);
            }
            else
            {
                PrepareThongTinBenhAn(model, noiTruBenhAn);
                model.ToanTrang = model.KhamBenhToanThan;

                var thanKinh = !string.IsNullOrEmpty(model.ThanKinh) ? ("\nThần kinh: " + model.ThanKinh) : ("");
                var coXuongKhop = !string.IsNullOrEmpty(model.CoXuongKhop) ? ("\nCơ-Xương-Khớp: " + model.CoXuongKhop) : ("");
                var rangHamMat = !string.IsNullOrEmpty(model.RangHamMat) ? "\nRăng - Hàm - Mặt: " + model.RangHamMat : ("");
                var tietNieu = !string.IsNullOrEmpty(model.ThanKinh) ? "\nNội tiết-dinh dưỡng: " + model.NoiTiet : ("");
                var sanPhuKhoa = !string.IsNullOrEmpty(model.SanPhuKhoa) ? "\nSản phụ khoa: " + model.SanPhuKhoa : ("");
                var daLieu = !string.IsNullOrEmpty(model.DaLieu) ? "\nDa liễu: " + model.DaLieu : ("");

                model.CacBoPhanKhac = thanKinh + coXuongKhop + rangHamMat + tietNieu + sanPhuKhoa + daLieu;

                var thongTinKhamBenh = JsonConvert.SerializeObject(model);
                noiTruBenhAn.ThongTinBenhAn = thongTinKhamBenh;


                _noiTruBenhAnRepository.Update(noiTruBenhAn);
            }

            model.NamSinh = noiTruBenhAn.YeuCauTiepNhan.NamSinh;
            return model;
        }
        public void LuuHoacCapNhatThongTinBenhAnSK(ThongTinBenhAn thongtin)
        {
            var noiTruBenhAn = _noiTruBenhAnRepository.TableNoTracking.Where(cc => cc.Id == thongtin.YeuCauTiepNhanId).FirstOrDefault();
            if (noiTruBenhAn != null)
            {
                var thongTinKhamBenh = JsonConvert.SerializeObject(thongtin);
                noiTruBenhAn.ThongTinBenhAn = thongTinKhamBenh;
                _noiTruBenhAnRepository.Update(noiTruBenhAn);
            }
        }

        #endregion

        #region Lấy thông tin bệnh án sản khoa mổ thường (tổng kết bệnh án)

        public ThongTinBenhAn GetThongTinBenhAnSanKhoaMoThuong(long yeuCauTiepNhanId)
        {
            var model = new ThongTinBenhAn();

            var noiTruBenhAn = BaseRepository.Table.Where(cc => cc.Id == yeuCauTiepNhanId)
                                                .Select(cc => cc.NoiTruBenhAn).Include(y => y.YeuCauTiepNhan)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauTiepNhan).ThenInclude(cc => cc.YeuCauDichVuKyThuats)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(c => c.YeuCauDichVuKyThuats)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(cc => cc.Icdchinh)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhChuanDoans).ThenInclude(c => c.ChuanDoan)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhChanDoanPhanBiets).ThenInclude(c => c.ICD)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                .ThenInclude(cc => cc.YeuCauKhamBenhICDKhacs).ThenInclude(c => c.ICD)
                                                 .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.BenhNhan).ThenInclude(cc => cc.BenhNhanTienSuBenhs)
                                                .FirstOrDefault();
            model.NamSinh = noiTruBenhAn.YeuCauTiepNhan.NamSinh;
            if (noiTruBenhAn != null && !string.IsNullOrEmpty(noiTruBenhAn.ThongTinBenhAn))
            {
                model = JsonConvert.DeserializeObject<ThongTinBenhAn>(noiTruBenhAn.ThongTinBenhAn);
            }
            else
            {
                PrepareThongTinBenhAn(model, noiTruBenhAn);
                model.ToanTrang = model.KhamBenhToanThan;

                var thanKinh = !string.IsNullOrEmpty(model.ThanKinh) ? ("\nThần kinh: " + model.ThanKinh) : ("");
                var coXuongKhop = !string.IsNullOrEmpty(model.CoXuongKhop) ? ("\nCơ-Xương-Khớp: " + model.CoXuongKhop) : ("");
                var rangHamMat = !string.IsNullOrEmpty(model.RangHamMat) ? "\nRăng - Hàm - Mặt: " + model.RangHamMat : ("");
                var tietNieu = !string.IsNullOrEmpty(model.ThanKinh) ? "\nNội tiết-dinh dưỡng: " + model.NoiTiet : ("");
                var sanPhuKhoa = !string.IsNullOrEmpty(model.SanPhuKhoa) ? "\nSản phụ khoa: " + model.SanPhuKhoa : ("");
                var daLieu = !string.IsNullOrEmpty(model.DaLieu) ? "\nDa liễu: " + model.DaLieu : ("");

                model.CacBoPhanKhac = thanKinh + coXuongKhop + rangHamMat + tietNieu + sanPhuKhoa + daLieu;
                var thongTinKhamBenh = JsonConvert.SerializeObject(model);
                noiTruBenhAn.ThongTinBenhAn = thongTinKhamBenh;

                _noiTruBenhAnRepository.Update(noiTruBenhAn);
            }


            return model;
        }
        public void LuuHoacCapNhatThongTinBenhAnSanKhoaMoThuong(ThongTinBenhAn thongtin)
        {
            var noiTruBenhAn = _noiTruBenhAnRepository.TableNoTracking.Where(cc => cc.Id == thongtin.YeuCauTiepNhanId).FirstOrDefault();
            if (noiTruBenhAn != null)
            {
                var thongTinKhamBenh = JsonConvert.SerializeObject(thongtin);
                noiTruBenhAn.ThongTinTongKetBenhAn = thongTinKhamBenh;
                _noiTruBenhAnRepository.Update(noiTruBenhAn);
            }
        }

        #endregion

        #region Lấy thông tin bệnh án nhi khoa

        public ThongTinBenhAn GetThongTinBenhAnNhiKhoa(long yeuCauTiepNhanId)
        {
            var model = new ThongTinBenhAn();
            var noiTruBenhAn = BaseRepository.TableNoTracking.Where(cc => cc.Id == yeuCauTiepNhanId)
                                                .Select(cc => cc.NoiTruBenhAn)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh).ThenInclude(cc => cc.Icdchinh)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhChuanDoans).ThenInclude(c => c.ChuanDoan)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                                                 .ThenInclude(cc => cc.YeuCauKhamBenhChanDoanPhanBiets).ThenInclude(c => c.ICD)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.YeuCauNhapVien).ThenInclude(c => c.YeuCauKhamBenh)
                                                .ThenInclude(cc => cc.YeuCauKhamBenhICDKhacs).ThenInclude(c => c.ICD)
                                                .Include(cc => cc.YeuCauTiepNhan).ThenInclude(c => c.BenhNhan).ThenInclude(c => c.BenhNhanTienSuBenhs)
                                                .FirstOrDefault();
            model.NamSinh = noiTruBenhAn.YeuCauTiepNhan.NamSinh;
            if (noiTruBenhAn != null && noiTruBenhAn.ThongTinBenhAn != null)
            {
                model = JsonConvert.DeserializeObject<ThongTinBenhAn>(noiTruBenhAn.ThongTinBenhAn);
            }
            else
            {
                PrepareThongTinBenhAn(model, noiTruBenhAn);
                var rangHamMat = model.RangHamMat != null ? "\nRăng - Hàm - Mặt: " + model.RangHamMat : "";
                model.TaiMuiHong += rangHamMat;

                var thongTinKhamBenh = JsonConvert.SerializeObject(model);
                noiTruBenhAn.ThongTinBenhAn = thongTinKhamBenh;

                _noiTruBenhAnRepository.Update(noiTruBenhAn);
            }

            return model;
        }

        public void LuuHoacCapNhatThongTinBenhAnNhiKhoa(ThongTinBenhAn model)
        {
            var noiTruBenhAn = _noiTruBenhAnRepository.TableNoTracking.Where(cc => cc.Id == model.YeuCauTiepNhanId).FirstOrDefault();
            if (noiTruBenhAn != null)
            {
                var thongTinBenhAnKhoaNhi = JsonConvert.SerializeObject(model);
                noiTruBenhAn.ThongTinBenhAn = thongTinBenhAnKhoaNhi;

                _noiTruBenhAnRepository.Update(noiTruBenhAn);
            }
        }

        #endregion       

        public void PrepareThongTinBenhAn(ThongTinBenhAn model, Camino.Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn noiTruBenhAn)
        {
            if (noiTruBenhAn.YeuCauTiepNhan != null)
            {
                var yeuCauKhamBenh = noiTruBenhAn.YeuCauTiepNhan.YeuCauNhapVien?.YeuCauKhamBenh;
                model.LyDoVaoVien = yeuCauKhamBenh.LyDoNhapVien;
                model.QuaTrinhHoiBenh = yeuCauKhamBenh.BenhSu;
                model.KhamBenhToanThan = yeuCauKhamBenh.KhamToanThan;

                if (noiTruBenhAn.YeuCauTiepNhan.BenhNhan.BenhNhanTienSuBenhs.Count > 0)
                {
                    var tienSuBanThanBenhNhans = noiTruBenhAn.YeuCauTiepNhan.BenhNhan
                                                         .BenhNhanTienSuBenhs.Where(c => c.LoaiTienSuBenh == EnumLoaiTienSuBenh.BanThan)
                                                         .Select(c => c.TenBenh).ToList();
                    var tienSuGiaDinhBenhNhans = noiTruBenhAn.YeuCauTiepNhan.BenhNhan
                                                               .BenhNhanTienSuBenhs.Where(c => c.LoaiTienSuBenh == EnumLoaiTienSuBenh.GiaDinh)
                                                               .Select(c => c.TenBenh).ToList();
                    if (tienSuBanThanBenhNhans.Count > 0)
                    {
                        foreach (var item in tienSuBanThanBenhNhans)
                        {
                            model.TienSuBenhBanThan += item + "\n";
                        }

                    }


                    if (tienSuGiaDinhBenhNhans.Count > 0)
                    {
                        foreach (var item in tienSuGiaDinhBenhNhans)
                        {
                            model.TienSuBenhGiaDinh += item + "\n";
                        }

                    }
                }
                var yeuCauTiepNhanId = noiTruBenhAn.YeuCauTiepNhan.YeuCauNhapVien.YeuCauKhamBenh.YeuCauTiepNhanId;
                var chiSoSinhTons = _ketQuaSinhHieuRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId).ToList();
                if (chiSoSinhTons.Any())
                {

                    foreach (var item in chiSoSinhTons)
                    {
                        var chiSoSinhTon = new ChiSoSinhTon();
                        chiSoSinhTon.NhipTho = item.NhipTho;
                        chiSoSinhTon.NhipTim = item.NhipTim;
                        chiSoSinhTon.SpO2 = item.SpO2;
                        chiSoSinhTon.ThanNhiet = item.ThanNhiet;
                        chiSoSinhTon.CanNang = item.CanNang;
                        chiSoSinhTon.ChieuCao = item.ChieuCao;
                        chiSoSinhTon.BMI = item.Bmi;
                        chiSoSinhTon.Glassgow = item.Glassgow;
                        chiSoSinhTon.HuyetApTamThu = item.HuyetApTamThu;
                        chiSoSinhTon.HuyetApTamTruong = item.HuyetApTamTruong;
                        chiSoSinhTon.NgayThucHien = item.CreatedOn?.ApplyFormatDateTimeSACH();
                        model.ChiSoSinhTons.Add(chiSoSinhTon);
                    }
                }

                var yeuCauDichVuKyThuats = BaseRepository.TableNoTracking.Where(cc => cc.Id == yeuCauTiepNhanId).Include(c => c.YeuCauDichVuKyThuats);
                var tenDVXetNghiems = yeuCauDichVuKyThuats.SelectMany(c => c.YeuCauDichVuKyThuats).Where(c => c.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem).Select(c => c.TenDichVu);
                model.CacXetNghiemCanLam = tenDVXetNghiems != null && tenDVXetNghiems.Any() ? string.Join(",", tenDVXetNghiems) : "";

                var thongTinKhamTheoDichVuData = yeuCauKhamBenh != null && yeuCauKhamBenh.ThongTinKhamTheoDichVuData != null ?
                                JsonConvert.DeserializeObject<ThongTinBenhNhanKhamKhacList>(yeuCauKhamBenh.ThongTinKhamTheoDichVuData) : null;

                if (thongTinKhamTheoDichVuData != null)
                {
                    foreach (var item in thongTinKhamTheoDichVuData.DataKhamTheoTemplate)
                    {
                        switch (item.Id)
                        {
                            case "TuanHoan":
                                model.TuanHoan = item.Value;
                                break;
                            case "TieuHoa":
                                model.TieuHoa = item.Value;
                                break;
                            case "ThanKinh":
                                model.ThanKinh = item.Value;
                                break;
                            case "HoHap":
                                model.HoHap = item.Value;
                                break;
                            case "ThanTietNieuSinhDuc":
                                model.ThanTietNieu = item.Value;
                                break;
                            case "RangHamMat":
                                model.RangHamMat = item.Value;
                                break;
                            case "CoXuongKhop":
                                model.CoXuongKhop = item.Value;
                                break;
                            case "NoiTietDinhDuong":
                                model.NoiTiet = item.Value;
                                break;
                            case "SanPhuKhoa":
                                model.SanPhuKhoa = item.Value;
                                break;
                            case "DaLieu":
                                model.DaLieu = item.Value;
                                break;

                            //Mat
                            case "ThiLucKhongKinhMatPhai":
                                model.Mat = "THỊ LỰC KHÔNG KÍNH:\nMắt phải :" + item.Value;
                                break;
                            case "ThiLucKhongKinhMatTrai":
                                model.Mat += "\nMắt trái :" + item.Value;
                                break;
                            case "NhanApMatPhai":
                                model.Mat += "NHÃN ÁP:\nMắt phải:\nMắt phải: " + item.Value;
                                break;
                            case "NhanApMatTrai":
                                model.Mat += "\nMắt trái: " + item.Value;
                                break;
                            case "ThiLucCoKinhPhai":
                                model.Mat += "THỊ LỰC CÓ KÍNH:\nMắt phải: " + item.Value;
                                break;
                            case "ThiLucCoKinhTrai":
                                model.Mat += "\nMắt trái: " + item.Value;
                                break;
                            case "SoiHienVi":
                                model.Mat += "\nSinh hiển vi: " + item.Value;
                                break;
                            case "SoiDayMat":
                                model.Mat += "\nSoi đáy mắt: " + item.Value;
                                break;
                            //Tai  mui hong
                            case "Tai":
                                model.TaiMuiHong += "Tai: " + item.Value;
                                break;
                            case "Mui":
                                model.TaiMuiHong += "\nMũi: " + item.Value;
                                break;
                            case "Hong":
                                model.TaiMuiHong += "\nHọng: " + item.Value;
                                break;
                            default:
                                break;
                        }
                    }
                }


                if (yeuCauKhamBenh != null)
                {
                    model.ICDChinh = yeuCauKhamBenh.Icdchinh?.Id;
                    model.TenICDChinh = yeuCauKhamBenh.Icdchinh.Ma + " - " + yeuCauKhamBenh.Icdchinh?.TenTiengViet;
                    model.ChuanDoan = yeuCauKhamBenh.GhiChuICDChinh;

                    if (yeuCauKhamBenh.YeuCauKhamBenhChanDoanPhanBiets.Any())
                    {
                        //chuẩn đoán kèm theo YeuCauKhamBenhChuanDoans
                        model.ChuanDoanPhanBiets = yeuCauKhamBenh.YeuCauKhamBenhChanDoanPhanBiets.Select(c => new ThongTinChuanDoanKemTheo
                        {
                            ICD = c.ICD.Id,
                            TenICD = c.ICD.Ma + " - " + c.ICD.TenTiengViet,
                            ChuanDoan = c.GhiChu,
                        }).ToList();
                    }
                    if (yeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Any())
                    {
                        //chuẩn đoán khác YeuCauKhamBenhICDKhacs
                        model.ChuanDoanKemTheos = yeuCauKhamBenh.YeuCauKhamBenhICDKhacs.Select(c => new ThongTinChuanDoanKemTheo
                        {
                            ICD = c.ICD.Id,
                            TenICD = c.ICD.Ma + " - " + c.ICD.TenTiengViet,
                            ChuanDoan = c.GhiChu,
                        }).ToList();
                    }
                }
            }
        }

        public string InPhieuKhamBenhNoiTru(long noiTruBenhAnId)
        {
            var content = string.Empty;
            var thongTinBenhNhan = _danhSachChoKhamService.ThongTinBenhNhanHienTai(noiTruBenhAnId);
            var noiTruBenhAn = _noiTruBenhAnRepository.GetById(noiTruBenhAnId, s => s
                                .Include(p => p.KhoaPhongNhapVien)
                                .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.BenhNhan).ThenInclude(p => p.BenhNhanDiUngThuocs)
                                .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.YeuCauNhapVien).ThenInclude(p => p.YeuCauKhamBenh)
                                );
            var template = _templateRepository.TableNoTracking.Where(x => x.Name.Equals("PhieuKhamBenh2")).First();
            var phongDangNhapHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var tenPhongKham = _phongBenhVienRepository.TableNoTracking.Where(p => p.Id == phongDangNhapHienTaiId).Select(p => p.Ten).FirstOrDefault();
            var thongTinBenhAn = new ThongTinBenhAn();
            var chanDoanKemTheo = string.Empty;
            if (!string.IsNullOrEmpty(noiTruBenhAn.ThongTinBenhAn))
            {
                thongTinBenhAn = JsonConvert.DeserializeObject<ThongTinBenhAn>(noiTruBenhAn.ThongTinBenhAn);
                if (thongTinBenhAn.ChuanDoanKemTheos != null && thongTinBenhAn.ChuanDoanKemTheos.Any())
                {
                    chanDoanKemTheo = string.Join("; ", thongTinBenhAn.ChuanDoanKemTheos.Select(p => p.TenICD));
                }
            }

            var tienSuDiUngThuoc = string.Empty;
            var tienSuDiUngThucAn = string.Empty;
            var tienSuDiUngKhac = string.Empty;
            var tienSuBenhBanThan = thongTinBenhAn.TienSuBenhBanThan;
            var tienSuBenhGiaDinh = thongTinBenhAn.TienSuBenhGiaDinh;

            var dacDiemLienQuanBenh = string.Empty;

            foreach (var item in noiTruBenhAn.YeuCauTiepNhan.BenhNhan.BenhNhanDiUngThuocs)
            {
                if (item.LoaiDiUng == Enums.LoaiDiUng.Thuoc)
                {
                    if (!string.IsNullOrEmpty(item.TenDiUng))
                    {
                        tienSuDiUngThuoc += item.TenDiUng + "; ";
                    }
                }
                else if (item.LoaiDiUng == Enums.LoaiDiUng.ThucAn)
                {
                    if (!string.IsNullOrEmpty(item.TenDiUng))
                    {
                        tienSuDiUngThucAn += item.TenDiUng + "; ";
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(item.TenDiUng))
                    {
                        tienSuDiUngKhac += item.TenDiUng + "; ";
                    }
                }
            }
            var ketQuaSinhHieu = new ChiSoSinhTon();
            if (thongTinBenhAn.ChiSoSinhTons != null && thongTinBenhAn.ChiSoSinhTons.Any())
            {
                ketQuaSinhHieu = thongTinBenhAn.ChiSoSinhTons.OrderByDescending(kq => kq.Id).Where(p => p.CanNang != null).FirstOrDefault();
            }
            var tenYCDuocPhamBV = string.Empty;

            var yeuCauDuocPhamBenhViens = noiTruBenhAn.YeuCauTiepNhan.YeuCauDuocPhamBenhViens.Where(p => p.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien).ToList();
            foreach (var item in yeuCauDuocPhamBenhViens)
            {
                tenYCDuocPhamBV += item.Ten + "; ";
            }
            var yeuCauVatTuBenhViens = noiTruBenhAn.YeuCauTiepNhan.YeuCauVatTuBenhViens.Where(p => p.TrangThai == Enums.EnumYeuCauVatTuBenhVien.DaThucHien).ToList();
            foreach (var item in yeuCauVatTuBenhViens)
            {
                tenYCDuocPhamBV += item.Ten + "; ";
            }
            if (noiTruBenhAn.LoaiBenhAn == LoaiBenhAn.NoiKhoa)
            {
                if (thongTinBenhAn.ThoiGianDiUng != 0 && thongTinBenhAn.ThoiGianDiUng != null)
                {
                    dacDiemLienQuanBenh += thongTinBenhAn.KyHieuDiUng + "(thời gian theo tháng): " + thongTinBenhAn.ThoiGianDiUng + "; ";
                }
                if (thongTinBenhAn.ThoiGianMaTuy != 0 && thongTinBenhAn.ThoiGianMaTuy != null)
                {
                    dacDiemLienQuanBenh += thongTinBenhAn.KyHieuMaTuy + "(thời gian theo tháng): " + thongTinBenhAn.ThoiGianMaTuy + "; ";
                }
                if (thongTinBenhAn.ThoiGianRuouBia != 0 && thongTinBenhAn.ThoiGianRuouBia != null)
                {
                    dacDiemLienQuanBenh += thongTinBenhAn.KyHieuRuouBia + "(thời gian theo tháng): " + thongTinBenhAn.ThoiGianRuouBia + "; ";
                }
                if (thongTinBenhAn.ThoiGianThuocLa != 0 && thongTinBenhAn.ThoiGianThuocLa != null)
                {
                    dacDiemLienQuanBenh += thongTinBenhAn.KyHieuThuocLa + "(thời gian theo tháng): " + thongTinBenhAn.ThoiGianThuocLa + "; ";
                }
                if (thongTinBenhAn.ThoiGianThuocLao != 0 && thongTinBenhAn.ThoiGianThuocLao != null)
                {
                    dacDiemLienQuanBenh += thongTinBenhAn.KyHieuThuocLao + "(thời gian theo tháng): " + thongTinBenhAn.ThoiGianThuocLao + "; ";
                }
                if (thongTinBenhAn.ThoiGianKhac != 0 && thongTinBenhAn.ThoiGianKhac != null)
                {
                    dacDiemLienQuanBenh += thongTinBenhAn.KyHieuKhac + "(thời gian theo tháng): " + thongTinBenhAn.ThoiGianKhac + "; ";
                }
                var tienSuBenh = string.Empty;
                var khamBenh = string.Empty;
                if (!string.IsNullOrEmpty(thongTinBenhAn.KhamBenhToanThan))
                {
                    khamBenh += "Toàn thân: " + thongTinBenhAn.KhamBenhToanThan + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.TuanHoan))
                {
                    khamBenh += "- Tuần hoàn: " + thongTinBenhAn.TuanHoan + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.HoHap))
                {
                    khamBenh += "- Hô hấp: " + thongTinBenhAn.HoHap + ";<br>";

                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.TieuHoa))
                {
                    khamBenh += "- Tiêu hóa: " + thongTinBenhAn.TieuHoa + ";<br>";

                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.ThanTietNieu))
                {
                    khamBenh += "- Thận-Tiết niệu-Sinh dục: " + thongTinBenhAn.ThanTietNieu + ";<br>";

                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.ThanKinh))
                {
                    khamBenh += "- Thần kinh: " + thongTinBenhAn.ThanKinh + ";<br>";

                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.CoXuongKhop))
                {
                    khamBenh += "- Cơ-Xương-Khớp: " + thongTinBenhAn.CoXuongKhop + ";<br>";

                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.TaiMuiHong))
                {
                    khamBenh += "- Tai-mũi-họng: " + thongTinBenhAn.TaiMuiHong + ";<br>";

                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.RangHamMat))
                {
                    khamBenh += "- Răng-hàm-mặt: " + thongTinBenhAn.RangHamMat + ";<br>";

                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.Mat))
                {
                    khamBenh += "- Mắt: " + thongTinBenhAn.Mat + ";<br>";

                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.NoiTiet))
                {
                    khamBenh += "- Nội tiết, dinh dưỡng và các bệnh lý khác: " + thongTinBenhAn.NoiTiet + ";<br>";

                }
                var SOBHYT = "";
                if (thongTinBenhNhan?.BHYTMaSoThe != null)
                {
                    var stringPlit = thongTinBenhNhan?.BHYTMaSoThe.Split("-");
                    SOBHYT = stringPlit[0];
                }

                var data = new ThongTinBenhKhac
                {
                    PhongKham = tenPhongKham,
                    So = noiTruBenhAn.SoBenhAn,
                    HoTenBenhNhan = thongTinBenhNhan.HoTenBenhNhan,
                    SinhNgay = thongTinBenhNhan.SinhNgay,
                    GioiTinh = thongTinBenhNhan.GioiTinh,
                    NgheNghiep = thongTinBenhNhan.NgheNghiep,
                    DanToc = thongTinBenhNhan.DanToc,
                    QuocTich = thongTinBenhNhan.QuocTich,
                    DiaChi = thongTinBenhNhan.SoNha,
                    SoNha = thongTinBenhNhan.SoNha,
                    XaPhuong = thongTinBenhNhan.XaPhuong,
                    Huyen = thongTinBenhNhan.Huyen,
                    TinhThanhPho = thongTinBenhNhan.TinhThanhPho,
                    NoiLamViec = thongTinBenhNhan.NoiLamViec,
                    DoiTuong = thongTinBenhNhan.DoiTuong,
                    BHYTNgayHetHan = thongTinBenhNhan.BHYTNgayHetHan,
                    BHYTMaSoThe = SOBHYT,
                    ChanDoanNoiGioiThieu = noiTruBenhAn?.YeuCauTiepNhan?.YeuCauNhapVien?.YeuCauKhamBenh?.ChanDoanCuaNoiGioiThieu,
                    NguoiLienHeQuanHeThanNhan = thongTinBenhNhan.NguoiLienHeQuanHeThanNhan,
                    NguoiLienHeQuanHeSoDienThoai = thongTinBenhNhan.NguoiLienHeQuanSoDienThoai,
                    SoDienThoai = thongTinBenhNhan.NguoiLienHeQuanSoDienThoai,
                    GioKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Hour.ConvertHourToString(),
                    NgayKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Day.ConvertDateToString(),
                    ThangKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Month.ConvertMonthToString(),
                    NamKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Year.ConvertYearToString(),
                    TienSuBenh = (!string.IsNullOrEmpty(tienSuBenhBanThan) ? "Bản thân: " + tienSuBenhBanThan : "")
                            + (!string.IsNullOrEmpty(tienSuBenhGiaDinh) ? "<br>Gia đình: " + tienSuBenhGiaDinh : "") +
                            (!string.IsNullOrEmpty(dacDiemLienQuanBenh) ? "<br> Đặc điểm liên quan bệnh: " + dacDiemLienQuanBenh : ""),
                    TienSuDiUng = string.IsNullOrEmpty(tienSuDiUngThuoc) && string.IsNullOrEmpty(tienSuDiUngThucAn) && string.IsNullOrEmpty(tienSuDiUngKhac)
                                        ? "Chưa phát hiện" : ((!string.IsNullOrEmpty(tienSuDiUngThuoc) ? "Thuốc: " + tienSuDiUngThuoc : "")
                                        + (!string.IsNullOrEmpty(tienSuDiUngThucAn) ? "Thức ăn: " + tienSuDiUngThucAn : "")
                                        + (!string.IsNullOrEmpty(tienSuDiUngKhac) ? "Khác: " + tienSuDiUngKhac : "")),
                    LyDoVaoKham = thongTinBenhAn.LyDoVaoVien,
                    LyDoVaoVien = thongTinBenhAn.LyDoVaoVien,
                    ThoiDiemTiepNhan = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatDate(),
                    BenhSu = thongTinBenhAn.QuaTrinhHoiBenh,
                    Mach = ketQuaSinhHieu?.NhipTim?.ApplyNumber(),
                    NhietDo = ketQuaSinhHieu?.ThanNhiet?.ApplyNumber(),
                    HuyetAp = (ketQuaSinhHieu?.HuyetApTamThu != null && ketQuaSinhHieu?.HuyetApTamTruong != null) ? (ketQuaSinhHieu?.HuyetApTamThu.ToString() + "/" + ketQuaSinhHieu?.HuyetApTamTruong.ToString()) : null,
                    NhipTho = ketQuaSinhHieu?.NhipTho == null ? null : ketQuaSinhHieu?.NhipTho.ToString(),
                    CanNang = ketQuaSinhHieu?.CanNang == null ? null : ketQuaSinhHieu?.CanNang.ToString(),
                    ChieuCao = ketQuaSinhHieu?.ChieuCao == null ? null : ketQuaSinhHieu?.ChieuCao.ToString(),
                    SpO2 = ketQuaSinhHieu?.SpO2 == null ? null : ((double?)Math.Round((ketQuaSinhHieu.SpO2.Value), 2)).ToString(),
                    BMI = ketQuaSinhHieu?.BMI == null ? null : ((double?)Math.Round((ketQuaSinhHieu.BMI.Value), 2)).ToString(),
                    KhamBenh = khamBenh,
                    KetQuaCanLamSang = thongTinBenhAn.TomTatBenhAn,
                    DaXuLi = tenYCDuocPhamBV,
                    HuongXuLy = thongTinBenhAn.HuongXuLyLoiDanBs,
                    XetNghiemDaCoVaLamMoi = thongTinBenhAn.CacXetNghiemCanLam,
                    ChanDoan = (!string.IsNullOrEmpty(thongTinBenhAn.ChuanDoan) ? thongTinBenhAn.TenICDChinh : "") + (!string.IsNullOrEmpty(chanDoanKemTheo) ? "<br>- Chẩn đoán kèm theo: " + chanDoanKemTheo : ""),
                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString()

                };

                //cập nhật data muốn xuống hàng theo y khách hàng
                var dataJson = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                var replaceData = dataJson.Replace("\\n", "<br/>");
                var newData = JsonConvert.DeserializeObject<ThongTinBenhKhac>(replaceData);
                //cập nhật data muốn xuống hàng theo y khách hàng

                content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, newData);
            }
            else if (noiTruBenhAn.LoaiBenhAn == LoaiBenhAn.NhiKhoa)
            {
                var khamBenh = string.Empty;
                if ((thongTinBenhAn.ChieuCao != 0 && thongTinBenhAn.ChieuCao != null) || (thongTinBenhAn.VongNguc != 0 && thongTinBenhAn.VongNguc != null)
                  || (thongTinBenhAn.VongDau != 0 && thongTinBenhAn.VongDau != null)
                  || !string.IsNullOrEmpty(thongTinBenhAn.KhamBenhToanThan))
                {
                    khamBenh += "1. Toàn thân<br>";
                }
                if (thongTinBenhAn.ChieuCao != 0 && thongTinBenhAn.ChieuCao != null)
                {
                    khamBenh += "Chiều cao(cm): " + thongTinBenhAn.ChieuCao?.ApplyNumber() + "; ";
                }
                if (thongTinBenhAn.VongNguc != 0 && thongTinBenhAn.VongNguc != null)
                {
                    khamBenh += "Vòng ngực(cm): " + thongTinBenhAn.VongNguc?.ApplyNumber() + "; ";
                }
                if (thongTinBenhAn.VongDau != 0 && thongTinBenhAn.VongDau != null)
                {
                    khamBenh += "Vòng đầu(cm): " + thongTinBenhAn.VongDau?.ApplyNumber() + "; ";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.KhamBenhToanThan))
                {
                    khamBenh += "- Toàn thân: " + thongTinBenhAn.KhamBenhToanThan + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.TuanHoan) || !string.IsNullOrEmpty(thongTinBenhAn.HoHap)
                   || !string.IsNullOrEmpty(thongTinBenhAn.TieuHoa) || !string.IsNullOrEmpty(thongTinBenhAn.ThanTietNieu)
                   || !string.IsNullOrEmpty(thongTinBenhAn.ThanKinh) || !string.IsNullOrEmpty(thongTinBenhAn.CoXuongKhop)
                   || !string.IsNullOrEmpty(thongTinBenhAn.TaiMuiHong) || !string.IsNullOrEmpty(thongTinBenhAn.RangHamMat)
                   )
                {
                    khamBenh += "2. Các cơ quan<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.TuanHoan))
                {
                    khamBenh += "- Tuần hoàn: " + thongTinBenhAn.TuanHoan + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.HoHap))
                {
                    khamBenh += "- Hô hấp: " + thongTinBenhAn.HoHap + ";<br>";

                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.TieuHoa))
                {
                    khamBenh += "- Tiêu hóa: " + thongTinBenhAn.TieuHoa + ";<br>";

                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.ThanTietNieu))
                {
                    khamBenh += "- Thận-Tiết niệu-Sinh dục: " + thongTinBenhAn.ThanTietNieu + ";<br>";

                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.ThanKinh))
                {
                    khamBenh += "- Thần kinh: " + thongTinBenhAn.ThanKinh + ";<br>";

                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.CoXuongKhop))
                {
                    khamBenh += "- Cơ-Xương-Khớp: " + thongTinBenhAn.CoXuongKhop + ";<br>";

                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.TaiMuiHong))
                {
                    khamBenh += "- Tai-mũi-họng, Răng-hàm-mặt: " + thongTinBenhAn.TaiMuiHong + ";<br>";

                }
                var SOBHYT = "";
                if (thongTinBenhNhan?.BHYTMaSoThe != null)
                {
                    var stringPlit = thongTinBenhNhan?.BHYTMaSoThe.Split("-");
                    SOBHYT = stringPlit[0];
                }
                var data = new ThongTinBenhKhac
                {
                    PhongKham = tenPhongKham,
                    So = noiTruBenhAn.SoBenhAn,
                    HoTenBenhNhan = thongTinBenhNhan.HoTenBenhNhan,
                    SinhNgay = thongTinBenhNhan.SinhNgay,
                    GioiTinh = thongTinBenhNhan.GioiTinh,
                    NgheNghiep = thongTinBenhNhan.NgheNghiep,
                    DanToc = thongTinBenhNhan.DanToc,
                    QuocTich = thongTinBenhNhan.QuocTich,
                    DiaChi = thongTinBenhNhan.SoNha,
                    SoNha = thongTinBenhNhan.SoNha,
                    XaPhuong = thongTinBenhNhan.XaPhuong,
                    Huyen = thongTinBenhNhan.Huyen,
                    TinhThanhPho = thongTinBenhNhan.TinhThanhPho,
                    NoiLamViec = thongTinBenhNhan.NoiLamViec,
                    DoiTuong = thongTinBenhNhan.DoiTuong,
                    BHYTNgayHetHan = thongTinBenhNhan.BHYTNgayHetHan,
                    BHYTMaSoThe = SOBHYT,
                    ChanDoanNoiGioiThieu = noiTruBenhAn?.YeuCauTiepNhan?.YeuCauNhapVien?.YeuCauKhamBenh?.ChanDoanCuaNoiGioiThieu,
                    NguoiLienHeQuanHeThanNhan = thongTinBenhNhan.NguoiLienHeQuanHeThanNhan,
                    NguoiLienHeQuanHeSoDienThoai = thongTinBenhNhan.NguoiLienHeQuanSoDienThoai,
                    SoDienThoai = thongTinBenhNhan.NguoiLienHeQuanSoDienThoai,
                    GioKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Hour.ConvertHourToString(),
                    NgayKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Day.ConvertDateToString(),
                    ThangKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Month.ConvertMonthToString(),
                    NamKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Year.ConvertYearToString(),
                    ThoiDiemTiepNhan = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatDate(),
                    TienSuBenh = (!string.IsNullOrEmpty(tienSuBenhBanThan) ? "Bản thân: " + tienSuBenhBanThan : "")
                            + (!string.IsNullOrEmpty(tienSuBenhGiaDinh) ? "<br>Gia đình: " + tienSuBenhGiaDinh : ""),
                    TienSuDiUng = string.IsNullOrEmpty(tienSuDiUngThuoc) && string.IsNullOrEmpty(tienSuDiUngThucAn) && string.IsNullOrEmpty(tienSuDiUngKhac)
                                        ? "Chưa phát hiện" : ((!string.IsNullOrEmpty(tienSuDiUngThuoc) ? "Thuốc: " + tienSuDiUngThuoc : "")
                                        + (!string.IsNullOrEmpty(tienSuDiUngThucAn) ? "Thức ăn: " + tienSuDiUngThucAn : "")
                                        + (!string.IsNullOrEmpty(tienSuDiUngKhac) ? "Khác: " + tienSuDiUngKhac : "")),
                    LyDoVaoVien = thongTinBenhAn.LyDoVaoVien,
                    LyDoVaoKham = thongTinBenhAn.LyDoVaoVien,
                    BenhSu = thongTinBenhAn.QuaTrinhHoiBenh,
                    Mach = ketQuaSinhHieu?.NhipTim?.ApplyNumber(),
                    NhietDo = ketQuaSinhHieu?.ThanNhiet?.ApplyNumber(),
                    HuyetAp = (ketQuaSinhHieu?.HuyetApTamThu != null && ketQuaSinhHieu?.HuyetApTamTruong != null) ? (ketQuaSinhHieu?.HuyetApTamThu.ToString() + "/" + ketQuaSinhHieu?.HuyetApTamTruong.ToString()) : null,
                    NhipTho = ketQuaSinhHieu?.NhipTho == null ? null : ketQuaSinhHieu?.NhipTho.ToString(),
                    CanNang = ketQuaSinhHieu?.CanNang == null ? null : ketQuaSinhHieu?.CanNang.ToString(),
                    ChieuCao = ketQuaSinhHieu?.ChieuCao == null ? null : ketQuaSinhHieu?.ChieuCao.ToString(),
                    SpO2 = ketQuaSinhHieu?.SpO2 == null ? null : ((double?)Math.Round((ketQuaSinhHieu.SpO2.Value), 2)).ToString(),
                    BMI = ketQuaSinhHieu?.BMI == null ? null : ((double?)Math.Round((ketQuaSinhHieu.BMI.Value), 2)).ToString(),
                    KhamBenh = khamBenh,
                    DaXuLi = tenYCDuocPhamBV,
                    ChanDoan = (!string.IsNullOrEmpty(thongTinBenhAn.ChuanDoan) ? thongTinBenhAn.ChuanDoan : "") + (!string.IsNullOrEmpty(chanDoanKemTheo) ? "<br>- Chẩn đoán kèm theo: " + chanDoanKemTheo : ""),
                    KetQuaCanLamSang = thongTinBenhAn.TomTatBenhAn,
                    HuongXuLy = thongTinBenhAn.HuongXuLyLoiDanBs,
                    XetNghiemDaCoVaLamMoi = thongTinBenhAn.CacXetNghiemCanLam,
                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString()
                };

                //cập nhật data muốn xuống hàng theo y khách hàng
                var dataJson = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                var replaceData = dataJson.Replace("\\n", "<br/>");
                var newData = JsonConvert.DeserializeObject<ThongTinBenhKhac>(replaceData);
                //cập nhật data muốn xuống hàng theo y khách hàng

                content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, newData);
            }
            else if (noiTruBenhAn.LoaiBenhAn == LoaiBenhAn.PhuKhoa)
            {
                var khamBenh = string.Empty;
                if (!string.IsNullOrEmpty(thongTinBenhAn.DaNienMac) || !string.IsNullOrEmpty(thongTinBenhAn.Hach) || !string.IsNullOrEmpty(thongTinBenhAn.Vu))
                {
                    khamBenh += "1. Toàn thân <br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.DaNienMac))
                {
                    khamBenh += "- Da niêm mạc: " + thongTinBenhAn.DaNienMac + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.Hach))
                {
                    khamBenh += "- Hạch: " + thongTinBenhAn.Hach + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.Vu))
                {
                    khamBenh += "- Vú: " + thongTinBenhAn.Vu + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.TuanHoan) || !string.IsNullOrEmpty(thongTinBenhAn.HoHap)
                    || !string.IsNullOrEmpty(thongTinBenhAn.TieuHoa) || !string.IsNullOrEmpty(thongTinBenhAn.ThanKinh)
                    || !string.IsNullOrEmpty(thongTinBenhAn.CoXuongKhop) || !string.IsNullOrEmpty(thongTinBenhAn.ThanTietNieu)
                    || !string.IsNullOrEmpty(thongTinBenhAn.Khac))
                {
                    khamBenh += "2. Các cơ quan <br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.TuanHoan))
                {
                    khamBenh += "- Tuần hoàn: " + thongTinBenhAn.TuanHoan + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.HoHap))
                {
                    khamBenh += "- Hô hấp: " + thongTinBenhAn.HoHap + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.TieuHoa))
                {
                    khamBenh += "- Tiêu hóa: " + thongTinBenhAn.TieuHoa + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.ThanTietNieu))
                {
                    khamBenh += "- Thận-Tiết niệu: " + thongTinBenhAn.ThanTietNieu + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.ThanKinh))
                {
                    khamBenh += "- Thần kinh: " + thongTinBenhAn.ThanKinh + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.CoXuongKhop))
                {
                    khamBenh += "- Cơ-Xương-Khớp: " + thongTinBenhAn.CoXuongKhop + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.Khac))
                {
                    khamBenh += "- Khác: " + thongTinBenhAn.Khac + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.CacDauHieuSinhDucThuPhat) || !string.IsNullOrEmpty(thongTinBenhAn.MoiLon)
                  || !string.IsNullOrEmpty(thongTinBenhAn.MoiBe) || !string.IsNullOrEmpty(thongTinBenhAn.AmVat)
                  || !string.IsNullOrEmpty(thongTinBenhAn.AmHo) || !string.IsNullOrEmpty(thongTinBenhAn.MangTrinh)
                  || !string.IsNullOrEmpty(thongTinBenhAn.TangSinhMon) || !string.IsNullOrEmpty(thongTinBenhAn.AmDao)
                  || !string.IsNullOrEmpty(thongTinBenhAn.CoTuCung) || !string.IsNullOrEmpty(thongTinBenhAn.ThanTuCung)
                  || !string.IsNullOrEmpty(thongTinBenhAn.PhanPhu) || !string.IsNullOrEmpty(thongTinBenhAn.CacTuiCung))
                {
                    khamBenh += "3. Khám chuyên khoa <br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.CacDauHieuSinhDucThuPhat) || !string.IsNullOrEmpty(thongTinBenhAn.MoiLon)
                 || !string.IsNullOrEmpty(thongTinBenhAn.MoiBe) || !string.IsNullOrEmpty(thongTinBenhAn.AmVat)
                 || !string.IsNullOrEmpty(thongTinBenhAn.AmHo) || !string.IsNullOrEmpty(thongTinBenhAn.MangTrinh)
                 || !string.IsNullOrEmpty(thongTinBenhAn.TangSinhMon))
                {
                    khamBenh += "a. Khám ngoài<br>";
                }

                if (!string.IsNullOrEmpty(thongTinBenhAn.CacDauHieuSinhDucThuPhat))
                {
                    khamBenh += "- Các dấu hiện sinh dục thứ phát: " + thongTinBenhAn.CacDauHieuSinhDucThuPhat + ";<br> ";
                }

                if (!string.IsNullOrEmpty(thongTinBenhAn.MoiLon))
                {
                    khamBenh += "- Môi lớn: " + thongTinBenhAn.MoiLon + ";<br>";
                }

                if (!string.IsNullOrEmpty(thongTinBenhAn.MoiBe))
                {
                    khamBenh += "- Môi bé: " + thongTinBenhAn.MoiBe + ";<br> ";
                }

                if (!string.IsNullOrEmpty(thongTinBenhAn.AmVat))
                {
                    khamBenh += "- Âm vật: " + thongTinBenhAn.AmVat + ";<br>";
                }

                if (!string.IsNullOrEmpty(thongTinBenhAn.AmHo))
                {
                    khamBenh += "- Âm hộ: " + thongTinBenhAn.AmHo + ";<br> ";
                }

                if (!string.IsNullOrEmpty(thongTinBenhAn.MangTrinh))
                {
                    khamBenh += "- Màng trinh: " + thongTinBenhAn.MangTrinh + ";<br>";
                }

                if (!string.IsNullOrEmpty(thongTinBenhAn.TangSinhMon))
                {
                    khamBenh += "- Tầng sinh môn: " + thongTinBenhAn.TangSinhMon + ";<br>";
                }

                if (!string.IsNullOrEmpty(thongTinBenhAn.AmDao)
                  || !string.IsNullOrEmpty(thongTinBenhAn.CoTuCung) || !string.IsNullOrEmpty(thongTinBenhAn.ThanTuCung)
                  || !string.IsNullOrEmpty(thongTinBenhAn.PhanPhu) || !string.IsNullOrEmpty(thongTinBenhAn.CacTuiCung))
                {
                    khamBenh += "b. Khám trong <br>";
                }

                if (!string.IsNullOrEmpty(thongTinBenhAn.AmDao))
                {
                    khamBenh += "- Âm đạo: " + thongTinBenhAn.AmDao + "; <br>";
                }

                if (!string.IsNullOrEmpty(thongTinBenhAn.CoTuCung))
                {
                    khamBenh += "- Cổ tử cung: " + thongTinBenhAn.CoTuCung + ";<br>";
                }

                if (!string.IsNullOrEmpty(thongTinBenhAn.ThanTuCung))
                {
                    khamBenh += "- Thân tử cung: " + thongTinBenhAn.ThanTuCung + ";<br> ";
                }

                if (!string.IsNullOrEmpty(thongTinBenhAn.PhanPhu))
                {
                    khamBenh += "- Phần phụ: " + thongTinBenhAn.PhanPhu + ";<br>";
                }

                if (!string.IsNullOrEmpty(thongTinBenhAn.CacTuiCung))
                {
                    khamBenh += "- Các túi cùng: " + thongTinBenhAn.CacTuiCung + ";<br>";
                }

                var SOBHYT = "";
                if (thongTinBenhNhan?.BHYTMaSoThe != null)
                {
                    var stringPlit = thongTinBenhNhan?.BHYTMaSoThe.Split("-");
                    SOBHYT = stringPlit[0];
                }
                var data = new ThongTinBenhKhac
                {
                    PhongKham = tenPhongKham,
                    So = noiTruBenhAn.SoBenhAn,
                    HoTenBenhNhan = thongTinBenhNhan.HoTenBenhNhan,
                    SinhNgay = thongTinBenhNhan.SinhNgay,
                    GioiTinh = thongTinBenhNhan.GioiTinh,
                    NgheNghiep = thongTinBenhNhan.NgheNghiep,
                    DanToc = thongTinBenhNhan.DanToc,
                    QuocTich = thongTinBenhNhan.QuocTich,
                    DiaChi = thongTinBenhNhan.SoNha,
                    SoNha = thongTinBenhNhan.SoNha,
                    XaPhuong = thongTinBenhNhan.XaPhuong,
                    Huyen = thongTinBenhNhan.Huyen,
                    TinhThanhPho = thongTinBenhNhan.TinhThanhPho,
                    NoiLamViec = thongTinBenhNhan.NoiLamViec,
                    DoiTuong = thongTinBenhNhan.DoiTuong,
                    BHYTNgayHetHan = thongTinBenhNhan.BHYTNgayHetHan,
                    BHYTMaSoThe = SOBHYT,
                    ChanDoanNoiGioiThieu = noiTruBenhAn?.YeuCauTiepNhan?.YeuCauNhapVien?.YeuCauKhamBenh?.ChanDoanCuaNoiGioiThieu,
                    NguoiLienHeQuanHeThanNhan = thongTinBenhNhan.NguoiLienHeQuanHeThanNhan,
                    SoDienThoai = thongTinBenhNhan.NguoiLienHeQuanSoDienThoai,
                    NguoiLienHeQuanHeSoDienThoai = thongTinBenhNhan.NguoiLienHeQuanSoDienThoai,
                    GioKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Hour.ConvertHourToString(),
                    NgayKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Day.ConvertDateToString(),
                    ThangKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Month.ConvertMonthToString(),
                    NamKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Year.ConvertYearToString(),
                    ThoiDiemTiepNhan = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatDate(),
                    TienSuBenh = (!string.IsNullOrEmpty(tienSuBenhBanThan) ? "Bản thân: " + tienSuBenhBanThan : "")
                            + (!string.IsNullOrEmpty(tienSuBenhGiaDinh) ? "<br>Gia đình: " + tienSuBenhGiaDinh : ""),
                    TienSuDiUng = string.IsNullOrEmpty(tienSuDiUngThuoc) && string.IsNullOrEmpty(tienSuDiUngThucAn) && string.IsNullOrEmpty(tienSuDiUngKhac)
                                        ? "Chưa phát hiện" : ((!string.IsNullOrEmpty(tienSuDiUngThuoc) ? "Thuốc: " + tienSuDiUngThuoc : "")
                                        + (!string.IsNullOrEmpty(tienSuDiUngThucAn) ? "Thức ăn: " + tienSuDiUngThucAn : "")
                                        + (!string.IsNullOrEmpty(tienSuDiUngKhac) ? "Khác: " + tienSuDiUngKhac : "")),
                    LyDoVaoVien = thongTinBenhAn.LyDoVaoVien,
                    LyDoVaoKham = thongTinBenhAn.LyDoVaoVien,
                    BenhSu = thongTinBenhAn.QuaTrinhHoiBenh,
                    Mach = ketQuaSinhHieu?.NhipTim?.ApplyNumber(),
                    NhietDo = ketQuaSinhHieu?.ThanNhiet?.ApplyNumber(),
                    HuyetAp = (ketQuaSinhHieu?.HuyetApTamThu != null && ketQuaSinhHieu?.HuyetApTamTruong != null) ? (ketQuaSinhHieu?.HuyetApTamThu.ToString() + "/" + ketQuaSinhHieu?.HuyetApTamTruong.ToString()) : null,
                    NhipTho = ketQuaSinhHieu?.NhipTho == null ? null : ketQuaSinhHieu?.NhipTho.ToString(),
                    CanNang = ketQuaSinhHieu?.CanNang == null ? null : ketQuaSinhHieu?.CanNang.ToString(),
                    ChieuCao = ketQuaSinhHieu?.ChieuCao == null ? null : ketQuaSinhHieu?.ChieuCao.ToString(),
                    SpO2 = ketQuaSinhHieu?.SpO2 == null ? null : ((double?)Math.Round((ketQuaSinhHieu.SpO2.Value), 2)).ToString(),
                    BMI = ketQuaSinhHieu?.BMI == null ? null : ((double?)Math.Round((ketQuaSinhHieu.BMI.Value), 2)).ToString(),
                    KhamBenh = khamBenh,
                    DaXuLi = tenYCDuocPhamBV,

                    HuongXuLy = thongTinBenhAn.HuongXuLyLoiDanBs,

                    ChanDoan = (!string.IsNullOrEmpty(thongTinBenhAn.ChuanDoan) ? thongTinBenhAn.ChuanDoan : "") + (!string.IsNullOrEmpty(chanDoanKemTheo) ? "<br>- Chẩn đoán kèm theo: " + chanDoanKemTheo : ""),
                    KetQuaCanLamSang = thongTinBenhAn.TomTatBenhAn,
                    XetNghiemDaCoVaLamMoi = thongTinBenhAn.CacXetNghiemCanLam,

                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString(),
                };

                //cập nhật data muốn xuống hàng theo y khách hàng
                var dataJson = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                var replaceData = dataJson.Replace("\\n", "<br/>");
                var newData = JsonConvert.DeserializeObject<ThongTinBenhKhac>(replaceData);
                //cập nhật data muốn xuống hàng theo y khách hàng

                content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, newData);
            }
            else if (noiTruBenhAn.LoaiBenhAn == LoaiBenhAn.SanKhoaMo || noiTruBenhAn.LoaiBenhAn == LoaiBenhAn.SanKhoaThuong)
            {
                var khamBenh = string.Empty;
                if (!string.IsNullOrEmpty(thongTinBenhAn.TuanHoan) || !string.IsNullOrEmpty(thongTinBenhAn.HoHap)
                   || !string.IsNullOrEmpty(thongTinBenhAn.TieuHoa) || !string.IsNullOrEmpty(thongTinBenhAn.ThanTietNieu)
                   || !string.IsNullOrEmpty(thongTinBenhAn.CacBoPhanKhac) || !string.IsNullOrEmpty(thongTinBenhAn.ToanTrang))
                {
                    khamBenh += "1. Toàn thân <br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.ToanTrang))
                {
                    khamBenh += "- Toàn trạng: " + thongTinBenhAn.ToanTrang + ";<br>";
                }
                khamBenh += "- Phù: " + (thongTinBenhAn.Phu ? "Có" : "Không") + ";<br>";
                if (!string.IsNullOrEmpty(thongTinBenhAn.TuanHoan))
                {
                    khamBenh += "- Tuần hoàn: " + thongTinBenhAn.TuanHoan + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.HoHap))
                {
                    khamBenh += "- Hô hấp: " + thongTinBenhAn.HoHap + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.TieuHoa))
                {
                    khamBenh += "- Tiêu hóa: " + thongTinBenhAn.TieuHoa + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.CacBoPhanKhac))
                {
                    khamBenh += "- Các bộ phận khác: " + thongTinBenhAn.CacBoPhanKhac + ";<br>";
                }

                if ((thongTinBenhAn.ChieuCaoTuCung != 0 && thongTinBenhAn.ChieuCaoTuCung != null) || (thongTinBenhAn.VongBung != 0 && thongTinBenhAn.VongBung != null)
                    || (thongTinBenhAn.TimThai != 0 && thongTinBenhAn.TimThai != null) || !string.IsNullOrEmpty(thongTinBenhAn.HinhDangTuCung)
                  || !string.IsNullOrEmpty(thongTinBenhAn.TuThe) || !string.IsNullOrEmpty(thongTinBenhAn.ConCoTC)
                  || !string.IsNullOrEmpty(thongTinBenhAn.Vu))
                {
                    khamBenh += "2. Khám ngoài <br>";
                }
                khamBenh += "- Bụng có sẹo cũ: " + (thongTinBenhAn.BungCoSoCu ? "Có" : "Không") + ";<br>";
                if (thongTinBenhAn.ChieuCaoTuCung != 0 && thongTinBenhAn.ChieuCaoTuCung != null)
                {
                    khamBenh += "- Chiều cao TC (cm): " + thongTinBenhAn.ChieuCaoTuCung?.ApplyNumber() + ";<br>";
                }

                if (thongTinBenhAn.VongBung != 0 && thongTinBenhAn.VongBung != null)
                {
                    khamBenh += "- Vòng bụng (cm): " + thongTinBenhAn.VongBung?.ApplyNumber() + ";<br>";
                }

                if (thongTinBenhAn.TimThai != 0 && thongTinBenhAn.TimThai != null)
                {
                    khamBenh += "- Tim thai (lần/phút): " + thongTinBenhAn.TimThai?.ApplyNumber() + ";<br> ";
                }

                if (!string.IsNullOrEmpty(thongTinBenhAn.HinhDangTuCung))
                {
                    khamBenh += "- Hình dạng TC: " + thongTinBenhAn.HinhDangTuCung + ";<br>";
                }

                if (!string.IsNullOrEmpty(thongTinBenhAn.TuThe))
                {
                    khamBenh += "- Tư thế: " + thongTinBenhAn.TuThe + ";<br> ";
                }

                if (!string.IsNullOrEmpty(thongTinBenhAn.ConCoTC))
                {
                    khamBenh += "- Cơn co TC: " + thongTinBenhAn.ConCoTC + ";<br>";
                }

                if (!string.IsNullOrEmpty(thongTinBenhAn.Vu))
                {
                    khamBenh += "- Vú: " + thongTinBenhAn.Vu + ";<br>";
                }

                if ((thongTinBenhAn.ChiSoBiShop != 0 && thongTinBenhAn.ChiSoBiShop != null)
                  || !string.IsNullOrEmpty(thongTinBenhAn.AmHo) || !string.IsNullOrEmpty(thongTinBenhAn.AmDao)
                  || !string.IsNullOrEmpty(thongTinBenhAn.CoTuCung) || !string.IsNullOrEmpty(thongTinBenhAn.PhanPhu)
                  || thongTinBenhAn.NgayVoNuocOi != null || !string.IsNullOrEmpty(thongTinBenhAn.MauSacNuocOi)
                  || !string.IsNullOrEmpty(thongTinBenhAn.Ngoi) || !string.IsNullOrEmpty(thongTinBenhAn.The)
                  || !string.IsNullOrEmpty(thongTinBenhAn.TangSinhMon) || thongTinBenhAn.DoLotId != null
                  || thongTinBenhAn.TinhTrangVoOiId != null || thongTinBenhAn.VoOiId != null
                  || !string.IsNullOrEmpty(thongTinBenhAn.NuocOiNhieuHayIt) || !string.IsNullOrEmpty(thongTinBenhAn.KieuThe)
                  || !string.IsNullOrEmpty(thongTinBenhAn.DuongKinhNhoHaVe))
                {
                    khamBenh += "3. Khám trong <br>";
                }

                if (thongTinBenhAn.ChiSoBiShop != 0 && thongTinBenhAn.ChiSoBiShop != null)
                {
                    khamBenh += "- Chỉ số Bishop (điểm): " + thongTinBenhAn.ChiSoBiShop?.ApplyNumber() + "; <br>";
                }

                if (!string.IsNullOrEmpty(thongTinBenhAn.AmHo))
                {
                    khamBenh += "- Âm hộ: " + thongTinBenhAn.AmHo + ";<br>";
                }

                if (!string.IsNullOrEmpty(thongTinBenhAn.AmDao))
                {
                    khamBenh += "- Âm đạo: " + thongTinBenhAn.AmDao + ";<br> ";
                }

                if (!string.IsNullOrEmpty(thongTinBenhAn.CoTuCung))
                {
                    khamBenh += "- Cổ tử cung: " + thongTinBenhAn.CoTuCung + ";<br>";
                }

                if (!string.IsNullOrEmpty(thongTinBenhAn.PhanPhu))
                {
                    khamBenh += "- Phần phụ: " + thongTinBenhAn.PhanPhu + ";<br>";
                }

                /////
                if (thongTinBenhAn.NgayVoNuocOi != null)
                {
                    khamBenh += "- Ối vỡ lúc: " + thongTinBenhAn.NgayVoNuocOi.Value.ApplyFormatDate() + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.MauSacNuocOi))
                {
                    khamBenh += "- Màu sắc nước ối: " + thongTinBenhAn.MauSacNuocOi + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.Ngoi))
                {
                    khamBenh += "- Ngôi: " + thongTinBenhAn.Ngoi + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.The))
                {
                    khamBenh += "- Thế: " + thongTinBenhAn.The + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.TangSinhMon))
                {
                    khamBenh += "- Tầng sinh môn: " + thongTinBenhAn.TangSinhMon + ";<br>";
                }
                if (thongTinBenhAn.DoLotId != 0 && thongTinBenhAn.DoLotId != null)
                {
                    khamBenh += "- Độ lọt: " + thongTinBenhAn.TenDoLot + ";<br>";
                }
                if (thongTinBenhAn.TinhTrangVoOiId != 0 && thongTinBenhAn.TinhTrangVoOiId != null)
                {
                    khamBenh += "- Tình trạng ối: " + thongTinBenhAn.TenTinhTrangVoOi + ";<br>";
                }
                if (thongTinBenhAn.VoOiId != 0 && thongTinBenhAn.VoOiId != null)
                {
                    khamBenh += "- Ối vỡ: " + thongTinBenhAn.TenVoOi + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.NuocOiNhieuHayIt))
                {
                    khamBenh += "- Nước ối nhiều hay ít: " + thongTinBenhAn.NuocOiNhieuHayIt + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.KieuThe))
                {
                    khamBenh += "- Kiểu thế: " + thongTinBenhAn.KieuThe + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.DuongKinhNhoHaVe))
                {
                    khamBenh += "- Đường kính nhô hạ vệ: " + thongTinBenhAn.DuongKinhNhoHaVe + ";<br>";
                }
                var SOBHYT = "";
                if (thongTinBenhNhan?.BHYTMaSoThe != null)
                {
                    var stringPlit = thongTinBenhNhan?.BHYTMaSoThe.Split("-");
                    SOBHYT = stringPlit[0];
                }
                var data = new ThongTinBenhKhac
                {
                    PhongKham = tenPhongKham,
                    So = noiTruBenhAn.SoBenhAn,
                    HoTenBenhNhan = thongTinBenhNhan.HoTenBenhNhan,
                    SinhNgay = thongTinBenhNhan.SinhNgay,
                    GioiTinh = thongTinBenhNhan.GioiTinh,
                    NgheNghiep = thongTinBenhNhan.NgheNghiep,
                    DanToc = thongTinBenhNhan.DanToc,
                    QuocTich = thongTinBenhNhan.QuocTich,
                    DiaChi = thongTinBenhNhan.SoNha,
                    SoNha = thongTinBenhNhan.SoNha,
                    XaPhuong = thongTinBenhNhan.XaPhuong,
                    Huyen = thongTinBenhNhan.Huyen,
                    TinhThanhPho = thongTinBenhNhan.TinhThanhPho,
                    NoiLamViec = thongTinBenhNhan.NoiLamViec,
                    DoiTuong = thongTinBenhNhan.DoiTuong,
                    BHYTNgayHetHan = thongTinBenhNhan.BHYTNgayHetHan,
                    BHYTMaSoThe = SOBHYT,
                    ChanDoanNoiGioiThieu = noiTruBenhAn?.YeuCauTiepNhan?.YeuCauNhapVien?.YeuCauKhamBenh?.ChanDoanCuaNoiGioiThieu,
                    NguoiLienHeQuanHeThanNhan = thongTinBenhNhan.NguoiLienHeQuanHeThanNhan,
                    NguoiLienHeQuanHeSoDienThoai = thongTinBenhNhan.NguoiLienHeQuanSoDienThoai,
                    SoDienThoai = thongTinBenhNhan.NguoiLienHeQuanSoDienThoai,
                    GioKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Hour.ConvertHourToString(),
                    NgayKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Day.ConvertDateToString(),
                    ThangKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Month.ConvertMonthToString(),
                    NamKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Year.ConvertYearToString(),
                    ThoiDiemTiepNhan = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatDate(),
                    TienSuBenh = (!string.IsNullOrEmpty(tienSuBenhBanThan) ? "Bản thân: " + tienSuBenhBanThan : "")
                            + (!string.IsNullOrEmpty(tienSuBenhGiaDinh) ? "<br>Gia đình: " + tienSuBenhGiaDinh : ""),
                    TienSuDiUng = string.IsNullOrEmpty(tienSuDiUngThuoc) && string.IsNullOrEmpty(tienSuDiUngThucAn) && string.IsNullOrEmpty(tienSuDiUngKhac)
                                        ? "Chưa phát hiện" : ((!string.IsNullOrEmpty(tienSuDiUngThuoc) ? "Thuốc: " + tienSuDiUngThuoc : "")
                                        + (!string.IsNullOrEmpty(tienSuDiUngThucAn) ? "Thức ăn: " + tienSuDiUngThucAn : "")
                                        + (!string.IsNullOrEmpty(tienSuDiUngKhac) ? "Khác: " + tienSuDiUngKhac : "")),
                    LyDoVaoVien = thongTinBenhAn.LyDoVaoVien,
                    LyDoVaoKham = thongTinBenhAn.LyDoVaoVien,
                    BenhSu = thongTinBenhAn.QuaTrinhHoiBenh,
                    Mach = ketQuaSinhHieu?.NhipTim?.ApplyNumber(),
                    NhietDo = ketQuaSinhHieu?.ThanNhiet?.ApplyNumber(),
                    HuyetAp = (ketQuaSinhHieu?.HuyetApTamThu != null && ketQuaSinhHieu?.HuyetApTamTruong != null) ? (ketQuaSinhHieu?.HuyetApTamThu.ToString() + "/" + ketQuaSinhHieu?.HuyetApTamTruong.ToString()) : null,
                    NhipTho = ketQuaSinhHieu?.NhipTho == null ? null : ketQuaSinhHieu?.NhipTho.ToString(),
                    CanNang = ketQuaSinhHieu?.CanNang == null ? null : ketQuaSinhHieu?.CanNang.ToString(),
                    ChieuCao = ketQuaSinhHieu?.ChieuCao == null ? null : ketQuaSinhHieu?.ChieuCao.ToString(),
                    SpO2 = ketQuaSinhHieu?.SpO2 == null ? null : ((double?)Math.Round((ketQuaSinhHieu.SpO2.Value), 2)).ToString(),
                    BMI = ketQuaSinhHieu?.BMI == null ? null : ((double?)Math.Round((ketQuaSinhHieu.BMI.Value), 2)).ToString(),
                    KhamBenh = khamBenh,
                    DaXuLi = tenYCDuocPhamBV,
                    HuongXuLy = thongTinBenhAn.HuongXuLyLoiDanBs,
                    ChanDoan = (!string.IsNullOrEmpty(thongTinBenhAn.ChuanDoan) ? thongTinBenhAn.ChuanDoan : "") + (!string.IsNullOrEmpty(chanDoanKemTheo) ? "<br>- Chẩn đoán kèm theo: " + chanDoanKemTheo : ""),
                    KetQuaCanLamSang = thongTinBenhAn.TomTatBenhAn,
                    XetNghiemDaCoVaLamMoi = thongTinBenhAn.CacXetNghiemCanLam,
                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString()
                };

                //cập nhật data muốn xuống hàng theo y khách hàng
                var dataJson = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                var replaceData = dataJson.Replace("\\n", "<br/>");
                var newData = JsonConvert.DeserializeObject<ThongTinBenhKhac>(replaceData);
                //cập nhật data muốn xuống hàng theo y khách hàng

                content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, newData);
            }
            else if (noiTruBenhAn.LoaiBenhAn == LoaiBenhAn.NgoaiKhoa || noiTruBenhAn.LoaiBenhAn == LoaiBenhAn.ThamMy)
            {
                var khamBenh = string.Empty;
                if (thongTinBenhAn.ThoiGianDiUng != 0 && thongTinBenhAn.ThoiGianDiUng != null)
                {
                    dacDiemLienQuanBenh += thongTinBenhAn.KyHieuDiUng + "(thời gian theo tháng): " + thongTinBenhAn.ThoiGianDiUng + "; ";
                }
                if (thongTinBenhAn.ThoiGianMaTuy != 0 && thongTinBenhAn.ThoiGianMaTuy != null)
                {
                    dacDiemLienQuanBenh += thongTinBenhAn.KyHieuMaTuy + "(thời gian theo tháng): " + thongTinBenhAn.ThoiGianMaTuy + "; ";
                }
                if (thongTinBenhAn.ThoiGianRuouBia != 0 && thongTinBenhAn.ThoiGianRuouBia != null)
                {
                    dacDiemLienQuanBenh += thongTinBenhAn.KyHieuRuouBia + "(thời gian theo tháng): " + thongTinBenhAn.ThoiGianRuouBia + "; ";
                }
                if (thongTinBenhAn.ThoiGianThuocLa != 0 && thongTinBenhAn.ThoiGianThuocLa != null)
                {
                    dacDiemLienQuanBenh += thongTinBenhAn.KyHieuThuocLa + "(thời gian theo tháng): " + thongTinBenhAn.ThoiGianThuocLa + "; ";
                }
                if (thongTinBenhAn.ThoiGianThuocLao != 0 && thongTinBenhAn.ThoiGianThuocLao != null)
                {
                    dacDiemLienQuanBenh += thongTinBenhAn.KyHieuThuocLao + "(thời gian theo tháng): " + thongTinBenhAn.ThoiGianThuocLao + "; ";
                }
                if (thongTinBenhAn.ThoiGianKhac != 0 && thongTinBenhAn.ThoiGianKhac != null)
                {
                    dacDiemLienQuanBenh += thongTinBenhAn.KyHieuKhac + "(thời gian theo tháng): " + thongTinBenhAn.ThoiGianKhac + "; ";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.KhamBenhToanThan))
                {
                    khamBenh += " 1. Toàn thân <br>" + "- Toàn thân: " + thongTinBenhAn.KhamBenhToanThan + ";<br>";
                }

                if (thongTinBenhAn.BoPhanTonThuongs != null && thongTinBenhAn.BoPhanTonThuongs.Any())
                {
                    khamBenh += " 2. Bệnh ngoại khoa <br>";
                    foreach (var item in thongTinBenhAn.BoPhanTonThuongs)
                    {
                        khamBenh += "<img style='height: 250px;width: 300px;align: top' src='" + item.HinhAnh + "'/><br> Mô tả: " + item.MoTa + "<br>";
                    }
                }

                if (!string.IsNullOrEmpty(thongTinBenhAn.TuanHoan) || !string.IsNullOrEmpty(thongTinBenhAn.HoHap)
                    || !string.IsNullOrEmpty(thongTinBenhAn.TieuHoa) || !string.IsNullOrEmpty(thongTinBenhAn.ThanKinh)
                    || !string.IsNullOrEmpty(thongTinBenhAn.CoXuongKhop) || !string.IsNullOrEmpty(thongTinBenhAn.ThanTietNieu)
                    || !string.IsNullOrEmpty(thongTinBenhAn.TaiMuiHong) || !string.IsNullOrEmpty(thongTinBenhAn.RangHamMat)
                    || !string.IsNullOrEmpty(thongTinBenhAn.Mat) || !string.IsNullOrEmpty(thongTinBenhAn.NoiTiet))
                {
                    khamBenh += " 3. Các cơ quan <br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.TuanHoan))
                {
                    khamBenh += "- Tuần hoàn: " + thongTinBenhAn.TuanHoan + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.HoHap))
                {
                    khamBenh += "- Hô hấp: " + thongTinBenhAn.HoHap + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.TieuHoa))
                {
                    khamBenh += "- Tiêu hóa: " + thongTinBenhAn.TieuHoa + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.ThanTietNieu))
                {
                    khamBenh += "- Thận-Tiết niệu: " + thongTinBenhAn.ThanTietNieu + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.ThanKinh))
                {
                    khamBenh += "- Thần kinh: " + thongTinBenhAn.ThanKinh + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.CoXuongKhop))
                {
                    khamBenh += "- Cơ-Xương-Khớp: " + thongTinBenhAn.CoXuongKhop + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.TaiMuiHong))
                {
                    khamBenh += "- Tai-Mũi-Họng: " + thongTinBenhAn.TaiMuiHong + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.RangHamMat))
                {
                    khamBenh += "- Răng-Hàm-Mặt: " + thongTinBenhAn.RangHamMat + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.Mat))
                {
                    khamBenh += "- Mắt: " + thongTinBenhAn.Mat + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.NoiTiet))
                {
                    khamBenh += "- Nội tiết, dinh dưỡng và các bệnh lý khác: " + thongTinBenhAn.NoiTiet + ";<br>";
                }
                var SOBHYT = "";
                if (thongTinBenhNhan?.BHYTMaSoThe != null)
                {
                    var stringPlit = thongTinBenhNhan?.BHYTMaSoThe.Split("-");
                    SOBHYT = stringPlit[0];
                }
                var data = new ThongTinBenhKhac
                {
                    PhongKham = tenPhongKham,
                    So = noiTruBenhAn.SoBenhAn,
                    HoTenBenhNhan = thongTinBenhNhan.HoTenBenhNhan,
                    SinhNgay = thongTinBenhNhan.SinhNgay,
                    GioiTinh = thongTinBenhNhan.GioiTinh,
                    NgheNghiep = thongTinBenhNhan.NgheNghiep,
                    DanToc = thongTinBenhNhan.DanToc,
                    QuocTich = thongTinBenhNhan.QuocTich,
                    DiaChi = thongTinBenhNhan.SoNha,
                    SoNha = thongTinBenhNhan.SoNha,
                    XaPhuong = thongTinBenhNhan.XaPhuong,
                    Huyen = thongTinBenhNhan.Huyen,
                    TinhThanhPho = thongTinBenhNhan.TinhThanhPho,
                    NoiLamViec = thongTinBenhNhan.NoiLamViec,
                    DoiTuong = thongTinBenhNhan.DoiTuong,
                    BHYTNgayHetHan = thongTinBenhNhan.BHYTNgayHetHan,
                    BHYTMaSoThe = SOBHYT,
                    ChanDoanNoiGioiThieu = noiTruBenhAn?.YeuCauTiepNhan?.YeuCauNhapVien?.YeuCauKhamBenh?.ChanDoanCuaNoiGioiThieu,
                    NguoiLienHeQuanHeThanNhan = thongTinBenhNhan.NguoiLienHeQuanHeThanNhan,
                    NguoiLienHeQuanHeSoDienThoai = thongTinBenhNhan.NguoiLienHeQuanSoDienThoai,
                    SoDienThoai = thongTinBenhNhan.NguoiLienHeQuanSoDienThoai,
                    GioKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Hour.ConvertHourToString(),
                    NgayKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Day.ConvertDateToString(),
                    ThangKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Month.ConvertMonthToString(),
                    NamKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Year.ConvertYearToString(),
                    LyDoVaoVien = thongTinBenhAn.LyDoVaoVien,
                    LyDoVaoKham = thongTinBenhAn.LyDoVaoVien,
                    ThoiDiemTiepNhan = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatDate(),
                    TienSuBenh = (!string.IsNullOrEmpty(tienSuBenhBanThan) ? "Bản thân: " + tienSuBenhBanThan : "")
                            + (!string.IsNullOrEmpty(tienSuBenhGiaDinh) ? "<br>Gia đình: " + tienSuBenhGiaDinh : "") +
                            (!string.IsNullOrEmpty(dacDiemLienQuanBenh) ? "<br> Đặc điểm liên quan bệnh: " + dacDiemLienQuanBenh : ""),
                    TienSuDiUng = string.IsNullOrEmpty(tienSuDiUngThuoc) && string.IsNullOrEmpty(tienSuDiUngThucAn) && string.IsNullOrEmpty(tienSuDiUngKhac)
                                        ? "Chưa phát hiện" : ((!string.IsNullOrEmpty(tienSuDiUngThuoc) ? "Thuốc: " + tienSuDiUngThuoc : "")
                                        + (!string.IsNullOrEmpty(tienSuDiUngThucAn) ? "Thức ăn: " + tienSuDiUngThucAn : "")
                                        + (!string.IsNullOrEmpty(tienSuDiUngKhac) ? "Khác: " + tienSuDiUngKhac : "")),
                    //BenhSu = benhSu,
                    BenhSu = thongTinBenhAn.QuaTrinhHoiBenh,
                    Mach = ketQuaSinhHieu?.NhipTim?.ApplyNumber(),
                    NhietDo = ketQuaSinhHieu?.ThanNhiet?.ApplyNumber(),
                    HuyetAp = (ketQuaSinhHieu?.HuyetApTamThu != null && ketQuaSinhHieu?.HuyetApTamTruong != null) ? (ketQuaSinhHieu?.HuyetApTamThu.ToString() + "/" + ketQuaSinhHieu?.HuyetApTamTruong.ToString()) : null,
                    NhipTho = ketQuaSinhHieu?.NhipTho == null ? null : ketQuaSinhHieu?.NhipTho.ToString(),
                    CanNang = ketQuaSinhHieu?.CanNang == null ? null : ketQuaSinhHieu?.CanNang.ToString(),
                    ChieuCao = ketQuaSinhHieu?.ChieuCao == null ? null : ketQuaSinhHieu?.ChieuCao.ToString(),
                    SpO2 = ketQuaSinhHieu?.SpO2 == null ? null : ((double?)Math.Round((ketQuaSinhHieu.SpO2.Value), 2)).ToString(),
                    BMI = ketQuaSinhHieu?.BMI == null ? null : ((double?)Math.Round((ketQuaSinhHieu.BMI.Value), 2)).ToString(),
                    KhamBenh = khamBenh,
                    DaXuLi = tenYCDuocPhamBV,
                    HuongXuLy = thongTinBenhAn.HuongXuLyLoiDanBs,
                    KetQuaCanLamSang = thongTinBenhAn.TomTatBenhAn,
                    XetNghiemDaCoVaLamMoi = thongTinBenhAn.CacXetNghiemCanLam,
                    ChanDoan = (!string.IsNullOrEmpty(thongTinBenhAn.ChuanDoan) ? thongTinBenhAn.ChuanDoan : "") + (!string.IsNullOrEmpty(chanDoanKemTheo) ? "<br>- Chẩn đoán kèm theo: " + chanDoanKemTheo : ""),
                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString()
                };

                //cập nhật data muốn xuống hàng theo y khách hàng
                var dataJson = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                var replaceData = dataJson.Replace("\\n", "<br/>");
                var newData = JsonConvert.DeserializeObject<ThongTinBenhKhac>(replaceData);
                //cập nhật data muốn xuống hàng theo y khách hàng

                content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, newData);
            }
            else if (noiTruBenhAn.LoaiBenhAn == LoaiBenhAn.TreSoSinh)
            {
                var khamBenh = string.Empty;
                if (!string.IsNullOrEmpty(thongTinBenhAn.TenNhanVienChuyenSoSinh)
                  || !string.IsNullOrEmpty(thongTinBenhAn.TenNhanVienChucDan))
                {
                    khamBenh += "Người chuyển sơ sinh: " + thongTinBenhAn.TenNhanVienChuyenSoSinh + "; Chức danh: " + thongTinBenhAn.TenNhanVienChucDan + "<br>";
                }

                if (thongTinBenhAn.DiTatBamSinh == true || thongTinBenhAn.CoHauMon == true
                   || !string.IsNullOrEmpty(thongTinBenhAn.TinhHinhSoSinhKhiVaoKhoa)
                  || !string.IsNullOrEmpty(thongTinBenhAn.TinhHinhToanThan)
                  || thongTinBenhAn.TinhTrangSoSinh != null)
                {
                    khamBenh += "1. Toàn thân <br>";
                }
                if (thongTinBenhAn.DiTatBamSinh == true)
                {
                    khamBenh += "- Dị tật bẩm sinh: Có;";
                    if (!string.IsNullOrEmpty(thongTinBenhAn.ChuThichDiTatBamSinh))
                    {
                        khamBenh += thongTinBenhAn.ChuThichDiTatBamSinh + "<br>";
                    }
                    else
                    {
                        khamBenh += "<br>";
                    }
                }
                if (thongTinBenhAn.CoHauMon == true)
                {
                    khamBenh += "- Có hậu môn: Có;<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.TinhHinhSoSinhKhiVaoKhoa))
                {
                    khamBenh += "- Tình hình sơ sinh khi vào khoa: " + thongTinBenhAn.TinhHinhSoSinhKhiVaoKhoa + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.TinhHinhToanThan))
                {
                    khamBenh += "- Tình trạng toàn thân: " + thongTinBenhAn.TinhHinhToanThan + ";<br>";
                }
                if (thongTinBenhAn.TrangThaiMauSacDa != null)
                {
                    khamBenh += "- Màu sắc da: " + thongTinBenhAn.TrangThaiMauSacDa.GetDescription() + ";";
                    if (thongTinBenhAn.TrangThaiMauSacDa == MauSacDa.Khac)
                    {
                        khamBenh += "- Ghi chú: " + thongTinBenhAn.ChuThichMauSacDa + ";<br>";
                    }
                    else
                    {
                        khamBenh += "<br>";
                    }
                }
                if ((thongTinBenhAn.HoHapNhiTho != 0 && thongTinBenhAn.HoHapNhiTho != null)
                    || (thongTinBenhAn.HoHapChiSoSilverman != 0 && thongTinBenhAn.HoHapChiSoSilverman != null)
                    || (thongTinBenhAn.TimMachNhipTim != 0 && thongTinBenhAn.TimMachNhipTim != null)
                    || !string.IsNullOrEmpty(thongTinBenhAn.Bung) || !string.IsNullOrEmpty(thongTinBenhAn.CacCoQuanSinhDucNgoai)
                    || !string.IsNullOrEmpty(thongTinBenhAn.XuongKhop) || !string.IsNullOrEmpty(thongTinBenhAn.ThanKinhPhanXa)
                    || !string.IsNullOrEmpty(thongTinBenhAn.ThanKinhTruongLucCo))
                {
                    khamBenh += "2. Các cơ quan khác<br>";
                }
                if ((thongTinBenhAn.HoHapNhiTho != 0 && thongTinBenhAn.HoHapNhiTho != null)
                    || (thongTinBenhAn.HoHapChiSoSilverman != 0 && thongTinBenhAn.HoHapChiSoSilverman != null))
                {
                    khamBenh += "Hô hấp<br>";
                }
                if (thongTinBenhAn.HoHapNhiTho != 0 && thongTinBenhAn.HoHapNhiTho != null)
                {
                    khamBenh += "- Nhịp thở(lần/phút): " + thongTinBenhAn.HoHapNhiTho?.ApplyNumber() + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.HoHapNgheTho))
                {
                    khamBenh += "- Nghe phổi: " + thongTinBenhAn.HoHapNgheTho + ";<br>";
                }
                if (thongTinBenhAn.HoHapChiSoSilverman != 0 && thongTinBenhAn.HoHapChiSoSilverman != null)
                {
                    khamBenh += "- Chỉ số Silverman (điểm): " + thongTinBenhAn.HoHapChiSoSilverman?.ApplyNumber() + ";<br>";
                }
                if (thongTinBenhAn.TimMachNhipTim != 0 && thongTinBenhAn.TimMachNhipTim != null)
                {
                    khamBenh += "Tim mạch<br>" + "- Nhịp tim (lần/phút): " + thongTinBenhAn.TimMachNhipTim?.ApplyNumber() + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.Bung))
                {
                    khamBenh += "- Bụng: " + thongTinBenhAn.Bung + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.CacCoQuanSinhDucNgoai))
                {
                    khamBenh += "- Các cơ quan sinh dục ngoài: " + thongTinBenhAn.CacCoQuanSinhDucNgoai + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.XuongKhop))
                {
                    khamBenh += "- Xương khớp: " + thongTinBenhAn.XuongKhop + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.ThanKinhPhanXa) || !string.IsNullOrEmpty(thongTinBenhAn.ThanKinhTruongLucCo))
                {
                    khamBenh += "Thần kinh<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.ThanKinhPhanXa))
                {
                    khamBenh += "- Phản xạ: " + thongTinBenhAn.ThanKinhPhanXa + ";<br>";
                }
                if (!string.IsNullOrEmpty(thongTinBenhAn.ThanKinhTruongLucCo))
                {
                    khamBenh += "- Trương lực cơ: " + thongTinBenhAn.ThanKinhTruongLucCo + ";<br>";
                }
                var SOBHYT = "";
                if (thongTinBenhNhan?.BHYTMaSoThe != null)
                {
                    var stringPlit = thongTinBenhNhan?.BHYTMaSoThe.Split("-");
                    SOBHYT = stringPlit[0];
                }
                var data = new ThongTinBenhKhac
                {
                    PhongKham = tenPhongKham,
                    So = noiTruBenhAn.SoBenhAn,
                    HoTenBenhNhan = thongTinBenhNhan.HoTenBenhNhan,
                    SinhNgay = thongTinBenhNhan.SinhNgay,
                    GioiTinh = thongTinBenhNhan.GioiTinh,
                    NgheNghiep = thongTinBenhNhan.NgheNghiep,
                    DanToc = thongTinBenhNhan.DanToc,
                    QuocTich = thongTinBenhNhan.QuocTich,
                    DiaChi = thongTinBenhNhan.SoNha,
                    SoNha = thongTinBenhNhan.SoNha,
                    XaPhuong = thongTinBenhNhan.XaPhuong,
                    Huyen = thongTinBenhNhan.Huyen,
                    TinhThanhPho = thongTinBenhNhan.TinhThanhPho,
                    NoiLamViec = thongTinBenhNhan.NoiLamViec,
                    DoiTuong = thongTinBenhNhan.DoiTuong,
                    BHYTNgayHetHan = thongTinBenhNhan.BHYTNgayHetHan,
                    BHYTMaSoThe = SOBHYT,
                    ChanDoanNoiGioiThieu = noiTruBenhAn?.YeuCauTiepNhan?.YeuCauNhapVien?.YeuCauKhamBenh?.ChanDoanCuaNoiGioiThieu,
                    NguoiLienHeQuanHeThanNhan = thongTinBenhNhan.NguoiLienHeQuanHeThanNhan,
                    NguoiLienHeQuanHeSoDienThoai = thongTinBenhNhan.NguoiLienHeQuanSoDienThoai,
                    SoDienThoai = thongTinBenhNhan.NguoiLienHeQuanSoDienThoai,
                    GioKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Hour.ConvertHourToString(),
                    NgayKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Day.ConvertDateToString(),
                    ThangKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Month.ConvertMonthToString(),
                    NamKham = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.Year.ConvertYearToString(),
                    ThoiDiemTiepNhan = noiTruBenhAn.YeuCauTiepNhan.ThoiDiemTiepNhan.ApplyFormatDate(),
                    LyDoVaoVien = thongTinBenhAn.LyDoVaoVien,
                    LyDoVaoKham = thongTinBenhAn.LyDoVaoVien,
                    //BenhSu = benhSu,
                    BenhSu = thongTinBenhAn.QuaTrinhHoiBenh,
                    Mach = ketQuaSinhHieu?.NhipTim?.ApplyNumber(),
                    NhietDo = ketQuaSinhHieu?.ThanNhiet?.ApplyNumber(),
                    HuyetAp = (ketQuaSinhHieu?.HuyetApTamThu != null && ketQuaSinhHieu?.HuyetApTamTruong != null) ? (ketQuaSinhHieu?.HuyetApTamThu.ToString() + "/" + ketQuaSinhHieu?.HuyetApTamTruong.ToString()) : null,
                    NhipTho = ketQuaSinhHieu?.NhipTho == null ? null : ketQuaSinhHieu?.NhipTho.ToString(),
                    CanNang = ketQuaSinhHieu?.CanNang == null ? null : ketQuaSinhHieu?.CanNang.ToString(),
                    ChieuCao = ketQuaSinhHieu?.ChieuCao == null ? null : ketQuaSinhHieu?.ChieuCao.ToString(),
                    SpO2 = ketQuaSinhHieu?.SpO2 == null ? null : ((double?)Math.Round((ketQuaSinhHieu.SpO2.Value), 2)).ToString(),
                    BMI = ketQuaSinhHieu?.BMI == null ? null : ((double?)Math.Round((ketQuaSinhHieu.BMI.Value), 2)).ToString(),
                    KhamBenh = khamBenh,
                    KetQuaCanLamSang = thongTinBenhAn.TomTatBenhAn,
                    XetNghiemDaCoVaLamMoi = thongTinBenhAn.CacXetNghiemCanLam,
                    DaXuLi = tenYCDuocPhamBV,
                    HuongXuLy = thongTinBenhAn.HuongXuLyLoiDanBs,
                    ChanDoan = (!string.IsNullOrEmpty(thongTinBenhAn.ChuanDoan) ? thongTinBenhAn.ChuanDoan : "") + (!string.IsNullOrEmpty(chanDoanKemTheo) ? "<br>- Chẩn đoán kèm theo: " + chanDoanKemTheo : ""),
                    Ngay = DateTime.Now.Day.ConvertDateToString(),
                    Thang = DateTime.Now.Month.ConvertMonthToString(),
                    Nam = DateTime.Now.Year.ConvertYearToString()
                };

                //cập nhật data muốn xuống hàng theo y khách hàng
                var dataJson = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                var replaceData = dataJson.Replace("\\n", "<br/>");
                var newData = JsonConvert.DeserializeObject<ThongTinBenhKhac>(replaceData);
                //cập nhật data muốn xuống hàng theo y khách hàng

                content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, newData);
            }

            return content;
        }

    }
}
