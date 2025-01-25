using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DichVuGiuongBenhVien;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuGiuong;
using Camino.Core.Domain.ValueObject.DichVuGiuongBenhVien;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Helpers;
using Newtonsoft.Json;

namespace Camino.Api.Models.MappingProfile
{
    public class DichVuGiuongBenhVienMappingProfile : Profile
    {
        public DichVuGiuongBenhVienMappingProfile()
        {
            CreateMap<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien, DichVuGiuongBenhVienViewModel>().IgnoreAllNonExisting()
                  .ForMember(d => d.DichVuGiuongBenhVienGiaBaoHiems, o => o.MapFrom(s => s.DichVuGiuongBenhVienGiaBaoHiems))
                  .ForMember(d => d.DichVuGiuongBenhVienGiaBenhViens, o => o.MapFrom(s => s.DichVuGiuongBenhVienGiaBenhViens))
                  .AfterMap((d, s) =>
                  {
                      s.DichVuGiuongModelText = d.DichVuGiuong?.TenChung; //d.DichVuGiuong == null ? null : d.Ma + " - " + d.Ten;
                      s.LoaiGiuongText = s.LoaiGiuong == null ? null : d.LoaiGiuong.GetDescription();
                      s.AnhXa = d.DichVuGiuongId != null;
                      //GetNoiThucHien(d, s);
                  });
            CreateMap<DichVuGiuongBenhVienViewModel, Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien>().IgnoreAllNonExisting()
                .ForMember(d => d.DichVuGiuong, o => o.Ignore())
                .ForMember(d => d.DichVuGiuongBenhVienGiaBaoHiems, o => o.Ignore())
                .ForMember(d => d.DichVuGiuongBenhVienGiaBenhViens, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    AddOrUpdateBaoHiem(s, d);
                    AddOrUpdateBenhVien(s, d);
                    //AddOrUpdateNoiThucHien(s, d);
                });



            CreateMap<DichVuGiuongBenhVienGiaBaoHiem, DichVuGiuongBenhVienGiaBaoHiemViewModel>().IgnoreAllNonExisting()
                .ForMember(d => d.DichVuGiuongBenhVien, o => o.Ignore());
            CreateMap<DichVuGiuongBenhVienGiaBaoHiemViewModel, DichVuGiuongBenhVienGiaBaoHiem>().IgnoreAllNonExisting()
                 .ForMember(d => d.DichVuGiuongBenhVien, o => o.Ignore());

            CreateMap<DichVuGiuongBenhVienGiaBenhVien, DichVuGiuongBenhVienGiaBenhVienViewModel>().IgnoreAllNonExisting()
                  .ForMember(d => d.DichVuGiuongBenhVien, o => o.Ignore())
                .ForMember(d => d.NhomGiaDichVuGiuongBenhVien, o => o.MapFrom(s => s.NhomGiaDichVuGiuongBenhVien));
            CreateMap<DichVuGiuongBenhVienGiaBenhVienViewModel, DichVuGiuongBenhVienGiaBenhVien>().IgnoreAllNonExisting()
                  .ForMember(d => d.DichVuGiuongBenhVien, o => o.Ignore())
                 .ForMember(d => d.NhomGiaDichVuGiuongBenhVien, o => o.MapFrom(s => s.NhomGiaDichVuGiuongBenhVien));

            CreateMap<NhomGiaDichVuGiuongBenhVien, NhomGiaDichVuGiuongBenhVienViewModel>().IgnoreAllNonExisting()
                 .ForMember(d => d.DichVuGiuongBenhVienGiaBenhViens, o => o.Ignore());
            CreateMap<NhomGiaDichVuGiuongBenhVienViewModel, NhomGiaDichVuGiuongBenhVien>().IgnoreAllNonExisting()
                .ForMember(d => d.DichVuGiuongBenhVienGiaBenhViens, o => o.Ignore());

            CreateMap<DichVuGiuongGridVo, DichVuGiuongBenhVienExportExcel>()
                .IgnoreAllNonExisting();

            CreateMap<DichVuKhamBenhBenhVienGiaBaoHiemVO, DichVuGiuongBenhVienExportExcelChild>()
                .AfterMap((source, dest) =>
                {
                    dest.KieuGia = "Giá Bảo Hiểm";
                    dest.LoaiGia = string.Empty;
                    dest.TiLeBaoHiemThanhToan = source.TiLeBaoHiemThanhToan.ToString();
                });

