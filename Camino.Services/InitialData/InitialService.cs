using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DichVuXetNghiems;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.CauHinh;
using ExcelDataReader;
using Microsoft.EntityFrameworkCore.Internal;

namespace Camino.Services.InitialData
{
    [ScopedDependency(ServiceType = typeof(IInitialService))]
    public class InitialService : MasterFileService<Role>, IInitialService
    {
        private const string AdminRole = "Admin";
        private readonly IRepository<RoleFunction> _roleFunctionRepository;
        private readonly IRepository<Camino.Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem> _dichVuXetNghiemRepository;
        private readonly IRepository<NhomICDTheoBenhVien> _nhomICDTheoBenhVienRepository;
        private readonly IRepository<ChuongICD> _chuongICDRepository;
        private readonly IRepository<ICD> _ICDRepository;
        private ICauHinhService _cauHinhService;
        public InitialService(IRepository<Role> repository, IRepository<RoleFunction> roleFunctionRepository, IRepository<Camino.Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem> dichVuXetNghiemRepository,
            IRepository<ChuongICD> chuongICDRepository, IRepository<ICD> ICDRepository, ICauHinhService cauHinhService, IRepository<NhomICDTheoBenhVien> nhomICDTheoBenhVienRepository) : base(repository)
        {
            _roleFunctionRepository = roleFunctionRepository;
            _dichVuXetNghiemRepository = dichVuXetNghiemRepository;
            _nhomICDTheoBenhVienRepository = nhomICDTheoBenhVienRepository;
            _chuongICDRepository = chuongICDRepository;
            _ICDRepository = ICDRepository;
            _cauHinhService = cauHinhService;
        }

        public void DummyData()
        {
            //var thuePhong = new ThuePhong
            //{
            //    BlockThoiGianTheoPhut = 120,
            //    GiaThue = 5000000,
            //    GiaThuePhatSinh = 1000000,
            //    PhanTramNgoaiGio = 10,
            //    PhanTramLeTet = 15,
            //    PhanTramPhatSinhNgoaiGio = 10,
            //    PhanTramPhatSinhLeTet = 15,
            //    ThoiDiemBatDau = new DateTime(2022, 4, 24, 15, 30, 0),
            //    ThoiDiemKetThuc = new DateTime(2022, 4, 24, 18, 30, 0),
            //};
            //var test = _cauHinhService.GetDonGiaThuePhongAsync(thuePhong).Result;
            //var thuePhong1 = new ThuePhong
            //{
            //    BlockThoiGianTheoPhut = 120,
            //    GiaThue = 5000000,
            //    GiaThuePhatSinh = 1000000,
            //    PhanTramNgoaiGio = 10,
            //    PhanTramLeTet = 15,
            //    PhanTramPhatSinhNgoaiGio = 10,
            //    PhanTramPhatSinhLeTet = 15,
            //    ThoiDiemBatDau = new DateTime(2022, 4, 27, 15, 30, 0),
            //    ThoiDiemKetThuc = new DateTime(2022, 4, 27, 18, 30, 0),
            //};
            //var test1 = _cauHinhService.GetDonGiaThuePhongAsync(thuePhong1).Result;

            //var thuePhong2 = new ThuePhong
            //{
            //    BlockThoiGianTheoPhut = 120,
            //    GiaThue = 5000000,
            //    GiaThuePhatSinh = 1000000,
            //    PhanTramNgoaiGio = 10,
            //    PhanTramLeTet = 15,
            //    PhanTramPhatSinhNgoaiGio = 10,
            //    PhanTramPhatSinhLeTet = 15,
            //    ThoiDiemBatDau = new DateTime(2022, 4, 30, 15, 30, 0),
            //    ThoiDiemKetThuc = new DateTime(2022, 4, 30, 18, 30, 0),
            //};
            //var test2 = _cauHinhService.GetDonGiaThuePhongAsync(thuePhong2).Result;

            //var thuePhong3 = new ThuePhong
            //{
            //    BlockThoiGianTheoPhut = 120,
            //    GiaThue = 5000000,
            //    GiaThuePhatSinh = 1000000,
            //    PhanTramNgoaiGio = 10,
            //    PhanTramLeTet = 15,
            //    PhanTramPhatSinhNgoaiGio = 10,
            //    PhanTramPhatSinhLeTet = 15,
            //    ThoiDiemBatDau = new DateTime(2022, 4, 27, 15, 0, 0),
            //    ThoiDiemKetThuc = new DateTime(2022, 4, 30, 17, 15, 0),
            //};
            //var test3 = _cauHinhService.GetDonGiaThuePhongAsync(thuePhong3).Result;

            //var thuePhong4 = new ThuePhong
            //{
            //    BlockThoiGianTheoPhut = 120,
            //    GiaThue = 5000000,
            //    GiaThuePhatSinh = 1000000,
            //    PhanTramNgoaiGio = 10,
            //    PhanTramLeTet = 15,
            //    PhanTramPhatSinhNgoaiGio = 10,
            //    PhanTramPhatSinhLeTet = 15,
            //    ThoiDiemBatDau = new DateTime(2022, 4, 27, 14, 59, 0),
            //    ThoiDiemKetThuc = new DateTime(2022, 4, 27, 21, 0, 0),
            //};
            //var test4 = _cauHinhService.GetDonGiaThuePhongAsync(thuePhong4).Result;

            //CheckICD();
            //DummyRole();
            //DummyPermissionAdmin();
            //DummyExcelNhomICDTheoBenhVien();
            //DummyExcelData();
        }

