using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.BHYT;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Helpers;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Camino.Services.BHYT;
using Camino.Core.Domain.ValueObject.BHYT;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Text;
using Camino.Api.Models.HamGuiHoSoWatchings;
using Camino.Services.HamGuiHoSoWatchings;
using Camino.Core.Domain.Entities.HamGuiHoSoWatchings;
using System.Xml.Linq;
using Camino.Services.GoiBaoHiemYTe;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.GoiBaoHiemYTe;
using System.Globalization;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;


namespace Camino.Api.Controllers
{
    public partial class BHYTController : CaminoBaseController
    {
        private IBaoHiemYTeService _BHYTService;
        private readonly ILocalizationService _localizationService;
        private IMapper _mapper;
        private IHamGuiHoSoWatchingService _hamGuiHoSoService;
        private IGoiBaoHiemYTeService _goiBaoHiemYTeService;
        private readonly IExcelService _excelService;


        public BHYTController(ILocalizationService localizationService, IBaoHiemYTeService BHYTService, IMapper mapper,
                              IExcelService excelService, IGoiBaoHiemYTeService goiBaoHiemYTeService,
                              IHamGuiHoSoWatchingService hamGuiHoSoService)
        {
            _BHYTService = BHYTService;
            _goiBaoHiemYTeService = goiBaoHiemYTeService;
            _localizationService = localizationService;
            _hamGuiHoSoService = hamGuiHoSoService;
            _excelService = excelService;
            _mapper = mapper;
        }


        #region Cập nhật gởi cổng BHYT 31/03/2022

        [HttpPost("GuiChiTietTungHoSoGiamDinhNguoiBenhs")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GuiBaoHiemYTe)]
        public async Task<ActionResult> GuiChiTietTungHoSoGiamDinhNguoiBenhs(ThongTinBenhNhan thongTinChiTietTungHoSoGiamDinh)
        {
            if (thongTinChiTietTungHoSoGiamDinh == null)
                throw new ApiException($"Không tìm thấy thông tin người bệnh");
            if(thongTinChiTietTungHoSoGiamDinh.HoSoChiTietThuoc != null)
            {
                foreach (var thuoc in thongTinChiTietTungHoSoGiamDinh.HoSoChiTietThuoc)
                {
                    if (string.IsNullOrEmpty(thuoc.MaLienKet))
                    {
                        thuoc.MaLienKet = thongTinChiTietTungHoSoGiamDinh.MaLienKet;
                    }
                }
            }
            if (thongTinChiTietTungHoSoGiamDinh.HoSoCanLamSang != null)
            {
                foreach (var cls in thongTinChiTietTungHoSoGiamDinh.HoSoCanLamSang)
                {
                    if (string.IsNullOrEmpty(cls.MaLienKet))
                    {
                        cls.MaLienKet = thongTinChiTietTungHoSoGiamDinh.MaLienKet;
                    }
                }
            }
                

            if (thongTinChiTietTungHoSoGiamDinh.HoSoChiTietDVKT != null)
            {
                foreach (var dvkt in thongTinChiTietTungHoSoGiamDinh.HoSoChiTietDVKT)
                {
                    if (string.IsNullOrEmpty(dvkt.MaLienKet))
                    {
                        dvkt.MaLienKet = thongTinChiTietTungHoSoGiamDinh.MaLienKet;
                    }
                }
            }                

            if (thongTinChiTietTungHoSoGiamDinh.HoSoChiTietDienBienBenh != null)
            {
                foreach (var db in thongTinChiTietTungHoSoGiamDinh.HoSoChiTietDienBienBenh)
                {
                    if (string.IsNullOrEmpty(db.MaLienKet))
                    {
                        db.MaLienKet = thongTinChiTietTungHoSoGiamDinh.MaLienKet;
                    }
                }
            }

            var watchingData = new HamGuiHoSoWatchingViewModel();

            var dataJson = JsonConvert.SerializeObject(thongTinChiTietTungHoSoGiamDinh);
            watchingData.DataJson = dataJson;
            watchingData.TimeSend = DateTime.Now;

            //Bước 1: Mã hóa file xml byte gởi qua bảo hiểm y tế mỗi XML convert stringBase64
            var thongTinBenhNhans = new List<ThongTinBenhNhan>();
            thongTinBenhNhans.Add(thongTinChiTietTungHoSoGiamDinh);

            var resultData = await _goiBaoHiemYTeService.GoiHoSoGiamDinhs(thongTinBenhNhans);
            if (resultData.ErrorCheck != true)
            {

                // Bước 2: Sau Khi Gởi Thành Công Lưu Kết Quả Trong Hệ Thống
                _goiBaoHiemYTeService.LuuLichSuDuLieuGuiCongBHYT(thongTinBenhNhans, true);
            }
            else
            {
                watchingData.XMLError = resultData.XMLError;
                watchingData.APIError = resultData.APIError;
                watchingData.ErrorCheck = resultData.ErrorCheck;
                watchingData.ErrorAPICheck = resultData.ErrorCheck;
            }

            return Ok(watchingData);
        }

        [HttpPost("DownloadTungHoSoGiamDinhNguoiBenh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GuiBaoHiemYTe)]
        public ActionResult DownloadTungHoSoGiamDinhNguoiBenh(ThongTinBenhNhan thongTinChiTietTungHoSoGiamDinh)
        {
            if (thongTinChiTietTungHoSoGiamDinh == null)
                throw new ApiException($"Không tìm thấy thông tin người bệnh");

            var thongTinBenhNhans = new List<ThongTinBenhNhan>();
            var watchingData = new HamGuiHoSoWatchingViewModel();

            thongTinBenhNhans.Add(thongTinChiTietTungHoSoGiamDinh);
            var resultData = _goiBaoHiemYTeService.addValueToXml(thongTinBenhNhans);

            foreach (var item in resultData.TenFileVOs)
            {
                watchingData.TenFileVOs.Add(new TenFileVO { TenFile = item.TenFile, DuLieu = item.DuLieu });
            }

            watchingData.NameFileDown = resultData.NameFileDown;
            watchingData.countFile = resultData.countFile;

            return Ok(watchingData);

        }

        #region màn hình lịch sử hồ sơ giám định

        [HttpPost("CapNhatDownloadHoSoGiamDinhXMLLichSuBHYT")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GuiBaoHiemYTe)]
        public ActionResult<int> CapNhatDownloadHoSoGiamDinhXMLLichSuBHYT(ThongTinBenhNhan thongTinBenhNhanViewModel)
        {
            var result = new List<ThongTinBenhNhan>();
            result.Add(thongTinBenhNhanViewModel);
            return Ok(_goiBaoHiemYTeService.LuuLichSuDuLieuGuiCongBHYT(result, false));
        }

        [HttpPost("GuiChiTietTungHoSoGiamDinhNguoiBenhLichSuBHYT")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GuiBaoHiemYTe)]
        public async Task<ActionResult> GuiChiTietTungHoSoGiamDinhNguoiBenhLichSuBHYT(ThongTinBenhNhan thongTinChiTietTungHoSoGiamDinh)
        {
            if (thongTinChiTietTungHoSoGiamDinh == null)
                throw new ApiException($"Không tìm thấy thông tin người bệnh");

            if (thongTinChiTietTungHoSoGiamDinh.HoSoChiTietThuoc != null)
            {
                foreach (var thuoc in thongTinChiTietTungHoSoGiamDinh.HoSoChiTietThuoc)
                {
                    if (string.IsNullOrEmpty(thuoc.MaLienKet))
                    {
                        thuoc.MaLienKet = thongTinChiTietTungHoSoGiamDinh.MaLienKet;
                    }
                }
            }
            if (thongTinChiTietTungHoSoGiamDinh.HoSoCanLamSang != null)
            {
                foreach (var cls in thongTinChiTietTungHoSoGiamDinh.HoSoCanLamSang)
                {
                    if (string.IsNullOrEmpty(cls.MaLienKet))
                    {
                        cls.MaLienKet = thongTinChiTietTungHoSoGiamDinh.MaLienKet;
                    }
                }
            }


            if (thongTinChiTietTungHoSoGiamDinh.HoSoChiTietDVKT != null)
            {
                foreach (var dvkt in thongTinChiTietTungHoSoGiamDinh.HoSoChiTietDVKT)
                {
                    if (string.IsNullOrEmpty(dvkt.MaLienKet))
                    {
                        dvkt.MaLienKet = thongTinChiTietTungHoSoGiamDinh.MaLienKet;
                    }
                }
            }

            if (thongTinChiTietTungHoSoGiamDinh.HoSoChiTietDienBienBenh != null)
            {
                foreach (var db in thongTinChiTietTungHoSoGiamDinh.HoSoChiTietDienBienBenh)
                {
                    if (string.IsNullOrEmpty(db.MaLienKet))
                    {
                        db.MaLienKet = thongTinChiTietTungHoSoGiamDinh.MaLienKet;
                    }
                }
            }

            var watchingData = new HamGuiHoSoWatchingViewModel();

            var dataJson = JsonConvert.SerializeObject(thongTinChiTietTungHoSoGiamDinh);
            watchingData.DataJson = dataJson;
            watchingData.TimeSend = DateTime.Now;

            //Bước 1: Mã hóa file xml byte gởi qua bảo hiểm y tế mỗi XML convert stringBase64
            var thongTinBenhNhans = new List<ThongTinBenhNhan>();
            thongTinBenhNhans.Add(thongTinChiTietTungHoSoGiamDinh);

            var resultData = await _goiBaoHiemYTeService.GoiHoSoGiamDinhs(thongTinBenhNhans);
            if (resultData.ErrorCheck != true)
            {

                // Bước 2: Sau Khi Gởi Thành Công Lưu Kết Quả Trong Hệ Thống
                _goiBaoHiemYTeService.LuuLichSuDuLieuGuiCongBHYT(thongTinBenhNhans, true);
            }
            else
            {
                watchingData.XMLError = resultData.XMLError;
                watchingData.APIError = resultData.APIError;
                watchingData.ErrorCheck = resultData.ErrorCheck;
                watchingData.ErrorAPICheck = resultData.ErrorCheck;
            }

            return Ok(watchingData);
        }

