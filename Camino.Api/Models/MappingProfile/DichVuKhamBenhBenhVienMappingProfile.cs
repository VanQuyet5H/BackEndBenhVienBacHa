using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DichVuKhamBenh;
using Camino.Api.Models.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuGiuongBenhVien;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Newtonsoft.Json;

namespace Camino.Api.Models.MappingProfile
{
    public class DichVuKhamBenhBenhVienMappingProfile : Profile
    {
        public DichVuKhamBenhBenhVienMappingProfile()
        {
            CreateMap<DichVuKhamBenhBenhVien, DichVuKhamBenhBenhVienViewModel>().IgnoreAllNonExisting()
                 .ForMember(d => d.DichVuKhamBenhBenhVienGiaBaoHiems, o => o.MapFrom(s => s.DichVuKhamBenhBenhVienGiaBaoHiems))
                 .ForMember(d => d.DichVuKhamBenhBenhVienGiaBenhViens, o => o.MapFrom(s => s.DichVuKhamBenhBenhVienGiaBenhViens))
                 .AfterMap((d, s) =>
                 {
                     s.DichVuKhamBenhModelText = d.DichVuKhamBenh?.TenChung;//d.DichVuKhamBenh == null ? null : d.DichVuKhamBenh.MaChung + " - " + d.DichVuKhamBenh.TenChung;
                         s.AnhXa = d.DichVuKhamBenhId != null;
                         GetNoiThucHien(d, s);
                     });
            CreateMap<DichVuKhamBenhBenhVienViewModel, DichVuKhamBenhBenhVien>().IgnoreAllNonExisting()
                 .ForMember(d => d.DichVuKhamBenhBenhVienGiaBaoHiems, o => o.Ignore())
                 .ForMember(d => d.DichVuKhamBenhBenhVienGiaBenhViens, o => o.Ignore())
                 .ForMember(d => d.DichVuKhamBenhBenhVienNoiThucHiens, o => o.Ignore())
                 .ForMember(d => d.DichVuKhamBenh, o => o.Ignore())
                 .AfterMap((s, d) =>
                 {
                     AddOrUpdateBaoHiem(s, d);
                     AddOrUpdateBenhVien(s, d);
                     AddOrUpdateNoiThucHien(s, d);
                 });

            CreateMap<DichVuKhamBenhBenhVienGiaBaoHiem, DichVuKhamBenhBenhVienGiaBaoHiemViewModel>().IgnoreAllNonExisting();
            CreateMap<DichVuKhamBenhBenhVienGiaBaoHiemViewModel, DichVuKhamBenhBenhVienGiaBaoHiem>().IgnoreAllNonExisting();

            CreateMap<DichVuKhamBenhBenhVienGiaBenhVien, DichVuKhamBenhBenhVienGiaBenhVienViewModel>().IgnoreAllNonExisting();
            CreateMap<DichVuKhamBenhBenhVienGiaBenhVienViewModel, DichVuKhamBenhBenhVienGiaBenhVien>().IgnoreAllNonExisting()
                 .ForMember(d => d.NhomGiaDichVuKhamBenhBenhVien, o => o.Ignore())
                 .ForMember(d => d.DichVuKhamBenhBenhVien, o => o.Ignore());

            CreateMap<NhomGiaDichVuKhamBenhBenhVien, NhomGiaDichVuKhamBenhBenhVienViewModel>().IgnoreAllNonExisting();
            CreateMap<NhomGiaDichVuKhamBenhBenhVienViewModel, NhomGiaDichVuKhamBenhBenhVien>().IgnoreAllNonExisting()
                .ForMember(d => d.DichVuKhamBenhBenhVienGiaBenhViens, o => o.Ignore());

            CreateMap<DichVuKhamBenhBenhVienVO, DichVuKhamBenhBenhVienExportExcel>().IgnoreAllNonExisting();
        }
        private void AddOrUpdateBaoHiem(DichVuKhamBenhBenhVienViewModel s, DichVuKhamBenhBenhVien d)
        {
           
            foreach (var model in s.DichVuKhamBenhBenhVienGiaBaoHiems)
            {
                if (model.Id == 0)
                {
                    var newEntity = new DichVuKhamBenhBenhVienGiaBaoHiem();
                    d.DichVuKhamBenhBenhVienGiaBaoHiems.Add(model.ToEntity(newEntity));
                }
                else
                {
                    var result = d.DichVuKhamBenhBenhVienGiaBaoHiems.Single(c => c.Id == model.Id);
                    result = model.ToEntity(result);
                    result.DichVuKhamBenhBenhVien = d;
                }
            }
            foreach (var model in d.DichVuKhamBenhBenhVienGiaBaoHiems)
            {
                if (model.Id != 0)
                {
                    var countModel = s.DichVuKhamBenhBenhVienGiaBaoHiems.Where(x => x.Id == model.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        model.WillDelete = true;
                    }

                }
            }
        }
        private void AddOrUpdateBenhVien(DichVuKhamBenhBenhVienViewModel s, DichVuKhamBenhBenhVien d)
        {
            foreach (var model in s.DichVuKhamBenhBenhVienGiaBenhViens)
            {
                if (model.Id == 0)
                {
                    var newEntity = new DichVuKhamBenhBenhVienGiaBenhVien();
                    d.DichVuKhamBenhBenhVienGiaBenhViens.Add(model.ToEntity(newEntity));
                }
                else
                {
                    var result = d.DichVuKhamBenhBenhVienGiaBenhViens.Single(c => c.Id == model.Id);
                   
                    result = model.ToEntity(result);
                    result.DichVuKhamBenhBenhVien = d;
                }
            }
            foreach (var model in d.DichVuKhamBenhBenhVienGiaBenhViens)
            {
                if (model.Id != 0)
                {
                    var countModel = s.DichVuKhamBenhBenhVienGiaBenhViens.Where(x => x.Id == model.Id).ToList();
                    if(countModel.Count ==0) {
                        model.WillDelete = true;
                        //for (int i = 0; i < d.DichVuKhamBenhBenhVienGiaBenhViens.OrderByDescending(p => p.Id).Where(x=>x.Id<model.Id && x.NhomGiaDichVuKhamBenhBenhVienId==model.NhomGiaDichVuKhamBenhBenhVienId && x.WillDelete==false).Count(); i++)
                        //{
                        //    if (i != 0) {
                        //        break;
                        //    }
                        //    else
                        //    {
                        //        d.DichVuKhamBenhBenhVienGiaBenhViens.Where(x => x.Id < model.Id && x.NhomGiaDichVuKhamBenhBenhVienId == model.NhomGiaDichVuKhamBenhBenhVienId && x.WillDelete == false).ToList()[i].DenNgay = null;
                        //    }
                        //}
                    }
                    
                }
            }
            foreach (var model in d.DichVuKhamBenhBenhVienGiaBaoHiems)
            {
                if (model.Id != 0)
                {
                    var countModel = s.DichVuKhamBenhBenhVienGiaBaoHiems.Where(x => x.Id == model.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        model.WillDelete = true;

                        //for (int i = 0; i < d.DichVuKhamBenhBenhVienGiaBaoHiems.OrderByDescending(p => p.Id).Where(x => x.Id < model.Id &&  x.WillDelete == false).Count(); i++)
                        //{
                        //    if (i != 0)
                        //    {
                        //        break;
                        //    }
                        //    else
                        //    {
                        //        d.DichVuKhamBenhBenhVienGiaBaoHiems.Where(x => x.Id < model.Id  && x.WillDelete == false).ToList()[i].DenNgay = null;
                        //    }
                        //}
                    }

                }
            }

        }