        #region private class

        private void DummyPermissionAdmin()
        {
            var listPermissionDefault = EnumHelper.GetListEnum<Enums.DocumentType>();
            var admin = BaseRepository.TableNoTracking.FirstOrDefault(p => p.Name.Contains(AdminRole));
            if (admin != null)
            {
                foreach (var permissionDefault in listPermissionDefault)
                {
                    var roleFunctionView = _roleFunctionRepository.TableNoTracking.FirstOrDefault(p =>
                        p.RoleId == admin.Id && p.SecurityOperation == Enums.SecurityOperation.View &&
                        p.DocumentType == permissionDefault);
                    if (roleFunctionView == null)
                    {
                        _roleFunctionRepository.Add(new RoleFunction
                        {
                            DocumentType = permissionDefault,
                            SecurityOperation = Enums.SecurityOperation.View,
                            RoleId = admin.Id
                        });
                    }

                    var roleFunctionAdd = _roleFunctionRepository.TableNoTracking.FirstOrDefault(p =>
                        p.RoleId == admin.Id && p.SecurityOperation == Enums.SecurityOperation.Add &&
                        p.DocumentType == permissionDefault);
                    if (roleFunctionAdd == null)
                    {
                        _roleFunctionRepository.Add(new RoleFunction
                        {
                            DocumentType = permissionDefault,
                            SecurityOperation = Enums.SecurityOperation.Add,
                            RoleId = admin.Id
                        });
                    }

                    var roleFunctionRemove = _roleFunctionRepository.TableNoTracking.FirstOrDefault(p =>
                        p.RoleId == admin.Id && p.SecurityOperation == Enums.SecurityOperation.Delete &&
                        p.DocumentType == permissionDefault);
                    if (roleFunctionRemove == null)
                    {
                        _roleFunctionRepository.Add(new RoleFunction
                        {
                            DocumentType = permissionDefault,
                            SecurityOperation = Enums.SecurityOperation.Delete,
                            RoleId = admin.Id
                        });
                    }

                    var roleFunctionUpdate = _roleFunctionRepository.TableNoTracking.FirstOrDefault(p =>
                        p.RoleId == admin.Id && p.SecurityOperation == Enums.SecurityOperation.Update &&
                        p.DocumentType == permissionDefault);
                    if (roleFunctionUpdate == null)
                    {
                        _roleFunctionRepository.Add(new RoleFunction
                        {
                            DocumentType = permissionDefault,
                            SecurityOperation = Enums.SecurityOperation.Update,
                            RoleId = admin.Id
                        });
                    }
                }
            }
        }
        private void DummyRole()
        {
            var listRoleDefault = EnumHelper.GetListEnum<Enums.DefaultRole>();
            foreach (var roleDefault in listRoleDefault)
            {
                if (!EnumerableExtensions.Any(
                    BaseRepository.TableNoTracking.Where(p => p.Name.Contains(roleDefault.GetDescription()))))
                {
                    var userType = Enums.UserType.KhachVangLai;
                    switch (roleDefault)
                    {
                        case Enums.DefaultRole.Admin:
                            userType = Enums.UserType.NhanVien;
                            break;
                        default:
                            break;
                    }
                    BaseRepository.Add(new Role
                    {
                        Name = roleDefault.GetDescription(),
                        UserType = userType,
                        IsDefault = true,
                    });
                }
            }
        }

