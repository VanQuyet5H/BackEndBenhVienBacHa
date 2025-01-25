using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.KhamBenh
{
    public class KhamBenhTemplateDichVuKhamSucKhoeViewModel
    {
        public long YeuCauKhamBenhId { get; set; }
        public Enums.ChuyenKhoaKhamSucKhoe? ChuyenKhoaKhamSucKhoe { get; set; }
        public string TenChuyenKhoa => ChuyenKhoaKhamSucKhoe.GetDescription();
        public string ThongTinKhamTheoDichVuTemplate { get; set; }
        public dynamic TemplateKhamCacCoQuanObj { get; set; }
        public string ThongTinKhamTheoDichVuData { get; set; }
        public Enums.EnumTrangThaiYeuCauKhamBenh? TrangThai { get; set; }
        public bool IsDaKham => TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham && TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham;
        public bool IsDungChuyenKhoaLogin { get; set; }
        public bool IsDisabled { get; set; }

        // BVHD-3257
        // Khi khám đoàn tất cả:
        // 1. check vào dv nào mới lưu dịch vụ đó.
        // 2. bỏ check là clear luôn data
        // 3. đã hoàn thành thì disabled
        public string ThongTinKhamTheoDichVuDataDefault { get; set; }
        public bool IsDaHoanThanhKham => TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham;
        public bool IsCheckedDichVu { get; set; }
        public bool IsChangeData { get; set; }
    }
}
