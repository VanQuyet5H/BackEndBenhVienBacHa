using Camino.Api.Auth;
using Camino.Api.Models.KhamBenh;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Extensions;
using static Camino.Core.Domain.Enums;
using System;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Domain.ValueObject.Grid;
using System.Net;
using Camino.Api.Models.Error;
using Camino.Api.Models.KhamBenh.ViewModelCheckValidators;
using Camino.Core.Helpers;
using Camino.Api.Models.YeuCauKhamBenh;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.GiuongBenhs;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Domain.ValueObject.QuayThuoc;
using Camino.Services.Helpers;

namespace Camino.Api.Controllers
{
    public partial class KhamBenhController
    {
        #region Get data grip
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChiTietAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, DocumentType.KhamDoanKhamBenh)]
        public ActionResult<GridDataSource> GetDataForGridChiTietAsync([FromBody]QueryInfo queryInfo)
        {
            // cần kiểm tra lại
            var gridData = _khamBenhService.GetDataForGridChiTietAsync(queryInfo, 15, 141);

            // hàm này có vẻ ko dùng tới
            throw new ApiException("KhamBenh/GetDataForGridChiTietAsync");
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChiTietAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, DocumentType.KhamDoanKhamBenh)]
        public ActionResult<GridDataSource> GetTotalPageForGridChiTietAsync([FromBody]QueryInfo queryInfo)
        {
            return null;
        }

        [HttpPost("GetObject")]
        public ActionResult GetObject([FromBody] GoiDichVuChiDinhGridVo model)
        {
            return Ok(model);
        }

        [HttpPost("GetDataDefaulDichVuKhac")]
        public async Task<List<KhamBenhGoiDichVuGridVo>> GetDataDefaulDichVuKhac(GridChiDinhDichVuQueryInfoVo queryInfo)
        {
            //var gridData = await _khamBenhService.GetDichVuKhacByTiepNhanBenhNhan(queryInfo);
            var gridData = await _khamBenhService.GetDichVuKhacByTiepNhanBenhNhanVer2(queryInfo);
            return gridData;
        }

        [HttpGet("GetDataDefaulGoiChietKhau")]
        public List<GoiDichVuChiDinhGridVo> GetDataDefaulGoiChietKhau(long yeuCauTiepNhanId) // hiện tại trong chỉ định chỉ có gói ko chiết khấu, hàm này cần xem xét bỏ
        {
            var gridData = _khamBenhService.GetGoiDichChietKhau(yeuCauTiepNhanId, true);
            return gridData;
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetChiDinhCuaBacSiKhacDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<GridDataSource>> GetChiDinhCuaBacSiKhacDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamBenhService.GetChiDinhCuaBacSiKhacDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetChiDinhBacSiKhacTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<GridDataSource>> GetChiDinhBacSiKhacTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamBenhService.GetChiDinhBacSiKhacTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }


        #endregion

        #region Them Chi Dinh

        // TODO: cần update
        [HttpPost("ThemGoiKhongChietKhau")]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> ThemGoiKhongChietKhau([FromBody] YeuCauThemGoi yeuCauViewModel)
        {
            await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(yeuCauViewModel.YeuCauKhamBenhId ?? 0);

            if (!yeuCauViewModel.DichVuChiDinhTheoGois.Any())
            {
                throw new ApiException(_localizationService.GetResource("ycdvcls.DichVuChiDinhTheoGois.Required"), (int)HttpStatusCode.BadRequest);
            }

            var lstDichVuDuocChonTrongGoi = new List<DichVuChiDinhTheoGoiViewModel>();
            foreach (var item in yeuCauViewModel.DichVuChiDinhTheoGois)
            {
                lstDichVuDuocChonTrongGoi.Add(JsonConvert.DeserializeObject<DichVuChiDinhTheoGoiViewModel>(item));
            }

            var gridData = _khamBenhService.GetGoiDichVuKhamBenhKhac(yeuCauViewModel.GoiDichVuId ?? 0, false);

            var countNewDvKT = 0;
            var countNewDVG = 0;
            // bổ sung nơi thực hiện
            if (gridData.Any() && lstDichVuDuocChonTrongGoi.Any())
            {
                var lstGoiChiTiets = gridData.SelectMany(x => x.GoiChietKhaus).ToList();
                var lstDichVu = new List<KhamBenhGoiDichVuGridVo>();

                foreach (var goiChiTiet in lstGoiChiTiets)
                {
                    switch (goiChiTiet.NhomId)
                    {
                        case (int)EnumNhomGoiDichVu.DichVuKyThuat:
                            if (lstDichVuDuocChonTrongGoi.Any(x => x.NhomDichVu == goiChiTiet.NhomId && x.Id == goiChiTiet.Id.Value))
                            {
                                var dvkt = _dichVuKyThuatBenhVienService.GetById(goiChiTiet.LoaiYeuCauDichVuId ?? 0, x => x.Include(o => o.DichVuKyThuatBenhVienGiaBaoHiems));
                                var dvktGiaBH = dvkt.DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(o => o.TuNgay <= DateTime.Now && (o.DenNgay == null || DateTime.Now <= o.DenNgay.Value));
                                //goiChiTiet.DonGiaBaoHiem = dvktGiaBH == null ? 0 : dvktGiaBH.Gia * (decimal)dvktGiaBH.TiLeBaoHiemThanhToan / 100;

                                if (dvktGiaBH != null)
                                {
                                    goiChiTiet.DonGiaBaoHiem = dvktGiaBH.Gia;
                                    goiChiTiet.TiLeBaoHiemThanhToan = dvktGiaBH.TiLeBaoHiemThanhToan;
                                }

                                // get nơi thực hiện mặc định
                                var query = new DropDownListRequestModel();
                                query.Take = 1;
                                query.ParameterDependencies = string.Format("{{DichVuId:{0}}}", goiChiTiet.LoaiYeuCauDichVuId ?? 0); //goiChiTiet.LoaiYeuCauDichVuId.Value;
                                var lstNoiThucHien = await _khamBenhService.GetPhongThucHienChiDinhKhamOrDichVuKyThuat(query);
                                if (lstNoiThucHien.Any())
                                {
                                    goiChiTiet.NoiThucHienId = lstNoiThucHien.First().KeyId;
                                    //query.Id = goiChiTiet.NoiThucHienId;
                                    query.ParameterDependencies = string.Format("{{NoiThucHienId:{0}}}", goiChiTiet.NoiThucHienId);
                                }

                                var lstNguoiThucHien = await _yeuCauKhamBenhService.GetBacSiKhams(query);
                                if (lstNguoiThucHien.Any())
                                {
                                    goiChiTiet.NguoiThucHienId = lstNguoiThucHien.First().KeyId;
                                }
                                lstDichVu.Add(goiChiTiet);
                                countNewDvKT++;
                            }
                            break;
                        case (int)EnumNhomGoiDichVu.DichVuGiuongBenh:
                            if (lstDichVuDuocChonTrongGoi.Any(x => x.NhomDichVu == goiChiTiet.NhomId && x.Id == goiChiTiet.Id.Value))
                            {
                                var dvgb = _dichVuGiuongBenhVienService.GetById(goiChiTiet.LoaiYeuCauDichVuId ?? 0, x => x.Include(o => o.DichVuGiuongBenhVienGiaBaoHiems));
                                var dvgbGiaBH = dvgb.DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(o => o.TuNgay <= DateTime.Now && (o.DenNgay == null || DateTime.Now <= o.DenNgay.Value));
                                //goiChiTiet.DonGiaBaoHiem = dvgbGiaBH == null ? 0 : dvgbGiaBH.Gia * (decimal)dvgbGiaBH.TiLeBaoHiemThanhToan / 100;
                                if (dvgbGiaBH != null)
                                {
                                    goiChiTiet.DonGiaBaoHiem = dvgbGiaBH.Gia;
                                    goiChiTiet.TiLeBaoHiemThanhToan = dvgbGiaBH.TiLeBaoHiemThanhToan;
                                }

                                // get nơi thực hiện mặc định
                                //var query = new DropDownListRequestModel()
                                //{
                                //    Id = goiChiTiet.LoaiYeuCauDichVuId.Value,
                                //    ParameterDependencies = string.Format("{{DichVuGiuongId:{0}}}", goiChiTiet.LoaiYeuCauDichVuId),
                                //    Take = 50
                                //};

                                var noiThucHien = await _khamBenhService.AutoGetThongTinNoiThucHienDichVuGiuong(goiChiTiet.LoaiYeuCauDichVuId.Value);
                                if (noiThucHien == null || noiThucHien.GiuongBenhVienId == null)
                                {
                                    throw new ApiException(_localizationService.GetResource("ChiDinhGoiDichVu.GiuongBenh.NotExists"));
                                }

                                goiChiTiet.NoiThucHienId = noiThucHien.PhongBenhVienId;
                                goiChiTiet.GiuongBenhId = noiThucHien.GiuongBenhVienId;
                                lstDichVu.Add(goiChiTiet);
                                countNewDVG++;
                            }
                            break;
                        case (int)EnumNhomGoiDichVu.VatTuTieuHao: break; // cần cập nhật
                        case (int)EnumNhomGoiDichVu.DuocPham: break; // cần cập nhật
                    }
                }
                gridData.First().GoiChietKhaus = lstDichVu;
            }

            var yeuCauTiepNhanChiTiet = await InsertGoiDichVuKhongChietKhauAsync(gridData, yeuCauViewModel.YeuCauTiepNhanId ?? 0, yeuCauViewModel.YeuCauKhamBenhId ?? 0);

            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            chiDinhDichVuResultVo.SoDuTaiKhoan = await _taikhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taikhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            //if(checkThemGoi!=null)
            //    throw new ApiException(_localizationService.GetResource(checkThemGoi), (int)HttpStatusCode.BadRequest);

            // xử lý kiểm tra thêm dịch vụ vượt quá số dư tài khoản
            if (yeuCauTiepNhanChiTiet != null)
            {
                // kiểm tra dvkt
                if (countNewDvKT > 0)
                {
                    chiDinhDichVuResultVo.IsVuotQuaSoDuTaiKhoan = yeuCauTiepNhanChiTiet.YeuCauDichVuKyThuats
                        //.Where(x => x.YeuCauKhamBenhId == yeuCauViewModel.YeuCauKhamBenhId && x.YeuCauTiepNhanId == yeuCauViewModel.YeuCauTiepNhanId)
                        .OrderByDescending(x => x.Id)
                        .Take(countNewDvKT)
                        .Any(x => x.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan);
                }

                if (countNewDVG > 0 && !chiDinhDichVuResultVo.IsVuotQuaSoDuTaiKhoan)
                {
                    chiDinhDichVuResultVo.IsVuotQuaSoDuTaiKhoan = yeuCauTiepNhanChiTiet.YeuCauDichVuGiuongBenhViens
                        //.Where(x => x.YeuCauKhamBenhId == yeuCauViewModel.YeuCauKhamBenhId && x.YeuCauTiepNhanId == yeuCauViewModel.YeuCauTiepNhanId)
                        .OrderByDescending(x => x.Id)
                        .Take(countNewDVG)
                        .Any(x => x.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan);
                }
            }

            return chiDinhDichVuResultVo;
        }


        [HttpPost("ThemYeuCauDichVuKyThuat")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamBenh, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> ThemYeuCauDichVuKyThuat([FromBody] KhamBenhYeuCauDichVuKyThuatViewModel yeuCauViewModel)
        {
            await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(yeuCauViewModel.YeuCauKhamBenhId ?? 0);

            var entity = yeuCauViewModel.ToEntity<YeuCauDichVuKyThuat>();
            var yctn = _yeuCauTiepNhanService.GetById(entity.YeuCauTiepNhanId, o =>
                     o.Include(yctns => yctns.DoiTuongUuDai).ThenInclude(dtud => dtud.DoiTuongUuDaiDichVuKyThuatBenhViens)
                                                            .ThenInclude(dtUDdvkybv => dtUDdvkybv.DichVuKyThuatBenhVien));
            var dvkt = _dichVuKyThuatBenhVienService.GetById(entity.DichVuKyThuatBenhVienId, x => x.Include(o => o.DichVuKyThuatBenhVienGiaBaoHiems));
            var dvktGiaBH = dvkt.DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(o => o.TuNgay <= DateTime.Now && (o.DenNgay == null || DateTime.Now <= o.DenNgay.Value));
            var dtudDVKTBV = yctn.DoiTuongUuDai?.DoiTuongUuDaiDichVuKyThuatBenhViens?.FirstOrDefault(o =>
                                                                        o.DichVuKyThuatBenhVienId == yeuCauViewModel.DichVuKyThuatBenhVienId && o.DichVuKyThuatBenhVien.CoUuDai == true);

            var yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId.Value);
            if (yeuCauTiepNhanChiTiet == null)
            {
                throw new ApiException("entity");
            }

            var coBHYT = yeuCauTiepNhanChiTiet.CoBHYT ?? false;
            var yeuCauKhamBenh = yeuCauTiepNhanChiTiet.YeuCauKhamBenhs
                .Where(x => x.Id == yeuCauViewModel.YeuCauKhamBenhId.Value).FirstOrDefault();
            var duocHuongBaoHiem = coBHYT && yeuCauKhamBenh != null && yeuCauKhamBenh.DuocHuongBaoHiem && dvktGiaBH != null && dvktGiaBH.Gia != 0;

            if (duocHuongBaoHiem)
            {
                entity.DuocHuongBaoHiem = true;
                entity.BaoHiemChiTra = null;
            }
            else
            {
                entity.DuocHuongBaoHiem = false;
                entity.BaoHiemChiTra = null;
            }

            entity.DuocHuongBaoHiem = duocHuongBaoHiem;
            entity.SoLan = 1;
            entity.TiLeUuDai = dtudDVKTBV?.TiLeUuDai;
            entity.TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan;
            entity.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
            entity.NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId();
            entity.NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();

            if (dvktGiaBH != null)
            {
                entity.DonGiaBaoHiem = dvktGiaBH.Gia;
                entity.TiLeBaoHiemThanhToan = dvktGiaBH.TiLeBaoHiemThanhToan;
            }

            yeuCauTiepNhanChiTiet.YeuCauDichVuKyThuats.Add(entity);

            var yeuCauKhamBenhDangKham = yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.FirstOrDefault(x => x.Id == yeuCauViewModel.YeuCauKhamBenhId);
            if (yeuCauKhamBenhDangKham != null && yeuCauKhamBenhDangKham.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham)
            {
                yeuCauKhamBenhDangKham.TrangThai = EnumTrangThaiYeuCauKhamBenh.DangKham;
                yeuCauKhamBenhDangKham.NoiThucHienId = yeuCauKhamBenhDangKham.NoiDangKyId; //_userAgentHelper.GetCurrentNoiLLamViecId();
                yeuCauKhamBenhDangKham.BacSiThucHienId = _userAgentHelper.GetCurrentUserId();
                yeuCauKhamBenhDangKham.ThoiDiemThucHien = DateTime.Now;

                YeuCauKhamBenhLichSuTrangThai trangThaiMoi = new YeuCauKhamBenhLichSuTrangThai
                {
                    TrangThaiYeuCauKhamBenh = yeuCauKhamBenhDangKham.TrangThai,
                    MoTa = yeuCauKhamBenhDangKham.TrangThai.GetDescription()
                };
                yeuCauKhamBenhDangKham.YeuCauKhamBenhLichSuTrangThais.Add(trangThaiMoi);
            }

            await _tiepNhanBenhNhanServiceService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            // kiểm tra chỉ định dich vụ vượt quá số dư tài khoản
            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            chiDinhDichVuResultVo.SoDuTaiKhoan = await _taikhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taikhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            var yeuCauVuaThem = yeuCauTiepNhanChiTiet.YeuCauDichVuKyThuats
                .LastOrDefault(x => x.YeuCauKhamBenhId == yeuCauViewModel.YeuCauKhamBenhId
                                    && x.YeuCauTiepNhanId == yeuCauViewModel.YeuCauTiepNhanId);
            chiDinhDichVuResultVo.IsVuotQuaSoDuTaiKhoan = yeuCauVuaThem != null && yeuCauVuaThem.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan;

            // nếu loại dvkt vừa thêm thuộc 4 nhóm này, thì chuyển yêu cầu khám bệnh hiện tại sang hàng chờ làm chỉ định
            if (entity.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat
                || entity.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh
                || entity.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang
                || entity.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem)
            {
                chiDinhDichVuResultVo.ChuyenHangDoiSangLamChiDinh = true;
                await _khamBenhService.CapNhatHangChoKhiChiDinhDichVuKyThuatAsync(entity.YeuCauTiepNhanId, entity.YeuCauKhamBenhId.Value, entity.NoiChiDinhId.Value);
            }

            return chiDinhDichVuResultVo;
        }


        [HttpPost("ThemYeuCauDichVuKyThuatMultiselect")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> ThemYeuCauDichVuKyThuatMultiselectAsync([FromBody] KhamBenhChiDinhDichVuKyThuatMultiselectViewModel yeuCauViewModel)
        {
            // kiểm tra yêu cầu khám bệnh trước khi thêm
            if (!yeuCauViewModel.IsKhamDoanTatCa)
            {
                if (yeuCauViewModel.IsKhamBenhDangKham)
                {
                    await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(
                        yeuCauViewModel.YeuCauKhamBenhId);
                }
                else
                {
                    await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(yeuCauViewModel.YeuCauKhamBenhId);
                }
            }


            var yeuCauVo = yeuCauViewModel.Map<ChiDinhDichVuKyThuatMultiselectVo>();


            //// get thông tin yêu cầu tiếp nhận hiện tại
            //var yeuCauTiepNhanChiTiet =
            //    await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);

            //Cập nhật 17/03/2022: get thông tin YCTN khi thêm dịch vụ
            var yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanKhiThemDichVuNgoaiTruByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);

            // kiểm tra nếu chưa nhập chẩn đoán sơ bộ thì ko cho thêm dvkt, ghi nhận thuốc/VTTH
            _yeuCauKhamBenhService.KiemTraChanDoanSoBoKhiThemDichVu(yeuCauTiepNhanChiTiet, yeuCauViewModel.YeuCauKhamBenhId);

            // kiểm tra nếu là dịch vụ thêm từ gói
            var dichVuChiDinhTheoGoiVo = new ChiDinhGoiDichVuTheoBenhNhanVo()
            {
                YeuCauTiepNhanId = yeuCauViewModel.YeuCauTiepNhanId,
                YeuCauKhamBenhId = yeuCauViewModel.YeuCauKhamBenhId
            };
            if (yeuCauViewModel.DichVuKyThuatTuGois != null && yeuCauViewModel.DichVuKyThuatTuGois.Any())
            {
                dichVuChiDinhTheoGoiVo.DichVus.AddRange(yeuCauViewModel.DichVuKyThuatTuGois
                    .Select(item => new ChiTietGoiDichVuChiDinhTheoBenhNhanVo()
                    {
                        Id = item.Id,
                        YeuCauGoiDichVuId = item.YeuCauGoiDichVuId ?? 0,
                        TenDichVu = item.TenDichVu,
                        ChuongTrinhGoiDichVuId = item.ChuongTrinhGoiDichVuId ?? 0,
                        ChuongTrinhGoiDichVuChiTietId = item.ChuongTrinhGoiDichVuChiTietId ?? 0,
                        DichVuBenhVienId = item.DichVuBenhVienId ?? 0,
                        NhomGoiDichVu = (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                        SoLuongSuDung = 1
                    }));

                var lstDichVuKyThuatDangChon = yeuCauViewModel.DichVuKyThuatBenhVienChiDinhs.Select(x => x).ToList();
                foreach (var strId in lstDichVuKyThuatDangChon)
                {
                    var itemObj = JsonConvert.DeserializeObject<ItemChiDinhDichVuKyThuatVo>(strId);
                    if (dichVuChiDinhTheoGoiVo.DichVus.Any(x => x.DichVuBenhVienId == itemObj.DichVuId))
                    {
                        var dichVuLoaiBo = yeuCauViewModel.DichVuKyThuatBenhVienChiDinhs.FirstOrDefault(x => x == strId);
                        yeuCauVo.DichVuKyThuatBenhVienChiDinhs.Remove(dichVuLoaiBo);
                    }
                }

                await _khamBenhService.XuLyThemChiDinhGoiDichVuTheoBenhNhanAsync(yeuCauTiepNhanChiTiet, dichVuChiDinhTheoGoiVo);
            }

            if (yeuCauVo.DichVuKyThuatBenhVienChiDinhs.Any())
            {
                if (yeuCauTiepNhanChiTiet.LoaiYeuCauTiepNhan == EnumLoaiYeuCauTiepNhan.KhamSucKhoe &&
                    yeuCauVo.LoaiDangNhap == HinhThucKhamBenh.KhamDoanNgoaiVien)
                {
                    var query = new DropDownListRequestModel()
                    {
                        ParameterDependencies = JsonConvert.SerializeObject(new KhoaPhongJsonVo()
                        {
                            HopDongKhamSucKhoeId = yeuCauTiepNhanChiTiet.HopDongKhamSucKhoeNhanVien?.HopDongKhamSucKhoeId,
                            DichVuId = yeuCauTiepNhanChiTiet.Id // biến này gán để cheat trong hàm get nơi thực hiện ngoại viện
                        }),
                        Take = Int32.MaxValue
                    };
                    yeuCauVo.NoiThucHienNgoaiVienTheoHopDongs = await _khamDoanService.GetKhoaPhongGoiKham(query);
                }
                
                await _khamBenhService.XuLyThemYeuCauDichVuKyThuatMultiselectAsync(yeuCauVo, yeuCauTiepNhanChiTiet);
            }

            await _tiepNhanBenhNhanServiceService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            // kiểm tra chỉ định dich vụ vượt quá số dư tài khoản
            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            chiDinhDichVuResultVo.SoDuTaiKhoan = await _taikhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taikhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            if (yeuCauVo.DichVuKyThuatBenhVienChiDinhs.Any())
            {
                var yeuCauVuaThem = yeuCauTiepNhanChiTiet.YeuCauDichVuKyThuats
                    .Where(x => x.YeuCauKhamBenhId == yeuCauViewModel.YeuCauKhamBenhId
                                        && x.YeuCauTiepNhanId == yeuCauViewModel.YeuCauTiepNhanId
                                        && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                        && (yeuCauVo.YeuCauDichVuKyThuatCuoiCungId == 0 || (x.Id > yeuCauVo.YeuCauDichVuKyThuatCuoiCungId))
                                        && x.YeuCauGoiDichVuId == null)
                    .ToList();
                chiDinhDichVuResultVo.IsVuotQuaSoDuTaiKhoan = yeuCauVuaThem.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan
                                                                                     && x.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan);

                // nếu loại dvkt vừa thêm thuộc 4 nhóm này, thì chuyển yêu cầu khám bệnh hiện tại sang hàng chờ làm chỉ định
                if (yeuCauVo.ChuyenHangDoiSangLamChiDinh) // && !yeuCauViewModel.IsKhamBenhDangKham)
                {
                    chiDinhDichVuResultVo.ChuyenHangDoiSangLamChiDinh = true;
                }
            }
            else
            {
                if (yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.First(x => x.Id == dichVuChiDinhTheoGoiVo.YeuCauKhamBenhId).TrangThai != EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh
                    && dichVuChiDinhTheoGoiVo.YeuCauDichVuKyThuatNews.Any(x => x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem
                                                                 || x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat
                                                                 || x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                                                 || x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang))
                {
                    chiDinhDichVuResultVo.ChuyenHangDoiSangLamChiDinh = true;
                }
            }

            if (chiDinhDichVuResultVo.ChuyenHangDoiSangLamChiDinh)
            {
                var phongHangDoiHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
                if (yeuCauViewModel.IsKhamBenhDangKham)
                {
                    var hangDoi = yeuCauTiepNhanChiTiet.YeuCauKhamBenhs
                        .Where(x => x.Id == yeuCauVo.YeuCauKhamBenhId && x.YeuCauTiepNhanId == yeuCauVo.YeuCauTiepNhanId)
                        .SelectMany(x => x.PhongBenhVienHangDois)
                        .OrderByDescending(x => x.Id)
                        .ToList();
                    if (hangDoi.Any())
                    {
                        phongHangDoiHienTaiId = hangDoi.First().PhongBenhVienId;
                    }
                }
                await _khamBenhService.CapNhatHangChoKhiChiDinhDichVuKyThuatAsync(yeuCauVo.YeuCauTiepNhanId, yeuCauVo.YeuCauKhamBenhId, phongHangDoiHienTaiId);
            }
            #region BVHD-3761
            if (!string.IsNullOrEmpty(yeuCauViewModel.BieuHienLamSang) || !string.IsNullOrEmpty(yeuCauViewModel.DichTeSarsCoV2))
            {
                _khamBenhService.UpdateDichVuKyThuatSarsCoVTheoYeuCauTiepNhan(yeuCauViewModel.YeuCauTiepNhanId, yeuCauViewModel.BieuHienLamSang, yeuCauViewModel.DichTeSarsCoV2);
            }
            #endregion BVHD-3761

            return chiDinhDichVuResultVo;
        }

        // TODO: cần cập nhật
        [HttpPost("ThemYeuCauKhamBenh")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> ThemYeuCauKhamBenh([FromBody] ChiDinhDichVuKhamBenhViewModel yeuCauViewModel)
        {
            // trường hợp chức năng chỉnh sửa yêu cầu khám bệnh đang khám
            if (yeuCauViewModel.IsKhamBenhDangKham)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(yeuCauViewModel.YeuCauKhamBenhTruocId);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(yeuCauViewModel.YeuCauKhamBenhTruocId);
            }

            if (yeuCauViewModel.Gia == null) // || yeuCauViewModel.Gia == 0)
            {
                throw new ApiException(_localizationService.GetResource("ChiDinh.LoaiGia.NotExists"));
            }

            //var yeuCauTiepNhanChiTiet =
            //    await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanKhiThemDichVuNgoaiTruByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);

            // kiểm tra nếu là dịch vụ thêm từ gói
            if (yeuCauViewModel.DichVuKhamBenhTuGoi != null)
            {
                var dichVuChiDinhTheoGoiVo = new ChiDinhGoiDichVuTheoBenhNhanVo()
                {
                    YeuCauTiepNhanId = yeuCauViewModel.YeuCauTiepNhanId,
                    YeuCauKhamBenhId = yeuCauViewModel.YeuCauKhamBenhTruocId
                };


                dichVuChiDinhTheoGoiVo.DichVus.Add(new ChiTietGoiDichVuChiDinhTheoBenhNhanVo()
                {
                    Id = yeuCauViewModel.DichVuKhamBenhTuGoi.Id,
                    YeuCauGoiDichVuId = yeuCauViewModel.DichVuKhamBenhTuGoi.YeuCauGoiDichVuId ?? 0,
                    TenDichVu = yeuCauViewModel.DichVuKhamBenhTuGoi.TenDichVu,
                    ChuongTrinhGoiDichVuId = yeuCauViewModel.DichVuKhamBenhTuGoi.ChuongTrinhGoiDichVuId ?? 0,
                    ChuongTrinhGoiDichVuChiTietId = yeuCauViewModel.DichVuKhamBenhTuGoi.ChuongTrinhGoiDichVuChiTietId ?? 0,
                    DichVuBenhVienId = yeuCauViewModel.DichVuKhamBenhTuGoi.DichVuBenhVienId ?? 0,
                    NhomGoiDichVu = (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh,
                    SoLuongSuDung = 1
                });

                await _khamBenhService.XuLyThemChiDinhGoiDichVuTheoBenhNhanAsync(yeuCauTiepNhanChiTiet, dichVuChiDinhTheoGoiVo);
            }
            // nếu là dịch vụ thêm bình thường
            else
            {

                var dichVuKhamBenhChiDinh = await _dichVuKhamBenhBenhVienService.GetByIdAsync(
                    yeuCauViewModel.DichVuKhamBenhBenhVienId ?? 0,
                    x => x.Include(y => y.DichVuKhamBenhBenhVienGiaBaoHiems));
                var giaBaoHiem = dichVuKhamBenhChiDinh.DichVuKhamBenhBenhVienGiaBaoHiems.FirstOrDefault(x =>
                    x.TuNgay.Date <= DateTime.Now.Date &&
                    (x.DenNgay == null || x.DenNgay.Value.Date >= DateTime.Now.Date));

                var entity = yeuCauViewModel.ToEntity<YeuCauKhamBenh>();
                entity.NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId();
                entity.NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();
                entity.TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan;
                entity.TrangThai = EnumTrangThaiYeuCauKhamBenh.ChuaKham;
                entity.BaoHiemChiTra = null;
                entity.KhongTinhPhi = yeuCauViewModel.TinhPhi != null ? !yeuCauViewModel.TinhPhi : null;

                if (giaBaoHiem != null)
                {
                    entity.DonGiaBaoHiem = giaBaoHiem.Gia;
                    entity.TiLeBaoHiemThanhToan = giaBaoHiem.TiLeBaoHiemThanhToan;
                }

                yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.Add(entity);

                var yeuCauKhamBenhDangKham =
                    yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.FirstOrDefault(x =>
                        x.Id == yeuCauViewModel.YeuCauKhamBenhTruocId);
                if (yeuCauKhamBenhDangKham != null &&
                    yeuCauKhamBenhDangKham.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham)
                {
                    yeuCauKhamBenhDangKham.TrangThai = EnumTrangThaiYeuCauKhamBenh.DangKham;
                    yeuCauKhamBenhDangKham.NoiThucHienId = yeuCauKhamBenhDangKham.NoiDangKyId; //_userAgentHelper.GetCurrentNoiLLamViecId();
                    yeuCauKhamBenhDangKham.BacSiThucHienId = _userAgentHelper.GetCurrentUserId();
                    yeuCauKhamBenhDangKham.ThoiDiemThucHien = DateTime.Now;

                    YeuCauKhamBenhLichSuTrangThai trangThaiMoi = new YeuCauKhamBenhLichSuTrangThai
                    {
                        TrangThaiYeuCauKhamBenh = yeuCauKhamBenhDangKham.TrangThai,
                        MoTa = yeuCauKhamBenhDangKham.TrangThai.GetDescription()
                    };
                    yeuCauKhamBenhDangKham.YeuCauKhamBenhLichSuTrangThais.Add(trangThaiMoi);
                }
            }

            await _tiepNhanBenhNhanServiceService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            // kiểm tra chỉ định dich vụ vượt quá số dư tài khoản
            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            chiDinhDichVuResultVo.SoDuTaiKhoan = await _taikhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taikhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            if (yeuCauViewModel.DichVuKhamBenhTuGoi == null)
            {
                var yeuCauVuaThem = yeuCauTiepNhanChiTiet.YeuCauKhamBenhs
                    .LastOrDefault(x => x.YeuCauKhamBenhTruocId == yeuCauViewModel.YeuCauKhamBenhTruocId
                                        && x.YeuCauTiepNhanId == yeuCauViewModel.YeuCauTiepNhanId);

                chiDinhDichVuResultVo.IsVuotQuaSoDuTaiKhoan = yeuCauVuaThem != null
                                                              && yeuCauVuaThem.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan
                                                              && yeuCauVuaThem.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan; //TrangThaiThanhToan.ChuaThanhToan;
            }
            return chiDinhDichVuResultVo;
        }

        // TODO: cần update
        [HttpPost("ThemYeuCauGiuongBenh")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamBenh, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> ThemYeuCauGiuongBenh([FromBody] KhamBenhYeuCauGiuongBenhViewModel yeuCauViewModel)
        {
            await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(yeuCauViewModel.YeuCauKhamBenhId ?? 0);
            var entity = yeuCauViewModel.ToEntity<YeuCauDichVuGiuongBenhVien>();
            var dvkt = _dichVuGiuongBenhVienService.GetById(entity.DichVuGiuongBenhVienId, x => x.Include(o => o.DichVuGiuongBenhVienGiaBaoHiems));
            var dvktGiaBH = dvkt.DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(o => o.TuNgay <= DateTime.Now && (o.DenNgay == null || DateTime.Now <= o.DenNgay.Value));

            var yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId.Value);
            if (yeuCauTiepNhanChiTiet == null)
            {
                throw new ApiException("entity");
            }

            var coBHYT = yeuCauTiepNhanChiTiet.CoBHYT ?? false;
            var yeuCauKhamBenh = yeuCauTiepNhanChiTiet.YeuCauKhamBenhs
                .Where(x => x.Id == yeuCauViewModel.YeuCauKhamBenhId.Value).FirstOrDefault();
            var duocHuongBaoHiem = yeuCauViewModel.DuocHuongBaoHiem != null && yeuCauViewModel.DuocHuongBaoHiem.Value
                                                                            && coBHYT && yeuCauKhamBenh != null && yeuCauKhamBenh.DuocHuongBaoHiem && dvktGiaBH != null && dvktGiaBH.Gia != 0;

            if (duocHuongBaoHiem)
            {
                entity.DuocHuongBaoHiem = true;
                entity.BaoHiemChiTra = null;
            }
            else
            {
                entity.DuocHuongBaoHiem = false;
                entity.BaoHiemChiTra = null;
            }

            entity.KhongTinhPhi = !yeuCauViewModel.TinhPhi;
            entity.DuocHuongBaoHiem = duocHuongBaoHiem;
            entity.TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan;
            entity.TrangThai = EnumTrangThaiGiuongBenh.ChuaThucHien;
            entity.NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId();
            entity.NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();

            if (dvktGiaBH != null)
            {
                entity.DonGiaBaoHiem = dvktGiaBH.Gia;
                entity.TiLeBaoHiemThanhToan = dvktGiaBH.TiLeBaoHiemThanhToan;
            }

            //var hoatDongGiuongBenh = new HoatDongGiuongBenh();
            //hoatDongGiuongBenh.GiuongBenhId = entity.GiuongBenhId.Value;
            //hoatDongGiuongBenh.ThoiDiemBatDau = entity.ThoiDiemBatDauSuDung.Value;
            //hoatDongGiuongBenh.YeuCauTiepNhanId = entity.YeuCauTiepNhanId;
            //hoatDongGiuongBenh.YeuCauKhamBenhId = entity.YeuCauKhamBenhId;
            //entity.HoatDongGiuongBenhs.Add(hoatDongGiuongBenh);

            yeuCauTiepNhanChiTiet.YeuCauDichVuGiuongBenhViens.Add(entity);

            var yeuCauKhamBenhDangKham = yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.FirstOrDefault(x => x.Id == yeuCauViewModel.YeuCauKhamBenhId);
            if (yeuCauKhamBenhDangKham != null && yeuCauKhamBenhDangKham.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham)
            {
                yeuCauKhamBenhDangKham.TrangThai = EnumTrangThaiYeuCauKhamBenh.DangKham;
                yeuCauKhamBenhDangKham.NoiThucHienId = yeuCauKhamBenhDangKham.NoiDangKyId; // _userAgentHelper.GetCurrentNoiLLamViecId();
                yeuCauKhamBenhDangKham.BacSiThucHienId = _userAgentHelper.GetCurrentUserId();
                yeuCauKhamBenhDangKham.ThoiDiemThucHien = DateTime.Now;

                YeuCauKhamBenhLichSuTrangThai trangThaiMoi = new YeuCauKhamBenhLichSuTrangThai
                {
                    TrangThaiYeuCauKhamBenh = yeuCauKhamBenhDangKham.TrangThai,
                    MoTa = yeuCauKhamBenhDangKham.TrangThai.GetDescription()
                };
                yeuCauKhamBenhDangKham.YeuCauKhamBenhLichSuTrangThais.Add(trangThaiMoi);
            }

            await _tiepNhanBenhNhanServiceService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            chiDinhDichVuResultVo.SoDuTaiKhoan = await _taikhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taikhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            var yeuCauVuaThem = yeuCauTiepNhanChiTiet.YeuCauDichVuGiuongBenhViens
                .LastOrDefault(x => x.YeuCauKhamBenhId == yeuCauViewModel.YeuCauKhamBenhId
                                    && x.YeuCauTiepNhanId == yeuCauViewModel.YeuCauTiepNhanId);
            chiDinhDichVuResultVo.IsVuotQuaSoDuTaiKhoan = yeuCauVuaThem != null
                                                          && yeuCauVuaThem.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan
                                                          && yeuCauVuaThem.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan;

            return chiDinhDichVuResultVo;
        }

        // TODO: cần update theo logic mới
        [HttpPost("ThemCauYeuVatTuBenhVien")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamBenh, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult> ThemYeuVatTuBenhVien([FromBody] KhamBenhYeuCauVatTuBenhVienViewModel yeuCauViewModel)
        {
            var entity = yeuCauViewModel.ToEntity<YeuCauVatTuBenhVien>();

            // cần cập nhật lại chỗ này
            entity.DuocHuongBaoHiem = false;
            entity.BaoHiemChiTra = null;

            //entity.GiaBaoHiemThanhToan = 0;
            entity.TrangThaiThanhToan = yeuCauViewModel.KhongTinhPhi == true ? TrangThaiThanhToan.DaThanhToan : TrangThaiThanhToan.ChuaThanhToan;
            entity.TrangThai = EnumYeuCauVatTuBenhVien.ChuaThucHien;
            entity.NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId();
            entity.NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();

            var yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId.Value);

            yeuCauTiepNhanChiTiet.YeuCauVatTuBenhViens.Add(entity);
            await _tiepNhanBenhNhanServiceService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);
            //await _yeuCauVatTuBenhVienService.AddAsync(entity);
            return NoContent();
        }

        // TODO: cần update theo logic mới
        [HttpPost("ThemYeuCauDuocPhamBenhVien")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamBenh, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult> ThemYeuCauDuocPhamBenhVien([FromBody] KhamBenhYeuCauDuocPhamViewModel yeuCauViewModel)
        {
            var entity = yeuCauViewModel.ToEntity<YeuCauDuocPhamBenhVien>();
            await _yeuCauDuocPhamBenhVienService.ThemYeuCauDuocPhamBenhVien(entity);
            if (entity == null)
                throw new ApiException("entity");

            var yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId.Value);

            yeuCauTiepNhanChiTiet.YeuCauDuocPhamBenhViens.Add(entity);
            await _tiepNhanBenhNhanServiceService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);
            return NoContent();
        }

        #endregion

        #region Xoa chi dinh

        [HttpPost("XoaDichVuKyThuat")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> XoaDichVuKyThuat(XoaChiDinhDichVuViewModel xoaDichVuViewModel)
        {
            if (!xoaDichVuViewModel.IsKhamDoanTatCa)
            {
                if (xoaDichVuViewModel.IsKhamBenhDangKham)
                {
                    await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(xoaDichVuViewModel
                        .YeuCauKhamBenhId);
                }
                else
                {
                    await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(xoaDichVuViewModel.YeuCauKhamBenhId);
                }
            }

            var entity = _yeuCauDichVuKyThuatService.GetById(xoaDichVuViewModel.DichVuId);
            //if (entity == null || entity.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan || entity.TrangThaiThanhToan == TrangThaiThanhToan.CapNhatThanhToan)
            //    throw new ApiException(_localizationService.GetResource("KhamBenhChiDinh.DichVuKyThuat.DaThanhToan"));

            if (entity != null && (entity.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien || entity.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien))
            {
                throw new ApiException(_localizationService.GetResource("KhamBenhChiDinh.DichVuKyThuat.DaThucHien"));
            }

            //var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(entity.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanKhiXoaRiengDichVuKyThuatNgoaiTruByIdAsync(entity.YeuCauTiepNhanId);
            foreach (var item in yeuCauTiepNhanChiTiet.YeuCauDichVuKyThuats)
            {
                if (item.Id == xoaDichVuViewModel.DichVuId)
                {
                    if (yeuCauTiepNhanChiTiet.YeuCauDuocPhamBenhViens.Any(x => x.YeuCauDichVuKyThuatId == item.Id && x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                        || yeuCauTiepNhanChiTiet.YeuCauVatTuBenhViens.Any(x => x.YeuCauDichVuKyThuatId == item.Id && x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy))
                    {
                        throw new ApiException(_localizationService.GetResource("ChiDinh.HuyDichDichVu.DaCoGhiNhanVTTHThuoc"));
                    }
                    //item.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.DaHuy;
                    //item.TrangThaiThanhToan = TrangThaiThanhToan.HuyThanhToan;
                    item.WillDelete = true;
                    if (!string.IsNullOrEmpty(xoaDichVuViewModel.LyDoHuyDichVu))
                    {
                        if (item.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        {
                            throw new ApiException(_localizationService.GetResource("ChiDinh.HuyDichDichVu.DaThanhToan"));
                        }
                        item.NhanVienHuyDichVuId = _userAgentHelper.GetCurrentUserId();
                        item.LyDoHuyDichVu = xoaDichVuViewModel.LyDoHuyDichVu;
                    }
                    else
                    {
                        //if (item.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan && item.TaiKhoanBenhNhanChis.Any())
                        //{
                        //    throw new ApiException(_localizationService.GetResource("ChiDinh.XoaDichDichVu.DaHuyThanhToan"));
                        //}

                        if (item.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan)
                        {
                            var coChi = await _khamBenhService.KiemTraTaiKhoanBenhNhanChiTheoDichVu(yeuCauKyThuatId: item.Id);
                            if(coChi)
                            {
                                throw new ApiException(_localizationService.GetResource("ChiDinh.XoaDichDichVu.DaHuyThanhToan"));
                            }
                        }
                    }

                    //BVHD-3825
                    //var mienGiam = item.MienGiamChiPhis.FirstOrDefault(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true));
                    var mienGiam = await _khamBenhService.GetMienGiamChiPhiTrongGoiTheoDichVu(yeuCauKyThuatId: item.Id);
                    if (mienGiam != null)
                    {
                        mienGiam.DaHuy = true;
                        mienGiam.WillDelete = true;

                        var giamSoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                        if (giamSoTienMienGiam < 0)
                        {
                            giamSoTienMienGiam = 0;
                        }
                        item.SoTienMienGiam = giamSoTienMienGiam;
                    }
                    break;
                }
            }

            await _tiepNhanBenhNhanServiceService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            chiDinhDichVuResultVo.SoDuTaiKhoan = await _taikhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taikhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            return chiDinhDichVuResultVo;
        }

        [HttpPost("XoaDichVuKhamBenh")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> XoaDichVuKhamBenh(XoaChiDinhDichVuViewModel xoaDichVuViewModel)
        {
            if (xoaDichVuViewModel.IsKhamBenhDangKham)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(xoaDichVuViewModel.YeuCauKhamBenhId);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(xoaDichVuViewModel.YeuCauKhamBenhId);
            }

            var entity = await _yeuCauKhamBenhService.GetByIdAsync(xoaDichVuViewModel.DichVuId);
            if (entity.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham && entity.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)
            {
                throw new ApiException(_localizationService.GetResource("ChiDinh.DichVuKham.DaThucHien"));

            }

            //var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(entity.YeuCauTiepNhanId);
            //var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanKhiXoaRiengDichVuKhamNgoaiTruByIdAsync(entity.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanKhiXoaRiengDichVuKhamTabChiDinhByIdAsync(entity.YeuCauTiepNhanId);
            foreach (var item in yeuCauTiepNhanChiTiet.YeuCauKhamBenhs)
            {
                if (item.Id == xoaDichVuViewModel.DichVuId)
                {
                    if (yeuCauTiepNhanChiTiet.YeuCauDuocPhamBenhViens.Any(x => x.YeuCauKhamBenhId == item.Id && x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                        || yeuCauTiepNhanChiTiet.YeuCauVatTuBenhViens.Any(x => x.YeuCauKhamBenhId == item.Id && x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy))
                    {
                        throw new ApiException(_localizationService.GetResource("ChiDinh.HuyDichDichVu.DaCoGhiNhanVTTHThuoc"));
                    }

                    item.WillDelete = true;
                    if (!string.IsNullOrEmpty(xoaDichVuViewModel.LyDoHuyDichVu))
                    {
                        if (item.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        {
                            throw new ApiException(_localizationService.GetResource("ChiDinh.HuyDichDichVu.DaThanhToan"));
                        }
                        item.NhanVienHuyDichVuId = _userAgentHelper.GetCurrentUserId();
                        item.LyDoHuyDichVu = xoaDichVuViewModel.LyDoHuyDichVu;
                    }
                    else
                    {
                        //if (item.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan && item.TaiKhoanBenhNhanChis.Any())
                        //{
                        //    throw new ApiException(_localizationService.GetResource("ChiDinh.XoaDichDichVu.DaHuyThanhToan"));
                        //}

                        //Cập nhật 06/12/2022
                        if (item.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan)
                        {
                            var coChi = await _khamBenhService.KiemTraTaiKhoanBenhNhanChiTheoDichVu(yeuCauKhamBenhId: item.Id);
                            if (coChi)
                            {
                                throw new ApiException(_localizationService.GetResource("ChiDinh.XoaDichDichVu.DaHuyThanhToan"));
                            }
                        }
                    }

                    //BVHD-3825
                    //var mienGiam = item.MienGiamChiPhis.FirstOrDefault(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true));

                    //Cập nhật 06/12/2022
                    var mienGiam = await _khamBenhService.GetMienGiamChiPhiTrongGoiTheoDichVu(yeuCauKhamBenhId: item.Id);
                    if (mienGiam != null)
                    {
                        mienGiam.DaHuy = true;
                        mienGiam.WillDelete = true;

                        var giamSoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                        if (giamSoTienMienGiam < 0)
                        {
                            giamSoTienMienGiam = 0;
                        }
                        item.SoTienMienGiam = giamSoTienMienGiam;
                    }
                    break;
                }
            }

            await _tiepNhanBenhNhanServiceService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            chiDinhDichVuResultVo.SoDuTaiKhoan = await _taikhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taikhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            return chiDinhDichVuResultVo;
        }

        [HttpPost("XoaNhieuDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> XoaNhieuDichVu(XoaNhieuDichVuViewModel xoaDichVu)
        {
            if (xoaDichVu.XoaNhieuChiDinhDichVuChiTiets.First().IsKhamBenhDangKham)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(xoaDichVu.XoaNhieuChiDinhDichVuChiTiets.First().YeuCauKhamBenhId);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(xoaDichVu.XoaNhieuChiDinhDichVuChiTiets.First().YeuCauKhamBenhId);
            }

            //var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(xoaDichVu.XoaNhieuChiDinhDichVuChiTiets.First().YeuCauTiepNhanId);
            //var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanKhiXoaDichVuNgoaiTruByIdAsync(xoaDichVu.XoaNhieuChiDinhDichVuChiTiets.First().YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanKhiXoaNhieuDichVuNgoaiTruByIdAsync(xoaDichVu.XoaNhieuChiDinhDichVuChiTiets.First().YeuCauTiepNhanId);

            #region Cập nhật 5/12/2022
            var lstDichVuKhamId = xoaDichVu.XoaNhieuChiDinhDichVuChiTiets.Where(x => x.LaDichVuKham).Select(x => x.DichVuId).Distinct().ToList();
            var lstDichVuKyThuatId = xoaDichVu.XoaNhieuChiDinhDichVuChiTiets.Where(x => x.LaDichVuKham != true).Select(x => x.DichVuId).Distinct().ToList();

            var lstDichVuKhamCoChi = new List<long>();
            var lstDichVuKyThuatCoChi = new List<long>();
            if(lstDichVuKhamId.Any())
            {
                lstDichVuKhamCoChi = await _khamBenhService.KiemTraTaiKhoanBenhNhanChiTheoDichVus(yeuCauKhamBenhIds: lstDichVuKhamId);
            }
            if (lstDichVuKyThuatId.Any())
            {
                lstDichVuKyThuatCoChi = await _khamBenhService.KiemTraTaiKhoanBenhNhanChiTheoDichVus(yeuCauKyThuatIds: lstDichVuKyThuatId);
            }
            #endregion

            foreach (var xoaDichVuViewModel in xoaDichVu.XoaNhieuChiDinhDichVuChiTiets)
            {
                if (xoaDichVuViewModel.LaDichVuKham)
                {
                    var dichVu =
                        yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.FirstOrDefault(x => x.Id == xoaDichVuViewModel.DichVuId);
                    if (dichVu != null)
                    {
                        if (dichVu.TrangThai != EnumTrangThaiYeuCauKhamBenh.ChuaKham && dichVu.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham)
                        {
                            throw new ApiException(_localizationService.GetResource("ChiDinh.DichVuKham.DaThucHien"));

                        }

                        if (yeuCauTiepNhanChiTiet.YeuCauDuocPhamBenhViens.Any(x => x.YeuCauKhamBenhId == dichVu.Id && x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                            || yeuCauTiepNhanChiTiet.YeuCauVatTuBenhViens.Any(x => x.YeuCauKhamBenhId == dichVu.Id && x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy))
                        {
                            throw new ApiException(_localizationService.GetResource("ChiDinh.HuyDichDichVu.DaCoGhiNhanVTTHThuoc"));
                        }

                        dichVu.WillDelete = true;
                        if (!string.IsNullOrEmpty(xoaDichVuViewModel.LyDoHuyDichVu))
                        {
                            if (dichVu.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                            {
                                throw new ApiException(_localizationService.GetResource("ChiDinh.HuyDichDichVu.DaThanhToan"));
                            }
                            dichVu.NhanVienHuyDichVuId = _userAgentHelper.GetCurrentUserId();
                            dichVu.LyDoHuyDichVu = xoaDichVuViewModel.LyDoHuyDichVu;
                        }
                        else
                        {
                            if (dichVu.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan && lstDichVuKhamCoChi.Contains(xoaDichVuViewModel.DichVuId)) // dichVu.TaiKhoanBenhNhanChis.Any())
                            {
                                throw new ApiException(_localizationService.GetResource("ChiDinh.XoaDichDichVu.DaHuyThanhToan"));
                            }
                        }

                        //BVHD-3825
                        var mienGiam = dichVu.MienGiamChiPhis.FirstOrDefault(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true));
                        if (mienGiam != null)
                        {
                            mienGiam.DaHuy = true;
                            mienGiam.WillDelete = true;

                            var giamSoTienMienGiam = dichVu.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                            if (giamSoTienMienGiam < 0)
                            {
                                giamSoTienMienGiam = 0;
                            }
                            dichVu.SoTienMienGiam = giamSoTienMienGiam;
                        }
                    }
                }
                else
                {
                    var dichVu =
                        yeuCauTiepNhanChiTiet.YeuCauDichVuKyThuats.FirstOrDefault(x => x.Id == xoaDichVuViewModel.DichVuId);
                    if (dichVu != null)
                    {
                        if (dichVu.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien || dichVu.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                        {
                            throw new ApiException(_localizationService.GetResource("KhamBenhChiDinh.DichVuKyThuat.DaThucHien"));
                        }

                        if (yeuCauTiepNhanChiTiet.YeuCauDuocPhamBenhViens.Any(x => x.YeuCauDichVuKyThuatId == dichVu.Id && x.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy)
                            || yeuCauTiepNhanChiTiet.YeuCauVatTuBenhViens.Any(x => x.YeuCauDichVuKyThuatId == dichVu.Id && x.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy))
                        {
                            throw new ApiException(_localizationService.GetResource("ChiDinh.HuyDichDichVu.DaCoGhiNhanVTTHThuoc"));
                        }
                        //item.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.DaHuy;
                        //item.TrangThaiThanhToan = TrangThaiThanhToan.HuyThanhToan;
                        dichVu.WillDelete = true;
                        if (!string.IsNullOrEmpty(xoaDichVuViewModel.LyDoHuyDichVu))
                        {
                            if (dichVu.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                            {
                                throw new ApiException(_localizationService.GetResource("ChiDinh.HuyDichDichVu.DaThanhToan"));
                            }
                            dichVu.NhanVienHuyDichVuId = _userAgentHelper.GetCurrentUserId();
                            dichVu.LyDoHuyDichVu = xoaDichVuViewModel.LyDoHuyDichVu;
                        }
                        else
                        {
                            if (dichVu.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan && lstDichVuKyThuatCoChi.Contains(xoaDichVuViewModel.DichVuId)) // dichVu.TaiKhoanBenhNhanChis.Any())
                            {
                                throw new ApiException(_localizationService.GetResource("ChiDinh.XoaDichDichVu.DaHuyThanhToan"));
                            }
                        }

                        //BVHD-3825
                        var mienGiam = dichVu.MienGiamChiPhis.FirstOrDefault(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true));
                        if (mienGiam != null)
                        {
                            mienGiam.DaHuy = true;
                            mienGiam.WillDelete = true;

                            var giamSoTienMienGiam = dichVu.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                            if (giamSoTienMienGiam < 0)
                            {
                                giamSoTienMienGiam = 0;
                            }
                            dichVu.SoTienMienGiam = giamSoTienMienGiam;
                        }
                    }
                }
            }

            await _tiepNhanBenhNhanServiceService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            chiDinhDichVuResultVo.SoDuTaiKhoan = await _taikhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taikhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            return chiDinhDichVuResultVo;
        }

        //todo: update theo logic mới
        [HttpPost("XoaDichVuVatTuTieuHao")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.KhamBenh, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult> XoaDichVuVatTuTieuHao(long id) //todo: cần update lại
        {

            var entity = await _yeuCauVatTuBenhVienService.GetByIdAsync(id);
            //if (entity == null || (entity.KhongTinhPhi != true && (entity.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan || entity.TrangThaiThanhToan == TrangThaiThanhToan.CapNhatThanhToan)))
            //    throw new ApiException(_localizationService.GetResource("KhamBenhChiDinh.VatTu.DaThanhToan"));
            if (entity.KhongTinhPhi == true && entity.TrangThai == EnumYeuCauVatTuBenhVien.DaThucHien)
            {
                throw new ApiException(_localizationService.GetResource("KhamBenhChiDinh.VatTu.DaThucHien"));
            }
            var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(entity.YeuCauTiepNhanId);
            foreach (var item in yeuCauTiepNhanChiTiet.YeuCauVatTuBenhViens)
            {
                if (item.Id == id)
                {
                    //item.TrangThai = EnumYeuCauVatTuBenhVien.DaHuy;
                    //item.TrangThaiThanhToan = TrangThaiThanhToan.HuyThanhToan;
                    item.WillDelete = true;
                    break;
                }
            }

            await _tiepNhanBenhNhanServiceService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            return NoContent();
        }

        //todo: update theo logic mới
        [HttpPost("XoaYeuCauDuocPham")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.KhamBenh, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult> XoaYeuCauDuocPham(long id) //todo: cần update
        {
            var yeuCauDuocPham = await _yeuCauDuocPhamBenhVienService.GetByIdAsync(id);
            //if (yeuCauDuocPham == null || (yeuCauDuocPham.KhongTinhPhi != true && (yeuCauDuocPham.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || yeuCauDuocPham.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)))
            //{
            //    throw new ApiException(_localizationService.GetResource("KhamBenhChiDinh.DuocPham.DaThanhToan"));
            //}
            if (yeuCauDuocPham.KhongTinhPhi == true && yeuCauDuocPham.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien)
            {
                throw new ApiException(_localizationService.GetResource("KhamBenhChiDinh.DuocPham.DaThucHien"));
            }
            //var error = await _yeuCauDuocPhamBenhVienService.XoaYeuCauDuocPhamBenhVien(id);
            //if (!string.IsNullOrEmpty(error))
            //    throw new ApiException(_localizationService.GetResource(error));

            var yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauDuocPham.YeuCauTiepNhanId);
            foreach (var item in yeuCauTiepNhanChiTiet.YeuCauDuocPhamBenhViens)
            {
                if (item.Id == id)
                {
                    //TODO update entity kho on 9/9/2020
                    //item.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= item.XuatKhoDuocPhamChiTietViTri.SoLuongXuat;
                    //item.WillDelete = true;
                    item.TrangThai = EnumYeuCauDuocPhamBenhVien.DaHuy;
                    item.TrangThaiThanhToan = TrangThaiThanhToan.HuyThanhToan;
                    //TODO update entity kho on 9/9/2020
                    //item.XuatKhoDuocPhamChiTietViTri.WillDelete = true; //todo: cần kiểm tra lại
                    //item.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.WillDelete = true;
                    //item.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.WillDelete = true;
                    item.WillDelete = true;
                    break;
                }
            }

            await _tiepNhanBenhNhanServiceService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);
            return NoContent();
        }

        [HttpPost("XoaDichVuGiuongBenh")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.KhamBenh, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> XoaDichVuGiuongBenh(XoaChiDinhDichVuViewModel xoaDichVuViewModel)
        {
            await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(xoaDichVuViewModel.YeuCauKhamBenhId);
            var entity = await _yeuCauDichVuGiuongBenhService.GetByIdAsync(xoaDichVuViewModel.DichVuId);
            if (entity.TrangThai == EnumTrangThaiGiuongBenh.DaThucHien || entity.TrangThai == EnumTrangThaiGiuongBenh.DaThucHien)
            {
                throw new ApiException(_localizationService.GetResource("KhamBenhChiDinh.DichVuGiuongBenh.DaThucHien"));
            }

            var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(entity.YeuCauTiepNhanId);
            foreach (var item in yeuCauTiepNhanChiTiet.YeuCauDichVuGiuongBenhViens)
            {
                if (item.Id == xoaDichVuViewModel.DichVuId)
                {
                    //item.TrangThai = EnumTrangThaiGiuongBenh.DaHuy;
                    //item.TrangThaiThanhToan = TrangThaiThanhToan.HuyThanhToan;
                    item.WillDelete = true;
                    //foreach (var giuongBenh in item.HoatDongGiuongBenhs)
                    //{
                    //    giuongBenh.WillDelete = true;
                    //}
                    break;
                }
            }

            await _tiepNhanBenhNhanServiceService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            chiDinhDichVuResultVo.SoDuTaiKhoan = await _taikhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taikhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            return chiDinhDichVuResultVo;
        }

        #endregion

        #region Update NoiChiDinh/NoiThucHien

        [HttpPost("UpdateNoiChiDinhGiuongBenh")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.KhamBenh)]
        public async Task<ActionResult> UpdateNoiChiDinhGiuongBenh(NoiChiDinhUpdate noiChiDinhUpdate)
        {
            var entity = await _yeuCauDichVuGiuongBenhService.GetByIdAsync(noiChiDinhUpdate.YeuCauId);
            entity.NoiThucHienId = noiChiDinhUpdate.KhoaPhongId;
            await _yeuCauDichVuGiuongBenhService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpPost("UpdateNoiChiDinhDuocPham")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.KhamBenh)]
        public async Task<ActionResult> UpdateNoiChiDinhDuocPham(long id, long khoaPhongId, long bacSiId)
        {
            var entity = await _yeuCauDuocPhamBenhVienService.GetByIdAsync(id);
            entity.NoiCapThuocId = khoaPhongId;
            entity.NhanVienCapThuocId = bacSiId;
            await _yeuCauDuocPhamBenhVienService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpPost("UpdateNoiChiDinhVatTuTieuHao")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.KhamBenh)]
        public async Task<ActionResult> UpdateNoiChiDinhVatTuTieuHao(long id, long khoaPhongId, long bacSiId)
        {
            var entity = await _yeuCauVatTuBenhVienService.GetByIdAsync(id);
            entity.NoiCapVatTuId = khoaPhongId;
            entity.NhanVienCapVatTuId = bacSiId;
            await _yeuCauVatTuBenhVienService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpPost("UpdateNoiChiDinhKyThuat")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.KhamBenh)]
        public async Task<ActionResult> UpdateNoiChiDinhKyThuat([FromBody] NoiChiDinhUpdate noiChiDinhUpdate)
        {
            var entity = await _yeuCauDichVuKyThuatService.GetByIdAsync(noiChiDinhUpdate.YeuCauId);
            entity.NoiThucHienId = noiChiDinhUpdate.KhoaPhongId;
            //entity.NhanVienThucHienId = bacSiId;
            await _yeuCauDichVuKyThuatService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpPost("UpdateNoiChiDinhKhamBenh")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.KhamBenh, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult> UpdateNoiChiDinhKhamBenh(long id, long khoaPhongId, long bacSiId)  // hàm này hiện tại ko sử dụng
        {
            var entity = await _yeuCauKhamBenhService.GetByIdAsync(id);
            entity.NoiThucHienId = khoaPhongId;
            entity.BacSiThucHienId = bacSiId;
            await _yeuCauKhamBenhService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpPost("CapNhatGridItemDichVuKyThuat")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> CapNhatGridItemDichVuKyThuatAsync(GridItemYeuCauDichVuKyThuatViewModel viewModel)
        {
            if (!viewModel.IsKhamDoanTatCa)
            {
                if (viewModel.IsKhamBenhDangKham)
                {
                    await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(viewModel.YeuCauKhamBenhId);
                }
                else
                {
                    await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(viewModel.YeuCauKhamBenhId);
                }
            }
            //var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(viewModel.YeuCauTiepNhanId);
            //var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanKhiXoaDichVuNgoaiTruByIdAsync(viewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet = new YeuCauTiepNhan();
            if (viewModel.IsDichVuKham != true
                && (viewModel.IsUpdateNoiThucHien || viewModel.IsUpdateNguoiThucHien || viewModel.IsUpdateBenhPhamXetNghiem))
            {
                yeuCauTiepNhanChiTiet = _yeuCauTiepNhanService.GetById(viewModel.YeuCauTiepNhanId,
                    x => x.Include(a => a.YeuCauDichVuKyThuats));
            }
            else
            {
                yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanKhiXoaDichVuNgoaiTruByIdAsync(viewModel.YeuCauTiepNhanId);
            }

            var flagUpdate = false;
            if (viewModel.IsDichVuKham != true)
            {
                foreach (var item in yeuCauTiepNhanChiTiet.YeuCauDichVuKyThuats)
                {
                    if (item.Id == viewModel.YeuCauDichVuKyThuatId)
                    {
                        flagUpdate = true;

                        long? yeuCauGoiDichVuKhuyenMaiId = null;
                        if (viewModel.IsUpdateLoaiGia)
                        {
                            if (viewModel.NhomGiaDichVuKyThuatBenhVienId != null && item.NhomGiaDichVuKyThuatBenhVienId != viewModel.NhomGiaDichVuKyThuatBenhVienId)
                            {
                                //if (item.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                                //{
                                //    throw new ApiException(_localizationService.GetResource("ChiDinh.LoaiGia.DaThanhToan"));
                                //}

                                item.NhomGiaDichVuKyThuatBenhVienId = viewModel.NhomGiaDichVuKyThuatBenhVienId.Value;
                                var donGiaBenhVien = await _khamBenhService.GetDonGiaBenhVienDichVuKyThuatAsync(item.DichVuKyThuatBenhVienId, viewModel.NhomGiaDichVuKyThuatBenhVienId.Value);
                                if (donGiaBenhVien == 0)
                                {
                                    throw new ApiException(_localizationService.GetResource("ChiDinh.LoaiGia.NotExists"));
                                }

                                //BVHD-3825
                                // kiểm tra nếu là dịch vụ khuyến mãi từ gói marketing
                                if (item.MienGiamChiPhis.Any(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true)))
                                {
                                    viewModel.IsSwapDichVuKhuyenMai = true;
                                    viewModel.LaDichVuKhuyenMai = true;
                                    yeuCauGoiDichVuKhuyenMaiId = item.MienGiamChiPhis
                                        .First(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true))
                                        .YeuCauGoiDichVuId.Value;
                                }

                                item.Gia = donGiaBenhVien;
                            }
                        }

                        if (viewModel.IsUpdateSoLan)
                        {
                            if (viewModel.SoLan == null || viewModel.SoLan == 0)
                            {
                                throw new ApiException(_localizationService.GetResource("DichVuKyThuat.SoLan.Range"));
                            }

                            // kiểm tra nếu là dịch vụ chỉ đinh từ gói marketing
                            if (item.YeuCauGoiDichVuId != null)
                            {
                                var soLuongConLai = await _khamBenhService.GetSoLuongConLaiDichVuKyThuatTrongGoiMarketingBenhNhanAsync(item.YeuCauGoiDichVuId.Value, item.DichVuKyThuatBenhVienId);
                                var soLuongKhaDung = soLuongConLai + item.SoLan;
                                if (soLuongKhaDung < viewModel.SoLan)
                                {
                                    throw new ApiException(string.Format(_localizationService.GetResource("DichVuKyThuat.SoLanConLaiTrongGoi.Range"), item.TenDichVu, soLuongKhaDung));
                                }
                            }

                            // kiểm tra nếu là dịch vụ khuyến mãi từ gói marketing
                            if (item.MienGiamChiPhis.Any(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true)))
                            {
                                var yeuCauGoiId = item.MienGiamChiPhis
                                    .First(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true))
                                    .YeuCauGoiDichVuId.Value;
                                var soLuongConLai = await _khamBenhService.GetSoLuongConLaiDichVuKyThuatKhuyenMaiTrongGoiMarketingBenhNhanAsync(yeuCauGoiId, item.DichVuKyThuatBenhVienId);
                                var soLuongKhaDung = soLuongConLai + item.SoLan;
                                if (soLuongKhaDung < viewModel.SoLan)
                                {
                                    throw new ApiException(string.Format(_localizationService.GetResource("DichVuKyThuat.SoLanConLaiTrongGoi.Range"), item.TenDichVu, soLuongKhaDung));
                                }

                                viewModel.IsSwapDichVuKhuyenMai = true;
                                viewModel.LaDichVuKhuyenMai = true;
                                yeuCauGoiDichVuKhuyenMaiId = yeuCauGoiId;
                            }

                            item.SoLan = viewModel.SoLan != null ? viewModel.SoLan.Value : item.SoLan;
                        }

                        if (viewModel.IsUpdateNoiThucHien)
                        {
                            if (viewModel.NoiThucHienId == null || viewModel.NoiThucHienId == 0)
                            {
                                throw new ApiException(_localizationService.GetResource("ycdvcls.NoiThucHienId.Required"));
                            }

                            var queryInfo = new DropDownListRequestModel()
                            {
                                //Id = viewModel.NoiThucHienId.Value,
                                ParameterDependencies = "{NoiThucHienId:" + viewModel.NoiThucHienId.Value + "}",
                                Take = 50
                            };

                            var lstBacSiKhams = await _yeuCauKhamBenhService.GetBacSiKhams(queryInfo);
                            //if (!lstBacSiKhams.Any())
                            //{
                            //    throw new ApiException(_localizationService.GetResource("ycdvcls.NoiThucHienId.BaSiNotExists"));
                            //}

                            item.NoiThucHienId = viewModel.NoiThucHienId;

                            // nếu nơi thực hiện mới ko có bác sĩ đang chọn, thì tự dộng chọn bác sĩ đầu tiên
                            if (lstBacSiKhams.Any() && !lstBacSiKhams.Any(x => x.KeyId == viewModel.NguoiThucHienId))
                            {
                                item.NhanVienThucHienId = lstBacSiKhams.First().KeyId;
                            }
                            else
                            {
                                item.NhanVienThucHienId = null;
                            }
                        }

                        if (viewModel.IsUpdateNguoiThucHien)
                        {
                            //if (viewModel.NguoiThucHienId == null || viewModel.NguoiThucHienId == 0)
                            //{
                            //    throw new ApiException(_localizationService.GetResource("ycdvcls.NhanVienThucHienId.Required"));
                            //}
                            item.NhanVienThucHienId = viewModel.NguoiThucHienId;
                        }
                        //item.NoiThucHienId = viewModel.NoiThucHienId != null ? viewModel.NoiThucHienId.Value : item.NoiThucHienId;

                        if (viewModel.IsUpdateBenhPhamXetNghiem)
                        {
                            item.BenhPhamXetNghiem = viewModel.BenhPhamXetNghiem;
                        }

                        if (viewModel.IsSwapDichVuGoi)
                        {
                            if (viewModel.LaDichVuTrongGoi == true)
                            {
                                var thongTin = new ThongTinDichVuTrongGoi()
                                {
                                    BenhNhanId = yeuCauTiepNhanChiTiet.BenhNhanId.Value,
                                    DichVuId = item.DichVuKyThuatBenhVienId,
                                    NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                                    SoLuong = item.SoLan
                                };
                                await _khamBenhService.GetYeuCauGoiDichVuTheoDichVuChiDinhAsync(thongTin);
                                item.Gia = thongTin.DonGia;
                                item.DonGiaTruocChietKhau = thongTin.DonGiaTruocChietKhau;
                                item.DonGiaSauChietKhau = thongTin.DonGiaSauChietKhau;
                                item.YeuCauGoiDichVuId = thongTin.YeuCauGoiDichVuId;

                                if (item.MienGiamChiPhis.Any(x => x.DaHuy != true))
                                {
                                    item.SoTienBaoHiemTuNhanChiTra = null;
                                    item.SoTienMienGiam = null;
                                    foreach (var mienGiam in item.MienGiamChiPhis.Where(a => a.DaHuy != true))
                                    {
                                        mienGiam.WillDelete = true;
                                    }

                                    foreach (var congNo in item.CongTyBaoHiemTuNhanCongNos.Where(a => a.DaHuy != true))
                                    {
                                        congNo.WillDelete = true;
                                    }
                                }
                            }
                            else
                            {
                                item.DonGiaTruocChietKhau = null;
                                item.DonGiaSauChietKhau = null;
                                item.YeuCauGoiDichVuId = null;

                                var donGiaBenhVien = await _khamBenhService.GetDonGiaBenhVienDichVuKyThuatAsync(item.DichVuKyThuatBenhVienId, item.NhomGiaDichVuKyThuatBenhVienId);
                                if (donGiaBenhVien == 0)
                                {
                                    throw new ApiException(_localizationService.GetResource("ChiDinh.LoaiGia.NotExists"));
                                }

                                item.Gia = donGiaBenhVien;
                            }
                        }

                        //BVHD-3825
                        if (viewModel.IsSwapDichVuKhuyenMai)
                        {
                            if (item.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                            {
                                throw new ApiException(_localizationService.GetResource("DichVuKhuyenMai.TrangThaiYeuCauDichVu.DaThanhToan"));
                            }

                            if (viewModel.LaDichVuKhuyenMai == true)
                            {
                                var thongTin = new ThongTinDichVuTrongGoi()
                                {
                                    BenhNhanId = yeuCauTiepNhanChiTiet.BenhNhanId.Value,
                                    DichVuId = item.DichVuKyThuatBenhVienId,
                                    NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                                    SoLuong = item.SoLan,
                                    NhomGiaId = item.NhomGiaDichVuKyThuatBenhVienId
                                };

                                //dùng cho trường hợp cập nhật số lượng hoặc loại giá
                                if (viewModel.IsUpdateLoaiGia || viewModel.IsUpdateSoLan)
                                {
                                    thongTin.YeuCauDichVuCapNhatSoLuongLoaiGiaId = item.Id;
                                    thongTin.YeucauGoiDichVuKhuyenMaiId = yeuCauGoiDichVuKhuyenMaiId;
                                }

                                await _tiepNhanBenhNhanService.GetYeuCauGoiDichVuKhuyenMaiTheoDichVuChiDinhAsync(thongTin);

                                if (item.MienGiamChiPhis.Any(x => x.DaHuy != true && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true) && x.YeuCauGoiDichVuId != null))
                                {
                                    //item.SoTienBaoHiemTuNhanChiTra = null;
                                    //item.SoTienMienGiam = null;
                                    foreach (var mienGiam in item.MienGiamChiPhis.Where(a => a.DaHuy != true && (a.TaiKhoanBenhNhanThuId == null || a.TaiKhoanBenhNhanThu.DaHuy != true) && a.YeuCauGoiDichVuId != null))
                                    {
                                        mienGiam.DaHuy = true;
                                        mienGiam.WillDelete = true;

                                        var giamSoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                                        if (giamSoTienMienGiam < 0)
                                        {
                                            giamSoTienMienGiam = 0;
                                        }
                                        item.SoTienMienGiam = giamSoTienMienGiam;
                                    }

                                    //foreach (var congNo in item.CongTyBaoHiemTuNhanCongNos.Where(a => a.DaHuy != true))
                                    //{
                                    //    congNo.WillDelete = true;
                                    //}
                                }

                                //trường hợp cập nhật số lượng hoặc loại giá thì giữ nguyên đơn giá chỉ định trước đó
                                if (!viewModel.IsUpdateLoaiGia && !viewModel.IsUpdateSoLan)
                                {
                                    item.Gia = thongTin.DonGia;
                                }
                                var thanhTien = item.SoLan * item.Gia;
                                var thanhTienMienGiam = item.SoLan * thongTin.DonGiaKhuyenMai.Value;

                                var tongTienMienGiam = (thanhTien > thanhTienMienGiam) ? (thanhTien - thanhTienMienGiam) : 0;
                                item.SoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault() + tongTienMienGiam;
                                item.MienGiamChiPhis.Add(new MienGiamChiPhi()
                                {
                                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                                    LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                                    LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoSoTien,
                                    SoTien = tongTienMienGiam,
                                    YeuCauGoiDichVuId = thongTin.YeuCauGoiDichVuId
                                });
                            }
                            else
                            {
                                var donGiaBenhVien = await _khamBenhService.GetDonGiaBenhVienDichVuKyThuatAsync(item.DichVuKyThuatBenhVienId, item.NhomGiaDichVuKyThuatBenhVienId);
                                if (donGiaBenhVien == 0)
                                {
                                    throw new ApiException(_localizationService.GetResource("ChiDinh.LoaiGia.NotExists"));
                                }

                                item.Gia = donGiaBenhVien;
                                var mienGiam = item.MienGiamChiPhis.FirstOrDefault(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true));
                                if (mienGiam != null)
                                {
                                    mienGiam.DaHuy = true;
                                    mienGiam.WillDelete = true;

                                    var giamSoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                                    if (giamSoTienMienGiam < 0)
                                    {
                                        giamSoTienMienGiam = 0;
                                    }
                                    item.SoTienMienGiam = giamSoTienMienGiam;
                                }
                            }
                        }
                        break;
                    }
                }
            }
            else
            {
                foreach (var item in yeuCauTiepNhanChiTiet.YeuCauKhamBenhs)
                {
                    if (item.Id == viewModel.YeuCauDichVuKyThuatId) // gán tạm id yêu cầu khám bằng biến này truyền từ FE
                    {
                        flagUpdate = true;
                        if (viewModel.IsSwapDichVuGoi)
                        {
                            if (viewModel.LaDichVuTrongGoi == true)
                            {
                                var thongTin = new ThongTinDichVuTrongGoi()
                                {
                                    BenhNhanId = yeuCauTiepNhanChiTiet.BenhNhanId.Value,
                                    DichVuId = item.DichVuKhamBenhBenhVienId,
                                    NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
                                    SoLuong = 1
                                };
                                await _khamBenhService.GetYeuCauGoiDichVuTheoDichVuChiDinhAsync(thongTin);
                                item.Gia = thongTin.DonGia;
                                item.DonGiaTruocChietKhau = thongTin.DonGiaTruocChietKhau;
                                item.DonGiaSauChietKhau = thongTin.DonGiaSauChietKhau;
                                item.YeuCauGoiDichVuId = thongTin.YeuCauGoiDichVuId;

                                if (item.MienGiamChiPhis.Any(x => x.DaHuy != true))
                                {
                                    item.SoTienBaoHiemTuNhanChiTra = null;
                                    item.SoTienMienGiam = null;
                                    foreach (var mienGiam in item.MienGiamChiPhis.Where(a => a.DaHuy != true))
                                    {
                                        mienGiam.WillDelete = true;
                                    }

                                    foreach (var congNo in item.CongTyBaoHiemTuNhanCongNos.Where(a => a.DaHuy != true))
                                    {
                                        congNo.WillDelete = true;
                                    }
                                }
                            }
                            else
                            {
                                item.DonGiaTruocChietKhau = null;
                                item.DonGiaSauChietKhau = null;
                                item.YeuCauGoiDichVuId = null;

                                var donGiaBenhVien = await _khamBenhService.GetDonGiaBenhVienDichVuKhamBenhAsync(item.DichVuKhamBenhBenhVienId, item.NhomGiaDichVuKhamBenhBenhVienId);
                                if (donGiaBenhVien == 0)
                                {
                                    throw new ApiException(_localizationService.GetResource("ChiDinh.LoaiGia.NotExists"));
                                }

                                item.Gia = donGiaBenhVien;
                            }
                        }

                        //BVHD-3825
                        if (viewModel.IsSwapDichVuKhuyenMai)
                        {
                            if (item.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                            {
                                throw new ApiException(_localizationService.GetResource("DichVuKhuyenMai.TrangThaiYeuCauDichVu.DaThanhToan"));
                            }

                            if (viewModel.LaDichVuKhuyenMai == true)
                            {
                                var thongTin = new ThongTinDichVuTrongGoi()
                                {
                                    BenhNhanId = yeuCauTiepNhanChiTiet.BenhNhanId.Value,
                                    DichVuId = item.DichVuKhamBenhBenhVienId,
                                    NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKhamBenh,
                                    SoLuong = 1,
                                    NhomGiaId = item.NhomGiaDichVuKhamBenhBenhVienId
                                };
                                await _tiepNhanBenhNhanService.GetYeuCauGoiDichVuKhuyenMaiTheoDichVuChiDinhAsync(thongTin);

                                if (item.MienGiamChiPhis.Any(x => x.DaHuy != true && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true) && x.YeuCauGoiDichVuId != null))
                                {
                                    //item.SoTienBaoHiemTuNhanChiTra = null;
                                    //item.SoTienMienGiam = null;
                                    foreach (var mienGiam in item.MienGiamChiPhis.Where(a => a.DaHuy != true && (a.TaiKhoanBenhNhanThuId == null || a.TaiKhoanBenhNhanThu.DaHuy != true) && a.YeuCauGoiDichVuId != null))
                                    {
                                        mienGiam.DaHuy = true;
                                        mienGiam.WillDelete = true;

                                        var giamSoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                                        if (giamSoTienMienGiam < 0)
                                        {
                                            giamSoTienMienGiam = 0;
                                        }
                                        item.SoTienMienGiam = giamSoTienMienGiam;
                                    }

                                    //foreach (var congNo in item.CongTyBaoHiemTuNhanCongNos.Where(a => a.DaHuy != true))
                                    //{
                                    //    congNo.WillDelete = true;
                                    //}
                                }

                                item.Gia = thongTin.DonGia;
                                var thanhTien = item.Gia;
                                var thanhTienMienGiam = thongTin.DonGiaKhuyenMai.Value;

                                var tongTienMienGiam = (thanhTien > thanhTienMienGiam) ? (thanhTien - thanhTienMienGiam) : 0;
                                item.SoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault() + tongTienMienGiam;
                                item.MienGiamChiPhis.Add(new MienGiamChiPhi()
                                {
                                    YeuCauTiepNhanId = item.YeuCauTiepNhanId,
                                    LoaiMienGiam = Enums.LoaiMienGiam.MienGiamThem,
                                    LoaiChietKhau = Enums.LoaiChietKhau.ChietKhauTheoSoTien,
                                    SoTien = tongTienMienGiam,
                                    YeuCauGoiDichVuId = thongTin.YeuCauGoiDichVuId
                                });
                            }
                            else
                            {
                                var donGiaBenhVien = await _khamBenhService.GetDonGiaBenhVienDichVuKhamBenhAsync(item.DichVuKhamBenhBenhVienId, item.NhomGiaDichVuKhamBenhBenhVienId);
                                if (donGiaBenhVien == 0)
                                {
                                    throw new ApiException(_localizationService.GetResource("ChiDinh.LoaiGia.NotExists"));
                                }

                                item.Gia = donGiaBenhVien;
                                var mienGiam = item.MienGiamChiPhis.FirstOrDefault(x => x.DaHuy != true && x.YeuCauGoiDichVuId != null && (x.TaiKhoanBenhNhanThuId == null || x.TaiKhoanBenhNhanThu.DaHuy != true));
                                if (mienGiam != null)
                                {
                                    mienGiam.WillDelete = true;

                                    var giamSoTienMienGiam = item.SoTienMienGiam.GetValueOrDefault() - mienGiam.SoTien;
                                    if (giamSoTienMienGiam < 0)
                                    {
                                        giamSoTienMienGiam = 0;
                                    }
                                    item.SoTienMienGiam = giamSoTienMienGiam;
                                }
                            }
                        }
                        break;
                    }
                }
            }

            if (!flagUpdate)
            {
                throw new ApiException(_localizationService.GetResource("ApiError.EntityNull"));
            }

            //if (viewModel.IsUpdateNguoiThucHien || viewModel.IsUpdateNoiThucHien)
            //{
            //    await _yeuCauTiepNhanService.UpdateAsync(yeuCauTiepNhanChiTiet);
            //}
            //else
            //{

            // update logic khi cập nhật nơi thực hiện dv chỉ định, sẽ cập nhật lại hàng đợi tương ứng
            await _tiepNhanBenhNhanServiceService.PrepareForEditDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            //}
            //return yeuCauKhamBenhDangKham;
            //return Ok();

            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            chiDinhDichVuResultVo.SoDuTaiKhoan = await _taikhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taikhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            return chiDinhDichVuResultVo;
        }

        #endregion

        #region Get lookup

        [HttpPost("GetListNhomDichVuKyThuat")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListNhomDichVuKyThuat([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khamBenhService.GetListNhomDichVuKyThuat(model);
            return Ok(lookup);
        }

        [HttpPost("GetListNhomDichVuBenhVien")]
        public async Task<ActionResult<ICollection<NhomDichVuBenhVienTreeViewVo>>> GetListNhomDichVuBenhVienAsync([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khamBenhService.GetListNhomDichVuBenhVienAsync(model);
            return Ok(lookup);
        }

        [HttpPost("GetListDichVuKyThuat")] // hàm này hiện tại không dùng, nếu ai dùng thì phải qua khám bệnh chỉ định dvkt tham khảo
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListDichVuKyThuat([FromBody]DropDownListRequestModel model)
        {
            var nhomDVKTId = CommonHelper.GetIdFromRequestDropDownList(model);
            var lookup = await _khamBenhService.GetListDichVuKyThuat(nhomDVKTId, model);
            return Ok(lookup);

        }

        [HttpPost("GetListDichVuKyThuatMultiSelect")]
        public async Task<ActionResult<ICollection<DichVuKyThuatBenhVienMultiSelectTemplateVo>>> GetListDichVuKyThuatMultiSelectAsync([FromBody]MultiselectQueryInfo model, bool isPTTT, bool isPhieuDieuTri = false)
        {
            var lookup = await _khamBenhService.GetListDichVuKyThuatMultiSelectAsync(model, isPTTT, isPhieuDieuTri);
            return Ok(lookup);
        }

        [HttpPost("GetListDichVuKhamBenh")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListDichVuKhamBenh([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khamBenhService.GetListDichVuKhamBenh(model);
            return Ok(lookup);
        }

        [HttpPost("GetDichVuGiuongBenhVien")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetDichVuGiuongBenhVien([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khamBenhService.GetListDichVuGiuongBenhVien(model);
            return Ok(lookup);
        }

        [HttpPost("GetVatTuYTeBenhVien")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetVatTuYTeBenhVien([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khamBenhService.GetListVatTuYTeBenhVien(model);
            return Ok(lookup);
        }


        [HttpPost("GetListDuocPhamBenhVien")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListDuocPhamBenhVien([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khamBenhService.GetListDuocPhamBenhVien(model);
            return Ok(lookup);
        }


        [HttpGet("GetDataLoaiGiaDichVu")]
        public ActionResult<LookupItemVo> GetDataLoaiGiaDichVu()
        {
            var lookup = _khamBenhService.GetDataLoaiGiaDichVu();
            return Ok(lookup);

        }

        [HttpGet("GetDataNoiThucHienDichVu")]
        public ActionResult<LookupItemVo> GetDataNoiThucHienDichVu()
        {
            var lookup = _khamBenhService.GetDataNoiThucHienDichVu();
            //lookup.Insert(0, new LookupItemVo { DisplayName = "Chọn nơi thực hiện", KeyId = 0 });
            return Ok(lookup);

        }


        [HttpPost("GetPhongThucHienChiDinhGiuong")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetPhongThucHienChiDinhGiuong([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khamBenhService.GetPhongThucHienChiDinhGiuong(model);
            return Ok(lookup);

        }

        [HttpPost("GetAllPhongBenhVienDangHoatDong")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetAllPhongBenhVienDangHoatDongAsync([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khamBenhService.GetAllPhongBenhVienDangHoatDongAsync(model);
            return Ok(lookup);

        }

        [HttpPost("GetListGiuongBenhTheoPhong")]
        public async Task<ActionResult<ICollection<LookupItemGiuongBenhVo>>> GetListGiuongBenhTheoPhongAsync([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khamBenhService.GetListGiuongBenhTheoPhongAsync(model);
            return Ok(lookup);
        }

        [HttpPost("GetPhongThucHienChiDinhDuocOrVatTu")]
        public async Task<ActionResult<ICollection<PhongKhamTemplateVo>>> GetPhongThucHienChiDinhDuocOrVatTu(DropDownListRequestModel model)
        {
            var lookup = await _khamBenhService.GetPhongThucHienChiDinhDuocOrVatTu(model);
            return Ok(lookup);
        }


        [HttpPost("GetPhongThucHienChiDinhKhamOrDichVuKyThuat")]
        public async Task<ActionResult<ICollection<PhongKhamTemplateVo>>> GetPhongThucHienChiDinhKhamOrDichVuKyThuat(DropDownListRequestModel model)
        {
            var lookup = await _khamBenhService.GetPhongThucHienChiDinhKhamOrDichVuKyThuat(model);
            return Ok(lookup);
        }




        [HttpPost("GetGoiCoChietKhau")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetGoiCoChietKhau([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khamBenhService.GetGoiChietKhau(model, true);
            return Ok(lookup);

        }

        [HttpPost("GetGoiKhongChietKhau")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetGoiKhongChietKhau([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khamBenhService.GetGoiChietKhau(model, false);
            return Ok(lookup);
        }

        [HttpPost("GetListBacSiThucHien")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListBacSiThucHienAsync([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khamBenhService.GetListBacSiThucHienAsync(model);
            return Ok(lookup);
        }

        [HttpPost("GetListDichVuTheoGoi")]
        public async Task<ActionResult<ICollection<DichVuTheoGoiVo>>> GetListDichVuTheoGoiAsync([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khamBenhService.GetListDichVuTheoGoiAsync(model);
            return Ok(lookup);
        }

        [HttpGet("GetListIdDichVuTheoGoi")]
        public async Task<ActionResult<ICollection<LookupCheckItemVo>>> GetListIdDichVuTheoGoiAsync(long goiDichVuId)
        {
            var lookup = await _khamBenhService.GetListIdDichVuTheoGoiAsync(goiDichVuId);
            return Ok(lookup);
        }

        [HttpPost("GetNhomGiaTheoLoaiDichVuKyThuat")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetNhomGiaTheoLoaiDichVuKyThuatAsync([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khamBenhService.GetNhomGiaTheoLoaiDichVuKyThuatAsync(model);
            return Ok(lookup);
        }
        
        [HttpPost("GetGhiChuDichVuCanLamSangs")]
        public async Task<ActionResult<ICollection<string>>> GetGhiChuDichVuCanLamSangsAsync([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khamBenhService.GetGhiChuDichVuCanLamSangsAsync(model);
            return Ok(lookup);
        }
        #endregion KhamBenh LS/CLS lookup 

        #region InPhieuChiDinh

        [HttpPost("InPhieuChiDinhh")]
        public ActionResult<string> InPhieuChiDinhh([FromBody] InPhieuChiDinh inPhieuChiDinh)
        {
            var hostingName = inPhieuChiDinh.Hosting;
            // nên check truyền lại input
            var htmlBangThuTienThuoc = _khamBenhService.InBaoCaoChiDinh(inPhieuChiDinh.YeuCauTiepNhanId, 
                inPhieuChiDinh.YeuCauKhamBenhId,
                hostingName, 
                inPhieuChiDinh.ListDichVuChiDinh
                , inPhieuChiDinh.InChungChiDinh, 
                inPhieuChiDinh.KieuInChung,
                inPhieuChiDinh.GhiChuCanLamSang, 
                inPhieuChiDinh.IsKhamDoanTatCa,
                inPhieuChiDinh.InDichVuBacSiChiDinh);
            return htmlBangThuTienThuoc;
        }


        #endregion

        #region Private funciton
        // TODO: cần kiểm tra tài khoản thanh toán vs insert Goi Dich Vu, Gia Bao Hiem ..
        private async Task<YeuCauTiepNhan> InsertGoiDichVuKhongChietKhauAsync(List<GoiDichVuChiDinhGridVo> listGoiDV, long yeuCauTiepNhan, long yeuCauKhamBenhId)
        {
            //try
            //{
            var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauTiepNhan);

            long? userId = _userAgentHelper.GetCurrentUserId();
            var entity = _yeuCauTiepNhanService.GetById(yeuCauTiepNhan,
                x => x.Include(p => p.YeuCauKhamBenhs).Include(p => p.YeuCauDichVuGiuongBenhViens).Include(p => p.YeuCauDichVuKyThuats).Include(p => p.YeuCauVatTuBenhViens)
                      .Include(p => p.YeuCauDuocPhamBenhViens).Include(yctns => yctns.DoiTuongUuDai).ThenInclude(dtud => dtud.DoiTuongUuDaiDichVuKyThuatBenhViens));


            var yeuCauKhamBenhDuocHuongBHYT = entity.YeuCauKhamBenhs.FirstOrDefault(x => x.Id == yeuCauKhamBenhId)?.DuocHuongBaoHiem ?? false;

            foreach (var item in listGoiDV.FirstOrDefault().GoiChietKhaus)
            {
                switch (item.NhomId)
                {
                    case (int)EnumNhomGoiDichVu.DichVuKhamBenh: //todo: hiện tại trong chỉ định ko sử dụng, có thể bỏ
                        var yeucauKham = new KhamBenhYeuCauKhamBenhViewModel();
                        yeucauKham.YeuCauTiepNhanId = yeuCauTiepNhan;
                        yeucauKham.DichVuKhamBenhBenhVienId = item.LoaiYeuCauDichVuId ?? 0;
                        yeucauKham.NhomGiaDichVuKhamBenhBenhVienId = item.NhomGiaDichVuBenhVienId;
                        yeucauKham.MaDichVu = item.Ma;
                        yeucauKham.TenDichVu = item.TenDichVu;
                        yeucauKham.MaDichVuTT37 = item.MaTT37;
                        yeucauKham.Gia = item.DonGia;
                        yeucauKham.DuocHuongBaoHiem = entity.CoBHYT ?? true;
                        yeucauKham.TrangThai = EnumTrangThaiYeuCauKhamBenh.DangKham;
                        yeucauKham.NhanVienChiDinhId = userId;
                        yeucauKham.NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();
                        yeucauKham.ThoiDiemChiDinh = DateTime.Now.Date;
                        yeucauKham.ThoiDiemDangKy = DateTime.Now.Date;
                        yeucauKham.SoLuong = Convert.ToInt32(item.SoLuong);
                        var entityYeuCau = yeucauKham.ToEntity<YeuCauKhamBenh>();
                        yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.Add(entityYeuCau);
                        break;
                    case (int)EnumNhomGoiDichVu.DichVuKyThuat:
                        var yeucauKT = new KhamBenhYeuCauDichVuKyThuatViewModel();
                        var dtudDVKTBV = entity.DoiTuongUuDai?.DoiTuongUuDaiDichVuKyThuatBenhViens?.FirstOrDefault(o => o.DichVuKyThuatBenhVienId == item.LoaiYeuCauDichVuId && o.DichVuKyThuatBenhVien.CoUuDai == true);
                        if (item.DonGiaBaoHiem != null && entity.CoBHYT == true && yeuCauKhamBenhDuocHuongBHYT)
                        {
                            yeucauKT.DuocHuongBaoHiem = true;
                            yeucauKT.BaoHiemChiTra = null;
                            //yeucauKT.GiaBaoHiemThanhToan = item.DonGiaBaoHiem * chiTietThanhToan.TiLeBaoHiemThanhToan * chiTietThanhToan.MucHuong;
                        }
                        else
                        {
                            yeucauKT.DuocHuongBaoHiem = false;
                            yeucauKT.BaoHiemChiTra = null;
                            //yeucauKT.GiaBaoHiemThanhToan = 0;
                        }
                        yeucauKT.TiLeUuDai = dtudDVKTBV?.TiLeUuDai;
                        yeucauKT.TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan;
                        yeucauKT.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
                        yeucauKT.YeuCauTiepNhanId = yeuCauTiepNhan;
                        yeucauKT.DichVuKyThuatBenhVienId = item.LoaiYeuCauDichVuId ?? 0;
                        yeucauKT.NhomGiaDichVuKyThuatBenhVienId = item.NhomGiaDichVuBenhVienId;
                        yeucauKT.MaDichVu = item.Ma;
                        yeucauKT.TenDichVu = item.TenDichVu;
                        yeucauKT.Ma4350DichVu = item.MaTT37;
                        yeucauKT.MaGiaDichVu = item.MaGiaDichVu;
                        yeucauKT.Gia = item.DonGia;
                        yeucauKT.NhanVienChiDinhId = userId;
                        yeucauKT.NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();
                        yeucauKT.DaThanhToan = false;
                        yeucauKT.ThoiDiemChiDinh = DateTime.Now.Date;
                        yeucauKT.ThoiDiemDangKy = DateTime.Now.Date;
                        yeucauKT.SoLan = Convert.ToInt32(item.SoLuong);
                        yeucauKT.LoaiDichVuKyThuat = GetLoaiDichVuKyThuatChiDinh(item.NhomChiPhiDichVuKyThuatId ?? 0);
                        yeucauKT.NoiThucHienId = item.NoiThucHienId;
                        yeucauKT.YeuCauKhamBenhId = yeuCauKhamBenhId;
                        yeucauKT.NhanVienThucHienId = item.NguoiThucHienId;
                        yeucauKT.NhomDichVuBenhVienId = item.NhomDichVuKyThuatBenhVienId.GetValueOrDefault();

                        yeucauKT.DonGiaBaoHiem = item.DonGiaBaoHiem;
                        yeucauKT.TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan;

                        var entityKT = yeucauKT.ToEntity<YeuCauDichVuKyThuat>();
                        yeuCauTiepNhanChiTiet.YeuCauDichVuKyThuats.Add(entityKT);
                        break;
                    case (int)EnumNhomGoiDichVu.DichVuGiuongBenh:
                        var yeucauGiuong = new KhamBenhYeuCauGiuongBenhViewModel();
                        if (item.DonGiaBaoHiem != null && entity.CoBHYT == true && yeuCauKhamBenhDuocHuongBHYT)
                        {
                            yeucauGiuong.DuocHuongBaoHiem = true;
                            yeucauGiuong.BaoHiemChiTra = null;
                            //yeucauGiuong.GiaBaoHiemThanhToan = item.DonGiaBaoHiem * chiTietThanhToan.TiLeBaoHiemThanhToan * chiTietThanhToan.MucHuong;
                        }
                        else
                        {
                            yeucauGiuong.DuocHuongBaoHiem = false;
                            yeucauGiuong.BaoHiemChiTra = null;
                            yeucauGiuong.GiaBaoHiemThanhToan = 0;
                        }
                        yeucauGiuong.YeuCauTiepNhanId = yeuCauTiepNhan;
                        yeucauGiuong.DichVuGiuongBenhVienId = item.LoaiYeuCauDichVuId ?? 0;
                        yeucauGiuong.NhomGiaDichVuGiuongBenhVienId = item.NhomGiaDichVuBenhVienId;
                        yeucauGiuong.Ma = item.Ma;
                        yeucauGiuong.Ten = item.TenDichVu;
                        yeucauGiuong.MaTT37 = item.MaTT37;
                        yeucauGiuong.Gia = item.DonGia;
                        yeucauGiuong.TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan;
                        yeucauGiuong.TrangThai = EnumTrangThaiGiuongBenh.ChuaThucHien;
                        yeucauGiuong.NhanVienChiDinhId = userId;
                        yeucauGiuong.NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();
                        yeucauGiuong.ThoiDiemChiDinh = DateTime.Now.Date;
                        yeucauGiuong.NoiThucHienId = item.NoiThucHienId;
                        yeucauGiuong.GiuongBenhId = item.GiuongBenhId;
                        yeucauGiuong.YeuCauKhamBenhId = yeuCauKhamBenhId;

                        yeucauGiuong.DonGiaBaoHiem = item.DonGiaBaoHiem;
                        yeucauGiuong.TiLeBaoHiemThanhToan = item.TiLeBaoHiemThanhToan;

                        var entityGiuong = yeucauGiuong.ToEntity<YeuCauDichVuGiuongBenhVien>();
                        yeuCauTiepNhanChiTiet.YeuCauDichVuGiuongBenhViens.Add(entityGiuong);
                        break;
                    case (int)EnumNhomGoiDichVu.VatTuTieuHao:
                        var yeucauVatTu = new KhamBenhYeuCauVatTuBenhVienViewModel();
                        yeucauVatTu.YeuCauTiepNhanId = yeuCauTiepNhan;
                        yeucauVatTu.VatTuBenhVienId = item.LoaiYeuCauDichVuId ?? 0;
                        yeucauVatTu.NhomVatTuId = item.NhomGiaDichVuBenhVienId;
                        yeucauVatTu.Ma = item.Ma;
                        yeucauVatTu.Ten = item.TenDichVu;
                        yeucauVatTu.Gia = item.DonGia;
                        yeucauVatTu.DuocHuongBaoHiem = false;
                        yeucauVatTu.BaoHiemChiTra = null;
                        yeucauVatTu.TrangThai = EnumYeuCauVatTuBenhVien.ChuaThucHien;
                        yeucauVatTu.NhanVienChiDinhId = userId;
                        yeucauVatTu.NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();
                        yeucauVatTu.ThoiDiemChiDinh = DateTime.Now.Date;
                        yeucauVatTu.SoLuong = Convert.ToInt32(item.SoLuong);
                        yeucauVatTu.YeuCauKhamBenhId = yeuCauKhamBenhId;
                        var entityVatTu = yeucauVatTu.ToEntity<YeuCauVatTuBenhVien>();
                        yeuCauTiepNhanChiTiet.YeuCauVatTuBenhViens.Add(entityVatTu);
                        break;
                    case (int)EnumNhomGoiDichVu.DuocPham:
                        var yeucauDuocPham = new KhamBenhYeuCauDuocPhamViewModel();
                        yeucauDuocPham.YeuCauTiepNhanId = yeuCauTiepNhan;
                        yeucauDuocPham.DuocPhamBenhVienId = item.LoaiYeuCauDichVuId ?? 0;
                        yeucauDuocPham.Ten = item.TenDichVu;
                        yeucauDuocPham.Gia = item.DonGia;
                        yeucauDuocPham.DuocHuongBaoHiem = false;
                        yeucauDuocPham.BaoHiemChiTra = null;
                        yeucauDuocPham.TrangThai = EnumYeuCauDuocPhamBenhVien.ChuaThucHien;
                        yeucauDuocPham.NhanVienChiDinhId = userId;
                        yeucauDuocPham.NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();
                        yeucauDuocPham.ThoiDiemChiDinh = DateTime.Now.Date;
                        yeucauDuocPham.SoLuong = Convert.ToInt32(item.SoLuong);
                        yeucauDuocPham.YeuCauKhamBenhId = yeuCauKhamBenhId;
                        var entityDuocPham = yeucauDuocPham.ToEntity<YeuCauDuocPhamBenhVien>();
                        await _yeuCauDuocPhamBenhVienService.ThemYeuCauDuocPhamBenhVien(entityDuocPham);
                        if (entityDuocPham == null)
                            throw new ApiException("entityDuocPham");
                        yeuCauTiepNhanChiTiet.YeuCauDuocPhamBenhViens.Add(entityDuocPham);
                        break;
                    default:
                        break;
                }
            }

            var yeuCauKhamBenhDangKham = yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.FirstOrDefault(x => x.Id == yeuCauKhamBenhId);
            if (yeuCauKhamBenhDangKham != null && yeuCauKhamBenhDangKham.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham)
            {
                yeuCauKhamBenhDangKham.TrangThai = EnumTrangThaiYeuCauKhamBenh.DangKham;
                yeuCauKhamBenhDangKham.NoiThucHienId = yeuCauKhamBenhDangKham.NoiDangKyId; //_userAgentHelper.GetCurrentNoiLLamViecId();
                yeuCauKhamBenhDangKham.BacSiThucHienId = _userAgentHelper.GetCurrentUserId();
                yeuCauKhamBenhDangKham.ThoiDiemThucHien = DateTime.Now;

                YeuCauKhamBenhLichSuTrangThai trangThaiMoi = new YeuCauKhamBenhLichSuTrangThai
                {
                    TrangThaiYeuCauKhamBenh = yeuCauKhamBenhDangKham.TrangThai,
                    MoTa = yeuCauKhamBenhDangKham.TrangThai.GetDescription()
                };
                yeuCauKhamBenhDangKham.YeuCauKhamBenhLichSuTrangThais.Add(trangThaiMoi);
            }

            await _tiepNhanBenhNhanServiceService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);
            return yeuCauTiepNhanChiTiet;
            //}
            //catch (Exception ex)
            //{
            //    throw new ApiException(_localizationService.GetResource(ex.Message.ToString()), (int)HttpStatusCode.BadRequest);
            //    //return ex.Message.ToString();
            //}
        }

        private LoaiDichVuKyThuat GetLoaiDichVuKyThuatChiDinh(EnumDanhMucNhomTheoChiPhi NhomChiPhi)
        {
            switch (NhomChiPhi)
            {
                case EnumDanhMucNhomTheoChiPhi.XetNghiem:
                    return LoaiDichVuKyThuat.XetNghiem;
                case EnumDanhMucNhomTheoChiPhi.ChuanDoanHinhAnh:
                    return LoaiDichVuKyThuat.ChuanDoanHinhAnh;
                case EnumDanhMucNhomTheoChiPhi.ThuThuatPhauThuat:
                    return LoaiDichVuKyThuat.ThuThuatPhauThuat;
                default:
                    return LoaiDichVuKyThuat.Khac;
            }
        }

        #endregion

        #region xử lý data (update)
        [HttpGet("GetChiTietGoiDichVu")]
        public GoiDichVuNoiThucHienViewModel GetChiTietGoiDichVu(long goiDichVuId)
        {
            var dataGoiDichVu = _khamBenhService.GetGoiDichVuKhamBenhKhac(goiDichVuId, false);
            var gridData = new GoiDichVuNoiThucHienViewModel();
            gridData.Id = goiDichVuId;

            var dataGoiDichVuChiTiet = dataGoiDichVu.SelectMany(x => x.GoiChietKhaus).OrderBy(x => x.Nhom).ToList();
            if (dataGoiDichVuChiTiet.Any())
            {
                foreach (var item in dataGoiDichVuChiTiet)
                {
                    var goiChiTiet = new GoiDichVuChiTietNoiThucHienViewModel()
                    {
                        Id = item.Id.Value,
                        Ma = item.Ma,
                        NhomId = item.NhomId,
                        Nhom = item.Nhom,
                        TenDichVu = item.TenDichVu,
                        LoaiGia = item.TenLoaiGia,
                        LoaiYeuCauDichVuId = item.LoaiYeuCauDichVuId.Value
                    };
                    if (item.NhomId == (int)EnumNhomGoiDichVu.DuocPham || item.NhomId == (int)EnumNhomGoiDichVu.VatTuTieuHao)
                    {
                        goiChiTiet.NoiThucHienId = 0;
                    }
                    gridData.GoiDichVuChiTietNoiThucHiens.Add(goiChiTiet);
                }
            }
            return gridData;
        }

        [HttpPost("KiemTraValiDationGoiDichVu")]
        public async Task<ActionResult> KiemTraValiDationGoiDichVu([FromBody] YeuCauThemGoi yeuCauViewModel)
        {
            return NoContent();
        }

        [HttpPost("KiemTraValiDationNoiThucHien")]
        public async Task<ActionResult> KiemTraValiDationNoiThucHien([FromBody] GoiDichVuNoiThucHienViewModel goiDichVuViewModel)
        {
            return NoContent();
        }

        //[HttpPost("XuLyCapNhatGiaBHTT")]
        //public async Task<ActionResult> XuLyCapNhatGiaBHTT(long yeuCauTiepNhanId, long yeuCauKhamBenhId)
        //{
        //    var yeuCauKhamBenh = await _yeuCauKhamBenhService.GetByIdAsync(yeuCauKhamBenhId);
        //    if (yeuCauKhamBenh != null && yeuCauKhamBenh.DuocHuongBaoHiem)
        //    {
        //        var soTienBHTTToanBo = await _cauhinhService.SoTienBHYTSeThanhToanToanBo();
        //        var chiTietThanhToan = new ChiTietBaoHiemThanhToanVo()
        //        {
        //            YeuCauKhamBenhId = yeuCauKhamBenhId,
        //            YeuCauTiepNhanId = yeuCauTiepNhanId,
        //            SoTienBHTTToanBo = soTienBHTTToanBo,
        //            IsUpdate = true
        //        };

        //        await _khamBenhService.XuLyChiTietBaoHiemThanhToanAsync(chiTietThanhToan);
        //    }

        //    return NoContent();
        //}

        [HttpGet("GetChiDinhThongTinDichVuKyThuat")]
        public async Task<ActionResult<DichVuKyThuatBenhVienTemplateVo>> GetChiDinhThongTinDichVuKyThuatAsync(long dichVuKyThuatBenhVienId)
        {
            var dichVu = await _khamBenhService.GetChiDinhThongTinDichVuKyThuatAsync(dichVuKyThuatBenhVienId);
            return dichVu;
        }

        [HttpGet("GetSoDoGiuongBenhTheoPhongKham")]
        public async Task<ActionResult<List<SoDoGiuongBenhTheoPhongKhamVo>>> GetSoDoGiuongBenhTheoPhongKhamAsync(long phongBenhVienId = 0, bool giuongTrong = false, bool giuongDangSuDung = false,
            long dichVuGiuongBenhVienId = 0, long noiThucHienId = 0, long giuongBenhId = 0)
        {
            var soDoGiuong = await _khamBenhService.GetSoDoGiuongBenhTheoPhongKhamAsync(phongBenhVienId, giuongTrong, giuongDangSuDung, dichVuGiuongBenhVienId, noiThucHienId, giuongBenhId);
            return soDoGiuong;
        }

        [HttpPost("NoiThucHien")]
        public async Task<ActionResult<ICollection<LookupItemTemplateVo>>> NoiThucHien([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauKhamBenhService.GetNoiThucHiens(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("BacSiKham")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> BacSiKham([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauKhamBenhService.GetBacSiKhams(queryInfo);
            return Ok(lookup);
        }


        [HttpPost("GetDichVuKhamBenh")]
        public async Task<ActionResult> GetDichVuKhamBenh(DropDownListRequestModel model)
        {
            var result = await _yeuCauKhamBenhService.GetDichVuKhamBenh(model);
            return Ok(result);
        }

        [HttpPost("LoaiGia")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> LoaiGia()
        {
            var lookup = await _yeuCauKhamBenhService.GetLoaiGia();
            return Ok(lookup);
        }

        [HttpPost("LoaiGiaHieuLucTheoDichVuKham")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> LoaiGiaHieuLucTheoDichVuKham(DropDownListRequestModel model)
        {
            var lookup = await _yeuCauKhamBenhService.LoaiGiaHieuLucTheoDichVuKham(model);
            return Ok(lookup);
        }

        [HttpPost("LoaiGiaHieuLucTheoDichVuKyThuat")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> LoaiGiaHieuLucTheoDichVuKyThuat(DropDownListRequestModel model)
        {
            var lookup = await _yeuCauKhamBenhService.LoaiGiaHieuLucTheoDichVuKyThuat(model);
            return Ok(lookup);
        }

        [HttpPost("LoaiGiaHieuLucTheoDichVuGiuong")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> LoaiGiaHieuLucTheoDichVuGiuong(DropDownListRequestModel model)
        {
            var lookup = await _yeuCauKhamBenhService.LoaiGiaHieuLucTheoDichVuKyThuat(model);
            return Ok(lookup);
        }

        [HttpGet("KiemTraChiDinhDichVuDaCoTheoYeuCauTiepNhan")]
        public async Task<ActionResult<bool>> KiemTraChiDinhDichVuDaThemTheoYeuCauTiepNhanAsync(long yeuCauTiepNhanId, long dichVuBenhVienId, Enums.EnumNhomGoiDichVu? nhomDichVu)
        {
            var kiemtra = await _yeuCauKhamBenhService.KiemTraChiDinhDichVuDaThemTheoYeuCauTiepNhanAsync(yeuCauTiepNhanId, dichVuBenhVienId, nhomDichVu);
            return kiemtra;
        }

        [HttpPost("KiemTraChiDinhDichVuKyThuatDaCoTheoYeuCauTiepNhan")]
        public async Task<ActionResult<bool>> KiemTraChiDinhDichVuKyThuatDaCoTheoYeuCauTiepNhanAsync([FromBody] KhamBenhChiDinhDichVuKyThuatMultiselectViewModel chiDinhViewModel)
        {
            var kiemtra = await _yeuCauKhamBenhService.KiemTraChiDinhDichVuKyThuatDaCoTheoYeuCauTiepNhanAsync(chiDinhViewModel.YeuCauTiepNhanId, chiDinhViewModel.DichVuKyThuatBenhVienChiDinhs);
            return kiemtra;
        }

        [HttpPost("KiemTraChiDinhGoiDichVuDaCoDichVuTheoYeuCauTiepNhan")]
        public async Task<ActionResult<bool>> KiemTraChiDinhGoiDichVuDaCoDichVuTheoYeuCauTiepNhanAsync([FromBody] YeuCauThemGoi yeuCauViewModel)
        {
            var kiemtra = await _yeuCauKhamBenhService.KiemTraChiDinhGoiDichVuDaCoDichVuTheoYeuCauTiepNhanAsync(yeuCauViewModel.YeuCauTiepNhanId ?? 0, yeuCauViewModel.GoiDichVuId ?? 0, yeuCauViewModel.DichVuChiDinhTheoGois);
            return kiemtra;
        }

        [HttpPost("GetListKhoSapXepUutien")]
        public async Task<ActionResult<ICollection<KhoSapXepUuTienLookupItemVo>>> GetListKhoSapXepUutienAsync([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _khamBenhService.GetListKhoSapXepUutienAsync(queryInfo);
            return Ok(lookup);
        }

        [HttpGet("GetLoaiKhoAsync")]
        public async Task<ActionResult<EnumLoaiKhoDuocPham>> GetLoaiKhoAsync(long khoId)
        {
            var loaiKho = await _khamBenhService.GetLoaiKhoAsync(khoId);
            return loaiKho;
        }

        [HttpPost("GetListDichVuCanGhiNhanVTTHThuoc")]
        public async Task<ActionResult<ICollection<DichVuCanGhiNhanVTTHThuocVo>>> GetListDichVuCanGhiNhanVTTHThuocAsync([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _khamBenhService.GetListDichVuCanGhiNhanVTTHThuocAsync(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetListVatTuTieuHaoThuoc")]
        public async Task<ActionResult<ICollection<VatTuThuocTieuHaoVo>>> GetListVatTuTieuHaoThuocAsync([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _khamBenhService.GetListVatTuTieuHaoThuocAsync(queryInfo);
            return Ok(lookup);
        }


        [HttpPost("ThemGhiNhanVatTuBenhVien")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> ThemGhiNhanVatTuBenhVienAsync([FromBody] ChiDinhGhiNhanVatTuThuocTieuHaoViewModel yeuCauViewModel)
        {
            if (yeuCauViewModel.IsKhamBenhDangKham)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(yeuCauViewModel.YeuCauKhamBenhId);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(yeuCauViewModel.YeuCauKhamBenhId);
            }

            var yeuCauVo = new ChiDinhGhiNhanVatTuThuocTieuHaoVo
            {
                YeuCauTiepNhanId = yeuCauViewModel.YeuCauTiepNhanId,
                YeuCauKhamBenhId = yeuCauViewModel.YeuCauKhamBenhId,
                DichVuChiDinhId = yeuCauViewModel.DichVuChiDinhId,
                DichVuGhiNhanId = yeuCauViewModel.DichVuGhiNhanId,
                KhoId = yeuCauViewModel.KhoId,
                SoLuong = yeuCauViewModel.SoLuong,
                TinhPhi = yeuCauViewModel.TinhPhi,
                LaDuocPhamBHYT = yeuCauViewModel.LaDuocPhamBHYT,
                LoaiNoiChiDinh = yeuCauViewModel.LoaiNoiChiDinh
            };

            var yeuCauTiepNhanChiTiet = _phauThuatThuThuatService.GetYeuCauTiepNhanForGhiNhanVatTuThuoc(yeuCauViewModel.YeuCauTiepNhanId);
            //var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);

            // kiểm tra nếu chưa nhập chẩn đoán sơ bộ thì ko cho thêm dvkt, ghi nhận thuốc/VTTH
            _yeuCauKhamBenhService.KiemTraChanDoanSoBoKhiThemDichVu(yeuCauTiepNhanChiTiet, yeuCauViewModel.YeuCauKhamBenhId);

            await _khamBenhService.XuLyThemGhiNhanVatTuBenhVienAsync(yeuCauVo, yeuCauTiepNhanChiTiet);

            // gọi hàm chung
            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            // cập nhật lại số lượng tồn
            if (yeuCauVo.NhapKhoDuocPhamChiTiets.Any() || yeuCauVo.NhapKhoVatTuChiTiets.Any())
            {
                await _khamBenhService.CapNhatSoLuongTonKhiGhiNhanVTTHThuocAsync(yeuCauVo.NhapKhoDuocPhamChiTiets, yeuCauVo.NhapKhoVatTuChiTiets);
            }

            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            chiDinhDichVuResultVo.SoDuTaiKhoan = await _taikhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taikhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            //Cập nhật 04/07/2022: bật cờ trường hợp thêm loại lĩnh bù -> xử lý xuất luôn
            chiDinhDichVuResultVo.LaLinhBu = yeuCauVo.LaLinhBu;

            return chiDinhDichVuResultVo;
            //return Ok();
        }

        [HttpGet("GetGridDataGhiNhanVTTHThuoc")]
        public async Task<List<GhiNhanVatTuTieuHaoThuocGroupParentGridVo>> GetGridDataGhiNhanVTTHThuocAsync(long yeuCauTiepNhanId, long yeuCauKhamBenhId)
        {
            //var gridData = await _khamBenhService.GetGridDataGhiNhanVTTHThuocAsync(yeuCauTiepNhanId, yeuCauKhamBenhId);
            var gridData = await _khamBenhService.GetGridDataGhiNhanVTTHThuocAsyncVer2(yeuCauTiepNhanId, yeuCauKhamBenhId);
            return gridData;
        }
        [HttpGet("GetGridDataGhiNhanVTTH")]
        public async Task<List<GhiNhanVatTuTieuHaoThuocGridVo>> GetGridDataGhiNhanVTTHAsync(long yeuCauTiepNhanId, long yeuCauKhamBenhId)
        {
            var gridData = await _khamBenhService.GetGridDataGhiNhanVTTHcAsync(yeuCauTiepNhanId, yeuCauKhamBenhId);
            return gridData;
        }

        [HttpPost("XuLyXoaYeuCauGhiNhanVTTHThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> XuLyXoaYeuCauGhiNhanVTTHThuocAsync(GridItemGhiNhanVTTHThuocViewModel xoaYeuCauViewModel)
        {
            if (xoaYeuCauViewModel.IsKhamBenhDangKham)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(xoaYeuCauViewModel.YeuCauKhamBenhId);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(xoaYeuCauViewModel.YeuCauKhamBenhId);
            }

            var yeuCauTiepNhanChiTiet = _phauThuatThuThuatService.GetYeuCauTiepNhanForGhiNhanVatTuThuoc(xoaYeuCauViewModel.YeuCauTiepNhanId);
            //var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(xoaYeuCauViewModel.YeuCauTiepNhanId);

            // xử lý xóa yeu cau duoc pham/ vat tu
            await _khamBenhService.XuLyXoaYeuCauGhiNhanVTTHThuocAsync(yeuCauTiepNhanChiTiet, xoaYeuCauViewModel.YeuCauGhiNhanVTTHThuocId);

            await _tiepNhanBenhNhanServiceService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);
            //await _tiepNhanBenhNhanServiceService.UpdateAsync(yeuCauTiepNhanChiTiet);

            // get lại thông tin số dư tài khoản người bệnh
            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            chiDinhDichVuResultVo.SoDuTaiKhoan = await _taikhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taikhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            return chiDinhDichVuResultVo;
        }


        [HttpPost("CapNhatGridItemGhiNhanVTTHThuoc")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> CapNhatGridItemGhiNhanVTTHThuocAsync(GridItemGhiNhanVTTHThuocViewModel viewModel)
        {
            if (viewModel.IsKhamBenhDangKham)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(viewModel.YeuCauKhamBenhId);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(viewModel.YeuCauKhamBenhId);
            }

            if (viewModel.IsCapNhatSoLuong && (viewModel.SoLuong == null || viewModel.SoLuong.Value == 0))
            {
                throw new ApiException(_localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.SoLuong.Required"));
            }

            var yeuCauTiepNhanChiTiet = _phauThuatThuThuatService.GetYeuCauTiepNhanForGhiNhanVatTuThuoc(viewModel.YeuCauTiepNhanId);
            //var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(viewModel.YeuCauTiepNhanId);

            var yeuCauVo = new ChiDinhGhiNhanVatTuThuocTieuHaoVo
            {
                YeuCauTiepNhanId = viewModel.YeuCauTiepNhanId,
                YeuCauKhamBenhId = viewModel.YeuCauKhamBenhId,
                //DichVuChiDinhId = yeuCauViewModel.DichVuChiDinhId,
                //DichVuGhiNhanId = yeuCauViewModel.DichVuGhiNhanId,
                //KhoId = yeuCauViewModel.KhoId,
                //SoLuong = yeuCauViewModel.SoLuong,
                //TinhPhi = yeuCauViewModel.TinhPhi,
                //LaDuocPhamBHYT = yeuCauViewModel.LaDuocPhamBHYT
                YeuCauGhiNhanVTTHThuocId = viewModel.YeuCauGhiNhanVTTHThuocId,
                IsCapNhatSoLuong = viewModel.IsCapNhatSoLuong,
                SoLuongCapNhat = viewModel.SoLuong,
                IsCapNhatTinhPhi = viewModel.IsCapNhatTinhPhi,
                TinhPhi = viewModel.TinhPhi
            };

            // xử lý cập nhật số luonjg, số lượng xuất
            await _khamBenhService.CapNhatGridItemGhiNhanVTTHThuocAsync(yeuCauTiepNhanChiTiet, yeuCauVo);

            await _tiepNhanBenhNhanServiceService.PrepareForEditDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            if (yeuCauVo.NhapKhoDuocPhamChiTiets.Any() || yeuCauVo.NhapKhoVatTuChiTiets.Any())
            {
                await _khamBenhService.CapNhatSoLuongTonKhiGhiNhanVTTHThuocAsync(yeuCauVo.NhapKhoDuocPhamChiTiets, yeuCauVo.NhapKhoVatTuChiTiets);
            }

            // get lại thông tin số dư tài khoản người bệnh
            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            chiDinhDichVuResultVo.SoDuTaiKhoan = await _taikhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taikhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            return chiDinhDichVuResultVo;
        }

        [HttpPost("XuLyXuatYeuCauGhiNhanVTTHThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task XuLyXuatYeuCauGhiNhanVTTHThuocAsync(GridItemGhiNhanVTTHThuocViewModel yeuCauViewModel)
        {
            if (yeuCauViewModel.IsKhamBenhDangKham)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(yeuCauViewModel.YeuCauKhamBenhId);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(yeuCauViewModel.YeuCauKhamBenhId);
            }

            // xử lý xuất
            var yeuCauVo = new ChiDinhGhiNhanVatTuThuocTieuHaoVo()
            {
                YeuCauTiepNhanId = yeuCauViewModel.YeuCauTiepNhanId,
                YeuCauKhamBenhId = yeuCauViewModel.YeuCauKhamBenhId
            };

            await _khamBenhService.XuLyXuatYeuCauGhiNhanVTTHThuocAsync(yeuCauVo);
        }
        #endregion

        #region Nhóm dịch vụ thường dùng

        #region grid
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetNhomDichVuThuongDungDataForGrid")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong, 
        //    DocumentType.DanhSachDieuTriNoiTru, DocumentType.PhauThuatThuThuatTheoNgay, DocumentType.TiemChungKhamSangLoc, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<GridDataSource>> GetNhomDichVuThuongDungDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamBenhService.GetNhomDichVuThuongDungDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetNhomDichVuThuongDungTotalPageForGrid")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong,
        //    DocumentType.DanhSachDieuTriNoiTru, DocumentType.PhauThuatThuThuatTheoNgay, DocumentType.TiemChungKhamSangLoc, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<GridDataSource>> GetNhomDichVuThuongDungTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamBenhService.GetNhomDichVuThuongDungTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetChiTietDichVuThuongDungTrongGoiDataForGrid")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong,
        //    DocumentType.DanhSachDieuTriNoiTru, DocumentType.PhauThuatThuThuatTheoNgay, DocumentType.TiemChungKhamSangLoc, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<GridDataSource>> GetChiTietDichVuThuongDungTrongGoiDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamBenhService.GetChiTietDichVuThuongDungTrongGoiDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetChiTietDichVuThuongDungTrongGoiTotalPageForGrid")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong,
        //    DocumentType.DanhSachDieuTriNoiTru, DocumentType.PhauThuatThuThuatTheoNgay, DocumentType.TiemChungKhamSangLoc, DocumentType.YeuCauTiepNhan)]
        public async Task<ActionResult<GridDataSource>> GetChiTietDichVuThuongDungTrongGoiTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamBenhService.GetChiTietDichVuThuongDungTrongGoiTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetGoiDichVuCuaBenhNhanDataForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<GridDataSource>> GetGoiDichVuCuaBenhNhanDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamBenhService.GetGoiDichVuCuaBenhNhanDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetGoiDichVuCuaBenhNhanTotalPageForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<GridDataSource>> GetGoiDichVuCuaBenhNhanTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamBenhService.GetGoiDichVuCuaBenhNhanTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetChiTietGoiDichVuCuaBenhNhanDataForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<GridDataSourceChiTietGoiDichVuTheoBenhNhan>> GetChiTietGoiDichVuCuaBenhNhanDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var searchObj = queryInfo.AdditionalSearchString.Split(';');
            var yeuCauGoiDichVuId = long.Parse(searchObj[0]);
            var yeuCauGoiDichVu = _yeuCauGoiDichVuService.GetById(yeuCauGoiDichVuId);
            var benhNhanId = yeuCauGoiDichVu.GoiSoSinh == true ? yeuCauGoiDichVu.BenhNhanSoSinhId : yeuCauGoiDichVu.BenhNhanId;
            var dichVuGiuongDaChiDinhs = await _dieuTriNoiTruService.GetThongTinSuDungDichVuGiuongTrongGoiAsync(benhNhanId ?? 0);

            var gridData = await _khamBenhService.GetChiTietGoiDichVuCuaBenhNhanDataForGridAsync(queryInfo, false, dichVuGiuongDaChiDinhs);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetChiTietGoiDichVuCuaBenhNhanTotalPageForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<GridDataSourceChiTietGoiDichVuTheoBenhNhan>> GetChiTietGoiDichVuCuaBenhNhanTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamBenhService.GetChiTietGoiDichVuCuaBenhNhanTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        #endregion

        [HttpPost("KiemTraValidationGoiDichVuThuongDung")]
        public async Task<ActionResult> KiemTraValidationGoiDichVuThuongDungAsync([FromBody] ChiDinhNhomGoiDichVuThuongDungViewModel chiDinhViewModel)
        {
            return Ok();
        }

        [HttpPost("KiemTraDichVuTrongGoiDaCoTheoYeuCauTiepNhan")]
        public async Task<ActionResult<List<ChiDinhGoiDichVuThuongDungDichVuLoiVo>>> KiemTraDichVuTrongGoiDaCoTheoYeuCauTiepNhanAsync([FromBody] ChiDinhNhomGoiDichVuThuongDungViewModel chiDinhViewModel)
        {
            var dichVuTrung = await _khamBenhService.KiemTraDichVuTrongGoiDaCoTheoYeuCauTiepNhanAsync(chiDinhViewModel.YeuCauTiepNhanId, chiDinhViewModel.GoiDichVuIds, null, (chiDinhViewModel.LaPhauThuatThuThuat ?? false));
            return dichVuTrung;
        }

        [HttpPost("ThemYeuGoiDichVuThuongDung")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> ThemYeuGoiDichVuThuongDungAsync([FromBody] ChiDinhNhomGoiDichVuThuongDungViewModel yeuCauViewModel)
        {
            // kiểm tra yêu cầu khám bệnh trước khi thêm
            if (yeuCauViewModel.IsKhamBenhDangKham)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(yeuCauViewModel.YeuCauKhamBenhId);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(yeuCauViewModel.YeuCauKhamBenhId);
            }

            // get thông tin yêu cầu tiếp nhận hiện tại
            //var yeuCauTiepNhanChiTiet =
            //    await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanKhiThemDichVuNgoaiTruByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);

            // kiểm tra nếu chưa nhập chẩn đoán sơ bộ thì ko cho thêm dvkt, ghi nhận thuốc/VTTH
            _yeuCauKhamBenhService.KiemTraChanDoanSoBoKhiThemDichVu(yeuCauTiepNhanChiTiet, yeuCauViewModel.YeuCauKhamBenhId);

            var yeuCauVo = yeuCauViewModel.Map<YeuCauThemGoiDichVuThuongDungVo>();
            await _khamBenhService.XuLyThemGoiDichVuThuongDungAsync(yeuCauTiepNhanChiTiet, yeuCauVo);
            await _tiepNhanBenhNhanServiceService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            // kiểm tra chỉ định dich vụ vượt quá số dư tài khoản
            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            chiDinhDichVuResultVo.SoDuTaiKhoan = await _taikhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taikhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            chiDinhDichVuResultVo.IsVuotQuaSoDuTaiKhoan = yeuCauVo.YeuCauKhamBenhNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan)
                                                           || yeuCauVo.YeuCauDichVuKyThuatNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan)
                                                           || yeuCauVo.YeuCauDichVuGiuongBenhVienNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan);

            if (yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.First(x => x.Id == yeuCauVo.YeuCauKhamBenhId).TrangThai != EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh
                && yeuCauVo.YeuCauDichVuKyThuatNews.Any(x => x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem
                                                          || x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat
                                                          || x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                                          || x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang))
            {
                chiDinhDichVuResultVo.ChuyenHangDoiSangLamChiDinh = true;
                var phongHangDoiHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
                if (yeuCauViewModel.IsKhamBenhDangKham)
                {
                    var hangDoi = yeuCauTiepNhanChiTiet.YeuCauKhamBenhs
                        .Where(x => x.Id == yeuCauVo.YeuCauKhamBenhId && x.YeuCauTiepNhanId == yeuCauVo.YeuCauTiepNhanId)
                        .SelectMany(x => x.PhongBenhVienHangDois)
                        .OrderByDescending(x => x.Id)
                        .ToList();
                    if (hangDoi.Any())
                    {
                        phongHangDoiHienTaiId = hangDoi.First().PhongBenhVienId;
                    }
                }
                await _khamBenhService.CapNhatHangChoKhiChiDinhDichVuKyThuatAsync(yeuCauVo.YeuCauTiepNhanId, yeuCauVo.YeuCauKhamBenhId, phongHangDoiHienTaiId);
            }
            return chiDinhDichVuResultVo;
        }

        [HttpGet("KiemTraDangKyGoiDichVuTheoBenhNhan")]
        public async Task<ActionResult<bool>> KiemTraDangKyGoiDichVuTheoBenhNhanAsync(long benhNhanId)
        {
            var kiemTra = await _khamBenhService.KiemTraDangKyGoiDichVuTheoBenhNhanAsync(benhNhanId);
            return kiemTra;
        }

        [HttpPost("KiemTraValidationChiDinhDichVuTrongGoiMarketing")]
        public async Task<ActionResult> KiemTraValidationChiDinhGoiDichVuTheoBenhNhanAsync([FromBody] ChiDinhGoiDichVuTheoBenhNhanViewModel chiDinhViewModel)
        {
            if (!chiDinhViewModel.DichVus.Any())
            {
                throw new ApiException(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.DichVu.Required"));
            }
            if (chiDinhViewModel.DichVus.Any(x => x.SoLuongSuDung == 0))
            {
                throw new ApiException(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.SoLuongChiDinh.Required"));
            }

            var yeuCauVo = chiDinhViewModel.Map<ChiDinhGoiDichVuTheoBenhNhanVo>();
            await _khamBenhService.KiemTraSoLuongConLaiCuaDichVuTrongGoiAsync(yeuCauVo);
            return Ok();
        }

        [HttpPost("KiemTraDichVuTrongGoiMarketingDaCoTheoYeuCauTiepNhan")]
        public async Task<ActionResult<List<ChiDinhGoiDichVuTheoBenhNhanDichVuLoiVo>>> KiemTraDichVuTrongGoiMarketingDaCoTheoYeuCauTiepNhanAsync([FromBody] ChiDinhGoiDichVuTheoBenhNhanViewModel chiDinhViewModel)
        {
            var dichVuTrung = await _khamBenhService.KiemTraValidationChiDinhGoiDichVuTheoBenhNhanAsync(chiDinhViewModel.YeuCauTiepNhanId, chiDinhViewModel.DichVus.Select(a => a.Id).ToList());
            return dichVuTrung;
        }

        [HttpPost("ThemChiDinhGoiDichVuTheoBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> ThemChiDinhGoiDichVuTheoBenhNhanAsync([FromBody] ChiDinhGoiDichVuTheoBenhNhanViewModel yeuCauViewModel)
        {
            // kiểm tra yêu cầu khám bệnh trước khi thêm
            if (yeuCauViewModel.IsKhamBenhDangKham)
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(yeuCauViewModel.YeuCauKhamBenhId ?? 0);
            }
            else
            {
                await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(yeuCauViewModel.YeuCauKhamBenhId ?? 0);
            }

            // get thông tin yêu cầu tiếp nhận hiện tại
            //var yeuCauTiepNhanChiTiet =
            //    await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);
            var yeuCauTiepNhanChiTiet =
                await _khamBenhService.GetYeuCauTiepNhanKhiThemDichVuNgoaiTruByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);

            // kiểm tra nếu chưa nhập chẩn đoán sơ bộ thì ko cho thêm dvkt, ghi nhận thuốc/VTTH
            _yeuCauKhamBenhService.KiemTraChanDoanSoBoKhiThemDichVu(yeuCauTiepNhanChiTiet, yeuCauViewModel.YeuCauKhamBenhId ?? 0);

            var yeuCauVo = yeuCauViewModel.Map<ChiDinhGoiDichVuTheoBenhNhanVo>();
            await _khamBenhService.XuLyThemChiDinhGoiDichVuTheoBenhNhanAsync(yeuCauTiepNhanChiTiet, yeuCauVo);
            await _tiepNhanBenhNhanServiceService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            // kiểm tra chỉ định dich vụ vượt quá số dư tài khoản
            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            chiDinhDichVuResultVo.SoDuTaiKhoan = await _taikhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taikhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            // dịch vụ chỉ định từ gói marketing ko cần kiểm tra trạng thái thanh toán
            //chiDinhDichVuResultVo.IsVuotQuaSoDuTaiKhoan = yeuCauVo.YeuCauKhamBenhNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan)
            //                                               || yeuCauVo.YeuCauDichVuKyThuatNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan)
            //                                               || yeuCauVo.YeuCauDichVuGiuongBenhVienNews.Any(x => x.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan);

            if (yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.First(x => x.Id == yeuCauVo.YeuCauKhamBenhId).TrangThai != EnumTrangThaiYeuCauKhamBenh.DangLamChiDinh
                && yeuCauVo.YeuCauDichVuKyThuatNews.Any(x => x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem
                                                          || x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat
                                                          || x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                                          || x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThamDoChucNang))
            {
                chiDinhDichVuResultVo.ChuyenHangDoiSangLamChiDinh = true;
                var phongHangDoiHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
                if (yeuCauViewModel.IsKhamBenhDangKham)
                {
                    var hangDoi = yeuCauTiepNhanChiTiet.YeuCauKhamBenhs
                        .Where(x => x.Id == yeuCauVo.YeuCauKhamBenhId && x.YeuCauTiepNhanId == yeuCauVo.YeuCauTiepNhanId)
                        .SelectMany(x => x.PhongBenhVienHangDois)
                        .OrderByDescending(x => x.Id)
                        .ToList();
                    if (hangDoi.Any())
                    {
                        phongHangDoiHienTaiId = hangDoi.First().PhongBenhVienId;
                    }
                }
                await _khamBenhService.CapNhatHangChoKhiChiDinhDichVuKyThuatAsync(yeuCauVo.YeuCauTiepNhanId, yeuCauVo.YeuCauKhamBenhId ?? 0, phongHangDoiHienTaiId);
            }

            chiDinhDichVuResultVo.IsVuotQuaBaoLanhGoi = yeuCauVo.IsVuotQuaBaoLanhGoi;
            return chiDinhDichVuResultVo;
        }

        [HttpPost("KiemTraDichVuKhamBenhChiDinhCoTrongGoiCuaBenhNhan")]
        public async Task<ActionResult<DichVuChiDinhCoTrongGoiCuaBenhNhanVo>> KiemTraDichVuKhamBenhChiDinhCoTrongGoiCuaBenhNhanAsync([FromBody] ChiDinhDichVuKhamBenhViewModel yeuCauViewModel)
        {
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(yeuCauViewModel.YeuCauTiepNhanId, x => x.Include(a => a.BenhNhan));
            var yeuCauVo = new DichVuChiDinhCoTrongGoiCuaBenhNhanVo()
            {
                BenhNhanId = yeuCauTiepNhan.BenhNhanId.Value
            };
            yeuCauVo.DichVuKhamBenhIds.Add(yeuCauViewModel.DichVuKhamBenhBenhVienId.Value);
            await _khamBenhService.KiemTraDichVuChiDinhCoTrongGoiCuaBenhNhanAsync(yeuCauVo);

            if (yeuCauVo.DichVuChiDinhCoTrongGois.Any())
            {
                var messTemplate = _localizationService.GetResource("KhamBenh.ChiDinh.DichVuChiDingTrungTrongGoi");
                yeuCauVo.Message =
                    string.Format(messTemplate, string.Join(",", yeuCauVo.DichVuChiDinhCoTrongGois.Select(x => x.TenDichVu).Distinct().ToList()),
                        string.Join(",", yeuCauVo.DichVuChiDinhCoTrongGois.Select(x => x.TenGoiDichVu).Distinct().ToList()),
                        yeuCauTiepNhan.BenhNhan.HoTen);
            }

            return yeuCauVo;
        }

        [HttpPost("KiemTraDichVuKyThuatChiDinhCoTrongGoiCuaBenhNhan")]
        public async Task<ActionResult<DichVuChiDinhCoTrongGoiCuaBenhNhanVo>> KiemTraDichVuKyThuatChiDinhCoTrongGoiCuaBenhNhanAsync([FromBody] KhamBenhChiDinhDichVuKyThuatMultiselectViewModel yeuCauViewModel)
        {
            //todo: có update bỏ await
            var yeuCauTiepNhan = _yeuCauTiepNhanService.GetById(yeuCauViewModel.YeuCauTiepNhanId, x => x.Include(a => a.BenhNhan));
            var yeuCauVo = new DichVuChiDinhCoTrongGoiCuaBenhNhanVo()
            {
                BenhNhanId = yeuCauTiepNhan.BenhNhanId.Value
            };

            var lstDichVuKyThuatDangChon = yeuCauViewModel.DichVuKyThuatBenhVienChiDinhs.Select(x => x).Distinct().ToList();
            foreach (var strId in lstDichVuKyThuatDangChon)
            {
                var itemObj = JsonConvert.DeserializeObject<ItemChiDinhDichVuKyThuatVo>(strId);

                yeuCauVo.DichVuKyThuatIds.Add(itemObj.DichVuId);
            }

            await _khamBenhService.KiemTraDichVuChiDinhCoTrongGoiCuaBenhNhanAsync(yeuCauVo);

            if (yeuCauVo.DichVuChiDinhCoTrongGois.Any())
            {
                var messTemplate = _localizationService.GetResource("KhamBenh.ChiDinh.DichVuChiDingTrungTrongGoi");
                yeuCauVo.Message =
                    string.Format(messTemplate, string.Join(",", yeuCauVo.DichVuChiDinhCoTrongGois.Select(x => x.TenDichVu).Distinct().ToList()),
                        string.Join(",", yeuCauVo.DichVuChiDinhCoTrongGois.Select(x => x.TenGoiDichVu).Distinct().ToList()),
                        yeuCauTiepNhan.BenhNhan.HoTen);
            }

            return yeuCauVo;
        }
        #endregion

        [HttpPut("CapNhatGhiChuCanLamSang")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh, DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult> CapNhatGhiChuCanLamSangAsync(UpdateGhiChuCanLamSangVo updateVo)
        {
            await _khamBenhService.CapNhatGhiChuCanLamSangAsync(updateVo);
            return Ok();
        }

        #region Cập nhật ghi nhận VTTH/Thuốc 24/05/2021
        [HttpPost("KiemTraValidtionSoLuongGhiNhanThuocVTTH")]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> KiemTraValidtionSoLuongGhiNhanThuocVTTH(UpdateSoLuongItemGhiNhanVTTHThuocViewModel viewModel)
        {
            return Ok();
        }

        [HttpPost("KiemTraTrungGhiNhanVTTHThuoc")]
        public async Task<ActionResult<bool>> KiemTraTrungGhiNhanVTTHThuoc(VTTHThuocCanKiemTraTrungKhiThemViewModel viewModel)
        {
            var result = false;
            if (viewModel.YeuCauTiepNhanId != null && !string.IsNullOrEmpty(viewModel.DichVuChiDinhId) && !string.IsNullOrEmpty(viewModel.DichVuGhiNhanId))
            {
                var info = new VTTHThuocCanKiemTraTrungKhiThemVo()
                {
                    YeuCauTiepNhanId =  viewModel.YeuCauTiepNhanId.Value,
                    YeuCauKhamBenhId = viewModel.YeuCauKhamBenhId,
                    DichVuChiDinhId = viewModel.DichVuChiDinhId,
                    DichVuGhiNhanId = viewModel.DichVuGhiNhanId
                };

                result = _khamBenhService.KiemTraTrungGhiNhanVTTHThuoc(info, viewModel.YeuCauTiepNhanId.Value);
                //var yeuCauTiepNhanChiTiet = _phauThuatThuThuatService.GetYeuCauTiepNhanForGhiNhanVatTuThuoc(viewModel.YeuCauTiepNhanId.Value);
                //result = _khamBenhService.KiemTraTrungGhiNhanVTTHThuoc(info, yeuCauTiepNhanChiTiet);
            }

            return result;
        }
        #endregion


        #region cập nhật kiểm tra dịch vụ khác 4 nhóm: PTTT, CDHA, TDCN, XN thì cho phép hoàn thành, hủy hoàn thành
        [HttpGet("GetYeuCauDichVuKyThuatById")]
        public async Task<ActionResult<TrangThaiThucHienYeuCauDichVuKyThuatViewModel>> GetYeuCauDichVuKyThuatByIdAsync(long yeuCauDichVuKyThuatId)
        {
            var yeuCauDichVuKyThuat = _yeuCauDichVuKyThuatService.GetById(yeuCauDichVuKyThuatId);
            var result = yeuCauDichVuKyThuat.ToModel<TrangThaiThucHienYeuCauDichVuKyThuatViewModel>();

            result.NhanVienThucHienId = result.NhanVienThucHienId == null ? _userAgentHelper.GetCurrentUserId() : result.NhanVienThucHienId;
            result.ThoiDiemThucHien = result.ThoiDiemThucHien == null ? DateTime.Now : result.ThoiDiemThucHien;

            return result;
        }

        [HttpPut("XuLyCapNhatThucHienYeuCauDichVuKyThuat")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh)]
        public async Task<ActionResult> XuLyCapNhatThucHienYeuCauDichVuKyThuatAsync([FromBody] TrangThaiThucHienYeuCauDichVuKyThuatViewModel viewModel)
        {
            // kiểm tra yêu cầu khám bệnh trước khi thêm
            if (viewModel.YeuCauKhamBenhId != null && !viewModel.LaKhamDoanTatCa)
            {
                if (viewModel.IsKhamBenhDangKham)
                {
                    await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhDangKhamAsync(viewModel.YeuCauKhamBenhId.Value);
                }
                else
                {
                    await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(viewModel.YeuCauKhamBenhId.Value);
                }
            }


            var yeuCauDichVuKyThuat = _yeuCauDichVuKyThuatService.GetById(viewModel.Id);
            if (yeuCauDichVuKyThuat.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
            {
                throw new ApiException(_localizationService.GetResource("CapNhatThucHienDichVuKyThuat.TrangThaiYeuCauDichVuKyThuat.DaHuy"));
            }
            viewModel.ToEntity(yeuCauDichVuKyThuat);
            if (viewModel.LaThucHienDichVu)
            {
                if (yeuCauDichVuKyThuat.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan
                    && yeuCauDichVuKyThuat.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                    && yeuCauDichVuKyThuat.TrangThaiThanhToan != TrangThaiThanhToan.CapNhatThanhToan)
                {
                    throw new ApiException(_localizationService.GetResource("CapNhatThucHienDichVuKyThuat.TrangThaiThanhToanYeuCauDichVuKyThuat.ChuaThanhToan"));
                }

                //yeuCauDichVuKyThuat.LyDoHuyTrangThaiDaThucHien = null;
                //yeuCauDichVuKyThuat.NhanVienHuyTrangThaiDaThucHienId = null;
                yeuCauDichVuKyThuat.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
            }
            else
            {
                yeuCauDichVuKyThuat.NhanVienHuyTrangThaiDaThucHienId = _userAgentHelper.GetCurrentUserId();
                yeuCauDichVuKyThuat.NhanVienThucHienId = yeuCauDichVuKyThuat.NhanVienKetLuanId = null;
                yeuCauDichVuKyThuat.ThoiDiemThucHien = yeuCauDichVuKyThuat.ThoiDiemHoanThanh = null;
                yeuCauDichVuKyThuat.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
            }

            _yeuCauDichVuKyThuatService.Update(yeuCauDichVuKyThuat);
            return Ok();
        }
        #endregion
        #region search grid popup  in Chi dinh 
        [HttpPost("TimKiemGridPopUpInChiDinhKhamBenh")]
        public async Task<ActionResult> TimKiemGridPopUpInChiDinhKhamBenh(TimKiemPopupInKhamBenhGoiDichVuVo model)
        {
            var grid = await _khamBenhService.GetDanhSachSearchPopupInChiDinhKhamBenhForGrid(model);
            return Ok(grid);
        }
        #endregion
        #region gét danh sách tất cả chỉ định của bệnh nhân 
        [HttpPost("GetDanhSachDichVuChiDinhCuaBenhNhan")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamBenh, DocumentType.LichSuKhamBenh)]
        public ActionResult<GridDataSource> GetDanhSachDichVuChiDinhCuaBenhNhan(long yeuCauTiepNhanId,long yeuCauKhamBenhId)
        {
            var gridData = _khamBenhService.GetDanhSachDichVuChiDinhCuaBenhNhan(yeuCauTiepNhanId, yeuCauKhamBenhId);
            return Ok(gridData);
        }
        #endregion

        #region BVHD-3298: [PHÁT SINH TRIỂN KHAI] [Chuyển DV khám] Chỉ cho phép chuyển phòng cho DV khám trong khám bệnh
        [HttpPost("GetListPhongThucHienDichVuTrongKhoaHienTai")]
        public async Task<ActionResult<ICollection<DichVuCanGhiNhanVTTHThuocVo>>> GetListPhongThucHienDichVuTrongKhoaHienTaiAsync([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _khamBenhService.GetListPhongThucHienDichVuTrongKhoaHienTaiAsync(queryInfo);
            return Ok(lookup);
        }

        [HttpPut("XuLyChuyenPhongThucHienDichVuKham")]
        public async Task<ActionResult> XuLyChuyenPhongThucHienDichVuKhamAsync(PhongKhamChuyenDenInfoViewModel phongKhamChuyenDenInfo)
        {
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            if (phongHienTaiId != phongKhamChuyenDenInfo.PhongHienTaiId)
            {
                throw new ApiException(_localizationService.GetResource("ChuyenPhongKhamBenh.PhongHienTaiId.KhongDung"));
            }

            var phongKhamChuyenDenInfoVo = new PhongKhamChuyenDenInfoVo()
            {
                HangDoiIds = phongKhamChuyenDenInfo.HangDoiIds,
                PhongThucHienId = phongKhamChuyenDenInfo.PhongThucHienId.Value
            };
            await _khamBenhService.XuLyChuyenPhongThucHienDichVuKhamAsync(phongKhamChuyenDenInfoVo);
            return Ok();
        }
        #endregion


        [HttpPut("XuLyCapNhatDichVuKhac")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh)]
        public async Task<ActionResult> XuLyCapNhatDichVuKhacAsync([FromBody] TrangThaiThucHienYeuCauDichVuKyThuatViewModel viewModel)
        {

            var yeuCauDichVuKyThuat = _yeuCauDichVuKyThuatService.GetById(viewModel.Id);
            if (yeuCauDichVuKyThuat.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
            {
                throw new ApiException(_localizationService.GetResource("CapNhatThucHienDichVuKyThuat.TrangThaiYeuCauDichVuKyThuat.DaHuy"));
            }
            viewModel.ToEntity(yeuCauDichVuKyThuat);
            if (viewModel.LaThucHienDichVu)
            {
                if (yeuCauDichVuKyThuat.TrangThaiThanhToan != TrangThaiThanhToan.BaoLanhThanhToan
                    && yeuCauDichVuKyThuat.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan
                    && yeuCauDichVuKyThuat.TrangThaiThanhToan != TrangThaiThanhToan.CapNhatThanhToan)
                {
                    throw new ApiException(_localizationService.GetResource("CapNhatThucHienDichVuKyThuat.TrangThaiThanhToanYeuCauDichVuKyThuat.ChuaThanhToan"));
                }         
                yeuCauDichVuKyThuat.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
            }
            else
            {
                yeuCauDichVuKyThuat.NhanVienHuyTrangThaiDaThucHienId = _userAgentHelper.GetCurrentUserId();
                yeuCauDichVuKyThuat.NhanVienThucHienId = yeuCauDichVuKyThuat.NhanVienKetLuanId = null;
                yeuCauDichVuKyThuat.ThoiDiemThucHien = yeuCauDichVuKyThuat.ThoiDiemHoanThanh = null;
                yeuCauDichVuKyThuat.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
            }

            _yeuCauDichVuKyThuatService.Update(yeuCauDichVuKyThuat);
            return Ok();
        }

    }
}
