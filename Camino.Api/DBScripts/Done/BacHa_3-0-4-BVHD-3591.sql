ALTER TABLE dbo.[HopDongKhamSucKhoe]
ADD NhanVienMoLaiHopDongId [bigint] NULL
GO

ALTER TABLE dbo.[HopDongKhamSucKhoe]
ADD LyDoMoLaiHopHopDong  NVARCHAR(1000) NULL
GO

ALTER TABLE dbo.[HopDongKhamSucKhoe]
ADD NgayMoLaiHopDong datetime  NULL
GO

Update dbo.CauHinh
Set [Value] = '3.0.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'