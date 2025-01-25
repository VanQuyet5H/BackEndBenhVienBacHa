SET IDENTITY_INSERT [dbo].[MauMayXetNghiem] ON 
GO
INSERT [dbo].[MauMayXetNghiem] ([Id], [Ma], [Ten], [TenTiengAnh], [NhaSanXuat], [NhomDichVuBenhVienId], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
VALUES (1044, N'HumaClotPro', N'HumaClotPro', N'HumaClotPro', NULL, 112, NULL, 1, 1, CAST(N'2020-09-01T00:00:00.000' AS DateTime), CAST(N'2020-09-01T00:00:00.000' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[MauMayXetNghiem] OFF

GO
SET IDENTITY_INSERT [dbo].[MayXetNghiem] ON 
INSERT [dbo].[MayXetNghiem] ([Id], [Ma], [Ten], [MauMayXetNghiemID], [NhaCungCap], [HieuLuc], [HostName], [PortName], [BaudRate], [DataBits], [StopBits], [Parity], [Handshake], [Encoding], [ReadBufferSize], [RtsEnable], [DtrEnable], [DiscardNull], [ConnectionMode], [ConnectionProtocol], [AutoOpenPort], [AutoOpenForm], [ConnectionStatus], [OpenById], [OpenDateTime], [CloseDateTime], [LogDataEnabled], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
VALUES (7, N'HumaClotPro', N'HumaClotPro', 1044, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, NULL, 0, 0, 2, NULL, NULL, NULL, 0, 244, 1, CAST(N'2021-03-12T14:32:49.583' AS DateTime), CAST(N'2021-01-13T11:37:34.083' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[MayXetNghiem] OFF

SET IDENTITY_INSERT [dbo].[DichVuXetNghiemKetNoiChiSo] ON 
GO
INSERT [dbo].[DichVuXetNghiemKetNoiChiSo] ([Id], [MaKetNoi], [TenKetNoi], [DichVuXetNghiemId], [MaChiSo], [MauMayXetNghiemId], [HieuLuc], [TiLe], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
VALUES (1870, N'FC1', N'FC1', 11411, N'FC1', 1044, 1, 1, NULL, 1, 1, CAST(N'2020-12-10T14:45:21.580' AS DateTime), CAST(N'2020-12-10T14:38:11.400' AS DateTime))
GO
INSERT [dbo].[DichVuXetNghiemKetNoiChiSo] ([Id], [MaKetNoi], [TenKetNoi], [DichVuXetNghiemId], [MaChiSo], [MauMayXetNghiemId], [HieuLuc], [TiLe], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
VALUES (1871, N'PC2', N'PC2', 21443, N'PC2', 1044, 1, 1, NULL, 1, 1, CAST(N'2020-12-10T14:45:21.580' AS DateTime), CAST(N'2020-12-10T14:38:11.400' AS DateTime))
GO
INSERT [dbo].[DichVuXetNghiemKetNoiChiSo] ([Id], [MaKetNoi], [TenKetNoi], [DichVuXetNghiemId], [MaChiSo], [MauMayXetNghiemId], [HieuLuc], [TiLe], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
VALUES (1872, N'PCR', N'PCR', 21444, N'PCR', 1044, 1, 1, NULL, 1, 1, CAST(N'2020-12-10T14:45:21.580' AS DateTime), CAST(N'2020-12-10T14:38:11.400' AS DateTime))
GO
INSERT [dbo].[DichVuXetNghiemKetNoiChiSo] ([Id], [MaKetNoi], [TenKetNoi], [DichVuXetNghiemId], [MaChiSo], [MauMayXetNghiemId], [HieuLuc], [TiLe], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
VALUES (1873, N'PC1', N'PC1', 21445, N'PC1', 1044, 1, 1, NULL, 1, 1, CAST(N'2020-12-10T14:45:21.580' AS DateTime), CAST(N'2020-12-10T14:38:11.400' AS DateTime))
GO
INSERT [dbo].[DichVuXetNghiemKetNoiChiSo] ([Id], [MaKetNoi], [TenKetNoi], [DichVuXetNghiemId], [MaChiSo], [MauMayXetNghiemId], [HieuLuc], [TiLe], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
VALUES (1874, N'ACR', N'ACR', 21449, N'ACR', 1044, 1, 1, NULL, 1, 1, CAST(N'2020-12-10T14:45:21.580' AS DateTime), CAST(N'2020-12-10T14:38:11.400' AS DateTime))
GO
INSERT [dbo].[DichVuXetNghiemKetNoiChiSo] ([Id], [MaKetNoi], [TenKetNoi], [DichVuXetNghiemId], [MaChiSo], [MauMayXetNghiemId], [HieuLuc], [TiLe], [MoTa], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) 
VALUES (1875, N'AC2', N'AC2', 21450, N'AC2', 1044, 1, 1, NULL, 1, 1, CAST(N'2020-12-10T14:45:21.580' AS DateTime), CAST(N'2020-12-10T14:38:11.400' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[DichVuXetNghiemKetNoiChiSo] OFF
GO

Update CauHinh
Set [Value] = '1.7.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'