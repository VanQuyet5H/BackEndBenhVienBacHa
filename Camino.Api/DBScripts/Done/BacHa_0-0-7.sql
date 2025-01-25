ALTER TABLE KhoaPhong
ADD IsDefault bit NOT NULL DEFAULT(0)
GO
UPDATE KhoaPhong
SET IsDefault = 1 WHERE [Id] = 13
GO
Update CauHinh
Set [Value] = '0.0.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'