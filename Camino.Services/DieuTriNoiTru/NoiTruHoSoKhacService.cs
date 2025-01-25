using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.Entities.QuocGias;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.GiayCamKetKyThuatMois;
using Camino.Core.Domain.ValueObject.GiayKhamChuaBenhTheoYcs;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static Camino.Core.Domain.Enums;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore.Internal;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject.GiayPhanUngThuoc;

namespace Camino.Services.DieuTriNoiTru
{
    [ScopedDependency(ServiceType = typeof(INoiTruHoSoKhacService))]
    public class NoiTruHoSoKhacService
        : MasterFileService<NoiTruHoSoKhac>
            , INoiTruHoSoKhacService
    {
        private readonly IRepository<Template> _templateRepository;
        private readonly IRepository<YeuCauTiepNhan> _yctnRepository;
        private readonly IRepository<NoiTruHoSoKhac> _noiTruHoSoKhacRepository;
        private readonly IRepository<Core.Domain.Entities.DanTocs.DanToc> _danTocRepository;
        private readonly IRepository<Core.Domain.Entities.NgheNghieps.NgheNghiep> _ngheNghiepRepository;
        private readonly IRepository<QuocGia> _quocGiaRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<NoiTruPhieuDieuTri> _noiTruPhieuDieuTriRepository;
        private readonly IRepository<Core.Domain.Entities.CauHinhs.CauHinh> _cauHinhRepository;
        private readonly IRepository<Core.Domain.Entities.BenhVien.BenhVien> _benhVienRepository;
        private readonly IRepository<Core.Domain.Entities.QuanHeThanNhans.QuanHeThanNhan> _quanHeThanNhanRepository;
        private readonly IRepository<Core.Domain.Entities.HocViHocHams.HocViHocHam> _hocViHocHamRepository;
        private readonly IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        private readonly IRepository<ICD> _icdRepository;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> _khoaPhongRepository;

        private readonly IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBVRepo;
        private readonly IRepository<Core.Domain.Entities.NhapKhoDuocPhamChiTiets.NhapKhoDuocPhamChiTiet> _nhapkhoDuocPhamChiTietRepo;
        public NoiTruHoSoKhacService
        (
            IRepository<NoiTruHoSoKhac> repository,
            IRepository<Template> templateRepository,
            IRepository<YeuCauTiepNhan> yctnRepository,
            IRepository<NoiTruHoSoKhac> noiTruHoSoKhacRepository,
            IRepository<User> userRepository,
            IRepository<NoiTruPhieuDieuTri> noiTruPhieuDieuTriRepository,
            IRepository<Core.Domain.Entities.CauHinhs.CauHinh> cauHinhRepository,
            IRepository<Core.Domain.Entities.BenhVien.BenhVien> benhVienRepository,
            IRepository<Core.Domain.Entities.QuanHeThanNhans.QuanHeThanNhan> quanHeThanNhanRepository,
            IRepository<Core.Domain.Entities.DanTocs.DanToc> danTocRepository,
            IRepository<Core.Domain.Entities.NgheNghieps.NgheNghiep> ngheNghiepRepository,
            IRepository<QuocGia> quocGiaRepository,
            IRepository<Core.Domain.Entities.HocViHocHams.HocViHocHam> hocViHocHamRepository,
            IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository,
            IRepository<ICD> icdRepository,
            IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository,
            IUserAgentHelper userAgentHelper,
            IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> khoaPhongRepository,
            IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBVRepo,
            IRepository<Core.Domain.Entities.NhapKhoDuocPhamChiTiets.NhapKhoDuocPhamChiTiet> nhapkhoDuocPhamChiTietRepo
        )
            : base(repository)
        {
            _templateRepository = templateRepository;
            _yctnRepository = yctnRepository;
            _noiTruHoSoKhacRepository = noiTruHoSoKhacRepository;
            _userRepository = userRepository;
            _noiTruPhieuDieuTriRepository = noiTruPhieuDieuTriRepository;
            _cauHinhRepository = cauHinhRepository;
            _benhVienRepository = benhVienRepository;
            _quanHeThanNhanRepository = quanHeThanNhanRepository;
            _danTocRepository = danTocRepository;
            _ngheNghiepRepository = ngheNghiepRepository;
            _quocGiaRepository = quocGiaRepository;
            _hocViHocHamRepository = hocViHocHamRepository;
            _nhanVienRepository = nhanVienRepository;
            _icdRepository = icdRepository;
            _phongBenhVienRepository = phongBenhVienRepository;
            _userAgentHelper = userAgentHelper;
            _khoaPhongRepository = khoaPhongRepository;
            _duocPhamBVRepo = duocPhamBVRepo;
            _nhapkhoDuocPhamChiTietRepo = nhapkhoDuocPhamChiTietRepo;
        }

        public async Task<ThongTinHoSoGetInfo> GetNoiTruHoSoKhac(long yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru loaiHoSo)
        {
            var thongTinHoSo = BaseRepository.TableNoTracking
                .Where(q => q.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                            q.LoaiHoSoDieuTriNoiTru == loaiHoSo)
                .Select(q => new ThongTinHoSoGetInfo
                {
                    Id = q.Id,
                    ThongTinHoSo = q.ThongTinHoSo
                });
            return await thongTinHoSo.LastOrDefaultAsync();
        }

        public async Task<List<ThongTinHoSoGetInfo>> GetListNoiTruHoSoKhac(long yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru? loaiHoSo)
        {
            var thongTinHoSo = BaseRepository.TableNoTracking
                .Where(q => q.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                            (loaiHoSo == null || q.LoaiHoSoDieuTriNoiTru == loaiHoSo))
                .Select(q => new ThongTinHoSoGetInfo
                {
                    Id = q.Id,
                    ThongTinHoSo = q.ThongTinHoSo,
                    NgayHoiChan = q.ThoiDiemThucHien,
                    LoaiHoSoDieuTriNoiTruId = (int)q.LoaiHoSoDieuTriNoiTru,
                    LoaiHoSoDieuTriNoiTruTen = q.LoaiHoSoDieuTriNoiTru.GetDescription()
                }).GroupBy(s => s.LoaiHoSoDieuTriNoiTruId)
               .Select(q => new ThongTinHoSoGetInfo
               {
                   Id = q.First().Id,
                   ThongTinHoSo = q.First().ThongTinHoSo,
                   NgayHoiChan = q.First().NgayHoiChan,
                   LoaiHoSoDieuTriNoiTruId = q.First().LoaiHoSoDieuTriNoiTruId,
                   LoaiHoSoDieuTriNoiTruTen = q.First().LoaiHoSoDieuTriNoiTruTen
               });
            return await thongTinHoSo.ToListAsync();
        }
        public async Task<ExistCurrentInfoResultVo> IsThisExistForCuringInfo(long yeuCauTiepNhanId)
        {
            var query = BaseRepository.TableNoTracking
                .Where(q => q.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                            q.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.PhieuTomTatThongTinDieuTri);
            var isExist = await query.AnyAsync();
            long? id = null;

            if (isExist)
            {
                id = query.FirstAsync().Result.Id;
            }

            return new ExistCurrentInfoResultVo
            {
                IsExist = isExist,
                Id = id
            };
        }

        public async Task<string> PhieuInBienBanCamKetPhauThuat(
            PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams)
        {
            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("BienBanCamKetThucHienPhauThuatThuThuat"));
            var result = await _yctnRepository.GetByIdAsync(dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId, s =>
                s.Include(x => x.BenhNhan).ThenInclude(x => x.NgheNghiep)

                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.KhoaPhongNhapVien)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruKhoaPhongDieuTris).ThenInclude(x => x.KhoaPhongChuyenDen)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruEkipDieuTris).ThenInclude(x => x.BacSi).ThenInclude(x => x.User)

                    .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(x => x.GiuongBenh).ThenInclude(x => x.PhongBenhVien)
            );
            var infoBn = await ThongTinBenhNhan(dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId);
            infoBn.Khoa = result.NoiTruBenhAn != null ? result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Any()
                ? result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Last().KhoaPhongChuyenDen.Ten
                : string.Empty : string.Empty;
            var maSoBvCauHinh = await _cauHinhRepository.TableNoTracking.FirstOrDefaultAsync(q => q.Name.Equals("BaoHiemYTe.BenhVienTiepNhan"));
            var maSoBv = maSoBvCauHinh.Value;
            var benhVien = await _benhVienRepository.TableNoTracking.FirstOrDefaultAsync(q => q.Ma.Equals(maSoBv));
            var diaChiBv = benhVien.DiaChi;
            var thongTinHoSo = await _noiTruHoSoKhacRepository.TableNoTracking
                .Where(bv => bv.YeuCauTiepNhanId == dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId &&
                             bv.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.BienBanCamKetPhauThuat)
                .Select(p => p.ThongTinHoSo).FirstOrDefaultAsync();
            BienBanCamKetPhauThuatVo bienBanCamKetThongTinHoSo;
            bienBanCamKetThongTinHoSo = !string.IsNullOrEmpty(thongTinHoSo)
                ? JsonConvert.DeserializeObject<BienBanCamKetPhauThuatVo>(thongTinHoSo)
                : new BienBanCamKetPhauThuatVo();
            var bsGiaiThich = bienBanCamKetThongTinHoSo.BsGiaiThich;
            var chanDoan = bienBanCamKetThongTinHoSo.ChanDoan;
            var phuongPhapPttt = bienBanCamKetThongTinHoSo.PhuongPhapPttt;
            var ngayHoiChan = bienBanCamKetThongTinHoSo.NgayHoiChan != null
                ? bienBanCamKetThongTinHoSo.NgayHoiChan.GetValueOrDefault().ApplyFormatDateTime()
                : string.Empty;
            var ngayHienTai = DateTime.Now.Day;
            var thangHienTai = DateTime.Now.Month;
            var namHienTai = DateTime.Now.Year;
            string thongTinNguoiBenhChong = "";
            string thongTinNguoiBenhQuanHeKhac = "";
            //<td><div class='container'><div class='label'>Sinh năm:</div><div class='value'>{{SinhNamNguoiCanBaoCk}}</div></div></td>
            if (bienBanCamKetThongTinHoSo.ThongTinNguoiBenhs.Any())
            {


                if (bienBanCamKetThongTinHoSo.ThongTinNguoiBenhs.Any(q => q.QuanHe == 2))
                {
                    foreach (var item in bienBanCamKetThongTinHoSo.ThongTinNguoiBenhs.Where(q => q.QuanHe == 2).ToList())
                    {
                        thongTinNguoiBenhChong += !string.IsNullOrEmpty(item.HoTen) ? "<tr><td width='65%'><div class='container'>" +
                                                                                          "<div class='label'>Và người cần báo tin của người bệnh(chồng):</div>" +
                                                                                          "<div class='value'>" + item.HoTen +
                                                                                          "</div>" +
                                                                                          "</div>" +
                                                                                          "</td>"
                            : "<tr><td width='65%'><div class='containerGD'>" +
                                   "<div class='label'>Và người cần báo tin của người bệnh(chồng):</div>" +
                                   "<div class='value breakword' ><div class='content'>" + "&nbsp;" +
                                   "</div>" +
                                   "</div>" +
                                   "</div>" +
                                   "</td>";

                        thongTinNguoiBenhChong += !string.IsNullOrEmpty(item.NamSinh?.ToString()) ? "<td><div class='container'>" +
                                                                                                        "<div class='label'>Sinh năm:</div>" +
                                                                                                        "<div class='value'>" + item.NamSinh +
                                                                                                        "</div>" +
                                                                                                        "</div>" +
                                                                                                        "</td>" +
                                                                                                        "</tr>"
                            : " <td><div class='containerGD'>" +
                                    "<div class='label'>Sinh năm:</div>" +
                                    "<div class='value breakword' ><div class='content'>" + "&nbsp;" +
                                    "</div>" +
                                    "</div>" +
                                    "</div>" +
                                    "</td>" +
                                    "</tr>";

                        thongTinNguoiBenhChong += !string.IsNullOrEmpty(item.DiaChi) ? " <tr><td width='65%'><div class='container'>" +
                                                                                             "<div class='label'>Địa chỉ:</div>" +
                                                                                             "<div class='value'>" + item.DiaChi +
                                                                                             "</div>" +
                                                                                             "</div>" +
                                                                                             "</td>"
                           : " <tr><td width='65%'><div class='containerGD'>" +
                                  "<div class='label'>Địa chỉ:</div>" +
                                  "<div class='value breakword'><div class='content'>" + "&nbsp;" +
                                  "</div>" +
                                  "</div>" +
                                  "</div>" +
                                  "</td>";

                        thongTinNguoiBenhChong += !string.IsNullOrEmpty(item.Cmnd) ? "<td><div class='container'>" +
                                                                                          "<div class='label'>CMND:</div>" +
                                                                                          "<div class='value'>" + item.Cmnd +
                                                                                          "</div>" +
                                                                                          "</div>" +
                                                                                          "</td>" +
                                                                                          "</tr>"
                           : " <td><div class='containerGD'>" +
                                  "<div class='label'>CMND:</div>" +
                                  "<div class='value breakword'>" +
                                  "<div class='content'>" + "&nbsp;" +
                                  "</div>" +
                                  "</div>" +
                                  "</div>" +
                                  "</td>" +
                                  "</tr>";
                    }
                }
                if (bienBanCamKetThongTinHoSo.ThongTinNguoiBenhs.Any(q => q.QuanHe != 2))
                {
                    foreach (var item in bienBanCamKetThongTinHoSo.ThongTinNguoiBenhs.Where(q => q.QuanHe != 2).ToList())
                    {
                        var quanHe =
                            await _quanHeThanNhanRepository.TableNoTracking.FirstOrDefaultAsync(e =>
                                e.Id == item.QuanHe);

                        thongTinNguoiBenhQuanHeKhac += !string.IsNullOrEmpty(quanHe.Ten) && !string.IsNullOrEmpty(item.HoTen) ? " <tr><td width='65%'><div class='container'><div class='label'> người cần báo tin của người bệnh" + "(" + quanHe.Ten + "):" + "</div><div class='value'>" + item.HoTen + "</div></div></td>"
                           : !string.IsNullOrEmpty(quanHe.Ten) && string.IsNullOrEmpty(item.HoTen) ? " <tr><td width='65%'><div class='containerGD'><div class='label'> người cần báo tin của người bệnh" + "(" + quanHe.Ten + "):" + "<div class='value breakword' ><div class='content'>" + "&nbsp;" + "</div></div></div>"
                           : string.IsNullOrEmpty(quanHe.Ten) && string.IsNullOrEmpty(item.HoTen) ? " <tr><td width='65%'><div class='containerGD'><div class='label'> người cần báo tin của người bệnh" + "(" + "......." + "):" + "</div>< div class='value breakword' ><div class='content'>" + "&nbsp;" + "</div></div></div></td>"
                           : " <tr><td width='65%'><div class='container'><div class='label'> người cần báo tin của người bệnh" + "(" + "......." + "):" + "</div>< div class='value breakword' ><div class='content'>" + item.HoTen + "</div></div></div></td>";

                        thongTinNguoiBenhQuanHeKhac += !string.IsNullOrEmpty(item.NamSinh?.ToString()) ? " <td><div class='container'><div class='label'>Sinh năm:</div><div class='value'>" + item.NamSinh + "</div></div></td></tr>"
                           : " <td><div class='containerGD'><div class='label'>Sinh năm:</div><div class='value breakword' ><div class='content'>" + "&nbsp;" + "</div></div></div></td></tr>";

                        thongTinNguoiBenhQuanHeKhac += !string.IsNullOrEmpty(item.DiaChi) ? " <tr><td width='65%'><div class='container'><div class='label'>Địa chỉ:</div><div class='value'>" + item.DiaChi + "</div></div></td>"
                           : " <tr><td width='65%'><div class='containerGD'><div class='label'>Địa chỉ:</div><div class='value breakword' ><div class='content'>" + "&nbsp;" + "</div></div></div></td>";

                        thongTinNguoiBenhQuanHeKhac += !string.IsNullOrEmpty(item.Cmnd) ? " <td><div class='container'><div class='label'>CMND:</div><div class='value'>" + item.Cmnd + "</div></div></td></tr>"
                           : " <td><div class='containerGD'><div class='label'>CMND:</div><div class='value breakword' ><div class='content'>" + "&nbsp;" + "</div></div></div></td></tr>";
                    }
                }
            }
            else
            {
                thongTinNguoiBenhChong += "<tr>" +
                    "<td width='65%'>" +
                        "<div class='containerGD'>" +
                            "<div class='label'>Và người cần báo tin của người bệnh(chồng):" +
                            "</div>" +
                            "<div class='value breakword' >" +
                            "<div class='content'>" + "&nbsp;" +
                            "</div>" +
                            "</div>" +
                        "</div>" +
                    "</td>";

                thongTinNguoiBenhChong += "<td>" +
                    "<div class='containerGD'>" +
                    "<div class='label'>Sinh năm:</div>" +
                    "<div class='value breakword' >" +
                    "<div class='content'>" + "&nbsp;" + "</div>" +
                    "</div>" +
                    "</div>" +
                    "</td>" +
                    "</tr>";

                thongTinNguoiBenhChong += "<tr><td width='65%'><div class='containerGD'><div class='label'>Địa chỉ:</div><div class='value breakword' ><div class='content'>" + "&nbsp;" + "</div></div></div></td>";

                thongTinNguoiBenhChong += "<td><div class='containerGD'><div class='label'>CMND:</div><div class='value breakword' ><div class='content'>" + "&nbsp;" + "</div></div></div></td></tr>";


                thongTinNguoiBenhQuanHeKhac += "<tr><td width='65%'><div class='container'><div class='label'> người cần báo tin của người bệnh" + "(" + "......." + "):" + "</div> <div class='value breakword'><div class='content'>" + "&nbsp;" + "</div></div></div></td>";

                thongTinNguoiBenhQuanHeKhac += "<td><div class='containerGD'><div class='label'>Sinh năm:</div> <div class='value breakword'> <div class='content'>" + "&nbsp;" + "</div></div></div></td></tr>";

                thongTinNguoiBenhQuanHeKhac += "<tr><td width='65%'><div class='containerGD'> <div class='label'>Địa chỉ:</div><div class='value breakword' ><div class='content'>" + "&nbsp;" + "</div></div></div></td>";

                thongTinNguoiBenhQuanHeKhac += "<td><div class='containerGD'><div class='label'>CMND:</div><div class='value breakword' ><div class='content'>" + "&nbsp;" + "</div></div></div></td></tr>";
            }

            string bSThucHien = string.Empty;
            if (bienBanCamKetThongTinHoSo.BacSiThucHien != null)
            {
                bSThucHien = _userRepository.TableNoTracking.Where(s => s.Id == bienBanCamKetThongTinHoSo.BacSiThucHien).Select(y => y.HoTen).FirstOrDefault();
            }
            var namSinh = DateHelper.DOBFormat(result.NgaySinh, result.ThangSinh, result.NamSinh);

            var data = new DataPhieuInCamKetPhauThuatVo();

            data.NguoiBenh = !string.IsNullOrEmpty(infoBn.HoTenNgBenh) ? "<div class='container'><div class='label'>Người bệnh:</div><div class='value'><b>" + "&nbsp;" + infoBn.HoTenNgBenh + "</b></div></div>"
                : "<div class='containerGD'><div class='label'>Người bệnh:</div><div class='value breakword' ><div class='content'>" + "&nbsp;" + "</div></div></div>";

            data.SinhNamText = !string.IsNullOrEmpty(namSinh) ? "<div class='container'><div class='label'>Sinh năm:</div><div class='value'>" + "&nbsp;<b>" + namSinh + "</b></div></div>"
                : "<div class='containerGD'><div class='label'>Sinh năm:</div><div class='value breakword' ><div class='content'>" + "&nbsp;" + "</div></div></div>";

            data.CMNDCCCDHC = !string.IsNullOrEmpty(infoBn.Cmnd) ? "<div class='container'><div class='label'>CMND/CCCD/HC:</div><div class='value'>" + "&nbsp;" + infoBn.Cmnd + "</div></div>"
                : "<div class='containerGD'><div class='label'>CMND/CCCD/HC:</div><div class='value breakword' ><div class='content'>" + "&nbsp;" + "</div></div></div>";

            data.MaSoBenhVien = !string.IsNullOrEmpty(maSoBv) ? "<div class='container'><div class='label'>Mã số bệnh viện:</div><div class='value'>" + "&nbsp;" + maSoBv + "</div></div>"
                : "<div class='containerGD'><div class='label'>Mã số bệnh viện:</div><<div class='value breakword' ><div class='content'>" + "&nbsp;" + "</div></div></div>";

            data.DiaChiBenhVien = !string.IsNullOrEmpty(diaChiBv) ? "<div class='container'><div class='label'>Địa chỉ:</div><div class='value'>" + "&nbsp;" + diaChiBv + "</div></div>"
                : "<div class='containerGD'><div class='label'>Địa chỉ:</div><div class='value breakword' ><div class='content'>" + "&nbsp;" + "</div></div></div>";


            data.ChungToiDaDuocBacSi = !string.IsNullOrEmpty(bsGiaiThich) ? "Chúng tôi đã được bác sĩ:&nbsp;" + bsGiaiThich : "Chúng tôi đã được bác sĩ:&nbsp; ....................................................";

            data.ChuanDoanBenh = !string.IsNullOrEmpty(chanDoan) ? "1. Chuẩn đoán bệnh:&nbsp;" + chanDoan : "1. Chuẩn đoán bệnh:&nbsp;...........................................................";

            data.PhuongPhapPhauThuatThuThuat = !string.IsNullOrEmpty(phuongPhapPttt) ? "phương pháp phẫu thuật/ thủ thuật:&nbsp;" + phuongPhapPttt : "phương pháp phẫu thuật/ thủ thuật:&nbsp;.............................................................";

            data.CanCuVaoBienBanHoiChuanNgay = !string.IsNullOrEmpty(ngayHoiChan) ? "Căn cứ vào Biên bản Hội chẩn ngày " + ngayHoiChan : "Căn cứ vào Biên bản Hội chẩn ngày.....................................";



            data.NgayHienTai = ngayHienTai > 9 ? ngayHienTai + " " : "0" + ngayHienTai;
            data.ThangHienTai = thangHienTai > 9 ? thangHienTai + " " : "0" + thangHienTai;
            data.NamHienTai = namHienTai + "";
            data.thongTinNguoiBenhChong = thongTinNguoiBenhChong;
            data.thongTinNguoiBenhQuanHeKhac = thongTinNguoiBenhQuanHeKhac;
            data.BSThucHien = bSThucHien;
            if (!string.IsNullOrEmpty(infoBn.Khoa))
            {
                data.Khoa = "<b>Khoa</b>" + infoBn.Khoa.Replace("Khoa", "");
            }

            //data.HoTenBenhNhan = result.HoTen;

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }

        public async Task<string> PhieuInBienBanHoiChanPhauThuat(BangTheoDoiHoiTinhHttpParamsRequest phieuInBienBanHoiChanParams)
        {
            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("BienBanHoiChanPhauThuat"));
            var result = await _yctnRepository.GetByIdAsync(phieuInBienBanHoiChanParams.YeuCauTiepNhanId, s =>
                s.Include(x => x.BenhNhan).ThenInclude(x => x.NgheNghiep)

                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.KhoaPhongNhapVien)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruKhoaPhongDieuTris).ThenInclude(x => x.KhoaPhongChuyenDen)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruEkipDieuTris).ThenInclude(x => x.BacSi).ThenInclude(x => x.User)

                    .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(x => x.GiuongBenh).ThenInclude(x => x.PhongBenhVien)
            );
            var infoBn = await ThongTinBenhNhan(phieuInBienBanHoiChanParams.YeuCauTiepNhanId);
            infoBn.Khoa = result.NoiTruBenhAn != null ? result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Any()
                ? result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Last().KhoaPhongChuyenDen.Ten
                : string.Empty : string.Empty;
            infoBn.Giuong = result.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan)
                ? result.YeuCauDichVuGiuongBenhViens.OrderBy(x => x.Id).LastOrDefault(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan)?.GiuongBenh.Ten
                : string.Empty;
            infoBn.Buong = result.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan)
                ? result.YeuCauDichVuGiuongBenhViens.Last(p => p.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).GiuongBenh.PhongBenhVien.Ten
                : string.Empty;
            BienBanHoiChanVo bienBanhoiChanVo;
            var thongTinHoSo = await _noiTruHoSoKhacRepository.TableNoTracking
                .Where(bv => bv.YeuCauTiepNhanId == phieuInBienBanHoiChanParams.YeuCauTiepNhanId &&
                             bv.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.BienBanHoiChanPhauThuat && bv.Id == phieuInBienBanHoiChanParams.IdNoiTruHoSo)
                .Select(p => p.ThongTinHoSo).FirstOrDefaultAsync();
            bienBanhoiChanVo = !string.IsNullOrEmpty(thongTinHoSo)
                ? JsonConvert.DeserializeObject<BienBanHoiChanVo>(thongTinHoSo)
                : new BienBanHoiChanVo();
            var khoa = infoBn.Khoa;
            var giuong = infoBn.Giuong;
            var buong = infoBn.Buong;
            var thanhVienThamGia = GetThanhVienThamGias(bienBanhoiChanVo.ThanhVienThamGias);

            var phauThuatVien = GetThanhVienThamGia(bienBanhoiChanVo.NhomPhauThuat);
            var gayMeVien = GetThanhVienThamGia(bienBanhoiChanVo.NhomGayMe);
            var tuNgayDieuTri = result.NoiTruBenhAn?.ThoiDiemNhapVien;
            var denNgayDieuTri = result.NoiTruBenhAn?.ThoiDiemRaVien;
            DateTime thoiGianHoiChanUTC = DateTime.Now;
            DateTime.TryParseExact(bienBanhoiChanVo.ThoiGianHoiChanUTC, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out thoiGianHoiChanUTC);
            DateTime ngayDocKQ = DateTime.Now;
            var data = new DataInBienBanHoiChanVo
            {
                Tuoi = DateTime.Now.Year - infoBn.NamSinh,
                HoTen = infoBn.HoTenNgBenh,
                GioiTinh = infoBn.GTNgBenh,
                Khoa = khoa,
                So = infoBn.MaSoTiepNhan,
                SoGiuong = giuong,
                SoBuong = buong,
                ChanDoan = bienBanhoiChanVo.ChanDoan,
                HoiChanMoTheoGio = thoiGianHoiChanUTC.Hour,
                HoiChanMoTheoPhut = thoiGianHoiChanUTC.Minute,
                HoiChanMoTheoNgay = thoiGianHoiChanUTC.Day,
                HoiChanMoTheoThang = thoiGianHoiChanUTC.Month,
                HoiChanMoTheoNam = thoiGianHoiChanUTC.Year,
                ThanhPhanThamGia = thanhVienThamGia,
                DienBien = bienBanhoiChanVo.Summary,
                Creatinin = bienBanhoiChanVo.Creatinin,
                DongMau = bienBanhoiChanVo.DongMau,
                NhomMau = bienBanhoiChanVo.NhomMau,
                Ure = bienBanhoiChanVo.Ure,
                Got = bienBanhoiChanVo.Got,
                Gpt = bienBanhoiChanVo.Gpt,
                DienTim = bienBanhoiChanVo.DienTim,
                XetNghiemKhac = bienBanhoiChanVo.XnKhac,
                NhomPhauThuat = phauThuatVien,
                NhomGayMe = gayMeVien,
                PhuongPhapGayMeTe = bienBanhoiChanVo.PhuongPhapGayMe,
                NgayGioPhauThuat = bienBanhoiChanVo.ThoiGianPhauThuat != null ? bienBanhoiChanVo.ThoiGianPhauThuat.GetValueOrDefault().ApplyFormatDateTime() : string.Empty,
                DuKienKhac = bienBanhoiChanVo.DuKienKhac,
                BsDieuTri = bienBanhoiChanVo.BacSiDieuTri,
                BsGayMe = bienBanhoiChanVo.BacSiGayMe,
                BsTruongKhoa = bienBanhoiChanVo.BacSiTruongKhoa,
                NguoiDuyetMo = bienBanhoiChanVo.BacSiDuyetMo,
                KetLuan = bienBanhoiChanVo.KetLuan,
                DuKienPhauThuat = bienBanhoiChanVo.DuKienPhauThuat,
                KetLuanTheoNgay = bienBanhoiChanVo.NgayThucHien?.Day,
                KetLuanTheoThang = bienBanhoiChanVo.NgayThucHien?.Month,
                KetLuanTheoNam = bienBanhoiChanVo.NgayThucHien?.Year,
                TuNgayDTri = tuNgayDieuTri?.Day,
                TuThangDTri = tuNgayDieuTri?.Month,
                TuNamDTri = tuNgayDieuTri?.Year,
                DenNgayDTri = denNgayDieuTri?.Day,
                DenThangDTri = denNgayDieuTri?.Month,
                DenNamDTri = denNgayDieuTri?.Year
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }
        private string GetThanhVienThamGias(List<ThanhVienThamGia> thanhVienThamGias)
        {
            var res = string.Empty;
            if (thanhVienThamGias != null)
            {

                int i = 1;
                foreach (var thanhVienThamGia in thanhVienThamGias)
                {
                    var nhanViens = _nhanVienRepository.TableNoTracking
                    .Include(v => v.HocHamHocVi).
                    Include(v => v.VanBangChuyenMon)
                    .Include(v => v.User).Where(d => d.Id == thanhVienThamGia.KeyId)
       ;

                    var lst = new List<LookupItemVo>();
                    if (nhanViens.Any())
                    {
                        lst = nhanViens.Select(d => new LookupItemVo()
                        {
                            KeyId = d.Id,
                            DisplayName = DisplayName(d)
                        }).ToList();
                    }
                    res += lst.Select(d => d.DisplayName).FirstOrDefault();
                    if (i < thanhVienThamGias.Count())
                    {
                        res += ", ";
                    }
                    i++;
                }
                return res;
            }

            return string.Empty;
        }
        private string GetThanhVienThamGia(List<string> thanhVienThamGias)
        {
            var res = string.Empty;
            if (thanhVienThamGias != null)
            {
                var thanhVienLast = thanhVienThamGias.LastOrDefault();
                foreach (var thanhVienThamGia in thanhVienThamGias)
                {
                    if (thanhVienThamGia == thanhVienLast)
                    {
                        res += thanhVienThamGia;
                    }
                    else
                    {
                        res += thanhVienThamGia;
                        res += ", ";
                    }
                }
                return res;
            }

            return string.Empty;
        }
        public async Task<List<ThongTinHoSoGetInfo>> GetListNoiTruHoSoKhacBangTheoDoi(long yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru? loaiHoSo)
        {
            var thongTinHoSo = BaseRepository.TableNoTracking
                .Where(q => q.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                            (loaiHoSo == null || q.LoaiHoSoDieuTriNoiTru == loaiHoSo))
                .Select(q => new ThongTinHoSoGetInfo
                {
                    Id = q.Id,
                    ThongTinHoSo = q.ThongTinHoSo,
                    NgayHoiChan = q.ThoiDiemThucHien,
                    LoaiHoSoDieuTriNoiTruId = (int)q.LoaiHoSoDieuTriNoiTru,
                    LoaiHoSoDieuTriNoiTruTen = q.LoaiHoSoDieuTriNoiTru.GetDescription()
                });
            return await thongTinHoSo.ToListAsync();
        }
        public async Task<string> PhieuInBangTheoDoiHoiTinh(
            BangTheoDoiHoiTinhHttpParamsRequest bangTheoDoiHoiTinhHttpParams)
        {
            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("BangTheoDoiHoiTinh"));
            var infoBn = await ThongTinBenhNhan(bangTheoDoiHoiTinhHttpParams.YeuCauTiepNhanId);

            BangTheoDoiHoiTinhVo bangTheoDoiHoiTinhVo;
            var thongTinHoSo = await _noiTruHoSoKhacRepository.TableNoTracking
                .Where(bv => bv.YeuCauTiepNhanId == bangTheoDoiHoiTinhHttpParams.YeuCauTiepNhanId &&
                             bv.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.BangTheoDoiHoiTinh && bv.Id == bangTheoDoiHoiTinhHttpParams.IdNoiTruHoSo)
                .Select(p => p.ThongTinHoSo).FirstOrDefaultAsync();
            bangTheoDoiHoiTinhVo = !string.IsNullOrEmpty(thongTinHoSo)
                ? JsonConvert.DeserializeObject<BangTheoDoiHoiTinhVo>(thongTinHoSo)
                : new BangTheoDoiHoiTinhVo();
            var data = new DataInBangTheoDoiHoiTinhVo
            {
                Tuoi = DateTime.Now.Year - infoBn.NamSinh,
                HoTen = infoBn.HoTenNgBenh,
                GioiTinh = infoBn.GTNgBenh,
                CachMo = bangTheoDoiHoiTinhVo.CachMo,
                PTVien = bangTheoDoiHoiTinhVo.PhauThuatVien,
                CachVoCam = bangTheoDoiHoiTinhVo.CachVoCam,
                PhongMo = bangTheoDoiHoiTinhVo.PhongMo,
                BSGMHS = bangTheoDoiHoiTinhVo.BsGmhs,
                GioNhan = bangTheoDoiHoiTinhVo.GioNhan != null ? bangTheoDoiHoiTinhVo.GioNhan.GetValueOrDefault().ApplyFormatDateTime() : string.Empty,
                GioChuyen = bangTheoDoiHoiTinhVo.GioChuyen != null ? bangTheoDoiHoiTinhVo.GioChuyen.GetValueOrDefault().ApplyFormatDateTime() : string.Empty,
                DieuDuongNhan = bangTheoDoiHoiTinhVo.DdNhan,
                NamH = bangTheoDoiHoiTinhVo.NgayThucHien?.Year,
                ThangH = bangTheoDoiHoiTinhVo.NgayThucHien?.Month,
                NgayH = bangTheoDoiHoiTinhVo.NgayThucHien?.Day,
                PhongHTSo = bangTheoDoiHoiTinhVo.PhongHoiTinhSo,
                TheoLenhYBS = bangTheoDoiHoiTinhVo.YlenhBs,
                VePhong = bangTheoDoiHoiTinhVo.VePhong
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }

        public async Task<string> PhieuInPhieuKhamGayMeTruocMo(
            PhieuDieuTriVaServicesHttpParams phieuKhamGayMeTruocMoHttpParams)
        {
            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuKhamGayMeTruocMo"));
            var infoBn = await ThongTinBenhNhan(phieuKhamGayMeTruocMoHttpParams.YeuCauTiepNhanId);

            PhieuKhamGayTeTruocMoVo phieuKhamGayTeTruocMoVo;
            var thongTinHoSo = await _noiTruHoSoKhacRepository.TableNoTracking
                .Where(bv => bv.YeuCauTiepNhanId == phieuKhamGayMeTruocMoHttpParams.YeuCauTiepNhanId &&
                             bv.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.PhieuKhamGayMeTruocMo)
                .Select(p => p.ThongTinHoSo).FirstOrDefaultAsync();
            phieuKhamGayTeTruocMoVo = !string.IsNullOrEmpty(thongTinHoSo)
                ? JsonConvert.DeserializeObject<PhieuKhamGayTeTruocMoVo>(thongTinHoSo)
                : new PhieuKhamGayTeTruocMoVo();
            string tmp = "\n";
            string replace = "<br>";
            string chanDoan = "";
            if (phieuKhamGayTeTruocMoVo.ChanDoan != null)
            {
                chanDoan = phieuKhamGayTeTruocMoVo.ChanDoan.Replace(tmp, replace);
            }
            string huongDieuTri = "";
            if (phieuKhamGayTeTruocMoVo.HuongDieuTri != null)
            {
                huongDieuTri = phieuKhamGayTeTruocMoVo.HuongDieuTri.Replace(tmp, replace);
            }
            string tienSuNoiKhoa = "";
            if (phieuKhamGayTeTruocMoVo.TienSuNoiKhoa != null)
            {
                tienSuNoiKhoa = phieuKhamGayTeTruocMoVo.TienSuNoiKhoa.Replace(tmp, replace);
            }
            string tienSuNgoaiKhoa = "";
            if (phieuKhamGayTeTruocMoVo.TienSuNgoaiKhoa != null)
            {
                tienSuNgoaiKhoa = phieuKhamGayTeTruocMoVo.TienSuNgoaiKhoa.Replace(tmp, replace);
            }
            string tienSuGayMe = "";
            if (phieuKhamGayTeTruocMoVo.TienSuGayMe != null)
            {
                tienSuGayMe = phieuKhamGayTeTruocMoVo.TienSuGayMe.Replace(tmp, replace);
            }
            string diUng = "";
            if (phieuKhamGayTeTruocMoVo.DiUng != null)
            {
                diUng = phieuKhamGayTeTruocMoVo.DiUng.Replace(tmp, replace);
            }
            string benhLayNhiem = "";
            if (phieuKhamGayTeTruocMoVo.BenhLayNhiem != null)
            {
                benhLayNhiem = phieuKhamGayTeTruocMoVo.BenhLayNhiem.Replace(tmp, replace);
            }
            string thuocDangDieuTri = "";
            if (phieuKhamGayTeTruocMoVo.ThuocDangDieuTri != null)
            {
                thuocDangDieuTri = phieuKhamGayTeTruocMoVo.ThuocDangDieuTri.Replace(tmp, replace);
            }
            string khamLamSang = "";
            if (phieuKhamGayTeTruocMoVo.KhamLamSang != null)
            {
                khamLamSang = phieuKhamGayTeTruocMoVo.KhamLamSang.Replace(tmp, replace);
            }
            string tuanHoan = "";
            if (phieuKhamGayTeTruocMoVo.TuanHoan != null)
            {
                tuanHoan = phieuKhamGayTeTruocMoVo.TuanHoan.Replace(tmp, replace);
            }
            string m = "";
            if (phieuKhamGayTeTruocMoVo.M != null)
            {
                m = phieuKhamGayTeTruocMoVo.M.Replace(tmp, replace);
            }
            string ha = "";
            if (phieuKhamGayTeTruocMoVo.Ha != null)
            {
                ha = phieuKhamGayTeTruocMoVo.Ha.Replace(tmp, replace);
            }
            string hoHap = "";
            if (phieuKhamGayTeTruocMoVo.HoHap != null)
            {
                hoHap = phieuKhamGayTeTruocMoVo.HoHap.Replace(tmp, replace);
            }
            string thanKinh = "";
            if (phieuKhamGayTeTruocMoVo.ThanKinh != null)
            {
                thanKinh = phieuKhamGayTeTruocMoVo.ThanKinh.Replace(tmp, replace);
            }
            string cotSong = "";
            if (phieuKhamGayTeTruocMoVo.CotSong != null)
            {
                cotSong = phieuKhamGayTeTruocMoVo.CotSong.Replace(tmp, replace);
            }
            string xnBatThuong = "";
            if (phieuKhamGayTeTruocMoVo.XnBatThuong != null)
            {
                xnBatThuong = phieuKhamGayTeTruocMoVo.XnBatThuong.Replace(tmp, replace);
            }
            string ycBoSung = "";
            if (phieuKhamGayTeTruocMoVo.YcBoSung != null)
            {
                ycBoSung = phieuKhamGayTeTruocMoVo.YcBoSung.Replace(tmp, replace);
            }
            string dkCachThuocVc = "";
            if (phieuKhamGayTeTruocMoVo.DkCachThuocVc != null)
            {
                dkCachThuocVc = phieuKhamGayTeTruocMoVo.DkCachThuocVc.Replace(tmp, replace);
            }
            string dkGiamDauSauMo = "";
            if (phieuKhamGayTeTruocMoVo.DkGiamDauSauMo != null)
            {
                dkGiamDauSauMo = phieuKhamGayTeTruocMoVo.DkGiamDauSauMo.Replace(tmp, replace);
            }
            string ylenhTruocMo = "";
            if (phieuKhamGayTeTruocMoVo.YlenhTruocMo != null)
            {
                ylenhTruocMo = phieuKhamGayTeTruocMoVo.YlenhTruocMo.Replace(tmp, replace);
            }
            string thuocTienMe = "";
            if (phieuKhamGayTeTruocMoVo.ThuocTienMe != null)
            {
                thuocTienMe = phieuKhamGayTeTruocMoVo.ThuocTienMe.Replace(tmp, replace);
            }
            var data = new DataInPhieuKhamGayMeTruocMoVo
            {
                Tuoi = DateTime.Now.Year - infoBn.NamSinh,
                HoTen = infoBn.HoTenNgBenh,
                GioiTinh = infoBn.GTNgBenh,
                NhomMau = infoBn.NhomMau,
                Nang = phieuKhamGayTeTruocMoVo.Nang,
                Asa = phieuKhamGayTeTruocMoVo.Asa,
                KcCamGiap = phieuKhamGayTeTruocMoVo.KcCamGiap,
                Cao = phieuKhamGayTeTruocMoVo.Cao,
                Mallampati = phieuKhamGayTeTruocMoVo.Mallampati,
                HaMieng = phieuKhamGayTeTruocMoVo.HaMieng,
                BuaAnCuoi = phieuKhamGayTeTruocMoVo.GioMoTruocBuaAn,
                ChuanDoan = chanDoan,
                HuongDieuTri = huongDieuTri,
                TienSuNgoaiKhoa = tienSuNgoaiKhoa,
                TienSuNoiKhoa = tienSuNoiKhoa,
                TienSuGayMeGayTe = tienSuGayMe,
                DiUng = diUng,
                BenhLayNhiem = benhLayNhiem,
                ThuocDangDieuTri = thuocDangDieuTri,
                KhamLamSang = khamLamSang,
                TuanHoan = tuanHoan,
                M = m,
                Ha = ha,
                HoHap = hoHap,
                ThanKinh = thanKinh,
                CotSong = cotSong,
                XetNghiemBatThuong = xnBatThuong,
                YeuCauBoxung = ycBoSung,
                DuKienVathuocVoCam = dkCachThuocVc,
                DuKienGiamDauSauMo = dkGiamDauSauMo,
                YlenhTruocMo = ylenhTruocMo,
                VaThuocTienMe = thuocTienMe,
                DdThucHien = phieuKhamGayTeTruocMoVo.DdThucHien,
                NgayKham = phieuKhamGayTeTruocMoVo.NgayKham != null ? phieuKhamGayTeTruocMoVo.NgayKham.GetValueOrDefault().ApplyFormatDate() : string.Empty,
                NgayThamLaiTruocMo = phieuKhamGayTeTruocMoVo.NgayKham != null ? phieuKhamGayTeTruocMoVo.NgayKham.GetValueOrDefault().ApplyFormatDate() : string.Empty,
                HoTenChuKyBSGMHS = phieuKhamGayTeTruocMoVo.BsGmHs,
                CapCuu = phieuKhamGayTeTruocMoVo.CapCuu == true ? @"<style>#cap-cuu:after {        content: '\2714';    }</style>" : string.Empty,
                DaDayDay = phieuKhamGayTeTruocMoVo.DaDayDay == true ? @"<style>#da-day-day:after {        content: '\2714';    }</style>" : string.Empty,
                ThuocLa = phieuKhamGayTeTruocMoVo.ThuocLa == true ? @"<style>#thuoc-la:after {        content: '\2714';    }</style>" : string.Empty,
                Ruou = phieuKhamGayTeTruocMoVo.Ruou == true ? @"<style>#ruou:after {        content: '\2714';    }</style>" : string.Empty,
                MaTuy = phieuKhamGayTeTruocMoVo.MaTuy == true ? @"<style>#ma-tuy:after {        content: '\2714';    }</style>" : string.Empty
            };

            if (phieuKhamGayTeTruocMoVo.RangGia == 0)
            {
                data.KhongRangGia = @"<style>#not-rang-gia:after {        content: '\2714';    }</style>";
            }
            else if (phieuKhamGayTeTruocMoVo.RangGia == 1)
            {
                data.RangGiaThaoDuoc = @"<style>#thao-duoc:after {        content: '\2714';    }</style>";
            }
            else if (phieuKhamGayTeTruocMoVo.RangGia == 2)
            {
                data.RangGiaCoDinh = @"<style>#co-dinh:after {        content: '\2714';    }</style>";
            }

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }

        public async Task<string> PhieuInGiayKhamChuaBenhTheoYc(
            PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams)
        {
            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuKhamChuaBenhTheoYeuCauCuaBenhAnMo"));
            var infoBn = await ThongTinBenhNhan(dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId);
            var result = await _yctnRepository.GetByIdAsync(dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId, s =>
                s.Include(x => x.BenhNhan).ThenInclude(x => x.NgheNghiep)

                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.KhoaPhongNhapVien)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruKhoaPhongDieuTris).ThenInclude(x => x.KhoaPhongChuyenDen)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruEkipDieuTris).ThenInclude(x => x.BacSi).ThenInclude(x => x.User)

                    .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(x => x.GiuongBenh).ThenInclude(x => x.PhongBenhVien)
            );
            GiayKhamChuaBenhTheoYcVo giayKhamChuaBenhTheoYc;
            var thongTinHoSo = await _noiTruHoSoKhacRepository.TableNoTracking
                .Where(bv => bv.YeuCauTiepNhanId == dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId &&
                             bv.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayKhamChuaBenhTheoYc)
                .Select(p => p.ThongTinHoSo).FirstOrDefaultAsync();
            giayKhamChuaBenhTheoYc = !string.IsNullOrEmpty(thongTinHoSo)
                ? JsonConvert.DeserializeObject<GiayKhamChuaBenhTheoYcVo>(thongTinHoSo)
                : new GiayKhamChuaBenhTheoYcVo();
            infoBn.Khoa = result.NoiTruBenhAn != null ? result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Any()
                ? result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Last().KhoaPhongChuyenDen.Ten
                : string.Empty : string.Empty;
            var khoa = infoBn.Khoa;
            var nguoiNhan = giayKhamChuaBenhTheoYc.NguoiNhan;
            var hoTenNguoiThan = giayKhamChuaBenhTheoYc.TaoLaAi == 1 ? giayKhamChuaBenhTheoYc.HoTenNguoiThan : string.Empty;
            var diaChinguoiThan = giayKhamChuaBenhTheoYc.TaoLaAi == 1 ? giayKhamChuaBenhTheoYc.DiaChiNguoiThan : string.Empty;
            var namSinh = giayKhamChuaBenhTheoYc.TaoLaAi == 1 ? giayKhamChuaBenhTheoYc.NamSinhNguoiThan : null;
            var gioiTinh = giayKhamChuaBenhTheoYc.TaoLaAi == 1 ? giayKhamChuaBenhTheoYc.GioiTinhNguoiThan != null ? giayKhamChuaBenhTheoYc.GioiTinhNguoiThan == 1 ? "Nam" : "Nữ" : string.Empty : string.Empty;
            var cmnd = giayKhamChuaBenhTheoYc.TaoLaAi == 1 ? giayKhamChuaBenhTheoYc.CmndNguoiThan : string.Empty;
            var noiCap = giayKhamChuaBenhTheoYc.TaoLaAi == 1 ? giayKhamChuaBenhTheoYc.NoiCapCmndNguoiThan : string.Empty;
            var noiLamViec = giayKhamChuaBenhTheoYc.TaoLaAi == 1 ? giayKhamChuaBenhTheoYc.NoiLamViecNguoiThan : string.Empty;
            var hoTenBenhNhan = infoBn.HoTenNgBenh;
            var bacSiChamSoc = giayKhamChuaBenhTheoYc.BacSiChamSoc;
            var loaiChuaTri = giayKhamChuaBenhTheoYc.LoaiChuaTri;
            var soTien = giayKhamChuaBenhTheoYc.SoTien != null ? giayKhamChuaBenhTheoYc.SoTien.GetValueOrDefault().ApplyVietnameseFloatNumber() : string.Empty;
            var soTienBangChu = giayKhamChuaBenhTheoYc.SoTienChu;
            var bsDieuTri = giayKhamChuaBenhTheoYc.BsDieuTri;
            var khiCanBaoTin = giayKhamChuaBenhTheoYc.KhiCanBaoTin;
            var ngay = DateTime.Now.Day;
            var thang = DateTime.Now.Month;
            var nam = DateTime.Now.Year;
            var maSoBvCauHinh = await _cauHinhRepository.TableNoTracking.FirstOrDefaultAsync(q => q.Name.Equals("BaoHiemYTe.BenhVienTiepNhan"));
            var maSoBv = maSoBvCauHinh.Value;
            var benhVien = await _benhVienRepository.TableNoTracking.FirstOrDefaultAsync(q => q.Ma.Equals(maSoBv));
            string danToc = string.Empty;
            string ngheNghiep = string.Empty;
            string ngoaiKieu = string.Empty;
            if (giayKhamChuaBenhTheoYc.DanTocNguoiThan != null)
            {
                var danTocObj = await _danTocRepository.TableNoTracking.Where(q => q.Id == giayKhamChuaBenhTheoYc.DanTocNguoiThan)
                    .FirstOrDefaultAsync();
                danToc = danTocObj.Ten;
            }
            if (giayKhamChuaBenhTheoYc.NgheNghiepNguoiThan != null)
            {
                var ngheNghiepObj = await _ngheNghiepRepository.TableNoTracking.Where(q => q.Id == giayKhamChuaBenhTheoYc.NgheNghiepNguoiThan)
                    .FirstOrDefaultAsync();
                ngheNghiep = ngheNghiepObj.Ten;
            }
            if (giayKhamChuaBenhTheoYc.NgoaiKieuNguoiThan != null)
            {
                var quocGiaObj = await _quocGiaRepository.TableNoTracking.Where(q => q.Id == giayKhamChuaBenhTheoYc.NgoaiKieuNguoiThan)
                    .FirstOrDefaultAsync();
                ngoaiKieu = quocGiaObj.Ten;
            }
            var data = new DatInGiayKhamChuaBenhTheoYc();
            if (giayKhamChuaBenhTheoYc.TaoLaAi == 1)
            {
                data = new DatInGiayKhamChuaBenhTheoYc
                {
                    Tuoi = namSinh != null ? (DateTime.Now.Year - namSinh).ToString() : "",
                    NguoiNhan = nguoiNhan,
                    HoTen = hoTenNguoiThan,
                    GioiTinh = gioiTinh,
                    So = infoBn.MaSoTiepNhan,
                    DiaChi = diaChinguoiThan,
                    Cmnd = cmnd,
                    CoQuanCap = noiCap,
                    NguoiDaiDien = hoTenBenhNhan,
                    SoTienUngTruoc = soTien,
                    SoTienUngTruocBangChu = soTienBangChu != null ? soTienBangChu.Substring(1, soTienBangChu.Length - 2) : string.Empty,
                    Ngay = ngay,
                    Thang = thang,
                    Nam = nam,
                    Khoa = khoa,
                    BenhVien = benhVien.Ten,
                    LoaiChuaTri = loaiChuaTri,
                    BacSiChamSoc = bacSiChamSoc,
                    BacSiDieuTri = bsDieuTri,
                    DanToc = danToc,
                    NgheNghiep = ngheNghiep,
                    NgoaiKieu = ngoaiKieu,
                    NoiLamViec = noiLamViec,
                    KhiCanBaoTin = khiCanBaoTin
                };
            }
            else
            {
                data = _noiTruHoSoKhacRepository.TableNoTracking
                       .Where(s => s.YeuCauTiepNhanId == dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId && s.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayKhamChuaBenhTheoYc)
                       .Select(s => new DatInGiayKhamChuaBenhTheoYc()
                       {
                           Tuoi = s.YeuCauTiepNhan.NamSinh != null ? (DateTime.Now.Year - s.YeuCauTiepNhan.NamSinh).ToString() : "",
                           NguoiNhan = nguoiNhan,
                           HoTen = s.YeuCauTiepNhan.HoTen,
                           GioiTinh = s.YeuCauTiepNhan.GioiTinh != null ? s.YeuCauTiepNhan.GioiTinh.GetDescription() : "",
                           So = infoBn.MaSoTiepNhan,
                           DiaChi = s.YeuCauTiepNhan.DiaChiDayDu,
                           Cmnd = s.YeuCauTiepNhan.SoChungMinhThu,
                           CoQuanCap = "", /// hiện tại yêu cầu tiếp nhận không có thông tin này 
                           NguoiDaiDien = hoTenBenhNhan,
                           SoTienUngTruoc = soTien,
                           SoTienUngTruocBangChu = soTienBangChu != null ? soTienBangChu.Substring(1, soTienBangChu.Length - 2) : string.Empty,
                           Ngay = ngay,
                           Thang = thang,
                           Nam = nam,
                           Khoa = khoa,
                           BenhVien = benhVien.Ten,
                           LoaiChuaTri = loaiChuaTri,
                           BacSiChamSoc = bacSiChamSoc,
                           BacSiDieuTri = bsDieuTri,
                           DanToc = s.YeuCauTiepNhan.DanTocId != null ? s.YeuCauTiepNhan.DanToc.Ten : "",
                           NgheNghiep = s.YeuCauTiepNhan.NgheNghiepId != null ? s.YeuCauTiepNhan.NgheNghiep.Ten : "",
                           NgoaiKieu = s.YeuCauTiepNhan.QuocTichId != null ? s.YeuCauTiepNhan.QuocTich.Ten : "",
                           NoiLamViec = noiLamViec,
                           KhiCanBaoTin = khiCanBaoTin
                       }).FirstOrDefault();
            }

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }

        public async Task<string> PhieuInTomTatHoSoBenhAn(
            PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams)
        {
            var template = await _templateRepository.TableNoTracking.FirstAsync(x => x.Name.Equals("TomTatHoSoBenhAn"));
            var infoBn = await ThongTinBenhNhan(dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId);
            var result = await _yctnRepository.GetByIdAsync(dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId, s =>
                s.Include(x => x.BenhNhan).ThenInclude(x => x.NgheNghiep)
                .Include(x => x.NgheNghiep)
                    .Include(x => x.DanToc)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.KhoaPhongNhapVien)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruKhoaPhongDieuTris).ThenInclude(x => x.KhoaPhongChuyenDen)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruEkipDieuTris).ThenInclude(x => x.BacSi).ThenInclude(x => x.User)

                    .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(x => x.GiuongBenh).ThenInclude(x => x.PhongBenhVien)
            );

            var thongTinHoSo = await _noiTruHoSoKhacRepository.TableNoTracking
                .Where(bv => bv.YeuCauTiepNhanId == dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId &&
                             bv.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.TomTatHoSoBenhAn)
                .Select(p => p.ThongTinHoSo).FirstOrDefaultAsync();

            var tomTatHoSoBenhAn = !string.IsNullOrEmpty(thongTinHoSo)
                ? JsonConvert.DeserializeObject<TomTatHoSoBenhAnVo>(thongTinHoSo)
                : new TomTatHoSoBenhAnVo();
            //DataInTomTatHoSoBenhAn
            var data = new
            {
                LogoUrl = dieuTriNoiTruVaServicesHttpParams.HostingName + "/assets/img/logo-bacha-full.png",
                NamSinh = infoBn.NamSinh,
                HoTen = infoBn.HoTenNgBenh,
                DanToc = result.DanToc.Ten,
                NgheNghiep = result.NgheNghiepId != null ? result.NgheNghiep.Ten : "",
                BHYTMaSoThe = result.BHYTMaSoThe,
                NoiLamViec = result.NoiLamViec,
                DiaChiDayDu = result.DiaChiDayDu,
                NgayVaoVien = infoBn.NgayVaoVien != null ? infoBn.NgayVaoVien.GetValueOrDefault().ApplyFormatDate() : string.Empty,
                NgayRaVien = infoBn.NgayRaVien != null ? infoBn.NgayRaVien.GetValueOrDefault().ApplyFormatDate() : string.Empty,
                BenhLyVaLamSang = tomTatHoSoBenhAn.DienBienLamSang,
                TomTatKQXNCLS = tomTatHoSoBenhAn.KqXnCls,
                PhuongPhapDieuTri = tomTatHoSoBenhAn.PpDieuTri,
                TinhTranNguoiBenhRaVien = tomTatHoSoBenhAn.TinhTrangChuyenTuyen,
                GhiChu = tomTatHoSoBenhAn.GhiChu,
                Ngay = DateTime.Now.Day,
                Thang = DateTime.Now.Month,
                Nam = DateTime.Now.Year,
                ThuTruongDonVi = tomTatHoSoBenhAn.GiamDoc,
                ChanDoanRaVien = infoBn.ChanDoanRaVien,
                ChanDoanVaoVien = infoBn.ChanDoanVaoVien,
                BarCodeImgBase64 = !string.IsNullOrEmpty(result.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(result.MaYeuCauTiepNhan) : "",
                MaTN = result.MaYeuCauTiepNhan,
                GT = infoBn.GioiTinh != null ? infoBn.GioiTinh.GetDescription() : ""
            };

            //if (infoBn.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNam)
            //{
            //    data.Male = @"<style>#male:after {        content: '\2714';    }</style>";
            //}

            //if (infoBn.GioiTinh == Enums.LoaiGioiTinh.GioiTinhNu)
            //{
            //    data.Nu = @"<style>#female:after {        content: '\2714';    }</style>";
            //}

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }

        public async Task<string> PhieuInGiayCamKetKyThuatMoi(
            PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams)
        {
            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("GiayCamKetTuNguyenThucHienKyThuatMoiTrongDieuTriGayMeHoiSucTheoYeuCau"));
            var infoBn = await ThongTinBenhNhan(dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId);
            var result = await _yctnRepository.GetByIdAsync(dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId, s =>
                s.Include(x => x.BenhNhan).ThenInclude(x => x.NgheNghiep)

                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.KhoaPhongNhapVien)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruKhoaPhongDieuTris).ThenInclude(x => x.KhoaPhongChuyenDen)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruEkipDieuTris).ThenInclude(x => x.BacSi).ThenInclude(x => x.User)

                    .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(x => x.GiuongBenh).ThenInclude(x => x.PhongBenhVien)
            );
            infoBn.Khoa = result.NoiTruBenhAn != null ? result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Any()
                ? result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Last().KhoaPhongChuyenDen.Ten
                : string.Empty : string.Empty;
            var khoa = infoBn.Khoa;
            GiayCamKetKyThuatMoiVo giayCamKetKyThuatMoi;
            var thongTinHoSo = await _noiTruHoSoKhacRepository.TableNoTracking
                .Where(bv => bv.YeuCauTiepNhanId == dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId &&
                             bv.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayCamKetKyThuatMoi)
                .Select(p => p.ThongTinHoSo).FirstOrDefaultAsync();
            giayCamKetKyThuatMoi = !string.IsNullOrEmpty(thongTinHoSo)
                ? JsonConvert.DeserializeObject<GiayCamKetKyThuatMoiVo>(thongTinHoSo)
                : new GiayCamKetKyThuatMoiVo();
            var hoTenNguoiThan = giayCamKetKyThuatMoi.TaoLaAi == 1 ? giayCamKetKyThuatMoi.HoTen : result.HoTen;
            var diaChinguoiThan = giayCamKetKyThuatMoi.TaoLaAi == 1 ? giayCamKetKyThuatMoi.DiaChi : result.DiaChiDayDu;
            var namSinh = giayCamKetKyThuatMoi.TaoLaAi == 1 ? giayCamKetKyThuatMoi.NamSinh : result.NamSinh;
            var gioiTinh = giayCamKetKyThuatMoi.TaoLaAi == 1 ? giayCamKetKyThuatMoi.GioiTinh != null ? giayCamKetKyThuatMoi.GioiTinh == 1 ? "Nam" : "Nữ" : string.Empty : result.GioiTinh != null ? result.GioiTinh.GetDescription() : "";
            var quanHe = giayCamKetKyThuatMoi.TaoLaAi == 1 ? giayCamKetKyThuatMoi.QuanHe : string.Empty;
            var hoTenBenhNhan = giayCamKetKyThuatMoi.TaoLaAi == 1 ? infoBn.HoTenNgBenh : result.HoTen;
            var nguoiVietCamKet = giayCamKetKyThuatMoi.TaoLaAi == 1 ? infoBn.HoTenNgBenh : result.HoTen;
            var chanDoan = "";
            var tmp = "\n";
            var replace = "<br>";
            if (giayCamKetKyThuatMoi.ChanDoan != null)
            {
                chanDoan = giayCamKetKyThuatMoi.ChanDoan.Replace(tmp, replace);
            }

            var giaiThich = "";
            if (giayCamKetKyThuatMoi.GiaiThich != null)
            {
                giaiThich = giayCamKetKyThuatMoi.GiaiThich.Replace(tmp, replace);
            }
            var soTien = giayCamKetKyThuatMoi.SoTien != null ? giayCamKetKyThuatMoi.SoTien.GetValueOrDefault().ApplyFormatMoneyVNDToDouble() : string.Empty;
            var soTienBangChu = giayCamKetKyThuatMoi.SoTienChu;
            var bsGmhs = giayCamKetKyThuatMoi.BsGmhs;
            var ngay = DateTime.Now.Day;
            var thang = DateTime.Now.Month;
            var nam = DateTime.Now.Year;
            var data = new DataInGiayCamKetKyThuatMoiVo
            {
                Tuoi = namSinh != null ? DateTime.Now.Year - namSinh : null,
                TenToiLa = hoTenNguoiThan,
                MaSoVienPhi = infoBn.MaSoTiepNhan,
                DiaChi = diaChinguoiThan,
                GioiTinh = gioiTinh,
                La = quanHe,
                CuaBenhNhan = hoTenBenhNhan,
                DangDieuTriTaiKhoa = khoa,
                ChanDoan = chanDoan,
                SauKhiDuocNgheBacSiGiaiThichVeViecThucHienKyThuat = giaiThich,
                SoTien = soTien,
                BangChu = soTienBangChu != null ? soTienBangChu.Substring(1, soTienBangChu.Length - 2) : string.Empty,
                NgayHienTai = ngay,
                ThangHienTai = thang,
                NamHienTai = nam,
                BsGmhs = bsGmhs,
                NguoiVietCamKet = nguoiVietCamKet
            };

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }

        public async Task<string> PhieuInBangKiemAnToanPhauThuat(
            PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams)
        {
            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("BangKiemAnToanPhauThuat"));
            var infoBn = await ThongTinBenhNhan(dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId);

            BangKiemAnToanPhauThuatVo phieuThongTinDieuTriVaCacDvModel;
            var thongTinHoSo = await _noiTruHoSoKhacRepository.TableNoTracking
                .Where(bv => bv.YeuCauTiepNhanId == dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId &&
                             bv.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.BangKiemAnToanPhauThuat)
                .Select(p => p.ThongTinHoSo).FirstOrDefaultAsync();
            phieuThongTinDieuTriVaCacDvModel = !string.IsNullOrEmpty(thongTinHoSo)
                ? JsonConvert.DeserializeObject<BangKiemAnToanPhauThuatVo>(thongTinHoSo)
                : new BangKiemAnToanPhauThuatVo();
            var ddChayNgoai = phieuThongTinDieuTriVaCacDvModel.DdChayNgoai;
            var ddDungCu = phieuThongTinDieuTriVaCacDvModel.DdDungCu;
            var ddGayMe = phieuThongTinDieuTriVaCacDvModel.DdGayMe;
            var bsGayMe = phieuThongTinDieuTriVaCacDvModel.BsGayMe;
            var phauThuatVien = phieuThongTinDieuTriVaCacDvModel.PhauThuatVien;
            var chuThich = phieuThongTinDieuTriVaCacDvModel.ChuThich;
            var chanDoanTruocMo = phieuThongTinDieuTriVaCacDvModel.ChanDoan;
            var phongMo = phieuThongTinDieuTriVaCacDvModel.PhongMo;
            var ngay = phieuThongTinDieuTriVaCacDvModel.NgayKiem?.Day;
            var thang = phieuThongTinDieuTriVaCacDvModel.NgayKiem?.Month;
            var nam = phieuThongTinDieuTriVaCacDvModel.NgayKiem?.Year;
            string tmp = "\n";
            string replace = "<br>";
            string chanDoan = "";
            if (phieuThongTinDieuTriVaCacDvModel.ChanDoan != null)
            {
                chanDoan = phieuThongTinDieuTriVaCacDvModel.ChanDoan.Replace(tmp, replace);
            }
            string chuThichs = "";
            if (phieuThongTinDieuTriVaCacDvModel.ChuThich != null)
            {
                chuThichs = phieuThongTinDieuTriVaCacDvModel.ChuThich.Replace(tmp, replace);
            }
            string phongMos = "";
            if (phieuThongTinDieuTriVaCacDvModel.PhongMo != null)
            {
                phongMos = phieuThongTinDieuTriVaCacDvModel.PhongMo.Replace(tmp, replace);
            }
            var data = new DataInBangKiemAnToanPhauThuatVo
            {
                Tuoi = DateTime.Now.Year - infoBn.NamSinh,
                HoTenNguoiBenh = infoBn.HoTenNgBenh,
                Gioi = infoBn.GTNgBenh,
                MaNguoiBenh = infoBn.MaBn,
                ChuanDoanTruocMo = chanDoan,
                PhongMo = phongMos,
                ChuThich = chuThichs,
                DdChayNgoai = ddChayNgoai,
                DdDungCu = ddDungCu,
                DdGayMe = ddGayMe,
                BsGayMe = bsGayMe,
                PhauThuatVien = phauThuatVien,
                Ngay = ngay,
                Thang = thang,
                Nam = nam
            };

            // BVHD-3871
            var dataIn = new DataInUpdateBangKiemAnToanPhauThuatVo();
            //Khoa
            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }
            var tenKhoa = _khoaPhongRepository.TableNoTracking.Where(s => s.Id == khoaId).Select(c => c.Ten).FirstOrDefault();
            
            var tenKhoaIn = string.Empty;
            if (!string.IsNullOrEmpty(tenKhoa))
            {
                tenKhoaIn = tenKhoa.Replace("Khoa", "");
            }
            dataIn.Khoa = "<b>Khoa</b>" + (!string.IsNullOrEmpty(tenKhoaIn) ? tenKhoaIn : "...............");
            //end Khoa


            //Mã Barcode
            dataIn.BarCodeImgBase64 = !string.IsNullOrEmpty(infoBn.MaSoTiepNhan.ToString()) ? BarcodeHelper.GenerateBarCode(infoBn.MaSoTiepNhan) : "";
            dataIn.MaTN = infoBn.MaSoTiepNhan;

            //HoTenNguoiBenh
            dataIn.HoTenNguoiBenh = !string.IsNullOrEmpty(data.HoTenNguoiBenh) ? $"<b>&nbsp;{data.HoTenNguoiBenh}</b>"  : "..............................................................................";
            
            // ngày tháng năm sinh
            var ns = DateHelper.DOBFormat(infoBn.NgaySinh, infoBn.ThangSinh, infoBn.NamSinh);
            dataIn.NgayThangNamSinh = !string.IsNullOrEmpty(ns) ? $"<div class='values'><b> &nbsp;{ns}&nbsp;</b></div>" : $"<div class='value'><b>{ns}</b><div>";

            // giơi tính
            dataIn.GioiTinh = infoBn.GioiTinh  != null ? $"<div class='values'><b> &nbsp;{infoBn.GioiTinh?.GetDescription()}</b></div>" : $"<div class='value'><b>{infoBn.GioiTinh?.GetDescription()}</b><div>";

            // ChuanDoanTruocMo
            dataIn.ChuanDoanTruocMo = !string.IsNullOrEmpty(phieuThongTinDieuTriVaCacDvModel.ChanDoan) ? $"<div class='values'> &nbsp;{phieuThongTinDieuTriVaCacDvModel.ChanDoan}</div>" : $"<div class='value'>{phieuThongTinDieuTriVaCacDvModel.ChanDoan}<div>";

            //NgayThangNam
            if (!string.IsNullOrEmpty(phieuThongTinDieuTriVaCacDvModel.NgayKiemString))
            {
                DateTime.TryParseExact(phieuThongTinDieuTriVaCacDvModel.NgayKiemString, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out DateTime tuNgay);
                dataIn.NgayThangNam = tuNgay != null ? $"<div class='values'> &nbsp;{tuNgay.ApplyFormatDate()}</div>" : $"<div class='value'>{tuNgay.ApplyFormatDate()}<div>";
            }
            //PhongMo
            dataIn.PhongMo = !string.IsNullOrEmpty(phieuThongTinDieuTriVaCacDvModel.PhongMo) ? $"<div class='values'> &nbsp;{phieuThongTinDieuTriVaCacDvModel.PhongMo}</div>" : $"<div class='value'>{phieuThongTinDieuTriVaCacDvModel.PhongMo}<div>";

            //PhuongPhapPhauThuat
            dataIn.PhuongPhapPhauThuat = !string.IsNullOrEmpty(phieuThongTinDieuTriVaCacDvModel.PhuongPhapPhauThuat) ? $"<div class='values'> &nbsp;{phieuThongTinDieuTriVaCacDvModel.PhuongPhapPhauThuat}</div>" : $"<div class='value'>{phieuThongTinDieuTriVaCacDvModel.PhuongPhapPhauThuat}<div>";


            //PhuongThucVoCam
            dataIn.PhuongThucVoCam = !string.IsNullOrEmpty(phieuThongTinDieuTriVaCacDvModel.PhuongPhapVoCam) ? $"<div class='values'> &nbsp;{phieuThongTinDieuTriVaCacDvModel.PhuongPhapVoCam}</div>" : $"<div class='value'>{phieuThongTinDieuTriVaCacDvModel.PhuongPhapVoCam}<div>";

            //BsCheckHoTenCo 
            dataIn.BsCheckHoTenCo = phieuThongTinDieuTriVaCacDvModel.BsCheckHoTen == true ? "<span class='square' style='text-align: center;'>X</span>" : "<span class='square'></span>";
            dataIn.BsCheckHoTenKhong = phieuThongTinDieuTriVaCacDvModel.BsCheckHoTen == false ? "<span class='square' style='text-align: center;'>X</span>" : "<span class='square'></span>";

            //CheckGioiThieuEkipCo 
            dataIn.CheckGioiThieuEkipCo = phieuThongTinDieuTriVaCacDvModel.CheckGioiThieuEkip == true ? "<span class='square' style='text-align: center;'>X</span>" : "<span class='square'></span>";
            dataIn.CheckGioiThieuEkipKhong = phieuThongTinDieuTriVaCacDvModel.CheckGioiThieuEkip == false ? "<span class='square' style='text-align: center;'>X</span>" : "<span class='square'></span>";

            //DdDemDungCuCCo
            dataIn.DdDemDungCuCCo = phieuThongTinDieuTriVaCacDvModel.DdDemDungCu == true ? "<span class='square' style='text-align: center;'>X</span>" : "<span class='square'></span>";
            dataIn.DdDemDungCuKhong = phieuThongTinDieuTriVaCacDvModel.DdDemDungCu == false ? "<span class='square' style='text-align: center;'>X</span>" : "<span class='square'></span>";

            //XnMoCo
            dataIn.XnMoCo = phieuThongTinDieuTriVaCacDvModel.XnMo == true ? "<span class='square' style='text-align: center;'>X</span>" : "<span class='square'></span>";
            dataIn.XnMoKhong = phieuThongTinDieuTriVaCacDvModel.XnMo == false ? "<span class='square' style='text-align: center;'>X</span>" : "<span class='square'></span>";

            //XNLaiThongTinNguoibenhCo
            dataIn.XNLaiThongTinNguoibenhCo = phieuThongTinDieuTriVaCacDvModel.XNLaiThongTinNguoibenh == true ? "<span class='square' style='text-align: center;'>X</span>" : "<span class='square'></span>";
            dataIn.XNLaiThongTinNguoibenhKhong = phieuThongTinDieuTriVaCacDvModel.XNLaiThongTinNguoibenh == false ? "<span class='square' style='text-align: center;'>X</span>" : "<span class='square'></span>";


            //DanhDauVungMoText
            if(phieuThongTinDieuTriVaCacDvModel.DanhDauVungMo == 1)
            {
                dataIn.DanhDauVungMoText  = "&nbsp;&nbsp;&nbsp;Có <span class='square' style='text-align: center;'>X</span>" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Không <span class='square'></span>" + "&nbsp;&nbsp;&nbsp;" + "Không áp dụng <span class='square'></span>";
            }
            if(phieuThongTinDieuTriVaCacDvModel.DanhDauVungMo == 2)
            {
                dataIn.DanhDauVungMoText = "&nbsp;&nbsp;&nbsp;Có <span class='square'></span>" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Không <span class='square' style='text-align: center;'>X</span>" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Không áp dụng <span class='square'></span>";
            }
            if (phieuThongTinDieuTriVaCacDvModel.DanhDauVungMo == 3)
            {
                dataIn.DanhDauVungMoText = "&nbsp;&nbsp;&nbsp;Có <span class='square'></span>" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Không <span class='square'></span>" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Không áp dụng <span class='square' style='text-align: center;'>X</span>";
            }

            //KhangSinhDuPhongbSuDungDcKhongText
            if (phieuThongTinDieuTriVaCacDvModel.XnKhangSinhDuPhong == 1)
            {
                dataIn.KhangSinhDuPhongbSuDungDcKhongText = "&nbsp;&nbsp;&nbsp;Có <span class='square' style='text-align: center;'>X</span>" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Không <span class='square'></span>" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Không áp dụng <span class='square'></span>";
            }
            if (phieuThongTinDieuTriVaCacDvModel.XnKhangSinhDuPhong == 2)
            {
                dataIn.KhangSinhDuPhongbSuDungDcKhongText = "&nbsp;&nbsp;&nbsp;Có <span class='square'></span>" + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + "Không <span class='square' style='text-align: center;'>X</span>" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Không áp dụng <span class='square'></span>";
            }
            if (phieuThongTinDieuTriVaCacDvModel.XnKhangSinhDuPhong == 3)
            {
                dataIn.KhangSinhDuPhongbSuDungDcKhongText = "&nbsp;&nbsp;&nbsp;Có <span class='square'></span>" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Không <span class='square'></span>" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Không áp dụng <span class='square' style='text-align: center;'>X</span>";
            }

            //DanNhanBenhPhamText
            if (phieuThongTinDieuTriVaCacDvModel.DanNhanBenhPham == 1)
            {
                dataIn.DanNhanBenhPhamText = "&nbsp;&nbsp;&nbsp;Có <span class='square' style='text-align: center;'>X</span>" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Không <span class='square'></span>" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Không áp dụng <span class='square'></span>";
            }
            if (phieuThongTinDieuTriVaCacDvModel.DanNhanBenhPham == 2)
            {
                dataIn.DanNhanBenhPhamText = "&nbsp;&nbsp;&nbsp;Có <span class='square'></span>" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Không <span class='square' style='text-align: center;'>X</span>" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Không áp dụng <span class='square'></span>";
            }
            if (phieuThongTinDieuTriVaCacDvModel.DanNhanBenhPham == 3)
            {
                dataIn.DanNhanBenhPhamText = "&nbsp;&nbsp;&nbsp;Có <span class='square'></span>" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Không <span class='square'></span>" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Không áp dụng <span class='square' style='text-align: center;'>X</span>";
            }

            //ThuocvaThietBiGayMeCoDayDuKhongCo
            dataIn.ThuocvaThietBiGayMeCoDayDuKhongCo = phieuThongTinDieuTriVaCacDvModel.ThuocvaThietBiGayMeCoDayDuKhong == true ? "<span class='square'>X</span>" : "<span class='square'></span>";
            dataIn.ThuocvaThietBiGayMeCoDayDuKhongKhong = phieuThongTinDieuTriVaCacDvModel.ThuocvaThietBiGayMeCoDayDuKhong == false ? "<span class='square'>X</span>" : "<span class='square'></span>";


            //XacNhanLiVTRachDaCo
            dataIn.XacNhanLiVTRachDaCo = phieuThongTinDieuTriVaCacDvModel.XnVtRachDa == true ? "<span class='square'>X</span>" : "<span class='square'></span>";
            dataIn.XacNhanLiVTRachDaKhong = phieuThongTinDieuTriVaCacDvModel.XnVtRachDa == false ? "<span class='square'>X</span>" : "<span class='square'></span>";

            //DamBaoAnToanVoKhuanCo
            dataIn.DamBaoAnToanVoKhuanCo = phieuThongTinDieuTriVaCacDvModel.DanLuu == true ? "<span class='square'>X</span>" : "<span class='square'></span>";
            dataIn.DamBaoAnToanVoKhuanKhong = phieuThongTinDieuTriVaCacDvModel.DanLuu == false ? "<span class='square'>X</span>" : "<span class='square'></span>";

            //XMonitorCo
            dataIn.XMonitorCo = phieuThongTinDieuTriVaCacDvModel.XnMonitor == true ? "<span class='square'>X</span>" : "<span class='square'></span>";
            dataIn.XMonitorKhong = phieuThongTinDieuTriVaCacDvModel.XnMonitor == false ? "<span class='square'>X</span>" : "<span class='square'></span>";

            //XMonitorCo
            dataIn.XacNhanDieuCanChuYCo = phieuThongTinDieuTriVaCacDvModel.XacNhanDieuCanChuYTrongPT == true ? "<span class='square'>X</span>" : "<span class='square'></span>";
            dataIn.XacNhanDieuCanChuYKhong = phieuThongTinDieuTriVaCacDvModel.XacNhanDieuCanChuYTrongPT == false ? "<span class='square'>X</span>" : "<span class='square'></span>";

            //XacNhanNhungDieuCanChuYVeHoiTinhChamSocSauMoCo
            dataIn.XacNhanNhungDieuCanChuYVeHoiTinhChamSocSauMoCo = phieuThongTinDieuTriVaCacDvModel.XacNhanDieuCanChuYVeHoiTinhVaChamSocSauMo == true ? "<span class='square'>X</span>" : "<span class='square'></span>";
            dataIn.XacNhanNhungDieuCanChuYVeHoiTinhChamSocSauMoKhong = phieuThongTinDieuTriVaCacDvModel.XacNhanDieuCanChuYVeHoiTinhVaChamSocSauMo == false ? "<span class='square'>X</span>" : "<span class='square'></span>";

            //NguoiiBenhTienSuBenhCo
            dataIn.NguoiiBenhTienSuBenhCo = phieuThongTinDieuTriVaCacDvModel.NguoiBenhCoTienSuDiUng == true ? "<span class='square'>X</span>" : "<span class='square'></span>";
            dataIn.NguoiiBenhTienSuBenhKhong = phieuThongTinDieuTriVaCacDvModel.NguoiBenhCoTienSuDiUng == false ? "<span class='square'>X</span>" : "<span class='square'></span>";

            //BSGayMeCoDieuGiChuYCo
            dataIn.BSGayMeCoDieuGiChuYCo = phieuThongTinDieuTriVaCacDvModel.BSGayMeCanChuYTrongGayMe == true ? "<span class='square'>X</span>" : "<span class='square'></span>";
            dataIn.BSGayMeCoDieuGiChuYKhong = phieuThongTinDieuTriVaCacDvModel.BSGayMeCanChuYTrongGayMe == false ? "<span class='square'>X</span>" : "<span class='square'></span>";

            //NguoiiBenhCoNguyCoCo
            dataIn.NguoiiBenhCoNguyCoCo = phieuThongTinDieuTriVaCacDvModel.XnKhoTho == true ? "<span class='square'>X</span>" : "<span class='square'></span>";
            dataIn.NguoiiBenhCoNguyCoKhong = phieuThongTinDieuTriVaCacDvModel.XnKhoTho == false ? "<span class='square'>X</span>" : "<span class='square'></span>";


            dataIn.ChuThich = phieuThongTinDieuTriVaCacDvModel.ChuThich;


            //NguoiiBenhCoNguyCoCo
            dataIn.DieuDuongDungCuCo = phieuThongTinDieuTriVaCacDvModel.BsXnChuY == true ? "<span class='square'>X</span>" : "<span class='square'></span>";
            dataIn.DieuDuongDungCuKhong = phieuThongTinDieuTriVaCacDvModel.BsXnChuY == false ? "<span class='square'>X</span>" : "<span class='square'></span>";

            //NguyCoMatMauCo
            dataIn.NguyCoMatMauCo = phieuThongTinDieuTriVaCacDvModel.XnMatMau == true ? "<span class='square'>X</span>" : "<span class='square'></span>";
            dataIn.NguyCoMatMauKhong = phieuThongTinDieuTriVaCacDvModel.XnMatMau == false ? "<span class='square'>X</span>" : "<span class='square'></span>";

            //ThucHienDemKimGacDungCuCo
            dataIn.ThucHienDemKimGacDungCuCo = phieuThongTinDieuTriVaCacDvModel.KimGacDungCu == true ? "<span class='square'>X</span>" : "<span class='square'></span>";
            dataIn.ThucHienDemKimGacDungCuKhong = phieuThongTinDieuTriVaCacDvModel.KimGacDungCu == false ? "<span class='square'>X</span>" : "<span class='square'></span>";

            //DanNhanBenhPhamText
            if (phieuThongTinDieuTriVaCacDvModel.DatPlaQueDaoDien == 1)
            {
                dataIn.DatPlaqueDienText = "&nbsp;&nbsp;&nbsp;Có <span class='square' style='text-align: center;'>X</span>" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Không <span class='square'></span>" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Không áp dụng <span class='square'></span>";
            }
            if (phieuThongTinDieuTriVaCacDvModel.DatPlaQueDaoDien == 2)
            {
                dataIn.DatPlaqueDienText = "&nbsp;&nbsp;&nbsp;Có <span class='square'></span>" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Không <span class='square' style='text-align: center;'>X</span>" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Không áp dụng <span class='square'></span>";
            }
            if (phieuThongTinDieuTriVaCacDvModel.DatPlaQueDaoDien == 3)
            {
                dataIn.DatPlaqueDienText = "&nbsp;&nbsp;&nbsp;Có <span class='square'></span>" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Không <span class='square'></span>" + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Không áp dụng <span class='square' style='text-align: center;'c>X</span>";
            }

            dataIn.DdChayNgoai = data.DdChayNgoai;
            dataIn.DdDungCu = data.DdDungCu;
            dataIn.DdGayMe = data.DdGayMe;
            dataIn.BsGayMe = data.BsGayMe;
            dataIn.PhauThuatVien = data.PhauThuatVien;
            dataIn.Vector = dieuTriNoiTruVaServicesHttpParams.HostingName + "/assets/img/vector.png";


            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, dataIn);
            return content;
        }

        public async Task<string> PhieuInThongTinDieuTriVaCacDichVu(
            PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams)
        {
            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuTTTTDieuTriVaCacDichVu"));
            var result = await _yctnRepository.GetByIdAsync(dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId, s =>
                s.Include(x => x.BenhNhan).ThenInclude(x => x.NgheNghiep)

                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.KhoaPhongNhapVien)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruKhoaPhongDieuTris).ThenInclude(x => x.KhoaPhongChuyenDen)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruEkipDieuTris).ThenInclude(x => x.BacSi).ThenInclude(x => x.User)

                    .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(x => x.GiuongBenh).ThenInclude(x => x.PhongBenhVien)
            );
            var infoBn = await ThongTinBenhNhan(dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId);
            infoBn.Khoa = result.NoiTruBenhAn != null ? result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Any()
                ? result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Last().KhoaPhongChuyenDen.Ten
                : string.Empty : string.Empty;

            CuringInfoAndServicesGeneralSheetVo phieuThongTinDieuTriVaCacDvModel;
            var thongTinHoSo = await _noiTruHoSoKhacRepository.TableNoTracking
                .Where(bv => bv.YeuCauTiepNhanId == dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId &&
                             bv.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.PhieuTomTatThongTinDieuTri)
                .Select(p => p.ThongTinHoSo).FirstOrDefaultAsync();
            phieuThongTinDieuTriVaCacDvModel = !string.IsNullOrEmpty(thongTinHoSo)
                ? JsonConvert.DeserializeObject<CuringInfoAndServicesGeneralSheetVo>(thongTinHoSo)
                : new CuringInfoAndServicesGeneralSheetVo();
            var hoTenBs = await _userRepository.TableNoTracking
                .Where(q => q.Id == phieuThongTinDieuTriVaCacDvModel.BsDieuTri.GetValueOrDefault())
                .Select(w => w.HoTen).FirstOrDefaultAsync();
         


            var data = new PhieuInThongTinDieuTriVaCacDichVu();

            data.BarCodeImgBase64 = !string.IsNullOrEmpty(result.MaYeuCauTiepNhan.ToString()) ? BarcodeHelper.GenerateBarCode(result.MaYeuCauTiepNhan.ToString()) : "";
            data.MaTN = result.MaYeuCauTiepNhan;

            data.HoTenNgBenh = !string.IsNullOrEmpty(infoBn.HoTenNgBenh) ? "<div class='container'>" +
                                                                                "<div class='label'>- Họ tên người bệnh:&nbsp;</div>" +
                                                                                $"<div class='values'><b>{infoBn.HoTenNgBenh}</b></div>" +
                                                                                "</div>"
                                                                                :
                                                                           "<div class='container'>" +
                                                                                "<div class='label'>- Họ tên người bệnh:&nbsp;</div>" +
                                                                                $"<div class='value'><b>{infoBn.HoTenNgBenh}</b></div>" +
                                                                                "</div>";

            var ns = DateHelper.DOBFormat(result.NgaySinh, result.ThangSinh, result.NamSinh);

            data.NgayThangNamSinh = !string.IsNullOrEmpty(ns) ? "<div class='container'>"+
                                                                      "<div class='label'>- Ngày/tháng/năm sinh:&nbsp;</div>"+
                                                                      $"<div class='values'><b>{ns}</b></div>" +
                                                                      "</div>"
                                                                      :
                                                                 "<div class='container'>" +
                                                                      "<div class='label'>- Ngày/tháng/năm sinh:&nbsp;</div>" +
                                                                      $"<div class='value'><b>{ns}</b></div>" +
                                                                      "</div>";

            data.GTNgBenh = !string.IsNullOrEmpty(infoBn.GioiTinh?.GetDescription()) ? "<div class='container'>" +
                                                                                            "<div class='label'>Giới tính:&nbsp;</div>" +
                                                                                            $"<div class='values'><b>{infoBn.GioiTinh?.GetDescription()}</b></div>" +
                                                                                            "</div>"
                                                                                            :
                                                                                        "<div class='container'>" +
                                                                                            "<div class='label'>Giới tính:&nbsp;</div>" +
                                                                                            $"<div class='value'><b>{infoBn.GioiTinh?.GetDescription()}</b></div>" +
                                                                                            "</div>";


            data.DiaChi = !string.IsNullOrEmpty(result.DiaChiDayDu) ? "<div class='container'>"+ 
                                                                            "<div class='label'>- Địa chỉ:&nbsp;</div>"+
                                                                            $"<div class='values'>{result.DiaChiDayDu}</div>" +
                                                                            "</div>"
                                                                      :
                                                                      "<div class='container'>" +
                                                                            "<div class='label'>- Địa chỉ:&nbsp;</div>" +
                                                                            $"<div class='value'>{result.DiaChiDayDu}</div>" +
                                                                            "</div>";

            var khoa = string.Empty;
            if(!string.IsNullOrEmpty(infoBn.Khoa))
            {
                khoa = infoBn.Khoa?.Replace("Khoa", "");
                khoa = khoa.Replace("KHOA", "");
            }

            data.Khoa = !string.IsNullOrEmpty(khoa) ? "<div class='container'>" +
                                                                  "<div class='label'>- Khoa:&nbsp;</div>" +
                                                                  $"<div class='values'>{khoa}</div>" +
                                                                  "</div>"
                                                           :
                                                           "<div class='container'>" +
                                                                  "<div class='label'>- Khoa:&nbsp;</div>" +
                                                                  $"<div class='value'>{khoa}</div>" +
                                                                  "</div>";
            var buong = string.Empty;
            if (!string.IsNullOrEmpty(infoBn.Buong))
            {
                buong = infoBn.Buong?.Replace("Buồng", "");
                buong = buong.Replace("BUỒNG", "");
            }

            data.Buong = !string.IsNullOrEmpty(buong) ? "<div class='container'>" +
                                                                "<div class='label'>Buồng:&nbsp;</div>" +
                                                                $"<div class='values'>{buong}</div>" +
                                                                "</div>"
                                                         :
                                                         "<div class='container'>" +
                                                                "<div class='label'>Buồng:&nbsp;</div>" +
                                                                $"<div class='value'>{buong}</div>" +
                                                                "</div>";

          
            var giuong = string.Empty;
            if (!string.IsNullOrEmpty(infoBn.Giuong))
            {
                giuong = infoBn.Giuong?.Replace("Giường", "");
                giuong = giuong.Replace("GIƯỜNG", "");
            }

            data.Giuong = !string.IsNullOrEmpty(buong) ? "<div class='container'>" +
                                                                "<div class='label'>Giường:&nbsp;</div>" +
                                                                $"<div class='values'>{giuong}</div>" +
                                                                "</div>"
                                                         :
                                                         "<div class='container'>" +
                                                                "<div class='label'>Giường:&nbsp;</div>" +
                                                                $"<div class='value'>{giuong}</div>" +
                                                                "</div>";

            data.ChanDoan = !string.IsNullOrEmpty(phieuThongTinDieuTriVaCacDvModel.ChanDoan) ? "<tr>" +
                                                                                                   "<td style='width:100%;word-break: break-work;' colspan='3'>" +
                                                                                                      "<div class='container'>" +
                                                                                                          $"<div class='values'>- Chẩn đoán:&nbsp;{phieuThongTinDieuTriVaCacDvModel.ChanDoan?.Replace("\n", "<br>")}</div>" +
                                                                                                          "</div>" +
                                                                                                   "</td>" +
                                                                                                "</tr>"
                                                                                               :
                                                                                               "<tr>" +
                                                                                                   "<td style='width:100%;word-break: break-work;' colspan='3'>" +
                                                                                                      "<div class='container'>" +
                                                                                                          "<div class='label'>- Chẩn đoán:&nbsp;</div>" +
                                                                                                          $"<div class='value'>&nbsp;</div>" +
                                                                                                          "</div>" +
                                                                                                   "</td>" +
                                                                                                "</tr>" +
                                                                                                "<tr>" +
                                                                                                   "<td style='width:100%;word-break: break-work;' colspan='3'>" +
                                                                                                      "<div class='container'>" +
                                                                                                          $"<div class='value'>&nbsp;</div>" +
                                                                                                          "</div>" +
                                                                                                   "</td>" +
                                                                                                "</tr>";

            data.DuKienPPDieuTri = !string.IsNullOrEmpty(phieuThongTinDieuTriVaCacDvModel.MethodCuringPlan) ? "<tr>" +
                                                                                                   "<td style='width:100%;word-break: break-work;' colspan='3'>" +
                                                                                                      "<div class='container'>" +
                                                                                                          $"<div class='values'>{phieuThongTinDieuTriVaCacDvModel.MethodCuringPlan?.Replace("\n", "<br>")}</div>" +
                                                                                                          "</div>" +
                                                                                                   "</td>" +
                                                                                                "</tr>"
                                                                                               :
                                                                                               "<tr>" +
                                                                                                   "<td style='width:100%;word-break: break-work;' colspan='3'>" +
                                                                                                      "<div class='container'>" +
                                                                                                          $"<div class='value'>&nbsp;</div>" +
                                                                                                          "</div>" +
                                                                                                   "</td>" +
                                                                                                "</tr>" +
                                                                                                "<tr>" +
                                                                                                   "<td style='width:100%;word-break: break-work;' colspan='3'>" +
                                                                                                      "<div class='container'>" +
                                                                                                          $"<div class='value'>&nbsp;</div>" +
                                                                                                          "</div>" +
                                                                                                   "</td>" +
                                                                                                "</tr>"+
                                                                                                 "<tr>" +
                                                                                                   "<td style='width:100%;word-break: break-work;' colspan='3'>" +
                                                                                                      "<div class='container'>" +
                                                                                                          $"<div class='value'>&nbsp;</div>" +
                                                                                                          "</div>" +
                                                                                                   "</td>" +
                                                                                                "</tr>";


            data.TienLuong = !string.IsNullOrEmpty(phieuThongTinDieuTriVaCacDvModel.ChanDoan) ? "<tr>" +
                                                                                          "<td style='width:100%;word-break: break-work;' colspan='3'>" +
                                                                                             "<div class='container'>" +
                                                                                                 $"<div class='values'>- Tiên lượng:&nbsp;{phieuThongTinDieuTriVaCacDvModel.TienLuong?.Replace("\n", "<br>")}</div>" +
                                                                                                 "</div>" +
                                                                                          "</td>" +
                                                                                       "</tr>"
                                                                                      :
                                                                                      "<tr>" +
                                                                                          "<td style='width:100%;word-break: break-work;' colspan='3'>" +
                                                                                             "<div class='container'>" +
                                                                                                 "<div class='label'>- Tiên lượng:&nbsp;</div>" +
                                                                                                 $"<div class='value'>&nbsp;</div>" +
                                                                                                 "</div>" +
                                                                                          "</td>" +
                                                                                       "</tr>" +
                                                                                       "<tr>" +
                                                                                          "<td style='width:100%;word-break: break-work;' colspan='3'>" +
                                                                                             "<div class='container'>" +
                                                                                                 $"<div class='value'>&nbsp;</div>" +
                                                                                                 "</div>" +
                                                                                          "</td>" +
                                                                                       "</tr>";

            data.NhungDieuCanLuuY = !string.IsNullOrEmpty(phieuThongTinDieuTriVaCacDvModel.LuuY) ? "<tr>" +
                                                                                         "<td style='width:100%;word-break: break-work;' colspan='3'>" +
                                                                                            "<div class='container'>" +
                                                                                                $"<div class='values'>- Những điều cần lưu ý trong quá trình điều trị <i>(Nội dung tư vấn về bệnh tật của BS):&nbsp;</i>{phieuThongTinDieuTriVaCacDvModel.LuuY?.Replace("\n", "<br>")}</div>" +
                                                                                                "</div>" +
                                                                                         "</td>" +
                                                                                      "</tr>"
                                                                                     :
                                                                                     "<tr>" +
                                                                                         "<td style='width:100%;word-break: break-work;' colspan='3'>" +
                                                                                            "<div class='container'>" +
                                                                                                "<div class='label'>- Những điều cần lưu ý trong quá trình điều trị <i>(Nội dung tư vấn về bệnh tật của BS):&nbsp;</i></div>" +
                                                                                                $"<div class='value'>&nbsp;</div>" +
                                                                                                "</div>" +
                                                                                         "</td>" +
                                                                                      "</tr>" +
                                                                                      "<tr>" +
                                                                                         "<td style='width:100%;word-break: break-work;' colspan='3'>" +
                                                                                            "<div class='container'>" +
                                                                                                $"<div class='value'>&nbsp;</div>" +
                                                                                                "</div>" +
                                                                                         "</td>" +
                                                                                      "</tr>"+
                                                                                       "<tr>" +
                                                                                         "<td style='width:100%;word-break: break-work;' colspan='3'>" +
                                                                                            "<div class='container'>" +
                                                                                                $"<div class='value'>&nbsp;</div>" +
                                                                                                "</div>" +
                                                                                         "</td>" +
                                                                                      "</tr>"+
                                                                                      "<tr>" +
                                                                                         "<td style='width:100%;word-break: break-work;' colspan='3'>" +
                                                                                            "<div class='container'>" +
                                                                                                $"<div class='value'>&nbsp;</div>" +
                                                                                                "</div>" +
                                                                                         "</td>" +
                                                                                      "</tr>";

            data.ThongTinVeGiaDV = !string.IsNullOrEmpty(phieuThongTinDieuTriVaCacDvModel.ServicesPriceInfo) ? "<tr>" +
                                                                                        "<td style='width:100%;word-break: break-work;' colspan='3'>" +
                                                                                           "<div class='container'>" +
                                                                                               $"<div class='values'>- Thông tin về giá các dịch vụ: <i>(giường dịch vụ theo yêu cầu, phẫu thuật theo yêu cầu, dịch vụ kỹ thuật cao, vv...)&nbsp;</i>{phieuThongTinDieuTriVaCacDvModel.ServicesPriceInfo?.Replace("\n", "<br>")}</div>" +
                                                                                               "</div>" +
                                                                                        "</td>" +
                                                                                     "</tr>"
                                                                                    :
                                                                                    "<tr>" +
                                                                                        "<td style='width:100%;word-break: break-work;' colspan='3'>" +
                                                                                           "<div class='container'>" +
                                                                                              "<div class='label'>- Thông tin về giá các dịch vụ: <i>(giường dịch vụ theo yêu cầu, phẫu thuật theo yêu cầu, dịch vụ kỹ thuật cao, vv...)</i>&nbsp;</div>" +
                                                                                               $"<div class='value'>&nbsp;</div>" +
                                                                                               "</div>" +
                                                                                        "</td>" +
                                                                                     "</tr>" +
                                                                                     "<tr>" +
                                                                                        "<td style='width:100%;word-break: break-work;' colspan='3'>" +
                                                                                           "<div class='container'>" +
                                                                                               $"<div class='value'>&nbsp;</div>" +
                                                                                               "</div>" +
                                                                                        "</td>" +
                                                                                     "</tr>"+
                                                                                      "<tr>" +
                                                                                        "<td style='width:100%;word-break: break-work;' colspan='3'>" +
                                                                                           "<div class='container'>" +
                                                                                               $"<div class='value'>&nbsp;</div>" +
                                                                                               "</div>" +
                                                                                        "</td>" +
                                                                                     "</tr>";
                                                                                    

            


            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }
            var tenKhoa = _khoaPhongRepository.TableNoTracking.Where(s => s.Id == khoaId).Select(c => c.Ten).FirstOrDefault();

            data.KhoaCreate = tenKhoa;

            var ngay = phieuThongTinDieuTriVaCacDvModel.NgayThucHien.GetValueOrDefault().Day;
            var thang = phieuThongTinDieuTriVaCacDvModel.NgayThucHien.GetValueOrDefault().Month;
            var nam = phieuThongTinDieuTriVaCacDvModel.NgayThucHien.GetValueOrDefault().Year;

            data.Ngay = ngay > 9 ? ngay + "" : "0" +ngay;

            data.Thang = thang > 9 ? thang + "" : "0" + thang;

            data.Nam = nam +"";
            data.NguoiBenh = infoBn.HoTenNgBenh;
            if(phieuThongTinDieuTriVaCacDvModel.BsDieuTri != null)
            {
                data.HoTenBacSi = _userRepository.TableNoTracking.Where(d=>d.Id == phieuThongTinDieuTriVaCacDvModel.BsDieuTri).Select(d=>d.HoTen).FirstOrDefault();
            }
            

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }

        private async Task<DataInPhieuDieuTriVaSerivcesVo> ThongTinBenhNhan(long yeuCauTiepNhanId)
        {
            var thongTinBenhNhanPhieuThuoc = _yctnRepository.TableNoTracking
                .Where(s => s.Id == yeuCauTiepNhanId)
                .Select(s => new DataInPhieuDieuTriVaSerivcesVo
                {
                    HoTenNgBenh = s.HoTen,
                    NamSinh = s.NamSinh,
                    GTNgBenh = s.GioiTinh.GetDescription(),
                    GioiTinh = s.GioiTinh,
                    DiaChi = s.BenhNhan.DiaChiDayDu,
                    Cmnd = s.SoChungMinhThu,
                    MaBn = s.BenhNhan.MaBN,
                    NhomMau = s.NhomMau != null ? s.NhomMau.GetDescription() : string.Empty,
                    MaSoTiepNhan = s.MaYeuCauTiepNhan,
                    NgayVaoVien = s.NoiTruBenhAn.ThoiDiemNhapVien,
                    NgayRaVien = s.NoiTruBenhAn.ThoiDiemRaVien,
                    //ChanDoanRaVien = s.NoiTruBenhAn.LoaiBenhAn != LoaiBenhAn.SanKhoaMo && s.NoiTruBenhAn.LoaiBenhAn != LoaiBenhAn.SanKhoaThuong  && s.NoiTruBenhAn.ChanDoanChinhRaVienICD != null ? 
                    //                     MaskHelper.ICDDisplay(s.NoiTruBenhAn.ChanDoanChinhRaVienICD.Ma,s.NoiTruBenhAn.ChanDoanChinhRaVienICD.TenTiengViet, s.NoiTruBenhAn.ChanDoanChinhRaVienGhiChu, KieuHienThiICD.MaGachNgangTenNgoacTronGhiChu) :
                    //                     (s.NoiTruBenhAn.NoiTruPhieuDieuTris.Any() != null && s.NoiTruBenhAn.NoiTruPhieuDieuTris.Last().ChanDoanChinhICD != null) ? 
                    //                     MaskHelper.ICDDisplay(s.NoiTruBenhAn.NoiTruPhieuDieuTris.Last().ChanDoanChinhICD.Ma, s.NoiTruBenhAn.NoiTruPhieuDieuTris.Last().ChanDoanChinhICD.TenTiengViet,s.NoiTruBenhAn.NoiTruPhieuDieuTris.Last().ChanDoanChinhGhiChu,KieuHienThiICD.MaGachNgangTenNgoacTronGhiChu): "",
                    ChanDoanVaoVien = s.YeuCauNhapVien.ChanDoanNhapVienGhiChu,
                    Buong = s.YeuCauDichVuGiuongBenhViens.Where(x => x.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(x => x.Id).Select(x => x.GiuongBenh.PhongBenhVien.Ten).FirstOrDefault(),
                    Giuong = s.YeuCauDichVuGiuongBenhViens.Where(x => x.DoiTuongSuDung == Enums.DoiTuongSuDung.BenhNhan).OrderBy(x => x.Id).Select(x => x.GiuongBenh.Ten).FirstOrDefault(),
                    LoaiBenhAn = s.NoiTruBenhAn.LoaiBenhAn,
                    NgaySinh = s.NgaySinh,
                    ThangSinh = s.ThangSinh
                });
            var thongTinBenhNhan = await thongTinBenhNhanPhieuThuoc.FirstAsync();
            var chanDoanFist = _yctnRepository.TableNoTracking.Where(s => s.Id == yeuCauTiepNhanId)
              .Select(d => new
              {
                  Ma = d.NoiTruBenhAn.ChanDoanChinhRaVienICD.Ma,
                  Ten = d.NoiTruBenhAn.ChanDoanChinhRaVienICD.TenTiengViet,
                  GhiChu = d.NoiTruBenhAn.ChanDoanChinhRaVienGhiChu,
                  MaNV = d.YeuCauNhapVien.ChanDoanNhapVienICD.Ma,
                  TenNV = d.YeuCauNhapVien.ChanDoanNhapVienICD.TenTiengViet,
                  GhiChuNV = d.YeuCauNhapVien.ChanDoanNhapVienGhiChu,
                  NoiTruPhieuDieuTris = d.NoiTruBenhAn.NoiTruPhieuDieuTris,
                  ChanDoanKemTheos = d.NoiTruBenhAn.NoiTruPhieuDieuTris.Any() ? d.NoiTruBenhAn.NoiTruPhieuDieuTris
                                                                              .SelectMany(df => df.NoiTruThamKhamChanDoanKemTheos)
                                                                              .Select(s => (s.ICD.Ma + "-" + s.ICD.TenTiengViet + "(" + s.GhiChu + ")")).ToList() : null
              }).ToList();
            if (chanDoanFist.Any())
            {
                thongTinBenhNhan.ChanDoanVaoVien = MaskHelper.ICDDisplay(chanDoanFist.First().MaNV, chanDoanFist.First().TenNV, chanDoanFist.First().GhiChuNV, KieuHienThiICD.MaGachNgangTenNgoacTronGhiChu);

                if (thongTinBenhNhan.LoaiBenhAn != LoaiBenhAn.SanKhoaMo && thongTinBenhNhan.LoaiBenhAn != LoaiBenhAn.SanKhoaThuong)
                {
                    thongTinBenhNhan.ChanDoanRaVien = MaskHelper.ICDDisplay(chanDoanFist.First().Ma, chanDoanFist.First().Ten, chanDoanFist.First().GhiChu, KieuHienThiICD.MaGachNgangTenNgoacTronGhiChu);
                }
                else
                {
                    var cd = string.Empty;
                    if (chanDoanFist.First().NoiTruPhieuDieuTris.Any())
                    {
                        var chanDoanKemTheo = string.Empty;
                        // lấy thông tin bệnh an phiếu điều trị ngày cuối cùng
                        //var chanDoanKemTheos = chanDoanFist.First().NoiTruPhieuDieuTris.Select(d => d.NoiTruThamKhamChanDoanKemTheos.Select(s => (s.ICD.Ma + "-" + s.ICD.TenTiengViet + "(" + s.GhiChu + ")"))).LastOrDefault();

                        if (chanDoanFist.First().ChanDoanKemTheos.Any())
                        {
                            chanDoanKemTheo = "Chẩn đoán kèm theo: " + string.Join("; ", chanDoanFist.First().ChanDoanKemTheos);
                        }
                        var chanDoanChinhICDId = chanDoanFist.First().NoiTruPhieuDieuTris.Select(d => d.ChanDoanChinhICDId).LastOrDefault();
                        var chanDoanChinhICDGhiChu = chanDoanFist.First().NoiTruPhieuDieuTris.Select(d => d.ChanDoanChinhGhiChu).LastOrDefault();

                        if (chanDoanChinhICDId != null)
                        {
                            var chanDoan = chanDoanChinhICDId != null ?
                                          ChanDoan((long)chanDoanChinhICDId) : "";

                            cd += chanDoan != null ? chanDoan + "(" + chanDoanChinhICDGhiChu + ")" : chanDoanChinhICDGhiChu;
                            if (chanDoanKemTheo != null)
                            {
                                cd += "; " + chanDoanKemTheo;
                            }
                        }
                        else
                        {
                            cd += chanDoanKemTheo;
                        }
                    }
                    thongTinBenhNhan.ChanDoanRaVien = cd;
                }
            }
            return thongTinBenhNhan;
        }

        public async Task<string> GetChanDoan(long yctnId)
        {
            var getChanDoan = _noiTruPhieuDieuTriRepository.TableNoTracking
                .Where(e => e.NgayDieuTri.Day == DateTime.Now.Day && e.NgayDieuTri.Month == DateTime.Now.Month && e.NgayDieuTri.Year == DateTime.Now.Year && e.NoiTruBenhAnId == yctnId)
                .Select(x => x.ChanDoanChinhGhiChu);
            return await getChanDoan.LastOrDefaultAsync();
        }
        public string GetChanDoanNhapVien(long yctnId)
        {
            var chanDoan = _yctnRepository.TableNoTracking.Where(x => x.YeuCauNhapVien != null && x.Id == yctnId)
                .Select(x =>
                             (string.IsNullOrEmpty(x.YeuCauNhapVien.ChanDoanNhapVienGhiChu) ? x.YeuCauNhapVien.ChanDoanNhapVienICD.Ma + " - " + x.YeuCauNhapVien.ChanDoanNhapVienICD.TenTiengViet
                                                                                            : x.YeuCauNhapVien.ChanDoanNhapVienGhiChu)).Where(d => d != null);
            return chanDoan.LastOrDefault();
        }

        public async Task<string> GetNguoiThucHien(long nguoiDangLogin)
        {
            var getNguoiLogin = _userRepository.TableNoTracking.Where(e => e.Id == nguoiDangLogin)
                .Select(q => q.HoTen);
            return await getNguoiLogin.FirstOrDefaultAsync();
        }

        public Task<List<LookupItemVo>> GetLoaiHoSoDieuTriNoiTru(DropDownListRequestModel model)
        {
            var lstEnums = EnumHelper.GetListEnum<Enums.LoaiHoSoDieuTriNoiTru>().Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).OrderBy(o => o.DisplayName).ToList();

            if (!string.IsNullOrEmpty(model.Query))
            {
                lstEnums = lstEnums.Where(p => p.DisplayName != null && p.DisplayName.ToLower().ConvertToUnSign()
                                                   .Contains(model.Query.ToLower().ConvertToUnSign())).ToList();
            }
            return Task.FromResult(lstEnums);
        }
        public PhieuBienBanHoiChanPhauThuatGridVo GetThongTinPhieuBienBanHoiChanPhauThuat(long yeuCauTiepNhanId)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId && x.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.BienBanHoiChanPhauThuat)
                                                                  .Select(s => new PhieuBienBanHoiChanPhauThuatGridVo()
                                                                  {
                                                                      YeuCauTiepNhanId = yeuCauTiepNhanId,
                                                                      LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.PhieuDeNghiTestTruocKhiDungThuoc,
                                                                      ThongTinHoSo = s.ThongTinHoSo,
                                                                      NhanVienThucHienId = s.NhanVienThucHienId,
                                                                      ThoiDiemThucHien = s.ThoiDiemThucHien,
                                                                      NoiThucHienId = s.NoiThucHienId,
                                                                      Id = s.Id,
                                                                      ListFile = s.NoiTruHoSoKhacFileDinhKems.Select(k => new FileChuKyPhieuBienBanHoiChanPhauThuatGridVo()
                                                                      {
                                                                          Id = k.Id,
                                                                          DuongDan = k.DuongDan,
                                                                          KichThuoc = k.KichThuoc,
                                                                          LoaiTapTin = k.LoaiTapTin,
                                                                          Ma = k.Ma,
                                                                          MoTa = k.MoTa,
                                                                          Ten = k.Ten,
                                                                          TenGuid = k.TenGuid
                                                                      }).ToList()
                                                                  }).OrderByDescending(s => s.Id).FirstOrDefault();
            return query;
        }
        public PhieuBienBanHoiChanPhauThuatGridVo ViewNoiTruHoSoKhac(long noiTruHoSoKhacId)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(x => x.Id == noiTruHoSoKhacId)
                                                                  .Select(s => new PhieuBienBanHoiChanPhauThuatGridVo()
                                                                  {
                                                                      YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                                                                      LoaiHoSoDieuTriNoiTru = Enums.LoaiHoSoDieuTriNoiTru.PhieuDeNghiTestTruocKhiDungThuoc,
                                                                      ThongTinHoSo = s.ThongTinHoSo,
                                                                      NhanVienThucHienId = s.NhanVienThucHienId,
                                                                      ThoiDiemThucHien = s.ThoiDiemThucHien,
                                                                      NoiThucHienId = s.NoiThucHienId,
                                                                      Id = s.Id,
                                                                      ListFile = s.NoiTruHoSoKhacFileDinhKems.Select(k => new FileChuKyPhieuBienBanHoiChanPhauThuatGridVo()
                                                                      {
                                                                          Id = k.Id,
                                                                          DuongDan = k.DuongDan,
                                                                          KichThuoc = k.KichThuoc,
                                                                          LoaiTapTin = k.LoaiTapTin,
                                                                          Ma = k.Ma,
                                                                          MoTa = k.MoTa,
                                                                          Ten = k.Ten,
                                                                          TenGuid = k.TenGuid
                                                                      }).ToList()
                                                                  }).OrderBy(s => s.Id).FirstOrDefault();
            return query;
        }
        public List<DanhSachBienBanHoiChanPhauThuat> DanhSachBienBanHoiPhauThuat(long yeuCauTiepNhanId)
        {
            List<DanhSachBienBanHoiChanPhauThuat> bbhcpt = new List<DanhSachBienBanHoiChanPhauThuat>();
            var listDanhSachHoSoKhacTheoYeuCauTiepNhan = _noiTruHoSoKhacRepository.TableNoTracking.Where(s => s.YeuCauTiepNhanId == yeuCauTiepNhanId && s.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.BienBanHoiChanPhauThuat)
               .Select(z => new { ThongTinHoSo = z.ThongTinHoSo, IdNoiTruHoSoKhac = z.Id }).ToList();
            foreach (var item in listDanhSachHoSoKhacTheoYeuCauTiepNhan)
            {
                DanhSachBienBanHoiChanPhauThuat bbhcptObj = new DanhSachBienBanHoiChanPhauThuat();
                if (item.ThongTinHoSo != null)
                {
                    bbhcptObj.IdNoiTruHoSoKhac = item.IdNoiTruHoSoKhac;
                    var stringJson = JsonConvert.DeserializeObject<DanhSachBienBanHoiChanPhauThuatJson>(item.ThongTinHoSo);
                    bbhcptObj.ChanDoan = stringJson.ChanDoan;
                    DateTime ngayHoiChan = DateTime.Now;
                    DateTime.TryParseExact(stringJson.ThoiGianHoiChanUTC, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out ngayHoiChan);
                    bbhcptObj.NgayHoiChanDisplay = ngayHoiChan.ApplyFormatDateTime();
                    bbhcptObj.ThanhVienThamGiaDisplay = GetThanhVienThamGias(stringJson.ThanhVienThamGias);
                    bbhcpt.Add(bbhcptObj);
                }
            }
            return bbhcpt;
        }
        public Task<List<long>> HoSoKhacIds(long yctn, Enums.LoaiHoSoDieuTriNoiTru loaiNoiTru)
        {
            var query = _noiTruHoSoKhacRepository.TableNoTracking.Where(s => s.YeuCauTiepNhanId == yctn && s.LoaiHoSoDieuTriNoiTru == loaiNoiTru).Select(d => d.Id).ToListAsync();
            return query;
        }

        #region update biên bản hội chẩn phẩu thuật
        public async Task<List<LookupItemVo>> GetListThongTinThanhVienThamGia(DropDownListRequestModel model)
        {


            var lst = new List<LookupItemVo>();
            if (model.Id != 0 && model.Id != null)
            {
                var nhanViens = _nhanVienRepository.TableNoTracking.Include(v => v.HocHamHocVi).Include(v => v.VanBangChuyenMon).Include(v => v.User)
               .ApplyLike(model.Query, g => g.User.HoTen, g => g.HocHamHocVi.Ma, g => g.VanBangChuyenMon.Ma).Where(d => d.Id == model.Id)
               .Take(model.Take);
                lst = nhanViens.Select(d => new LookupItemVo()
                {
                    KeyId = d.Id,
                    DisplayName = DisplayName(d)
                }).ToList();
            }
            else
            {
                var nhanViens = _nhanVienRepository.TableNoTracking.Include(v => v.HocHamHocVi).Include(v => v.VanBangChuyenMon).Include(v => v.User)
               .ApplyLike(model.Query, g => g.User.HoTen, g => g.HocHamHocVi.Ma, g => g.VanBangChuyenMon.Ma)
               .Take(model.Take);
                if (nhanViens.Any())
                {
                    lst = nhanViens.Select(d => new LookupItemVo()
                    {
                        KeyId = d.Id,
                        DisplayName = DisplayName(d)
                    }).ToList();
                }
            }

            return lst;
        }
        public string DisplayName(Core.Domain.Entities.NhanViens.NhanVien nv)
        {
            var stringEmpty = string.Empty;
            if (nv.HocHamHocVi != null && nv.VanBangChuyenMon != null)
            {
                stringEmpty = nv.HocHamHocVi.Ma + " " + nv.VanBangChuyenMon.Ma + ". " + nv.User.HoTen;
            }
            else if (nv.HocHamHocVi != null && nv.VanBangChuyenMon == null)
            {
                stringEmpty = nv.HocHamHocVi.Ma + " " + nv.User.HoTen;
            }
            else if (nv.HocHamHocVi == null && nv.VanBangChuyenMon != null)
            {
                stringEmpty = nv.VanBangChuyenMon.Ma + ". " + nv.User.HoTen;
            }
            else
            {
                stringEmpty = nv.User.HoTen;
            }
            return stringEmpty;
        }
        #endregion
        public string ChanDoan(long idICD)
        {
            var cd = _icdRepository.TableNoTracking.Where(d => d.Id == idICD).Select(d => (d.Ma + "-" + d.TenTiengViet));
            return cd.FirstOrDefault();
        }
        public string BodyHtml(string title, string value)
        {
            var content = string.Empty;


            return content;
        }

        public async Task<string> PhieuInGiayThoaThuanLuaChonDichVuKhamTheoYeuCau(
           PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams)
        {
            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuInGiayThoaThuanLuaChonDichVuKhamTheoYeuCau"));
            var infoBn = await ThongTinBenhNhan(dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId);
            var result = await _yctnRepository.GetByIdAsync(dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId, s =>
                s.Include(x => x.BenhNhan).ThenInclude(x => x.NgheNghiep)

                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.KhoaPhongNhapVien)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruKhoaPhongDieuTris).ThenInclude(x => x.KhoaPhongChuyenDen)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruEkipDieuTris).ThenInclude(x => x.BacSi).ThenInclude(x => x.User)

                    .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(x => x.GiuongBenh).ThenInclude(x => x.PhongBenhVien)
                    .Include(x => x.DanToc)
                    .Include(x => x.QuocTich)
                    .Include(x => x.NgheNghiep)
            );
            infoBn.Khoa = result.NoiTruBenhAn != null ? result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Any()
                ? result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Last().KhoaPhongChuyenDen.Ten
                : string.Empty : string.Empty;
            var khoa = infoBn.Khoa;
            GiayThoaThuanLuaChonDichVuKhamTheoYeuCauVo giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo;
            var thongTinHoSo = await _noiTruHoSoKhacRepository.TableNoTracking
                .Where(bv => bv.YeuCauTiepNhanId == dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId &&
                             bv.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayThoaThuanLuaChonDichVuKhamChuabenhTheoYeuCau)
                .Select(p => p.ThongTinHoSo).FirstOrDefaultAsync();
            giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo = !string.IsNullOrEmpty(thongTinHoSo)
                ? JsonConvert.DeserializeObject<GiayThoaThuanLuaChonDichVuKhamTheoYeuCauVo>(thongTinHoSo)
                : new GiayThoaThuanLuaChonDichVuKhamTheoYeuCauVo();

            var hoTenNguoiThan = giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.TaoLaAi == 1 ? giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.HoTen : result.HoTen;
            var diaChinguoiThan = giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.TaoLaAi == 1 ? giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.DiaChi : result.DiaChiDayDu;
            var namSinh = giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.TaoLaAi == 1 ? giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.NamSinh : result.NamSinh;
            var gioiTinh = giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.TaoLaAi == 1 ? giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.GioiTinh != null ? giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.GioiTinh == 1 ? "Nam" : "Nữ" : string.Empty : result.GioiTinh != null ? result.GioiTinh.GetDescription() : "";
            var quanHe = giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.TaoLaAi == 1 ? giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.QuanHe : string.Empty;
            var hoTenBenhNhan = giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.TaoLaAi == 1 ? infoBn.HoTenNgBenh : result.HoTen;
            var nguoiVietCamKet = giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.TaoLaAi == 1 ? infoBn.HoTenNgBenh : result.HoTen;
            var chanDoan = "";
            var tmp = "\n";
            var replace = "<br>";
            if (giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.KhiCanBaoTin != null)
            {
                chanDoan = giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.KhiCanBaoTin.Replace(tmp, replace);
            }

            var giaiThich = "";
            if (giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.BacSiKham != null)
            {
                giaiThich = giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.BacSiKham.Replace(tmp, replace);
            }
            var soTien = giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.SoTien != null ? giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.SoTien.GetValueOrDefault().ApplyFormatMoneyVNDToDouble() : string.Empty;
            var soTienBangChu = giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.SoTienChu;
            var bsGmhs = giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.BsGmhs;
            var ngay = DateTime.Now.Day;
            var thang = DateTime.Now.Month;
            var nam = DateTime.Now.Year;
            var bsKham = string.Empty;
            if (giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.BacSiKhams.Count() > 0)
            {
                var nvs = _userRepository.TableNoTracking.Where(d => giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.BacSiKhams.Contains(d.Id)).Select(d => d.HoTen).ToList();

                if (nvs != null) {
                    bsKham = nvs.Join(";");
                }
            }

            var data = new DataInGiayThoaThuanLuaChonDichVuKhamTheoYeuCauVo();
            data.BarCodeImgBase64 = !string.IsNullOrEmpty(result.MaYeuCauTiepNhan.ToString()) ? BarcodeHelper.GenerateBarCode(result.MaYeuCauTiepNhan.ToString()) : "";


            data.Ten = !string.IsNullOrEmpty(hoTenBenhNhan) ? "<div class='container'><div class='label'>Tên tôi là:&nbsp;</div><div class='values'><b>" + hoTenBenhNhan + "</b></div></div>"
                : "<div class='container'><div class='label'>Tên tôi là:&nbsp;</div><div class='value'><b>" + hoTenBenhNhan + "</b></div></div>";
            var ns = DateHelper.DOBFormat(result.NgaySinh, result.ThangSinh, result.NamSinh);

            data.NamSinh = !string.IsNullOrEmpty(ns) ? "<div class='container'><div class='label'>Ngày/tháng/năm sinh:&nbsp;</div><div class='values'><b>" + ns + "</b></div></div>"
                : "<div class='container'><div class='label'>Ngày/tháng/năm sinh:&nbsp;</div><div class='value'><b>" + ns + "</b></div></div>";

            data.GioiTinh = !string.IsNullOrEmpty(gioiTinh) ? "<div class='container'><div class='label'>Giới tính:&nbsp;</div><div class='values'><b>" + gioiTinh + "</b></div></div>"
                : "<div class='container'><div class='label'>Giới tính:&nbsp;</div><div class='value'>" + gioiTinh + "</div></div>";

            data.CMND = !string.IsNullOrEmpty(result.SoChungMinhThu) ? "<div class='container'><div class='label'>CMND/CCCD/Hộ chiếu:&nbsp;</div><div class='values'><b>" + result.SoChungMinhThu + "</b></div></div>"
                : "<div class='container'><div class='label'>CMND/CCCD/Hộ chiếu:&nbsp;</div><div class='value'>" + result.SoChungMinhThu + "</div></div>";
            string coQuanCap = string.Empty;

            data.CoQuanCap = !string.IsNullOrEmpty(coQuanCap) ? "<div class='container'><div class='label'>Cơ quan cấp:&nbsp;</div><div class='values'>" + coQuanCap + "</div></div>"
                : "<div class='container'><div class='label'>Cơ quan cấp:&nbsp;</div><div class='value'><b>" + coQuanCap + "</b></div></div>";

            data.DanToc = !string.IsNullOrEmpty(result.DanToc.Ten) ? "<div class='container'><div class='label'>Dân tộc:&nbsp;</div><div class='values'>" + result.DanToc.Ten + "</div></div>"
                : "<div class='container'><div class='label'>Dân tộc:&nbsp;</div><div class='value'>" + result.DanToc.Ten + "</div></div>";

            data.QuocTich = !string.IsNullOrEmpty(result.QuocTich.Ten) ? "<div class='container'><div class='label'>Quốc tịch:&nbsp;</div><div class='values'>" + result.QuocTich.Ten + "</div></div>"
                : "<div class='container'><div class='label'>Quốc tịch:&nbsp;</div><div class='value'>" + result.QuocTich.Ten + "</div></div>";

            if (result.NgheNghiep != null)
            {
                data.NgheNghiep = !string.IsNullOrEmpty(result.NgheNghiep.Ten) ? "<div class='container'><div class='label'>Nghề nghiệp:&nbsp;</div><div class='values'>" + result.NgheNghiep.Ten + "</b></div></div>"
                : "<div class='container'><div class='label'>Nghề nghiệp:&nbsp;</div><div class='value'>" + result.NgheNghiep.Ten + "</div></div>";
            }
            else
            {
                var ngheNghiep = string.Empty;
                data.NgheNghiep = "<div class='container'><div class='label'>Nghề nghiệp:&nbsp;</div><div class='value'>" + ngheNghiep + "</div></div>";
            }


            data.NoiLamViec = !string.IsNullOrEmpty(result.NoiLamViec) ? "<div class='container'><div class='label'>Nơi làm việc:&nbsp;</div><div class='values'>" + result.NoiLamViec + "</div></div>"
                : "<div class='container'><div class='label'>Nơi làm việc:&nbsp;</div><div class='value'>" + result.NoiLamViec + "</div></div>";

            data.DiaChi = !string.IsNullOrEmpty(result.DiaChiDayDu) ? "<div class='container'><div class='label'>Địa chỉ:&nbsp;</div><div class='values'>" + result.DiaChiDayDu + "</div></div>"
                : "<div class='container'><div class='label'>Địa chỉ:&nbsp;</div><div class='value'>" + result.DiaChiDayDu + "</div></div>";

            data.KhiCanBaoTin = !string.IsNullOrEmpty(giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.KhiCanBaoTin) ? "<div class='container'><div class='label'>Khi cần báo tin:&nbsp;</div><div class='values'>" + giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.KhiCanBaoTin + "</div></div>"
                : "<div class='container'><div class='label'>Khi cần báo tin:&nbsp;</div><div class='value'>" + giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.KhiCanBaoTin + "</div></div>";

            data.NguoiDaiDien = !string.IsNullOrEmpty(hoTenNguoiThan) ? "<div class='container'><div class='label'>là người bệnh/đại diện gia đình người bệnh/họ tên là:&nbsp;</div><div class='values'>" + hoTenBenhNhan + "</div></div>"
                : "<div class='container'><div class='label'>là người bệnh/đại diện gia đình người bệnh/họ tên là:&nbsp;</div><div class='value'>" + hoTenNguoiThan + "</div></div>";

            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }
            var tenKhoa = _khoaPhongRepository.TableNoTracking.Where(s => s.Id == khoaId).Select(c => c.Ten).FirstOrDefault();

            data.Khoa = !string.IsNullOrEmpty(tenKhoa) ? "<div class='container'><div class='label'>hiện đang khám/điều trị tại khoa:&nbsp;</div><div class='values'>" + tenKhoa + " Bệnh viện đa khoa Quốc tế Bắc Hà." + "</div></div>"
                : "<div class='container'><div class='label'>hiện đang khám/điều trị tại khoa:&nbsp;</div><div class='value'>" + tenKhoa + " Bệnh viện đa khoa Quốc tế Bắc Hà." + "</div></div>"; ;

            data.BacSiKham = !string.IsNullOrEmpty(bsKham) ? bsKham
                : "........................";
            data.NamTaiBuongLoai = !string.IsNullOrEmpty(giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.NamTaiBuongLoai) ? giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.NamTaiBuongLoai
                : "........................................";
            data.SoTien = !string.IsNullOrEmpty(soTien) ? soTien
                : ".........................";

            var listKyTuCanSplit = new List<string>() { "(", ")" };
            if (!string.IsNullOrEmpty(soTienBangChu))
            {
                foreach (var item in listKyTuCanSplit)
                {
                    soTienBangChu = soTienBangChu.Replace(item, "");
                }
            }
            data.SoTienBangChu = !string.IsNullOrEmpty(soTienBangChu) ? ("<i>" + soTienBangChu + "</i>")
                : ".......................................";

            data.NgayThangNam = "ngày " + (giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.NgayThucHien.GetValueOrDefault().Day > 9 ? giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.NgayThucHien.GetValueOrDefault().Day + "" : "0" + giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.NgayThucHien.GetValueOrDefault().Day) +
                                 " tháng " + (giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.NgayThucHien.GetValueOrDefault().Month > 9 ? giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.NgayThucHien.GetValueOrDefault().Month + "" : "0" + giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.NgayThucHien.GetValueOrDefault().Month) +
                                 " năm " + giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.NgayThucHien.GetValueOrDefault().Year;
            data.BacSiDieuTriChuKy = giayThoaThuanLuaChonDichVuKhamTheoYeuCauVo.BsGmhs;
            data.NguoiDaiDienChuKy = hoTenNguoiThan;
            data.MaTN = "Mã TN:"+ result.MaYeuCauTiepNhan;
       
            data.KhoaDangIn ="<b>" + tenKhoa + "</b>";
            


            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }
        #region BVHD-3815
        public async Task<string> PhieuInGiayCamKetKyThuatMoiHS(
           PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams)
        {
            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("GiayCamKetTuNguyenThucHienKyThuatMoiTrongDieuTriGayMeHoiSuc"));
            var infoBn = await ThongTinBenhNhan(dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId);
            var result = await _yctnRepository.GetByIdAsync(dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId, s =>
                s.Include(x => x.BenhNhan).ThenInclude(x => x.NgheNghiep)

                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.KhoaPhongNhapVien)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruKhoaPhongDieuTris).ThenInclude(x => x.KhoaPhongChuyenDen)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruEkipDieuTris).ThenInclude(x => x.BacSi).ThenInclude(x => x.User)

                    .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(x => x.GiuongBenh).ThenInclude(x => x.PhongBenhVien)
            );
            infoBn.Khoa = result.NoiTruBenhAn != null ? result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Any()
                ? result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Last().KhoaPhongChuyenDen.Ten
                : string.Empty : string.Empty;
            var khoa = infoBn.Khoa;
            GiayCamKetKyThuatMoiHSVo giayCamKetKyThuatMoi;
            var thongTinHoSo = await _noiTruHoSoKhacRepository.TableNoTracking
                .Where(bv => bv.YeuCauTiepNhanId == dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId &&
                             bv.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayCamKetKyThuatMoiHS)
                .Select(p => p.ThongTinHoSo).FirstOrDefaultAsync();
            giayCamKetKyThuatMoi = !string.IsNullOrEmpty(thongTinHoSo)
                ? JsonConvert.DeserializeObject<GiayCamKetKyThuatMoiHSVo>(thongTinHoSo)
                : new GiayCamKetKyThuatMoiHSVo();
            var hoTenNguoiThan = giayCamKetKyThuatMoi.TaoLaAi == 1 ? giayCamKetKyThuatMoi.HoTen : result.HoTen;
            var diaChinguoiThan = giayCamKetKyThuatMoi.TaoLaAi == 1 ? giayCamKetKyThuatMoi.DiaChi : result.DiaChiDayDu;
            var namSinh = giayCamKetKyThuatMoi.TaoLaAi == 1 ? giayCamKetKyThuatMoi.NamSinh : result.NamSinh;
            var gioiTinh = giayCamKetKyThuatMoi.TaoLaAi == 1 ? giayCamKetKyThuatMoi.GioiTinh != null ? giayCamKetKyThuatMoi.GioiTinh == 1 ? "Nam" : "Nữ" : string.Empty : result.GioiTinh != null ? result.GioiTinh.GetDescription() : "";
            var quanHe = giayCamKetKyThuatMoi.TaoLaAi == 1 ? giayCamKetKyThuatMoi.QuanHe : string.Empty;
            var hoTenBenhNhan = giayCamKetKyThuatMoi.TaoLaAi == 1 ? infoBn.HoTenNgBenh : result.HoTen;
            var nguoiVietCamKet = giayCamKetKyThuatMoi.TaoLaAi == 1 ? infoBn.HoTenNgBenh : result.HoTen;
            var chanDoan = "";
            var tmp = "\n";
            var replace = "<br>";
            if (giayCamKetKyThuatMoi.ChanDoan != null)
            {
                chanDoan = giayCamKetKyThuatMoi.ChanDoan.Replace(tmp, replace);
            }


            var soTien = giayCamKetKyThuatMoi.SoTien != null ? giayCamKetKyThuatMoi.SoTien.GetValueOrDefault().ApplyFormatMoneyVNDToDouble() : string.Empty;
            var soTienBangChu = giayCamKetKyThuatMoi.SoTienChu;
            var bsGmhs = giayCamKetKyThuatMoi.BacSyThucHien;
            var ngay = DateTime.Now.Day;
            var thang = DateTime.Now.Month;
            var nam = DateTime.Now.Year;


            var ngayThangNamSinh = string.Empty;
            ngayThangNamSinh = DateHelper.DOBFormat(result.NgaySinh, result.ThangSinh, result.NamSinh);

            var data = new DataInGiayCamKetKyThuatMoiHSVo();
            data.BarCodeImgBase64 = !string.IsNullOrEmpty(result.MaYeuCauTiepNhan) ? BarcodeHelper.GenerateBarCode(result.MaYeuCauTiepNhan) : "";
            data.MaTN = result.MaYeuCauTiepNhan;

            data.TenToiLa = !string.IsNullOrEmpty(hoTenBenhNhan) ? "<div class='container'><div class='label'>Tên tôi là:&nbsp;</div><div class='values'><b>" + hoTenBenhNhan + "</b></div></div>"
                : "<div class='container'><div class='label'>Tên tôi là:&nbsp;</div><div class='value'>" + hoTenBenhNhan + "</div></div>";

            data.NamSinh = !string.IsNullOrEmpty(ngayThangNamSinh) ? "<div class='container'><div class='label'>Năm sinh:&nbsp;</div><div class='values'><b>" + ngayThangNamSinh + "</b></div></div>"
                : "<div class='container'><div class='label'>Năm sinh:&nbsp;</div><div class='value'>" + ngayThangNamSinh + "</div></div>";

            data.GioiTinh = !string.IsNullOrEmpty(gioiTinh) ? "<div class='container'><div class='label'>Giới:&nbsp;</div><div class='values'><b>" + gioiTinh + "</b></div></div>"
                : "<div class='container'><div class='label'>Giới:&nbsp;</div><div class='value'>" + gioiTinh + "</div></div>";
            data.DiaChi = !string.IsNullOrEmpty(result.DiaChiDayDu) ? "<div class='container'><div class='label'>Địa chỉ:&nbsp;</div><div class='values'>" + result.DiaChiDayDu + "</div></div>"
               : "<div class='container'><div class='label'>Địa chỉ:&nbsp;</div><div class='value'>" + result.DiaChiDayDu + "</div></div>";
            data.ChanDoan = !string.IsNullOrEmpty(chanDoan) ? "<div class='container'><div class='label'>Chẩn đoán:&nbsp;</div><div class='values'>" + chanDoan + "</div></div>"
               : "<div class='container'><div class='label'>Chẩn đoán:&nbsp;</div><div class='value'>" + chanDoan + "</div></div>";
            data.NguoiThan = !string.IsNullOrEmpty(hoTenNguoiThan) ? "<div class='container'><div class='label'>là người bệnh/đại diện gia đình người bệnh/họ tên là:&nbsp;</div><div class='values'>" + hoTenNguoiThan + "</div></div>"
                : "<div class='container'><div class='label'>là người bệnh/đại diện gia đình người bệnh/họ tên là:&nbsp;</div><div class='value'>" + hoTenNguoiThan + "</div></div>";
            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }
            var tenKhoa = _khoaPhongRepository.TableNoTracking.Where(s => s.Id == khoaId).Select(c => c.Ten).FirstOrDefault();
            data.Khoa = !string.IsNullOrEmpty(tenKhoa) ? "<div class='values'>" + tenKhoa + "&nbsp;Bệnh viện Đa khoa Quốc tế Bắc Hà." + "</div>"
                : "<div class='value'>" + ".........................." + "</div>" + "<div class='values'>" + "&nbsp;bệnh viện Đa khoa Quốc tế Bắc Hà." + "</div>";

            data.KhoaCreate = !string.IsNullOrEmpty(tenKhoa) ? tenKhoa.Replace("Khoa", "") : "...........";

            data.SoTien = !string.IsNullOrEmpty(soTien) ? "&nbsp;" + soTien
               : ".............................";
            var listKyTuCanSplit = new List<string>() { "(", ")" };
            if (!string.IsNullOrEmpty(soTienBangChu))
            {
                foreach (var item in listKyTuCanSplit)
                {
                    soTienBangChu = soTienBangChu.Replace(item, "");
                }
            }
            data.SoTienBangChu = !string.IsNullOrEmpty(soTienBangChu) ? "&nbsp;<i>" + soTienBangChu + "</i>"
              : ".........................................................";
            data.BacSyThucHien = bsGmhs;
            data.NguoiDaiDien = hoTenNguoiThan;
            data.NgayThangNam = "ngày " + (giayCamKetKyThuatMoi.NgayThucHien.GetValueOrDefault().Day > 9 ? giayCamKetKyThuatMoi.NgayThucHien.GetValueOrDefault().Day + "" : "0" + giayCamKetKyThuatMoi.NgayThucHien.GetValueOrDefault().Day) +
                                 " tháng " + (giayCamKetKyThuatMoi.NgayThucHien.GetValueOrDefault().Month > 9 ? giayCamKetKyThuatMoi.NgayThucHien.GetValueOrDefault().Month + "" : "0" + giayCamKetKyThuatMoi.NgayThucHien.GetValueOrDefault().Month) +
                                 " năm " + giayCamKetKyThuatMoi.NgayThucHien.GetValueOrDefault().Year;

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }
        #endregion
        public async Task<List<LookupItemVo>> GetListNhanViens(DropDownListRequestModel model)
        {
            var nvs =
               _userRepository.TableNoTracking
                 .ApplyLike(model.Query, g => g.HoTen)
                  .Take(model.Take)
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.HoTen,
                    KeyId = i.Id
                });
            return await nvs.ToListAsync();
        }
        #region BVHD 3874 giấy phản ứng thuôc
        public async Task<List<ThongTinHoSoGetInfo>> GetDSNoiTruHoSoKhac(long yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru loaiHoSo)
        {
            var thongTinHoSo = BaseRepository.TableNoTracking
                .Where(q => q.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                            q.LoaiHoSoDieuTriNoiTru == loaiHoSo)
                .Select(q => new ThongTinHoSoGetInfo
                {
                    Id = q.Id,
                    ThongTinHoSo = q.ThongTinHoSo
                });
            return await thongTinHoSo.ToListAsync();
        }

        public async Task<ThongTinHoSoGetInfo> GetNoiTruHoSoKhacId(long noiTruHoSoKhacId, Enums.LoaiHoSoDieuTriNoiTru loaiHoSo)
        {
            var thongTinHoSo = BaseRepository.TableNoTracking
                .Where(q => q.Id == noiTruHoSoKhacId &&
                            q.LoaiHoSoDieuTriNoiTru == loaiHoSo)
                .Select(q => new ThongTinHoSoGetInfo
                {
                    Id = q.Id,
                    ThongTinHoSo = q.ThongTinHoSo
                });
            return await thongTinHoSo.LastOrDefaultAsync();
        }
        public async Task<List<DuocPhamTheoKhoThuocTuTrucTheoBenhNhanTemplateVo>> GetListDuocPhamTheoTuTrucKhoaPhongBenhNhanDangNamNoiTru(DropDownListRequestModel model)
        {
            // lấy tất cả các kho thuộc khoa phòng bệnh người bệnh đang nằm nội trú
            // to do
            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }

            //lấy theo kho thuốc tủ trực khoa phòng NB nằm nội trú

            var khoIds = _khoaPhongRepository.TableNoTracking.Where(d => d.Id == khoaId)
                .SelectMany(d => d.KhoDuocPhams).Where(d => d.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe && d.LoaiDuocPham == true)
                .Select(d => d.Id).ToList();




            // duoc phẩm còn hsd , số lấy tất cả dược phẩm đang có số lượng xuất nhỏ hơn số lượng nhập
            var nhapKhoDuocPhamChiTiets = await _nhapkhoDuocPhamChiTietRepo.TableNoTracking
                                                       .Where(nkct => nkct.HanSuDung >= DateTime.Now &&
                                                                       nkct.SoLuongNhap > nkct.SoLuongDaXuat &&
                                                                       khoIds.Contains(nkct.NhapKhoDuocPhams.KhoId))
                                                       .Include(d => d.DuocPhamBenhViens).ThenInclude(d => d.DuocPham)
                                                       .ApplyLike(model.Query, o => o.DuocPhamBenhViens.DuocPham.Ten)
                                                       .Take(model.Take)
                                                       .ToListAsync();


            var query = nhapKhoDuocPhamChiTiets.Select(d => new DuocPhamTheoKhoThuocTuTrucTheoBenhNhanTemplateVo
            {
                DuocPhamBenhVienId = d.DuocPhamBenhVienId,
                SoLoSX = d.Solo,
                Ten = d.DuocPhamBenhViens.DuocPham.Ten,
                NuocSX = d.DuocPhamBenhViens.DuocPham.NuocSanXuat,
                KeyId = d.Id,
                HamLuong = d.DuocPhamBenhViens.DuocPham.HamLuong
            }).GroupBy(d => d.SoLoSX).Select(item => new DuocPhamTheoKhoThuocTuTrucTheoBenhNhanTemplateVo {
                DuocPhamBenhVienId = item.First().DuocPhamBenhVienId,
                SoLoSX = item.First().SoLoSX,
                Ten = item.First().Ten,
                NuocSX = item.First().NuocSX,
                KeyId = item.First().KeyId,
                HamLuong = item.First().HamLuong
            }).ToList();
            return query;
        }


        public async Task<string> PhieuINPhanUngThuoc(
         PhanUngThuocVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams)
        {
            var template = _templateRepository.TableNoTracking.First(x => x.Name.Equals("PhieuPhanUngThuoc"));
            var infoBn = await ThongTinBenhNhan(dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId);

            var result = await _yctnRepository.GetByIdAsync(dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId, s =>
                s.Include(x => x.BenhNhan).ThenInclude(x => x.NgheNghiep)

                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruPhieuDieuTris)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.KhoaPhongNhapVien)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruKhoaPhongDieuTris).ThenInclude(x => x.KhoaPhongChuyenDen)
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.NoiTruEkipDieuTris).ThenInclude(x => x.BacSi).ThenInclude(x => x.User)

                    .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(x => x.GiuongBenh).ThenInclude(x => x.PhongBenhVien)
            );

            infoBn.Khoa = result.NoiTruBenhAn != null ? result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Any()
                ? result.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.Last().KhoaPhongChuyenDen.Ten
                : string.Empty : string.Empty;
            var khoa = infoBn.Khoa;
            GiayPhanUngThuocVO giayPhanUngThuocVO;
            var thongTinHoSo = await _noiTruHoSoKhacRepository.TableNoTracking
                .Where(bv => bv.YeuCauTiepNhanId == dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId &&
                             bv.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayPhanUngThuoc && bv.Id == dieuTriNoiTruVaServicesHttpParams.NoiTruHoSoKhacId)
                .Select(p => p.ThongTinHoSo).FirstOrDefaultAsync();
            giayPhanUngThuocVO = !string.IsNullOrEmpty(thongTinHoSo)
                ? JsonConvert.DeserializeObject<GiayPhanUngThuocVO>(thongTinHoSo)
                : new GiayPhanUngThuocVO();

            var hoTenNguoiThan = giayPhanUngThuocVO.TaoLaAi == 1 ? giayPhanUngThuocVO.HoTen : result.HoTen;

            var diaChinguoiThan = giayPhanUngThuocVO.TaoLaAi == 1 ? giayPhanUngThuocVO.DiaChi : result.DiaChiDayDu;

            var namSinh = giayPhanUngThuocVO.TaoLaAi == 1 ? giayPhanUngThuocVO.NamSinh : result.NamSinh;

            var gioiTinh = giayPhanUngThuocVO.TaoLaAi == 1 ? giayPhanUngThuocVO.GioiTinh != null ? giayPhanUngThuocVO.GioiTinh == 1 ? "Nam" : "Nữ" : string.Empty : result.GioiTinh != null ? result.GioiTinh.GetDescription() : "";

            var hoTenBenhNhan = giayPhanUngThuocVO.TaoLaAi == 1 ? infoBn.HoTenNgBenh : result.HoTen;
            var nguoiVietCamKet = giayPhanUngThuocVO.TaoLaAi == 1 ? infoBn.HoTenNgBenh : result.HoTen;

            var chanDoan = "";
            var tmp = "\n";
            var replace = "<br>";
            if (giayPhanUngThuocVO.ChanDoan != null)
            {
                chanDoan = giayPhanUngThuocVO.ChanDoan.Replace(tmp, replace);
            }

            var data = new GiayPhaUngThuocInVo();


            // khoa
            long khoaId = 0;
            var phongBenhVien = _phongBenhVienRepository.Table.FirstOrDefault(o => o.Id == _userAgentHelper.GetCurrentNoiLLamViecId());
            if (phongBenhVien != null)
            {
                khoaId = phongBenhVien.KhoaPhongId;
            }
            var tenKhoa = _khoaPhongRepository.TableNoTracking.Where(s => s.Id == khoaId).Select(c => c.Ten).FirstOrDefault();
            var tenKhoaIn = string.Empty;
            if (!string.IsNullOrEmpty(tenKhoa))
            {
                tenKhoaIn = tenKhoa.Replace("Khoa", "");
            }
            data.KhoaCreate = "<b>Khoa</b>" + (!string.IsNullOrEmpty(tenKhoaIn) ? tenKhoaIn : "...............");

            //MaTN
            data.MaTN = infoBn.MaSoTiepNhan;

            //BarCodeImgBase64
            data.BarCodeImgBase64 = !string.IsNullOrEmpty(infoBn.MaSoTiepNhan) ? BarcodeHelper.GenerateBarCode(infoBn.MaSoTiepNhan) : "";

            //TenToiLa
            data.TenToiLa = !string.IsNullOrEmpty(infoBn.HoTenNgBenh) ? $"- Họ tên: <b>{infoBn.HoTenNgBenh}</b>" : ".....................................................................................";
            //Nam sinh
            var ngayThangNamSinh = string.Empty;
            ngayThangNamSinh = DateHelper.DOBFormat(result.NgaySinh, result.ThangSinh, result.NamSinh);

            data.NamSinh = !string.IsNullOrEmpty(ngayThangNamSinh) ? $"- Ngày/tháng/năm sinh: <b>{ngayThangNamSinh}</b>" : "..............";

            // GioiTinh
            data.GioiTinh = !string.IsNullOrEmpty(infoBn.GioiTinh?.GetDescription()) ? $"– Giới tính: <b>{infoBn.GioiTinh?.GetDescription()}</b>" : "..............";

            // So giuong 
            var giuong = string.Empty;
            if (infoBn.Giuong != null)
            {
                giuong = infoBn.Giuong.Replace("Giường", "");
                giuong = giuong.Replace("giường", "");
                giuong = giuong.Replace("GIƯỜNG", "");
            }
            data.SoGiuong = !string.IsNullOrEmpty(giuong) ? $"- Số giường: {giuong}" : "<div class='container'>" +
                                                                                             "<div class='label'>- Số giường: </div>" +
                                                                                             $"<div class='value'>{giuong}</div>" +
                                                                                             "</div>";

            // buong
            var buong = string.Empty;
            if (infoBn.Buong != null)
            {
                buong = infoBn.Buong.Replace("Buồng", "");
                buong = buong.Replace("buồng", "");
            }
            data.SoBuong = !string.IsNullOrEmpty(buong) ? $"- Số buồng: {buong}" : "<div class='container'>" +
                                                                                             "<div class='label'>- Số buồng: </div>" +
                                                                                             $"<div class='value'>{buong}</div>" +
                                                                                             "</div>";

            //chẩn đoán
            data.ChanDoan = !string.IsNullOrEmpty(chanDoan) ? $"- Chẩn đoán: {chanDoan}" : "<div class='container'>" +
                                                                                             "<div class='label'>- Chẩn đoán: </div>" +
                                                                                             $"<div class='value'>{chanDoan}</div>" +
                                                                                             "</div>";

            //bs chỉ định
            data.BSChinhDinh = !string.IsNullOrEmpty(giayPhanUngThuocVO.BSChiDinh) ? $"- Bác sỹ chỉ định: {giayPhanUngThuocVO.BSChiDinh}" :  "<div class='container'>" +
                                                                                                                                             "<div class='label'>- Bác sỹ chỉ định: </div>" +
                                                                                                                                             $"<div class='value'>{giayPhanUngThuocVO.BSChiDinh}</div>" +
                                                                                                                                             "</div>";
           

                  //TenThuoc
            data.TenThuoc = !string.IsNullOrEmpty(giayPhanUngThuocVO.TenThuocText) ? $"<div style='padding-top: 11px;'><b>&nbsp;&nbsp;&nbsp;TÊN THUỐC: {giayPhanUngThuocVO.TenThuocText}</b></div>" 
                : "<br>" +
                  "<div class='container' style='padding-top: 11px;'>" +
                  "<div class='label'><b>&nbsp;&nbsp;&nbsp;TÊN THUỐC:</b> </div>" +
                   "<div class='value'></div>" +
                   "</div>";

            //NuocSX
            data.NuocSX = !string.IsNullOrEmpty(giayPhanUngThuocVO.NuocSX) ? $"<div style='padding-top: 11px;'>&nbsp;&nbsp;&nbsp;Thuốc sản xuất tại nước: {giayPhanUngThuocVO.NuocSX}</div>"
                : "<br>" +
                  "<div class='container' style='padding-top: 11px;'>" +
                  "<div class='label'>&nbsp;&nbsp;&nbsp;Thuốc sản xuất tại nước:</div>" +
                   "<div class='value'></div>" +
                   "</div>";

            //SoLo
            data.SoLo = !string.IsNullOrEmpty(giayPhanUngThuocVO.SoLo) ? $"<div style='padding-top: 11px;'>&nbsp;&nbsp;&nbsp;Số lô sản xuất (in trên nhãn lọ): {giayPhanUngThuocVO.SoLo}</div>"
                : "<br>" +
                  "<div class='container' style='padding-top: 11px;'>" +
                  "<div class='label'>&nbsp;&nbsp;&nbsp;Số lô sản xuất (in trên nhãn lọ): </div>" +
                   "<div class='value'></div>" +
                   "</div>";

            // Phản ứng 1
            data.KetQuaPhanUng1 = giayPhanUngThuocVO.KetQuaId == 1 ? KetQuaPhanUngThuocNoiTruHoSoKhac.AmTinh.GetDescription() : "";

            // Phản ứng2
            data.KetQuaPhanUng2 = giayPhanUngThuocVO.KetQuaId == 2 ? KetQuaPhanUngThuocNoiTruHoSoKhac.DuongTinh.GetDescription() : "";

            // Phản ứng 3
            data.KetQuaPhanUng3 = giayPhanUngThuocVO.KetQuaId == 3 ? KetQuaPhanUngThuocNoiTruHoSoKhac.NghiNgo.GetDescription() : "";


            ////NgayGioThangnamCamKet
            //var thoiDiemLapPhieu = _noiTruHoSoKhacRepository.TableNoTracking
            //  .Where(bv => bv.YeuCauTiepNhanId == dieuTriNoiTruVaServicesHttpParams.YeuCauTiepNhanId &&
            //               bv.LoaiHoSoDieuTriNoiTru == Enums.LoaiHoSoDieuTriNoiTru.GiayPhanUngThuoc && 
            //               bv.Id == dieuTriNoiTruVaServicesHttpParams.NoiTruHoSoKhacId
            //               ).Select(d => d.ThoiDiemThucHien).FirstOrDefault();
        
            // data.NgayGioThangnamCamKet = "Giờ &nbsp;" + thoiDiemLapPhieu.ApplyFormatTime() + "&nbsp;ngày " + (thoiDiemLapPhieu.Day > 9 ? thoiDiemLapPhieu.Day + "" : "0" + thoiDiemLapPhieu.Day) +
            //                     " tháng " + (thoiDiemLapPhieu.Month > 9 ? thoiDiemLapPhieu.Month + "" : "0" + thoiDiemLapPhieu.Month) +
            //                     " năm " + thoiDiemLapPhieu.Year;
            //NguoiViet
            if(giayPhanUngThuocVO.TaoLaAi != null)
            {
                data.NguoiViet = giayPhanUngThuocVO.TaoLaAi == 1 ? giayPhanUngThuocVO.HoTen : infoBn.HoTenNgBenh;
            }
            

            data.DieuDuongPhanUng = !string.IsNullOrEmpty(giayPhanUngThuocVO.DieuDuongThucHienText) ?
                "<div class='container' >" +
                   $"<div class='label'>Họ tên: {giayPhanUngThuocVO.DieuDuongThucHienText}</div>" +
                   "</div>"
                   :
                "<div class='container' >" +
                  "<div class='label'>Họ tên: </div>" +
                   $"<div class='value'>{giayPhanUngThuocVO.DieuDuongThucHienText}</div>" +
                   "</div>";

            var cd = string.Empty;
            if(giayPhanUngThuocVO.DieuDuongThucHienId != null)
            {
                cd = _nhanVienRepository.TableNoTracking.Where(d => d.Id == giayPhanUngThuocVO.DieuDuongThucHienId).Select(d => d.ChucDanh.Ten).FirstOrDefault();
            }
            data.ChucDanhDieuDuongPhanUng = !string.IsNullOrEmpty(cd) ? "<div class='container' >" +
                  $"<div class='label'>Chức danh:&nbsp;{cd} </div>" +
                   "</div>"
                   :
                "<div class='container' >" +
                  "<div class='label'>Chức danh:</div>" +
                   $"<div class='value'>&nbsp;{cd}</div>" +
                   "</div>";

            var cdBS = string.Empty;
            if (giayPhanUngThuocVO.BSDocPhanUngId != null)
            {
                cdBS = _nhanVienRepository.TableNoTracking.Where(d => d.Id == giayPhanUngThuocVO.BSDocPhanUngId).Select(d => d.ChucDanh.Ten).FirstOrDefault();
            }
            data.ChucDanhBSDocPhanUng = !string.IsNullOrEmpty(cdBS) ? "<div class='container' >" +
                    $"<div class='label'>Chức danh:&nbsp;{cdBS} </div>" +
                   "</div>"
                   :
                "<div class='container' >" +
                  "<div class='label'>Chức danh:&nbsp;</div>" +
                   $"<div class='value'>{cdBS}</div>" +
                   "</div>";


            if (!string.IsNullOrEmpty(giayPhanUngThuocVO.ThoiGianThuPhanUngString))
            {
                DateTime thoigian = DateTime.Now;
                DateTime.TryParseExact(giayPhanUngThuocVO.ThoiGianThuPhanUngString, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out thoigian);
                
                data.NgayGioDieuDuongPhanUngThu = "Giờ " + thoigian.ApplyFormatTime()  + " &nbsp;ngày " + (thoigian.Day > 9 ? thoigian.Day + "" : "0" + thoigian.Day) +
                                  " tháng " + (thoigian.Month > 9 ? thoigian.Month + "" : "0" + thoigian.Month) +
                                  " năm " + thoigian.Year;
            }
            else
            {
                data.NgayGioDieuDuongPhanUngThu = "Giờ " + "&nbsp;&nbsp;" + " &nbsp;ngày " +
                                  " tháng " + "&nbsp;&nbsp;" +
                                  " năm " + "&nbsp;&nbsp;";
            }

            data.DieuDuongThuPhanUng = giayPhanUngThuocVO.DieuDuongThucHienText;



            if (!string.IsNullOrEmpty(giayPhanUngThuocVO.ThoiGianDocPhanUngString))
            {
                DateTime thoigian = DateTime.Now;
                DateTime.TryParseExact(giayPhanUngThuocVO.ThoiGianDocPhanUngString, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out thoigian);

                data.NgayGioBSDocPhanUng = "Giờ " + thoigian.ApplyFormatTime() + " &nbsp;ngày " + (thoigian.Day > 9 ? thoigian.Day + "" : "0" + thoigian.Day) +
                                  " tháng " + (thoigian.Month > 9 ? thoigian.Month + "" : "0" + thoigian.Month) +
                                  " năm " + thoigian.Year;
            }
            else
            {
                data.NgayGioBSDocPhanUng = "Giờ " + "&nbsp;&nbsp;" + " &nbsp;ngày " +
                                  " tháng " + "&nbsp;&nbsp;" +
                                  " năm " + "&nbsp;&nbsp;";
            }


            if (!string.IsNullOrEmpty(giayPhanUngThuocVO.ThoiGianLamPhieuString))
            {
                DateTime thoigian = DateTime.Now;
                DateTime.TryParseExact(giayPhanUngThuocVO.ThoiGianLamPhieuString, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out thoigian);

                data.NgayGioThangnamCamKet = "Giờ " + thoigian.ApplyFormatTime() + " &nbsp;ngày " + (thoigian.Day > 9 ? thoigian.Day + "" : "0" + thoigian.Day) +
                                  " tháng " + (thoigian.Month > 9 ? thoigian.Month + "" : "0" + thoigian.Month) +
                                  " năm " + thoigian.Year;
            }
            else
            {
                data.NgayGioThangnamCamKet = "Giờ " + "&nbsp;&nbsp;" + " &nbsp;ngày " +
                                  " tháng " + "&nbsp;&nbsp;" +
                                  " năm " + "&nbsp;&nbsp;";
            }

            data.HoTenBSDocPhanUng =!string.IsNullOrEmpty(giayPhanUngThuocVO.BSDocPhanUngText) ?
                 "<div class='container' >" +
                  $"<div class='label'>Họ tên: {giayPhanUngThuocVO.BSDocPhanUngText}</div>" +
                   "</div>"
                   :
                "<div class='container' >" +
                  "<div class='label'>Họ tên: </div>" +
                   $"<div class='value'>{giayPhanUngThuocVO.BSDocPhanUngText}</div>" +
                   "</div>"; ;
            data.BacSiDocPhanUng = giayPhanUngThuocVO.BSDocPhanUngText;


            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);
            return content;
        }

        #endregion
    }
}
