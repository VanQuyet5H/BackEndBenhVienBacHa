using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Camino.Core.Configuration;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DichVuXetNghiems;
using Camino.Core.Domain.Entities.MayXetNghiems;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.BaoCaoLuuketQuaXeNghiemTrongNgay;
using Camino.Core.Domain.ValueObject.XetNghiems;

namespace Camino.Services.Helpers
{
    public static class LISHelper
    {
        public static KetQuaTuMayXetNghiem GetKetQuaTuMayXetNghiemHumaClot(string[] lines,string maMayXetNghiem)
        {
            if (lines.Length > 0)
            {
                var lineBarcode = lines.FirstOrDefault(o => o.StartsWith("sample_name"));
                if (lineBarcode != null)
                {
                    var ketQua = new KetQuaTuMayXetNghiem
                    {
                        BarCodeNumber = Convert.ToInt32(lineBarcode.Split(';')[1].Trim()),
                        MaMayXetNghiem = maMayXetNghiem
                    };
                    var lineMethodAbbrev = lines.FirstOrDefault(o => o.StartsWith("method_abbrev"));
                    if (lineMethodAbbrev != null)
                    {
                        var methodAbbrev = lineMethodAbbrev.Split(';')[1].Trim();
                        if (methodAbbrev == "PT-li")
                        {
                            foreach (var line in lines)
                            {
                                var lineSplit = line.Split(';');
                                if (lineSplit[0].Trim() == "clot")
                                {
                                    var valueAndUnit = lineSplit[1].Replace("=", "").Trim().Split(' ');
                                    var ketQuaChiTiet = new KetQuaTuMayXetNghiemChiTiet
                                    {
                                        MaChiSo = "PCR",
                                        GiaTri = valueAndUnit[0].Replace(",","."),
                                        DonVi = valueAndUnit.Length > 1 ? valueAndUnit[1] : string.Empty
                                    };
                                    ketQua.KetQuaTuMayXetNghiemChiTiets.Add(ketQuaChiTiet);
                                }
                                else if (lineSplit[0].Trim() == "clot-conv1")
                                {
                                    var valueAndUnit = lineSplit[1].Replace("=", "").Trim().Split(' ');
                                    var ketQuaChiTiet = new KetQuaTuMayXetNghiemChiTiet
                                    {
                                        MaChiSo = "PC1",
                                        GiaTri = valueAndUnit[0].Replace(",", "."),
                                        DonVi = valueAndUnit.Length > 1 ? valueAndUnit[1] : string.Empty
                                    };
                                    ketQua.KetQuaTuMayXetNghiemChiTiets.Add(ketQuaChiTiet);
                                }
                                else if (lineSplit[0].Trim() == "clot-conv2")
                                {
                                    var valueAndUnit = lineSplit[1].Replace("=", "").Trim().Split(' ');
                                    var ketQuaChiTiet = new KetQuaTuMayXetNghiemChiTiet
                                    {
                                        MaChiSo = "PC2",
                                        GiaTri = valueAndUnit[0].Replace(",", "."),
                                        DonVi = valueAndUnit.Length > 1 ? valueAndUnit[1] : string.Empty
                                    };
                                    ketQua.KetQuaTuMayXetNghiemChiTiets.Add(ketQuaChiTiet);
                                }
                            }
                        }
                        else if(methodAbbrev == "aPTT")
                        {
                            foreach (var line in lines)
                            {
                                var lineSplit = line.Split(';');
                                if (lineSplit[0].Trim() == "clot")
                                {
                                    var valueAndUnit = lineSplit[1].Replace("=", "").Trim().Split(' ');
                                    var ketQuaChiTiet = new KetQuaTuMayXetNghiemChiTiet
                                    {
                                        MaChiSo = "ACR",
                                        GiaTri = valueAndUnit[0].Replace(",", "."),
                                        DonVi = valueAndUnit.Length > 1 ? valueAndUnit[1] : string.Empty
                                    };
                                    ketQua.KetQuaTuMayXetNghiemChiTiets.Add(ketQuaChiTiet);
                                }
                                else if (lineSplit[0].Trim() == "clot-conv2")
                                {
                                    var valueAndUnit = lineSplit[1].Replace("=", "").Trim().Split(' ');
                                    var ketQuaChiTiet = new KetQuaTuMayXetNghiemChiTiet
                                    {
                                        MaChiSo = "AC2",
                                        GiaTri = valueAndUnit[0].Replace(",", "."),
                                        DonVi = valueAndUnit.Length > 1 ? valueAndUnit[1] : string.Empty
                                    };
                                    ketQua.KetQuaTuMayXetNghiemChiTiets.Add(ketQuaChiTiet);
                                }
                            }
                        }
                        else if (methodAbbrev == "Fib")
                        {
                            foreach (var line in lines)
                            {
                                var lineSplit = line.Split(';');
                                if (lineSplit[0].Trim() == "clot-conv1")
                                {
                                    var valueAndUnit = lineSplit[1].Replace("=", "").Trim().Split(' ');
                                    var ketQuaChiTiet = new KetQuaTuMayXetNghiemChiTiet
                                    {
                                        MaChiSo = "FC1",
                                        GiaTri = valueAndUnit[0].Replace(",", "."),
                                        DonVi = valueAndUnit.Length > 1 ? valueAndUnit[1] : string.Empty
                                    };
                                    ketQua.KetQuaTuMayXetNghiemChiTiets.Add(ketQuaChiTiet);
                                }
                            }
                        }
                        return ketQua;
                    }
                }
            }
            return null;
        }