        private void AddOrUpdateNoiThucHien(DichVuKhamBenhBenhVienViewModel s, DichVuKhamBenhBenhVien d)
        {
            // kiểm tra thêm nơi thực hiện dv bệnh viện
            foreach (var item in s.NoiThucHienIds)
            {
                var noiThucHien = JsonConvert.DeserializeObject<ItemNoiThucHienDichVuBenhVienVo>(item);
                if (!d.DichVuKhamBenhBenhVienNoiThucHiens.Any(x =>
                    (x.KhoaPhongId != null && noiThucHien.PhongId == null && x.KhoaPhongId == noiThucHien.KhoaId)
                    || (x.KhoaPhongId == null && noiThucHien.PhongId != null && x.PhongBenhVienId == noiThucHien.PhongId)))
                {

                    if (noiThucHien.PhongId == null) // nơi thực hiện là khoa
                    {
                        var newEntity = new DichVuKhamBenhBenhVienNoiThucHien()
                        {
                            KhoaPhongId = noiThucHien.KhoaId
                        };
                        d.DichVuKhamBenhBenhVienNoiThucHiens.Add(newEntity);
                    }
                    else
                    {
                        var newEntity = new DichVuKhamBenhBenhVienNoiThucHien()
                        {
                            PhongBenhVienId = noiThucHien.PhongId
                        };
                        d.DichVuKhamBenhBenhVienNoiThucHiens.Add(newEntity);
                    }
                }
            }


            // kiểm tra xóa nơi thực hiện
            if (s.Id != 0)
            {
                var willDelete = true;
                foreach (var item in d.DichVuKhamBenhBenhVienNoiThucHiens.Where(x => x.Id != 0))
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

        private void GetNoiThucHien(DichVuKhamBenhBenhVien d, DichVuKhamBenhBenhVienViewModel s)
        {
            var templateKeyId = "\"KhoaId\": {0}, \"PhongId\": {1}";
            var lstKhoa = new List<long>();
            foreach (var noiThucHien in d.DichVuKhamBenhBenhVienNoiThucHiens)
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
    }
}
