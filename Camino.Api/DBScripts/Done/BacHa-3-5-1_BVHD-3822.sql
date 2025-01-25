ALTER TABLE DichVuKyThuatBenhVien
ADD ChuyenKhoaChuyenNganhId [bigint] NULL
GO 

ALTER TABLE DichVuKyThuatBenhVien
ADD  SoPhimXquang  int NULL
GO 


CREATE TABLE [dbo].[ChuyenKhoaChuyenNganh](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ten] [nvarchar](250) NULL,
	[MoTa] [nvarchar](1000) NULL,
	[HieuLuc] [bit] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
 CONSTRAINT [PK_ChuyenKhoaChuyenNganh] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[ChuyenKhoaChuyenNganh] ON 

INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (1, N'Hồi sức cấp cứu và Chống độc', N'Hồi sức cấp cứu và Chống độc', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (2, N'Nội khoa', N'Nội khoa', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (3, N'Nhi khoa', N'Nhi khoa', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (4, N'Lao (ngoại lao)', N'Lao (ngoại lao)', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (5, N'Da liễu', N'Da liễu', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (6, N'Tâm thần', N'Tâm thần', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (7, N'Nội tiết', N'Nội tiết', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (8, N'Y học cổ truyền', N'Y học cổ truyền', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (9, N'Gây mê hồi sức', N'Gây mê hồi sức', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (10, N'Ngoại khoa', N'Ngoại khoa', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (11, N'Bỏng', N'Bỏng', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (12, N'Ung bướu', N'Ung bướu', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (13, N'Phụ sản', N'Phụ sản', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (14, N'Mắt', N'Mắt', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (15, N'Tai mũi họng', N'Tai mũi họng', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (16, N'Răng hàm mặt', N'Răng hàm mặt', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (17, N'Phục hồi chức năng', N'Phục hồi chức năng', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (18, N'Điện quang', N'Điện quang', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (19, N'Y học hạt nhân', N'Y học hạt nhân', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (20, N'Nội soi chẩn đoán, can thiệp', N'Nội soi chẩn đoán, can thiệp', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (21, N'Thăm dò chức năng', N'Thăm dò chức năng', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (22, N'Huyết học – truyền máu', N'Huyết học – truyền máu', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (23, N'Hoá sinh', N'Hoá sinh', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (24, N'Vi sinh', N'Vi sinh', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (25, N'Giải phẫu bệnh và TB bệnh học', N'Giải phẫu bệnh và TB bệnh học', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (26, N'Vi phẫu', N'Vi phẫu', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (27, N'Phẫu thuật nội soi', N'Phẫu thuật nội soi', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
INSERT [dbo].[ChuyenKhoaChuyenNganh] ([Id], [Ten], [MoTa], [HieuLuc], [CreatedById], [LastUserId], [LastTime], [CreatedOn]) VALUES (28, N'Tạo hình – Thẩm mỹ', N'Tạo hình – Thẩm mỹ', 1, 1, 1, CAST(N'2022-01-01T00:00:00.000' AS DateTime), CAST(N'2022-01-01T00:00:00.000' AS DateTime))
SET IDENTITY_INSERT [dbo].[ChuyenKhoaChuyenNganh] OFF
GO


update [dbo].[DichVuKyThuatBenhVien] set LoaiPhauThuatThuThuat=N'Thủ thuật loại 1' where LoaiPhauThuatThuThuat = 'T1'
update [dbo].[DichVuKyThuatBenhVien] set LoaiPhauThuatThuThuat=N'Thủ thuật loại 2' where LoaiPhauThuatThuThuat = 'T2'
update [dbo].[DichVuKyThuatBenhVien] set LoaiPhauThuatThuThuat=N'Thủ thuật loại 3' where LoaiPhauThuatThuThuat = 'T3'
update [dbo].[DichVuKyThuatBenhVien] set LoaiPhauThuatThuThuat=N'Thủ thuật loại đặc biệt' where LoaiPhauThuatThuThuat = 'TDB'

update [dbo].[DichVuKyThuatBenhVien] set LoaiPhauThuatThuThuat=N'Phẫu thuật loại 1' where LoaiPhauThuatThuThuat = 'P1'
update [dbo].[DichVuKyThuatBenhVien] set LoaiPhauThuatThuThuat=N'Phẫu thuật loại 2' where LoaiPhauThuatThuThuat = 'P2'
update [dbo].[DichVuKyThuatBenhVien] set LoaiPhauThuatThuThuat=N'Phẫu thuật loại 3' where LoaiPhauThuatThuThuat = 'P3'
update [dbo].[DichVuKyThuatBenhVien] set LoaiPhauThuatThuThuat=N'Phẫu thuật loại đặc biệt' where LoaiPhauThuatThuThuat = 'PDB'


Update dbo.CauHinh
Set [Value] = '3.5.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'