        public static KetQuaTuMayXetNghiem GetKetQuaTuMayXetNghiem(string content, LISConfig lisConfig, List<MayXetNghiem> mayXetNghiems)
        {
            var lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            return GetKetQuaTuMayXetNghiem(lines, lisConfig, mayXetNghiems);
        }

        public static KetQuaTuMayXetNghiem GetKetQuaTuMayXetNghiem(string[] lines, LISConfig lisConfig, List<MayXetNghiem> mayXetNghiems)
        {
            if (lines.Length > 0 && lisConfig.BarCodeNumberPositions != null && lisConfig.BarCodeNumberPositions.Length > 0)
            {
                //var barcode = FindValueWithPositions(lines, lisConfig.BarCodeNumberPositions, out var positionIndex);
                var r = FindBarCodeAndDevice(lines, lisConfig, mayXetNghiems, out var positionIndex);
                if (!string.IsNullOrEmpty(r.Item1))
                {
                    var ketQua = new KetQuaTuMayXetNghiem
                    {
                        BarCodeNumber = Convert.ToInt32(r.Item1),
                        MaMayXetNghiem = r.Item2
                    };
                    var assayCodePosition = lisConfig.AssayCodePositions[positionIndex].Split('.');
                    var resultValuePosition = lisConfig.ResultValuePositions[positionIndex].Split('.');
                    var resultUnitPosition = lisConfig.ResultUnitPositions[positionIndex].Split('.');
                    var resultTimePosition = lisConfig.ResultTimePositions[positionIndex].Split('.');
                    foreach (var line in lines)
                    {
                        var assayCode = FindValue(line, assayCodePosition[0], Convert.ToInt32(assayCodePosition[1]), Convert.ToInt32(assayCodePosition[2]));
                        if (lisConfig.DeviceModelIds[positionIndex] == "1043" && assayCode != null && assayCode.IndexOf("/") > 0)
                        {
                            assayCode = assayCode.Substring(0, assayCode.IndexOf("/"));
                        }
                        if (!string.IsNullOrEmpty(assayCode))
                        {
                            string giaTriUuTien = null;
                            string preFlag = null;
                            if (lisConfig.DeviceModelIds[positionIndex] == "1043")
                            {
                                giaTriUuTien = FindValue(line, resultValuePosition[0], Convert.ToInt32(resultValuePosition[1]), 1);
                                var r6 = FindValue(line, "R", 6, 0);
                                if (r6 != null && r6.Equals("HH"))
                                {
                                    preFlag = ">";
                                }
                                else if (r6 != null && r6.Equals("LL"))
                                {
                                    preFlag = "<";
                                }

                            }
                            var ketQuaChiTiet = new KetQuaTuMayXetNghiemChiTiet
                            {
                                MaChiSo = assayCode,
                                GiaTri = (preFlag??string.Empty) +(!string.IsNullOrEmpty(giaTriUuTien) ? giaTriUuTien : FindValue(line, resultValuePosition[0], Convert.ToInt32(resultValuePosition[1]), Convert.ToInt32(resultValuePosition[2]))),
                                DonVi = FindValue(line, resultUnitPosition[0], Convert.ToInt32(resultUnitPosition[1]), Convert.ToInt32(resultUnitPosition[2])),
                                ThoiGian = FindValue(line, resultTimePosition[0], Convert.ToInt32(resultTimePosition[1]), Convert.ToInt32(resultTimePosition[2])),
                            };
                            ketQua.KetQuaTuMayXetNghiemChiTiets.Add(ketQuaChiTiet);
                        }
                    }
                    return ketQua;
                }
            }
            return null;
        }

