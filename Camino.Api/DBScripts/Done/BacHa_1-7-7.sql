ALTER TABLE KetQuaVaKetLuanMau ADD KetQuaMau NVARCHAR(2000) NULL
GO
ALTER TABLE KetQuaVaKetLuanMau ADD KetLuanMau NVARCHAR(2000) NULL
GO

UPDATE KetQuaVaKetLuanMau SET KetQuaMau = NoiDung WHERE LoaiKetQuaVaKetLuanMau = 1
GO
UPDATE KetQuaVaKetLuanMau SET KetLuanMau = NoiDung WHERE LoaiKetQuaVaKetLuanMau = 2
GO

ALTER TABLE KetQuaVaKetLuanMau ALTER COLUMN KetQuaMau NVARCHAR(2000) NOT NULL
GO
ALTER TABLE KetQuaVaKetLuanMau ALTER COLUMN KetLuanMau NVARCHAR(2000) NOT NULL
GO

ALTER TABLE KetQuaVaKetLuanMau DROP COLUMN NoiDung
GO
ALTER TABLE KetQuaVaKetLuanMau DROP COLUMN LoaiKetQuaVaKetLuanMau


GO
Update CauHinh
Set [Value] = '1.7.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'