using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamDoan;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.ValueObject.KhamDoan;
using System.Linq;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.MappingProfile
{
    public class KhamDoanGoiKhamSucKhoeChungMappingProfile : Profile
    {
        public KhamDoanGoiKhamSucKhoeChungMappingProfile()
        {
            CreateMap<GoiKhamSucKhoeChung, GoiKhamSucKhoeChungViewModel>().IgnoreAllNonExisting();

            CreateMap<GoiKhamSucKhoeChungViewModel, GoiKhamSucKhoeChung>().IgnoreAllNonExisting()
                .ForMember(d => d.GoiKhamSucKhoeChungDichVuKhamBenhs, o => o.Ignore())
                .ForMember(d => d.GoiKhamSucKhoeChungDichVuDichVuKyThuats, o => o.Ignore())
                .AfterMap((d, s) =>
                {
                    AddOrUpdateGoiKhamSucKhoeDichVuKhamBenhChung(d, s);
                    AddOrUpdateGoiKhamSucKhoeDichVuDVKTChung(d, s);
                });

            CreateMap<GoiKhamSucKhoeChungDichVuKhamBenh, GoiKhamDichVuKhamSucKhoeDoanChungViewModel>().IgnoreAllNonExisting()
                .ForMember(d => d.GoiKhamSucKhoeChungNoiThucHiens, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    d.DichVuKyThuatBenhVienId = s.DichVuKhamBenhBenhVien?.Id;
                    d.TenDichVuKyThuatBenhVien = s.DichVuKhamBenhBenhVien?.Ten;
                    d.NoiThucHienString += string.Join("; ", s.GoiKhamSucKhoeChungNoiThucHiens.Select(p => p.PhongBenhVien.Ten));
                    d.LoaiGia = s.NhomGiaDichVuKhamBenhBenhVien?.Ten;
                    d.NhomGiaDichVuKyThuatBenhVienId = s.NhomGiaDichVuKhamBenhBenhVien.Id;
                    d.Nhom = NhomDichVuChiDinhKhamSucKhoe.KhamBenh;
                    d.LaDichVuKham = true;
                    d.HinhThucKhamBenh = s.GoiKhamSucKhoeChungNoiThucHiens.Any(p => p.PhongBenhVien.HopDongKhamSucKhoeId == null) ? HinhThucKhamBenh.NoiVien : HinhThucKhamBenh.KhamDoanNgoaiVien;
                    d.GoiKhamSucKhoeChungNoiThucHienIds.AddRange(s.GoiKhamSucKhoeChungNoiThucHiens.Select(p => p.PhongBenhVienId));
                    foreach (var item in d.GoiKhamSucKhoeChungNoiThucHienIds)
                    {
                        var goiKhamSucKhoeNoiThucHien = new GoiKhamSucKhoeChungNoiThucHienViewModel
                        {
                            PhongBenhVienId = item,
                            GoiKhamSucKhoeChungDichVuKhamBenhId = s.Id
                        };
                        d.GoiKhamSucKhoeChungNoiThucHiens.Add(goiKhamSucKhoeNoiThucHien);
                    }
                });

            CreateMap<GoiKhamDichVuKhamSucKhoeDoanChungViewModel, GoiKhamSucKhoeChungDichVuKhamBenh>().IgnoreAllNonExisting()
                .ForMember(d => d.GoiKhamSucKhoeChungNoiThucHiens, o => o.Ignore())
                    .AfterMap((d, s) =>
                    {
                        AddOrUpdateGoiKhamSucKhoeDichVuKhamBenhNoiThucHienChung(d, s);
                    });

            CreateMap<GoiKhamSucKhoeChungDichVuDichVuKyThuat, GoiKhamDichVuKhamSucKhoeDoanChungViewModel>().IgnoreAllNonExisting()
                .ForMember(d => d.GoiKhamSucKhoeChungNoiThucHiens, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    d.DichVuKyThuatBenhVienId = s.DichVuKyThuatBenhVien?.Id;
                    d.TenDichVuKyThuatBenhVien = s.DichVuKyThuatBenhVien?.Ten;
                    d.NoiThucHienString += string.Join("; ", s.GoiKhamSucKhoeChungNoiThucHiens.Select(p => p.PhongBenhVien.Ten));
                    d.LoaiGia = s.NhomGiaDichVuKyThuatBenhVien?.Ten;
                    d.NhomGiaDichVuKyThuatBenhVienId = s.NhomGiaDichVuKyThuatBenhVien.Id;
                    d.MaNhomDichVuBenhVien = s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.Ma;
                    d.MaNhomDichVuBenhVienCha = s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.NhomDichVuBenhVienCha?.Ma;
                    d.Nhom = (s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.Ma == "XN" || s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.NhomDichVuBenhVienCha?.Ma == "XN") ? NhomDichVuChiDinhKhamSucKhoe.XetNghiem :
                            ((s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.Ma == "CĐHA" || s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.NhomDichVuBenhVienCha?.Ma == "CĐHA") ? NhomDichVuChiDinhKhamSucKhoe.ChuanDoanHinhAnh :
                            (s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.Ma == "TDCN" || s.DichVuKyThuatBenhVien?.NhomDichVuBenhVien?.NhomDichVuBenhVienCha?.Ma == "TDCN") ? NhomDichVuChiDinhKhamSucKhoe.ThamDoChucNang : NhomDichVuChiDinhKhamSucKhoe.KH);
                    d.HinhThucKhamBenh = s.GoiKhamSucKhoeChungNoiThucHiens.Any(p => p.PhongBenhVien.HopDongKhamSucKhoeId == null) ? HinhThucKhamBenh.NoiVien : HinhThucKhamBenh.KhamDoanNgoaiVien;
                    d.GoiKhamSucKhoeChungNoiThucHienIds.AddRange(s.GoiKhamSucKhoeChungNoiThucHiens.Select(p => p.PhongBenhVienId));
                    foreach (var item in d.GoiKhamSucKhoeChungNoiThucHienIds)
                    {
                        var goiKhamSucKhoeNoiThucHien = new GoiKhamSucKhoeChungNoiThucHienViewModel
                        {
                            PhongBenhVienId = item,
                            GoiKhamSucKhoeChungDichVuKhamBenhId = s.Id
                        };
                        d.GoiKhamSucKhoeChungNoiThucHiens.Add(goiKhamSucKhoeNoiThucHien);
                    }
                });

            CreateMap<GoiKhamDichVuKhamSucKhoeDoanChungViewModel, GoiKhamSucKhoeChungDichVuDichVuKyThuat>().IgnoreAllNonExisting()
                .ForMember(d => d.GoiKhamSucKhoeChungNoiThucHiens, o => o.Ignore())
                 .AfterMap((d, s) =>
                 {
                     AddOrUpdateGoiKhamSucKhoeDichVuDVKTNoiThucHienChung(d, s);
                 });

            CreateMap<GoiKhamSucKhoeChungNoiThucHien, GoiKhamSucKhoeChungNoiThucHienViewModel>().IgnoreAllNonExisting();
            CreateMap<GoiKhamSucKhoeChungNoiThucHienViewModel, GoiKhamSucKhoeChungNoiThucHien>().IgnoreAllNonExisting();
            CreateMap<GoiKhamSucKhoeChungDoanVo, GoiKhamSucKhoeDoanChungExportExcel>().IgnoreAllNonExisting();


        }

        #region Dịch vụ khám bệnh bệnh viện

        private void AddOrUpdateGoiKhamSucKhoeDichVuKhamBenhChung(GoiKhamSucKhoeChungViewModel viewModel, GoiKhamSucKhoeChung entity)
        {
            foreach (var item in viewModel.GoiKhamSucKhoeChungDichVuKhamBenhs)
            {
                if (item.Id == 0)
                {
                    var newEntity = new GoiKhamSucKhoeChungDichVuKhamBenh();
                    var goiKhamSucKhoeDichVuKhamBenh = item.ToEntity(newEntity);
                    goiKhamSucKhoeDichVuKhamBenh.DichVuKhamBenhBenhVienId = item.DichVuKyThuatBenhVienId.Value;
                    goiKhamSucKhoeDichVuKhamBenh.NhomGiaDichVuKhamBenhBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId;
                    entity.GoiKhamSucKhoeChungDichVuKhamBenhs.Add(goiKhamSucKhoeDichVuKhamBenh);
                }
                else
                {
                    var result = entity.GoiKhamSucKhoeChungDichVuKhamBenhs.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in entity.GoiKhamSucKhoeChungDichVuKhamBenhs)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.GoiKhamSucKhoeChungDichVuKhamBenhs.Where(x => x.Id == item.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        item.WillDelete = true;
                    }

                }
            }
        }
        private void AddOrUpdateGoiKhamSucKhoeDichVuKhamBenhNoiThucHienChung(GoiKhamDichVuKhamSucKhoeDoanChungViewModel viewModel, GoiKhamSucKhoeChungDichVuKhamBenh entity)
        {
            foreach (var item in viewModel.GoiKhamSucKhoeChungNoiThucHiens)
            {
                if (item.Id == 0)
                {
                    var newEntity = new GoiKhamSucKhoeChungNoiThucHien();
                    entity.GoiKhamSucKhoeChungNoiThucHiens.Add(item.ToEntity(newEntity));
                }
                else
                {
                    var result = entity.GoiKhamSucKhoeChungNoiThucHiens.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in entity.GoiKhamSucKhoeChungNoiThucHiens)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.GoiKhamSucKhoeChungNoiThucHiens.Where(x => x.Id == item.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        item.WillDelete = true;
                    }

                }
            }
        }

        #endregion

        #region Dịch vụ kỹ thuật bệnh viện

        private void AddOrUpdateGoiKhamSucKhoeDichVuDVKTChung(GoiKhamSucKhoeChungViewModel viewModel, GoiKhamSucKhoeChung entity)
        {
            foreach (var item in viewModel.GoiKhamSucKhoeChungDichVuDichVuKyThuats)
            {
                if (item.Id == 0)
                {
                    var newEntity = new GoiKhamSucKhoeChungDichVuDichVuKyThuat();
                    entity.GoiKhamSucKhoeChungDichVuDichVuKyThuats.Add(item.ToEntity(newEntity));
                }
                else
                {
                    var result = entity.GoiKhamSucKhoeChungDichVuDichVuKyThuats.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in entity.GoiKhamSucKhoeChungDichVuDichVuKyThuats)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.GoiKhamSucKhoeChungDichVuDichVuKyThuats.Where(x => x.Id == item.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        item.WillDelete = true;
                    }
                }
            }
        }

        private void AddOrUpdateGoiKhamSucKhoeDichVuDVKTNoiThucHienChung(GoiKhamDichVuKhamSucKhoeDoanChungViewModel viewModel, GoiKhamSucKhoeChungDichVuDichVuKyThuat entity)
        {
            foreach (var item in viewModel.GoiKhamSucKhoeChungNoiThucHiens)
            {
                if (item.Id == 0)
                {
                    var newEntity = new GoiKhamSucKhoeChungNoiThucHien();
                    entity.GoiKhamSucKhoeChungNoiThucHiens.Add(item.ToEntity(newEntity));
                }
                else
                {
                    var result = entity.GoiKhamSucKhoeChungNoiThucHiens.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in entity.GoiKhamSucKhoeChungNoiThucHiens)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.GoiKhamSucKhoeChungNoiThucHiens.Where(x => x.Id == item.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        item.WillDelete = true;
                    }
                }
            }
        }

        #endregion

    }
}
