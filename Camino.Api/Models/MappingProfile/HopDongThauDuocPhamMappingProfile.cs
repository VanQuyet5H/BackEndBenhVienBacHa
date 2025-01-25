using AutoMapper;
using Camino.Api.Models.HopDongThauDuocPham;
using Camino.Core.Domain.Entities.HopDongThauDuocPhams;
using Camino.Core.Helpers;
using System;
using System.Linq;
using Camino.Api.Extensions;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.HopDongThauDuocPham;

namespace Camino.Api.Models.MappingProfile
{
    public class HopDongThauDuocPhamMappingProfile : Profile
    {
        public HopDongThauDuocPhamMappingProfile()
        {
            CreateMap<Core.Domain.Entities.HopDongThauDuocPhams.HopDongThauDuocPham, HopDongThauDuocPhamViewModel>()
                    .AfterMap((source, dest) =>
                    {
                        dest.NhaThau = source.NhaThau != null ? source.NhaThau.Ten : string.Empty;
                        dest.CongBoDisplay = source.CongBo != null ? Convert.ToDateTime(source.CongBo).ApplyFormatDate() : null;
                        dest.NgayKyDisplay = source.NgayKy != null ? Convert.ToDateTime(source.NgayKy).ApplyFormatDate() : null;
                        dest.NgayHetHanDisplay = source.NgayHetHan != null ? Convert.ToDateTime(source.NgayHetHan).ApplyFormatDate() : null;
                        dest.NgayHieuLucDisplay = source.NgayHieuLuc != null ? Convert.ToDateTime(source.NgayHieuLuc).ApplyFormatDate() : null;
                        dest.TenLoaiThau = source.LoaiThau.GetDescription();
                        dest.TenLoaiThuocThau = source.LoaiThuocThau.GetDescription();
                    });

            CreateMap<HopDongThauDuocPhamViewModel, Core.Domain.Entities.HopDongThauDuocPhams.HopDongThauDuocPham>()
                      .ForMember(d => d.NhaThau, o => o.Ignore())
                      .ForMember(x => x.HopDongThauDuocPhamChiTiets, o => o.Ignore())
                      .AfterMap((source, dest) =>
                      {               //(mr, m)
                          foreach (var model in dest.HopDongThauDuocPhamChiTiets)
                          {
                              if (source.HopDongThauDuocPhamChiTiets.All(x => x.Id != model.Id))
                              {
                                  model.WillDelete = true;
                              }
                          }
                          foreach (var model in source.HopDongThauDuocPhamChiTiets)
                          {
                              if (model.Id == 0)
                              {
                                  var entity = new HopDongThauDuocPhamChiTiet
                                  {
                                      DuocPhamId = model.DuocPhamId.Value,
                                      //HopDongThauDuocPhamId = model.HopDongThauDuocPhamId,
                                      Gia = model.Gia.Value,
                                      SoLuong = model.SoLuong.Value,
                                      SoLuongDaCap = model.SoLuongDaCap
                                  };
                                  dest.HopDongThauDuocPhamChiTiets.Add(entity);
                              }
                              else
                              {
                                  if (dest.HopDongThauDuocPhamChiTiets.Any())
                                  {
                                      var updatedSubItems = source.HopDongThauDuocPhamChiTiets
                                          .Where(sir => dest.HopDongThauDuocPhamChiTiets.Any(si => si.Id == sir.Id)).ToList();
                                      HopDongThauDuocPhamChiTiet entity;
                                      foreach (var sir in updatedSubItems)
                                      {
                                          entity = dest.HopDongThauDuocPhamChiTiets.SingleOrDefault(si => si.Id == sir.Id);
                                          if (entity != null)
                                          {
                                              entity.DuocPhamId = sir.DuocPhamId.Value;
                                              entity.Gia = sir.Gia.Value;
                                              entity.SoLuong = sir.SoLuong.Value;
                                              entity.SoLuongDaCap = sir.SoLuongDaCap;
                                          }
                                      }

                                  }
                              }
                          }
                      });

            CreateMap<HopDongThauDuocPhamGridVo, HopDongThauDuocPhamExportExcel>()
                .AfterMap((source, dest) =>
                {
                    dest.Nam = source.Nam.ToString();
                });

            CreateMap<HopDongThauDuocPhamChiTietGridVo, HopDongThauDuocPhamExportExcelChild>().IgnoreAllNonExisting();
        }
    }
}
