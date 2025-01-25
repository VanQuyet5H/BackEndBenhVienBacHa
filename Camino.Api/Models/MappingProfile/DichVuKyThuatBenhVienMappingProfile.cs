using System.Collections.Generic;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DichVuKhamBenhBenhViens;
using Camino.Api.Models.DichVuKyThuatBenhVien;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using System.Linq;
using Camino.Api.Models.DichVuGiuongBenhVien;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.DichVuGiuongBenhVien;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Newtonsoft.Json;
using static Camino.Core.Domain.Enums;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class DichVuKyThuatBenhVienMappingProfile : Profile
    {
        public DichVuKyThuatBenhVienMappingProfile()
        {
            CreateMap<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien, DichVuKyThuatBenhVienViewModel>().IgnoreAllNonExisting()
                .ForMember(d => d.DichVuKyThuatBenhVienGiaBaoHiems, o => o.MapFrom(s => s.DichVuKyThuatBenhVienGiaBaoHiems))
                .ForMember(d => d.DichVuKyThuatVuBenhVienGiaBenhViens, o => o.MapFrom(s => s.DichVuKyThuatVuBenhVienGiaBenhViens))
                .ForMember(d => d.DinhMucDuocPhamVTYTTrongDichVus, o => o.MapFrom(s => s.DichVuKyThuatBenhVienDinhMucDuocPhamVatTus))
                .ForMember(d => d.LoaiPhauThuatThuThuatId, o => o.MapFrom(s => EnumHelper.GetValueFromDescription<LoaiPTTT>(s.LoaiPhauThuatThuThuat)))
                .AfterMap((s, d) =>
                {
                    d.DichVuKyThuatModelText = s.DichVuKyThuat?.TenChung;//s.DichVuKyThuat == null ? null : (s.Ma + " - " + s.Ten);
                    //d.KhoaPhongIds = s.DichVuKyThuatBenhVienNoiThucHiens.Where(x => x.KhoaPhongId != null).Select(x => x.KhoaPhongId.Value).ToList();
                    //d.PhongBenhVienIds = s.DichVuKyThuatBenhVienNoiThucHiens.Where(x => x.PhongBenhVienId != null).Select(x => x.PhongBenhVienId.Value).ToList();
                    d.AnhXa = s.DichVuKyThuatId != null;
                    d.TenNhomDichVuBenhVien = s.NhomDichVuBenhVien?.Ten;//s.NhomDichVuBenhVien != null ? s.NhomDichVuBenhVien.Ma + " - " + s.NhomDichVuBenhVien.Ten : null;
                    GetNoiThucHien(s,d);
                    
                    // xử lý nơi thực hiện ưu tiên
                    var noiThucHienUuTienNguoiDungThietLap = s.DichVuKyThuatBenhVienNoiThucHienUuTiens.FirstOrDefault(x => x.LoaiNoiThucHienUuTien == Enums.LoaiNoiThucHienUuTien.NguoiDung);
                    if (noiThucHienUuTienNguoiDungThietLap != null)
                    {
                        d.NoiThucHienUuTienId = noiThucHienUuTienNguoiDungThietLap.PhongBenhVienId;
                        d.TenNoiThucHienUuTien = noiThucHienUuTienNguoiDungThietLap.PhongBenhVien.Ten; //noiThucHienUuTienNguoiDungThietLap.PhongBenhVien.Ma + " - " + noiThucHienUuTienNguoiDungThietLap.PhongBenhVien.Ten;
                    }

                    d.LaVacxin = s.DichVuKyThuatBenhVienTiemChungs.Any();
                    if (d.LaVacxin != true)
                    {
                        d.DichVuKyThuatBenhVienTiemChung = new DichVuKyThuatBenhVienTiemChungViewModel();
                    }
                    else
                    {
                        d.DichVuKyThuatBenhVienTiemChung = s.DichVuKyThuatBenhVienTiemChungs.OrderByDescending(x => x.Id).First().ToModel<DichVuKyThuatBenhVienTiemChungViewModel>();
                    }
                    // xử lý get data DichVuKyThuatBenhVienDinhMucDuocPhamVatTus
                    //todo
                    if (s.DichVuKyThuatBenhVienDinhMucDuocPhamVatTus.Any())
                    {
                        d.DinhMucDuocPhamVTYTTrongDichVus= s.DichVuKyThuatBenhVienDinhMucDuocPhamVatTus.Select(dd => new  DinhMucDuocPhamVTYTTrongDichVuViewModel(){ 
                            DichVuKyThuatBenhVienId = dd.DichVuKyThuatBenhVienId,
                            Id = dd.Id,
                            DuocPhamBenhVienId = dd.DuocPhamBenhVienId,
                            VatTuBenhVienId = dd.VatTuBenhVienId,
                            SoLuong = dd.SoLuong,
                            KhongTinhPhi = dd.KhongTinhPhi
                        }).ToList();
                      
                    }
                });
            CreateMap<DichVuKyThuatBenhVienViewModel, Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien>().IgnoreAllNonExisting()
                .ForMember(d => d.DichVuKyThuatBenhVienGiaBaoHiems, o => o.Ignore())
                .ForMember(d => d.DichVuKyThuatVuBenhVienGiaBenhViens, o => o.Ignore())
                .ForMember(d => d.DichVuKyThuatBenhVienNoiThucHiens, o => o.Ignore())
                .ForMember(d => d.DichVuKyThuat, o => o.Ignore())
                .ForMember(d => d.DichVuKyThuatBenhVienDinhMucDuocPhamVatTus, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    AddOrUpdateBaoHiem(s, d);
                    AddOrUpdateBenhVien(s, d);
                    AddOrUpdateNoiThucHien(s,d);
                    AddOrUpdateVacxin(s, d);
                    AddOrUpdateDichVuKyThuatBenhVienDinhMucDuocPhamVatTu(s, d);
                    // xử lý nơi thực hiện ưu tiên
                    if (s.NoiThucHienUuTienId != null && s.NoiThucHienUuTienId != 0)
                    {
                        var noiThucHienUuTienNguoiDungThietLap = d.DichVuKyThuatBenhVienNoiThucHienUuTiens.FirstOrDefault(x => x.LoaiNoiThucHienUuTien == Enums.LoaiNoiThucHienUuTien.NguoiDung);
                        if (noiThucHienUuTienNguoiDungThietLap != null)
                        {
                            noiThucHienUuTienNguoiDungThietLap.PhongBenhVienId = s.NoiThucHienUuTienId.Value;
                        }
                        else
                        {
                            noiThucHienUuTienNguoiDungThietLap = new DichVuKyThuatBenhVienNoiThucHienUuTien()
                            {
                                LoaiNoiThucHienUuTien = Enums.LoaiNoiThucHienUuTien.NguoiDung,
                                PhongBenhVienId = s.NoiThucHienUuTienId.Value
                            };
                            d.DichVuKyThuatBenhVienNoiThucHienUuTiens.Add(noiThucHienUuTienNguoiDungThietLap);
                        }
                    }
                    else
                    {
                        var noiThucHienUuTienNguoiDungThietLap = d.DichVuKyThuatBenhVienNoiThucHienUuTiens.FirstOrDefault(x => x.LoaiNoiThucHienUuTien == Enums.LoaiNoiThucHienUuTien.NguoiDung);
                        if (noiThucHienUuTienNguoiDungThietLap != null)
                        {
                            noiThucHienUuTienNguoiDungThietLap.WillDelete = true;
                        }
                    }
                });

            CreateMap<DichVuKyThuatBenhVienGiaBaoHiem, DichVuKyThuatBenhVienGiaBaoHiemViewModel>().IgnoreAllNonExisting().ForMember(d => d.DichVuKyThuatBenhVien, o => o.Ignore());
            CreateMap<DichVuKyThuatBenhVienGiaBaoHiemViewModel, DichVuKyThuatBenhVienGiaBaoHiem>().IgnoreAllNonExisting()
                 .ForMember(d => d.DichVuKyThuatBenhVien, o => o.Ignore());

            CreateMap<DichVuKyThuatBenhVienGiaBenhVien, DichVuKyThuatVuBenhVienGiaBenhVienViewModel>().IgnoreAllNonExisting()
                .ForMember(d => d.DichVuKyThuatBenhVien, o => o.Ignore())
                .ForMember(d => d.NhomGiaDichVuKyThuatBenhVien, o => o.MapFrom(s => s.NhomGiaDichVuKyThuatBenhVien));
            CreateMap<DichVuKyThuatVuBenhVienGiaBenhVienViewModel, DichVuKyThuatBenhVienGiaBenhVien>().IgnoreAllNonExisting()
                 .ForMember(d => d.DichVuKyThuatBenhVien, o => o.Ignore())
                 .ForMember(d => d.NhomGiaDichVuKyThuatBenhVien, o => o.Ignore());

            CreateMap<NhomGiaDichVuKyThuatBenhVien, NhomGiaDichVuKyThuatBenhVienViewModel>().IgnoreAllNonExisting();
                 //.ForMember(d => d.DichVuKyThuatVuBenhVienGiaBenhViens, o => o.Ignore());
            CreateMap<NhomGiaDichVuKyThuatBenhVienViewModel, NhomGiaDichVuKyThuatBenhVien>().IgnoreAllNonExisting()
                .ForMember(d => d.DichVuKyThuatVuBenhVienGiaBenhViens, o => o.Ignore());

            CreateMap<DichVuKyThuatBenhVienGridVo, DichVuKyThuatBenhVienExportExcel>().IgnoreAllNonExisting();

            
            CreateMap<DichVuKyThuatBenhVienDinhMucDuocPhamVatTu, DinhMucDuocPhamVTYTTrongDichVuViewModel>().IgnoreAllNonExisting();
            CreateMap<DinhMucDuocPhamVTYTTrongDichVuViewModel, DichVuKyThuatBenhVienDinhMucDuocPhamVatTu>().IgnoreAllNonExisting()
            .ForMember(d => d.DichVuKyThuatBenhVien, o => o.Ignore());
        }
        private void AddOrUpdateBaoHiem(DichVuKyThuatBenhVienViewModel s, Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien d)
        {

            foreach (var model in s.DichVuKyThuatBenhVienGiaBaoHiems)
            {
                if (model.Id == 0)
                {
                    var newEntity = new DichVuKyThuatBenhVienGiaBaoHiem();
                    d.DichVuKyThuatBenhVienGiaBaoHiems.Add(model.ToEntity(newEntity));
                }
                else
                {

                    var result = d.DichVuKyThuatBenhVienGiaBaoHiems.Single(c => c.Id == model.Id);
                    result = model.ToEntity(result);
                    //result.DichVuKyThuatBenhVien = d;
                }
            }

            foreach (var model in d.DichVuKyThuatBenhVienGiaBaoHiems)
            {
                if (model.Id != 0)
                {
                    var countModel = s.DichVuKyThuatBenhVienGiaBaoHiems.Where(x => x.Id == model.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        model.WillDelete = true;
                    }

                }
            }
        }
        private void AddOrUpdateBenhVien(DichVuKyThuatBenhVienViewModel s, Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien d)
        {


            foreach (var model in s.DichVuKyThuatVuBenhVienGiaBenhViens)
            {
                if (model.Id == 0)
                {
                    var newEntity = new DichVuKyThuatBenhVienGiaBenhVien();
                    d.DichVuKyThuatVuBenhVienGiaBenhViens.Add(model.ToEntity(newEntity));
                }
                else
                {

                    var result = d.DichVuKyThuatVuBenhVienGiaBenhViens.Single(c => c.Id == model.Id);

                    result = model.ToEntity(result);
                    result.DichVuKyThuatBenhVien = d;

                }
            }
            foreach (var model in d.DichVuKyThuatVuBenhVienGiaBenhViens)
            {
                int count = 0;
                if (model.Id != 0)
                {
                    var countModel = s.DichVuKyThuatVuBenhVienGiaBenhViens.Where(x => x.Id == model.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        model.WillDelete = true;

                        for (int i = 0; i < d.DichVuKyThuatVuBenhVienGiaBenhViens.OrderByDescending(p => p.Id).Where(x => x.Id < model.Id && x.NhomGiaDichVuKyThuatBenhVienId == model.NhomGiaDichVuKyThuatBenhVienId && x.WillDelete == false).Count(); i++)
                        {
                            if (i != 0)
                            {
                                break;
                            }
                            else
                            {
                                d.DichVuKyThuatVuBenhVienGiaBenhViens.Where(x => x.Id < model.Id && x.NhomGiaDichVuKyThuatBenhVienId == model.NhomGiaDichVuKyThuatBenhVienId && x.WillDelete == false).ToList()[i].DenNgay = null;
                            }
                        }
                    }

                }
            }
            foreach (var model in d.DichVuKyThuatBenhVienGiaBaoHiems)
            {
                int count = 0;
                if (model.Id != 0)
                {
                    var countModel = s.DichVuKyThuatBenhVienGiaBaoHiems.Where(x => x.Id == model.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        model.WillDelete = true;

                        for (int i = 0; i < d.DichVuKyThuatBenhVienGiaBaoHiems.OrderByDescending(p => p.Id).Where(x => x.Id < model.Id && x.WillDelete == false).Count(); i++)
                        {
                            if (i != 0)
                            {
                                break;
                            }
                            else
                            {
                                d.DichVuKyThuatBenhVienGiaBaoHiems.Where(x => x.Id < model.Id && x.WillDelete == false).ToList()[i].DenNgay = null;
                            }
                        }
                    }

                }
            }

        }
        private void AddOrUpdateNoiThucHien(DichVuKyThuatBenhVienViewModel s, Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien d)
        {
            // kiểm tra thêm nơi thực hiện dv bệnh viện
            foreach (var item in s.NoiThucHienIds)
            {
                var noiThucHien = JsonConvert.DeserializeObject<ItemNoiThucHienDichVuBenhVienVo>(item);
                if (!d.DichVuKyThuatBenhVienNoiThucHiens.Any(x =>
                    (x.KhoaPhongId != null && noiThucHien.PhongId == null && x.KhoaPhongId == noiThucHien.KhoaId)
                    || (x.KhoaPhongId == null && noiThucHien.PhongId != null && x.PhongBenhVienId == noiThucHien.PhongId)))
                {

                    if (noiThucHien.PhongId == null) // nơi thực hiện là khoa
                    {
                        var newEntity = new DichVuKyThuatBenhVienNoiThucHien()
                        {
                            KhoaPhongId = noiThucHien.KhoaId
                        };
                        d.DichVuKyThuatBenhVienNoiThucHiens.Add(newEntity);
                    }
                    else
                    {
                        var newEntity = new DichVuKyThuatBenhVienNoiThucHien()
                        {
                            PhongBenhVienId = noiThucHien.PhongId
                        };
                        d.DichVuKyThuatBenhVienNoiThucHiens.Add(newEntity);
                    }
                }
            }


            // kiểm tra xóa nơi thực hiện
            if (s.Id != 0)
            {
                var willDelete = true;
                foreach (var item in d.DichVuKyThuatBenhVienNoiThucHiens.Where(x => x.Id != 0))
                {
                    willDelete = true;
                    foreach (var noiThucHien in s.NoiThucHienIds)
                    {
                        var noiThucHienObj = JsonConvert.DeserializeObject<ItemNoiThucHienDichVuBenhVienVo>(noiThucHien);
                        if ((item.KhoaPhongId != null && noiThucHienObj.PhongId == null && noiThucHienObj.KhoaId == item.KhoaPhongId)  // khoa
                            || (item.PhongBenhVienId != null && noiThucHienObj.PhongId != null && noiThucHienObj.PhongId == item.PhongBenhVienId)) // phòng
                        {
                            willDelete = false;
                            break;
                        }
                    }
                    item.WillDelete = willDelete;
                }
            }
        }

        private void GetNoiThucHien(Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien d, DichVuKyThuatBenhVienViewModel s)
        {
            var templateKeyId = "\"KhoaId\": {0}, \"PhongId\": {1}";
            var lstKhoa = new List<long>();
            foreach (var noiThucHien in d.DichVuKyThuatBenhVienNoiThucHiens)
            {
                if (noiThucHien.PhongBenhVienId == null)
                {
                    lstKhoa.Add(noiThucHien.KhoaPhongId.Value);
                    s.NoiThucHienIds.Add("{" + string.Format(templateKeyId, noiThucHien.KhoaPhongId, "\"\"") + "}");
                }
                else
                {
                    lstKhoa.Add(noiThucHien.PhongBenhVien.KhoaPhongId);
                    s.NoiThucHienIds.Add("{" + string.Format(templateKeyId, noiThucHien.PhongBenhVien.KhoaPhongId, noiThucHien.PhongBenhVienId) + "}");
                }
            }
            s.KhoaPhongIds = lstKhoa.Distinct().ToList();
        }

        private void AddOrUpdateVacxin(DichVuKyThuatBenhVienViewModel s, Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien d)
        {
            // mỗi dvkt bệnh viện thì chỉ tạo 1 DVKTBVTiemChung (chỉ có 1 vacxin)
            if (s.LaVacxin == true)
            {
                // kiểm tra thêm
                if (s.DichVuKyThuatBenhVienTiemChung.Id == 0)
                {
                    var newEntity = new DichVuKyThuatBenhVienTiemChung();
                    d.DichVuKyThuatBenhVienTiemChungs.Add(s.DichVuKyThuatBenhVienTiemChung.ToEntity(newEntity));
                }
                else
                {
                    var result = d.DichVuKyThuatBenhVienTiemChungs.Single(c => c.Id == s.DichVuKyThuatBenhVienTiemChung.Id);
                    result = s.DichVuKyThuatBenhVienTiemChung.ToEntity(result);
                }

                // kiểm tra xóa vacxin
                foreach (var item in d.DichVuKyThuatBenhVienTiemChungs.Where(x => x.Id != s.DichVuKyThuatBenhVienTiemChung.Id))
                {
                    item.WillDelete = true;
                }
            }
            else
            {
                // kiểm tra xóa vacxin
                foreach (var item in d.DichVuKyThuatBenhVienTiemChungs.Where(x => x.Id != 0))
                {
                    item.WillDelete = true;
                }
            }
        }

        private void AddOrUpdateDichVuKyThuatBenhVienDinhMucDuocPhamVatTu(DichVuKyThuatBenhVienViewModel s, Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien d)
        {

            foreach (var model in s.DinhMucDuocPhamVTYTTrongDichVus)
            {
                if (model.Id == 0)
                {
                    //var newEntity = new DichVuKyThuatBenhVienDinhMucDuocPhamVatTu();
                    var newEntity = model.ToEntity <DichVuKyThuatBenhVienDinhMucDuocPhamVatTu>();
                    d.DichVuKyThuatBenhVienDinhMucDuocPhamVatTus.Add(model.ToEntity(newEntity));
                }
                else
                {

                    var result = d.DichVuKyThuatBenhVienDinhMucDuocPhamVatTus.Single(c => c.Id == model.Id);
                    result = model.ToEntity(result);
                    //result.DichVuKyThuatBenhVien = d;
                }
            }

            foreach (var model in d.DichVuKyThuatBenhVienDinhMucDuocPhamVatTus)
            {
                if (model.Id != 0)
                {
                    var countModel = s.DinhMucDuocPhamVTYTTrongDichVus.Where(x => x.Id == model.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        model.WillDelete = true;
                    }

                }
            }
        }
    }
}