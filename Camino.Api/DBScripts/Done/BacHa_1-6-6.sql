
--can kiem tra khi chay
update NhomGiaDichVuGiuongBenhVien set Ten=N'Bao phòng' where Id=2;
delete DichVuGiuongBenhVienGiaBenhVien where NhomGiaDichVuGiuongBenhVienId=3;
delete TaiKhoanBenhNhanChi where YeuCauDichVuGiuongBenhVienChiPhiBenhVienId in (select Id from YeuCauDichVuGiuongBenhVienChiPhiBenhVien where NhomGiaDichVuGiuongBenhVienId=3);
delete YeuCauDichVuGiuongBenhVienChiPhiBenhVien where NhomGiaDichVuGiuongBenhVienId=3;
delete HoatDongGiuongBenh where YeuCauDichVuGiuongBenhVienId in (select Id from YeuCauDichVuGiuongBenhVien where NhomGiaDichVuGiuongBenhVienId=3);
delete YeuCauDichVuGiuongBenhVien where NhomGiaDichVuGiuongBenhVienId=3;
delete NhomGiaDichVuGiuongBenhVien where Id=3;
SET IDENTITY_INSERT [dbo].[NhomDichVuBenhVien] ON
GO
INSERT [dbo].[NhomDichVuBenhVien] ([Id], [Ma], [Ten], [MoTa], [IsDefault], [CreatedById], [LastUserId], [LastTime], [CreatedOn], [NhomDichVuBenhVienChaId]) VALUES (200, N'SA', N'SUẤT ĂN', NULL, 1, 1, 1, CAST(N'2020-09-15T00:00:00.000' AS DateTime), CAST(N'2020-09-15T00:00:00.000' AS DateTime), NULL)
GO
SET IDENTITY_INSERT [dbo].[NhomDichVuBenhVien] OFF
GO

Update CauHinh
Set [Value] = '1.6.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'