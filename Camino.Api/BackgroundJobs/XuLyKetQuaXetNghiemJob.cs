using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Configuration;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.MayXetNghiems;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.XetNghiems;
using Camino.Core.Helpers;
using Camino.Core.Infrastructure;
using Camino.Data;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.BackgroundJobs
{
    [ScopedDependency(ServiceType = typeof(IXuLyKetQuaXetNghiemJob))]
    public class XuLyKetQuaXetNghiemJob : IXuLyKetQuaXetNghiemJob
    {
        private const int SoFileMapToiDa = 10;
        private readonly IRepository<PhienXetNghiem> _phienXetNghiemRepository;
        private readonly IRepository<MayXetNghiem> _mayXetNghiemRepository;
        private readonly LISConfig _lisConfig;
        private readonly ILoggerManager _logger;

        public XuLyKetQuaXetNghiemJob(IRepository<PhienXetNghiem> phienXetNghiemRepository, IRepository<MayXetNghiem> mayXetNghiemRepository, LISConfig lisConfig, ILoggerManager logger)
        {
            _lisConfig = lisConfig;
            _logger = logger;
            _phienXetNghiemRepository = phienXetNghiemRepository;
            _mayXetNghiemRepository = mayXetNghiemRepository;
        }

        public void Run()
        {
            if (Directory.Exists(_lisConfig.ResultFolder))
            {
                var resultFolder = new DirectoryInfo(_lisConfig.ResultFolder);
                var backupFolder = Path.Combine(_lisConfig.ResultFolder, "Backup", DateTime.Now.ToString("yyMMdd"));
                var errorFolder = Path.Combine(_lisConfig.ResultFolder, "Error", DateTime.Now.ToString("yyMMdd"));
                var notMapFolder = Path.Combine(_lisConfig.ResultFolder, "NotMap", DateTime.Now.ToString("yyMMdd"));

                var files = resultFolder.GetFiles().OrderBy(o => o.CreationTime).ToList();
                if (files.Any())
                {
                    var thoiDiemBatDau = DateTime.Now.AddHours((-1) * _lisConfig.PhienXetNghiemSoGioToiDa);
                    var mayXetNghiems = _mayXetNghiemRepository.TableNoTracking.Where(o => o.HieuLuc).ToList();
                    var phienXetNghiems = _phienXetNghiemRepository.Table.Include(o => o.MauXetNghiems)
                        .ThenInclude(o => o.PhienXetNghiem)
                        .Include(o => o.PhienXetNghiemChiTiets).ThenInclude(o => o.KetQuaXetNghiemChiTiets).ThenInclude(o => o.DichVuXetNghiem).ThenInclude(o => o.DichVuXetNghiemKetNoiChiSos)
                        .Where(o=>o.ThoiDiemBatDau >= thoiDiemBatDau && o.ThoiDiemKetLuan == null);
                    var fileMap = 0;
                    foreach (var fileInfo in files)
                    {
                        KetQuaTuMayXetNghiem ketQuaTuMay = null;
                        var mapKetQua = false;
                        try
                        {
                            if (fileInfo.Extension.ToLower().Equals(".csv") && _lisConfig.HumaClotDeviceIds.Contains(fileInfo.Name.Split('_')[0]))
                            {
                                ketQuaTuMay = LISHelper.GetKetQuaTuMayXetNghiemHumaClot(File.ReadAllLines(fileInfo.FullName), mayXetNghiems.First(o=>o.Id.ToString() == fileInfo.Name.Split('_')[0]).Ma);
                            }
                            else
                            {
                                ketQuaTuMay = LISHelper.GetKetQuaTuMayXetNghiem(File.ReadAllLines(fileInfo.FullName), _lisConfig, mayXetNghiems);
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"LIS error {fileInfo.Name}: {ex}");
                            MoveFileToFolder(fileInfo, errorFolder);
                        }
                        if (ketQuaTuMay != null)
                        {
                            var mayXetNghiem = mayXetNghiems.FirstOrDefault(o => o.Ma == ketQuaTuMay.MaMayXetNghiem);
                            if (mayXetNghiem != null)
                            {
                                foreach (var phienXetNghiem in phienXetNghiems.Where(o => o.BarCodeNumber == ketQuaTuMay.BarCodeNumber).OrderByDescending(o => o.Id))
                                {
                                    foreach (var ketQuaTuMayXetNghiemChiTiet in ketQuaTuMay.KetQuaTuMayXetNghiemChiTiets)
                                    {
                                        var ketQuaXetNghiemChiTiet = phienXetNghiem.PhienXetNghiemChiTiets.SelectMany(o => o.KetQuaXetNghiemChiTiets)
                                            .Where(o => o.DichVuXetNghiem.DichVuXetNghiemKetNoiChiSos.Any(cs => cs.HieuLuc && cs.MaChiSo == ketQuaTuMayXetNghiemChiTiet.MaChiSo && cs.MauMayXetNghiemId == mayXetNghiem.MauMayXetNghiemID))
                                            .OrderBy(o => o.LanThucHien).LastOrDefault();
                                        if (ketQuaXetNghiemChiTiet != null && ketQuaXetNghiemChiTiet.DaDuyet != true)
                                        {
                                            MapGiaTriTuMay(ketQuaXetNghiemChiTiet, ketQuaTuMayXetNghiemChiTiet);
                                            ketQuaXetNghiemChiTiet.ThoiDiemNhanKetQua = DateTime.Now;
                                            ketQuaXetNghiemChiTiet.MayXetNghiemId = mayXetNghiem.Id;
                                            ketQuaXetNghiemChiTiet.MauMayXetNghiemId = mayXetNghiem.MauMayXetNghiemID;
                                            ketQuaXetNghiemChiTiet.PhienXetNghiemChiTiet.PhienXetNghiem.ChoKetQua = false;
                                            if (ketQuaXetNghiemChiTiet.PhienXetNghiemChiTiet.ThoiDiemCoKetQua == null)
                                            {
                                                ketQuaXetNghiemChiTiet.PhienXetNghiemChiTiet.ThoiDiemCoKetQua = DateTime.Now;
                                            }

                                            mapKetQua = true;
                                        }
                                    }
                                    if (mapKetQua)
                                    {
                                        break;
                                    }
                                }
                                if (mapKetQua)
                                {
                                    _phienXetNghiemRepository.Context.SaveChanges();
                                    MoveFileToFolder(fileInfo, backupFolder);
                                    fileMap++;
                                    if (fileMap > SoFileMapToiDa) break;
                                }
                                
                            }
                        }
                        //gữi file kq trong 24h
                        if (!mapKetQua && fileInfo.CreationTime < DateTime.Now.AddHours(-24))
                        {
                            MoveFileToFolder(fileInfo, notMapFolder);
                        }

                        //if (ketQuaTuMay != null)
                        //{
                        //    var mayXetNghiem = mayXetNghiems.FirstOrDefault(o => o.Ma == ketQuaTuMay.MaMayXetNghiem);
                        //    var phienXetNghiem = phienXetNghiems.Where(o => o.BarCodeNumber == ketQuaTuMay.BarCodeNumber).OrderBy(o => o.Id).LastOrDefault();
                        //    if (phienXetNghiem != null && mayXetNghiem != null)
                        //    {
                        //        foreach (var ketQuaTuMayXetNghiemChiTiet in ketQuaTuMay.KetQuaTuMayXetNghiemChiTiets)
                        //        {
                        //            var ketQuaXetNghiemChiTiet = phienXetNghiem.PhienXetNghiemChiTiets.SelectMany(o => o.KetQuaXetNghiemChiTiets)
                        //                .Where(o => o.DichVuXetNghiem.DichVuXetNghiemKetNoiChiSos.Any(cs => cs.HieuLuc && cs.MaChiSo == ketQuaTuMayXetNghiemChiTiet.MaChiSo && cs.MauMayXetNghiemId == mayXetNghiem.MauMayXetNghiemID))
                        //                .OrderBy(o => o.LanThucHien).LastOrDefault();
                        //            if (ketQuaXetNghiemChiTiet != null && ketQuaXetNghiemChiTiet.DaDuyet != true)
                        //            {
                        //                MapGiaTriTuMay(ketQuaXetNghiemChiTiet, ketQuaTuMayXetNghiemChiTiet);
                        //                ketQuaXetNghiemChiTiet.ThoiDiemNhanKetQua = DateTime.Now;
                        //                ketQuaXetNghiemChiTiet.MayXetNghiemId = mayXetNghiem.Id;
                        //                ketQuaXetNghiemChiTiet.MauMayXetNghiemId = mayXetNghiem.MauMayXetNghiemID;
                        //                mapKetQua = true;
                        //            }
                        //        }
                        //    }
                        //    if (mapKetQua)
                        //    {
                        //        _phienXetNghiemRepository.Update(phienXetNghiem);
                        //        MoveFileToFolder(fileInfo, backupFolder);
                        //        fileMap++;
                        //        if (fileMap > SoFileMapToiDa) break;
                        //    }
                        //}
                        //if (!mapKetQua && fileInfo.CreationTime < DateTime.Now.AddHours((-1)* _lisConfig.PhienXetNghiemSoGioToiDa))
                        //{
                        //    MoveFileToFolder(fileInfo, notMapFolder);
                        //}
                    }
                }
            }
        }

        private void MapGiaTriTuMay(KetQuaXetNghiemChiTiet ketQuaXetNghiemChiTiet, KetQuaTuMayXetNghiemChiTiet ketQuaTuMayXetNghiemChiTiet)
        {
            var style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
            var culture = CultureInfo.CreateSpecificCulture("en-GB");
            if (ketQuaXetNghiemChiTiet.TiLe != null && !ketQuaXetNghiemChiTiet.TiLe.Value.AlmostEqual(1)
                && double.TryParse(ketQuaTuMayXetNghiemChiTiet.GiaTri, style, culture, out double giaTri) 
                && (giaTri.ToString("0.######").IndexOf('.') < 0 || giaTri.ToString("0.######").IndexOf('.') > giaTri.ToString("0.######").Length - 4))
            {
                if (ketQuaTuMayXetNghiemChiTiet.DonVi.ToLower() == "g/l" && ketQuaXetNghiemChiTiet.DonVi.ToLower() == "g/l")
                {
                    ketQuaXetNghiemChiTiet.GiaTriTuMay = ketQuaTuMayXetNghiemChiTiet.GiaTri;
                }
                else if (ketQuaTuMayXetNghiemChiTiet.DonVi.ToLower() == "g/dl" && ketQuaXetNghiemChiTiet.DonVi.ToLower() == "g/l")
                {
                    ketQuaXetNghiemChiTiet.GiaTriTuMay = (10 * giaTri).ToString("0.##");
                }
                else
                {
                    ketQuaXetNghiemChiTiet.GiaTriTuMay = (ketQuaXetNghiemChiTiet.TiLe * giaTri ?? giaTri).ToString("0.##");
                }
            }
            else
            {
                ketQuaXetNghiemChiTiet.GiaTriTuMay = ketQuaTuMayXetNghiemChiTiet.GiaTri;
            }
            var status = BenhVienHelper.GetStatusForXetNghiem(ketQuaXetNghiemChiTiet.GiaTriMin, ketQuaXetNghiemChiTiet.GiaTriMax,
                ketQuaXetNghiemChiTiet.GiaTriNguyHiemMin, ketQuaXetNghiemChiTiet.GiaTriNguyHiemMax,
                ketQuaXetNghiemChiTiet.GiaTriTuMay);
            if (status != 1)
            {
                ketQuaXetNghiemChiTiet.ToDamGiaTri = true;
            }
            else
            {
                ketQuaXetNghiemChiTiet.ToDamGiaTri = false;
            }
        }

        private void MoveFileToFolder(FileInfo fileInfo, string folder)
        {
            try
            {
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                File.Move(fileInfo.FullName, Path.Combine(folder, fileInfo.Name));
            }
            catch (Exception ex)
            {
                _logger.LogError($"LIS move file error {fileInfo.Name}: {ex}");
            }
        }
    }
}
