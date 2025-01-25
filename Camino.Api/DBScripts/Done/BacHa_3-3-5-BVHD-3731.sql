

CREATE TABLE [dbo].[NoiDungGhiChuMiemGiam](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Ma] [nvarchar](20) NOT NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,
	[NoiDungMiemGiam] [nvarchar](max) NOT NULL
 CONSTRAINT [PK_NoiDungGhiChuMiemGiam] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE YeuCauKhamBenh ADD NoiDungGhiChuMiemGiamId bigint NULL 
ALTER TABLE YeuCauDichVuKyThuat ADD NoiDungGhiChuMiemGiamId bigint NULL
ALTER TABLE YeuCauDuocPhamBenhVien ADD NoiDungGhiChuMiemGiamId bigint NULL 
ALTER TABLE YeuCauVatTuBenhVien ADD NoiDungGhiChuMiemGiamId bigint NULL 
ALTER TABLE DonThuocThanhToanChiTiet ADD NoiDungGhiChuMiemGiamId bigint NULL 
ALTER TABLE YeuCauDichVuGiuongBenhVien ADD NoiDungGhiChuMiemGiamId int NULL 
ALTER TABLE YeuCauGoiDichVu ADD NoiDungGhiChuMiemGiamId bigint NULL 
ALTER TABLE YeuCauTruyenMau ADD NoiDungGhiChuMiemGiamId bigint NULL 
ALTER TABLE YeuCauDichVuGiuongBenhVienChiPhiBenhVien ADD NoiDungGhiChuMiemGiamId bigint NULL 

Update dbo.CauHinh
Set [Value] = '3.3.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'