            CreateMap<DichVuKhamBenhBenhVienGiaBenhVienVO, DichVuGiuongBenhVienExportExcelChild>()
                .AfterMap((source, dest) =>
                {
                    dest.KieuGia = "Giá Bệnh Viện";
                    dest.TiLeBaoHiemThanhToan = string.Empty;
                });
        }
        private void AddOrUpdateBaoHiem(DichVuGiuongBenhVienViewModel s, Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien d)
        {

            foreach (var model in s.DichVuGiuongBenhVienGiaBaoHiems)
            {
                if (model.Id == 0)
                {
                    var newEntity = new DichVuGiuongBenhVienGiaBaoHiem();
                    d.DichVuGiuongBenhVienGiaBaoHiems.Add(model.ToEntity(newEntity));
                }
                else
                {

                    var result = d.DichVuGiuongBenhVienGiaBaoHiems.Single(c => c.Id == model.Id);
                    result = model.ToEntity(result);
                    //result.DichVuKyThuatBenhVien = d;
                }
            }

            foreach (var model in d.DichVuGiuongBenhVienGiaBaoHiems)
            {
                if (model.Id != 0)
                {
                    var countModel = s.DichVuGiuongBenhVienGiaBaoHiems.Where(x => x.Id == model.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        model.WillDelete = true;
                    }

                }
            }
        }
        private void AddOrUpdateBenhVien(DichVuGiuongBenhVienViewModel s, Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien d)
        {


            foreach (var model in s.DichVuGiuongBenhVienGiaBenhViens)
            {
                if (model.Id == 0)
                {
                    var newEntity = new DichVuGiuongBenhVienGiaBenhVien();
                    d.DichVuGiuongBenhVienGiaBenhViens.Add(model.ToEntity(newEntity));
                }
                else
                {

                    var result = d.DichVuGiuongBenhVienGiaBenhViens.Single(c => c.Id == model.Id);

                    result = model.ToEntity(result);
                    result.DichVuGiuongBenhVien = d;

                }
            }
            foreach (var model in d.DichVuGiuongBenhVienGiaBenhViens)
            {
                int count = 0;
                if (model.Id != 0)
                {
                    var countModel = s.DichVuGiuongBenhVienGiaBenhViens.Where(x => x.Id == model.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        model.WillDelete = true;

                        for (int i = 0; i < d.DichVuGiuongBenhVienGiaBenhViens.OrderByDescending(p => p.Id).Where(x => x.Id < model.Id && x.NhomGiaDichVuGiuongBenhVienId == model.NhomGiaDichVuGiuongBenhVienId && x.WillDelete == false).Count(); i++)
                        {
                            if (i != 0)
                            {
                                break;
                            }
                            else
                            {
                                d.DichVuGiuongBenhVienGiaBenhViens.Where(x => x.Id < model.Id && x.NhomGiaDichVuGiuongBenhVienId == model.NhomGiaDichVuGiuongBenhVienId && x.WillDelete == false).ToList()[i].DenNgay = null;
                            }
                        }
                    }

                }
            }
            foreach (var model in d.DichVuGiuongBenhVienGiaBaoHiems)
            {
                int count = 0;
                if (model.Id != 0)
                {
                    var countModel = s.DichVuGiuongBenhVienGiaBaoHiems.Where(x => x.Id == model.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        model.WillDelete = true;

                        for (int i = 0; i < d.DichVuGiuongBenhVienGiaBaoHiems.OrderByDescending(p => p.Id).Where(x => x.Id < model.Id && x.WillDelete == false).Count(); i++)
                        {
                            if (i != 0)
                            {
                                break;
                            }
                            else
                            {
                                d.DichVuGiuongBenhVienGiaBaoHiems.Where(x => x.Id < model.Id && x.WillDelete == false).ToList()[i].DenNgay = null;
                            }
                        }
                    }

                }
            }

        }
        private void AddOrUpdateNoiThucHien(DichVuGiuongBenhVienViewModel s, Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien d)
        {
            // kiểm tra thêm nơi thực hiện dv bệnh viện
            foreach (var item in s.NoiThucHienIds)
            {
                var noiThucHien = JsonConvert.DeserializeObject<ItemNoiThucHienDichVuBenhVienVo>(item);
                if (!d.DichVuGiuongBenhVienNoiThucHiens.Any(x =>
                    (x.KhoaPhongId != null && noiThucHien.PhongId == null && x.KhoaPhongId == noiThucHien.KhoaId)
                    || (x.KhoaPhongId == null && noiThucHien.PhongId != null && x.PhongBenhVienId == noiThucHien.PhongId)))
                {

                    if (noiThucHien.PhongId == null) // nơi thực hiện là khoa
                    {
                        var newEntity = new DichVuGiuongBenhVienNoiThucHien()
                        {
                            KhoaPhongId = noiThucHien.KhoaId
                        };
                        d.DichVuGiuongBenhVienNoiThucHiens.Add(newEntity);
                    }
                    else
                    {
                        var newEntity = new DichVuGiuongBenhVienNoiThucHien()
                        {
                            PhongBenhVienId = noiThucHien.PhongId
                        };
                        d.DichVuGiuongBenhVienNoiThucHiens.Add(newEntity);
                    }
                }
            }


            // kiểm tra xóa nơi thực hiện
            if (s.Id != 0)
            {
                var willDelete = true;
                foreach (var item in d.DichVuGiuongBenhVienNoiThucHiens.Where(x => x.Id != 0))
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

        private void GetNoiThucHien(Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien d, DichVuGiuongBenhVienViewModel s)
        {
            var templateKeyId = "\"KhoaId\": {0}, \"PhongId\": {1}";
            var lstKhoa = new List<long>();
            foreach (var noiThucHien in d.DichVuGiuongBenhVienNoiThucHiens)
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
