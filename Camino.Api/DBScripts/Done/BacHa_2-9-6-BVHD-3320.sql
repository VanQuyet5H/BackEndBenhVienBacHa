ALTER TABLE dbo.[NhapKhoMauChiTiet]
ADD NguoiLamXetNghiemHoaHopId [bigint] NULL
GO

ALTER TABLE dbo.[NhapKhoMauChiTiet]
ADD NguoiLamXetNghiemHoaHop  NVARCHAR(100) NULL
GO

Update dbo.CauHinh
Set [Value] = '2.9.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'
