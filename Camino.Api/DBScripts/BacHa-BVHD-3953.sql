ALTER TABLE PhauThuatThuThuatEkipBacSi
ALTER COLUMN NhanVienId BIGINT NULL
GO

ALTER TABLE PhauThuatThuThuatEkipBacSi
ADD BacSiHopTacId BIGINT NULL
GO

ALTER TABLE PhauThuatThuThuatEkipBacSi  WITH CHECK ADD FOREIGN KEY(BacSiHopTacId)
REFERENCES [dbo].[NoiGioiThieu] ([Id])
GO

ALTER TABLE NoiGioiThieu
ADD LaBacSiHopTac BIT NULL
GO

Go
UPDATE CauHinh
Set [Value] = '3.4.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'