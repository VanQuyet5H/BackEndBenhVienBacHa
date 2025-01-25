using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DonViHanhChinh;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.TrieuChungs;
using Camino.Data;
using Camino.Services.ExportImport.Help;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.DonViHanhChinh
{
    [ScopedDependency(ServiceType = typeof(IDonViHanhChinhService))]

    public class DonViHanhChinhService : MasterFileService<Core.Domain.Entities.DonViHanhChinhs.DonViHanhChinh>, IDonViHanhChinhService
    {
        public DonViHanhChinhService(IRepository<Core.Domain.Entities.DonViHanhChinhs.DonViHanhChinh> repository) : base(repository)
        {
        }
        public async Task<GridDataSource> GetDataForGridAsync(DonViHanhChinhQueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var vo = new DonViHanhChinhInfo();
                vo = JsonConvert.DeserializeObject<DonViHanhChinhInfo>(queryInfo.AdditionalSearchString);

                queryInfo.CapHanhChinhId = vo.CapHanhChinhId;
                queryInfo.TrucThuocDonViHanhChinhId = vo.TrucThuocDonViHanhChinhId;

                queryInfo.TinhThanhId = vo.TinhThanhId != null ? vo.TinhThanhId : 0;
                queryInfo.QuanHuyenId = vo.QuanHuyenId != null ? vo.QuanHuyenId : 0;
                queryInfo.PhuongXaId = vo.PhuongXaId != null ? vo.PhuongXaId : 0;
                queryInfo.KhomApId = vo.KhomApId != null ? vo.KhomApId : 0;
            }

            var query = BaseRepository.TableNoTracking;
            query = query.Where(a => a.TrucThuocDonViHanhChinhId == queryInfo.TrucThuocDonViHanhChinhId);

            switch (queryInfo.CapHanhChinhId)
            {
                case CapHanhChinh.TinhThanhPho:
                    if (queryInfo.TinhThanhId != null)
                    {
                        query = query.Where(x => queryInfo.TinhThanhId == null || queryInfo.TinhThanhId == 0 || x.CapHanhChinh == CapHanhChinh.TinhThanhPho && x.Id == queryInfo.TinhThanhId);
                    }
                    break;
                case CapHanhChinh.QuanHuyen:
                    if (queryInfo.TinhThanhId != null || queryInfo.QuanHuyenId != null)
                    {
                        query = query.Where(x => queryInfo.QuanHuyenId == null || queryInfo.QuanHuyenId == 0 || x.CapHanhChinh == CapHanhChinh.QuanHuyen && x.Id == queryInfo.QuanHuyenId);
                    }
                    break;
                case CapHanhChinh.PhuongXa:
                    if (queryInfo.TinhThanhId != null || queryInfo.QuanHuyenId != null || queryInfo.PhuongXaId != null)
                    {
                        query = query.Where(x => queryInfo.PhuongXaId == null || queryInfo.PhuongXaId == 0 || x.CapHanhChinh == CapHanhChinh.PhuongXa && x.Id == queryInfo.PhuongXaId);
                    }
                    break;
            }
            var gridVo = query
                .Select(s => new DonViHanhChinhGridVo
                {
                    Id = s.Id,
                    Ma = s.Ma,
                    Ten = s.Ten,
                    TenVietTat = s.TenVietTat,
                    CapHanhChinhId = s.CapHanhChinh,
                    TrucThuocDonViHanhChinhId = s.TrucThuocDonViHanhChinhId,
                    TrucThuocDonViHanhChinh = s.TrucThuocDonViHanhChinh != null ? s.TrucThuocDonViHanhChinh.Ten : null
                })
                .ApplyLike(queryInfo.SearchTerms,g=>g.Ten);

            var countResult = queryInfo.LazyLoadPage == true ? 0 : await gridVo.CountAsync();
            var queryResult = await gridVo.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArrayAsync();
            return new GridDataSource { Data = queryResult, TotalRowCount = countResult };
        }
        public async Task<GridDataSource> GetTotalPageForGridAsync(DonViHanhChinhQueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var vo = new DonViHanhChinhInfo();
                vo = JsonConvert.DeserializeObject<DonViHanhChinhInfo>(queryInfo.AdditionalSearchString);

                queryInfo.CapHanhChinhId = vo.CapHanhChinhId;
                queryInfo.TrucThuocDonViHanhChinhId = vo.TrucThuocDonViHanhChinhId;

                queryInfo.TinhThanhId = vo.TinhThanhId != null ? vo.TinhThanhId : 0;
                queryInfo.QuanHuyenId = vo.QuanHuyenId != null ? vo.QuanHuyenId : 0;
                queryInfo.PhuongXaId = vo.PhuongXaId != null ? vo.PhuongXaId : 0;
                queryInfo.KhomApId = vo.KhomApId != null ? vo.KhomApId : 0;
            }
            var query = BaseRepository.TableNoTracking;

            query = query.Where(a => a.TrucThuocDonViHanhChinhId == queryInfo.TrucThuocDonViHanhChinhId);

            switch (queryInfo.CapHanhChinhId)
            {
                case CapHanhChinh.TinhThanhPho:
                    if (queryInfo.TinhThanhId != null)
                    {
                        query = query.Where(x => queryInfo.TinhThanhId == null || queryInfo.TinhThanhId == 0 || x.CapHanhChinh == CapHanhChinh.TinhThanhPho && x.Id == queryInfo.TinhThanhId);
                    }
                    break;
                case CapHanhChinh.QuanHuyen:
                    if (queryInfo.TinhThanhId != null || queryInfo.QuanHuyenId != null)
                    {
                        query = query.Where(x => queryInfo.QuanHuyenId == null || queryInfo.QuanHuyenId == 0 || x.CapHanhChinh == CapHanhChinh.QuanHuyen && x.Id == queryInfo.QuanHuyenId);
                    }
                    break;
                case CapHanhChinh.PhuongXa:
                    if (queryInfo.TinhThanhId != null || queryInfo.QuanHuyenId != null || queryInfo.PhuongXaId != null)
                    {
                        query = query.Where(x => queryInfo.PhuongXaId == null || queryInfo.PhuongXaId == 0 || x.CapHanhChinh == CapHanhChinh.PhuongXa && x.Id == queryInfo.PhuongXaId);
                    }
                    break;
            }
            var gridVo = query
                .Select(s => new DonViHanhChinhGridVo
                {
                    Id = s.Id,
                    Ma = s.Ma,
                    Ten = s.Ten,
                    TenVietTat = s.TenVietTat,
                })
                .ApplyLike(queryInfo.SearchTerms, g => g.Ten);

            var countTask = gridVo.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }


        public async Task<List<LookupItemTemplateVo>> GetListDonViHanhChinh(DropDownListRequestModel model)
        {
            var list = await BaseRepository.TableNoTracking
               .Where(p => p.Ma.Contains(model.Query ?? "") || p.Ten.Contains(model.Query ?? ""))
               .Take(model.Take)
               .ToListAsync();
            var query = list.Select(item => new LookupItemTemplateVo
            {
                DisplayName = item.Ten,
                KeyId = item.Id,
                Ten = item.Ten,
                Ma = item.Ma
            }).ToList();
            return query;
        }

        public async Task<List<LookupItemVo>> GetTinhThanhLookup(DonViHanhChinhLookupQueryInfo model)
        {
            var data = await BaseRepository.TableNoTracking
                .Where(s => s.CapHanhChinh == CapHanhChinh.TinhThanhPho).Select(s => new LookupItemVo
                {
                    KeyId = s.Id,
                    DisplayName = s.Ten
                }).ApplyLike(model.Query, s => s.DisplayName)
                .OrderBy(o => o.DisplayName).ToListAsync();
            return data;
        }

        public async Task<List<LookupItemVo>> GetPhuongXaLookup(DonViHanhChinhLookupQueryInfo model, long quanHuyenId, long? tinhThanhId = null)
        {
            var data = await BaseRepository.TableNoTracking
                .Where(s => s.CapHanhChinh == CapHanhChinh.PhuongXa && s.TrucThuocDonViHanhChinhId == quanHuyenId &&
                (tinhThanhId == null || s.TrucThuocDonViHanhChinh.TrucThuocDonViHanhChinhId == tinhThanhId))
                .Select(s => new LookupItemVo
                {
                    KeyId = s.Id,
                    DisplayName = s.Ten
                }).ApplyLike(model.Query, s => s.DisplayName).OrderBy(o => o.DisplayName).ToListAsync();
            return data;
        }

        public async Task<List<LookupItemVo>> GetKhomApLookup(DonViHanhChinhLookupQueryInfo model, long khomApId, long? quanHuyenId, long? tinhThanhId = null)
        {
            var data = await BaseRepository.TableNoTracking
                .Where(s => 
                (quanHuyenId == null || s.TrucThuocDonViHanhChinh.TrucThuocDonViHanhChinhId == quanHuyenId) &&
                (tinhThanhId == null || s.TrucThuocDonViHanhChinh.TrucThuocDonViHanhChinh.TrucThuocDonViHanhChinhId == tinhThanhId))
                .Select(s => new LookupItemVo
                {
                    KeyId = s.Id,
                    DisplayName = s.Ten
                }).ApplyLike(model.Query, s => s.DisplayName).OrderBy(o => o.DisplayName).ToListAsync();
            return data;
        }

        public async Task<List<LookupItemVo>> GetQuanHuyenLookup(DonViHanhChinhLookupQueryInfo model, long tinhThanhId)
        {
            var data = await BaseRepository.TableNoTracking
                .Where(s => s.CapHanhChinh == CapHanhChinh.QuanHuyen && s.TrucThuocDonViHanhChinhId == tinhThanhId)
                .Select(s => new LookupItemVo
                {
                    KeyId = s.Id,
                    DisplayName = s.Ten
                }).ApplyLike(model.Query, s => s.DisplayName).OrderBy(o => o.DisplayName).ToListAsync();
            return data;
        }
        public bool CheckExistMaCapTinhThanhPho(string ma, long donViId = 0)
        {
            if (donViId > 0 && BaseRepository.TableNoTracking.Any(a => a.Id != donViId && a.CapHanhChinh == CapHanhChinh.TinhThanhPho && a.Ma == ma))
                return false;
            if (donViId <= 0 && BaseRepository.TableNoTracking.Any(a => a.CapHanhChinh == CapHanhChinh.TinhThanhPho && a.Ma == ma))
                return false;

            return true;
        }

        public bool CheckExistMaCapQuanHuyen(long trucThuocDonViHanhChinhId, string ma, long donViId = 0)
        {
            if (donViId > 0 &&
                BaseRepository.TableNoTracking
                .Any(a => a.Id != donViId && a.CapHanhChinh == CapHanhChinh.QuanHuyen && a.Ma == ma && a.TrucThuocDonViHanhChinhId == trucThuocDonViHanhChinhId))
                return false;

            if (donViId <= 0 &&
                BaseRepository.TableNoTracking
                .Any(a => a.CapHanhChinh == CapHanhChinh.QuanHuyen && a.Ma == ma && a.TrucThuocDonViHanhChinhId == trucThuocDonViHanhChinhId))
                return false;

            return true;
        }

        public bool CheckExistMaCapPhuongXa(long trucThuocDonViHanhChinhId, string ma, long donViId = 0)
        {
            if (donViId > 0 &&
                BaseRepository.TableNoTracking
                .Any(a => a.Id != donViId && a.CapHanhChinh == CapHanhChinh.PhuongXa && a.Ma == ma && a.TrucThuocDonViHanhChinhId == trucThuocDonViHanhChinhId))
                return false;

            if (donViId <= 0 &&
                BaseRepository.TableNoTracking
                .Any(a => a.CapHanhChinh == CapHanhChinh.PhuongXa && a.Ma == ma && a.TrucThuocDonViHanhChinhId == trucThuocDonViHanhChinhId))
                return false;

            return true;
        }
        public List<DonViHanhChinhExcel> GetDataExportExecl(DonViHanhChinhQueryInfo queryInfo)
        {
           
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                var vo = new DonViHanhChinhInfo();
                vo = JsonConvert.DeserializeObject<DonViHanhChinhInfo>(queryInfo.AdditionalSearchString);

                queryInfo.CapHanhChinhId = vo.CapHanhChinhId;
                queryInfo.TrucThuocDonViHanhChinhId = vo.TrucThuocDonViHanhChinhId;

                queryInfo.TinhThanhId = vo.TinhThanhId != null ? vo.TinhThanhId : 0;
                queryInfo.QuanHuyenId = vo.QuanHuyenId != null ? vo.QuanHuyenId : 0;
                queryInfo.PhuongXaId = vo.PhuongXaId != null ? vo.PhuongXaId : 0;
                queryInfo.KhomApId = vo.KhomApId != null ? vo.KhomApId : 0;
            }

            var queryAlls = BaseRepository.TableNoTracking.Select(s => new DonViHanhChinhExcel
            {
                Id = s.Id,
                Ma = s.Ma,
                Ten = s.Ten,
                TenVietTat = s.TenVietTat,
                CapHanhChinhId = s.CapHanhChinh,
                TrucThuocDonViHanhChinhId = s.TrucThuocDonViHanhChinhId,
                TrucThuocDonViHanhChinh = s.TrucThuocDonViHanhChinh != null ? s.TrucThuocDonViHanhChinh.Ten : null
            })
            .ToList();

            var queryTinh = BaseRepository.TableNoTracking;
            queryTinh = queryTinh.Where(a => a.TrucThuocDonViHanhChinhId == queryInfo.TrucThuocDonViHanhChinhId);
          
            var gridVo = queryTinh
                .Select(s => new DonViHanhChinhExcel
                {
                    Id = s.Id,
                    Ma = s.Ma,
                    Ten = s.Ten,
                    TenVietTat = s.TenVietTat,
                    CapHanhChinhId = s.CapHanhChinh,
                    TrucThuocDonViHanhChinhId = s.TrucThuocDonViHanhChinhId,
                    TrucThuocDonViHanhChinh = s.TrucThuocDonViHanhChinh != null ? s.TrucThuocDonViHanhChinh.Ten : null
                })
                .ToList();

            foreach (var item in gridVo)
            {
                item.DonViHanhChinhs = GetChildrenTree(queryAlls.ToList(), item.Id, (long)item.CapHanhChinhId);
            }
            return gridVo.ToList();
        }
        public static List<DonViHanhChinhExcel> GetChildrenTree(List<DonViHanhChinhExcel> comments, long Id, long level) 
        {
            var query = comments
                .Where(c => c.TrucThuocDonViHanhChinhId != null && c.TrucThuocDonViHanhChinhId == Id && (long)c.CapHanhChinhId == level + 1)
                .Select(s => new DonViHanhChinhExcel
                {
                    Id = s.Id,
                    Ma = s.Ma,
                    Ten = s.Ten,
                    TenVietTat = s.TenVietTat,
                    CapHanhChinhId = s.CapHanhChinhId,
                    TrucThuocDonViHanhChinhId = s.TrucThuocDonViHanhChinhId,
                    TrucThuocDonViHanhChinh = s.TrucThuocDonViHanhChinh,
                    DonViHanhChinhs = GetChildrenTree(comments, s.Id, (long)s.CapHanhChinhId)
                })
                .ToList();

            return query;
        }
        //public virtual byte[] ExportExelDonViHanhChinh(List<DonViHanhChinhExcel> list)
        //{

        //    using (var stream = new MemoryStream())
        //    {
        //        using (var xlPackage = new ExcelPackage(stream))
        //        {
        //            var worksheet = xlPackage.Workbook.Worksheets.Add("DANH MỤC ĐƠN VỊ HÀNH CHÍNH");
        //            // set row
        //            worksheet.DefaultRowHeight = 16;

        //            // SET chiều rộng cho từng cột tương ứng
        //            worksheet.Column(1).Width = 10;
        //            worksheet.Column(2).Width = 50;
        //            worksheet.Column(3).Width = 30;
        //            worksheet.Column(4).Width = 30;
        //            worksheet.Column(5).Width = 30;
        //            worksheet.Column(6).Width = 30;
        //            worksheet.Column(7).Width = 30;

        //            //worksheet.Row(6).Height = 30;
        //            string[] SetColumnItems = { "A", "B", "C", "D", "E", "F", "G","H" };

        //            using (var range = worksheet.Cells["A2:H2"])
        //            {
        //                range.Worksheet.Cells["A1:H1"].Merge = true;
        //                range.Worksheet.Cells["A1:H1"].Value = "DANH MỤC ĐƠN VỊ HÀNH CHÍNH";
        //                range.Worksheet.Cells["A1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                range.Worksheet.Cells["A1:H1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                range.Worksheet.Cells["A1:H1"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
        //                range.Worksheet.Cells["A1:H1"].Style.Font.Color.SetColor(Color.Black);
        //                range.Worksheet.Cells["A1:H1"].Style.Font.Bold = true;
        //            }


        //            using (var range = worksheet.Cells["A3:H3"])
        //            {
        //                range.Worksheet.Cells["A3:H3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        //                range.Worksheet.Cells["A3:H3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                range.Worksheet.Cells["A3:H3"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
        //                range.Worksheet.Cells["A3:H3"].Style.Font.SetFromFont(new Font("Times New Roman", 12));
        //                range.Worksheet.Cells["A3:H3"].Style.Font.Color.SetColor(Color.Black);
        //                range.Worksheet.Cells["A3:H3"].Style.Font.Bold = true;
        //                range.Worksheet.Cells["A3:H3"].Style.WrapText = true;

        //                range.Worksheet.Cells["A3:H3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        //                range.Worksheet.Cells["A3:H3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                range.Worksheet.Cells["A3:H3"].Style.Font.Bold = true;


        //                range.Worksheet.Cells["A3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        //                range.Worksheet.Cells["A3"].Value = "STT";

        //                range.Worksheet.Cells["B3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        //                range.Worksheet.Cells["B3"].Value = "MÃ ĐƠN VỊ";

        //                range.Worksheet.Cells["C3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        //                range.Worksheet.Cells["C3"].Value = $"Số lượng khách hàng{Environment.NewLine}(2)";// "Số lượng khách hàng";

        //                range.Worksheet.Cells["D3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        //                range.Worksheet.Cells["D3"].Value = $"Doanh Thu theo giá niêm yết{Environment.NewLine}(3)";// "Doanh Thu theo giá niêm yết";

        //                range.Worksheet.Cells["E3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        //                range.Worksheet.Cells["E4"].Value = $"Miễn giảm{Environment.NewLine}(4)";// "Miễn giảm ";

        //                range.Worksheet.Cells["F3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        //                range.Worksheet.Cells["F3"].Value = $"BHYT chi trả{Environment.NewLine}(5)";// "BHYT chi trả";

        //                range.Worksheet.Cells["G3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        //                range.Worksheet.Cells["G3"].Value = $"Doanh thu thực tế thu được từ khách hàng{Environment.NewLine}(6)";// "Doanh thu thực tế thu được từ khách hàng";


        //                int index = 5;
        //                int stt = 1;
        //                foreach (var item in datas.ToList())
        //                {
        //                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
        //                    {
        //                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
        //                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
        //                        //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
        //                    }



        //                    worksheet.Cells["A" + index].Value = stt++;
        //                    worksheet.Cells["A" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


        //                    worksheet.Cells["B" + index].Value = item.NguonKhachHang;
        //                    worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

        //                    worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                    worksheet.Cells["C" + index].Value = item.SoLuongKhachHang;

        //                    worksheet.Cells["D" + index].Value = item.DoanhThuTheoGiaNiemYet;
        //                    worksheet.Cells["E" + index].Value = item.MienGiam;

        //                    worksheet.Cells["F" + index].Value = item.BaoHiemChiTra;
        //                    worksheet.Cells["G" + index].Value = item.DoanhThuThucTeDuocThuTuKhachHang;
        //                    worksheet.Cells["G" + index].Style.Font.Bold = true;




        //                    // FOR MAT D -> k
        //                    //worksheet.Cells["C" + index].Style.Numberformat.Format = "#,##0.00";
        //                    worksheet.Cells["D" + index].Style.Numberformat.Format = "#,##0.00";
        //                    worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";

        //                    worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
        //                    worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";


        //                    index++;
        //                }
        //                if (datas.Any())
        //                {
        //                    var total = new SumTongHopDoanhThuTheoNguonBenhNhanGridVo
        //                    {
        //                        TotalSoLuongKhachHang = datas.Sum(o => o.SoLuongKhachHang),
        //                        TotalDoanhThuTheoGiaNiemYet = datas.Sum(o => o.DoanhThuTheoGiaNiemYet),
        //                        TotalMienGiam = datas.Sum(o => o.MienGiam),
        //                        TotalBaoHiemChiTra = datas.Sum(o => o.BaoHiemChiTra),
        //                        TotalDoanhThuThucTeDuocThuTuKhachHang = datas.Sum(o => o.DoanhThuThucTeDuocThuTuKhachHang)
        //                    };
        //                    for (int ii = 0; ii < SetColumnItems.Length; ii++)
        //                    {
        //                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;

        //                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
        //                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.SetFromFont(new Font("Times New Roman", 11));
        //                        worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Color.SetColor(Color.Black);
        //                        //worksheet.Cells[SetColumnItems[ii].ToString() + index].Style.Font.Bold = true;
        //                    }

        //                    worksheet.Cells["A" + index].Style.Font.Bold = true;

        //                    worksheet.Cells["A" + index].Value = "";

        //                    worksheet.Cells["B" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    worksheet.Cells["B" + index].Style.Font.Bold = true;
        //                    worksheet.Cells["B" + index].Value = "Cộng";

        //                    worksheet.Cells["C" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                    worksheet.Cells["C" + index].Style.Font.Bold = true;
        //                    worksheet.Cells["C" + index].Value = total.TotalSoLuongKhachHang;

        //                    worksheet.Cells["D" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                    worksheet.Cells["D" + index].Style.Font.Bold = true;
        //                    worksheet.Cells["D" + index].Value = total.TotalDoanhThuTheoGiaNiemYet;

        //                    worksheet.Cells["E" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                    worksheet.Cells["E" + index].Style.Font.Bold = true;
        //                    worksheet.Cells["E" + index].Value = total.TotalMienGiam;

        //                    worksheet.Cells["F" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                    worksheet.Cells["F" + index].Style.Font.Bold = true;
        //                    worksheet.Cells["F" + index].Value = total.TotalBaoHiemChiTra;

        //                    worksheet.Cells["G" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                    worksheet.Cells["G" + index].Style.Font.Bold = true;
        //                    worksheet.Cells["G" + index].Value = total.TotalDoanhThuThucTeDuocThuTuKhachHang;


        //                    // FOR MAT D -> k
        //                    //worksheet.Cells["C" + index].Style.Numberformat.Format = "#,##0.00";
        //                    worksheet.Cells["D" + index].Style.Numberformat.Format = "#,##0.00";
        //                    worksheet.Cells["E" + index].Style.Numberformat.Format = "#,##0.00";

        //                    worksheet.Cells["F" + index].Style.Numberformat.Format = "#,##0.00";
        //                    worksheet.Cells["G" + index].Style.Numberformat.Format = "#,##0.00";
        //                    worksheet.Cells["H" + index].Style.Numberformat.Format = "#,##0.00";
        //                    worksheet.Cells["I" + index].Style.Numberformat.Format = "#,##0.00";

        //                    worksheet.Cells["J" + index].Style.Numberformat.Format = "#,##0.00";
        //                    worksheet.Cells["K" + index].Style.Numberformat.Format = "#,##0.00";

        //                }
        //            }





        //            xlPackage.Save();
        //        }
        //        return stream.ToArray();
        //    }

        //}
    }
}