        private long ChuyenNhomDichVuBenhVien(int serviceGroupId)
        {
            if (serviceGroupId == 2) return 112;
            if (serviceGroupId == 3) return 115;
            if (serviceGroupId == 4) return 117;
            if (serviceGroupId == 5) return 114;
            if (serviceGroupId == 6) return 113;
            if (serviceGroupId == 83) return 116;
            return serviceGroupId;
        }
        private void CheckICD()
        {
            var icdFilePath = "Resource//ICD4469.xlsx";
            Dictionary<string, string> icd4469 = new Dictionary<string, string>();
            if (File.Exists(icdFilePath))
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var stream = File.Open(icdFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        int count = 0;
                        ChuongICD chuongICD = null;
                        while (reader.Read()) //Each row of the file
                        {
                            if (count > 2)
                            {
                                try
                                {
                                    var maBenh = reader.GetValue(16).ToString();
                                    var tenBenh = reader.GetValue(19).ToString();
                                    if(maBenh == "J13" && icd4469.TryGetValue("J13", out var v))
                                    {
                                        continue;
                                    }
                                    icd4469.Add(maBenh, tenBenh);
                                }
                                catch (Exception e)
                                {
                                    return;
                                }
                            }
                            count++;
                        }
                    }
                }
            }
            var myICDs = _ICDRepository.TableNoTracking.Select(o=>new { o.Ma, o.TenTiengViet }).ToDictionary(x => x.Ma, x => x.TenTiengViet);

            var c = icd4469.Count();

            var icdThua = new List<string>();
            var icdThieu = new List<string>();

