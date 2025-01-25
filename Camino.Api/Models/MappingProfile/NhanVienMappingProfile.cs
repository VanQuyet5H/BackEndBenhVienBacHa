using AutoMapper;
using Camino.Api.Models.NhanVien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.Users;
using Camino.Api.Extensions;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.NhanVien;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.Entities.KhoNhanVienQuanLys;
using Camino.Api.Models.KhoaPhongNhanVien;
using Camino.Core.Domain.Entities.HoSoNhanVienDinhKems;

namespace Camino.Api.Models.MappingProfile
{
    public class NhanVienMappingProfile : Profile
    {
        public NhanVienMappingProfile()
        {
            CreateMap<Core.Domain.Entities.NhanViens.NhanVien, NhanVienViewModel>()
                 .ForMember(d => d.KhoaPhongNhanViens, o => o.Ignore())
                  .ForMember(d => d.KhoNhanVienQuanLys, o => o.Ignore())
                  .ForMember(d => d.HoSoNhanVienFileDinhKems, o => o.Ignore())
                .AfterMap((s, d) =>
              {
                  //if (s.NhanVienRoles != null)
                  //{
                  //    d.QuyenHans = s.NhanVienRoles.Where(o => o.Role.UserType == Core.Domain.Enums.UserType.NhanVien).Select(o => o.RoleId).ToList();
                  //}
                  ////------------------
                  //d.Avatar = s.User.Avatar;
                  d.DiaChi = s.User.DiaChi;
                  d.Email = s.User.Email;
                  d.GioiTinh = s.User.GioiTinh;
                  d.HoTen = s.User.HoTen;
                  d.NgaySinh = s.User.NgaySinh;
                  d.SoCMT = s.User.SoChungMinhThu;
                  d.SoDienThoai = s.User.SoDienThoai;
                  d.TextTenChucDanh = s.ChucDanh?.Ten != null ? s.ChucDanh.Ma + " - " + s.ChucDanh.Ten : "";
                  d.TextTenHocHamHocVi = s.HocHamHocVi?.Ten != null ? s.HocHamHocVi.Ma + " - " + s.HocHamHocVi.Ten : "";
                  d.TextTenPhamViHanhNghe = s.PhamViHanhNghe?.Ten != null ? s.PhamViHanhNghe.Ma + " - " + s.PhamViHanhNghe.Ten : "";
                  d.TextTenVanBangChuyenMon = s.VanBangChuyenMon?.Ten != null ? s.VanBangChuyenMon.TenVietTat + " - " + s.VanBangChuyenMon.Ten : "";
                  d.TextTenQuyenHan = s.QuyenHan;
                  d.KhoaPhongIds = s.KhoaPhongNhanViens.Any() != true ? null : s.KhoaPhongNhanViens.Select(p => p.KhoaPhongId).ToList();
                  d.KhoNhanVienQuanLyIds = s.KhoNhanVienQuanLys.Any() != true ? null : s.KhoNhanVienQuanLys.Select(p => p.KhoId).ToList();

                  if (s.NhanVienRoles.Any())
                  {
                      d.LstRole.AddRange(s.NhanVienRoles.Any() ? s.NhanVienRoles.Select(p => p.RoleId) : null);
                  }
                  if (s.NhanVienChucVus.Any())
                  {
                      d.ChucVuIds.AddRange(s.NhanVienChucVus.Any() ? s.NhanVienChucVus.Select(p => p.ChucVuId) : null);
                  }
                  if (s.HoSoNhanVienFileDinhKems.Any())
                  {
                      var list =s.HoSoNhanVienFileDinhKems.Select(g => new HoSoNhanVienFileDinhKemModel
                      {
                          Ma = g.Ma,
                          Ten = g.Ten,
                          TenGuid = g.TenGuid,
                          DuongDan = g.DuongDan,
                          LoaiTapTin = g.LoaiTapTin,
                          MoTa = g.MoTa,
                          KichThuoc = g.KichThuoc,
                          NhanVienId = (long)g.NhanVienId, // to do
                          Id = g.Id
                      }).ToList();
                      d.HoSoNhanVienFileDinhKems.AddRange(list);
                  }
              });

            CreateMap<KhoNhanVienQuanLyModel, KhoNhanVienQuanLy>()
            .ForMember(d => d.NhanVien, o => o.Ignore())
            .ForMember(d => d.Kho, o => o.Ignore());

            CreateMap<NhanVienViewModel, Core.Domain.Entities.NhanViens.NhanVien>().IgnoreAllNonExisting()
                .ForMember(d => d.KhoaPhongNhanViens, o => o.Ignore())
                .ForMember(d => d.KhoNhanVienQuanLys, o => o.Ignore())
                .ForMember(d => d.NhanVienRoles, o => o.Ignore())
                .ForMember(d => d.LichPhanCongNgoaiTrus, o => o.Ignore())
                .ForMember(d => d.HoatDongNhanViens, o => o.Ignore())
                .ForMember(d => d.LichSuHoatDongNhanViens, o => o.Ignore())
                .ForMember(d => d.NhanVienChucVus, o => o.Ignore())
                .ForMember(d => d.HoSoNhanVienFileDinhKems, o => o.Ignore())
                .AfterMap((s, d) =>
            {
                if (d.User == null)
                    d.User = new User();
                d.User.SoDienThoai = s.SoDienThoai;
                d.User.Region = Core.Domain.Enums.Region.Internal;
                d.User.Email = s.Email;
                d.User.SoChungMinhThu = s.SoCMT;
                d.User.NgaySinh = s.NgaySinh;
                d.User.HoTen = s.HoTen;
                d.User.SoChungMinhThu = s.SoCMT;
                d.User.DiaChi = s.DiaChi;
                d.User.GioiTinh = s.GioiTinh;
                d.QuyenHan = s.TextTenQuyenHan;

                foreach (var model in d.KhoaPhongNhanViens)
                {
                    if (s.KhoaPhongNhanViens.All(c => c.Id != model.Id))
                    {
                        model.WillDelete = true;
                    }
                }
                foreach (var model in s.KhoaPhongNhanViens)
                {
                    if (model.Id == 0)
                    {
                        var newEntity = new Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien();
                        d.KhoaPhongNhanViens.Add(model.ToEntity(newEntity));
                    }
                    else
                    {
                        if (d.KhoaPhongNhanViens.Any())
                        {
                            var result = d.KhoaPhongNhanViens.Single(c => c.Id == model.Id);
                            result = model.ToEntity(result);
                        }

                    }
                }


                foreach (var model in d.KhoNhanVienQuanLys)
                {
                    if (s.KhoNhanVienQuanLys.All(c => c.Id != model.Id))
                    {
                        model.WillDelete = true;
                    }
                }

                foreach (var model in s.KhoNhanVienQuanLys)
                {
                    if (model.Id == 0)
                    {
                        var newEntity = new Core.Domain.Entities.KhoNhanVienQuanLys.KhoNhanVienQuanLy();
                        d.KhoNhanVienQuanLys.Add(model.ToEntity(newEntity));
                    }
                    else
                    {
                        if (d.KhoNhanVienQuanLys.Any())
                        {
                            var result = d.KhoNhanVienQuanLys.Single(c => c.Id == model.Id);
                            result = model.ToEntity(result);
                        }

                    }
                }


                //foreach(var nhanVienRole in d.NhanVienRoles)
                //{
                //   if(s.QuyenHans.All(o => o != nhanVienRole.RoleId))
                //    {
                //        nhanVienRole.WillDelete = true;
                //    }
                //}

                //foreach(var role in s.QuyenHans)
                //{
                //    if(d.NhanVienRoles.All(x => x.RoleId!= role))
                //    {
                //        d.NhanVienRoles.Add(new NhanVienRole
                //        {
                //            RoleId = role
                //        });
                //    }
                //}
            });

            CreateMap<ProfileNhanVien, Core.Domain.Entities.NhanViens.NhanVien>().IgnoreAllNonExisting()
                .ForMember(d => d.KhoaPhongNhanViens, o => o.Ignore())
                .ForMember(d => d.HoSoNhanVienFileDinhKems, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    if (d.User == null)
                        d.User = new User();
                    d.User.SoDienThoai = s.SoDienThoai;
                    d.User.Region = Core.Domain.Enums.Region.Internal;
                    d.User.Email = s.Email;
                    d.User.SoChungMinhThu = s.SoCMT;
                    d.User.NgaySinh = s.NgaySinh;
                    d.User.HoTen = s.HoTen;
                    d.User.SoChungMinhThu = s.SoCMT;
                    d.User.DiaChi = s.DiaChi;
                    d.User.GioiTinh = s.GioiTinh;
                    d.QuyenHan = s.TextTenQuyenHan;
                    foreach (var model in d.KhoaPhongNhanViens)
                    {
                        if (s.KhoaPhongNhanViens.All(c => c.Id != model.Id))
                        {
                            model.WillDelete = true;
                        }
                    }
                    foreach (var model in s.KhoaPhongNhanViens)
                    {
                        if (model.Id == 0)
                        {
                            var newEntity = new Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien();
                            d.KhoaPhongNhanViens.Add(model.ToEntity(newEntity));
                        }
                        else
                        {
                            if (d.KhoaPhongNhanViens.Any())
                            {
                                var result = d.KhoaPhongNhanViens.Single(c => c.Id == model.Id);
                                result = model.ToEntity(result);
                            }

                        }
                    }
                   
                });

