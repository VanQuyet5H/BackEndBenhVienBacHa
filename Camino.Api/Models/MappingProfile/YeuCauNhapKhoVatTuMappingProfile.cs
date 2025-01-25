using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.NhapKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;
using Camino.Core.Domain.ValueObject.YeuCauNhapKhoVatTu;
using System;
using System.Linq;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.HopDongThauVatTus;
using Camino.Services.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class YeuCauNhapKhoVatTuMappingProfile : Profile
    {
        public YeuCauNhapKhoVatTuMappingProfile()
        {
            CreateMap<YeuCauNhapKhoVatTuViewModel, YeuCauNhapKhoVatTu>().IgnoreAllNonExisting()
                .ForMember(d => d.YeuCauNhapKhoVatTuChiTiets, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    if (s.Id == 0)
                    {
                        AddOrUpdate(s, d);
                    }
                    else if (s.Id != 0)
                    {
                        foreach (var model in d.YeuCauNhapKhoVatTuChiTiets.Where(e => e.HopDongThauVatTu != null && e.HopDongThauVatTu.HeThongTuPhatSinh != true))
                        {
                            if (!s.NhapKhoVatTuChiTiets.Any(c => c.Id == model.Id))
                            {
                                model.WillDelete = true;
                            }
                        }

                        foreach (var model in d.YeuCauNhapKhoVatTuChiTiets.Where(e => e.HopDongThauVatTu != null && e.HopDongThauVatTu.HeThongTuPhatSinh == true))
                        {
                            if (!s.NhapKhoVatTuChiTiets.Any(c => c.Id == model.Id))
                            {
                                model.WillDelete = true;
                                if (model.HopDongThauVatTu.HopDongThauVatTuChiTiets.Any(e =>
                                    e.VatTuId == model.VatTuBenhVienId && e.WillDelete != true))
                                {
                                    model.HopDongThauVatTu.HopDongThauVatTuChiTiets
                                            .First(e => e.VatTuId == model.VatTuBenhVienId && e.WillDelete != true)
                                        .SoLuong -= model.SoLuongNhap;
                                    if (model.HopDongThauVatTu.HopDongThauVatTuChiTiets.First(e =>
                                                e.VatTuId == model.VatTuBenhVienId && e.WillDelete != true)
                                            .SoLuong == 0)
                                    {
                                        model.HopDongThauVatTu.HopDongThauVatTuChiTiets
                                            .First(e => e.VatTuId == model.VatTuBenhVienId && e.WillDelete != true)
                                            .WillDelete = true;
                                    }
                                }

                                if (model.HopDongThauVatTu.HopDongThauVatTuChiTiets.All(e => e.WillDelete))
                                {
                                    model.HopDongThauVatTu.WillDelete = true;
                                }
                            }
                        }

                        if (s.NhapKhoVatTuChiTiets.Any(x => x.LoaiNhap == 1))
                        {
                            UpdateTuHopDongThau(s, d);
                        }

                        if (s.NhapKhoVatTuChiTiets.Any(x => x.LoaiNhap == 2))
                        {
                            UpdateTuNhaCungCap(s, d);
                        }
                    }
                });
            CreateMap<YeuCauNhapKhoVatTu, YeuCauNhapKhoVatTuViewModel>().IgnoreAllNonExisting()
                .ForMember(d => d.NhapKhoVatTuChiTiets, o => o.MapFrom(y => y.YeuCauNhapKhoVatTuChiTiets));

            CreateMap<YeuCauNhapKhoKSNKChiTietViewModel, YeuCauNhapKhoVatTuChiTiet>().IgnoreAllNonExisting();
            CreateMap<YeuCauNhapKhoVatTuChiTiet, YeuCauNhapKhoKSNKChiTietViewModel>().IgnoreAllNonExisting().AfterMap((s, d) =>
            {
                d.HopDongThauDisplay = s.HopDongThauVatTu?.SoHopDong;
                d.VatTuDisplay = s.VatTuBenhVien?.VatTus?.Ten;
                d.NhaThauId = s.HopDongThauVatTu?.NhaThauId;
                d.NhaThauDisplay = s.HopDongThauVatTu?.NhaThau?.Ten;
            });

            CreateMap<YeuCauNhapKhoKSNKChiTietViewModel, YeuCauNhapKhoVatTuChiTietGridVo>().IgnoreAllNonExisting();
            CreateMap<YeuCauNhapKhoVatTuChiTietGridVo, YeuCauNhapKhoKSNKChiTietViewModel>().IgnoreAllNonExisting();

        }

        private void AddOrUpdate(YeuCauNhapKhoVatTuViewModel s, YeuCauNhapKhoVatTu d)
        {
            foreach (var model in d.YeuCauNhapKhoVatTuChiTiets)
            {
                if (!s.NhapKhoVatTuChiTiets.Any(c => c.Id == model.Id))
                {
                    model.WillDelete = true;
                }
            }
            foreach (var model in s.NhapKhoVatTuChiTiets)
            {
                if (model.Id == 0)
                {
                    var newEntity = new YeuCauNhapKhoVatTuChiTiet();
                    var yeuCauNhapKhoChiTiet = model.ToEntity(newEntity);
                    if (model.LoaiNhap == 2)
                    {
                        if (d.YeuCauNhapKhoVatTuChiTiets.Any(e => e.HopDongThauVatTu != null &&
                            e.HopDongThauVatTu.NhaThauId == model.NhaThauId && e.HopDongThauVatTu.HeThongTuPhatSinh == true))
                        {
                            yeuCauNhapKhoChiTiet.HopDongThauVatTu = d.YeuCauNhapKhoVatTuChiTiets
                                .First(e => e.HopDongThauVatTu != null && e.HopDongThauVatTu.NhaThauId == model.NhaThauId)
                                .HopDongThauVatTu;

                            if (d.YeuCauNhapKhoVatTuChiTiets
                                .First(e => e.HopDongThauVatTu != null && e.HopDongThauVatTu.NhaThauId == model.NhaThauId)
                                .HopDongThauVatTu.HopDongThauVatTuChiTiets.Any(q =>
                                    q.VatTuId == model.VatTuBenhVienId.GetValueOrDefault()))
                            {
                                d.YeuCauNhapKhoVatTuChiTiets
                                        .First(e => e.HopDongThauVatTu != null && e.HopDongThauVatTu.NhaThauId == model.NhaThauId)
                                        .HopDongThauVatTu.HopDongThauVatTuChiTiets.First(q =>
                                            q.VatTuId == model.VatTuBenhVienId.GetValueOrDefault()).SoLuong +=
                                    model.SoLuongNhap.GetValueOrDefault();
                            }
                            else if (d.YeuCauNhapKhoVatTuChiTiets
                                .First(e => e.HopDongThauVatTu != null && e.HopDongThauVatTu.NhaThauId == model.NhaThauId)
                                .HopDongThauVatTu.HopDongThauVatTuChiTiets.Any(q =>
                                    q.VatTuId != model.VatTuBenhVienId.GetValueOrDefault()))
                            {
                                yeuCauNhapKhoChiTiet.HopDongThauVatTu.HopDongThauVatTuChiTiets.Add(
                                    new HopDongThauVatTuChiTiet
                                    {
                                        Id = 0,
                                        HopDongThauVatTuId = d.YeuCauNhapKhoVatTuChiTiets
                                            .First(e => e.HopDongThauVatTu != null && e.HopDongThauVatTu.NhaThauId == model.NhaThauId)
                                            .HopDongThauVatTuId,
                                        SoLuong = model.SoLuongNhap.GetValueOrDefault(),
                                        VatTuId = model.VatTuBenhVienId.GetValueOrDefault(),
                                        Gia = Convert.ToDecimal(model.DonGiaNhap.GetValueOrDefault()),
                                        SoLuongDaCap = 0
                                    });
                            }
                        }
                        else
                        {
                            var hopDongThauVatTuEntity =
                                new Core.Domain.Entities.HopDongThauVatTus.HopDongThauVatTu
                                {
                                    Id = 0,
                                    NhaThauId = model.NhaThauId.GetValueOrDefault(),
                                    SoHopDong = ResourceHelper.CreateSoHopDong(true),
                                    SoQuyetDinh = ResourceHelper.CreateSoQuyetDinh(true),
                                    CongBo = DateTime.Now,
                                    NgayKy = DateTime.Now,
                                    NgayHieuLuc = DateTime.Now,
                                    NgayHetHan = DateTime.Now,
                                    LoaiThau = Enums.EnumLoaiThau.ThauRieng,
                                    NhomThau = "NTHT01",
                                    GoiThau = "HT",
                                    Nam = DateTime.Now.Year,
                                    HeThongTuPhatSinh = true
                                };
                            yeuCauNhapKhoChiTiet.HopDongThauVatTu = hopDongThauVatTuEntity;
                            var hopDongThauChiTiet = new HopDongThauVatTuChiTiet
                            {
                                Id = 0,
                                HopDongThauVatTuId = hopDongThauVatTuEntity.Id,
                                SoLuong = model.SoLuongNhap.GetValueOrDefault(),
                                VatTuId = model.VatTuBenhVienId.GetValueOrDefault(),
                                Gia = Convert.ToDecimal(model.DonGiaNhap.GetValueOrDefault()),
                                SoLuongDaCap = 0
                            };
                            yeuCauNhapKhoChiTiet.HopDongThauVatTu.HopDongThauVatTuChiTiets.Add(hopDongThauChiTiet);
                        }
                    }
                    d.YeuCauNhapKhoVatTuChiTiets.Add(yeuCauNhapKhoChiTiet);
                }
                else
                {
                    if (d.YeuCauNhapKhoVatTuChiTiets.Any())
                    {
                        var result = d.YeuCauNhapKhoVatTuChiTiets.Single(c => c.Id == model.Id);
                        model.YeuCauNhapKhoVatTuId = d.Id;
                        result = model.ToEntity(result);
                    }
                }
            }
        }

        private void UpdateTuHopDongThau(YeuCauNhapKhoVatTuViewModel s, YeuCauNhapKhoVatTu d)
        {
            foreach (var model in s.NhapKhoVatTuChiTiets.Where(e => e.LoaiNhap == 1))
            {
                if (model.Id == 0)
                {
                    var newEntity = new YeuCauNhapKhoVatTuChiTiet();
                    var yeuCauNhapKhoChiTiet = model.ToEntity(newEntity);
                    d.YeuCauNhapKhoVatTuChiTiets.Add(yeuCauNhapKhoChiTiet);
                }
                else
                {
                    if (d.YeuCauNhapKhoVatTuChiTiets.Any())
                    {
                        var result = d.YeuCauNhapKhoVatTuChiTiets.Single(c => c.Id == model.Id);
                        model.YeuCauNhapKhoVatTuId = d.Id;
                        result = model.ToEntity(result);
                    }
                }
            }
        }

        private void UpdateTuNhaCungCap(YeuCauNhapKhoVatTuViewModel s, YeuCauNhapKhoVatTu d)
        {
            foreach (var model in s.NhapKhoVatTuChiTiets.Where(e => e.LoaiNhap == 2 && e.Id != 0))
            {
                // dk nếu chỉnh sửa nhưng ko thay đổi nhà thầu
                if (d.YeuCauNhapKhoVatTuChiTiets.Any(c => c.Id == model.Id) &&
                s.OldNhapKhoVatTuChiTiets.Any(e => e.Id == model.Id && e.LoaiNhap == 2
                                                                           && e.NhaThauId == model.NhaThauId))
                {
                    var result = d.YeuCauNhapKhoVatTuChiTiets.Single(c => c.Id == model.Id);
                    model.YeuCauNhapKhoVatTuId = d.Id;
                    model.DaCapNhat = true;
                    result = model.ToEntity(result);
                    result.HopDongThauVatTu.NhaThauId = model.NhaThauId.GetValueOrDefault();
                    result.HopDongThauVatTu.HopDongThauVatTuChiTiets.First(e =>
                            e.VatTuId == s.OldNhapKhoVatTuChiTiets.First(t => t.Id == model.Id).VatTuBenhVienId)
                        .VatTuId = model.VatTuBenhVienId.GetValueOrDefault();

                    if (result.HopDongThauVatTu.HopDongThauVatTuChiTiets.Where(e =>
                            e.VatTuId == model.VatTuBenhVienId.GetValueOrDefault()).Count() > 1)
                    {
                        var ignoreFirst = false;
                        foreach (var hopDongChiTietItem in result.HopDongThauVatTu.HopDongThauVatTuChiTiets.Where(e =>
                            e.VatTuId == model.VatTuBenhVienId.GetValueOrDefault()))
                        {
                            if (ignoreFirst)
                            {
                                hopDongChiTietItem.WillDelete = true;
                            }

                            ignoreFirst = true;
                        }
                    }

                    // cập nhật lại số lượng cho hợp đồng thầu chi tiết có phát sinh
                    if (model.DaCongDon != true)
                    {
                        result.HopDongThauVatTu.HopDongThauVatTuChiTiets.First(e =>
                                e.VatTuId == s.OldNhapKhoVatTuChiTiets.First(t => t.Id == model.Id)
                                    .VatTuBenhVienId)
                            .SoLuong = s.NhapKhoVatTuChiTiets.Where(e => e.LoaiNhap == 2 &&
                                                                            e.VatTuBenhVienId ==
                                                                            model.VatTuBenhVienId &&
                                                                            e.NhaThauId == model.NhaThauId).Sum(q =>
                            q.SoLuongNhap.GetValueOrDefault());
                        foreach (var nhapKhoDuocPhamChiTiet in s.NhapKhoVatTuChiTiets.Where(e => e.LoaiNhap == 2 &&
                                                                                                    e.VatTuBenhVienId ==
                                                                                                    model.VatTuBenhVienId &&
                                                                                                    e.NhaThauId == model.NhaThauId))
                        {
                            nhapKhoDuocPhamChiTiet.DaCongDon = true;
                        }
                    }
                }
            }

            // những yêu cầu nhập kho qua chỉnh sửa nhưng có thay đổi nhà cung cấp
            foreach (var model in s.NhapKhoVatTuChiTiets.Where(e => e.LoaiNhap == 2 && e.Id != 0 && e.DaCapNhat != true))
            {
                if (d.YeuCauNhapKhoVatTuChiTiets.Any(c => c.Id == model.Id) &&
                    s.OldNhapKhoVatTuChiTiets.Any(e => e.Id == model.Id && e.LoaiNhap == 2
                                                                           && e.NhaThauId != model.NhaThauId))
                {
                    var result = d.YeuCauNhapKhoVatTuChiTiets.Single(c => c.Id == model.Id);
                    model.YeuCauNhapKhoVatTuId = d.Id;
                    result = model.ToEntity(result);
                    // nếu có chỉnh sửa nhà thầu nhưng nhà thầu hiện tồn tại trong hệ thống
                    if (s.NhapKhoVatTuChiTiets.Where(w => w.LoaiNhap == 2 && w.DaCapNhat != true && w.Id != 0)
                        .Any(e => s.OldNhapKhoVatTuChiTiets != null && e.NhaThauId == s.OldNhapKhoVatTuChiTiets.First(
                                      q => q.LoaiNhap == 2 && q.Id == model.Id).NhaThauId))
                    {
                        var soLuongDaCapCurrent = result.HopDongThauVatTu.HopDongThauVatTuChiTiets.First(e => s.OldNhapKhoVatTuChiTiets != null &&
                            e.VatTuId == s.OldNhapKhoVatTuChiTiets.First(t => t.Id == model.Id)
                                .VatTuBenhVienId).SoLuongDaCap;
                        result.HopDongThauVatTu.HopDongThauVatTuChiTiets.First(e => s.OldNhapKhoVatTuChiTiets != null &&
                                e.VatTuId == s.OldNhapKhoVatTuChiTiets.First(t => t.Id == model.Id)
                                    .VatTuBenhVienId)
                            .WillDelete = true;
                        if (result.HopDongThauVatTu.HopDongThauVatTuChiTiets.All(e => e.WillDelete))
                        {
                            result.HopDongThauVatTu.WillDelete = true;
                        }
                        result.HopDongThauVatTuId = d.YeuCauNhapKhoVatTuChiTiets.First(e => s.NhapKhoVatTuChiTiets != null && e.Id ==
                            s.NhapKhoVatTuChiTiets.First(w => w.LoaiNhap == 2 && w.DaCapNhat && w.NhaThauId ==
                                model.NhaThauId).Id).HopDongThauVatTuId;

                        result.HopDongThauVatTu = d.YeuCauNhapKhoVatTuChiTiets.First(e => s.NhapKhoVatTuChiTiets != null && e.Id ==
                            s.NhapKhoVatTuChiTiets.First(w => w.LoaiNhap == 2 && w.DaCapNhat && w.NhaThauId ==
                                model.NhaThauId).Id).HopDongThauVatTu;

                        // nếu như nhà thầu đó đang có dược phẩm trùng trong hệ thống sắp lưu
                        if (d.YeuCauNhapKhoVatTuChiTiets.First(e => e.HopDongThauVatTu != null &&
                                                                       e.HopDongThauVatTu.HeThongTuPhatSinh ==
                                                                       true && e.HopDongThauVatTu.NhaThauId ==
                                                                       model.NhaThauId
                                                                       && e.HopDongThauVatTu.WillDelete != true)
                            .HopDongThauVatTu.HopDongThauVatTuChiTiets.Any(q =>
                                q.VatTuId == model.VatTuBenhVienId && q.WillDelete != true))
                        {
                            if (model.DaCongDon != true)
                            {
                                d.YeuCauNhapKhoVatTuChiTiets.First(e => e.HopDongThauVatTu != null &&
                                                                           e.HopDongThauVatTu.HeThongTuPhatSinh == true &&
                                                                           e.HopDongThauVatTu.NhaThauId == model.NhaThauId
                                                                           && e.HopDongThauVatTu.WillDelete != true)
                                        .HopDongThauVatTu.HopDongThauVatTuChiTiets
                                        .First(q => q.VatTuId == model.VatTuBenhVienId && q.WillDelete != true).SoLuong +=
                                    model.SoLuongNhap.GetValueOrDefault();
                            }
                        }
                        // nếu như nhà thầu đó đang có dược phẩm nhưng ko trùng trong hệ thống sắp lưu
                        else if (d.YeuCauNhapKhoVatTuChiTiets.First(e => e.HopDongThauVatTu != null &&
                                                                            e.HopDongThauVatTu.HeThongTuPhatSinh ==
                                                                            true && e.HopDongThauVatTu.NhaThauId ==
                                                                            model.NhaThauId
                                                                            && e.HopDongThauVatTu.WillDelete != true)
                            .HopDongThauVatTu.HopDongThauVatTuChiTiets.Any(q =>
                                q.VatTuId != model.VatTuBenhVienId && q.WillDelete != true))
                        {
                            var hopDongThauChiTiet = new HopDongThauVatTuChiTiet
                            {
                                Id = 0,
                                HopDongThauVatTuId = d.YeuCauNhapKhoVatTuChiTiets.First(e => s.NhapKhoVatTuChiTiets != null && e.Id ==
                                                                                             s.NhapKhoVatTuChiTiets.First(w => w.LoaiNhap == 2 && w.DaCapNhat && w.NhaThauId ==
                                                                                                                               model.NhaThauId).Id).HopDongThauVatTuId,
                                SoLuong = model.SoLuongNhap.GetValueOrDefault(),
                                VatTuId = model.VatTuBenhVienId.GetValueOrDefault(),
                                Gia = Convert.ToDecimal(model.DonGiaNhap.GetValueOrDefault()),
                                SoLuongDaCap = soLuongDaCapCurrent
                            };
                            result.HopDongThauVatTu.HopDongThauVatTuChiTiets.Add(hopDongThauChiTiet);
                        }
                    }
                    // nếu có chỉnh sửa nhà thầu nhưng nhà thầu hiện không tồn tại trong hệ thống
                    else if (s.NhapKhoVatTuChiTiets.Where(w => w.LoaiNhap == 2 && w.DaCapNhat != true && w.Id != 0)
                        .Any(e => s.OldNhapKhoVatTuChiTiets != null && e.NhaThauId != s.OldNhapKhoVatTuChiTiets.First(
                                      q => q.LoaiNhap == 2 && q.Id == model.Id).NhaThauId))
                    {
                        var hopDongThauVatTuEntity =
                            new Core.Domain.Entities.HopDongThauVatTus.HopDongThauVatTu
                            {
                                Id = 0,
                                NhaThauId = model.NhaThauId.GetValueOrDefault(),
                                SoHopDong = ResourceHelper.CreateSoHopDong(true),
                                SoQuyetDinh = ResourceHelper.CreateSoQuyetDinh(true),
                                CongBo = DateTime.Now,
                                NgayKy = DateTime.Now,
                                NgayHieuLuc = DateTime.Now,
                                NgayHetHan = DateTime.Now,
                                LoaiThau = Enums.EnumLoaiThau.ThauRieng,
                                NhomThau = "NTHT01",
                                GoiThau = "HT",
                                Nam = DateTime.Now.Year,
                                HeThongTuPhatSinh = true
                            };
                        var soLuongDaCapCurrent = result.HopDongThauVatTu.HopDongThauVatTuChiTiets.First(e => s.OldNhapKhoVatTuChiTiets != null &&
                            e.VatTuId == s.OldNhapKhoVatTuChiTiets.First(t => t.Id == model.Id)
                                .VatTuBenhVienId).SoLuongDaCap;
                        result.HopDongThauVatTu.HopDongThauVatTuChiTiets.First(e => s.OldNhapKhoVatTuChiTiets != null &&
                            e.VatTuId == s.OldNhapKhoVatTuChiTiets.First(t => t.Id == model.Id)
                                .VatTuBenhVienId).SoLuong -= model.SoLuongNhap.GetValueOrDefault();
                        if (result.HopDongThauVatTu.HopDongThauVatTuChiTiets.First(e => s.OldNhapKhoVatTuChiTiets != null &&
                                e.VatTuId == s.OldNhapKhoVatTuChiTiets.First(t => t.Id == model.Id)
                                    .VatTuBenhVienId).SoLuong == 0)
                        {
                            result.HopDongThauVatTu.HopDongThauVatTuChiTiets.First(e => s.OldNhapKhoVatTuChiTiets != null &&
                                    e.VatTuId == s.OldNhapKhoVatTuChiTiets.First(t => t.Id == model.Id)
                                        .VatTuBenhVienId)
                                .WillDelete = true;
                        }
                        if (result.HopDongThauVatTu.HopDongThauVatTuChiTiets.All(e => e.WillDelete))
                        {
                            result.HopDongThauVatTu.WillDelete = true;
                        }

                        result.HopDongThauVatTuId = 0;
                        result.HopDongThauVatTu = hopDongThauVatTuEntity;
                        var hopDongThauChiTiet = new HopDongThauVatTuChiTiet
                        {
                            Id = 0,
                            HopDongThauVatTuId = hopDongThauVatTuEntity.Id,
                            SoLuong = model.SoLuongNhap.GetValueOrDefault(),
                            VatTuId = model.VatTuBenhVienId.GetValueOrDefault(),
                            Gia = Convert.ToDecimal(model.DonGiaNhap.GetValueOrDefault()),
                            SoLuongDaCap = soLuongDaCapCurrent
                        };
                        result.HopDongThauVatTu.HopDongThauVatTuChiTiets.Add(hopDongThauChiTiet);
                    }
                }
            }
            foreach (var model in s.NhapKhoVatTuChiTiets.Where(e => e.LoaiNhap == 2 && e.Id == 0))
            {
                var newEntity = new YeuCauNhapKhoVatTuChiTiet();
                var yeuCauNhapKhoChiTiet = model.ToEntity(newEntity);
                // thêm yêu cầu nhập kho chi tiết từ ncc mà có nhà thầu tồn tại trong hệ thống
                if (d.YeuCauNhapKhoVatTuChiTiets.Any(e => e.HopDongThauVatTu != null &&
                    e.HopDongThauVatTu.HeThongTuPhatSinh == true &&
                    e.HopDongThauVatTu.NhaThauId == model.NhaThauId && e.HopDongThauVatTu.WillDelete != true))
                {
                    yeuCauNhapKhoChiTiet.HopDongThauVatTuId = d.YeuCauNhapKhoVatTuChiTiets.First(e => e.HopDongThauVatTu != null &&
                        e.HopDongThauVatTu.HeThongTuPhatSinh == true &&
                        e.HopDongThauVatTu.NhaThauId == model.NhaThauId).HopDongThauVatTuId;
                    yeuCauNhapKhoChiTiet.HopDongThauVatTu = d.YeuCauNhapKhoVatTuChiTiets.First(e => e.HopDongThauVatTu != null &&
                        e.HopDongThauVatTu.HeThongTuPhatSinh == true &&
                        e.HopDongThauVatTu.NhaThauId == model.NhaThauId).HopDongThauVatTu;

                    // nếu như nhà thầu đó đang có dược phẩm trùng trong hệ thống sắp lưu
                    if (d.YeuCauNhapKhoVatTuChiTiets.First(e => e.HopDongThauVatTu != null &&
                                                                e.HopDongThauVatTu.HeThongTuPhatSinh == true &&
                                                                e.HopDongThauVatTu.NhaThauId == model.NhaThauId
                                                                && e.HopDongThauVatTu.WillDelete != true)
                        .HopDongThauVatTu.HopDongThauVatTuChiTiets
                        .Any(q => q.VatTuId == model.VatTuBenhVienId && q.WillDelete != true))
                    {
                        if (model.DaCongDon != true)
                        {
                            d.YeuCauNhapKhoVatTuChiTiets.First(e => e.HopDongThauVatTu != null &&
                                                                       e.HopDongThauVatTu.HeThongTuPhatSinh == true &&
                                                                       e.HopDongThauVatTu.NhaThauId == model.NhaThauId
                                                                       && e.HopDongThauVatTu.WillDelete != true)
                                    .HopDongThauVatTu.HopDongThauVatTuChiTiets
                                    .First(q => q.VatTuId == model.VatTuBenhVienId && q.WillDelete != true).SoLuong +=
                                model.SoLuongNhap.GetValueOrDefault();
                        }
                        var hopDongThauChiTiet = new HopDongThauVatTuChiTiet
                        {
                            Id = 0,
                            HopDongThauVatTuId = d.YeuCauNhapKhoVatTuChiTiets.First(e => e.HopDongThauVatTu != null &&
                                                                                         e.HopDongThauVatTu.HeThongTuPhatSinh == true &&
                                                                                         e.HopDongThauVatTu.NhaThauId == model.NhaThauId).HopDongThauVatTuId,
                            SoLuong = model.SoLuongNhap.GetValueOrDefault(),
                            VatTuId = model.VatTuBenhVienId.GetValueOrDefault(),
                            Gia = Convert.ToDecimal(model.DonGiaNhap.GetValueOrDefault()),
                            SoLuongDaCap = 0
                        };

                        yeuCauNhapKhoChiTiet.HopDongThauVatTu.HopDongThauVatTuChiTiets.Add(hopDongThauChiTiet);
                    }
                    // nếu như nhà thầu đó đang có dược phẩm nhưng ko trùng trong hệ thống sắp lưu
                    else if (d.YeuCauNhapKhoVatTuChiTiets.First(e => e.HopDongThauVatTu != null &&
                                                                        e.HopDongThauVatTu.HeThongTuPhatSinh ==
                                                                        true && e.HopDongThauVatTu.NhaThauId ==
                                                                        model.NhaThauId
                                                                        && e.HopDongThauVatTu.WillDelete != true)
                        .HopDongThauVatTu.HopDongThauVatTuChiTiets.Any(q =>
                            q.VatTuId != model.VatTuBenhVienId && q.WillDelete != true))
                    {
                        var hopDongThauChiTiet = new HopDongThauVatTuChiTiet
                        {
                            Id = 0,
                            HopDongThauVatTuId = d.YeuCauNhapKhoVatTuChiTiets.First(e => e.HopDongThauVatTu != null &&
                                e.HopDongThauVatTu.HeThongTuPhatSinh == true &&
                                e.HopDongThauVatTu.NhaThauId == model.NhaThauId).HopDongThauVatTuId,
                            SoLuong = model.SoLuongNhap.GetValueOrDefault(),
                            VatTuId = model.VatTuBenhVienId.GetValueOrDefault(),
                            Gia = Convert.ToDecimal(model.DonGiaNhap.GetValueOrDefault()),
                            SoLuongDaCap = 0
                        };

                        yeuCauNhapKhoChiTiet.HopDongThauVatTu.HopDongThauVatTuChiTiets.Add(hopDongThauChiTiet);
                    }
                }
                // thêm yêu cầu nhập kho chi tiết từ ncc mà có nhà thầu vẫn chưa tồn tại trong hệ thống hoặc vẫn chưa có nhà cung cấp nào trong hệ thống
                else if (d.YeuCauNhapKhoVatTuChiTiets.Any(e => e.HopDongThauVatTu != null &&
                   e.HopDongThauVatTu.HeThongTuPhatSinh == true &&
                   e.HopDongThauVatTu.NhaThauId != model.NhaThauId && e.HopDongThauVatTu.WillDelete != true) ||
                         !d.YeuCauNhapKhoVatTuChiTiets.Where(e => e.HopDongThauVatTu != null && e.HopDongThauVatTu.WillDelete != true)
                             .Any(e => e.HopDongThauVatTu != null && e.HopDongThauVatTu.HeThongTuPhatSinh == true))
                {
                    var hopDongThauDuocPhamEntity =
                                new Core.Domain.Entities.HopDongThauVatTus.HopDongThauVatTu
                                {
                                    Id = 0,
                                    NhaThauId = model.NhaThauId.GetValueOrDefault(),
                                    SoHopDong = ResourceHelper.CreateSoHopDong(true),
                                    SoQuyetDinh = ResourceHelper.CreateSoQuyetDinh(true),
                                    CongBo = DateTime.Now,
                                    NgayKy = DateTime.Now,
                                    NgayHieuLuc = DateTime.Now,
                                    NgayHetHan = DateTime.Now,
                                    LoaiThau = Enums.EnumLoaiThau.ThauRieng,
                                    NhomThau = "NTHT01",
                                    GoiThau = "HT",
                                    Nam = DateTime.Now.Year,
                                    HeThongTuPhatSinh = true
                                };
                    yeuCauNhapKhoChiTiet.HopDongThauVatTu = hopDongThauDuocPhamEntity;
                    var hopDongThauChiTiet = new HopDongThauVatTuChiTiet
                    {
                        Id = 0,
                        HopDongThauVatTuId = hopDongThauDuocPhamEntity.Id,
                        SoLuong = model.SoLuongNhap.GetValueOrDefault(),
                        VatTuId = model.VatTuBenhVienId.GetValueOrDefault(),
                        Gia = Convert.ToDecimal(model.DonGiaNhap.GetValueOrDefault()),
                        SoLuongDaCap = 0
                    };
                    yeuCauNhapKhoChiTiet.HopDongThauVatTu.HopDongThauVatTuChiTiets.Add(hopDongThauChiTiet);
                }
                d.YeuCauNhapKhoVatTuChiTiets.Add(yeuCauNhapKhoChiTiet);
            }
        }
    }
}
