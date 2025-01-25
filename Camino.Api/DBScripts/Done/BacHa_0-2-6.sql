
ALTER TABLE BenhNhanTienSuBenh
ADD
	LoaiTienSuBenh INT NULL
GO

UPDATE BenhNhanTienSuBenh SET LoaiTienSuBenh = 1
GO

ALTER TABLE BenhNhanTienSuBenh
	ALTER COLUMN LoaiTienSuBenh INT NOT NULL
GO

ALTER TABLE BenhNhanTienSuBenh
ALTER COLUMN TenBenh NVARCHAR(200) NOT NULL
GO

ALTER TABLE BenhNhanTienSuBenh
DROP Column NgayPhatHien
GO

ALTER TABLE BenhNhanTienSuBenh
DROP Column TinhTrangBenh
GO

Update CauHinh
Set [Value] = '0.2.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'