            CreateMap<Core.Domain.Entities.NhanViens.NhanVien, ProfileNhanVien>()
                .ForMember(d => d.HoSoNhanVienFileDinhKems, o => o.Ignore())
               .AfterMap((s, d) =>
               {
                   d.Password = s.User.Password;
                   d.DiaChi = s.User.DiaChi;
                   d.Email = s.User.Email;
                   d.GioiTinh = s.User.GioiTinh;
                   d.HoTen = s.User.HoTen;
                   d.NgaySinh = s.User.NgaySinh;
                   d.SoCMT = s.User.SoChungMinhThu;
                   d.SoDienThoai = s.User.SoDienThoai;
                   d.TextTenChucDanh = s.ChucDanh?.Ten != null ? s.ChucDanh.Ma + " - " + s.ChucDanh.Ten : "";
                   d.TextTenHocHamHocVi = s.HocHamHocVi?.Ten != null ? s.HocHamHocVi.Ma + " - " + s.HocHamHocVi.Ten : "";
                   d.TextTenPhamViHanhNghe = s.PhamViHanhNghe?.Ten != null ? s.PhamViHanhNghe.Ma + " - " + s.PhamViHanhNghe.Ten : "";
                   d.TextTenVanBangChuyenMon = s.VanBangChuyenMon?.Ten != null ? s.VanBangChuyenMon.TenVietTat + " - " + s.VanBangChuyenMon.Ten : "";
                   d.TextTenQuyenHan = s.QuyenHan;
                   d.KhoaPhongIds = s.KhoaPhongNhanViens.Any() != true ? null : s.KhoaPhongNhanViens.Select(p => p.KhoaPhongId).ToList();
                   if (s.NhanVienChucVus.Any())
                   {
                       d.ChucVuIds.AddRange(s.NhanVienChucVus.Any() ? s.NhanVienChucVus.Select(p => p.ChucVuId) : null);
                   }
                   if (s.HoSoNhanVienFileDinhKems.Any())
                   {
                       var list = s.HoSoNhanVienFileDinhKems.Select(g => new HoSoNhanVienFileDinhKemModel
                       {
                           Ma = g.Ma,
                           Ten = g.Ten,
                           TenGuid = g.TenGuid,
                           DuongDan = g.DuongDan,
                           LoaiTapTin = g.LoaiTapTin,
                           MoTa = g.MoTa,
                           KichThuoc = g.KichThuoc,
                           NhanVienId = (long)g.NhanVienId, // to do
                           Id = g.Id
                       }).ToList();
                       d.HoSoNhanVienFileDinhKems.AddRange(list);
                   }
               });

            CreateMap<NhanVienGridVo, NhanVienExportExcel>().IgnoreAllNonExisting();
        }
    }
}
