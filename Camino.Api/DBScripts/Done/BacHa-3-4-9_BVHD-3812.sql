UPDATE TemplateKhamSangLocTiemChung
SET ComponentDynamics = Replace(ComponentDynamics, N'"IsTrongBenhVien": true,',  N'"NhomKhamSangLoc": 2,')
Go
UPDATE TemplateKhamSangLocTiemChung
SET ComponentDynamics = Replace(ComponentDynamics, N'"IsTrongBenhVien": false,',  N'"NhomKhamSangLoc": 1,')
Go
UPDATE YeuCauDichVuKyThuatKhamSangLocTiemChung
SET ThongTinKhamSangLocTiemChungTemplate = Replace(ThongTinKhamSangLocTiemChungTemplate, N'"IsTrongBenhVien": true,',  N'"NhomKhamSangLoc": 2,')
GO
UPDATE YeuCauDichVuKyThuatKhamSangLocTiemChung
SET ThongTinKhamSangLocTiemChungTemplate = Replace(ThongTinKhamSangLocTiemChungTemplate, N'"IsTrongBenhVien": false,',  N'"NhomKhamSangLoc": 1,')
GO
INSERT INTO TemplateKhamSangLocTiemChung(Ten, TieuDe, ComponentDynamics, CreatedById, LastUserId, LastTime, CreatedOn)
VALUES(N'KhamSangLocDoiTuongCovid', N'Khám sàng lọc đối tượng tiêm vắc xin covid', N'{
  "NhomKhamSangLoc": 3,
  "ComponentDynamics": [
    {
      "Type": 4,
      "Id": "DaTiemMui1Group",
      "Label": "Đã tiêm mũi 1 vắc xin phòng COVID-19:",
      "fxFlex": "100%",
      "fxFlexSm": "100%",
      "groupItems": [
        {
          "Type": 5,
          "Id": "DaTiemMui1GroupKhong",
          "Label": "Chưa tiêm",
          "Value": true,
          "fxFlex": "100%",
          "fxFlexSm": "100%"
        },
        {
          "Type": 5,
          "Id": "DaTiemMui1GroupCo",
          "Label": "Đã tiêm",
          "Value": null,
          "fxFlex": "100%",
          "fxFlexSm": "100%"
        },
        {
          "Type": 3,
          "Id": "DaTiemMui1GroupText",
          "Label": "Loại vắc xin",
          "Value": null,
          "fxFlex": "100%",
          "fxFlexSm": "100%",
          "maxlength": 1000,
          "requiredCheckbox": true
        }
      ]
    },
    {
      "Type": 1,
      "Id": "SangLoc",
      "Label": "Sàng lọc",
      "Value": null,
      "fxFlex": "75px",
      "fxFlexSm": "75px",
      "IsBoldText": true
    },
    {
      "Type": 1,
      "Id": "SangLoc",
      "Label": "(Người đi tiêm điền sẵn mục 1 đến 9; Bác sỹ khám sàng lọc sẽ kiểm tra lại)",
      "Value": null,
      "fxFlex": "auto",
      "fxFlexSm": "auto",
      "IsLightText": true
    },
    {
      "Type": 10,
      "fxFlex": "70%",
      "fxFlexSm": "70%",
      "Id": "Blank",
      "Value": null
    },
    {
      "Type": 4,
      "Id": "Group1",
      "Label": "1. Tiền sử rõ ràng phản vệ với vắc xin phòng COVID-19 lần trước hoặc các thành phần của vắc xin phòng COVID-19.",
      "fxFlex": "80%",
      "fxFlexSm": "60%",
      "groupItems": [
        {
          "Type": 5,
          "Id": "Group1Khong",
          "Label": "Không",
          "Value": true,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        },
        {
          "Type": 5,
          "Id": "Group1Co",
          "Label": "Có",
          "Value": null,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        }
      ]
    },
    {
      "Type": 4,
      "Id": "Group2",
      "Label": "2. Tiền sử rõ ràng bị COVID-19 trong vòng 6 tháng",
      "fxFlex": "80%",
      "fxFlexSm": "60%",
      "groupItems": [
        {
          "Type": 5,
          "Id": "Group2Khong",
          "Label": "Không",
          "Value": true,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        },
        {
          "Type": 5,
          "Id": "Group2Co",
          "Label": "Có",
          "Value": null,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        }
      ]
    },
    {
      "Type": 4,
      "Id": "Group3",
      "Label": "3. Đang mắc bệnh cấp tính",
      "fxFlex": "80%",
      "fxFlexSm": "60%",
      "groupItems": [
        {
          "Type": 5,
          "Id": "Group3Khong",
          "Label": "Không",
          "Value": true,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        },
        {
          "Type": 5,
          "Id": "Group3Co",
          "Label": "Có",
          "Value": null,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        }
      ]
    },
    {
      "Type": 1,
      "Id": "Group4",
      "Label": "4. Phụ nữ mang thai *",
      "fxFlex": "100%",
      "fxFlexSm": "100%"
    },
    {
      "Type": 1,
      "Id": "Group4Space",
      "Label": "",
      "Value": null,
      "fxFlex": "5%",
      "fxFlexSm": "5%"
    },
    {
      "Type": 4,
      "Id": "Group413Tuan",
      "Label": "4a. Phụ nữ mang thai < 13 tuần",
      "fxFlex": "75%",
      "fxFlexSm": "55%",
      "groupItems": [
        {
          "Type": 5,
          "Id": "Group413TuanKhong",
          "Label": "Không",
          "Value": true,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        },
        {
          "Type": 5,
          "Id": "Group413TuanCo",
          "Label": "Có",
          "Value": null,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        }
      ]
    },
    {
      "Type": 1,
      "Id": "Group4Space",
      "Label": "",
      "Value": null,
      "fxFlex": "5%",
      "fxFlexSm": "5%"
    },
    {
      "Type": 4,
      "Id": "Group4Hon13Tuan",
      "Label": "4b. Phụ nữ mang thai &ge; 13 tuần",
      "fxFlex": "75%",
      "fxFlexSm": "55%",
      "groupItems": [
        {
          "Type": 5,
          "Id": "Group4Hon13TuanKhong",
          "Label": "Không",
          "Value": true,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        },
        {
          "Type": 5,
          "Id": "Group4Hon13TuanCo",
          "Label": "Có",
          "Value": null,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        }
      ]
    },
    {
      "Type": 4,
      "Id": "Group5",
      "Label": "5. Phản vệ độ 3 trở lên với bất kỳ dị nguyên nào",
      "fxFlex": "80%",
      "fxFlexSm": "60%",
      "groupItems": [
        {
          "Type": 5,
          "Id": "Group5Khong",
          "Label": "Không",
          "Value": true,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        },
        {
          "Type": 5,
          "Id": "Group5Co",
          "Label": "Có",
          "Value": null,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        },
        {
          "Type": 3,
          "Id": "Group5Text",
          "Label": "Loại tác nhân dị ứng",
          "Value": null,
          "fxFlex": "100%",
          "fxFlexSm": "100%",
          "maxlength": 1000,
          "requiredCheckbox": true
        }
      ]
    },
    {
      "Type": 4,
      "Id": "Group6",
      "Label": "6. Đang bị suy giảm miễn dịch nặng, ung thư giai đoạn cuối đang điều trị hóa trị, xạ trị",
      "fxFlex": "80%",
      "fxFlexSm": "60%",
      "groupItems": [
        {
          "Type": 5,
          "Id": "Group6Khong",
          "Label": "Không",
          "Value": true,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        },
        {
          "Type": 5,
          "Id": "Group6Co",
          "Label": "Có",
          "Value": null,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        }
      ]
    },
    {
      "Type": 4,
      "Id": "Group7",
      "Label": "7. Tiền sử dị ứng với bất kỳ dị nguyên nào",
      "fxFlex": "80%",
      "fxFlexSm": "60%",
      "groupItems": [
        {
          "Type": 5,
          "Id": "Group7Khong",
          "Label": "Không",
          "Value": true,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        },
        {
          "Type": 5,
          "Id": "Group7Co",
          "Label": "Có",
          "Value": null,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        }
      ]
    },
    {
      "Type": 4,
      "Id": "Group8",
      "Label": "8. Tiền sử rối loạn đông máu/cầm máu",
      "fxFlex": "80%",
      "fxFlexSm": "60%",
      "groupItems": [
        {
          "Type": 5,
          "Id": "Group8Khong",
          "Label": "Không",
          "Value": true,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        },
        {
          "Type": 5,
          "Id": "Group8Co",
          "Label": "Có",
          "Value": null,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        }
      ]
    },
    {
      "Type": 4,
      "Id": "Group9",
      "Label": "9. Rối loạn tri giác, rối loạn hành vi",
      "fxFlex": "80%",
      "fxFlexSm": "60%",
      "groupItems": [
        {
          "Type": 5,
          "Id": "Group9Khong",
          "Label": "Không",
          "Value": true,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        },
        {
          "Type": 5,
          "Id": "Group9Co",
          "Label": "Có",
          "Value": null,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        }
      ]
    },
    {
      "Type": 4,
      "Id": "Group10",
      "Label": "10. Bất thường dấu hiệu sống, nếu có ghi rõ:",
      "fxFlex": "80%",
      "fxFlexSm": "60%",
      "groupItems": [
        {
          "Type": 5,
          "Id": "Group10Khong",
          "Label": "Không",
          "Value": true,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        },
        {
          "Type": 5,
          "Id": "Group10Co",
          "Label": "Có",
          "Value": null,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        },
        {
          "Type": 3,
          "Id": "Group10Text",
          "Label": "",
          "Value": null,
          "fxFlex": "100%",
          "fxFlexSm": "100%",
          "maxlength": 1000,
          "requiredCheckbox": true
        },
        {
          "Type": 6,
          "Id": "Group10NhietDo",
          "Label": "Nhiệt độ",
          "SubLabel": "độ C",
          "Value": null,
          "fxFlex": "40%",
          "fxFlexSm": "40%",
          "fxFlexSub": "10%",
          "fxFlexSmSub": "10%",
          "min": 1,
          "max": 9999999,
          "requiredCheckbox": true
        },
        {
          "Type": 6,
          "Id": "Group10Mach",
          "Label": "Mạch ***",
          "SubLabel": "lần/phút",
          "Value": null,
          "fxFlex": "40%",
          "fxFlexSm": "40%",
          "fxFlexSub": "10%",
          "fxFlexSmSub": "10%",
          "min": 1,
          "max": 9999999,
          "requiredCheckbox": true
        },
        {
          "Type": 6,
          "Id": "Group10HuyetAp",
          "Label": "Huyết áp **",
          "SubLabel": "mmHg",
          "Value": null,
          "fxFlex": "40%",
          "fxFlexSm": "40%",
          "fxFlexSub": "10%",
          "fxFlexSmSub": "10%",
          "min": 1,
          "max": 9999999,
          "requiredCheckbox": true
        },
        {
          "Type": 6,
          "Id": "Group10NhipTho",
          "Label": "Nhịp thở ***",
          "SubLabel": "lần/phút",
          "Value": null,
          "fxFlex": "40%",
          "fxFlexSm": "40%",
          "fxFlexSub": "10%",
          "fxFlexSmSub": "10%",
          "min": 1,
          "max": 9999999,
          "requiredCheckbox": true
        }
      ]
    }
  ]
}', 1, 1, GETDATE(), GETDATE())
GO

Update dbo.CauHinh
Set [Value] = '3.4.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'