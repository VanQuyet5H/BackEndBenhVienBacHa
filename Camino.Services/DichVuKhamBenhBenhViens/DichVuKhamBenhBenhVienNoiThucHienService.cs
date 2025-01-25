using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Data;

namespace Camino.Services.DichVuKhamBenhBenhViens
{
    [ScopedDependency(ServiceType = typeof(IDichVuKhamBenhBenhVienNoiThucHienService))]
    public class DichVuKhamBenhBenhVienNoiThucHienService: MasterFileService<DichVuKhamBenhBenhVienNoiThucHien>, IDichVuKhamBenhBenhVienNoiThucHienService
    {
        public DichVuKhamBenhBenhVienNoiThucHienService(IRepository<DichVuKhamBenhBenhVienNoiThucHien> repository) :
            base(repository)
        {

        }
    }
}
