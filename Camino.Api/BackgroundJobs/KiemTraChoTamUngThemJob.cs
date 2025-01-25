using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Data;
using Camino.Services.CauHinh;
using Camino.Services.YeuCauTiepNhans;

namespace Camino.Api.BackgroundJobs
{
    [ScopedDependency(ServiceType = typeof(IKiemTraChoTamUngThemJob))]
    public class KiemTraChoTamUngThemJob : IKiemTraChoTamUngThemJob
    {
        private readonly IRepository<YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        private readonly ICauHinhService _cauHinhService;
        private readonly IThuNganNoiTruService _thuNganNoiTruService;

        public KiemTraChoTamUngThemJob(IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepository, ICauHinhService cauHinhService, IThuNganNoiTruService thuNganNoiTruService)
        {
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
            _cauHinhService = cauHinhService;
            _thuNganNoiTruService = thuNganNoiTruService;
        }

        public void Run()
        {
            var yctnCanKiemTras = _yeuCauTiepNhanRepository.TableNoTracking.Where(o =>
                o.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru &&
                o.TrangThaiYeuCauTiepNhan == Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien && o.ChoTamUngThem != true &&
                o.NoiTruBenhAn != null && o.NoiTruBenhAn.DaQuyetToan != true).ToList();
            var cauHinhNoiTru = _cauHinhService.LoadSetting<CauHinhNoiTru>();

            foreach (var yctnCanKiemTra in yctnCanKiemTras)
            {
                //var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetDanhSachChiPhiKhamChuaBenhChuaThu(yctnCanKiemTra.Id).Result.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum();
                var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetSoTienBNConPhaiThanhToan(yctnCanKiemTra.Id).Result;
                var soTienDaTamUng = _thuNganNoiTruService.GetSoTienDaTamUngAsync(yctnCanKiemTra.Id).Result;
                if (soTienDaTamUng <= chiPhiKhamChuaBenh + cauHinhNoiTru.SoTienHanMucPhaiTamUng)
                {
                    var yctn = _yeuCauTiepNhanRepository.GetById(yctnCanKiemTra.Id);
                    yctn.ChoTamUngThem = true;
                    _yeuCauTiepNhanRepository.Update(yctn);
                }
            }
        }
    }
}
