using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.Voucher;
using Camino.Core.Domain.Entities.Vouchers;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Voucher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.MappingProfile
{
    public class VoucherMappingProfile : Profile
    {
        public VoucherMappingProfile()
        {
            CreateMap<Core.Domain.Entities.Vouchers.Voucher, VoucherMarketingViewModel>().IgnoreAllNonExisting()
                .ForMember(p => p.GhiChu, o => o.MapFrom(p => p.MoTa));
            CreateMap<VoucherMarketingViewModel, Core.Domain.Entities.Vouchers.Voucher>().IgnoreAllNonExisting()
                //.ForMember(d => d.VoucherChiTietMienGiams, o => o.MapFrom(s => s.lstVoucherChiTietMienGiam));
                .AfterMap((s, d) =>
                {
                    d.ChietKhauTatCaDichVu = s.LoaiDichVuVoucherMarketing == EnumLoaiDichVuVoucherMarketing.TatCaDichVu ? true : false;
                    d.MoTa = s.GhiChu;
                    AddOrUpdate(s, d);
                });

            CreateMap<VoucherChiTietMienGiam, VoucherChiTietMienGiamViewModel>().IgnoreAllNonExisting();
            CreateMap<VoucherChiTietMienGiamViewModel, VoucherChiTietMienGiam>().IgnoreAllNonExisting()
                .AfterMap((s, d) =>
                {
                    d.DichVuKhamBenhBenhVienId = s.LoaiDichVuBenhVien == EnumDichVuTongHop.KhamBenh ? s.DichVuId : (long?)null;
                    d.DichVuKyThuatBenhVienId = s.LoaiDichVuBenhVien == EnumDichVuTongHop.KyThuat ? s.DichVuId : (long?)null;

                    d.NhomGiaDichVuKhamBenhBenhVienId = s.LoaiDichVuBenhVien == EnumDichVuTongHop.KhamBenh ? s.LoaiGiaId : (long?)null;
                    d.NhomGiaDichVuKyThuatBenhVienId = s.LoaiDichVuBenhVien == EnumDichVuTongHop.KyThuat ? s.LoaiGiaId : (long?)null;

                    d.TiLeChietKhau = s.LoaiChietKhau == LoaiChietKhau.ChietKhauTheoTiLe ? s.TiLeChietKhau : (int?)null;
                    d.SoTienChietKhau = s.LoaiChietKhau == LoaiChietKhau.ChietKhauTheoSoTien ? s.SoTienChietKhau : (decimal?)null;

                    d.NhomDichVuKhamBenh = null;
                    d.NhomDichVuBenhVienId = s.NhomDichVuId == 0 || s.NhomDichVuId == null ? null : s.NhomDichVuId;
                });

            CreateMap<VoucherMarketingGridVo, VoucherExportExcel>().IgnoreAllNonExisting();
        }

        private void AddOrUpdate(VoucherMarketingViewModel s, Core.Domain.Entities.Vouchers.Voucher d)
        {
            switch(s.LoaiDichVuVoucherMarketing)
            {
                case EnumLoaiDichVuVoucherMarketing.DichVu:
                    AddOrUpdateDichVuAndNhomDichVu(s, d);
                    break;
                case EnumLoaiDichVuVoucherMarketing.NhomDichVu:
                    AddOrUpdateDichVuAndNhomDichVu(s, d);
                    break;
                case EnumLoaiDichVuVoucherMarketing.TatCaDichVu:
                    AddOrUpdateTatCaDichVu(s, d);
                    break;
            }
        }

        private void AddOrUpdateDichVuAndNhomDichVu(VoucherMarketingViewModel s, Core.Domain.Entities.Vouchers.Voucher d)
        {
            //Xoá DV
            foreach (var chiTietMienGiam in d.VoucherChiTietMienGiams)
            {
                if (chiTietMienGiam.NhomDichVuBenhVienId == null && chiTietMienGiam.NhomDichVuKhamBenh != true  && !s.lstVoucherChiTietMienGiam.Any(c => c.Id == chiTietMienGiam.Id))
                {
                    chiTietMienGiam.WillDelete = true;
                }
            }

            //Xoá nhóm DV
            foreach (var chiTietMienGiam in d.VoucherChiTietMienGiams)
            {
                if (chiTietMienGiam.DichVuKyThuatBenhVienId == null && chiTietMienGiam.DichVuKhamBenhBenhVienId == null && !s.lstVoucherChiTietMienGiamNhomDichVu.Any(c => c.Id == chiTietMienGiam.Id))
                {
                    chiTietMienGiam.WillDelete = true;
                }
            }

            //Thêm & sửa DV
            foreach (var chiTietMienGiamVM in s.lstVoucherChiTietMienGiam)
            {
                if (chiTietMienGiamVM.Id == 0)
                {
                    var newEntity = new VoucherChiTietMienGiam();
                    d.VoucherChiTietMienGiams.Add(chiTietMienGiamVM.ToEntity(newEntity));
                }
                else
                {
                    if (d.VoucherChiTietMienGiams.Any())
                    {
                        var result = d.VoucherChiTietMienGiams.Single(c => c.Id == chiTietMienGiamVM.Id);
                        //chiTietMienGiamVM.VoucherId = d.Id;
                        result = chiTietMienGiamVM.ToEntity(result);
                    }
                }
            }

            //Thêm & sửa nhóm DV
            foreach (var chiTietMienGiamVM in s.lstVoucherChiTietMienGiamNhomDichVu)
            {
                if (chiTietMienGiamVM.Id == 0)
                {
                    var newEntity = new VoucherChiTietMienGiam();
                    d.VoucherChiTietMienGiams.Add(chiTietMienGiamVM.ToEntity(newEntity));
                }
                else
                {
                    if (d.VoucherChiTietMienGiams.Any())
                    {
                        var result = d.VoucherChiTietMienGiams.Single(c => c.Id == chiTietMienGiamVM.Id);
                        chiTietMienGiamVM.VoucherId = d.Id;
                        result = chiTietMienGiamVM.ToEntity(result);
                    }
                }
            }
        }

        private void AddOrUpdateTatCaDichVu(VoucherMarketingViewModel s, Core.Domain.Entities.Vouchers.Voucher d)
        {
            //Xoá
            if (d.VoucherChiTietMienGiams.Any())
            {
                foreach (var chiTietMienGiam in d.VoucherChiTietMienGiams)
                {
                    chiTietMienGiam.WillDelete = true;
                }
            }
        }
    }
}
