INSERT [dbo].[KhoaPhong] 
([Ten], [Ma], [LoaiKhoaPhong], [CoKhamNgoaiTru], [SoTienThuTamUng], [IsDisabled], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn], [IsDefault])
VALUES (N'Khoa Khám Đoàn Ngoại Viện', N'KKDNV', 2, 1, NULL, NULL, NULL, 1, 1, CAST(N'2021-01-01T00:00:00.000' AS DateTime), CAST(N'2021-01-01T00:00:00.000' AS DateTime), 0)

GO
Update CauHinh
Set [Value] = '1.8.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'