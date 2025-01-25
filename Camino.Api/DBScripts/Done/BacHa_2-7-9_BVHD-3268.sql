SET IDENTITY_INSERT [dbo].[NhomDichVuBenhVien] ON 
GO
INSERT INTO [dbo].[NhomDichVuBenhVien]
           ([Id]
		   ,[Ma]
           ,[Ten]
           ,[MoTa]
           ,[IsDefault]
           ,[CreatedById]
           ,[LastUserId]
           ,[LastTime]
           ,[CreatedOn]
           ,[NhomDichVuBenhVienChaId])
     VALUES
           (300
		   ,N'SLTC'
           ,N'SÀNG LỌC TIÊM CHỦNG'
           ,N'Dịch vụ khám sàng lọc tiêm chủng'
           ,1
           ,1
           ,1
           ,GETDATE()
           ,GETDATE()
           ,NULL)
GO
SET IDENTITY_INSERT [dbo].[NhomDichVuBenhVien] OFF
GO

INSERT INTO [dbo].[DichVuKyThuatBenhVien]
           ([NgayBatDau]
           ,[HieuLuc]
           ,[CreatedById]
           ,[LastUserId]
           ,[LastTime]
           ,[CreatedOn]
           ,[Ma]
           ,[Ten]
           ,[NhomDichVuBenhVienId])
     VALUES
           (N'2021-01-01'
           ,1
           ,1
           ,1
           ,GETDATE()
           ,GETDATE()
           ,N'KTV'
           ,N'Khám tiêm Vacxin'
           ,300)
GO

DECLARE @DichVuId BIGINT = IDENT_CURRENT('DichVuKyThuatBenhVien'),
		@NhomGiaId BIGINT = (SELECT TOP 1 id FROM NhomGiaDichVuKyThuatBenhVien WHERE TEN LIKE N'%thường%')
INSERT INTO [dbo].[DichVuKyThuatBenhVienGiaBenhVien]
           ([DichVuKyThuatBenhVienId]
           ,[NhomGiaDichVuKyThuatBenhVienId]
           ,[Gia]
           ,[TuNgay]
           ,[DenNgay]
           ,[CreatedById]
           ,[LastUserId]
           ,[LastTime]
           ,[CreatedOn])
     VALUES
           (@DichVuId
           ,@NhomGiaId
           ,0
           ,N'2021-01-01'
           ,NULL
           ,1
           ,1
           ,GETDATE()
           ,GETDATE())
GO

ALTER TABLE YeuCauDichVuKyThuatKhamSangLocTiemChung
ADD NoiTheoDoiSauTiemId BIGINT NULL


-- cần bổ sung thêm nơi thực hiện và cập nhật lại mã dịch vụ

GO
Update dbo.CauHinh
Set [Value] = '2.7.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'