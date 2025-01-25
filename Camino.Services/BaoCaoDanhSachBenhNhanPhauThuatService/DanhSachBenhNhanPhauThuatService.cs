using System;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaoDanhSachBenhNhanPhauThuatValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;

namespace Camino.Services.BaoCaoDanhSachBenhNhanPhauThuatService
{
    [ScopedDependency(ServiceType = typeof(IDanhSachBenhNhanPhauThuatService))]
    public class DanhSachBenhNhanPhauThuatService : MasterFileService<Core.Domain.Entities.NgheNghieps.NgheNghiep>, IDanhSachBenhNhanPhauThuatService
    {
        public DanhSachBenhNhanPhauThuatService(IRepository<Core.Domain.Entities.NgheNghieps.NgheNghiep> repository) : base(repository)
        { }

        public GridDataSource GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var fakeList = new BaoCaoDanhSachBenhNhanPhauThuatVo[2];
            fakeList[0] = new BaoCaoDanhSachBenhNhanPhauThuatVo
            {
                Id = 1,
                ThoiGianVaoVien = new DateTime(2021, 1, 17, 9, 8, 10),
                MaTn = "TN0883758",
                HoTenBn = "Nguyễn Thị Nga",
                DiaChi = "Gia Lâm, Long Biên, Hà Nội",
                NgaySinh = new DateTime(1994, 3, 9),
                GioiTinh = Enums.LoaiGioiTinh.GioiTinhNu,
                Khoa = "Khoa Sản",
                ChanDoan = "Chuyển dạ con lần 2 thai 38 tuần 4 ngày/ sẹo mổ đẻ cũ",
                PhuongPhapPhauThuat = "Mổ ngang đoạn dưới tử cung",
                PhuongPhapVoCam = "Tê tủy sống",
                Ptv = "Lưu Quốc Khải",
                GayMe = "Đặng Thanh Long",
                TinhTrangSauPt = "ổn định",
                ThoiGianPhauThuatTu = new DateTime(2021, 1, 17, 15, 8, 10),
                ThoiGianPhauThuatDen = new DateTime(2021, 1, 19, 15, 8, 10),
                CapCuu = "Cấp cứu",
                TaiBien = "ung thư"
            };
            fakeList[1] = new BaoCaoDanhSachBenhNhanPhauThuatVo
            {
                Id = 2,
                ThoiGianVaoVien = new DateTime(2021, 1, 17, 9, 8, 10),
                MaTn = "TN0883758",
                HoTenBn = "Nguyễn Văn Sinh",
                DiaChi = "Gia Lâm, Long Biên, Hà Nội",
                NgaySinh = new DateTime(1994, 3, 9),
                GioiTinh = Enums.LoaiGioiTinh.GioiTinhNam,
                Khoa = "Cấp cứu",
                ChanDoan = "Gãy xương chậu",
                PhuongPhapPhauThuat = "Mổ...",
                PhuongPhapVoCam = "Gây mê",
                Ptv = "Lưu Quốc Khải",
                GayMe = "Đặng Thanh Long",
                TinhTrangSauPt = "ổn định",
                ThoiGianPhauThuatTu = new DateTime(2021, 1, 17, 15, 8, 10),
                ThoiGianPhauThuatDen = new DateTime(2021, 1, 19, 15, 8, 10),
                CapCuu = "Cấp cứu",
                TaiBien = "ung thư"
            };

            return new GridDataSource
            {
                Data = fakeList,
                TotalRowCount = 0
            };
        }

        public GridDataSource GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            return new GridDataSource
            {
                TotalRowCount = 2
            };
        }
    }
}
