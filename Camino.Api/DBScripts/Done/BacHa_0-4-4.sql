CREATE TABLE [dbo].[DichVuBenhVienTongHop](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[LoaiDichVuBenhVien] [int] NOT NULL,--1:DichVuKhamBenh, 2:DichVuKyThuat, 3:DichVuGiuong--
	[DichVuKhamBenhBenhVienId] [bigint] NULL,
	[DichVuKyThuatBenhVienId] [bigint] NULL,
	[DichVuGiuongBenhVienId] [bigint] NULL,
	[Ma] [nvarchar](50) NULL,
	[Ten] [nvarchar](250) NOT NULL,
	[MoTa] [nvarchar](4000) NULL,
	[HieuLuc] [bit] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[LastUserId] [bigint] NOT NULL,
	[LastTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModified] [timestamp] NOT NULL,	
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DichVuBenhVienTongHop]  WITH CHECK ADD  CONSTRAINT [FK_DichVuBenhVienTongHop_DichVuGiuongBenhVien] FOREIGN KEY([DichVuGiuongBenhVienId])
REFERENCES [dbo].[DichVuGiuongBenhVien] ([Id])
GO

ALTER TABLE [dbo].[DichVuBenhVienTongHop] CHECK CONSTRAINT [FK_DichVuBenhVienTongHop_DichVuGiuongBenhVien]
GO

ALTER TABLE [dbo].[DichVuBenhVienTongHop]  WITH CHECK ADD  CONSTRAINT [FK_DichVuBenhVienTongHop_DichVuKhamBenhBenhVien] FOREIGN KEY([DichVuKhamBenhBenhVienId])
REFERENCES [dbo].[DichVuKhamBenhBenhVien] ([Id])
GO

ALTER TABLE [dbo].[DichVuBenhVienTongHop] CHECK CONSTRAINT [FK_DichVuBenhVienTongHop_DichVuKhamBenhBenhVien]
GO

ALTER TABLE [dbo].[DichVuBenhVienTongHop]  WITH CHECK ADD  CONSTRAINT [FK_DichVuBenhVienTongHop_DichVuKyThuatBenhVien] FOREIGN KEY([DichVuKyThuatBenhVienId])
REFERENCES [dbo].[DichVuKyThuatBenhVien] ([Id])
GO

ALTER TABLE [dbo].[DichVuBenhVienTongHop] CHECK CONSTRAINT [FK_DichVuBenhVienTongHop_DichVuKyThuatBenhVien]
GO

INSERT INTO [DichVuBenhVienTongHop] ([LoaiDichVuBenhVien], [DichVuKhamBenhBenhVienId], [Ma],[Ten],[MoTa],[HieuLuc],[CreatedById],[LastUserId],[LastTime],[CreatedOn])
SELECT 1, [Id], [Ma],[Ten],[MoTa],[HieuLuc],[CreatedById],[LastUserId],[LastTime],[CreatedOn]
FROM [DichVuKhamBenhBenhVien];
GO
INSERT INTO [DichVuBenhVienTongHop] ([LoaiDichVuBenhVien], [DichVuKyThuatBenhVienId], [Ma],[Ten],[MoTa],[HieuLuc],[CreatedById],[LastUserId],[LastTime],[CreatedOn])
SELECT 2, [Id], [Ma],[Ten],[MoTa],[HieuLuc],[CreatedById],[LastUserId],[LastTime],[CreatedOn]
FROM [DichVuKyThuatBenhVien];
GO
INSERT INTO [DichVuBenhVienTongHop] ([LoaiDichVuBenhVien], [DichVuGiuongBenhVienId], [Ma],[Ten],[MoTa],[HieuLuc],[CreatedById],[LastUserId],[LastTime],[CreatedOn])
SELECT 3, [Id], [Ma],[Ten],[MoTa],[HieuLuc],[CreatedById],[LastUserId],[LastTime],[CreatedOn]
FROM [DichVuGiuongBenhVien];
GO

CREATE FUNCTION [LayDiaChiDayDu] (@dia_chi nvarchar(200), @phuong_xa_id bigint, @quan_huyen_id bigint, @tinh_thanh_id bigint)  
RETURNS nvarchar(500)
AS  
BEGIN 
	DECLARE @ten_phuong_xa nvarchar(100);
	DECLARE @ten_quan_huyen nvarchar(100);
	DECLARE @ten_tinh_thanh nvarchar(100);
	DECLARE @dia_chi_day_du nvarchar(500);

    SELECT @ten_phuong_xa = [Ten]
    FROM [DonViHanhChinh]
    WHERE [Id] = @phuong_xa_id;

	SELECT @ten_quan_huyen = [Ten]
    FROM [DonViHanhChinh]
    WHERE [Id] = @quan_huyen_id;

	SELECT @ten_tinh_thanh = [Ten]
    FROM [DonViHanhChinh]
    WHERE [Id] = @tinh_thanh_id;

	SET @dia_chi_day_du = CASE WHEN @dia_chi IS NULL THEN '' ELSE @dia_chi END;
	SET @dia_chi_day_du = @dia_chi_day_du + (CASE WHEN @ten_phuong_xa IS NULL THEN '' ELSE (CASE WHEN @dia_chi_day_du = '' THEN '' ELSE ', ' END) + @ten_phuong_xa END);
	SET @dia_chi_day_du = @dia_chi_day_du + (CASE WHEN @ten_quan_huyen IS NULL THEN '' ELSE (CASE WHEN @dia_chi_day_du = '' THEN '' ELSE ', ' END) + @ten_quan_huyen END);
	SET @dia_chi_day_du = @dia_chi_day_du + (CASE WHEN @ten_tinh_thanh IS NULL THEN '' ELSE (CASE WHEN @dia_chi_day_du = '' THEN '' ELSE ', ' END) + @ten_tinh_thanh END);
	
	RETURN @dia_chi_day_du;
END;
GO

ALTER TABLE [BenhNhan]
ADD [DiaChiDayDu] AS dbo.[LayDiaChiDayDu](DiaChi,PhuongXaId,QuanHuyenId,TinhThanhId);
GO

ALTER TABLE [YeuCauTiepNhan]
ADD [DiaChiDayDu] AS dbo.[LayDiaChiDayDu](DiaChi,PhuongXaId,QuanHuyenId,TinhThanhId);

Update CauHinh
Set [Value] = '0.4.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'
