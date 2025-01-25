using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.NhapKhoDuocPhams;
using Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams;
using Camino.Core.Domain.ValueObject.YeuCauNhapKhoDuocPham;
using System;
using System.Linq;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.HopDongThauDuocPhams;
using Camino.Services.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class YeuCauNhapKhoDuocPhamMappingProfile : Profile
    {
        public YeuCauNhapKhoDuocPhamMappingProfile()
        {
            CreateMap<YeuCauNhapKhoDuocPhamViewModel, YeuCauNhapKhoDuocPham>().IgnoreAllNonExisting()
                .ForMember(d => d.YeuCauNhapKhoDuocPhamChiTiets, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    if (s.Id == 0)
                    {
                        AddOrUpdate(s, d);
                    }
                    else if (s.Id != 0)
                    {
                        foreach (var model in d.YeuCauNhapKhoDuocPhamChiTiets.Where(e => e.HopDongThauDuocPham != null && e.HopDongThauDuocPham.HeThongTuPhatSinh != true))
                        {
                            if (!s.NhapKhoDuocPhamChiTiets.Any(c => c.Id == model.Id))
                            {
                                model.WillDelete = true;
                            }
                        }

                        foreach (var model in d.YeuCauNhapKhoDuocPhamChiTiets.Where(e => e.HopDongThauDuocPham != null && e.HopDongThauDuocPham.HeThongTuPhatSinh == true))
                        {
                            if (!s.NhapKhoDuocPhamChiTiets.Any(c => c.Id == model.Id))
                            {
                                model.WillDelete = true;

                                if (model.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.Any(e =>
                                    e.DuocPhamId == model.DuocPhamBenhVienId && e.WillDelete != true))
                                {
                                    model.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.First(e =>
                                            e.DuocPhamId == model.DuocPhamBenhVienId && e.WillDelete != true)
                                        .SoLuong -= model.SoLuongNhap;
                                    if (model.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.First(e =>
                                                e.DuocPhamId == model.DuocPhamBenhVienId && e.WillDelete != true)
                                            .SoLuong == 0)
                                    {
                                        model.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.First(e =>
                                                e.DuocPhamId == model.DuocPhamBenhVienId && e.WillDelete != true)
                                            .WillDelete = true;
                                    }
                                }

                                if (model.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.All(e => e.WillDelete))
                                {
                                    model.HopDongThauDuocPham.WillDelete = true;
                                }
                            }
                        }

                        if (s.NhapKhoDuocPhamChiTiets.Any(x => x.LoaiNhap == 1))
                        {
                            UpdateTuHopDongThau(s, d);
                        }

                        if (s.NhapKhoDuocPhamChiTiets.Any(x => x.LoaiNhap == 2))
                        {
                            UpdateTuNhaCungCap(s, d);
                        }
                    }
                });
            CreateMap<YeuCauNhapKhoDuocPham, YeuCauNhapKhoDuocPhamViewModel>().IgnoreAllNonExisting()
                .ForMember(d => d.NhapKhoDuocPhamChiTiets, o => o.MapFrom(y => y.YeuCauNhapKhoDuocPhamChiTiets));

            CreateMap<YeuCauNhapKhoDuocPhamChiTietViewModel, YeuCauNhapKhoDuocPhamChiTiet>().IgnoreAllNonExisting();
            CreateMap<YeuCauNhapKhoDuocPhamChiTiet, YeuCauNhapKhoDuocPhamChiTietViewModel>().IgnoreAllNonExisting().AfterMap((s, d) =>
            {
                d.HopDongThauDisplay = s.HopDongThauDuocPham?.SoHopDong;
                d.DuocPhamDisplay = s.DuocPhamBenhVien?.DuocPham?.Ten;
                d.NhaThauId = s.HopDongThauDuocPham?.NhaThauId;
                d.NhaThauDisplay = s.HopDongThauDuocPham?.NhaThau?.Ten;
            });


            CreateMap<YeuCauNhapKhoDuocPhamChiTietViewModel, YeuCauNhapKhoDuocPhamChiTietGridVo>().IgnoreAllNonExisting();
            CreateMap<YeuCauNhapKhoDuocPhamChiTietGridVo, YeuCauNhapKhoDuocPhamChiTietViewModel>().IgnoreAllNonExisting();

        }



        private void AddOrUpdate(YeuCauNhapKhoDuocPhamViewModel s, YeuCauNhapKhoDuocPham d)
        {
            foreach (var model in d.YeuCauNhapKhoDuocPhamChiTiets)
            {
                if (!s.NhapKhoDuocPhamChiTiets.Any(c => c.Id == model.Id))
                {
                    model.WillDelete = true;
                }
            }
            foreach (var model in s.NhapKhoDuocPhamChiTiets)
            {
                if (model.Id == 0)
                {
                    var newEntity = new YeuCauNhapKhoDuocPhamChiTiet();
                    var yeuCauNhapKhoChiTiet = model.ToEntity(newEntity);

                    if (model.LoaiNhap == 2)
                    {
                        if (d.YeuCauNhapKhoDuocPhamChiTiets.Any(e =>
                            e.HopDongThauDuocPham != null && e.HopDongThauDuocPham.NhaThauId == model.NhaThauId && e.HopDongThauDuocPham.HeThongTuPhatSinh == true))
                        {
                            yeuCauNhapKhoChiTiet.HopDongThauDuocPham = d.YeuCauNhapKhoDuocPhamChiTiets
                                .First(e => e.HopDongThauDuocPham != null && e.HopDongThauDuocPham.NhaThauId == model.NhaThauId)
                                .HopDongThauDuocPham;

                            if (d.YeuCauNhapKhoDuocPhamChiTiets
                                .First(e => e.HopDongThauDuocPham != null && e.HopDongThauDuocPham.NhaThauId == model.NhaThauId)
                                .HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.Any(q =>
                                    q.DuocPhamId == model.DuocPhamBenhVienId.GetValueOrDefault()))
                            {
                                d.YeuCauNhapKhoDuocPhamChiTiets
                                        .First(e => e.HopDongThauDuocPham != null && e.HopDongThauDuocPham.NhaThauId == model.NhaThauId)
                                        .HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.First(q =>
                                            q.DuocPhamId == model.DuocPhamBenhVienId.GetValueOrDefault()).SoLuong +=
                                    model.SoLuongNhap.GetValueOrDefault();
                            }
                            else if (d.YeuCauNhapKhoDuocPhamChiTiets
                                .First(e => e.HopDongThauDuocPham != null && e.HopDongThauDuocPham.NhaThauId == model.NhaThauId)
                                .HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.Any(q =>
                                    q.DuocPhamId != model.DuocPhamBenhVienId.GetValueOrDefault()))
                            {
                                yeuCauNhapKhoChiTiet.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.Add(new HopDongThauDuocPhamChiTiet
                                {
                                    Id = 0,
                                    HopDongThauDuocPhamId = d.YeuCauNhapKhoDuocPhamChiTiets
                                        .First(e => e.HopDongThauDuocPham != null && e.HopDongThauDuocPham.NhaThauId == model.NhaThauId).HopDongThauDuocPham
                                        .Id,
                                    SoLuong = model.SoLuongNhap.GetValueOrDefault(),
                                    DuocPhamId = model.DuocPhamBenhVienId.GetValueOrDefault(),
                                    Gia = Convert.ToDecimal(model.DonGiaNhap.GetValueOrDefault()),
                                    SoLuongDaCap = 0
                                });
                            }
                        }
                        else
                        {
                            var hopDongThauDuocPhamEntity =
                                new Core.Domain.Entities.HopDongThauDuocPhams.HopDongThauDuocPham
                                {
                                    Id = 0,
                                    NhaThauId = model.NhaThauId.GetValueOrDefault(),
                                    SoHopDong = ResourceHelper.CreateSoHopDong(),
                                    SoQuyetDinh = ResourceHelper.CreateSoQuyetDinh(),
                                    CongBo = DateTime.Now,
                                    NgayKy = DateTime.Now,
                                    NgayHieuLuc = DateTime.Now,
                                    NgayHetHan = DateTime.Now,
                                    LoaiThau = Enums.EnumLoaiThau.ThauRieng,
                                    LoaiThuocThau = Enums.EnumLoaiThuocThau.TongHop,
                                    NhomThau = "NTHT01",
                                    GoiThau = "HT",
                                    Nam = DateTime.Now.Year,
                                    HeThongTuPhatSinh = true
                                };
                            yeuCauNhapKhoChiTiet.HopDongThauDuocPham = hopDongThauDuocPhamEntity;
                            var hopDongThauChiTiet = new HopDongThauDuocPhamChiTiet
                            {
                                Id = 0,
                                HopDongThauDuocPhamId = hopDongThauDuocPhamEntity.Id,
                                SoLuong = model.SoLuongNhap.GetValueOrDefault(),
                                DuocPhamId = model.DuocPhamBenhVienId.GetValueOrDefault(),
                                Gia = Convert.ToDecimal(model.DonGiaNhap.GetValueOrDefault()),
                                SoLuongDaCap = 0
                            };
                            yeuCauNhapKhoChiTiet.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.Add(hopDongThauChiTiet);
                        }
                    }
                    d.YeuCauNhapKhoDuocPhamChiTiets.Add(yeuCauNhapKhoChiTiet);
                }
                else
                {
                    if (d.YeuCauNhapKhoDuocPhamChiTiets.Any())
                    {
                        var result = d.YeuCauNhapKhoDuocPhamChiTiets.Single(c => c.Id == model.Id);
                        model.YeuCauNhapKhoDuocPhamId = d.Id;
                        result = model.ToEntity(result);

                    }
                }
            }
        }

        private void UpdateTuHopDongThau(YeuCauNhapKhoDuocPhamViewModel s, YeuCauNhapKhoDuocPham d)
        {
            foreach (var model in s.NhapKhoDuocPhamChiTiets.Where(e => e.LoaiNhap == 1))
            {
                if (model.Id == 0)
                {
                    var newEntity = new YeuCauNhapKhoDuocPhamChiTiet();
                    var yeuCauNhapKhoChiTiet = model.ToEntity(newEntity);
                    d.YeuCauNhapKhoDuocPhamChiTiets.Add(yeuCauNhapKhoChiTiet);
                }
                else
                {
                    if (d.YeuCauNhapKhoDuocPhamChiTiets.Any())
                    {
                        var result = d.YeuCauNhapKhoDuocPhamChiTiets.Single(c => c.Id == model.Id);
                        model.YeuCauNhapKhoDuocPhamId = d.Id;
                        result = model.ToEntity(result);

                    }
                }
            }
        }

        private void UpdateTuNhaCungCap(YeuCauNhapKhoDuocPhamViewModel s, YeuCauNhapKhoDuocPham d)
        {
            foreach (var model in s.NhapKhoDuocPhamChiTiets.Where(e => e.LoaiNhap == 2 && e.Id != 0))
            {
                // dk nếu chỉnh sửa nhưng ko thay đổi nhà thầu
                if (d.YeuCauNhapKhoDuocPhamChiTiets.Any(c => c.Id == model.Id) &&
                    s.OldNhapKhoDuocPhamChiTiets.Any(e => e.Id == model.Id && e.LoaiNhap == 2
                                                                           && e.NhaThauId == model.NhaThauId))
                {
                    var result = d.YeuCauNhapKhoDuocPhamChiTiets.Single(c => c.Id == model.Id);
                    model.YeuCauNhapKhoDuocPhamId = d.Id;
                    model.DaCapNhat = true;
                    result = model.ToEntity(result);
                    result.HopDongThauDuocPham.NhaThauId = model.NhaThauId.GetValueOrDefault();
                    result.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.First(e => s.OldNhapKhoDuocPhamChiTiets != null &&
                            e.DuocPhamId == s.OldNhapKhoDuocPhamChiTiets.First(t => t.Id == model.Id).DuocPhamBenhVienId)
                        .DuocPhamId = model.DuocPhamBenhVienId.GetValueOrDefault();

                    if (result.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.Where(e =>
                            e.DuocPhamId == model.DuocPhamBenhVienId.GetValueOrDefault()).Count() > 1)
                    {
                        var ignoreFirst = false;
                        foreach (var hopDongChiTietItem in result.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.Where(e =>
                            e.DuocPhamId == model.DuocPhamBenhVienId.GetValueOrDefault()))
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
                        result.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.First(e => s.OldNhapKhoDuocPhamChiTiets != null &&
                                e.DuocPhamId == s.OldNhapKhoDuocPhamChiTiets.First(t => t.Id == model.Id)
                                    .DuocPhamBenhVienId)
                            .SoLuong = s.NhapKhoDuocPhamChiTiets.Where(e => e.LoaiNhap == 2 &&
                                                                            e.DuocPhamBenhVienId ==
                                                                            model.DuocPhamBenhVienId &&
                                                                            e.NhaThauId == model.NhaThauId).Sum(q =>
                            q.SoLuongNhap.GetValueOrDefault());
                        foreach (var nhapKhoDuocPhamChiTiet in s.NhapKhoDuocPhamChiTiets.Where(e => e.LoaiNhap == 2 &&
                                                                                      e.DuocPhamBenhVienId ==
                                                                                      model.DuocPhamBenhVienId &&
                                                                                      e.NhaThauId == model.NhaThauId))
                        {
                            nhapKhoDuocPhamChiTiet.DaCongDon = true;
                        }
                    }
                }
            }

            // những yêu cầu nhập kho qua chỉnh sửa nhưng có thay đổi nhà cung cấp
            foreach (var model in s.NhapKhoDuocPhamChiTiets.Where(e => e.LoaiNhap == 2 && e.Id != 0 && e.DaCapNhat != true))
            {
                if (d.YeuCauNhapKhoDuocPhamChiTiets.Any(c => c.Id == model.Id) &&
                    s.OldNhapKhoDuocPhamChiTiets.Any(e => e.Id == model.Id && e.LoaiNhap == 2
                                                                           && e.NhaThauId != model.NhaThauId))
                {
                    var result = d.YeuCauNhapKhoDuocPhamChiTiets.Single(c => c.Id == model.Id);
                    model.YeuCauNhapKhoDuocPhamId = d.Id;
                    result = model.ToEntity(result);
                    // nếu có chỉnh sửa nhà thầu nhưng nhà thầu hiện tồn tại trong hệ thống
                    if (s.NhapKhoDuocPhamChiTiets.Where(w => w.LoaiNhap == 2 && w.DaCapNhat != true && w.Id != 0)
                        .Any(e => s.OldNhapKhoDuocPhamChiTiets != null && e.NhaThauId == s.OldNhapKhoDuocPhamChiTiets.First(
                                      q => q.LoaiNhap == 2 && q.Id == model.Id).NhaThauId))
                    {
                        var soLuongDaCapCurrent = result.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.First(e => s.OldNhapKhoDuocPhamChiTiets != null &&
                            e.DuocPhamId == s.OldNhapKhoDuocPhamChiTiets.First(t => t.Id == model.Id)
                                .DuocPhamBenhVienId).SoLuongDaCap;
                        result.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.First(e =>
                                e.DuocPhamId == s.OldNhapKhoDuocPhamChiTiets.First(t => t.Id == model.Id)
                                    .DuocPhamBenhVienId)
                            .WillDelete = true;
                        if (result.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.All(e => e.WillDelete))
                        {
                            result.HopDongThauDuocPham.WillDelete = true;
                        }
                        result.HopDongThauDuocPhamId = d.YeuCauNhapKhoDuocPhamChiTiets.First(e => s.OldNhapKhoDuocPhamChiTiets != null && e.Id ==
                            s.NhapKhoDuocPhamChiTiets.First(w => w.LoaiNhap == 2 && w.DaCapNhat && w.NhaThauId ==
                                model.NhaThauId).Id).HopDongThauDuocPhamId;

                        result.HopDongThauDuocPham = d.YeuCauNhapKhoDuocPhamChiTiets.First(e => s.OldNhapKhoDuocPhamChiTiets != null && e.Id ==
                            s.NhapKhoDuocPhamChiTiets.First(w => w.LoaiNhap == 2 && w.DaCapNhat && w.NhaThauId ==
                                model.NhaThauId).Id).HopDongThauDuocPham;

                        // nếu như nhà thầu đó đang có dược phẩm trùng trong hệ thống sắp lưu
                        if (d.YeuCauNhapKhoDuocPhamChiTiets.First(e => e.HopDongThauDuocPham != null &&
                           e.HopDongThauDuocPham.HeThongTuPhatSinh == true && e.HopDongThauDuocPham.NhaThauId == model.NhaThauId
                          && e.HopDongThauDuocPham.WillDelete != true).HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.
                          Any(q => q.DuocPhamId == model.DuocPhamBenhVienId && q.WillDelete != true))
                        {
                            if (model.DaCongDon != true)
                            {
                                d.YeuCauNhapKhoDuocPhamChiTiets.First(e => e.HopDongThauDuocPham != null &&
                                                                           e.HopDongThauDuocPham.HeThongTuPhatSinh == true &&
                                                                           e.HopDongThauDuocPham.NhaThauId == model.NhaThauId
                                                                           && e.HopDongThauDuocPham.WillDelete != true)
                                        .HopDongThauDuocPham.HopDongThauDuocPhamChiTiets
                                        .First(q => q.DuocPhamId == model.DuocPhamBenhVienId && q.WillDelete != true).SoLuong +=
                                    model.SoLuongNhap.GetValueOrDefault();
                            }
                        }
                        // nếu như nhà thầu đó đang có dược phẩm nhưng ko trùng trong hệ thống sắp lưu
                        else if (d.YeuCauNhapKhoDuocPhamChiTiets.First(e => e.HopDongThauDuocPham != null &&
                                                                            e.HopDongThauDuocPham.HeThongTuPhatSinh == true && e.HopDongThauDuocPham.NhaThauId == model.NhaThauId
                                                                            && e.HopDongThauDuocPham.WillDelete != true).HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.
                            Any(q => q.DuocPhamId != model.DuocPhamBenhVienId && q.WillDelete != true))
                        {
                            var hopDongThauChiTiet = new HopDongThauDuocPhamChiTiet
                            {
                                Id = 0,
                                HopDongThauDuocPhamId = d.YeuCauNhapKhoDuocPhamChiTiets.First(e => s.NhapKhoDuocPhamChiTiets != null && e.Id ==
                                                                                                   s.NhapKhoDuocPhamChiTiets.First(w => w.LoaiNhap == 2 && w.DaCapNhat && w.NhaThauId ==
                                                                                                                                        model.NhaThauId).Id).HopDongThauDuocPhamId,
                                SoLuong = model.SoLuongNhap.GetValueOrDefault(),
                                DuocPhamId = model.DuocPhamBenhVienId.GetValueOrDefault(),
                                Gia = Convert.ToDecimal(model.DonGiaNhap.GetValueOrDefault()),
                                SoLuongDaCap = soLuongDaCapCurrent
                            };
                            result.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.Add(hopDongThauChiTiet);
                        }
                    }
                    // nếu có chỉnh sửa nhà thầu nhưng nhà thầu hiện không tồn tại trong hệ thống
                    else if (s.NhapKhoDuocPhamChiTiets.Where(w => w.LoaiNhap == 2 && w.DaCapNhat != true && w.Id != 0)
                        .Any(e => s.OldNhapKhoDuocPhamChiTiets != null && e.NhaThauId != s.OldNhapKhoDuocPhamChiTiets.First(
                                      q => q.LoaiNhap == 2 && q.Id == model.Id).NhaThauId))
                    {
                        var hopDongThauDuocPhamEntity =
                            new Core.Domain.Entities.HopDongThauDuocPhams.HopDongThauDuocPham
                            {
                                Id = 0,
                                NhaThauId = model.NhaThauId.GetValueOrDefault(),
                                SoHopDong = ResourceHelper.CreateSoHopDong(),
                                SoQuyetDinh = ResourceHelper.CreateSoQuyetDinh(),
                                CongBo = DateTime.Now,
                                NgayKy = DateTime.Now,
                                NgayHieuLuc = DateTime.Now,
                                NgayHetHan = DateTime.Now,
                                LoaiThau = Enums.EnumLoaiThau.ThauRieng,
                                LoaiThuocThau = Enums.EnumLoaiThuocThau.TongHop,
                                NhomThau = "NTHT01",
                                GoiThau = "HT",
                                Nam = DateTime.Now.Year,
                                HeThongTuPhatSinh = true
                            };
                        var soLuongDaCapCurrent = result.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.First(e => s.OldNhapKhoDuocPhamChiTiets != null &&
                            e.DuocPhamId == s.OldNhapKhoDuocPhamChiTiets.First(t => t.Id == model.Id)
                                .DuocPhamBenhVienId).SoLuongDaCap;
                        result.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.First(e => s.OldNhapKhoDuocPhamChiTiets != null &&
                            e.DuocPhamId == s.OldNhapKhoDuocPhamChiTiets.First(t => t.Id == model.Id)
                                .DuocPhamBenhVienId).SoLuong -= model.SoLuongNhap.GetValueOrDefault();
                        if (result.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.First(e => s.OldNhapKhoDuocPhamChiTiets != null &&
                                e.DuocPhamId == s.OldNhapKhoDuocPhamChiTiets.First(t => t.Id == model.Id)
                                    .DuocPhamBenhVienId).SoLuong == 0)
                        {
                            result.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.First(e => s.OldNhapKhoDuocPhamChiTiets != null &&
                                    e.DuocPhamId == s.OldNhapKhoDuocPhamChiTiets.First(t => t.Id == model.Id)
                                        .DuocPhamBenhVienId)
                                .WillDelete = true;
                        }

                        if (result.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.All(e => e.WillDelete))
                        {
                            result.HopDongThauDuocPham.WillDelete = true;
                        }

                        result.HopDongThauDuocPhamId = 0;
                        result.HopDongThauDuocPham = hopDongThauDuocPhamEntity;
                        var hopDongThauChiTiet = new HopDongThauDuocPhamChiTiet
                        {
                            Id = 0,
                            HopDongThauDuocPhamId = hopDongThauDuocPhamEntity.Id,
                            SoLuong = model.SoLuongNhap.GetValueOrDefault(),
                            DuocPhamId = model.DuocPhamBenhVienId.GetValueOrDefault(),
                            Gia = Convert.ToDecimal(model.DonGiaNhap.GetValueOrDefault()),
                            SoLuongDaCap = soLuongDaCapCurrent
                        };
                        result.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.Add(hopDongThauChiTiet);
                    }
                }
            }

            foreach (var model in s.NhapKhoDuocPhamChiTiets.Where(e => e.LoaiNhap == 2 && e.Id == 0))
            {
                var newEntity = new YeuCauNhapKhoDuocPhamChiTiet();
                var yeuCauNhapKhoChiTiet = model.ToEntity(newEntity);

                // thêm yêu cầu nhập kho chi tiết từ ncc mà có nhà thầu tồn tại trong hệ thống
                if (d.YeuCauNhapKhoDuocPhamChiTiets.Any(e => e.HopDongThauDuocPham != null &&
                    e.HopDongThauDuocPham.HeThongTuPhatSinh == true &&
                    e.HopDongThauDuocPham.NhaThauId == model.NhaThauId && e.HopDongThauDuocPham.WillDelete != true))
                {
                    yeuCauNhapKhoChiTiet.HopDongThauDuocPhamId = d.YeuCauNhapKhoDuocPhamChiTiets.First(e => e.HopDongThauDuocPham != null &&
                        e.HopDongThauDuocPham.HeThongTuPhatSinh == true &&
                        e.HopDongThauDuocPham.NhaThauId == model.NhaThauId).HopDongThauDuocPhamId;
                    yeuCauNhapKhoChiTiet.HopDongThauDuocPham = d.YeuCauNhapKhoDuocPhamChiTiets.First(e => e.HopDongThauDuocPham != null &&
                        e.HopDongThauDuocPham.HeThongTuPhatSinh == true &&
                        e.HopDongThauDuocPham.NhaThauId == model.NhaThauId).HopDongThauDuocPham;

                    // nếu như nhà thầu đó đang có dược phẩm trùng trong hệ thống sắp lưu
                    if (d.YeuCauNhapKhoDuocPhamChiTiets.First(e => e.HopDongThauDuocPham != null &&
                       e.HopDongThauDuocPham.HeThongTuPhatSinh == true && e.HopDongThauDuocPham.NhaThauId == model.NhaThauId
                      && e.HopDongThauDuocPham.WillDelete != true).HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.
                      Any(q => q.DuocPhamId == model.DuocPhamBenhVienId && q.WillDelete != true))
                    {
                        if (model.DaCongDon != true)
                        {
                            d.YeuCauNhapKhoDuocPhamChiTiets.First(e => e.HopDongThauDuocPham != null &&
                                                                       e.HopDongThauDuocPham.HeThongTuPhatSinh == true &&
                                                                       e.HopDongThauDuocPham.NhaThauId == model.NhaThauId
                                                                       && e.HopDongThauDuocPham.WillDelete != true)
                                    .HopDongThauDuocPham.HopDongThauDuocPhamChiTiets
                                    .First(q => q.DuocPhamId == model.DuocPhamBenhVienId && q.WillDelete != true).SoLuong +=
                                model.SoLuongNhap.GetValueOrDefault();
                        }
                        //
                    }
                    // nếu như nhà thầu đó đang có dược phẩm nhưng ko trùng trong hệ thống sắp lưu
                    else if (d.YeuCauNhapKhoDuocPhamChiTiets.First(e => e.HopDongThauDuocPham != null &&
                                                                        e.HopDongThauDuocPham.HeThongTuPhatSinh == true && e.HopDongThauDuocPham.NhaThauId == model.NhaThauId
                                                                        && e.HopDongThauDuocPham.WillDelete != true).HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.
                        Any(q => q.DuocPhamId != model.DuocPhamBenhVienId && q.WillDelete != true))
                    {
                        var hopDongThauChiTiet = new HopDongThauDuocPhamChiTiet
                        {
                            Id = 0,
                            HopDongThauDuocPhamId = d.YeuCauNhapKhoDuocPhamChiTiets.First(e => e.HopDongThauDuocPham != null && e.HopDongThauDuocPham.HeThongTuPhatSinh == true &&
                                                                                               e.HopDongThauDuocPham.NhaThauId == model.NhaThauId).HopDongThauDuocPhamId,
                            SoLuong = model.SoLuongNhap.GetValueOrDefault(),
                            DuocPhamId = model.DuocPhamBenhVienId.GetValueOrDefault(),
                            Gia = Convert.ToDecimal(model.DonGiaNhap.GetValueOrDefault()),
                            SoLuongDaCap = 0
                        };

                        yeuCauNhapKhoChiTiet.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.Add(hopDongThauChiTiet);
                    }
                }
                // thêm yêu cầu nhập kho chi tiết từ ncc mà có nhà thầu vẫn chưa tồn tại trong hệ thống hoặc vẫn chưa có nhà cung cấp nào trong hệ thống
                else if (d.YeuCauNhapKhoDuocPhamChiTiets.Any(e => e.HopDongThauDuocPham != null &&
                   e.HopDongThauDuocPham.HeThongTuPhatSinh == true &&
                   e.HopDongThauDuocPham.NhaThauId != model.NhaThauId && e.HopDongThauDuocPham.WillDelete != true) ||
                         !d.YeuCauNhapKhoDuocPhamChiTiets.Where(e => e.HopDongThauDuocPham != null && e.HopDongThauDuocPham.WillDelete != true)
                             .Any(e => e.HopDongThauDuocPham != null && e.HopDongThauDuocPham.HeThongTuPhatSinh == true))
                {
                    var hopDongThauDuocPhamEntity =
                                new Core.Domain.Entities.HopDongThauDuocPhams.HopDongThauDuocPham
                                {
                                    Id = 0,
                                    NhaThauId = model.NhaThauId.GetValueOrDefault(),
                                    SoHopDong = ResourceHelper.CreateSoHopDong(),
                                    SoQuyetDinh = ResourceHelper.CreateSoQuyetDinh(),
                                    CongBo = DateTime.Now,
                                    NgayKy = DateTime.Now,
                                    NgayHieuLuc = DateTime.Now,
                                    NgayHetHan = DateTime.Now,
                                    LoaiThau = Enums.EnumLoaiThau.ThauRieng,
                                    LoaiThuocThau = Enums.EnumLoaiThuocThau.TongHop,
                                    NhomThau = "NTHT01",
                                    GoiThau = "HT",
                                    Nam = DateTime.Now.Year,
                                    HeThongTuPhatSinh = true
                                };
                    yeuCauNhapKhoChiTiet.HopDongThauDuocPham = hopDongThauDuocPhamEntity;
                    var hopDongThauChiTiet = new HopDongThauDuocPhamChiTiet
                    {
                        Id = 0,
                        HopDongThauDuocPhamId = hopDongThauDuocPhamEntity.Id,
                        SoLuong = model.SoLuongNhap.GetValueOrDefault(),
                        DuocPhamId = model.DuocPhamBenhVienId.GetValueOrDefault(),
                        Gia = Convert.ToDecimal(model.DonGiaNhap.GetValueOrDefault()),
                        SoLuongDaCap = 0
                    };
                    yeuCauNhapKhoChiTiet.HopDongThauDuocPham.HopDongThauDuocPhamChiTiets.Add(hopDongThauChiTiet);
                }
                d.YeuCauNhapKhoDuocPhamChiTiets.Add(yeuCauNhapKhoChiTiet);
            }
        }
    }
}
