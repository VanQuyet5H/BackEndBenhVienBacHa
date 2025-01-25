ALTER TABLE YeuCauDichVuKyThuatTiemChung
ADD LieuLuong NVARCHAR(500) NULL
GO

UPDATE TemplateKhamSangLocTiemChung
SET ComponentDynamics = N'{
    "IsTrongBenhVien": true,
    "ComponentDynamics": [
        {
            "Type": 6,
            "Id": "CanNang",
            "Label": "Cân nặng",
            "SubLabel": "g",
            "Value": null,
            "fxFlex": "10%",
            "fxFlexSm": "10%",
            "required": true,
            "min": 1,
            "max": 9999999
        },
        {
            "Type": 6,
            "Id": "ThanNhiet",
            "Label": "Thân nhiệt",
            "SubLabel": "&deg;C",
            "Value": null,
            "fxFlex": "10%",
            "fxFlexSm": "10%",
            "required": true,
            "min": 1,
            "max": 9999
        },
        {
            "Type": 6,
            "Id": "TuoiThaiKhiSinh",
            "Label": "Tuổi thai",
            "SubLabel": "tuần",
            "Value": null,
            "fxFlex": "10%",
            "fxFlexSm": "10%",
            "required": true,
            "min": 1,
            "max": 100
        },
        {
            "Type": 10,
            "fxFlex": "60%",
            "fxFlexSm": "60%",
            "Id": "Blank",
            "Value": null
        },
        {
            "Type": 4,
            "Id": "GroupXetNghiemHbsAg",
            "Label": "Mẹ đã xét nghiệm HbsAg: ",
            "fxFlex": "15%",
            "fxFlexSm": "20%",
            "groupItems": [
                {
                    "Type": 5,
                    "Id": "GroupXetNghiemHbsAgKhong",
                    "Label": "Không",
                    "Value": true,
                    "fxFlex": "10%",
                    "fxFlexSm": "10%"
                },
                {
                    "Type": 5,
                    "Id": "GroupXetNghiemHbsAgCo",
                    "Label": "Có",
                    "Value": null,
                    "fxFlex": "10%",
                    "fxFlexSm": "10%"
                },
                {
                    "Type": 4,
                    "Id": "GroupKetQuaHbsAg",
                    "Label": "Kết quả",
                    "fxFlex": "5%",
                    "fxFlexSm": "20%",
                    "requiredCheckbox": true
                },
                {
                    "Type": 5,
                    "Id": "GroupKetQuaHbsAgCoChild",
                    "Label": "Dương tính",
                    "Value": null,
                    "fxFlex": "10%",
                    "fxFlexSm": "15%",
                    "requiredCheckbox": true,
                    "isNullable": true
                },
                {
                    "Type": 5,
                    "Id": "GroupKetQuaHbsAgKhongChild",
                    "Label": "Âm tính",
                    "Value": null,
                    "fxFlex": "10%",
                    "fxFlexSm": "15%",
                    "requiredCheckbox": true,
                    "isNullable": true
                }
            ]
        },
        {
            "Type": 10,
            "fxFlex": "30%",
            "fxFlexSm": "0",
            "Id": "Blank",
            "Value": null
        },
        {
            "Type": 4,
            "Id": "Group1",
            "Label": "1. Tình trạng sức khoẻ chưa ổn định",
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
            "Label": "2. Sốt/Hạ thân nhiệt (Sốt nhiệt độ &ge; 37,5&deg;C; Hạ thân nhiệt: nhiệt độ &le; 35,5&deg;C)",
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
            "Label": "3. Khóc bé hoặc không khóc",
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
            "Type": 4,
            "Id": "Group4",
            "Label": "4. Da, môi không hồng",
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
                }
            ]
        },
        {
            "Type": 4,
            "Id": "Group5",
            "Label": "5. Bú kém hoặc bỏ bú",
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
            "Label": "6. Tuổi thai < 28 tuần",
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
            "Label": "7. Trẻ < 34 tuần tuổi*",
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
            "Label": "8. Trẻ < 2000g, mẹ có HbsAg(-)",
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
            "Label": "9. Các chống chỉ định khác, nếu có ghi rõ:",
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
                }
            ]
        },
        {
            "Type": 1,
            "Id": "GhiChu1",
            "Label": "*: Chỉ áp dụng với vắc xin giảm độc lực.",
            "Value": null,
            "fxFlex": "100%",
            "fxFlexSm": "100%",
            "IsLightText": true
        },
        {
            "Type": 4,
            "Id": "GroupKhamSangLocChuyenKhoa",
            "Label": "Khám sàng lọc theo chuyên khoa:",
            "fxFlex": "20%",
            "fxFlexSm": "20%",
            "boldText": true,
            "groupItems": [
                {
                    "Type": 5,
                    "Id": "GroupKhamSangLocChuyenKhoaKhong",
                    "Label": "Không",
                    "Value": true,
                    "fxFlex": "10%",
                    "fxFlexSm": "10%"
                },
                {
                    "Type": 5,
                    "Id": "GroupKhamSangLocChuyenKhoaCo",
                    "Label": "Có",
                    "Value": null,
                    "fxFlex": "10%",
                    "fxFlexSm": "10%"
                },
                {
                    "Type": 3,
                    "Id": "GroupChuyenKhoaText",
                    "Label": "Chuyên khoa",
                    "Value": null,
                    "fxFlex": "20%",
                    "fxFlexSm": "20%",
                    "maxlength": 1000,
                    "requiredCheckbox": true
                },
                {
                    "Type": 3,
                    "Id": "LyDo",
                    "Label": "Lý do",
                    "Value": null,
                    "fxFlex": "100%",
                    "fxFlexSm": "100%",
                    "maxlength": 1000,
                    "requiredCheckbox": true
                },
                {
                    "Type": 3,
                    "Id": "KetQua",
                    "Label": "Kết quả",
                    "Value": null,
                    "fxFlex": "100%",
                    "fxFlexSm": "100%",
                    "maxlength": 1000,
                    "requiredCheckbox": true
                },
                {
                    "Type": 3,
                    "Id": "KetLuan",
                    "Label": "Kết luận",
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
WHERE Ten = N'KhamSangLocChungTreSoSinhTrongBenhVien'

Update dbo.CauHinh
Set [Value] = '3.0.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'