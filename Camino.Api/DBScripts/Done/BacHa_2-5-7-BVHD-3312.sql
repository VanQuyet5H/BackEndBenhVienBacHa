-- tạo column mới
ALTER TABLE NoiTruPhieuDieuTriChiTietDienBien
ADD NoiTruBenhAnId BIGINT NULL
GO

ALTER TABLE NoiTruPhieuDieuTriChiTietDienBien
ADD NgayDieuTri DATETIME NULL
GO

ALTER TABLE NoiTruPhieuDieuTriChiTietYLenh
ADD NoiTruBenhAnId BIGINT NULL
GO

ALTER TABLE NoiTruPhieuDieuTriChiTietYLenh
ADD NgayDieuTri DATETIME NULL
GO

ALTER TABLE [dbo].[NoiTruPhieuDieuTriChiTietDienBien]  WITH CHECK ADD  CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietDienBien_NoiTruBenhAn] FOREIGN KEY([NoiTruBenhAnId])
REFERENCES [dbo].[NoiTruBenhAn] ([Id])
GO

ALTER TABLE [dbo].[NoiTruPhieuDieuTriChiTietDienBien] CHECK CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietDienBien_NoiTruBenhAn]
GO

ALTER TABLE [dbo].[NoiTruPhieuDieuTriChiTietYLenh]  WITH CHECK ADD  CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietYLenh_NoiTruBenhAn] FOREIGN KEY([NoiTruBenhAnId])
REFERENCES [dbo].[NoiTruBenhAn] ([Id])
GO

ALTER TABLE [dbo].[NoiTruPhieuDieuTriChiTietYLenh] CHECK CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietYLenh_NoiTruBenhAn]
GO

-- cập nhật data
UPDATE n
SET n.NoiTruBenhAnId = p.NoiTruBenhAnId,
	n.NgayDieuTri = p.NgayDieuTri
FROM NoiTruPhieuDieuTriChiTietDienBien n
LEFT JOIN NoiTruPhieuDieuTri p ON n.NoiTruPhieuDieuTriId = p.Id
GO

UPDATE yl
SET yl.NoiTruBenhAnId = p.NoiTruBenhAnId,
	yl.NgayDieuTri = p.NgayDieuTri
FROM NoiTruPhieuDieuTriChiTietYLenh yl
LEFT JOIN NoiTruPhieuDieuTri p ON yl.NoiTruPhieuDieuTriId = p.Id
GO

-- set lại not null
ALTER TABLE NoiTruPhieuDieuTriChiTietDienBien
ALTER COLUMN NoiTruBenhAnId BIGINT NOT NULL
GO

ALTER TABLE NoiTruPhieuDieuTriChiTietDienBien
ALTER COLUMN NgayDieuTri DATETIME NOT NULL
GO

ALTER TABLE NoiTruPhieuDieuTriChiTietYLenh
ALTER COLUMN NoiTruBenhAnId BIGINT NOT NULL
GO

ALTER TABLE NoiTruPhieuDieuTriChiTietYLenh
ALTER COLUMN NgayDieuTri DATETIME NOT NULL
GO

-- xóa column
ALTER TABLE NoiTruPhieuDieuTriChiTietDienBien
DROP COLUMN NoiTruPhieuDieuTriId
GO

ALTER TABLE NoiTruPhieuDieuTriChiTietYLenh
DROP COLUMN NoiTruPhieuDieuTriId
GO

UPDATE NoiTruBenhAn
SET ThoiDiemTongHopYLenh = NULL

GO
Update dbo.CauHinh
Set [Value] = '2.5.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'
