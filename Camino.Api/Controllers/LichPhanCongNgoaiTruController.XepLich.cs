using Camino.Api.Auth;
using Camino.Api.Models.Error;
using Camino.Api.Models.LichKhoaPhong;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.LichPhanCongNgoaiTrus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Camino.Api.Controllers
{
    public partial class LichPhanCongNgoaiTruController
    {
        #region CRUD
        [HttpPost("GetPhongByKhoaId")]
        public async Task<ActionResult<LichKhoaPhongViewModel>> GetPhongByKhoaId(long KhoaPhongId, DateTime fromDate, DateTime toDate)
        {
            var ListModel = await _phongBenhVienService.GetListPhongBenhVienByKhoaPhongId(KhoaPhongId);
            var listPhong = ListModel.Select(p => p.Id).ToList();
            if (!listPhong.Any())
                return null;
            var listLichPhanCong = await _lichPhanCongNgoaiTruService.GetListLichPhanCong(listPhong, fromDate, toDate);
            if (listLichPhanCong.Any())
            {
                List<LichKhoaPhongViewModel> Phongs = new List<LichKhoaPhongViewModel>();
                await CreateLichNhanVien(Phongs, KhoaPhongId, listLichPhanCong, false, fromDate, toDate);
                Phongs.First().TenKhoa = ListModel.First()?.KhoaPhong?.Ten ?? "";
                return Ok(Phongs);
            }
            else
            {
                List<LichKhoaPhongViewModel> Phongs = new List<LichKhoaPhongViewModel>();
                await CreateLichNhanVienDefault(Phongs, KhoaPhongId);
                if (!Phongs.Any())
                {
                    return null;
                }
                Phongs.First().TenKhoa = ListModel.First()?.KhoaPhong?.Ten ?? "";
                return Ok(Phongs);
            }
        }

        [HttpPost("GetDataLastWeek")]
        public async Task<ActionResult<LichKhoaPhongViewModel>> GetDataLastWeek(long KhoaPhongId, DateTime fromDate, DateTime toDate)
        {
            var listPhong = await _phongBenhVienService.GetListPhongBenhVien(KhoaPhongId);
            if (listPhong == null)
            {
                return null;
            }
            var listLichPhanCong = await _lichPhanCongNgoaiTruService.GetListLichLastWeek(listPhong);
            if (listLichPhanCong == null)
            {
                return null;
            }
            if (listLichPhanCong.Any())
            {
                List<LichKhoaPhongViewModel> Phongs = new List<LichKhoaPhongViewModel>();
                await CreateLichNhanVien(Phongs, KhoaPhongId, listLichPhanCong, true, fromDate, toDate);
                return Ok(Phongs);
            }
            else
            {
                List<LichKhoaPhongViewModel> Phongs = new List<LichKhoaPhongViewModel>();
                await CreateLichNhanVienDefault(Phongs, KhoaPhongId);
                return Ok(Phongs);
            }
        }

        [HttpPost("XepLichPhanCong")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucLichPhanCongNgoaiTru)]
        public async Task<ActionResult> XepLichPhanCong([FromBody] PhongsViewModel lichKhoaPhongViewModel)
        {
            var listModel = new List<LichPhanCongNgoaiTru>();
            var listModelUpdate = new List<LichPhanCongNgoaiTru>();
            var listModelDelete = new List<LichPhanCongNgoaiTru>();
            foreach (var item in lichKhoaPhongViewModel.Phong)
            {
                if (item.BacSis.Any())
                {
                    listModel.AddRange(item.BacSis.Where(p => p.NhanVienId != 0 && p.Id == 0).Select(p => new LichPhanCongNgoaiTru
                    {
                        PhongNgoaiTruId = item.PhongBenhVienId,
                        NhanVienId = p.NhanVienId,
                        NgayPhanCong = GetDayOfWeekToDateNow(p.Thu, item.ToDate, (7 * 0)),
                        LoaiThoiGianPhanCong = item.CaTrucInt,
                    }));
                    listModelUpdate.AddRange(item.BacSis.Where(p => p.NhanVienId != 0 && p.Id != 0).Select(p => new LichPhanCongNgoaiTru
                    {
                        Id = p.Id,
                        PhongNgoaiTruId = item.PhongBenhVienId,
                        NhanVienId = p.NhanVienId,
                        NgayPhanCong = GetDayOfWeekToDateNow(p.Thu, item.ToDate, (7 * 0)),
                        LoaiThoiGianPhanCong = item.CaTrucInt,
                    }));
                    listModelDelete.AddRange(item.BacSis.Where(p => p.NhanVienId == 0 && p.Id != 0).Select(p => new LichPhanCongNgoaiTru
                    {
                        Id = p.Id,
                        PhongNgoaiTruId = item.PhongBenhVienId,
                        NhanVienId = p.NhanVienId,
                        NgayPhanCong = p.NgayPhanCong,
                        LoaiThoiGianPhanCong = p.LoaiThoiGianPhanCong
                    }));
                }
                if (item.YTas.Any())
                {
                    listModel.AddRange(item.YTas.Where(p => p.NhanVienId != 0 && p.Id == 0).Select(p => new LichPhanCongNgoaiTru
                    {
                        PhongNgoaiTruId = item.PhongBenhVienId,
                        NhanVienId = p.NhanVienId,
                        NgayPhanCong = GetDayOfWeekToDateNow(p.Thu, item.ToDate, (7 * 0)),
                        LoaiThoiGianPhanCong = item.CaTrucInt
                    }));
                    listModelUpdate.AddRange(item.YTas.Where(p => p.NhanVienId != 0 && p.Id != 0).Select(p => new LichPhanCongNgoaiTru
                    {
                        Id = p.Id,
                        PhongNgoaiTruId = item.PhongBenhVienId,
                        NhanVienId = p.NhanVienId,
                        NgayPhanCong = GetDayOfWeekToDateNow(p.Thu, item.ToDate, (7 * 0)),
                        LoaiThoiGianPhanCong = item.CaTrucInt
                    }));
                    listModelDelete.AddRange(item.YTas.Where(p => p.NhanVienId == 0 && p.Id != 0).Select(p => new LichPhanCongNgoaiTru
                    {
                        Id = p.Id,
                        PhongNgoaiTruId = item.PhongBenhVienId,
                        NhanVienId = p.NhanVienId,
                        NgayPhanCong = p.NgayPhanCong,
                        LoaiThoiGianPhanCong = p.LoaiThoiGianPhanCong
                    }));
                }
                listModelDelete.AddRange(item.NhanVienTrucDelete.Select(p => new LichPhanCongNgoaiTru { Id = p.Id }));
            }
            if (listModel.Count == 0 && listModelUpdate.Count == 0 && listModelDelete.Count == 0)
                throw new ApiException(_localizationService.GetResource("LichPhanCong.NhanVien.Required"), (int)HttpStatusCode.BadRequest);

            _lichPhanNgoaiTruRepository.AutoCommitEnabled = false;
            var entityDict = _lichPhanNgoaiTruRepository.Table.Where(e => listModelUpdate.Select(v => v.Id).Contains(e.Id) || listModelDelete.Select(v => v.Id).Contains(e.Id));
            foreach (var entity in entityDict)
            {
                var modelDelete = listModelDelete.Where(p => p.Id == entity.Id);
                var modelUpdate = listModelUpdate.Where(p => p.Id == entity.Id);
                if (modelDelete.Any())
                {   //wait Delete Lich Phan Cong 
                    _lichPhanNgoaiTruRepository.Delete(entity);
                }
                if (modelUpdate.Any())
                {   // wait Update Lich Phan Cong
                    entity.NhanVienId = modelUpdate.First().NhanVienId;
                    _lichPhanNgoaiTruRepository.Update(entity);
                }
            }
            foreach (var entity in listModel)
            {
                _lichPhanNgoaiTruRepository.Add(entity); // wait them Lich Phan Cong
            }
            if (lichKhoaPhongViewModel.Phong.FirstOrDefault().IsCopy == true) // wait Xoa Lich Phan Tuan Hien Tai Neu Co
            {
                DateTime toDateDel = GetDayOfWeekToDateNow(2, lichKhoaPhongViewModel.Phong.First()?.ToDate ?? DateTime.Now, 0); // t2 
                DateTime fromDateDel = GetDayOfWeekToDateNow(8, lichKhoaPhongViewModel.Phong.First()?.ToDate ?? DateTime.Now, 0); // CN 
                List<long> ids = lichKhoaPhongViewModel.Phong.Select(p => p.PhongBenhVienId).ToList();
                var listLichPhanCongs = await _lichPhanCongNgoaiTruService.GetListLichPhanCong(ids, fromDateDel, toDateDel);
                if (listLichPhanCongs.Any())
                    foreach (var entity in listLichPhanCongs)
                        _lichPhanNgoaiTruRepository.Delete(entity); // wait Delete Lich Phan Cong Khi Chon Rule Copy
            }

            _lichPhanNgoaiTruRepository.Context.SaveChanges(); // ok save 1 lan
            if (lichKhoaPhongViewModel.Phong.FirstOrDefault().IsPrint == true)
            {
                var html = await CreatePrintTemplateAsync(lichKhoaPhongViewModel.Phong);
                return Ok(html);
            }
            return Ok(null);
        }

        [HttpPost("LichPhanCongCopy")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucLichPhanCongNgoaiTru)]
        public async Task<ActionResult> LichPhanCongCopy([FromBody] PhongsViewModel lichKhoaPhongViewModel)
        {
            int intWeek = lichKhoaPhongViewModel.Phong.First()?.CopyForWeek ?? 0;
            var listModel = new List<LichPhanCongNgoaiTru>();
            var listModelUpdate = new List<LichPhanCongNgoaiTru>();
            var listModelDelete = new List<LichPhanCongNgoaiTru>();
            var listModelCopyLich = new List<LichPhanCongNgoaiTru>();

            foreach (var item in lichKhoaPhongViewModel.Phong)
            {
                for (int i = 0; i <= intWeek; i++)
                {
                    if (i == 0)
                    { //update Lich Hien Tai
                        if (item.BacSis.Any())
                        {
                            listModel.AddRange(item.BacSis.Where(p => p.NhanVienId != 0 && p.Id == 0).Select(p => new LichPhanCongNgoaiTru
                            {
                                PhongNgoaiTruId = item.PhongBenhVienId,
                                NhanVienId = p.NhanVienId,
                                NgayPhanCong = GetDayOfWeekToDateNow(p.Thu, item.ToDate, (7 * 0)),
                                LoaiThoiGianPhanCong = item.CaTrucInt,
                            }));
                            listModelUpdate.AddRange(item.BacSis.Where(p => p.NhanVienId != 0 && p.Id != 0).Select(p => new LichPhanCongNgoaiTru
                            {
                                Id = p.Id,
                                PhongNgoaiTruId = item.PhongBenhVienId,
                                NhanVienId = p.NhanVienId,
                                NgayPhanCong = GetDayOfWeekToDateNow(p.Thu, item.ToDate, (7 * 0)),
                                LoaiThoiGianPhanCong = item.CaTrucInt,
                            }));
                            listModelDelete.AddRange(item.BacSis.Where(p => p.NhanVienId == 0 && p.Id != 0).Select(p => new LichPhanCongNgoaiTru
                            {
                                Id = p.Id,
                                PhongNgoaiTruId = item.PhongBenhVienId,
                                NhanVienId = p.NhanVienId,
                                NgayPhanCong = p.NgayPhanCong,
                                LoaiThoiGianPhanCong = p.LoaiThoiGianPhanCong
                            }));
                        }
                        if (item.YTas.Any())
                        {
                            listModel.AddRange(item.YTas.Where(p => p.NhanVienId != 0 && p.Id == 0).Select(p => new LichPhanCongNgoaiTru
                            {
                                PhongNgoaiTruId = item.PhongBenhVienId,
                                NhanVienId = p.NhanVienId,
                                NgayPhanCong = GetDayOfWeekToDateNow(p.Thu, item.ToDate, (7 * 0)),
                                LoaiThoiGianPhanCong = item.CaTrucInt
                            }));
                            listModelUpdate.AddRange(item.YTas.Where(p => p.NhanVienId != 0 && p.Id != 0).Select(p => new LichPhanCongNgoaiTru
                            {
                                Id = p.Id,
                                PhongNgoaiTruId = item.PhongBenhVienId,
                                NhanVienId = p.NhanVienId,
                                NgayPhanCong = GetDayOfWeekToDateNow(p.Thu, item.ToDate, (7 * 0)),
                                LoaiThoiGianPhanCong = item.CaTrucInt
                            }));
                            listModelDelete.AddRange(item.YTas.Where(p => p.NhanVienId == 0 && p.Id != 0).Select(p => new LichPhanCongNgoaiTru
                            {
                                Id = p.Id,
                                PhongNgoaiTruId = item.PhongBenhVienId,
                                NhanVienId = p.NhanVienId,
                                NgayPhanCong = p.NgayPhanCong,
                                LoaiThoiGianPhanCong = p.LoaiThoiGianPhanCong
                            }));
                        }
                    }
                    else
                    {  // copy lich cho intWeek tuan toi

                        if (item.BacSis.Any())
                        {
                            listModelCopyLich.AddRange(item.BacSis.Where(p => p.NhanVienId != 0).Select(p => new LichPhanCongNgoaiTru
                            {
                                Id = 0,
                                PhongNgoaiTruId = item.PhongBenhVienId,
                                NhanVienId = p.NhanVienId,
                                NgayPhanCong = GetDayOfWeekToDateNow(p.Thu, item.ToDate, (7 * i)),
                                LoaiThoiGianPhanCong = item.CaTrucInt,
                            }));
                        }
                        if (item.YTas.Any())
                        {
                            listModelCopyLich.AddRange(item.YTas.Where(p => p.NhanVienId != 0).Select(p => new LichPhanCongNgoaiTru
                            {
                                Id = 0,
                                PhongNgoaiTruId = item.PhongBenhVienId,
                                NhanVienId = p.NhanVienId,
                                NgayPhanCong = GetDayOfWeekToDateNow(p.Thu, item.ToDate, (7 * i)),
                                LoaiThoiGianPhanCong = item.CaTrucInt,
                            }));
                        }
                    }

                }
                listModelDelete.AddRange(item.NhanVienTrucDelete.Select(p => new LichPhanCongNgoaiTru { Id = p.Id }));
            }
            if (listModel.Count == 0 && listModelUpdate.Count == 0 && listModelDelete.Count == 0 && listModelCopyLich.Count == 0)
            {
                throw new ApiException(_localizationService.GetResource("LichPhanCong.NhanVien.Required"), (int)HttpStatusCode.BadRequest);
            }
            if (listModel.Any() || listModelCopyLich.Any())
            {
                if (lichKhoaPhongViewModel.Phong.First().CopyForWeek > 0) // Xoa Lich Phan Cong Cac Tuan Trc Do Neu Co
                {
                    DateTime toDateDel = GetDayOfWeekToDateNow(2, lichKhoaPhongViewModel.Phong.First()?.ToDate ?? DateTime.Now, 7); // t2 tuần tới
                    DateTime fromDateDel = GetDayOfWeekToDateNow(8, lichKhoaPhongViewModel.Phong.First()?.ToDate ?? DateTime.Now, (7 * intWeek)); // CN của intWeek tuần tới
                    List<long> ids = lichKhoaPhongViewModel.Phong.Select(p => p.PhongBenhVienId).ToList();
                    var listLichPhanCongs = await _lichPhanCongNgoaiTruService.GetListLichPhanCong(ids, fromDateDel, toDateDel);
                    //wait Xoa Lich Cu
                    if (listLichPhanCongs.Any())
                        foreach (var entity in listLichPhanCongs)
                            _lichPhanNgoaiTruRepository.Delete(entity);
                    //wait Thêm Lich Hien Tai va Lich CoPy
                    var listEntity = listModelCopyLich.Concat(listModel);
                    if (listEntity.Any())
                        foreach (var entity in listEntity)
                            _lichPhanNgoaiTruRepository.Add(entity);

                }

            }
            //update delete lich Cu
            _lichPhanNgoaiTruRepository.AutoCommitEnabled = false;
            var entityDict = _lichPhanNgoaiTruRepository.Table.Where(e => listModelUpdate.Select(v => v.Id).Contains(e.Id) || listModelDelete.Select(v => v.Id).Contains(e.Id));
            foreach (var entity in entityDict)
            {
                var modelDelete = listModelDelete.Where(p => p.Id == entity.Id);
                var modelUpdate = listModelUpdate.Where(p => p.Id == entity.Id);
                if (modelDelete.Any())
                {
                    _lichPhanNgoaiTruRepository.Delete(entity);
                }
                if (modelUpdate.Any())
                {
                    entity.NhanVienId = modelUpdate.First().NhanVienId;
                    _lichPhanNgoaiTruRepository.Update(entity);
                }
            }
            _lichPhanNgoaiTruRepository.Context.SaveChanges(); // Ok Save 1 lan
            return Ok(null);
        }

        private async Task CreateLichNhanVienDefault(List<LichKhoaPhongViewModel> Phongs, long KhoaPhongId)
        {
            //tao lich khi chua dc xep lich cho cac phong - create 
            var listPhong = await _phongBenhVienService.GetNamePhongBenhVienCreate(KhoaPhongId);
            foreach (var item in listPhong)
            {
                List<NhanVienLichPhanCongViewModel> bs = new List<NhanVienLichPhanCongViewModel>();
                List<NhanVienLichPhanCongViewModel> yta = new List<NhanVienLichPhanCongViewModel>();
                List<NhanVienLichPhanCongViewModel> bschieu = new List<NhanVienLichPhanCongViewModel>();
                List<NhanVienLichPhanCongViewModel> ytachieu = new List<NhanVienLichPhanCongViewModel>();
                for (int i = 2; i <= 8; i++)
                {
                    bs.Add(new NhanVienLichPhanCongViewModel
                    {
                        PhongBenhVienId = item.KeyId,
                        NhanVienId = 0,
                        TenNhanVien = "",
                        NgayPhanCong = DateTime.Now,
                        LoaiThoiGianPhanCong = Enums.EnumLoaiThoiGianPhanCong.Sang,
                        IsBacSi = true,
                        Thu = i,
                    });
                    yta.Add(new NhanVienLichPhanCongViewModel
                    {
                        PhongBenhVienId = item.KeyId,
                        NhanVienId = 0,
                        TenNhanVien = "",
                        NgayPhanCong = DateTime.Now,
                        LoaiThoiGianPhanCong = Enums.EnumLoaiThoiGianPhanCong.Sang,
                        IsBacSi = false,
                        Thu = i,
                    });
                    bschieu.Add(new NhanVienLichPhanCongViewModel
                    {
                        PhongBenhVienId = item.KeyId,
                        NhanVienId = 0,
                        TenNhanVien = "",
                        NgayPhanCong = DateTime.Now,
                        LoaiThoiGianPhanCong = Enums.EnumLoaiThoiGianPhanCong.Chieu,
                        IsBacSi = true,
                        Thu = i,
                    });
                    ytachieu.Add(new NhanVienLichPhanCongViewModel
                    {
                        PhongBenhVienId = item.KeyId,
                        NhanVienId = 0,
                        TenNhanVien = "",
                        NgayPhanCong = DateTime.Now,
                        LoaiThoiGianPhanCong = Enums.EnumLoaiThoiGianPhanCong.Chieu,
                        IsBacSi = false,
                        Thu = i,
                    });
                }
                //update Moi
                var PhongBuoiSang = new LichKhoaPhongViewModel();
                var PhongBuoiChieu = new LichKhoaPhongViewModel();
                // add Phong Buoi sang
                PhongBuoiSang.NhanVienTrucDelete = new List<NhanVienLichPhanCongViewModel>() { };
                PhongBuoiSang.TenPhong = item.DisplayName;
                PhongBuoiSang.PhongBenhVienId = item.KeyId;
                PhongBuoiSang.CaTruc = Enums.EnumLoaiThoiGianPhanCong.Sang.GetDescription(); ;
                PhongBuoiSang.CaTrucInt = Enums.EnumLoaiThoiGianPhanCong.Sang;
                PhongBuoiSang.BacSis = bs;
                PhongBuoiSang.YTas = yta;
                PhongBuoiSang.IsShowCopyForWeek = true;
                Phongs.Add(PhongBuoiSang);
                // add Phong Buoi Chiều
                PhongBuoiChieu.NhanVienTrucDelete = new List<NhanVienLichPhanCongViewModel>() { };
                PhongBuoiChieu.TenPhong = item.DisplayName;
                PhongBuoiChieu.PhongBenhVienId = item.KeyId;
                PhongBuoiChieu.CaTruc = Enums.EnumLoaiThoiGianPhanCong.Chieu.GetDescription();
                PhongBuoiChieu.CaTrucInt = Enums.EnumLoaiThoiGianPhanCong.Chieu;
                PhongBuoiChieu.BacSis = bschieu;
                PhongBuoiChieu.YTas = ytachieu;
                PhongBuoiChieu.IsShowCopyForWeek = true;
                Phongs.Add(PhongBuoiChieu);
            }
        }

        private async Task CreateLichNhanVien(List<LichKhoaPhongViewModel> Phongs, long KhoaPhongId,
            List<LichPhanCongNgoaiTru> listLichPhanCong, bool isCopy, DateTime fromDate, DateTime toDate)
        {
            List<long> PhongNgoaiTruIds = listLichPhanCong.Select(p => p.PhongNgoaiTruId).Distinct().ToList();
            //tao lich khi da xep lich cho cac phong- xem chi tiet
            var listPhong = await _phongBenhVienService.GetNamePhongBenhVienDetail(KhoaPhongId, PhongNgoaiTruIds);
            foreach (var item in listPhong)
            {
                List<NhanVienLichPhanCongViewModel> bs = new List<NhanVienLichPhanCongViewModel>();
                List<NhanVienLichPhanCongViewModel> yta = new List<NhanVienLichPhanCongViewModel>();
                List<NhanVienLichPhanCongViewModel> bschieu = new List<NhanVienLichPhanCongViewModel>();
                List<NhanVienLichPhanCongViewModel> ytachieu = new List<NhanVienLichPhanCongViewModel>();
                for (int i = 2; i <= 8; i++)
                {
                    bs.Add(new NhanVienLichPhanCongViewModel
                    {
                        PhongBenhVienId = item.KeyId,
                        NhanVienId = 0,
                        TenNhanVien = "",
                        NgayPhanCong = DateTime.Now,
                        LoaiThoiGianPhanCong = Enums.EnumLoaiThoiGianPhanCong.Sang,
                        IsBacSi = true,
                        Thu = i,
                    });
                    yta.Add(new NhanVienLichPhanCongViewModel
                    {
                        PhongBenhVienId = item.KeyId,
                        NhanVienId = 0,
                        TenNhanVien = "",
                        NgayPhanCong = DateTime.Now,
                        LoaiThoiGianPhanCong = Enums.EnumLoaiThoiGianPhanCong.Sang,
                        IsBacSi = false,
                        Thu = i,
                    });
                    bschieu.Add(new NhanVienLichPhanCongViewModel
                    {
                        PhongBenhVienId = item.KeyId,
                        NhanVienId = 0,
                        TenNhanVien = "",
                        NgayPhanCong = DateTime.Now,
                        LoaiThoiGianPhanCong = Enums.EnumLoaiThoiGianPhanCong.Chieu,
                        IsBacSi = true,
                        Thu = i,
                    });
                    ytachieu.Add(new NhanVienLichPhanCongViewModel
                    {
                        PhongBenhVienId = item.KeyId,
                        NhanVienId = 0,
                        TenNhanVien = "",
                        NgayPhanCong = DateTime.Now,
                        LoaiThoiGianPhanCong = Enums.EnumLoaiThoiGianPhanCong.Chieu,
                        IsBacSi = false,
                        Thu = i,
                    });
                }
                //update Moi
                var PhongBuoiSang = new LichKhoaPhongViewModel();
                var PhongBuoiChieu = new LichKhoaPhongViewModel();
                // add Phong Buoi sang
                PhongBuoiSang.IsShowCopyForWeek = listLichPhanCong.Any() ? false : true;
                PhongBuoiSang.NhanVienTrucDelete = new List<NhanVienLichPhanCongViewModel>() { };
                PhongBuoiSang.TenPhong = item.DisplayName;
                PhongBuoiSang.PhongBenhVienId = item.KeyId;
                PhongBuoiSang.CaTruc = Enums.EnumLoaiThoiGianPhanCong.Sang.GetDescription(); ;
                PhongBuoiSang.CaTrucInt = Enums.EnumLoaiThoiGianPhanCong.Sang;
                PhongBuoiSang.BacSis = bs;
                PhongBuoiSang.YTas = yta;
                PhongBuoiSang.IsShowCopyForWeek = false;
                PhongBuoiChieu.IsShowCopyForWeek = false;
                // add Phong Buoi Chiều
                PhongBuoiChieu.NhanVienTrucDelete = new List<NhanVienLichPhanCongViewModel>() { };
                PhongBuoiChieu.TenPhong = item.DisplayName;
                PhongBuoiChieu.PhongBenhVienId = item.KeyId;
                PhongBuoiChieu.CaTruc = Enums.EnumLoaiThoiGianPhanCong.Chieu.GetDescription();
                PhongBuoiChieu.CaTrucInt = Enums.EnumLoaiThoiGianPhanCong.Chieu;
                PhongBuoiChieu.BacSis = bschieu;
                PhongBuoiChieu.YTas = ytachieu;
                if (listLichPhanCong.Any())
                {
                    for (int i = 0; i < listLichPhanCong.Count; i++)
                    {
                        if (listLichPhanCong[i].PhongNgoaiTruId == item.KeyId)
                        {
                            switch (listLichPhanCong[i].LoaiThoiGianPhanCong)
                            {
                                case Enums.EnumLoaiThoiGianPhanCong.Sang:
                                    int thu = GetDayOfWeekToDate(listLichPhanCong[i].NgayPhanCong);
                                    if (checkBacSiOrYTa(listLichPhanCong[i]?.NhanVien?.ChucDanh?.NhomChucDanh.Id ?? 0))
                                    {
                                        PhongBuoiSang.BacSis.RemoveAll(p => p.NhanVienId == 0 && p.IsBacSi == true && p.Thu == thu);
                                        PhongBuoiSang.BacSis.Add(new NhanVienLichPhanCongViewModel
                                        {
                                            Id = (isCopy == false ? listLichPhanCong[i].Id : 0),
                                            PhongBenhVienId = listLichPhanCong[i].PhongNgoaiTruId,
                                            NhanVienId = listLichPhanCong[i].NhanVienId,
                                            TenNhanVien = listLichPhanCong[i].NhanVien.User.HoTen,
                                            NgayPhanCong = (isCopy == false ? listLichPhanCong[i].NgayPhanCong : toDate),
                                            LoaiThoiGianPhanCong = Enums.EnumLoaiThoiGianPhanCong.Sang,
                                            IsBacSi = true,
                                            Thu = thu
                                        });
                                    }
                                    else
                                    {
                                        PhongBuoiSang.YTas.RemoveAll(p => p.NhanVienId == 0 && p.IsBacSi == false && p.Thu == thu);
                                        PhongBuoiSang.YTas.Add(new NhanVienLichPhanCongViewModel
                                        {
                                            Id = (isCopy == false ? listLichPhanCong[i].Id : 0),
                                            PhongBenhVienId = listLichPhanCong[i].PhongNgoaiTruId,
                                            NhanVienId = listLichPhanCong[i].NhanVienId,
                                            TenNhanVien = listLichPhanCong[i].NhanVien.User.HoTen,
                                            NgayPhanCong = (isCopy == false ? listLichPhanCong[i].NgayPhanCong : toDate),
                                            LoaiThoiGianPhanCong = Enums.EnumLoaiThoiGianPhanCong.Sang,
                                            IsBacSi = false,
                                            Thu = thu
                                        });
                                    }
                                    break;
                                case Enums.EnumLoaiThoiGianPhanCong.Chieu:
                                    int thu1 = GetDayOfWeekToDate(listLichPhanCong[i].NgayPhanCong);
                                    if (checkBacSiOrYTa(listLichPhanCong[i]?.NhanVien?.ChucDanh?.NhomChucDanh.Id ?? 0))
                                    {
                                        PhongBuoiChieu.BacSis.RemoveAll(p => p.NhanVienId == 0 && p.IsBacSi == true && p.Thu == thu1);
                                        PhongBuoiChieu.BacSis.Add(new NhanVienLichPhanCongViewModel
                                        {
                                            Id = (isCopy == false ? listLichPhanCong[i].Id : 0),
                                            PhongBenhVienId = listLichPhanCong[i].PhongNgoaiTruId,
                                            NhanVienId = listLichPhanCong[i].NhanVienId,
                                            TenNhanVien = listLichPhanCong[i].NhanVien.User.HoTen,
                                            NgayPhanCong = (isCopy == false ? listLichPhanCong[i].NgayPhanCong : toDate),
                                            LoaiThoiGianPhanCong = Enums.EnumLoaiThoiGianPhanCong.Chieu,
                                            IsBacSi = checkBacSiOrYTa(listLichPhanCong[i]?.NhanVien?.ChucDanh?.NhomChucDanh.Id ?? 0),
                                            Thu = thu1
                                        }); ;
                                    }
                                    else
                                    {
                                        PhongBuoiChieu.YTas.RemoveAll(p => p.NhanVienId == 0 && p.IsBacSi == false && p.Thu == thu1);
                                        PhongBuoiChieu.YTas.Add(new NhanVienLichPhanCongViewModel
                                        {
                                            Id = (isCopy == false ? listLichPhanCong[i].Id : 0),
                                            PhongBenhVienId = listLichPhanCong[i].PhongNgoaiTruId,
                                            NhanVienId = listLichPhanCong[i].NhanVienId,
                                            TenNhanVien = listLichPhanCong[i].NhanVien.User.HoTen,
                                            NgayPhanCong = (isCopy == false ? listLichPhanCong[i].NgayPhanCong : toDate),
                                            LoaiThoiGianPhanCong = Enums.EnumLoaiThoiGianPhanCong.Chieu,
                                            IsBacSi = checkBacSiOrYTa(listLichPhanCong[i]?.NhanVien?.ChucDanh?.NhomChucDanh.Id ?? 0),
                                            Thu = thu1
                                        });
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                Phongs.Add(PhongBuoiSang);
                Phongs.Add(PhongBuoiChieu);
            }
        }

        private async Task<string> CreatePrintTemplateAsync(List<LichKhoaPhongViewModel> Phongs)
        {
            List<long> ids = Phongs.Select(p => p.PhongBenhVienId).ToList();
            var listLichPhanCong = await _lichPhanCongNgoaiTruService.GetListLichPhanCong(ids, Phongs.First()?.FromDate ?? DateTime.Now, Phongs.First()?.ToDate ?? DateTime.Now);
            var TenPhong = Phongs.GroupBy(p => p.PhongBenhVienId).Select(grp => grp.First());
            DateTime dateTimeMon = Phongs.First()?.ToDate ?? DateTime.Now;
            string host = Phongs.First()?.HostingPrint;
            var Templeta = _yeuCauKhamBenhService.GetBodyByName("InLichPhanCongNgoaiTru");
            var data = new PrintWeek
            {
                TenKhoa = Phongs.First()?.TenKhoa,
                HostName = host,
                ToDateFromDate = dateTimeMon.ToString("dd/MM/yyyy") + " - " + dateTimeMon.AddDays(6).ToString("dd/MM/yyyy"),
                NgayThu2 = dateTimeMon.AddDays(0).ToString("dd/MM/yyyy"),
                NgayThu3 = dateTimeMon.AddDays(1).ToString("dd/MM/yyyy"),
                NgayThu4 = dateTimeMon.AddDays(2).ToString("dd/MM/yyyy"),
                NgayThu5 = dateTimeMon.AddDays(3).ToString("dd/MM/yyyy"),
                NgayThu6 = dateTimeMon.AddDays(4).ToString("dd/MM/yyyy"),
                NgayThu7 = dateTimeMon.AddDays(5).ToString("dd/MM/yyyy"),
                NgayThu8 = dateTimeMon.AddDays(6).ToString("dd/MM/yyyy"),
            };
            var html = TemplateHelpper.FormatTemplateWithContentTemplate(Templeta, data);

            var tdNhaVienTempleta = "<td style='text-align: center; border: 1px solid grey;border - right: 0px none black;border - top - width:3px;margin - top:-3px;'>";
            foreach (var item in TenPhong)
            {
                var htmlBuoiSang = "";
                var htmlBuoiChieu = "";
                htmlBuoiSang += "<tr style='border: 1px solid grey;'>"
                                  + "<td style='width: 190px;text-align: center; font-weight: bold;width: 5em;left: 0;top: auto;border: 1px solid grey;border - right: 0px none black;border - top - width:3px;margin - top:-3px;' rowspan='2'>" + item.TenPhong + "</td>"
                                  + "<td style='width: 190px;text-align: center; font-weight: bold;width: 5em;left: 0;top: auto;border: 1px solid grey;border - right: 0px none black;border - top - width:3px;margin - top:-3px;'> Sáng</td>"
                             ;
                htmlBuoiChieu += "<tr style='border: 1px solid grey;'>"
                                  + "<td style='width: 190px;text-align: center; font-weight: bold;width: 5em;left: 0;top: auto;border: 1px solid grey;border - right: 0px none black;border - top - width:3px;margin - top:-3px;'> Chiều</td>"
                            ;
                for (int i = 2; i < 9; i++)
                {
                    string bsSang = tdNhaVienTempleta, ytaSang = tdNhaVienTempleta, bsChieu = tdNhaVienTempleta, ytaChieu = tdNhaVienTempleta;
                    switch (i)
                    {
                        case 2:
                            bsSang += GetTenNV(true, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Sang, DayOfWeek.Monday, item.PhongBenhVienId);
                            ytaSang += GetTenNV(false, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Sang, DayOfWeek.Monday, item.PhongBenhVienId);
                            htmlBuoiSang += bsSang + "</td>" + ytaSang + "</td>";

                            bsChieu += GetTenNV(true, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Chieu, DayOfWeek.Monday, item.PhongBenhVienId);
                            ytaChieu += GetTenNV(false, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Chieu, DayOfWeek.Monday, item.PhongBenhVienId);
                            htmlBuoiChieu += bsChieu + "</td>" + ytaChieu + "</td>";
                            break;
                        case 3:
                            bsSang += GetTenNV(true, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Sang, DayOfWeek.Tuesday, item.PhongBenhVienId);
                            ytaSang += GetTenNV(false, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Sang, DayOfWeek.Tuesday, item.PhongBenhVienId);
                            htmlBuoiSang += bsSang + "</td>" + ytaSang + "</td>";

                            bsChieu += GetTenNV(true, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Chieu, DayOfWeek.Tuesday, item.PhongBenhVienId);
                            ytaChieu += GetTenNV(false, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Chieu, DayOfWeek.Tuesday, item.PhongBenhVienId);
                            htmlBuoiChieu += bsChieu + "</td>" + ytaChieu + "</td>";
                            break;
                        case 4:
                            bsSang += GetTenNV(true, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Sang, DayOfWeek.Wednesday, item.PhongBenhVienId);
                            ytaSang += GetTenNV(false, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Sang, DayOfWeek.Wednesday, item.PhongBenhVienId);
                            htmlBuoiSang += bsSang + "</td>" + ytaSang + "</td>";

                            bsChieu += GetTenNV(true, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Chieu, DayOfWeek.Wednesday, item.PhongBenhVienId);
                            ytaChieu += GetTenNV(false, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Chieu, DayOfWeek.Wednesday, item.PhongBenhVienId);
                            htmlBuoiChieu += bsChieu + "</td>" + ytaChieu + "</td>";
                            break;
                        case 5:
                            bsSang += GetTenNV(true, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Sang, DayOfWeek.Thursday, item.PhongBenhVienId);
                            ytaSang += GetTenNV(false, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Sang, DayOfWeek.Thursday, item.PhongBenhVienId);
                            htmlBuoiSang += bsSang + "</td>" + ytaSang + "</td>";

                            bsChieu += GetTenNV(true, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Chieu, DayOfWeek.Thursday, item.PhongBenhVienId);
                            ytaChieu += GetTenNV(false, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Chieu, DayOfWeek.Thursday, item.PhongBenhVienId);
                            htmlBuoiChieu += bsChieu + "</td>" + ytaChieu + "</td>";
                            break;
                        case 6:
                            bsSang += GetTenNV(true, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Sang, DayOfWeek.Friday, item.PhongBenhVienId);
                            ytaSang += GetTenNV(false, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Sang, DayOfWeek.Friday, item.PhongBenhVienId);
                            htmlBuoiSang += bsSang + "</td>" + ytaSang + "</td>";

                            bsChieu += GetTenNV(true, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Chieu, DayOfWeek.Friday, item.PhongBenhVienId);
                            ytaChieu += GetTenNV(false, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Chieu, DayOfWeek.Friday, item.PhongBenhVienId);
                            htmlBuoiChieu += bsChieu + "</td>" + ytaChieu + "</td>";
                            break;
                        case 7:
                            bsSang += GetTenNV(true, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Sang, DayOfWeek.Saturday, item.PhongBenhVienId);
                            ytaSang += GetTenNV(false, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Sang, DayOfWeek.Saturday, item.PhongBenhVienId);
                            htmlBuoiSang += bsSang + "</td>" + ytaSang + "</td>";

                            bsChieu += GetTenNV(true, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Chieu, DayOfWeek.Saturday, item.PhongBenhVienId);
                            ytaChieu += GetTenNV(false, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Chieu, DayOfWeek.Saturday, item.PhongBenhVienId);
                            htmlBuoiChieu += bsChieu + "</td>" + ytaChieu + "</td>";
                            break;
                        case 8:
                            bsSang += GetTenNV(true, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Sang, DayOfWeek.Sunday, item.PhongBenhVienId);
                            ytaSang += GetTenNV(false, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Sang, DayOfWeek.Sunday, item.PhongBenhVienId);
                            htmlBuoiSang += bsSang + "</td>" + ytaSang + "</td>";

                            bsChieu += GetTenNV(true, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Chieu, DayOfWeek.Sunday, item.PhongBenhVienId);
                            ytaChieu += GetTenNV(false, listLichPhanCong, Enums.EnumLoaiThoiGianPhanCong.Chieu, DayOfWeek.Sunday, item.PhongBenhVienId);
                            htmlBuoiChieu += bsChieu + "</td>" + ytaChieu + "</td>";
                            break;
                        default:
                            break;
                    }
                }
                html += htmlBuoiSang + "</tr>" + htmlBuoiChieu + "</tr>";
            }
            html += "</tbody></table>";
            return html;
        }

        private bool checkBacSiOrYTa(long NhomChucDanhIdNV)
        {
            bool checkBacsi = false;
            int bacsi = (int)Enums.EnumNhomChucDanh.BacSi;
            int BacSiDuPhong = (int)Enums.EnumNhomChucDanh.BacSiDuPhong;
            if ((int)NhomChucDanhIdNV == bacsi || (int)NhomChucDanhIdNV == BacSiDuPhong)
                checkBacsi = true;
            return checkBacsi;
        }

        private DateTime GetDayOfWeekToDateNow(int Thu, DateTime day, int copyWeek)
        {
            DateTime dayOfWeek;
            switch (Thu)
            {
                case 2:
                    dayOfWeek = day.AddDays((int)DayOfWeek.Monday - (int)day.DayOfWeek + copyWeek);
                    break;
                case 3:
                    dayOfWeek = day.AddDays((int)DayOfWeek.Tuesday - (int)day.DayOfWeek + copyWeek);
                    break;
                case 4:
                    dayOfWeek = day.AddDays((int)DayOfWeek.Wednesday - (int)day.DayOfWeek + copyWeek);
                    break;
                case 5:
                    dayOfWeek = day.AddDays((int)DayOfWeek.Thursday - (int)day.DayOfWeek + copyWeek);
                    break;
                case 6:
                    dayOfWeek = day.AddDays((int)DayOfWeek.Friday - (int)day.DayOfWeek + copyWeek);
                    break;
                case 7:
                    dayOfWeek = day.AddDays((int)DayOfWeek.Saturday - (int)day.DayOfWeek + copyWeek);
                    break;
                case 8:
                    dayOfWeek = day.AddDays((int)DayOfWeek.Sunday - (int)day.DayOfWeek + copyWeek + 7);
                    break;
                default:
                    dayOfWeek = DateTime.Now;
                    break;
            }
            return dayOfWeek;
        }

        private int GetDayOfWeekToDate(DateTime day)
        {
            int thu = 2;
            switch (day.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    thu = 2;
                    break;
                case DayOfWeek.Tuesday:
                    thu = 3;
                    break;
                case DayOfWeek.Wednesday:
                    thu = 4;
                    break;
                case DayOfWeek.Thursday:
                    thu = 5;
                    break;
                case DayOfWeek.Friday:
                    thu = 6;
                    break;
                case DayOfWeek.Saturday:
                    thu = 7;
                    break;
                case DayOfWeek.Sunday:
                    thu = 8;
                    break;
                default:
                    break;
            }
            return thu;
        }

        private string GetTenNV(bool isBacSi, List<LichPhanCongNgoaiTru> modelLich, Enums.EnumLoaiThoiGianPhanCong CaTruc, DayOfWeek dayOfWeek, long PhongNgoaiTruID)
        {
            var lisstM = modelLich.Where(p => p.LoaiThoiGianPhanCong == CaTruc
                && checkBacSiOrYTa(p.NhanVien?.ChucDanh?.NhomChucDanhId ?? 0) == isBacSi
                && p.NgayPhanCong.DayOfWeek == dayOfWeek
                && p.PhongNgoaiTruId == PhongNgoaiTruID).Select(p => p.NhanVien.User.HoTen);
            string divTenNV = "";
            foreach (var item in lisstM)
            {
                divTenNV += "<div>" + item + "</div>";
            }


            return divTenNV;
        }
        #endregion
    }

}