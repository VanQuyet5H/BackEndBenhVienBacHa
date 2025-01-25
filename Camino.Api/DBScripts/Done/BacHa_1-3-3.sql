ALTER TABLE KetQuaSinhHieu
ADD ThoiDiemThucHien DATETIME NULL
GO

ALTER TABLE NoiTruPhieuDieuTri
ADD BenhNhanCapCuu BIT NULL
GO

UPDATE dbo.CauHinh
Set [Value] = '1.3.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'