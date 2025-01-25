
INSERT [dbo].[CauHinh] ( [Name], [DataType], [Description], [Value], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES ( N'CauHinhTiepNhan.QuocTich', 2, N'Quốc tịch', N'1', 1, 1, CAST(N'2020-07-17T11:15:32.717' AS DateTime), CAST(N'2020-07-13T16:00:00.000' AS DateTime))
GO
INSERT [dbo].[CauHinh] ( [Name], [DataType], [Description], [Value], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES ( N'CauHinhTiepNhan.TinhThanhPho', 2, N'Tỉnh/Thành phố', N'1', 1, 1, CAST(N'2020-07-17T11:15:32.717' AS DateTime), CAST(N'2020-07-13T16:00:00.000' AS DateTime))
GO
INSERT [dbo].[CauHinh] ( [Name], [DataType], [Description], [Value], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES ( N'CauHinhTiepNhan.DanToc', 2, N'Dân tộc', N'1', 1, 1, CAST(N'2020-07-17T11:15:32.717' AS DateTime), CAST(N'2020-07-13T16:00:00.000' AS DateTime))
GO
INSERT [dbo].[CauHinh] ( [Name], [DataType], [Description], [Value], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES ( N'CauHinhTiepNhan.LyDoTiepNhan', 2, N'Lý do tiếp nhận', N'5', 1, 1, CAST(N'2020-07-17T11:15:32.717' AS DateTime), CAST(N'2020-07-13T16:00:00.000' AS DateTime))
GO
INSERT [dbo].[CauHinh] ( [Name], [DataType], [Description], [Value], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES ( N'CauHinhTiepNhan.HinhThucDen', 2, N'Hình thức đến', N'1', 1, 1, CAST(N'2020-07-17T11:15:32.717' AS DateTime), CAST(N'2020-07-13T16:00:00.000' AS DateTime))
GO

Update CauHinh
Set [Value] = '1.0.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'