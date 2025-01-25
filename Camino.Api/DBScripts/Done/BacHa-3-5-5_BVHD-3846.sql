ALter TABLE [dbo].[Kho]
	Add [LaKhoKSNK] [bit] NULL
GO
SET IDENTITY_INSERT [dbo].[Kho] ON 
GO
INSERT [dbo].[Kho] ([Id], [Ten], [LoaiKho], [KhoaPhongId], [PhongBenhVienId], [IsDefault], [CreatedById], [LastUserId], [LastTime], [CreatedOn], [MaKhoBenhVien], [LoaiDuocPham], [LoaiVatTu], [LaKhoKSNK]) 
	VALUES (300, N'Kho Hành Chính', 10, 26, NULL, 1, 1, 1, CAST(N'2022-03-03T16:10:28.813' AS DateTime), CAST(N'2022-03-03T16:10:28.813' AS DateTime), N'KHC', NULL, 1, 1)
GO
INSERT [dbo].[Kho] ([Id], [Ten], [LoaiKho], [KhoaPhongId], [PhongBenhVienId], [IsDefault], [CreatedById], [LastUserId], [LastTime], [CreatedOn], [MaKhoBenhVien], [LoaiDuocPham], [LoaiVatTu], [LaKhoKSNK]) 
	VALUES (301, N'Kho KSNK', 11, 19, NULL, 1, 1, 1, CAST(N'2022-03-03T16:10:28.813' AS DateTime), CAST(N'2022-03-03T16:10:28.813' AS DateTime), N'KKSNK', 1, 1, 1)
GO
INSERT [dbo].[Kho] ([Id], [Ten], [LoaiKho], [KhoaPhongId], [PhongBenhVienId], [IsDefault], [CreatedById], [LastUserId], [LastTime], [CreatedOn], [MaKhoBenhVien], [LoaiDuocPham], [LoaiVatTu], [LaKhoKSNK]) 
	VALUES (302, N'KSNK tủ khoa Nội', 5, 5, NULL, 0, 1, 1, CAST(N'2022-03-03T16:10:28.813' AS DateTime), CAST(N'2022-03-03T16:10:28.813' AS DateTime), NULL, 1, 1, 1)
GO
INSERT [dbo].[Kho] ([Id], [Ten], [LoaiKho], [KhoaPhongId], [PhongBenhVienId], [IsDefault], [CreatedById], [LastUserId], [LastTime], [CreatedOn], [MaKhoBenhVien], [LoaiDuocPham], [LoaiVatTu], [LaKhoKSNK]) 
	VALUES (303, N'KSNK tủ khoa Ngoại', 5, 6, NULL, 0, 1, 1, CAST(N'2022-03-03T16:10:28.813' AS DateTime), CAST(N'2022-03-03T16:10:28.813' AS DateTime), NULL, 1, 1, 1)
GO
INSERT [dbo].[Kho] ([Id], [Ten], [LoaiKho], [KhoaPhongId], [PhongBenhVienId], [IsDefault], [CreatedById], [LastUserId], [LastTime], [CreatedOn], [MaKhoBenhVien], [LoaiDuocPham], [LoaiVatTu], [LaKhoKSNK]) 
	VALUES (304, N'KSNK tủ khoa Nhi', 5, 7, NULL, 0, 1, 1, CAST(N'2022-03-03T16:10:28.813' AS DateTime), CAST(N'2022-03-03T16:10:28.813' AS DateTime), NULL, 1, 1, 1)
GO
INSERT [dbo].[Kho] ([Id], [Ten], [LoaiKho], [KhoaPhongId], [PhongBenhVienId], [IsDefault], [CreatedById], [LastUserId], [LastTime], [CreatedOn], [MaKhoBenhVien], [LoaiDuocPham], [LoaiVatTu], [LaKhoKSNK]) 
	VALUES (305, N'KSNK tủ khoa Sản', 5, 8, NULL, 0, 1, 1, CAST(N'2022-03-03T16:10:28.813' AS DateTime), CAST(N'2022-03-03T16:10:28.813' AS DateTime), NULL, 1, 1, 1)
GO
INSERT [dbo].[Kho] ([Id], [Ten], [LoaiKho], [KhoaPhongId], [PhongBenhVienId], [IsDefault], [CreatedById], [LastUserId], [LastTime], [CreatedOn], [MaKhoBenhVien], [LoaiDuocPham], [LoaiVatTu], [LaKhoKSNK]) 
	VALUES (306, N'KSNK tủ khoa Thẩm mỹ', 5, 9, NULL, 0, 1, 1, CAST(N'2022-03-03T16:10:28.813' AS DateTime), CAST(N'2022-03-03T16:10:28.813' AS DateTime), NULL, 1, 1, 1)
GO
INSERT [dbo].[Kho] ([Id], [Ten], [LoaiKho], [KhoaPhongId], [PhongBenhVienId], [IsDefault], [CreatedById], [LastUserId], [LastTime], [CreatedOn], [MaKhoBenhVien], [LoaiDuocPham], [LoaiVatTu], [LaKhoKSNK]) 
	VALUES (307, N'KSNK tủ khoa GMHS', 5, 2, NULL, 0, 1, 1, CAST(N'2022-03-03T16:10:28.813' AS DateTime), CAST(N'2022-03-03T16:10:28.813' AS DateTime), NULL, 1, 1, 1)
GO
INSERT [dbo].[Kho] ([Id], [Ten], [LoaiKho], [KhoaPhongId], [PhongBenhVienId], [IsDefault], [CreatedById], [LastUserId], [LastTime], [CreatedOn], [MaKhoBenhVien], [LoaiDuocPham], [LoaiVatTu], [LaKhoKSNK]) 
	VALUES (308, N'KSNK tủ khoa Khám bệnh', 5, 4, NULL, 0, 1, 1, CAST(N'2022-03-03T16:10:28.813' AS DateTime), CAST(N'2022-03-03T16:10:28.813' AS DateTime), NULL, 1, 1, 1)
GO
INSERT [dbo].[Kho] ([Id], [Ten], [LoaiKho], [KhoaPhongId], [PhongBenhVienId], [IsDefault], [CreatedById], [LastUserId], [LastTime], [CreatedOn], [MaKhoBenhVien], [LoaiDuocPham], [LoaiVatTu], [LaKhoKSNK]) 
	VALUES (309, N'KSNK tủ khoa Xét nghiệm', 5, 12, NULL, 0, 1, 1, CAST(N'2022-03-03T16:10:28.813' AS DateTime), CAST(N'2022-03-03T16:10:28.813' AS DateTime), NULL, 1, 1, 1)
GO
INSERT [dbo].[Kho] ([Id], [Ten], [LoaiKho], [KhoaPhongId], [PhongBenhVienId], [IsDefault], [CreatedById], [LastUserId], [LastTime], [CreatedOn], [MaKhoBenhVien], [LoaiDuocPham], [LoaiVatTu], [LaKhoKSNK]) 
	VALUES (310, N'KSNK tủ khoa CĐHA-TDCN', 5, 11, NULL, 0, 1, 1, CAST(N'2022-03-03T16:10:28.813' AS DateTime), CAST(N'2022-03-03T16:10:28.813' AS DateTime), NULL, 1, 1, 1)
GO
SET IDENTITY_INSERT [dbo].[Kho] OFF
GO

Update dbo.CauHinh
Set [Value] = '3.5.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'
