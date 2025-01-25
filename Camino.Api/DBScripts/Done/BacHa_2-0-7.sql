ALTER TABLE dbo.DichVuXetNghiemKetNoiChiSo ADD NotSendOrder BIT NULL;
GO
INSERT [dbo].[DichVuXetNghiemKetNoiChiSo] ([MaKetNoi], [TenKetNoi], [DichVuXetNghiemId], [MaChiSo], [MauMayXetNghiemId], [HieuLuc], [TiLe], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn], [NotSendOrder]) 
  VALUES (N'965', N'965', 21364, N'965', 1043, 1, 1, NULL, 1, 1, CAST(N'2020-12-10T14:45:21.580' AS DateTime), CAST(N'2020-12-10T14:38:11.400' AS DateTime), 1)
GO
INSERT [dbo].[DichVuXetNghiemKetNoiChiSo] ([MaKetNoi], [TenKetNoi], [DichVuXetNghiemId], [MaChiSo], [MauMayXetNghiemId], [HieuLuc], [TiLe], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn], [NotSendOrder]) 
  VALUES (N'964', N'964', 21388, N'964', 1043, 1, 1, NULL, 1, 1, CAST(N'2020-12-10T14:45:21.580' AS DateTime), CAST(N'2020-12-10T14:38:11.400' AS DateTime), 1)
GO
INSERT [dbo].[DichVuXetNghiemKetNoiChiSo] ([MaKetNoi], [TenKetNoi], [DichVuXetNghiemId], [MaChiSo], [MauMayXetNghiemId], [HieuLuc], [TiLe], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn], [NotSendOrder]) 
  VALUES (N'963', N'963', 11487, N'963', 1043, 1, 1, NULL, 1, 1, CAST(N'2020-12-10T14:45:21.580' AS DateTime), CAST(N'2020-12-10T14:38:11.400' AS DateTime), 1)
GO
INSERT [dbo].[DichVuXetNghiemKetNoiChiSo] ([MaKetNoi], [TenKetNoi], [DichVuXetNghiemId], [MaChiSo], [MauMayXetNghiemId], [HieuLuc], [TiLe], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn], [NotSendOrder]) 
  VALUES (N'962', N'962', 11483, N'962', 1043, 1, 1, NULL, 1, 1, CAST(N'2020-12-10T14:45:21.580' AS DateTime), CAST(N'2020-12-10T14:38:11.400' AS DateTime), 1)
GO

UPDATE CauHinh
Set [Value] = '2.0.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'