        #endregion


        #endregion

        [HttpPost("GetLichSuKhamChuaBenh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.None)]
        public ActionResult GetLichSuKhamChuaBenh(ThongTinBenhNhanModel thongTinBenhNhan)
        {
            if (thongTinBenhNhan.NgaySinh != null)
            {
                thongTinBenhNhan.NgaySinh = thongTinBenhNhan.NgaySinh.GetValueOrDefault().Date;
            }
            var benhNhanThongTin = _mapper.Map<ThongTinBenhNhanXemVO>(thongTinBenhNhan);
            var result = _BHYTService.GetThongTin(benhNhanThongTin);
            //var result = GetThongTin(thongTinBenhNhan);
            if (result != null && !(result.maKetQua.Equals("000") || result.maKetQua.Equals("004")))
            {
                ShowLoi(result.maKetQua);
            }
            if (result != null)
            {
                if (result.maKetQua.Equals("401"))
                {
                    result = _BHYTService.GetTokenAndAutoResendThongTin(benhNhanThongTin);
                    if (result == null)
                    {
                        throw new ApiException(_localizationService.GetResource("BHYT.CannotAccess"));
                    }
                    if (result != null && !(result.maKetQua.Equals("000") || result.maKetQua.Equals("004")))
                    {
                        ShowLoi(result.maKetQua);
                    }
                }
            }
            else
            {
                throw new ApiException(_localizationService.GetResource("BHYT.CannotAccess"));
            }

            return Ok(result);
        }
        [HttpPost("GetEnumGioiTinh")]
        public Task<List<LookupItemVo>> GetEnumGioiTinh()
        {
            var listEnum = EnumHelper.GetListEnum<Enums.LoaiGioiTinh>();

            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            })
                .ToList();
            return Task.FromResult(result);
        }
        [HttpPost("GetEnumLyDoVaoVien")]
        public Task<List<LookupItemVo>> GetEnumLyDoVaoVien()
        {
            var listEnum = EnumHelper.GetListEnum<Enums.EnumLyDoVaoVien>();

            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            })
                .ToList();
            return Task.FromResult(result);
        }
        [HttpPost("GetEnumMatainan")]
        public Task<List<LookupItemVo>> GetEnumMatainan()
        {
            var listEnum = EnumHelper.GetListEnum<Enums.EnumMaTaiNan>();

            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            })
                .ToList();
            return Task.FromResult(result);
        }
        [HttpPost("GetEnumKetQuaDieuTri")]
        public Task<List<LookupItemVo>> GetEnumKetQuaDieuTri()
        {
            var listEnum = EnumHelper.GetListEnum<Enums.EnumKetQuaDieuTri>();

            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            })
                .ToList();
            return Task.FromResult(result);
        }
        [HttpPost("GetEnumTinhTrangRaVien")]
        public Task<List<LookupItemVo>> GetEnumTinhTrangRaVien()
        {
            var listEnum = EnumHelper.GetListEnum<Enums.EnumTinhTrangRaVien>();

            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            })
                .ToList();
            return Task.FromResult(result);
        }
        [HttpPost("GetEnumMaLoaiKCB")]
        public Task<List<LookupItemVo>> GetEnumMaLoaiKCB()
        {
            var listEnum = EnumHelper.GetListEnum<Enums.EnumMaHoaHinhThucKCB>();

            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            })
                .ToList();
            return Task.FromResult(result);
        }
        [HttpPost("GetEnumMaPhuongThucThanhToan")]
        public Task<List<LookupItemVo>> GetEnumMaPhuongThucThanhToan()
        {
            var listEnum = EnumHelper.GetListEnum<Enums.EnumMaPhuongThucThanhToan>();

            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            })
                .ToList();
            return Task.FromResult(result);
        }
        [HttpPost("GetMaNhom")]
        public Task<List<LookupItemVo>> GetMaNhom()
        {
            var listEnum = EnumHelper.GetListEnum<Enums.EnumDanhMucNhomTheoChiPhi>();

            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).Where(x => x.KeyId == 4 || x.KeyId == 5 || x.KeyId == 6 || x.KeyId == 7)
                .ToList();
            return Task.FromResult(result);
        }
        [HttpPost("GetMaNhomDVKT")]
        public Task<List<LookupItemVo>> GetMaNhomDVKT()
        {
            var listEnum = EnumHelper.GetListEnum<Enums.EnumDanhMucNhomTheoChiPhi>();

            var result = listEnum.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).Where(x => x.KeyId == 1 || x.KeyId == 2 || x.KeyId == 3 || x.KeyId == 8 || x.KeyId == 9 || x.KeyId == 10 || x.KeyId == 11 || x.KeyId == 12 || x.KeyId == 13 || x.KeyId == 14 || x.KeyId == 15 || x.KeyId == 18)
                .ToList();
            return Task.FromResult(result);
        }
        [HttpPost("XacNhanUser")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.None)]
        public ActionResult XacNhanUser(userConfirmViewModel thongTinViewModel)
        {
            if (thongTinViewModel.userName.Equals("01824_BV") && thongTinViewModel.pass.Equals("@hoaiduc123"))
            {
                return Ok(true);
            }
            return Ok(false);
        }

        [HttpPost("GetLichSuChiTietKhamChuaBenh")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.None)]
        public ActionResult GetLichSuChiTietKhamChuaBenh()
        {
            var token = TokenBHYT.GetTokenAPI().token;
            var id_token = TokenBHYT.GetTokenAPI().id_token;

            var takeAPI = new RestClient($"http://egw.baohiemxahoi.gov.vn/api/egw/nhanHoSoKCBChiTiet?token=" + token + "&id_token=" + id_token + "&username=01824_BV&password=35f60479a061e7c58e415f47e5b087af&maHoSo=1230235961");
            var requesttoken = new RestRequest(Method.POST);
            IRestResponse responseLichSu = takeAPI.Execute(requesttoken);
            if (responseLichSu.IsSuccessful)
            {
                var content = JsonConvert.DeserializeObject<ChiTietLichSuKhamBenhModel>(responseLichSu.Content);
                if (Convert.ToInt64(content.maKetQua) == 401)
                {
                    var takeAPItoken = new RestClient($"http://egw.baohiemxahoi.gov.vn/api/token/take");
                    var requesttokenAPI = new RestRequest(Method.POST);
                    requesttokenAPI.AddJsonBody(
                                        new { username = "01824_BV", password = "35f60479a061e7c58e415f47e5b087af" }); // uses JsonSerializerTokenBHYTModel
                    IRestResponse responsetoken = takeAPItoken.Execute(requesttokenAPI);
                    if (responsetoken.IsSuccessful)
                    {
                        var contentToken = JsonConvert.DeserializeObject<ThongTinTokenMoiViewModel>(responsetoken.Content);
                        TokenBHYT.ModifyTokenBHYT(contentToken.APIKey.access_token, contentToken.APIKey.id_token);
                        token = TokenBHYT.GetTokenAPI().token;
                        id_token = TokenBHYT.GetTokenAPI().id_token;
                        takeAPI = new RestClient($"http://egw.baohiemxahoi.gov.vn/api/egw/nhanHoSoKCBChiTiet?token=" + token + "&id_token=" + id_token + "&username=01824_BV&password=35f60479a061e7c58e415f47e5b087af&maHoSo=1230235961");
                        requesttoken = new RestRequest(Method.POST);
                        responseLichSu = takeAPI.Execute(requesttoken);
                        if (responseLichSu.IsSuccessful)
                        {
                            content = JsonConvert.DeserializeObject<ChiTietLichSuKhamBenhModel>(responseLichSu.Content);
                        }
                        return Ok();
                    }
                }
                return Ok();
            }

            return Ok();
        }
        [HttpPost("GetThongTinCSKCB")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.None)]
        public ActionResult GetThongTinCSKCB()
        {
            var takeAPI1 = new RestClient($"http://egw.baohiemxahoi.gov.vn/api/token/take");
            var requesttoken1 = new RestRequest(Method.POST);
            requesttoken1.AddJsonBody(
                                new { username = "01824_BV", password = "35f60479a061e7c58e415f47e5b087af" }); // uses JsonSerializerTokenBHYTModel
            IRestResponse responsetoken = takeAPI1.Execute(requesttoken1);
            if (responsetoken.IsSuccessful)
            {
                var contentToken = JsonConvert.DeserializeObject<ThongTinTokenMoiViewModel>(responsetoken.Content);
                TokenBHYT.ModifyTokenBHYT(contentToken.APIKey.access_token, contentToken.APIKey.id_token);
            }
            var token = TokenBHYT.GetTokenAPI().token;
            var id_token = TokenBHYT.GetTokenAPI().id_token;

            var takeAPI = new RestClient($"http://egw.baohiemxahoi.gov.vn/api/egw/NhanThongTinCSKCB?username=01824_BV&password=35f60479a061e7c58e415f47e5b087af&token=" + token + "&id_token=" + id_token + "&macskcb=01824");
            var requesttoken = new RestRequest(Method.POST);
            IRestResponse responseLichSu = takeAPI.Execute(requesttoken);
            if (responseLichSu.IsSuccessful)
            {
                //var content = JsonConvert.DeserializeObject<ChiTietLichSuKhamBenhModel>(responseLichSu.Content);


                return Ok();
            }

            return Ok();
        }
        [HttpGet("DownloadXML")]
        public ActionResult DownloadXML(string NameFileDown, string nameFile)
        {
            var byteFileTongHop = _BHYTService.pathFileTongHop(NameFileDown);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + nameFile + ".xml");
            this.Response.ContentType = "application/xml";
            return new FileContentResult(byteFileTongHop, "application/xml");

        }
        [HttpPost("DeleteXML")]
        public ActionResult DeleteXML(string NameFileDown)
        {
            System.IO.File.Delete(@NameFileDown);
            return Ok();

        }

        private ThongTinBHYTViewModel GetThongTin(ThongTinBenhNhanModel thongTinBenhNhan)
        {
            var month = thongTinBenhNhan.NgaySinh?.Month < 10 ? "0" + thongTinBenhNhan.NgaySinh?.Month : thongTinBenhNhan.NgaySinh?.Month + "";
            var day = thongTinBenhNhan.NgaySinh?.Day < 10 ? "0" + thongTinBenhNhan.NgaySinh?.Day : thongTinBenhNhan.NgaySinh?.Day + "";
            var ngaySinh = day + "/" + month + "/" + thongTinBenhNhan.NgaySinh?.Year;
            var token = TokenBHYT.GetTokenAPI().token;
            var id_token = TokenBHYT.GetTokenAPI().id_token;
            var client = new RestClient($"http://egw.baohiemxahoi.gov.vn/api/egw/KQNhanLichSuKCB2019?token=" + token + "&id_token=" + id_token + "&username=01824_BV&password=35f60479a061e7c58e415f47e5b087af");

            var ngaySinhSendRequest = ngaySinh;
            if (thongTinBenhNhan.NgaySinh == null)
            {
                ngaySinhSendRequest = thongTinBenhNhan.NamSinh + "";
            }

            var request = new RestRequest(Method.POST);
            request.AddJsonBody(
                new { maThe = thongTinBenhNhan.MaThe, hoTen = thongTinBenhNhan.TenBenhNhan, ngaySinh = ngaySinhSendRequest }); // uses JsonSerializer
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                var content = JsonConvert.DeserializeObject<ThongTinBHYTViewModel>(response.Content);
                return content;

            }
            return null;
        }
        private void ShowLoi(string loiApi)
        {
            if (loiApi == "051")
            {
                throw new ApiException(_localizationService.GetResource("Mã thẻ không đúng"),
                            (int)HttpStatusCode.BadRequest);
            }
            else
                if (loiApi == "060")
            {
                throw new ApiException(_localizationService.GetResource("Họ tên không đúng"),
                            (int)HttpStatusCode.BadRequest);
            }
            else
                if (loiApi == "061")
            {
                throw new ApiException(_localizationService.GetResource("Họ tên không đúng (đúng kí tự đầu)"),
                            (int)HttpStatusCode.BadRequest);
            }
            else
                if (loiApi == "070")
            {
                throw new ApiException(_localizationService.GetResource("Ngày sinh không đúng"),
                            (int)HttpStatusCode.BadRequest);
            }
            else
                if (loiApi == "205")
            {
                throw new ApiException(_localizationService.GetResource("Ngày sinh không đúng"),
                            (int)HttpStatusCode.BadRequest);
            }
            else
                if (loiApi == "001")
            {
                throw new ApiException(_localizationService.GetResource("Thẻ do BHXH Bộ Quốc Phòng quản lý, đề nghị kiểm tra thẻ và thông tin giấy tờ tùy thân"),
                    (int)HttpStatusCode.BadRequest);
            }
            else
                if (loiApi == "002")
            {
                throw new ApiException(_localizationService.GetResource("Thẻ do BHXH Bộ Công An quản lý, đề nghị kiểm tra thẻ và thông tin giấy tờ tùy thân"),
                    (int)HttpStatusCode.BadRequest);
            }
            else
                if (loiApi == "003")
            {
                throw new ApiException(_localizationService.GetResource("Thẻ cũ hết giá trị sử dụng nhưng đã được cấp thẻ mới"),
                    (int)HttpStatusCode.BadRequest);
            }
            //else
            //    if (loiApi == "004")
            //{
            //    throw new ApiException(_localizationService.GetResource("Thẻ cũ còn giá trị sử dụng nhưng đã được cấp thẻ mới"),
            //        (int)HttpStatusCode.BadRequest);
            //}
            else
                if (loiApi == "010")
            {
                throw new ApiException(_localizationService.GetResource("Thẻ hết giá trị sử dụng"),
                    (int)HttpStatusCode.BadRequest);
            }
            else
                if (loiApi == "052")
            {
                throw new ApiException(_localizationService.GetResource("Mã tỉnh cấp thẻ(kí tự thứ 4,5 của mã thẻ) không đúng"),
                    (int)HttpStatusCode.BadRequest);
            }
            else
                if (loiApi == "053")
            {
                throw new ApiException(_localizationService.GetResource("Mã quyền lợi thẻ(kí tự thứ 3 của mã thẻ) không đúng"),
                    (int)HttpStatusCode.BadRequest);
            }
            else
                if (loiApi == "050")
            {
                throw new ApiException(_localizationService.GetResource("Không thấy thông tin thẻ bhyt"),
                    (int)HttpStatusCode.BadRequest);
            }
            else
                if (loiApi == "100")
            {
                throw new ApiException(_localizationService.GetResource("Lỗi khi lấy dữ liệu sổ thẻ"),
                    (int)HttpStatusCode.BadRequest);
            }
            else
                if (loiApi == "101")
            {
                throw new ApiException(_localizationService.GetResource("Lỗi server"),
                    (int)HttpStatusCode.BadRequest);
            }
            else
                if (loiApi == "110")
            {
                throw new ApiException(_localizationService.GetResource("Thẻ đã thu hồi"),
                    (int)HttpStatusCode.BadRequest);
            }
            else
                if (loiApi == "120")
            {
                throw new ApiException(_localizationService.GetResource("Thẻ đã báo giảm"),
                    (int)HttpStatusCode.BadRequest);
            }
            else
                if (loiApi == "121")
            {
                throw new ApiException(_localizationService.GetResource("Thẻ đã báo giảm. Giảm chuyển ngoại tỉnh"),
                    (int)HttpStatusCode.BadRequest);
            }
            else
                if (loiApi == "122")
            {
                throw new ApiException(_localizationService.GetResource("Thẻ đã báo giảm. Giảm chuyển nội tỉnh"),
                    (int)HttpStatusCode.BadRequest);
            }
            else
                if (loiApi == "123")
            {
                throw new ApiException(_localizationService.GetResource("Thẻ đã báo giảm. Thu hồi do tăng lại cùng đơn vị"),
                    (int)HttpStatusCode.BadRequest);
            }
            else
                if (loiApi == "124")
            {
                throw new ApiException(_localizationService.GetResource("Thẻ đã báo giảm.Ngừng tham gia"),
                    (int)HttpStatusCode.BadRequest);
            }
            else
                if (loiApi == "130")
            {
                throw new ApiException(_localizationService.GetResource("Trẻ em không xuất trình thẻ"),
                    (int)HttpStatusCode.BadRequest);
            }
            else
                if (loiApi == "205")
            {
                throw new ApiException(_localizationService.GetResource("Lỗi sai định dạng tham số truyền vào"),
                    (int)HttpStatusCode.BadRequest);
            }

            // riêng lỗi 401 phải xử lý get lại tocken
            //else
            //    if (loiApi == "401")
            //{
            //    throw new ApiException(_localizationService.GetResource("Lỗi xác thực tài khoản"),
            //        (int)HttpStatusCode.BadRequest);
            //}
        }
        [HttpPost("GetLichSuKhamChuaBenh2")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.None)]
        public ActionResult GetLichSuKhamChuaBenh2()
        {
            var token = TokenBHYT.GetTokenAPI().token;
            var id_token = TokenBHYT.GetTokenAPI().id_token;
            var client = new RestClient($"http://egw.baohiemxahoi.gov.vn/api/egw/nhanLichSuKCB?token=" + token + "&id_token=" + id_token + "&username=01824_BV&password=35f60479a061e7c58e415f47e5b087af");

            var request = new RestRequest(Method.POST);
            request.AddJsonBody(
                new
                {
                    maThe = "DN4797932331580",
                    hoTen = "Nguyễn Hữu Khánh Hoàng",
                    ngaySinh = "12/03/1997",
                    gioiTinh = 1,
                    maCSKCB = "79009",
                    ngayBD = "01/09/2019",
                    ngayKT = "31/12/2019"
                }); // uses JsonSerializer
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                var content = JsonConvert.DeserializeObject<ThongTinBHYTViewModel>(response.Content);


            }
            return Ok();
        }
        //var result = GetThongTin(thongTinBenhNhan);

        [HttpPost("GetInfoFromURL")]
        public ActionResult GetInfoFromURL(string model)
        {
            //var result = new ThongTinBHYTViewModel();
            var lstString = model.Split('|');



            if (lstString.Any())
            {
                var maThe = lstString[0].Substring(lstString[0].Length - 15, lstString[0].Length - (lstString[0].Length - 15));

                var modelThongTin = new ThongTinBenhNhanModel
                {
                    MaThe = maThe,
                    NgaySinh = DateTime.ParseExact(lstString[2], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                    TenBenhNhan = CommonHelper.DecodeHexString(lstString[1]),
                };
                var benhNhanThongTin = _mapper.Map<ThongTinBenhNhanXemVO>(modelThongTin);
                //result = GetThongTin(modelThongTin);
                var result = _BHYTService.GetThongTin(benhNhanThongTin);
                if (result != null && !(result.maKetQua.Equals("000") || result.maKetQua.Equals("004")))
                {
                    ShowLoi(result.maKetQua);
                }
                if (result != null)
                {
                    if (result.maKetQua.Equals("401"))
                    {
                        result = _BHYTService.GetTokenAndAutoResendThongTin(benhNhanThongTin);
                        if (result == null)
                        {
                            throw new ApiException(_localizationService.GetResource("BHYT.CannotAccess"));
                        }
                        if (result != null && !(result.maKetQua.Equals("000") || result.maKetQua.Equals("004")))
                        {
                            ShowLoi(result.maKetQua);
                        }
                    }
                }
                else
                {
                    //throw new ApiException(_localizationService.GetResource("BHYT.CannotAccess"));

                    var resultNotConnect1 = new ThongTinBHYTVO
                    {
                        maThe = lstString[0],
                        hoTen = CommonHelper.DecodeHexString(lstString[1]),
                        ngaySinh = lstString[2],
                        gioiTinh = lstString[3] == "1" ? "Nam" : "Nu",
                        diaChi = CommonHelper.DecodeHexString(lstString[4]),
                        gtTheTu = lstString[6],
                        ngayDu5Nam = lstString[12],

                        maSoBHXH = "404",
                        isConnectSuccessfully = false,
                    };
                    return Ok(resultNotConnect1);
                }
                return Ok(result);
            }

            return Ok(null);
        }


        #region Danh Sách Bảo Hiểm Y tế

        #region Danh Sách Bảo Hiểm Y Tế đang chờ gởi

        [HttpPost("GetThongTinBenhNhanCoBHYT")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GuiBaoHiemYTe)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<ThongTinBenhNhanGoiBHYT> GetThongTinBenhNhanCoBHYT(DanhSachYeuCauTiepNhanIds danhSachYeuCauTiepNhanId)
        {
            var model = _goiBaoHiemYTeService.GetThongTinBenhNhanCoBHYT(danhSachYeuCauTiepNhanId);
            return Ok(model);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GuiBaoHiemYTe)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
        ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _goiBaoHiemYTeService.GetDanhSachGoiBaoHiemYteForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GuiBaoHiemYTe)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _goiBaoHiemYTeService.GetDanhSachGoiBaoHiemYteTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #endregion

        #region Đang sửa hàm download giám định

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDanhSachLichSuBHYTForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GuiBaoHiemYTe)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachLichSuBHYTForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _goiBaoHiemYTeService.GetDanhSachLichSuBHYTForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetDanhSachLichSuBHYTTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GuiBaoHiemYTe)]
        public async Task<ActionResult<GridDataSource>> GetDanhSachLichSuBHYTTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _goiBaoHiemYTeService.GetDanhSachLichSuBHYTTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [HttpPost("GuiHoSoGiamDinh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GuiBaoHiemYTe)]
        public async Task<ActionResult> GuiHoSoGiamDinh(ThongTinBenhNhanViewModel thongTinBenhNhanViewModel)
        {
            //if (thongTinBenhNhanViewModel.IsDownLoad == false)
            //{
            //    var takeAPIToken = new RestClient($"http://egw.baohiemxahoi.gov.vn/api/token/take");
            //    var requesttokenAPI = new RestRequest(Method.POST);
            //    requesttokenAPI.AddJsonBody(
            //                        new { username = "01824_BV", password = "35f60479a061e7c58e415f47e5b087af" }); // uses JsonSerializerTokenBHYTModel
            //    IRestResponse responsetoken = takeAPIToken.Execute(requesttokenAPI);
            //    if (responsetoken.IsSuccessful)
            //    {
            //        var contentToken = JsonConvert.DeserializeObject<ThongTinTokenMoiViewModel>(responsetoken.Content);
            //        TokenBHYT.ModifyTokenBHYT(contentToken.APIKey.access_token, contentToken.APIKey.id_token);
            //    }

            //}

            var watchingData = new HamGuiHoSoWatchingViewModel();
            watchingData.DataJson = thongTinBenhNhanViewModel.DataJson;
            watchingData.TimeSend = DateTime.Now;

            if (thongTinBenhNhanViewModel.TienVatTuYTe != null)
            {
                thongTinBenhNhanViewModel.TienVatTuYTe = (double)(Math.Round(Convert.ToDecimal(thongTinBenhNhanViewModel.TienVatTuYTe), 2));
            }
            if (thongTinBenhNhanViewModel.TienThuoc != null)
            {
                thongTinBenhNhanViewModel.TienThuoc = (double)(Math.Round(Convert.ToDecimal(thongTinBenhNhanViewModel.TienThuoc), 2));
            }
            if (thongTinBenhNhanViewModel.TienTongChi != null)
            {
                thongTinBenhNhanViewModel.TienTongChi = (double)(Math.Round(Convert.ToDecimal(thongTinBenhNhanViewModel.TienTongChi), 2));
            }
            if (thongTinBenhNhanViewModel.TienBenhNhanThanhToan != null)
            {
                thongTinBenhNhanViewModel.TienBenhNhanThanhToan = (double)(Math.Round(Convert.ToDecimal(thongTinBenhNhanViewModel.TienBenhNhanThanhToan), 2));
            }
            if (thongTinBenhNhanViewModel.TienBaoHiemThanhToan != null)
            {
                thongTinBenhNhanViewModel.TienBaoHiemThanhToan = (double)(Math.Round(Convert.ToDecimal(thongTinBenhNhanViewModel.TienBaoHiemThanhToan), 2));
            }
            if (thongTinBenhNhanViewModel.TienNguonKhac != null)
            {
                thongTinBenhNhanViewModel.TienNguonKhac = (double)(Math.Round(Convert.ToDecimal(thongTinBenhNhanViewModel.TienNguonKhac), 2));
            }
            if (thongTinBenhNhanViewModel.TienNgoaiDanhSach != null)
            {
                thongTinBenhNhanViewModel.TienNgoaiDanhSach = (double)(Math.Round(Convert.ToDecimal(thongTinBenhNhanViewModel.TienNgoaiDanhSach), 2));
            }
            if (thongTinBenhNhanViewModel.TienBenhNhanCungChiTra != null)
            {
                thongTinBenhNhanViewModel.TienBenhNhanCungChiTra = (double)(Math.Round(Convert.ToDecimal(thongTinBenhNhanViewModel.TienBenhNhanCungChiTra), 2));
            }

            var thang = thongTinBenhNhanViewModel.ThoiGian?.Month > 10 ? thongTinBenhNhanViewModel.ThoiGian?.Month.ToString() : "0" + thongTinBenhNhanViewModel.ThoiGian?.Month;
            var ngay = thongTinBenhNhanViewModel.ThoiGian?.Day > 10 ? thongTinBenhNhanViewModel.ThoiGian?.Day.ToString() : "0" + thongTinBenhNhanViewModel.ThoiGian?.Day;
            //thongTinBenhNhanViewModel.NgaySinh = thongTinBenhNhanViewModel.ThoiGian?.Year.ToString() + thang + ngay;
            var gioNgayra = thongTinBenhNhanViewModel.NgayRaTime?.Hour > 10 ? thongTinBenhNhanViewModel.NgayRaTime?.Hour.ToString() : "0" + thongTinBenhNhanViewModel.NgayRaTime?.Hour;
            var phutNgayra = thongTinBenhNhanViewModel.NgayRaTime?.Minute > 10 ? thongTinBenhNhanViewModel.NgayRaTime?.Minute.ToString() : "0" + thongTinBenhNhanViewModel.NgayRaTime?.Minute;
            var ngayNgayra = thongTinBenhNhanViewModel.NgayRaTime?.Day > 10 ? thongTinBenhNhanViewModel.NgayRaTime?.Day.ToString() : "0" + thongTinBenhNhanViewModel.NgayRaTime?.Day;
            var thangNgayra = thongTinBenhNhanViewModel.NgayRaTime?.Month > 10 ? thongTinBenhNhanViewModel.NgayRaTime?.Month.ToString() : "0" + thongTinBenhNhanViewModel.NgayRaTime?.Month;
            thongTinBenhNhanViewModel.NgayRa = thongTinBenhNhanViewModel.NgayRaTime?.Year.ToString() + thangNgayra + ngayNgayra + gioNgayra + phutNgayra;

            var gioNgayvao = thongTinBenhNhanViewModel.NgayVaoTime?.Hour > 10 ? thongTinBenhNhanViewModel.NgayVaoTime?.Hour.ToString() : "0" + thongTinBenhNhanViewModel.NgayVaoTime?.Hour;
            var phutNgayvao = thongTinBenhNhanViewModel.NgayVaoTime?.Minute > 10 ? thongTinBenhNhanViewModel.NgayVaoTime?.Minute.ToString() : "0" + thongTinBenhNhanViewModel.NgayVaoTime?.Minute;
            var ngayNgayvao = thongTinBenhNhanViewModel.NgayVaoTime?.Day > 10 ? thongTinBenhNhanViewModel.NgayVaoTime?.Day.ToString() : "0" + thongTinBenhNhanViewModel.NgayVaoTime?.Day;
            var thangNgayvao = thongTinBenhNhanViewModel.NgayVaoTime?.Month > 10 ? thongTinBenhNhanViewModel.NgayVaoTime?.Month.ToString() : "0" + thongTinBenhNhanViewModel.NgayVaoTime?.Month;
            thongTinBenhNhanViewModel.NgayVao = thongTinBenhNhanViewModel.NgayVaoTime?.Year.ToString() + thangNgayvao + ngayNgayvao + gioNgayvao + phutNgayvao;

            var result = _mapper.Map<ThongTinBenhNhan>(thongTinBenhNhanViewModel);
            var resultData = thongTinBenhNhanViewModel.IsDownLoad == true ? await _BHYTService.GoiHoSoGiamDinh(result, true) : await _BHYTService.GoiHoSoGiamDinh(result, false);

            if (resultData != null)
            {
                if (resultData.ErrorCheck == false)
                {
                    watchingData.XMLJson = resultData.XMLJson;
                    //    if (thongTinBenhNhanViewModel.IsDownLoad == false)
                    //    {
                    //        var token = TokenBHYT.GetTokenAPI().token;
                    //        var id_token = TokenBHYT.GetTokenAPI().id_token;
                    //        var takeAPI = new RestClient($"https://egw.baohiemxahoi.gov.vn/api/egw/guiHoSoGiamDinh4210?token=" + token + "&id_token=" + id_token + "&username=01824_BV&password=35f60479a061e7c58e415f47e5b087af&loaiHoSo=3&maTinh=01&maCSKCB=01824");
                    //        var requesttoken = new RestRequest(Method.POST);
                    //        requesttoken.AddJsonBody(
                    //                            new { fileHS = resultData.ByteData }); // uses JsonSerializerTokenBHYTModel
                    //        IRestResponse responsetokenData = takeAPI.Execute(requesttoken);
                    //        if (responsetokenData.IsSuccessful)
                    //        {
                    //            watchingData.APIError = responsetokenData.Content;
                    //            watchingData.ErrorAPICheck = false;
                    //            var contentHOSo = JsonConvert.DeserializeObject<ThongTinHoSoMoiViewModel>(responsetokenData.Content);
                    //            watchingData.MaKetQua = contentHOSo.maKetQua;
                    //            if (contentHOSo.maKetQua.Equals("201"))
                    //            {
                    //                watchingData.NoiDungKetQua = "Định dạng XML không đúng";
                    //            }
                    //            else if (contentHOSo.maKetQua.Equals("200"))
                    //            {
                    //                watchingData.NoiDungKetQua = "GỬi thành công";
                    //            }
                    //            else if (contentHOSo.maKetQua.Equals("202"))
                    //            {
                    //                watchingData.NoiDungKetQua = "Nội dung XML không đúng";
                    //            }
                    //            else if (contentHOSo.maKetQua.Equals("204"))
                    //            {
                    //                watchingData.NoiDungKetQua = "File XML không có nội dung";
                    //            }
                    //            else if (contentHOSo.maKetQua.Equals("401"))
                    //            {
                    //                watchingData.NoiDungKetQua = "Lỗi xác thực";
                    //            }
                    //            else if (contentHOSo.maKetQua.Equals("408"))
                    //            {
                    //                watchingData.NoiDungKetQua = "Request Timeout";
                    //            }
                    //            else if (contentHOSo.maKetQua.Equals("500"))
                    //            {
                    //                watchingData.NoiDungKetQua = "Lỗi Sever";
                    //            }
                    //            else if (contentHOSo.maKetQua.Equals("10"))
                    //            {
                    //                watchingData.NoiDungKetQua = "Lỗi lấy thông tin từ sever số thẻ";
                    //            }

                    //        }
                    //        else
                    //        {
                    //            watchingData.APIError = responsetokenData.Content;
                    //            watchingData.ErrorAPICheck = true;
                    //        }
                    //    }

                    //}
                    //else
                    //{
                    //    watchingData.XMLError = resultData.XMLError;
                }

                var resWatchingData = _mapper.Map<HamGuiHoSoWatching>(watchingData);
                _hamGuiHoSoService.Add(resWatchingData);

                //Giả sử gởi thành công cổng bhyt
                var contentHOSo = true;
                if (contentHOSo)
                {
                    //cập nhật yêu cầu tiếp nhận đã gởi bhyt thành công và lưu nội dung đã gởi
                    //if (!string.IsNullOrEmpty(thongTinBenhNhanViewModel.MaLienKet))
                    //{
                    //    watchingData.NoiDungKetQua = "GỬi thành công";
                    //    var yeuCauTiepNhanId = long.Parse(thongTinBenhNhanViewModel.MaLienKet);
                    //    _goiBaoHiemYTeService.CapNhatThongTinBHYT(yeuCauTiepNhanId, resWatchingData);
                    //}

                }
            }
            return Ok(watchingData);
        }

        [HttpPost("DowloadHoSoGiamDinh")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GuiBaoHiemYTe)]
        public async Task<ActionResult> DowloadHoSoGiamDinh(ThongTinBenhNhanViewModels thongTinBenhNhanViewModels)
        {
            var thongTinBenhNhans = new List<ThongTinBenhNhan>();
            var watchingData = new HamGuiHoSoWatchingViewModel();
            foreach (var thongTinBenhNhanViewModel in thongTinBenhNhanViewModels.ThongTinBenhNhanVMs)
            {
                watchingData.DataJson = thongTinBenhNhanViewModel.DataJson;
                watchingData.TimeSend = DateTime.Now;

                var result = _mapper.Map<ThongTinBenhNhan>(thongTinBenhNhanViewModel);
                thongTinBenhNhans.Add(result);
            }


            var resultData = _BHYTService.addValueToXml(thongTinBenhNhans);
            foreach (var item in resultData.TenFileVOs)
            {
                watchingData.TenFileVOs.Add(new TenFileVO { TenFile = item.TenFile, DuLieu = item.DuLieu });
            }

            watchingData.NameFileDown = resultData.NameFileDown;
            watchingData.countFile = resultData.countFile;

            return Ok(watchingData);
        }


        [HttpPost("GetThongTinGoiBHYTVersion/{yeuCauTiepNhanId}")]
        public Task<List<LookupItemVo>> GetThongTin(long yeuCauTiepNhanId)
        {
            var result = _goiBaoHiemYTeService.GetThongTinGoiBHYTVersion(yeuCauTiepNhanId);
            return Task.FromResult(result);
        }

        #endregion

        #region CẬP NHẬT HÀM GỞI GIÁM ĐINH VÀ XUẤT XML VA LOAD THÔNG TIN BHYT

        [HttpPost("GetThongTinHoSoGiamDinhs")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GuiBaoHiemYTe)]
        public ActionResult<ThongTinBenhNhan> GetThongTinHoSoGiamDinhs(DanhSachYeuCauTiepNhanIds danhSachYeuCauTiepNhanId)
        {
            var thongTinBenhNhanCoBHYTs = _goiBaoHiemYTeService.GetThongTinBenhNhanCoBHYT(danhSachYeuCauTiepNhanId);

            return Ok(thongTinBenhNhanCoBHYTs);
        }

        [HttpPost("DownloadHoSoGiamDinhXMLs")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GuiBaoHiemYTe)]
        public ActionResult DowloadHoSoGiamDinhs(DanhSachYeuCauTiepNhanIds danhSachYeuCauTiepNhanId)
        {
            var thongTinBenhNhanCoBHYTs = _goiBaoHiemYTeService.GetThongTinBenhNhanCoBHYT(danhSachYeuCauTiepNhanId);
            if (thongTinBenhNhanCoBHYTs.Any(cc => !string.IsNullOrEmpty(cc.ErrorMessage)))
            {
                var errorData = thongTinBenhNhanCoBHYTs.Where(cc => !string.IsNullOrEmpty(cc.ErrorMessage)).Select(cc => cc.ErrorMessage).FirstOrDefault();
                return Ok(errorData);
            }


            var thongTinBenhNhans = new List<ThongTinBenhNhan>();
            var watchingData = new HamGuiHoSoWatchingViewModel();

            foreach (var thongTinBenhNhanViewModel in thongTinBenhNhanCoBHYTs)
            {
                watchingData.DataJson = thongTinBenhNhanViewModel.DataJson;
                watchingData.TimeSend = DateTime.Now;
                thongTinBenhNhans.Add(thongTinBenhNhanViewModel);
            }
            var resultData = _goiBaoHiemYTeService.addValueToXml(thongTinBenhNhans);

            foreach (var item in resultData.TenFileVOs)
            {
                watchingData.TenFileVOs.Add(new TenFileVO { TenFile = item.TenFile, DuLieu = item.DuLieu });
            }

            watchingData.NameFileDown = resultData.NameFileDown;
            watchingData.countFile = resultData.countFile;

            return Ok(watchingData);

        }

        [HttpPost("GuiHoSoGiamDinhs")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GuiBaoHiemYTe)]
        public async Task<ActionResult> GuiHoSoGiamDinhs(DanhSachYeuCauTiepNhanIds danhSachYeuCauTiepNhanId)
        {
            //Kiểm tra mã hóa 
            var watchingData = new HamGuiHoSoWatchingViewModel();
            var thongTinBenhNhans = new List<ThongTinBenhNhan>();
            var thongTinBenhNhanCoBHYTs = _goiBaoHiemYTeService.GetThongTinBenhNhanCoBHYT(danhSachYeuCauTiepNhanId);
            if (thongTinBenhNhanCoBHYTs.Any(cc => !string.IsNullOrEmpty(cc.ErrorMessage)))
            {
                var errorData = thongTinBenhNhanCoBHYTs.Where(cc => !string.IsNullOrEmpty(cc.ErrorMessage)).Select(cc => cc.ErrorMessage).FirstOrDefault();
                return Ok(errorData);
            }

            if (thongTinBenhNhanCoBHYTs.Any())
            {
                foreach (var thongTinBenhNhanCoBHYT in thongTinBenhNhanCoBHYTs)
                {
                    var dataJson = JsonConvert.SerializeObject(thongTinBenhNhanCoBHYT);
                    foreach (var thongTinBenhNhanViewModel in thongTinBenhNhanCoBHYTs)
                    {
                        watchingData.DataJson = dataJson;
                        watchingData.TimeSend = DateTime.Now;
                    }
                }

                // Bước 1: Mã hóa file xml byte gởi qua bảo hiểm y tế mỗi XML convert stringBase64
                //var resultData = await _goiBaoHiemYTeService.GoiHoSoGiamDinhs(thongTinBenhNhanCoBHYTs);
                //if(resultData.ErrorCheck != true)
                // {

                //Bước 2: Sau Khi Gởi Thành Công Lưu Kết Quả Trong Hệ Thống
                //_goiBaoHiemYTeService.LuuThongTinDaGoiVaoHeThong(thongTinBenhNhanCoBHYTs);
                // }
                // else
                // {
                //watchingData.XMLError = resultData.XMLError;
                //watchingData.APIError = resultData.APIError;
                //watchingData.ErrorCheck = resultData.ErrorCheck;
                //watchingData.ErrorAPICheck = resultData.ErrorCheck;
                //}
            }

            return Ok(watchingData);
        }

        [HttpPost("DownloadHoSoGiamDinhXML")]
        public ActionResult DownloadHoSoGiamDinhXML(string NameFileDown)
        {
            var byteFileTongHop = _BHYTService.pathFileTongHop(NameFileDown);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=tong_hop.xml");
            this.Response.ContentType = "application/xml";
            return new FileContentResult(byteFileTongHop, "application/xml");
        }

        [HttpPost("GetThongTinChiTietBaoHiemYTe")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GuiBaoHiemYTe)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<ThongTinBenhNhan> GetThongTinChiTietBaoHiemYTe(GoiDanhSachThongTinBenhNhanCoBHYT danhSach)
        {
            var model = _goiBaoHiemYTeService.GetThongTinChiTietBaoHiemYTe(danhSach);
            return Ok(model);
        }

        [HttpPost("HoSoGiamDinhXML")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GuiBaoHiemYTe)]
        public async Task<ActionResult> HoSoGiamDinhXML(ThongTinBenhNhan thongTinBenhNhanViewModel)
        {
            var thongTinBenhNhans = new List<ThongTinBenhNhan>();
            var watchingData = new HamGuiHoSoWatchingViewModel();

            watchingData.DataJson = thongTinBenhNhanViewModel.DataJson;
            watchingData.TimeSend = DateTime.Now;
            thongTinBenhNhans.Add(thongTinBenhNhanViewModel);

            foreach (var item in thongTinBenhNhans)
            {
                var error = KiemTraThongTinBenhNhan(item);
                if (!string.IsNullOrEmpty(error))
                    return Ok(KiemTraThongTinBenhNhan(item));
            }
            _goiBaoHiemYTeService.LuuLichSuDuLieuGuiCongBHYT(thongTinBenhNhans, false);
            var resultData = _goiBaoHiemYTeService.addValueToXml(thongTinBenhNhans);

            foreach (var item in resultData.TenFileVOs)
            {
                watchingData.TenFileVOs.Add(new TenFileVO { TenFile = item.TenFile, DuLieu = item.DuLieu });
            }

            watchingData.NameFileDown = resultData.NameFileDown;
            watchingData.countFile = resultData.countFile;

            return Ok(watchingData);
        }

        [HttpPost("KiemTraYeuCauTiepNhanGoiBHYT/{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.GuiBaoHiemYTe)]
        public ActionResult<string> KiemTraYeuCauTiepNhanGoiBHYT(long id)
        {
            var resultData = _goiBaoHiemYTeService.KiemTraYeuCauTiepNhanGoiBHYT(id);
            return Ok(resultData);
        }

        private string KiemTraThongTinBenhNhan(ThongTinBenhNhan thongTinBenhNhan)
        {
            if (thongTinBenhNhan == null)
                return "Yêu cầu tiếp nhận không tồn tại";
            if (string.IsNullOrEmpty(thongTinBenhNhan.MaLienKet))
                return "Vui lòng kiểm tra mã liên kết";
            if (string.IsNullOrEmpty(thongTinBenhNhan.MaBenhNhan))
                return "Vui lòng kiểm tra mã người bệnh";
            if (string.IsNullOrEmpty(thongTinBenhNhan.HoTen))
                return "Vui lòng kiểm tra họ tên người bệnh";
            if (thongTinBenhNhan.NgaySinh == null)
                return "Vui lòng kiểm tra ngày sinh người bệnh";
            if (thongTinBenhNhan.GioiTinh == null)
                return "Vui lòng kiểm tra giới tính";
            if (string.IsNullOrEmpty(thongTinBenhNhan.DiaChi))
                return "Vui lòng kiểm tra địa chỉ người bệnh";
            if (string.IsNullOrEmpty(thongTinBenhNhan.MaThe))
                return "Vui lòng kiểm tra thẻ người bệnh";
            if (string.IsNullOrEmpty(thongTinBenhNhan.MaCoSoKCBBanDau))
                return "Vui lòng kiểm tra mã cơ sở kcb ban đầu";
            if (thongTinBenhNhan.GiaTriTheTu == null)
                return "Vui lòng kiểm tra giá trị thẻ từ";
            if (thongTinBenhNhan.GiaTriTheDen == null)
                return "Vui lòng kiểm tra giá trị thẻ đến";
            //if (thongTinBenhNhan.MienCungChiTra == null)
            //    return "Vui lòng kiểm tra miễm cùng chi trả";
            if (string.IsNullOrEmpty(thongTinBenhNhan.TenBenh))
                return "Vui lòng kiểm tra tên người bệnh";
            if (thongTinBenhNhan.MaBenh == null)
                return "Vui lòng kiểm tra mã bệnh";
            if (thongTinBenhNhan.LyDoVaoVien == null)
                return "Vui lòng kiểm tra lý do vào viện";
            if (thongTinBenhNhan.NgayVao == null)
                return "Vui lòng kiểm tra ngày vào";
            if (thongTinBenhNhan.NgayRa == null)
                return "Vui lòng kiểm tra ngày ra";
            if (thongTinBenhNhan.TienBaoHiemThanhToan == null)
                return "Vui lòng kiểm tra tiền bảo hiểm thanh toán";
            if (thongTinBenhNhan.TienTongChi == null)
                return "Vui lòng kiểm tra tiền tổng chi";
            if (thongTinBenhNhan.NamQuyetToan == null)
                return "Vui lòng kiểm tra năm quyết toán";
            if (thongTinBenhNhan.ThangQuyetToan == null)
                return "Vui lòng kiểm tra tháng quyết toán";
            if (thongTinBenhNhan.MaLoaiKCB == null)
                return "Vui lòng kiểm tra mã loại khám chữa bệnh";
            if (string.IsNullOrEmpty(thongTinBenhNhan.MaKhoa))
                return "Vui lòng kiểm tra mã khoa";
            if (string.IsNullOrEmpty(thongTinBenhNhan.MaCSKCB))
                return "Vui lòng kiểm tra mã CSKCB";


            //Kiểm tra thông tin thuốc
            foreach (var hoSoChiTietThuoc in thongTinBenhNhan.HoSoChiTietThuoc)
            {
                if (string.IsNullOrEmpty(hoSoChiTietThuoc.MaThuoc))
                    return "Vui lòng kiểm tra mã thuốc";
                if (hoSoChiTietThuoc.MaNhom == null)
                    return "Vui lòng kiểm tra mã nhóm";
                if (string.IsNullOrEmpty(hoSoChiTietThuoc.TenThuoc))
                    return "Vui lòng kiểm tra tên thuốc";
                if (string.IsNullOrEmpty(hoSoChiTietThuoc.DonViTinh))
                    return "Vui lòng kiểm tra đơn vi tính";
                if (hoSoChiTietThuoc.SoLuong == null)
                    return "Vui lòng kiểm tra số lượng";
                if (hoSoChiTietThuoc.DonGia == null)
                    return "Vui lòng kiểm tra đơn giá";
                if (hoSoChiTietThuoc.TyLeThanhToan == null)
                    return "Vui lòng kiểm tra tỷ lệ thanh toán";
                if (hoSoChiTietThuoc.ThanhTien == null)
                    return "Vui lòng kiểm tra thành tiền";
                if (string.IsNullOrEmpty(hoSoChiTietThuoc.MaKhoa))
                    return "Vui lòng kiểm tra mã khoa";
                if (string.IsNullOrEmpty(hoSoChiTietThuoc.MaBenh))
                    return "Vui lòng kiểm tra mã bệnh";
                if (hoSoChiTietThuoc.NgayYLenh == null)
                    return "Vui lòng kiểm tra ngày y lệnh";
                if (hoSoChiTietThuoc.PhamVi == null)
                    return "Vui lòng kiểm tra phạm vi";
                if (hoSoChiTietThuoc.MucHuong == null)
                    return "Vui lòng kiểm tra mức hưởng";
                if (hoSoChiTietThuoc.TienBaoHiemThanhToan == null)
                    return "Vui lòng kiểm tra tiền bảo hiểm thanh toán";
                if (hoSoChiTietThuoc.MaPhuongThucThanhToan == null)
                    return "Vui lòng kiểm tra mã phương thức thanh toán";
            }

            foreach (var hoSoChiTietDVKT in thongTinBenhNhan.HoSoChiTietDVKT)
            {
                if (hoSoChiTietDVKT.MaNhom == null)
                    return "Vui lòng kiểm tra mã nhóm dịch vụ kỹ thuật";
                if (hoSoChiTietDVKT.PhamVi == null)
                    return "Vui lòng kiểm tra phạm vi dịch vụ kỹ thuật";
                if (hoSoChiTietDVKT.SoLuong == null)
                    return "Vui lòng kiểm tra số lượng dịch vụ kỹ thuật";
                if (hoSoChiTietDVKT.DonGia == null)
                    return "Vui lòng kiểm tra đơn giá dịch vụ kỹ thuật";
                if (hoSoChiTietDVKT.MucHuong == null)
                    return "Vui lòng kiểm tra mức hưởng dịch vụ kỹ thuật";
                if (hoSoChiTietDVKT.TienBaoHiemThanhToan == null)
                    return "Vui lòng kiểm tra tiền bảo hiểm thanh toán dịch vụ kỹ thuật";
                if (string.IsNullOrEmpty(hoSoChiTietDVKT.MaKhoa))
                    return "Vui lòng kiểm tra mã khoa dịch vụ kỹ thuật";
                //if (string.IsNullOrEmpty(hoSoChiTietDVKT.MaBacSi))
                //    return "Vui lòng kiểm tra mã bác sỹ dịch vụ kỹ thuật";
                if (string.IsNullOrEmpty(hoSoChiTietDVKT.MaBenh))
                    return "Vui lòng kiểm tra mã bệnh dịch vụ kỹ thuật";
                if (hoSoChiTietDVKT.NgayYLenh == null)
                    return "Vui lòng kiểm tra ngày y lệnh dịch vụ kỹ thuật";
                if (hoSoChiTietDVKT.MaPhuongThucThanhToan == null)
                    return "Vui lòng kiểm tra mã phương thức thanh toán dịch vụ kỹ thuật";
            }

            return null;
        }

        #endregion

        #region Xuất Excel Danh Sách Bảo Hiểm Y Tế

        [HttpPost("ExportBaoHiemYTe")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.GuiBaoHiemYTe)]
        public async Task<ActionResult> ExportBaoHiemYTe([FromBody]QueryInfo queryInfo)
        {

            var gridData = await _goiBaoHiemYTeService.GetDanhSachGoiBaoHiemYteForGridAsync(queryInfo, true);
            var danhSachBaoHiemYTe = gridData.Data.Select(p => (GoiBaoHiemYTeVo)p).ToList();
            var dataExcel = danhSachBaoHiemYTe.Map<List<GoiBaoHiemYTeExportExcel>>();

            var lstValueObject = new List<(string, string)>();

            lstValueObject.Add((nameof(GoiBaoHiemYTeExportExcel.MaTN), "Mã TN"));
            lstValueObject.Add((nameof(GoiBaoHiemYTeExportExcel.MaBN), "Mã BN"));
            lstValueObject.Add((nameof(GoiBaoHiemYTeExportExcel.HoTen), "Tên Người Bệnh"));
            lstValueObject.Add((nameof(GoiBaoHiemYTeExportExcel.NamSinh), "Năm Sinh"));
            lstValueObject.Add((nameof(GoiBaoHiemYTeExportExcel.GioiTinh), "Giới Tính"));
            lstValueObject.Add((nameof(GoiBaoHiemYTeExportExcel.DiaChi), "Địa Chỉ"));
            lstValueObject.Add((nameof(GoiBaoHiemYTeExportExcel.ThoiGianTiepNhanStr), "Tiếp Nhận Lúc"));
            lstValueObject.Add((nameof(GoiBaoHiemYTeExportExcel.MucHuong), "Mức hưởng"));

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Gởi BHYT trong ngày", 2, "Gởi BHYT trong ngày");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=GoiBaoHiemYTe" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("ExportLichSuBaoHiemYTe")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.LichSuGuiBHYT)]
        public async Task<ActionResult> ExportLichSuBaoHiemYTe([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _goiBaoHiemYTeService.GetDanhSachLichSuBHYTForGridAsync(queryInfo, true);
            var dslsBHYT = gridData.Data.Select(p => (GoiBaoHiemYTeVo)p).ToList();
            var dataExcel = dslsBHYT.Map<List<LichSuGoiBaoHiemYTeExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(GoiBaoHiemYTeExportExcel.MaTN), "Mã TN"));
            lstValueObject.Add((nameof(GoiBaoHiemYTeExportExcel.MaBN), "Mã BN"));
            lstValueObject.Add((nameof(GoiBaoHiemYTeExportExcel.HoTen), "Tên Người Bệnh"));
            lstValueObject.Add((nameof(GoiBaoHiemYTeExportExcel.NamSinh), "Năm Sinh"));
            lstValueObject.Add((nameof(GoiBaoHiemYTeExportExcel.GioiTinh), "Giới Tính"));
            lstValueObject.Add((nameof(GoiBaoHiemYTeExportExcel.DiaChi), "Địa Chỉ"));
            lstValueObject.Add((nameof(GoiBaoHiemYTeExportExcel.ThoiGianTiepNhanStr), "Tiếp Nhận Lúc"));
            lstValueObject.Add((nameof(GoiBaoHiemYTeExportExcel.MucHuong), "Mức hưởng"));

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Lịch sử Hồ sơ giám định BHYT", 2, "Lịch sử Hồ sơ giám định BHYT");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=LichSuGiamDinhBHYT" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        #endregion

        #endregion
    }
}
