INSERT [dbo].[CauHinh] ([Name], [DataType], [Description], [Value], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
  VALUES (N'CauHinhNoiTru.DuocPhamBenhVienSinhPham',3, N'Sinh Phẩm', N'Sinh Phẩm', 1, 1,GETDATE(),GETDATE())
GO
INSERT [dbo].[CauHinh] ([Name], [DataType], [Description], [Value], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
  VALUES (N'CauHinhNoiTru.DuocPhamBenhVienSinhPhamChanDoan',3, N'Sinh Phẩm Chẩn Đoán', N'Sinh Phẩm Chẩn Đoán', 1, 1,GETDATE(),GETDATE())
GO

Update dbo.CauHinh
Set [Value] = '2.9.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'