            foreach(var i in icd4469)
            {
                if (!myICDs.TryGetValue(i.Key, out var v))
                    icdThieu.Add(i.Key);
            }
            foreach (var i in myICDs)
            {
                if (!icd4469.TryGetValue(i.Key, out var v))
                    icdThua.Add(i.Key);
            }

        }
        private void DummyExcelNhomICDTheoBenhVien()
        {
            var nhomICDFilePath = "Resource//NhomICDTheoBenhVien.xlsx";

            if (!_nhomICDTheoBenhVienRepository.Table.Any())
            { 
                if (File.Exists(nhomICDFilePath))
                {
                    var chuongICDs = _chuongICDRepository.TableNoTracking.ToList();
                    List<NhomICDTheoBenhVien> nhomICDTheoBenhViens = new List<NhomICDTheoBenhVien>();
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    using (var stream = File.Open(nhomICDFilePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            int count = 0;
                            ChuongICD chuongICD = null;
                            while (reader.Read()) //Each row of the file
                            {
                                
                                try
                                {
                                    var stt = reader.GetValue(1).ToString();
                                    if (stt.ToLower().Contains("c"))
                                    {
                                        chuongICD = chuongICDs.First(o => o.Stt == stt);
                                        continue;
                                    }

                                    var ten = reader.GetValue(2).ToString();
                                    var arrTen = ten.Split('-');
                                    var tenTV = arrTen[0].Trim();
                                    var tenTA = arrTen.Length > 1 ? arrTen[1].Trim() : string.Empty;
                                    var maICD = reader.GetValue(3).ToString();
                                    var arrMaICD = maICD.Split(',');

                                    var maICDs = new List<string>();
                                    foreach(var icd in arrMaICD)
                                    {
                                        if (icd.Contains("-"))
                                        {
                                            var tuICD = icd.Split('-')[0].Trim();
                                            var denICD = icd.Split('-')[1].Trim();
                                            if (tuICD.StartsWith(denICD[0]))
                                            {
                                                int tuSoICD = int.Parse(tuICD.Substring(1, 2));
                                                int denSoICD = int.Parse(denICD.Substring(1, 2));
                                                for(int i = tuSoICD; i <= denSoICD; i++)
                                                {
                                                    maICDs.Add($"{tuICD[0]}{i.ToString("00")}");
                                                }
                                            }
                                            else
                                            {

                                            }
                                        }
                                        else
                                        {
                                            maICDs.Add(icd.Trim());
                                        }
                                    }

                                    var nhomICDTheoBenhVien = new NhomICDTheoBenhVien
                                    {
                                        ChuongICDId = chuongICD.Id,
                                        Stt = stt,
                                        Ma = maICD,
                                        TenTiengViet = tenTV,
                                        TenTiengAnh = tenTA,
                                        HieuLuc = true,
                                        MoTa = string.Join(';', maICDs)
                                    };
                                    nhomICDTheoBenhViens.Add(nhomICDTheoBenhVien);
                                }
                                catch (Exception e)
                                {
                                    return;
                                }

                                
                                count++;
                            }
                        }
                    }
                    if (nhomICDTheoBenhViens.Count > 0)
                    {
                        _nhomICDTheoBenhVienRepository.AddRange(nhomICDTheoBenhViens);
                    }
                }
            }
            else
            {
                List<NhomICDTheoBenhVien> nhomICDTheoBenhViens = _nhomICDTheoBenhVienRepository.TableNoTracking.ToList();
                List<string> maIcdTrungs = new List<string>();
                foreach (var nhomICDTheoBenhVien in nhomICDTheoBenhViens)
                {
                    var arrMaICD = nhomICDTheoBenhVien.MoTa.Split(';');
                    foreach (var maIcd in arrMaICD)
                    {
                        foreach (var nhomICDTheoBenhVienKhac in nhomICDTheoBenhViens.Where(o => o.Id != nhomICDTheoBenhVien.Id))
                        {
                            if (nhomICDTheoBenhVienKhac.MoTa.Split(';').Contains(maIcd))
                            {
                                maIcdTrungs.Add(maIcd);
                            }
                        }
                    }
                }

                var icds = _ICDRepository.TableNoTracking.Select(o=>o.Ma).ToList();
                var maNhomICDs = new List<string>();
                foreach (var nhomICDTheoBenhVien in nhomICDTheoBenhViens)
                {
                    var arrMaICD = nhomICDTheoBenhVien.MoTa.Split(';');
                    maNhomICDs.AddRange(arrMaICD.ToList());
                }

                var l = icds.Where(o => !maNhomICDs.Contains(o.Substring(0, 3))).ToList();
                var str = string.Join(',', l.Select(o=> o.Substring(0, 3)).Distinct());
            }
        }

        private void DummyExcelData()
        {
            var dvxnFilePath = "Resource//dvxn.xls";

            if (_dichVuXetNghiemRepository.Table.Any())
            {
                var dvs = _dichVuXetNghiemRepository.Table.ToList();
                foreach (var dichVuXetNghiem in dvs)
                {
                    if (dichVuXetNghiem.DichVuXetNghiemChaId == null && dichVuXetNghiem.Ma.Contains(".") && dichVuXetNghiem.CapDichVu > 1)
                    {
                        var maDvCha = dichVuXetNghiem.Ma.Substring(0, dichVuXetNghiem.Ma.LastIndexOf("."));
                        var dvCha = dvs.FirstOrDefault(o => o.Ma == maDvCha);
                        if (dvCha != null)
                        {
                            dichVuXetNghiem.DichVuXetNghiemChaId = dvCha.Id;
                        }
                    }
                }
                _dichVuXetNghiemRepository.Context.SaveChanges();
            }
            else
            {
                if (File.Exists(dvxnFilePath))
                {
                    List<Camino.Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem> dvs = new List<Camino.Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem>();
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    using (var stream = File.Open(dvxnFilePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            int count = 0;

                            while (reader.Read()) //Each row of the file
                            {
                                if (count != 0)
                                {
                                    try
                                    {
                                        var dv = new Camino.Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem
                                        {
                                            Id = (int)(reader.GetValue(0) as double?),
                                            //Id0 = (int)(reader.GetValue(0) as double?),
                                            //NhomDichVuBenhVienId0 = (int)(reader.GetValue(1) as double?),
                                            NhomDichVuBenhVienId = ChuyenNhomDichVuBenhVien((int)(reader.GetValue(1) as double?)),
                                            Ma = reader.GetValue(2) as string ?? (reader.GetValue(2) as double?).ToString(),
                                            Ten = reader.GetValue(3) as string,
                                            CapDichVu = (int)(reader.GetValue(4) as double?),
                                            DonVi = reader.GetValue(6) as string,
                                            SoThuTu = (int)(reader.GetValue(7) as double?),
                                            HieuLuc = true,
                                            NamMin = reader.GetValue(9) as string ?? (reader.GetValue(9) as double?).ToString(),
                                            NamMax = reader.GetValue(10) as string ?? (reader.GetValue(10) as double?).ToString(),
                                            NuMin = reader.GetValue(11) as string ?? (reader.GetValue(11) as double?).ToString(),
                                            NuMax = reader.GetValue(12) as string ?? (reader.GetValue(12) as double?).ToString(),
                                            TreEmMin = reader.GetValue(13) as string ?? (reader.GetValue(13) as double?).ToString(),
                                            TreEmMax = reader.GetValue(14) as string ?? (reader.GetValue(14) as double?).ToString(),
                                            NguyHiemMax = reader.GetValue(15) as string ?? (reader.GetValue(15) as double?).ToString(),
                                            NguyHiemMin = reader.GetValue(16) as string ?? (reader.GetValue(16) as double?).ToString(),
                                            KieuDuLieu = reader.GetValue(17) as string ?? (reader.GetValue(17) as double?).ToString(),
                                            TreEm6Min = reader.GetValue(18) as string ?? (reader.GetValue(18) as double?).ToString(),
                                            TreEm6Max = reader.GetValue(19) as string ?? (reader.GetValue(19) as double?).ToString(),
                                            TreEm612Min = reader.GetValue(20) as string ?? (reader.GetValue(20) as double?).ToString(),
                                            TreEm612Max = reader.GetValue(21) as string ?? (reader.GetValue(21) as double?).ToString(),
                                            TreEm1218Min = reader.GetValue(22) as string ?? (reader.GetValue(22) as double?).ToString(),
                                            TreEm1218Max = reader.GetValue(23) as string ?? (reader.GetValue(23) as double?).ToString()
                                        };
                                        if (dv.DonVi == "NULL") dv.DonVi = null;
                                        if (dv.NamMin == "NULL") dv.NamMin = null;
                                        if (dv.NamMax == "NULL") dv.NamMax = null;
                                        if (dv.NuMin == "NULL") dv.NuMin = null;
                                        if (dv.NuMax == "NULL") dv.NuMax = null;
                                        if (dv.TreEmMin == "NULL") dv.TreEmMin = null;
                                        if (dv.TreEmMax == "NULL") dv.TreEmMax = null;
                                        if (dv.NguyHiemMax == "NULL") dv.NguyHiemMax = null;
                                        if (dv.NguyHiemMin == "NULL") dv.NguyHiemMin = null;
                                        if (dv.KieuDuLieu == "NULL") dv.KieuDuLieu = null;
                                        if (dv.TreEm6Min == "NULL") dv.TreEm6Min = null;
                                        if (dv.TreEm6Max == "NULL") dv.TreEm6Max = null;
                                        if (dv.TreEm612Min == "NULL") dv.TreEm612Min = null;
                                        if (dv.TreEm612Max == "NULL") dv.TreEm612Max = null;
                                        if (dv.TreEm1218Min == "NULL") dv.TreEm1218Min = null;
                                        if (dv.TreEm1218Max == "NULL") dv.TreEm1218Max = null;

                                        dvs.Add(dv);

                                    }
                                    catch (Exception e)
                                    {
                                        return;
                                    }

                                }
                                count++;
                            }
                        }
                    }
                    if (dvs.Count > 0)
                    {
                        _dichVuXetNghiemRepository.AddRange(dvs);
                    }
                }
            }
        }

        #endregion private class
    }
}