using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.FileKetQuaCanLamSangs;
using Camino.Core.Domain.Entities.KetQuaNhomXetNghiems;
using Camino.Data;
using Camino.Services.KetQuaNhomXetNghiem;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Camino.Services.KetQuaNhomXetNghiem
{
    [ScopedDependency(ServiceType = typeof(IKetQuaNhomXetNghiemService))]
    public class KetQuaNhomXetNghiemService : MasterFileService<Camino.Core.Domain.Entities.KetQuaNhomXetNghiems.KetQuaNhomXetNghiem>
            , IKetQuaNhomXetNghiemService
    {
        private readonly IRepository<FileKetQuaCanLamSang> _fileKetQuaCanLamSangRepository;
        public KetQuaNhomXetNghiemService
        (
            IRepository<Camino.Core.Domain.Entities.KetQuaNhomXetNghiems.KetQuaNhomXetNghiem> repository,
            IRepository<FileKetQuaCanLamSang> fileKetQuaCanLamSangRepository
        )
            : base(repository)
        {
            _fileKetQuaCanLamSangRepository = fileKetQuaCanLamSangRepository;
        }

        public void DeleteFileNhomXetNghiems(long yeuCauTiepNhanId, long nhomDichVuBenhVienId)
        {
            var nhomXetNghiems = BaseRepository.TableNoTracking.Where(cc => cc.YeuCauTiepNhanId == yeuCauTiepNhanId && cc.NhomDichVuBenhVienId == nhomDichVuBenhVienId).Include(cc => cc.FileKetQuaCanLamSangs).ToList();
            if (nhomXetNghiems.Any())
            {
                foreach (var nhomXetNghiem in nhomXetNghiems)
                {

                    if (nhomXetNghiem.FileKetQuaCanLamSangs.Any())
                    {
                        var dataFiles = nhomXetNghiem.FileKetQuaCanLamSangs.ToList();
                        foreach (var fileKetQuaCanLamSang in dataFiles)
                        {
                            _fileKetQuaCanLamSangRepository.Delete(fileKetQuaCanLamSang);
                        }
                    }                  
                    BaseRepository.Delete(nhomXetNghiem);
                }
            }

        }
    }
}
