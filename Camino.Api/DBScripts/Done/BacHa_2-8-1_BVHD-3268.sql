INSERT INTO Kho (Ten, LoaiKho, KhoaPhongId, PhongBenhVienId, IsDefault, CreatedById, LastUserId, LastTime, CreatedOn, MaKhoBenhVien, LoaiDuocPham, LoaiVatTu)
	VALUES(N'Kho Vắc xin', 9, NULL, NULL, 1, 1, 1, GETDATE(), GETDATE(), N'KVX', 1, 0)

DECLARE @NhomDichVuBenhVienTiemChungId BIGINT = (SELECT TOP 1 Id FROM NhomDichVuBenhVien WHERE Ma = N'TC')
INSERT INTO [dbo].[CauHinh]
           ([Name]
           ,[DataType]
           ,[Description]
           ,[Value]
           ,[CreatedById]
           ,[LastUserId]
           ,[LastTime]
           ,[CreatedOn])
     VALUES
           (N'CauHinhTiemChung.NhomDichVuTiemChung'
           ,2
           ,N'Nhóm dịch vụ tiêm chủng'
           ,CAST(ISNULL(@NhomDichVuBenhVienTiemChungId, 0) as NVARCHAR)
           ,1
           ,1
           ,GETDATE()
           ,GETDATE())	

INSERT INTO TemplateKhamSangLocTiemChung (Ten, TieuDe, ComponentDynamics, CreatedById, LastUserId, LastTime, CreatedOn)
	VALUES(N'KhamSangLocDoiTuongTrenMotThangTuoi', N'Khám sàng lọc đối tượng trên 1 tháng tuổi', N'{
    "ComponentDynamics": [
        {
            "Type": 4,
            "Id": "Group1",
            "Label": "1. Sốc, phẩn ứng nặng sau lần tiêm chủng trước?",
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
            "Label": "2. Đang mắc bệnh cấp tính hoặc bệnh mạn tính tiến triển*",
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
            "Label": "3. Đang hoặc mới kết thúc đợt điều trị corticoid liều cao (tương đương prednison > 2mg/kg/ngày), hoá trị, xạ trị, gammaglobulin**",
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
            "Label": "4. Sốt/Hạ thân nhiệt (sốt nhiệt độ &ge; 37,5&deg;C; Hạ thân nhiệt, nhiệt độ &le; 35,5&deg;C",
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
            "Label": "5. Nghe tim bất thường* * *",
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
            "Label": "6. Nhịp thở, nghe phổi bất thường",
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
            "Label": "7. Tri giác bất thường (li bì hoặc kích thích)",
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
            "Label": "8. Cân nặng < 2000g",
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
            "Label": "9. Các chống chỉ định/tạm hoãn khác, nếu có ghi rõ:",
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
            "Label": "*: Không hoãn tiêm vắc xin đối với trẻ có bệnh nhẹ (ho, sổ mũi, tiêu chảy mức độ nhẹ... và không sốt), bú tốt, ăn tốt.",
            "Value": null,
            "fxFlex": "100%",
            "fxFlexSm": "100%",
            "IsLightText": true
        },
        {
            "Type": 1,
            "Id": "GhiChu2",
            "Label": "**: Trừ kháng huyết thanh viêm gan B. Tiêu chuẩn này chỉ áp dụng với vắc xin sống giảm độc lực.",
            "Value": null,
            "fxFlex": "100%",
            "fxFlexSm": "100%",
            "IsLightText": true
        }
    ]
}', 1, 1, GETDATE(), GETDATE())

INSERT INTO TemplateKhamSangLocTiemChung (Ten, TieuDe, ComponentDynamics, CreatedById, LastUserId, LastTime, CreatedOn)
	VALUES(N'KhamSangLocChungTreSoSinh', N'Khám sàng lọc chung trẻ sơ sinh', N'{
    "ComponentDynamics": [
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
            "Label": "3. Khóc bé hoặc không khóc được",
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
            "Label": "6. Tuổi thai < 34 tuần",
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
            "Label": "7. Trẻ < 2000g",
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
            "Label": "8. Các chống chỉ định khác, nếu có ghi rõ:",
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
                },
                {
                    "Type": 3,
                    "Id": "Group8Text",
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
            "Label": "*: Không hoãn tiêm vắc xin đối với trẻ có bệnh nhẹ (ho, sổ mũi, tiêu chảy mức độ nhẹ... và không sốt), bú tốt, ăn tốt.",
            "Value": null,
            "fxFlex": "100%",
            "fxFlexSm": "100%",
            "IsLightText": true
        },
        {
            "Type": 1,
            "Id": "GhiChu2",
            "Label": "**: Trừ kháng huyết thanh viêm gan B. Tiêu chuẩn này chỉ áp dụng với vắc xin sống giảm độc lực.",
            "Value": null,
            "fxFlex": "100%",
            "fxFlexSm": "100%",
            "IsLightText": true
        }
    ]
}', 1, 1, GETDATE(), GETDATE())

Update dbo.CauHinh
Set [Value] = '2.8.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'