using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.XuatKhoQuaTangs;
using Camino.Data;
using Camino.Services.Helpers;

namespace Camino.Services.XuatKhoQuaTangMarketing
{
    [ScopedDependency(ServiceType = typeof(IXuatKhoQuaTangService))]
    public partial class XuatKhoQuaTangService : MasterFileService<XuatKhoQuaTang>, IXuatKhoQuaTangService
    {
        private readonly IRepository<XuatKhoQuaTangChiTiet> _xuatKhoQuaTangChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        private readonly IRepository<Core.Domain.Entities.BenhNhans.BenhNhan> _benhNhanRepository;

        public XuatKhoQuaTangService(
            IRepository<XuatKhoQuaTang> repository,
            IRepository<XuatKhoQuaTangChiTiet> xuatKhoQuaTangChiTietRepository,
            IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository,
            IRepository<Core.Domain.Entities.BenhNhans.BenhNhan> benhNhanRepository
            ) : base(repository)
        {
            _xuatKhoQuaTangChiTietRepository = xuatKhoQuaTangChiTietRepository;
            _nhanVienRepository = nhanVienRepository;
            _benhNhanRepository = benhNhanRepository;
        }
    }
}
