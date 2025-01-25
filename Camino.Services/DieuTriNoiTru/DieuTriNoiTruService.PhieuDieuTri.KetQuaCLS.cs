using Camino.Core.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Data;
using static Camino.Core.Domain.Enums;
using Camino.Core.Helpers;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.KetQuaCLS;
using System.Linq.Dynamic.Core;
using Camino.Core.Domain.Entities.XetNghiems;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        #region Kết quả cận lâm sàng 

        #region Chuẩn đoán hình ảnh và thăm dò chức năng

        public GridDataSource GetDataNoiTruKetQuaCDHATDCN(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var data = queryInfo.AdditionalSearchString.Split(";");

            var yeuCauTiepNhanId = long.Parse(data[0]);
            var phieuDieuTriId = long.Parse(data[1]);

            var yeuCauKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                                                               && x.NoiTruPhieuDieuTriId == phieuDieuTriId)
                                                              .Include(cc => cc.PhienXetNghiemChiTiets)
                                                              .Include(cc => cc.NhanVienKetLuan).ThenInclude(cc => cc.User)
                                                              .Include(cc => cc.NhanVienThucHien).ThenInclude(cc => cc.User)
                                                              .Include(cc => cc.NhomDichVuBenhVien);

            var query = yeuCauKyThuats.Where(x => (x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien) &&
                                                  (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh))
                                                               .Select(s => new KetQuaCLSGridVo()
                                                               {
                                                                   Id = s.Id,
                                                                   NoiDung = s.TenDichVu,
                                                                   NguoiThucHien = s.NhanVienThucHien != null ? s.NhanVienThucHien.User.HoTen : null,
                                                                   NgayThucHien = s.ThoiDiemThucHien != null ? s.ThoiDiemThucHien.Value.ApplyFormatDateTimeSACH() : null,
                                                                   BacSiKetLuan = s.NhanVienKetLuan != null ? s.NhanVienKetLuan.User.HoTen : null,
                                                                   NgayKetLuan = s.ThoiDiemKetLuan != null ? s.ThoiDiemKetLuan.Value.ApplyFormatDateTimeSACH() : null,
                                                                   LoaiKetQuaId = s.NhomDichVuBenhVien.NhomDichVuBenhVienChaId,
                                                                   LoaiKetQuaCLS = s.LoaiDichVuKyThuat.GetDescription(),
                                                                   YeuCauTiepNhanId = yeuCauTiepNhanId,
                                                                   IsDisable = true,
                                                               });

            query = query.ApplyLike(queryInfo.SearchTerms.ToLower(), g => g.NoiDung, g => g.NgayThucHien, g => g.BacSiKetLuan, g => g.BacSiKetLuanRemoveDictrict, g => g.NoiDungRemoveDictrict);
            var dataCanLamSangs = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();

            return new GridDataSource { Data = dataCanLamSangs };
        }

        public GridDataSource GetTotalNoiTruKetQuaCDHATDCN(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var data = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauTiepNhanId = long.Parse(data[0]);
            var phieuDieuTriId = long.Parse(data[1]);

           
            var yeuCauKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId
                                                               && x.NoiTruPhieuDieuTriId == phieuDieuTriId)
                .Include(cc => cc.PhienXetNghiemChiTiets)
                .Include(cc => cc.NhanVienKetLuan).ThenInclude(cc => cc.User)
                .Include(cc => cc.NhanVienThucHien).ThenInclude(cc => cc.User)
                .Include(cc => cc.NhomDichVuBenhVien);

            var query = yeuCauKyThuats.Where(x => (x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien) &&
                                                  (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh))
                                                               .Select(s => new KetQuaCLSGridVo()
                                                               {
                                                                   Id = s.Id,
                                                                   NoiDung = s.TenDichVu,
                                                                   NguoiThucHien = s.NhanVienThucHien != null ? s.NhanVienThucHien.User.HoTen : null,
                                                                   NgayThucHien = s.ThoiDiemThucHien != null ? s.ThoiDiemThucHien.Value.ApplyFormatDateTimeSACH() : null,
                                                                   BacSiKetLuan = s.NhanVienKetLuan != null ? s.NhanVienKetLuan.User.HoTen : null,
                                                                   NgayKetLuan = s.ThoiDiemKetLuan != null ? s.ThoiDiemKetLuan.Value.ApplyFormatDateTimeSACH() : null,
                                                                   LoaiKetQuaId = s.NhomDichVuBenhVien.NhomDichVuBenhVienChaId,
                                                                   LoaiKetQuaCLS = s.LoaiDichVuKyThuat.GetDescription(),
                                                                   YeuCauTiepNhanId = yeuCauTiepNhanId,
                                                                   IsDisable = true,
                                                               });

            query = query.ApplyLike(queryInfo.SearchTerms.ToLower(), g => g.NoiDung, g => g.NgayThucHien, g => g.BacSiKetLuan, g => g.BacSiKetLuanRemoveDictrict, g => g.NoiDungRemoveDictrict);

            var dataOrderBy = query.AsQueryable().OrderBy(queryInfo.SortString);
            var countTask = dataOrderBy.Count();

            return new GridDataSource { TotalRowCount = countTask };
        }

        #endregion

        #region  Xét nghiệm 

        public GridDataSource GetDataNoiTruKetQuaXetNghiem(long yeuCauTiepNhanId, long phieuDieuTriHienTaiId)
        {
            var yeuCauDichVuKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                          x.NoiTruPhieuDieuTriId == phieuDieuTriHienTaiId &&
                                                                         (x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien) && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                                                                         .Include(x => x.NhanVienThucHien).ThenInclude(x => x.User)
                                                                         .Include(x => x.NhanVienKetLuan).ThenInclude(x => x.User);
            var lstYeuCauDichVuKyThuatId = yeuCauDichVuKyThuats.Select(c => c.Id).ToList();

            var kqXetNghiemCTs = new List<KetQuaXetNghiemChiTiet>();
            if (lstYeuCauDichVuKyThuatId.Any())
            {
                kqXetNghiemCTs = _ketQuaXetNghiemChiTietRepository.TableNoTracking
                .Where(o => lstYeuCauDichVuKyThuatId.Contains(o.YeuCauDichVuKyThuatId) && o.PhienXetNghiemChiTiet.ThoiDiemKetLuan != null)
                .Include(o => o.PhienXetNghiemChiTiet).ThenInclude(o => o.NhomDichVuBenhVien)
                .Include(x => x.NhomDichVuBenhVien)
                .Include(x => x.YeuCauDichVuKyThuat)
                .Include(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                .Include(x => x.DichVuKyThuatBenhVien)
                .Include(x => x.MayXetNghiem)
                .Include(x => x.DichVuXetNghiem)
                .ToList();
            }

            var listChiTiet = new List<KetQuaXetNghiemChiTiet>();
            var chiTietKetQuaXetNghiems = new List<KQXetNghiemChiTiet>();

            foreach (var yeuCauDichVuKyThuatId in lstYeuCauDichVuKyThuatId)
            {
                var allkqXetNghiemCTs = kqXetNghiemCTs.Where(o => o.YeuCauDichVuKyThuatId == yeuCauDichVuKyThuatId);
                if (!allkqXetNghiemCTs.Any()) continue;

                var res = allkqXetNghiemCTs.GroupBy(o => o.PhienXetNghiemChiTietId).OrderBy(o => o.Key).Last().ToList();

                listChiTiet.AddRange(res);
            }

            listChiTiet = listChiTiet.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList();
            chiTietKetQuaXetNghiems = AddDetailDataChild(listChiTiet, listChiTiet, new List<KQXetNghiemChiTiet>(), true);

            return new GridDataSource { Data = chiTietKetQuaXetNghiems.ToArray() };
        }

        public GridDataSource GetDataNoiTruKetQuaXetNghiemOld(long yeuCauTiepNhanId , long phieuDieuTriHienTaiId)
        { 
            var yeuCauDichVuKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(x => x.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                          x.NoiTruPhieuDieuTriId == phieuDieuTriHienTaiId &&
                                                                         (x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien) && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                                                                         .Include(x => x.NhanVienThucHien).ThenInclude(x => x.User)
                                                                         .Include(x => x.NhanVienKetLuan).ThenInclude(x => x.User);
            var lstYeuCauDichVuKyThuatId = yeuCauDichVuKyThuats.Select(c => c.Id);
            var phienXetNghiemCTs = yeuCauDichVuKyThuats.SelectMany(c => c.PhienXetNghiemChiTiets).Where(c=>c.ThoiDiemKetLuan != null)
                                            .Include(x => x.KetQuaXetNghiemChiTiets)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.NhomDichVuBenhVien)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.DichVuKyThuatBenhVien)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.MayXetNghiem)
                                            .Include(x => x.NhomDichVuBenhVien)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.DichVuXetNghiem)
                                            .Include(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienYeuCau).ThenInclude(x => x.User)
                                            .Include(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User);            

            var listChiTiet = new List<KetQuaXetNghiemChiTiet>();
            var chiTietKetQuaXetNghiems = new List<KQXetNghiemChiTiet>();

            foreach (var yeuCauDichVuKyThuatId in lstYeuCauDichVuKyThuatId)
            {
                if (!phienXetNghiemCTs.Where(p => p.YeuCauDichVuKyThuatId == yeuCauDichVuKyThuatId).Last().KetQuaXetNghiemChiTiets.Any()) continue;
                var res = phienXetNghiemCTs.Where(p => p.YeuCauDichVuKyThuatId == yeuCauDichVuKyThuatId).Last().KetQuaXetNghiemChiTiets.ToList();

                listChiTiet.AddRange(res);
            }

            listChiTiet = listChiTiet.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList();
            chiTietKetQuaXetNghiems = AddDetailDataChild(listChiTiet, listChiTiet, new List<KQXetNghiemChiTiet>(), true);

            return new GridDataSource { Data = chiTietKetQuaXetNghiems.ToArray() };
        }      

        private List<KQXetNghiemChiTiet> AddDetailDataChild(List<KetQuaXetNghiemChiTiet> lstChiTietNhomConLai,
                                                            List<KetQuaXetNghiemChiTiet> lstChiTietNhomChild, List<KQXetNghiemChiTiet> result,
                                                            bool theFirst = false, int level = 1)
        {
            if (!lstChiTietNhomChild.Any() && theFirst != true) return result;

            List<long> lstIdSearch = new List<long>();
            //add root
            if (theFirst)
            {
                var lstParent = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == null).ToList();
                foreach (var parent in lstParent)
                {
                    var ketQua = new KQXetNghiemChiTiet
                    {
                        Id = parent.Id,
                        Ten = parent.YeuCauDichVuKyThuat.TenDichVu,
                        YeuCauDichVuKyThuatId = parent.YeuCauDichVuKyThuatId,
                        GiaTriCu = parent.GiaTriCu,
                        GiaTriNhapTay = parent.GiaTriNhapTay,
                        GiaTriTuMay = parent.GiaTriTuMay,
                        GiaTriDuyet = parent.GiaTriDuyet,
                        ToDamGiaTri = parent.ToDamGiaTri,
                        Csbt = (!string.IsNullOrEmpty(parent.GiaTriMin) ? parent.GiaTriMin + " - " : "") + (!string.IsNullOrEmpty(parent.GiaTriMax) ? parent.GiaTriMax : ""),
                        DonVi = parent.DonVi,
                        //Duyet = parent.DaDuyet ?? false,
                        Duyet = parent.NhanVienDuyetId != null,
                        ThoiDiemGuiYeuCau = parent.ThoiDiemGuiYeuCau,
                        ThoiDiemNhanKetQua = parent.ThoiDiemNhanKetQua,
                        MayXetNghiemId = parent.MayXetNghiemId,
                        TenMayXetNghiem = parent.MayXetNghiem?.Ten,
                        ThoiDiemDuyetKetQua = parent.ThoiDiemDuyetKetQua,
                        NguoiDuyet = parent.NhanVienDuyet?.User.HoTen,
                        LoaiMau = parent.DichVuKyThuatBenhVien.LoaiMauXetNghiem.GetDescription(),
                        DichVuXetNghiemId = parent.DichVuXetNghiemId,
                        DaGoiDuyet = parent.PhienXetNghiemChiTiet?.DaGoiDuyet,
                        //structure tree
                        Level = level,
                        Nhom = parent.NhomDichVuBenhVien.Ten,
                        NhomId = parent.NhomDichVuBenhVienId,
                        IdChilds = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId
                            && p.YeuCauDichVuKyThuatId == parent.YeuCauDichVuKyThuatId).Select(p => p.Id).ToList(),
                        NhomDichVuBenhVienId = parent.NhomDichVuBenhVienId,
                    };
                    lstIdSearch.Add(parent.DichVuXetNghiemId);
                    result.Add(ketQua);
                }
            }
            else
            {
                var lstReOrderBySTT = lstChiTietNhomChild.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList();
                foreach (var parent in lstReOrderBySTT)
                {
                    var ketQua = new KQXetNghiemChiTiet
                    {
                        Id = parent.Id,
                        Ten = parent.DichVuXetNghiem?.Ten,
                        YeuCauDichVuKyThuatId = parent.YeuCauDichVuKyThuatId,
                        GiaTriCu = parent.GiaTriCu,
                        GiaTriNhapTay = parent.GiaTriNhapTay,
                        GiaTriTuMay = parent.GiaTriTuMay,
                        GiaTriDuyet = parent.GiaTriDuyet,
                        ToDamGiaTri = parent.ToDamGiaTri,
                        Csbt = (!string.IsNullOrEmpty(parent.GiaTriMin) ? parent.GiaTriMin + " - " : "") + (!string.IsNullOrEmpty(parent.GiaTriMax) ? parent.GiaTriMax : ""),
                        DonVi = parent.DonVi,
                        Duyet = parent.NhanVienDuyetId != null,
                        ThoiDiemGuiYeuCau = parent.ThoiDiemGuiYeuCau,
                        ThoiDiemNhanKetQua = parent.ThoiDiemNhanKetQua,
                        MayXetNghiemId = parent.MayXetNghiemId,
                        TenMayXetNghiem = parent.MayXetNghiem?.Ten,
                        ThoiDiemDuyetKetQua = parent.ThoiDiemDuyetKetQua,
                        NguoiDuyet = parent.NhanVienDuyet?.User.HoTen,
                        DaGoiDuyet = parent.PhienXetNghiemChiTiet?.DaGoiDuyet,
                        LoaiMau = parent.NhomDichVuBenhVien.Ten,
                        DichVuXetNghiemId = parent.DichVuXetNghiemId,
                        //structure tree
                        Level = level,

                        LoaiKetQuaTuMay = BenhVienHelper.GetStatusForXetNghiem(parent.GiaTriMin, parent.GiaTriMax
                            , parent.GiaTriNguyHiemMin, parent.GiaTriNguyHiemMax
                            , parent.GiaTriTuMay),
                        NhomId = parent.NhomDichVuBenhVienId,
                        Nhom = parent.NhomDichVuBenhVien.Ten,
                        IdChilds = lstChiTietNhomConLai.Where(p => p.DichVuXetNghiemChaId == parent.DichVuXetNghiemId
                                                                    && p.YeuCauDichVuKyThuatId == parent.YeuCauDichVuKyThuatId).Select(p => p.Id).ToList(),
                        NhomDichVuBenhVienId = parent.NhomDichVuBenhVienId,
                        DichVuXetNghiemChaId = parent.DichVuXetNghiemChaId,

                    };
                    lstIdSearch.Add(parent.DichVuXetNghiemId);
                    var index = result.FindIndex(x => x.DichVuXetNghiemId == parent.DichVuXetNghiemChaId);
                    if (index >= 0)
                    {
                        var listChilds = result.Count(x => x.DichVuXetNghiemChaId == parent.DichVuXetNghiemChaId);
                        result.Insert(index + 1 + listChilds, ketQua);
                    }
                }
            }

            lstIdSearch = lstIdSearch.Distinct().ToList();
            var lstChiTietChild = lstChiTietNhomConLai.Where(p => lstIdSearch.Any(o => o == p.DichVuXetNghiemChaId)).ToList();
            level++;
            return AddDetailDataChild(lstChiTietNhomConLai, lstChiTietChild, result, false, level);
        }

        #endregion

        #endregion

        #region BVHD-3575
        public async Task<GridDataSource> GetDataForGridLichSuKhamAsync(QueryInfo queryInfo)
        {
            var data = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauTiepNhanNoiTruId = long.Parse(data[0]);
            //var phieuDieuTriId = long.Parse(data[1]);

            #region BVHD-3893
            long phieuDieuTriId = 0;
            long? yeuCauTiepNhanNgoaiTruId = null;
            DateTime? ngayDieuTri = null;
            bool loadTatCaDichVuKham = false;

            //BVHD-3893: trường hợp màn hình khám bệnh, chỉ truyền YCTN ngoại trú Id, ko truyền phiếu điều trị Id
            if (data.Length >= 2)
            {
                phieuDieuTriId = long.Parse(data[1]);
                var phieuDieuTri = _noiTruPhieuDieuTriRepository.TableNoTracking
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.YeuCauTiepNhan)
                    .FirstOrDefault(x => x.Id == phieuDieuTriId);
                yeuCauTiepNhanNgoaiTruId = phieuDieuTri?.NoiTruBenhAn?.YeuCauTiepNhan?.YeuCauTiepNhanNgoaiTruCanQuyetToanId;
                ngayDieuTri = phieuDieuTri?.NgayDieuTri.Date;
            }
            else
            {
                //BVHD-3893: dùng biến này để gán tạm YCTN Id trường hợp màn hình khám bệnh
                yeuCauTiepNhanNgoaiTruId = yeuCauTiepNhanNoiTruId;

                //màn hình khám bệnh thì load hết tất cả dv khám đã hoàn thành
                loadTatCaDichVuKham = true;
            }
            #endregion

            //var phieuDieuTri = _noiTruPhieuDieuTriRepository.TableNoTracking
            //    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.YeuCauTiepNhan)
            //    .FirstOrDefault(x => x.Id == phieuDieuTriId);
            //var yeuCauTiepNhanNgoaiTruId = phieuDieuTri?.NoiTruBenhAn?.YeuCauTiepNhan?.YeuCauTiepNhanNgoaiTruCanQuyetToanId;
            //var ngayDieuTri = phieuDieuTri?.NgayDieuTri.Date;

            var query = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(o => o.YeuCauTiepNhanId == yeuCauTiepNhanNgoaiTruId

                            && (loadTatCaDichVuKham || 
                            (
                                o.LaChiDinhTuNoiTru != null
                                && o.LaChiDinhTuNoiTru == true
                                //BVHD-3893
                                && (phieuDieuTriId == 0 || o.ThoiDiemDangKy.Date == ngayDieuTri)
                            ))

                            && o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham)
                .ApplyLike(queryInfo.SearchTerms?.Trim(), x => x.TenDichVu, x => x.BacSiThucHien.User.HoTen, x => x.BacSiKetLuan.User.HoTen)
                .Select(s => new DanhSachDaKhamGridVo
                {
                    Id = s.Id,
                    YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                    MaBN = $"{s.YeuCauTiepNhan.BenhNhanId ?? 0}",
                    Phong = s.NoiKetLuan.Ten,
                    BaSiKham = s.BacSiThucHien != null ? s.BacSiThucHien.User.HoTen : null,
                    ThoiDiemThucHien = s.ThoiDiemThucHien,
                    BacSiKetLuan = s.BacSiKetLuanId != null ? s.BacSiKetLuan.User.HoTen : null,
                    ThoiDiemHoanThanh = s.ThoiDiemHoanThanh,
                    HoTen = s.YeuCauTiepNhan.HoTen,
                    LyDoKham = s.TrieuChungTiepNhan,
                    ThoiDiemDangKy = s.ThoiDiemHoanThanh,
                    ThoiDiemDangKyDisplay = s.ThoiDiemDangKy.ApplyFormatDate(),
                    CachGiaiQuyet = s.CachGiaiQuyet,
                    //TrieuChungLamSang = s.YeuCauKhamBenhTrieuChungs.Where(x => x.YeuCauKhamBenhId == s.Id).FirstOrDefault().TrieuChung.Ten,
                    ChuanDoanICD = s.GhiChuICDChinh,
                    TenDichVuKham = s.TenDichVu
                }).OrderByDescending(p => p.ThoiDiemDangKy);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalPageForGridLichSuKhamAsync(QueryInfo queryInfo)
        {
            var data = queryInfo.AdditionalSearchString.Split(";");
            var yeuCauTiepNhanNoiTruId = long.Parse(data[0]);
            //var phieuDieuTriId = long.Parse(data[1]);

            #region BVHD-3893
            long phieuDieuTriId = 0;
            long? yeuCauTiepNhanNgoaiTruId = null;
            DateTime? ngayDieuTri = null;
            bool loadTatCaDichVuKham = false;

            //BVHD-3893: trường hợp màn hình khám bệnh, chỉ truyền YCTN ngoại trú Id, ko truyền phiếu điều trị Id
            if (data.Length >= 2)
            {
                phieuDieuTriId = long.Parse(data[1]);
                var phieuDieuTri = _noiTruPhieuDieuTriRepository.TableNoTracking
                    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.YeuCauTiepNhan)
                    .FirstOrDefault(x => x.Id == phieuDieuTriId);
                yeuCauTiepNhanNgoaiTruId = phieuDieuTri?.NoiTruBenhAn?.YeuCauTiepNhan?.YeuCauTiepNhanNgoaiTruCanQuyetToanId;
                ngayDieuTri = phieuDieuTri?.NgayDieuTri.Date;
            }
            else
            {
                //BVHD-3893: dùng biến này để gán tạm YCTN Id trường hợp màn hình khám bệnh
                yeuCauTiepNhanNgoaiTruId = yeuCauTiepNhanNoiTruId;

                //màn hình khám bệnh thì load hết tất cả dv khám đã hoàn thành
                loadTatCaDichVuKham = true;
            }
            #endregion

            //var phieuDieuTri = _noiTruPhieuDieuTriRepository.TableNoTracking
            //    .Include(x => x.NoiTruBenhAn).ThenInclude(x => x.YeuCauTiepNhan)
            //    .FirstOrDefault(x => x.Id == phieuDieuTriId);
            //var yeuCauTiepNhanNgoaiTruId = phieuDieuTri?.NoiTruBenhAn?.YeuCauTiepNhan?.YeuCauTiepNhanNgoaiTruCanQuyetToanId;
            //var ngayDieuTri = phieuDieuTri?.NgayDieuTri.Date;

            var query = _yeuCauKhamBenhRepository.TableNoTracking
                .Where(o => o.YeuCauTiepNhanId == yeuCauTiepNhanNgoaiTruId

                            && (loadTatCaDichVuKham ||
                                (
                                    o.LaChiDinhTuNoiTru != null
                                    && o.LaChiDinhTuNoiTru == true
                                    //BVHD-3893
                                    && (phieuDieuTriId == 0 || o.ThoiDiemDangKy.Date == ngayDieuTri)
                                ))

                            && o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.DaKham)
                .ApplyLike(queryInfo.SearchTerms?.Trim(), x => x.TenDichVu, x => x.BacSiThucHien.User.HoTen, x => x.BacSiKetLuan.User.HoTen)
                .Select(s => new DanhSachDaKhamGridVo
                {
                    Id = s.Id,
                });

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        #endregion
    }
}
