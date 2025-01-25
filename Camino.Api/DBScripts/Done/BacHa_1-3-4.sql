INSERT INTO [dbo].[CauHinh]
           ([Name]
           ,[DataType]
           ,[Description]
           ,[Value]
           ,[CreatedById]
           ,[LastUserId]
           ,[LastTime]
           ,[CreatedOn])
     VALUES
           (N'CauHinhNoiTru.SoLuongBenhNhanToiDaTrenGiuong'
           ,2
           ,N'Số lượng bệnh nhân tối đa trên 1 giường'
           ,3
           ,1
           ,1
           ,GETDATE()
           ,GETDATE())
GO
ALTER TABLE DichVuGiuongBenhVienGiaBenhVien
ADD BaoPhong BIT NULL
GO

ALTER TABLE PhongBenhVien
ADD Tang NVARCHAR(20) NULL
GO

ALTER TABLE YeuCauDichVuGiuongBenhVien
ADD BaoPhong BIT NULL
GO

UPDATE dbo.CauHinh
Set [Value] = '1.3.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'