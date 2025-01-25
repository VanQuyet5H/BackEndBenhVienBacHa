UPDATE TemplateKhamSangLocTiemChung 
SET ComponentDynamics = N'{
  "NhomKhamSangLoc": 3,
  "ComponentDynamics": [
    {
      "Type": 4,
      "Id": "DaTiemMui1Group",
      "Label": "Đã tiêm vắc xin phòng COVID-19:",
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
          "Id": "DaTiemMui1GroupCo1",
          "Label": "Đã tiêm mũi 1",
          "Value": null,
          "fxFlex": "100%",
          "fxFlexSm": "100%"
        },
        {
          "Type": 3,
          "Id": "DaTiemMui1GroupCo1Text",
          "Label": "Loại vắc xin",
          "Value": null,
          "fxFlex": "50%",
          "fxFlexSm": "50%",
          "maxlength": 1000,
          "requiredCheckbox": true
        },
        {
          "Type": 7,
          "Id": "DaTiemMui1GroupCo1Date",
          "Label": "Ngày tiêm",
          "Value": null,
          "fxFlex": "50%",
          "fxFlexSm": "50%",
          "requiredCheckbox": true
        },
        {
          "Type": 5,
          "Id": "DaTiemMui1GroupCo2",
          "Label": "Đã tiêm mũi 2",
          "Value": null,
          "fxFlex": "100%",
          "fxFlexSm": "100%"
        },
        {
          "Type": 3,
          "Id": "DaTiemMui1GroupCo2Text",
          "Label": "Loại vắc xin",
          "Value": null,
          "fxFlex": "50%",
          "fxFlexSm": "50%",
          "maxlength": 1000,
          "requiredCheckbox": true
        },
        {
          "Type": 7,
          "Id": "DaTiemMui1GroupCo2Date",
          "Label": "Ngày tiêm",
          "Value": null,
          "fxFlex": "50%",
          "fxFlexSm": "50%",
          "requiredCheckbox": true
        },
        {
          "Type": 5,
          "Id": "DaTiemMui1GroupCo3",
          "Label": "Đã tiêm mũi 3",
          "Value": null,
          "fxFlex": "100%",
          "fxFlexSm": "100%"
        },
        {
          "Type": 3,
          "Id": "DaTiemMui1GroupCo3Text",
          "Label": "Loại vắc xin",
          "Value": null,
          "fxFlex": "50%",
          "fxFlexSm": "50%",
          "maxlength": 1000,
          "requiredCheckbox": true
        },
        {
          "Type": 7,
          "Id": "DaTiemMui1GroupCo3Date",
          "Label": "Ngày tiêm",
          "Value": null,
          "fxFlex": "50%",
          "fxFlexSm": "50%",
          "requiredCheckbox": true
        }
      ]
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
      "Label": "2. Đang mắc bệnh cấp tính",
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
      "Type": 1,
      "Id": "Group3",
      "Label": "3. Phụ nữ mang thai",
      "fxFlex": "100%",
      "fxFlexSm": "100%"
    },
    {
      "Type": 1,
      "Id": "Group3Space",
      "Label": "",
      "Value": null,
      "fxFlex": "5%",
      "fxFlexSm": "5%"
    },
    {
      "Type": 4,
      "Id": "Group313Tuan",
      "Label": "3a. Phụ nữ mang thai < 13 tuần",
      "fxFlex": "75%",
      "fxFlexSm": "55%",
      "groupItems": [
        {
          "Type": 5,
          "Id": "Group313TuanKhong",
          "Label": "Không",
          "Value": true,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        },
        {
          "Type": 5,
          "Id": "Group313TuanCo",
          "Label": "Có",
          "Value": null,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        }
      ]
    },
    {
      "Type": 1,
      "Id": "Group3Space",
      "Label": "",
      "Value": null,
      "fxFlex": "5%",
      "fxFlexSm": "5%"
    },
    {
      "Type": 4,
      "Id": "Group3Hon13Tuan",
      "Label": "3b. Phụ nữ mang thai &ge; 13 tuần",
      "fxFlex": "75%",
      "fxFlexSm": "55%",
      "groupItems": [
        {
          "Type": 5,
          "Id": "Group3Hon13TuanKhong",
          "Label": "Không",
          "Value": true,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        },
        {
          "Type": 5,
          "Id": "Group3Hon13TuanCo",
          "Label": "Có",
          "Value": null,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        }
      ]
    },
    {
      "Type": 4,
      "Id": "Group4",
      "Label": "4. Phản vệ độ 3 trở lên với bất kỳ dị nguyên nào",
      "fxFlex": "80%",
      "fxFlexSm": "60%",
      "groupItems": [
        {
          "Type": 5,
          "Id": "Group4Khong",
          "Label": "Không",
          "Value": true,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        },
        {
          "Type": 5,
          "Id": "Group4Co",
          "Label": "Có",
          "Value": null,
          "fxFlex": "10%",
          "fxFlexSm": "20%"
        },
        {
          "Type": 3,
          "Id": "Group4Text",
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
      "Id": "Group5",
      "Label": "5. Đang bị suy giảm miễn dịch nặng, ung thư giai đoạn cuối đang điều trị hóa trị, xạ trị",
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
        }
      ]
    },
    {
      "Type": 4,
      "Id": "Group6",
      "Label": "6. Tiền sử dị ứng với bất kỳ dị nguyên nào",
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
      "Label": "7. Tiền sử rối loạn đông máu/cầm máu",
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
      "Label": "8. Rối loạn tri giác, rối loạn hành vi",
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
      "Label": "9. Bất thường dấu hiệu sống, nếu có ghi rõ:",
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
        },
        {
          "Type": 3,
          "Id": "Group9Text",
          "Label": "",
          "Value": null,
          "fxFlex": "100%",
          "fxFlexSm": "100%",
          "maxlength": 1000,
          "requiredCheckbox": true
        },
        {
          "Type": 6,
          "Id": "Group9NhietDo",
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
          "Id": "Group9Mach",
          "Label": "Mạch",
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
          "Id": "Group9HuyetAp",
          "Label": "Huyết áp",
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
          "Id": "Group9NhipTho",
          "Label": "Nhịp thở",
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
    },
    {
      "Type": 4,
      "Id": "Group10",
      "Label": "10. Các chống chỉ định/trì hoãn khác, nếu có ghi rõ:",
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
        }
      ]
    }
  ]
}'
WHere Ten = N'KhamSangLocDoiTuongCovid'

GO
Update dbo.CauHinh
Set [Value] = '3.5.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'