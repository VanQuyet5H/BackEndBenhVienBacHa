using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.NhapKhoDuocPhams
{
    [ScopedDependency(ServiceType = typeof(IYeuCauNhapKhoDuocPhamService))]
    public class YeuCauNhapKhoDuocPhamService : MasterFileService<YeuCauNhapKhoDuocPham>, IYeuCauNhapKhoDuocPhamService
    {
        private readonly IRepository<Kho> _khoRepository;
        private readonly IRepository<Camino.Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienRepository;
        public YeuCauNhapKhoDuocPhamService(IRepository<YeuCauNhapKhoDuocPham> repository
            , IRepository<Kho> khoRepository, IRepository<Camino.Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienRepository) : base(repository)
        {
            _khoRepository = khoRepository;
            _duocPhamBenhVienRepository = duocPhamBenhVienRepository;
        }

        public async Task<long> GetKhoTong1Id()
        {
            return (await _khoRepository.TableNoTracking.FirstOrDefaultAsync(p => p.LoaiKho == Core.Domain.Enums.EnumLoaiKhoDuocPham.KhoTongDuocPhamCap1))?.Id ?? 0;
        }

        public async Task<long?> SuggestNhomDuocPham(long duocPhamId)
        {
            var entity = await _duocPhamBenhVienRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == duocPhamId);
            if (entity != null)
            {
                return entity.DuocPhamBenhVienPhanNhomId;
            }

            return null;
        }

        public async Task UpdateDuocPhamBenhVienPhanNhom(long id, long duocPhamBenhVienPhanNhomId)
        {
            var entity = await _duocPhamBenhVienRepository.Table.FirstOrDefaultAsync(p => p.Id == id);
            if (entity != null)
            {
                entity.DuocPhamBenhVienPhanNhomId = duocPhamBenhVienPhanNhomId;
                await _duocPhamBenhVienRepository.UpdateAsync(entity);
            }
        }
        public async Task<bool> KiemTraNgayHoaDon(DateTime? ngayHoaDon, DateTime? NgayHienTai)
        {
            if (ngayHoaDon != null && NgayHienTai != null)
            {
                if (ngayHoaDon.Value.Date > NgayHienTai.Value.Date)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
