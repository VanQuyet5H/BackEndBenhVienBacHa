using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaoHoatDongKhoaKhamBenhs;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;

namespace Camino.Services.BaoCaoHoatDongKhoaKhamBenhService
{
    [ScopedDependency(ServiceType = typeof(IBaoCaoHoatDongKhoaKhamBenhService))]
    public class BaoCaoHoatDongKhoaKhamBenhService : MasterFileService<Core.Domain.Entities.NgheNghieps.NgheNghiep>, IBaoCaoHoatDongKhoaKhamBenhService
    {
        public BaoCaoHoatDongKhoaKhamBenhService(IRepository<Core.Domain.Entities.NgheNghieps.NgheNghiep> repository) : base(repository)
        { }

        public GridDataSource GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var fakeList = new BaoCaoHoatDongKhoaKhamBenhVo[3];
            fakeList[0] = new BaoCaoHoatDongKhoaKhamBenhVo
            {
                Id = 1,
                DichVu = "abc",
                Bhyt = 120,
                KskDoan = 4,
                Ksk = 5,
                TreEm = 3,
                SoNguoiBenhVaoVien = 2
            };
            fakeList[1] = new BaoCaoHoatDongKhoaKhamBenhVo
            {
                Id = 2,
                DichVu = "bcd",
                Bhyt = 100,
                VienPhi = 40,
                KskDoan = 3,
                Ksk = 1,
                Goi = 4,
                SoNguoiBenhDieuTriNgoaiTru = 2
            };
            fakeList[2] = new BaoCaoHoatDongKhoaKhamBenhVo
            {
                Id = 3,
                DichVu = "def",
                Bhyt = 50,
                VienPhi = 30,
                KskDoan = 3,
                Ksk = 1,
                Goi = 4,
                SoNgayDieuTriNgoaiTru = 2
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
                TotalRowCount = 3
            };
        }
    }
}
