using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.BenhNhans;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Helpers;
using System;
using System.Linq;
using Camino.Core.Domain.Entities.BenhNhanCongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Api.Models.YeuCauKhamBenh;

namespace Camino.Api.Models.MappingProfile
{
    public class QuanLyBenhNhanMappingProfile : Profile
    {
        public QuanLyBenhNhanMappingProfile()
        {
            CreateMap<BenhNhan, QuanLyBenhNhanViewModel>()
                 .ForMember(d => d.BenhNhanCongTyBaoHiemTuNhans, o => o.MapFrom(s => s.BenhNhanCongTyBaoHiemTuNhans))
                .ForMember(d => d.BenhNhanDiUngThuocs, o => o.MapFrom(s => s.BenhNhanDiUngThuocs))
                .ForMember(d => d.BenhNhanTienSuBenhs, o => o.MapFrom(s => s.BenhNhanTienSuBenhs))
                 .AfterMap((s, d) =>
                 {
                     if (s.NgaySinh != null && s.ThangSinh != null && s.NamSinh != null)
                     {
                         d.NgayThangNamSinh = new DateTime(s.NamSinh.GetValueOrDefault(), s.ThangSinh.GetValueOrDefault(), s.NgaySinh.GetValueOrDefault(), 0, 0, 0);
                     }
                     if (s.NgheNghiep != null)
                     {
                         d.NgheNghiepText = s.NgheNghiep.Ten;
                     }
                     if (s.QuocTich != null)
                     {
                         d.QuocTichText = s.QuocTich.Ten;
                     }
                     if (s.NguoiLienHeQuanHeNhanThan != null)
                     {
                         d.NguoiLienHeQuanHeNhanThanText = s.NguoiLienHeQuanHeNhanThan.Ten;
                     }
                     if (s.NhomMau != null)
                     {
                         d.NhomMauText = s.NhomMau.GetDescription();
                     }
                     d.GioiTinhDisplay = s.GioiTinh == null ? "" : s.GioiTinh.GetDescription();
                    
                 });
            CreateMap<QuanLyBenhNhanViewModel, BenhNhan>().IgnoreAllNonExisting()
                .ForMember(d => d.BenhNhanDiUngThuocs, o => o.Ignore())
                .ForMember(d => d.BenhNhanTienSuBenhs, o => o.Ignore())
                .ForMember(d => d.YeuCauTiepNhans, o => o.Ignore())
                .ForMember(d => d.BenhNhanCongTyBaoHiemTuNhans, o => o.Ignore())
                .ForMember(d => d.DonThuocThanhToans, o => o.Ignore())
                  .AfterMap((viewModel, entity) =>
                  {
                      if (viewModel.NgayThangNamSinh != null)
                      {
                          entity.NgaySinh = viewModel.NgayThangNamSinh.GetValueOrDefault().Day;
                          entity.ThangSinh = viewModel.NgayThangNamSinh.GetValueOrDefault().Month;
                          entity.NamSinh = viewModel.NgayThangNamSinh.GetValueOrDefault().Year;
                      }
                      AddOrUpdateBaoHiemTuNhan(viewModel, entity);
                      AddOrUpdateBenhNhanDiUngThuoc(viewModel, entity);
                      AddOrUpdateBenhNhanTienSuBenh(viewModel, entity);
                  });
            CreateMap<CongTyBaoHiemTuNhan, BenhNhanBaoHiemTuNhansViewModel>().IgnoreAllNonExisting();
            CreateMap<CongTyBaoHiemTuNhan, BenhNhanCongTyBaoHiemTuNhanViewModel>().IgnoreAllNonExisting();

            CreateMap<BenhNhanDiUngThuoc, BenhNhanDiUngThuocsViewModel>().IgnoreAllNonExisting();
            CreateMap<BenhNhanTienSuBenh, BenhNhanTienSuBenhsViewModel>().IgnoreAllNonExisting();

        }

        private void AddOrUpdateBaoHiemTuNhan(QuanLyBenhNhanViewModel viewModel, BenhNhan entity)
        {
            foreach (var item in viewModel.BenhNhanCongTyBaoHiemTuNhans)
            {
                if (item.Id == 0)
                {
                    var newEntity = new BenhNhanCongTyBaoHiemTuNhan();
                    entity.BenhNhanCongTyBaoHiemTuNhans.Add(item.ToEntity(newEntity));
                }
                else
                {
                    var result = entity.BenhNhanCongTyBaoHiemTuNhans.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in entity.BenhNhanCongTyBaoHiemTuNhans)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.BenhNhanCongTyBaoHiemTuNhans.Where(x => x.Id == item.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        item.WillDelete = true;
                    }

                }
            }
        }

        private void AddOrUpdateBenhNhanDiUngThuoc(QuanLyBenhNhanViewModel viewModel, BenhNhan entity)
        {
            foreach (var item in viewModel.BenhNhanDiUngThuocs)
            {
                if (item.Id == 0)
                {
                    var newEntity = new BenhNhanDiUngThuoc();
                    entity.BenhNhanDiUngThuocs.Add(item.ToEntity(newEntity));
                }
                else
                {
                    var result = entity.BenhNhanDiUngThuocs.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in entity.BenhNhanDiUngThuocs)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.BenhNhanDiUngThuocs.Where(x => x.Id == item.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        item.WillDelete = true;
                    }

                }
            }
        }


        private void AddOrUpdateBenhNhanTienSuBenh(QuanLyBenhNhanViewModel viewModel, BenhNhan entity)
        {
            foreach (var item in viewModel.BenhNhanTienSuBenhs)
            {
                if (item.Id == 0)
                {
                    var newEntity = new BenhNhanTienSuBenh();
                    entity.BenhNhanTienSuBenhs.Add(item.ToEntity(newEntity));
                }
                else
                {
                    var result = entity.BenhNhanTienSuBenhs.Single(c => c.Id == item.Id);
                    result = item.ToEntity(result);
                }
            }
            foreach (var item in entity.BenhNhanTienSuBenhs)
            {
                if (item.Id != 0)
                {
                    var countModel = viewModel.BenhNhanTienSuBenhs.Where(x => x.Id == item.Id).ToList();
                    if (countModel.Count == 0)
                    {
                        item.WillDelete = true;
                    }

                }
            }
        }
    }
}