        public static (Enums.EnumNhomMau?, Enums.EnumYeuToRh?) GetKetQuaNhomMau(ICollection<KetQuaXetNghiemChiTiet> ketQuaXetNghiemChiTiets)
        {
            Enums.EnumNhomMau? nhomMauABO = null;
            Enums.EnumYeuToRh? nhomMauRh = null;

            foreach (var ketQuaXetNghiemChiTiet in ketQuaXetNghiemChiTiets)
            {
                if (ketQuaXetNghiemChiTiet.DichVuXetNghiemTen != null && ketQuaXetNghiemChiTiet.DichVuXetNghiemTen.ToLower().Contains("định nhóm máu"))
                {
                    var ketQua = !string.IsNullOrEmpty(ketQuaXetNghiemChiTiet.GiaTriDuyet)
                        ? ketQuaXetNghiemChiTiet.GiaTriDuyet
                        : (!string.IsNullOrEmpty(ketQuaXetNghiemChiTiet.GiaTriNhapTay) ? ketQuaXetNghiemChiTiet.GiaTriNhapTay : ketQuaXetNghiemChiTiet.GiaTriTuMay);
                    if (ketQua == "A" || ketQua == "\"A\"")
                    {
                        nhomMauABO = Enums.EnumNhomMau.A;
                    }
                    if (ketQua == "B" || ketQua == "\"B\"")
                    {
                        nhomMauABO = Enums.EnumNhomMau.B;
                    }
                    if (ketQua == "O" || ketQua == "\"O\"")
                    {
                        nhomMauABO = Enums.EnumNhomMau.O;
                    }
                    if (ketQua == "AB" || ketQua == "\"AB\"")
                    {
                        nhomMauABO = Enums.EnumNhomMau.AB;
                    }
                    if (ketQua == "Rh(+)" || ketQua == "Rh+")
                    {
                        nhomMauRh = Enums.EnumYeuToRh.DuongTinh;
                    }
                    if (ketQua == "Rh(-)" || ketQua == "Rh-")
                    {
                        nhomMauRh = Enums.EnumYeuToRh.Amtinh;
                    }
                }
            }

            return (nhomMauABO, nhomMauRh);
        }

        private static (string, string) FindBarCodeAndDevice(string[] lines, LISConfig lisConfig, List<MayXetNghiem> mayXetNghiems, out int positionIndex)
        {
            positionIndex = -1;
            for (int i = 0; i < lisConfig.BarCodeNumberPositions.Length; i++)
            {
                var barcode = FindValue(lines, lisConfig.BarCodeNumberPositions[i]);
                var device = FindValue(lines, lisConfig.DeviceCodePositions[i]);
                
                if (barcode != null && device != null)
                {
                    var mayXetNghiem = mayXetNghiems.FirstOrDefault(o =>
                        o.Ma.ToLower().Equals(device.ToLower()) &&
                        o.MauMayXetNghiemID.ToString() == lisConfig.DeviceModelIds[i]);
                    if (mayXetNghiem != null)
                    {
                        positionIndex = i;
                        return (barcode, device);
                    }
                }
            }
            return (null, null);
        }

        //private static string FindValueWithPositions(string[] lines, string[] positions, out int positionIndex)
        //{
        //    positionIndex = -1;
        //    for (int i = 0; i < positions.Length; i++)
        //    {
        //        var result = FindValue(lines, positions[i]);
        //        if (result != null)
        //        {
        //            positionIndex = i;
        //            return result;
        //        }
        //    }
        //    return null;
        //}

        private static string FindValue(string[] lines, string position)
        {
            var partsOfPosition = position.Split('.');
            int segIndex = Convert.ToInt32(partsOfPosition[1]);
            int segSubIndex = Convert.ToInt32(partsOfPosition[2]);
            foreach (var line in lines)
            {
                var r = FindValue(line, partsOfPosition[0], segIndex, segSubIndex);
                if (r != null) return r;
            }
            return null;
        }

        private static string FindValue(string line, string prefix, int segIndex, int segSubIndex)
        {
            var segs = line.Split('|');
            if (segs[0].Equals(prefix))
            {
                if (segs.Length > segIndex)
                {
                    var segSubs = segs[segIndex].Split('^');
                    if (segSubs.Length > segSubIndex && segSubs[segSubIndex].Trim().Length > 0)
                    {
                        return segSubs[segSubIndex].Trim();
                    }
                }
            }
            return null;
        }

