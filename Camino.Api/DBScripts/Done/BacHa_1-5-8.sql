INSERT [dbo].[CauHinh] ([Name], [DataType], [Description], [Value], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
VALUES (N'CauHinhPhieuThu.TaiKhoanNo', 3, N'Tài khoản nợ', N'111101', 1, 4, CAST(N'2020-07-16T10:21:21.973' AS DateTime), CAST(N'2020-03-03T00:00:00.000' AS DateTime))
GO
INSERT [dbo].[CauHinh] ([Name], [DataType], [Description], [Value], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
VALUES (N'CauHinhPhieuThu.TaiKhoanCo', 3, N'Tài khoản có', N'131101', 1, 4, CAST(N'2020-07-16T10:21:21.973' AS DateTime), CAST(N'2020-03-03T00:00:00.000' AS DateTime))
GO

Update dbo.CauHinh
Set [Value] = '1.5.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'