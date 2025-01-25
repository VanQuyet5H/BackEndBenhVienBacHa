INSERT [dbo].[CauHinh] ([Name], [DataType], [Description], [Value], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (N'CauHinhBaoCao.VatTuSapHetHanNgayHetHan', 2, N'Ngày sắp hết hạn vật tư tồn kho', N'30', 1, 4, CAST(N'2020-07-16T10:21:21.973' AS DateTime), CAST(N'2020-03-03T00:00:00.000' AS DateTime))
GO
INSERT [dbo].[CauHinh] ([Name], [DataType], [Description], [Value], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (N'CauHinhBaoCao.DuocPhamSapHetHanNgayHetHan', 2, N'Ngày sắp hết hạn dược phẩm tồn kho', N'30', 1, 4, CAST(N'2020-07-16T10:21:21.973' AS DateTime), CAST(N'2020-03-03T00:00:00.000' AS DateTime))
GO

UPDATE CauHinh
SET [Value] = '0.9.4'
WHERE [Name] = 'CauHinhHeThong.DatabaseVesion'