        //Sắp xếp theo thứ tự Nhóm : 205 Huyết học -> 204 Sinh hoá -> 208 Vi sinh- > 252 Nước tiểu -> 246 Sinh học phân tử -> 113 Tế bào giải phẫu bệnh
        private static long GetThuTuSapXepTheoNhom(long nhomDichVuBenhVienId)
        {
            switch (nhomDichVuBenhVienId)
            {
                case 205:
                    return 1;
                case 204:
                    return 2;
                case 208:
                    return 3;
                case 252:
                    return 4;
                case 246:
                    return 5;
                case 113:
                    return 6;
                default:
                    return nhomDichVuBenhVienId + 6;
            }
        }

        private static string GetTenKetQuaXetNghiem(string tenDichVu, long dichVuXetNghiemId, long? mauMayXetNghiemId, List<DichVuXetNghiemKetNoiChiSo> dichVuXetNghiemKetNoiChiSos)
        {
            var ketNoiChiSo = dichVuXetNghiemKetNoiChiSos.FirstOrDefault(o => o.DichVuXetNghiemId == dichVuXetNghiemId &&
                (mauMayXetNghiemId == null || o.MauMayXetNghiemId == mauMayXetNghiemId));
            return ketNoiChiSo?.TenKetNoi ?? tenDichVu;
        }

        public static string GetKetQuaCuaPhienXetNghiem(IEnumerable<KetQuaPhienXetNghiemChiTiet> ketQuaPhienXetNghiemChiTiets, List<DichVuXetNghiemKetNoiChiSo> dichVuXetNghiemKetNoiChiSos)
        {
            if (ketQuaPhienXetNghiemChiTiets == null || dichVuXetNghiemKetNoiChiSos == null)
                return string.Empty;
            var groupByDichVu = ketQuaPhienXetNghiemChiTiets.GroupBy(o => o.YeuCauDichVuKyThuatId);
            var ketQuaPhienXetNghiemChiTietLasts = groupByDichVu.Select(o => o.OrderBy(i => i.LanThucHien).Last());
            var groupByNhom = ketQuaPhienXetNghiemChiTietLasts.GroupBy(o => o.NhomDichVuBenhVienId).OrderBy(o=> GetThuTuSapXepTheoNhom(o.Key));
            List<string> strKetTheoNhoms = new List<string>();
            foreach (var nhomKetQua in groupByNhom)
            {
                var ketQuaCap1s = nhomKetQua
                    .Select(o => new
                        {
                            o.YeuCauDichVuKyThuatId,
                            o.YeuCauDichVuKyThuatTen,
                        ketQuaChiTiet = o.KetQuaXetNghiemChiTiets.FirstOrDefault(ct => ct.CapDichVu == 1 && ct.ThoiDiemDuyetKetQua != null)
                        })
                    .Where(o=>o.ketQuaChiTiet != null)
                    .OrderBy(o=>o.ketQuaChiTiet.SoThuTu??0).ThenByDescending(o=>o.ketQuaChiTiet.DichVuXetNghiemId);
                List<string> strKetQuaCap1s = new List<string>();
                foreach (var ketQuaCap1 in ketQuaCap1s)
                {
                    var tenDichVu = GetTenKetQuaXetNghiem(ketQuaCap1.YeuCauDichVuKyThuatTen,ketQuaCap1.ketQuaChiTiet.DichVuXetNghiemId, ketQuaCap1.ketQuaChiTiet.MauMayXetNghiemId,dichVuXetNghiemKetNoiChiSos);
                    var ketQuaCap2s = nhomKetQua
                        .First(o=>o.YeuCauDichVuKyThuatId == ketQuaCap1.YeuCauDichVuKyThuatId)
                        .KetQuaXetNghiemChiTiets.Where(ct => ct.CapDichVu==2).OrderBy(ct => ct.SoThuTu ?? 0).ThenByDescending(ct => ct.DichVuXetNghiemId).ToList();
                    if (ketQuaCap2s.Any())
                    {
                        List<string> strKetQuaCap2s = new List<string>();
                        foreach (var ketQuaXetNghiemChiTiet in ketQuaCap2s)
                        {
                            var tenXetNghiem = GetTenKetQuaXetNghiem(ketQuaXetNghiemChiTiet.DichVuXetNghiemTen,
                                ketQuaXetNghiemChiTiet.DichVuXetNghiemId, ketQuaXetNghiemChiTiet.MauMayXetNghiemId,
                                dichVuXetNghiemKetNoiChiSos);
                            var ketQua = !string.IsNullOrEmpty(ketQuaXetNghiemChiTiet.GiaTriDuyet)
                                ? ketQuaXetNghiemChiTiet.GiaTriDuyet
                                : (!string.IsNullOrEmpty(ketQuaXetNghiemChiTiet.GiaTriNhapTay) ? ketQuaXetNghiemChiTiet.GiaTriNhapTay : ketQuaXetNghiemChiTiet.GiaTriTuMay);
                            strKetQuaCap2s.Add($"{tenXetNghiem}:{ketQua}");
                        }
                        strKetQuaCap1s.Add($"{tenDichVu}:{string.Join(';', strKetQuaCap2s)}");
                    }
                    else
                    {
                        var ketQua = !string.IsNullOrEmpty(ketQuaCap1.ketQuaChiTiet.GiaTriDuyet)
                            ? ketQuaCap1.ketQuaChiTiet.GiaTriDuyet
                            : (!string.IsNullOrEmpty(ketQuaCap1.ketQuaChiTiet.GiaTriNhapTay) ? ketQuaCap1.ketQuaChiTiet.GiaTriNhapTay : ketQuaCap1.ketQuaChiTiet.GiaTriTuMay);
                        strKetQuaCap1s.Add($"{tenDichVu}:{ketQua}");
                    }
                }
                strKetTheoNhoms.Add($"{nhomKetQua.First().NhomDichVuBenhVienTen}: {string.Join(';', strKetQuaCap1s)}");
            }
            return string.Join(Environment.NewLine, strKetTheoNhoms);
        }

