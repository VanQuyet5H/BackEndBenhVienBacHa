using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.HeThong;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.Helpers;
using Camino.Services.PhongBenhVien;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.HoatDongNhanVien
{
    [ScopedDependency(ServiceType = typeof(IHoatDongNhanVienService))]
    public class HoatDongNhanVienService : MasterFileService<Core.Domain.Entities.NhanViens.HoatDongNhanVien>, IHoatDongNhanVienService
    {
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        private readonly IRepository<Camino.Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien> _khoaPhongNhanVienRepository;

        public HoatDongNhanVienService(IUserAgentHelper userAgentHelper, IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository,
            IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository,
            IRepository<Core.Domain.Entities.NhanViens.HoatDongNhanVien> repository,
            IRepository<Camino.Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien> khoaPhongNhanVienRepository
            ) : base(repository)
        {
            _userAgentHelper = userAgentHelper;
            _nhanVienRepository = nhanVienRepository;
            _phongBenhVienRepository = phongBenhVienRepository;
            _khoaPhongNhanVienRepository = khoaPhongNhanVienRepository;
        }

        public Core.Domain.Entities.NhanViens.HoatDongNhanVien GetHoatDongNhanVienByNhanVien(long nhanVienId)
        {
            var hoatDongNhanVien = BaseRepository.TableNoTracking.Where(cc => cc.NhanVienId == nhanVienId).LastOrDefault();
            return hoatDongNhanVien;
        }

        public Core.Domain.Entities.NhanViens.HoatDongNhanVien IsNhanVienHoatDongExists(long currentUserId, long phongKhamId)
        {
            return BaseRepository.TableNoTracking.Where(cc => cc.NhanVienId == currentUserId && cc.PhongBenhVienId == phongKhamId).FirstOrDefault();
        }

        public async Task<Core.Domain.Entities.NhanViens.HoatDongNhanVien> LuuHoatDongNhanVienAsync(long currentUserId, long phongKhamId)
        {
            var nhanVien = _nhanVienRepository.GetById(currentUserId,
                o => o.Include(x => x.HoatDongNhanViens));

            if (nhanVien.HoatDongNhanViens.All(o => o.PhongBenhVienId != phongKhamId))
            {
                nhanVien.HoatDongNhanViens.Add(new Core.Domain.Entities.NhanViens.HoatDongNhanVien
                {
                    NhanVienId = currentUserId,
                    PhongBenhVienId = phongKhamId,
                    ThoiDiemBatDau = DateTime.Now
                });
            }
            nhanVien.LichSuHoatDongNhanViens.Add(new LichSuHoatDongNhanVien()
            {
                NhanVienId = currentUserId,
                PhongBenhVienId = phongKhamId,
                ThoiDiemBatDau = DateTime.Now
            });
            foreach (var nhanVienHoatDongNhanVien in nhanVien.HoatDongNhanViens.Where(o => o.PhongBenhVienId != phongKhamId))
            {
                nhanVienHoatDongNhanVien.WillDelete = true;
            }
            _nhanVienRepository.Update(nhanVien);
            return nhanVien.HoatDongNhanViens.First(o => o.PhongBenhVienId == phongKhamId);
        }

        public async Task<LookupItemVo> GetPhongBenhVienByCurrentUser()
        {
            var lookupItemVo = new LookupItemVo();
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var hoatdongNhanVienPhongKhamLast = BaseRepository.TableNoTracking.Where(cc => cc.NhanVienId == currentUserId).LastOrDefault();
            if (hoatdongNhanVienPhongKhamLast != null)
            {
                lookupItemVo =
                    await _phongBenhVienRepository.TableNoTracking
                        .Where(x => x.Id == hoatdongNhanVienPhongKhamLast.PhongBenhVienId)
                        .Select(item => new LookupItemVo()
                        {
                            DisplayName = item.Ten + " - " + item.Ma,
                            KeyId = item.Id
                        }).FirstOrDefaultAsync();

                return lookupItemVo;
            }
            return lookupItemVo;
        }

        public async Task<LookupItemVo> GetPhongChinhLamViec()
        {
            var lookupItemVo = new LookupItemVo();
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var phongChinhLamViecNhanVienPhongBenhVienId = _khoaPhongNhanVienRepository.TableNoTracking.Where(cc => cc.NhanVienId == currentUserId).Where(xx => xx.LaPhongLamViecChinh == true)
                                                .Select(cc => cc.PhongBenhVienId).FirstOrDefault();
            if (phongChinhLamViecNhanVienPhongBenhVienId.HasValue)
            {
                lookupItemVo =
                    await _phongBenhVienRepository.TableNoTracking
                        .Where(x => x.Id == phongChinhLamViecNhanVienPhongBenhVienId)
                        .Select(item => new LookupItemVo()
                        {
                            DisplayName = item.Ten + " - " + item.Ma,
                            KeyId = item.Id
                        }).FirstOrDefaultAsync();

                return lookupItemVo;
            }
            return lookupItemVo;
        }
    }
}