        public static string GetKetQuaDichVuXetNghiem(string tenDichVuKyThuat, List<KetQuaChiSoXetNghiemChiTietVo> ketQuaXetNghiemChiTiets, List<DichVuXetNghiemKetNoiChiSo> dichVuXetNghiemKetNoiChiSos)
        {
            var tenDichVu = tenDichVuKyThuat;
            var ketQuaCap1 = ketQuaXetNghiemChiTiets.FirstOrDefault(ct => ct.CapDichVu == 1 && ct.ThoiDiemDuyetKetQua != null);
            var ketQuaCap2s = ketQuaXetNghiemChiTiets.Where(ct => ct.CapDichVu == 2 && ct.ThoiDiemDuyetKetQua != null).OrderBy(ct => ct.SoThuTu ?? 0).ThenByDescending(ct => ct.DichVuXetNghiemId).ToList();
            if (ketQuaCap2s.Any())
            {
                List<string> strKetQuaCap2s = new List<string>();
                foreach (var ketQuaXetNghiemChiTiet in ketQuaCap2s)
                {
                    var tenXetNghiem = GetTenKetQuaXetNghiem(ketQuaXetNghiemChiTiet.DichVuXetNghiemTen,
                        ketQuaXetNghiemChiTiet.DichVuXetNghiemId, ketQuaXetNghiemChiTiet.MauMayXetNghiemId,
                        dichVuXetNghiemKetNoiChiSos);
                    var ketQua = !string.IsNullOrEmpty(ketQuaXetNghiemChiTiet.GiaTriDuyet)
                        ? ketQuaXetNghiemChiTiet.GiaTriDuyet
                        : (!string.IsNullOrEmpty(ketQuaXetNghiemChiTiet.GiaTriNhapTay) ? ketQuaXetNghiemChiTiet.GiaTriNhapTay : ketQuaXetNghiemChiTiet.GiaTriTuMay);
                    strKetQuaCap2s.Add($"{tenXetNghiem}:{ketQua}");
                }
                return $"{tenDichVu} : {string.Join("; ", strKetQuaCap2s)}";
            }
            else
            {
                var ketQua = !string.IsNullOrEmpty(ketQuaCap1?.GiaTriDuyet)
                    ? ketQuaCap1.GiaTriDuyet
                    : (!string.IsNullOrEmpty(ketQuaCap1?.GiaTriNhapTay) ? ketQuaCap1.GiaTriNhapTay : ketQuaCap1?.GiaTriTuMay);
                return $"{tenDichVu} : {ketQua}";
            }
        }

        public static string GetChiSoTrungBinh(string giaTriMin, string giaTriMax)
        {
            if (!string.IsNullOrEmpty(giaTriMin) && !string.IsNullOrEmpty(giaTriMax))
            {
                return $"{giaTriMin} - {giaTriMax}";
            }
            if (!string.IsNullOrEmpty(giaTriMin))
            {
                return giaTriMin;
            }
            if (!string.IsNullOrEmpty(giaTriMax))
            {
                return giaTriMax;
            }
            return string.Empty;
        }
    }
}
