
ALTER TABLE dbo.YeuCauKhamBenh
 ADD TinhTrangBenhNhanChuyenVien NVARCHAR(2000) NULL;

ALTER TABLE dbo.YeuCauKhamBenh
ADD LoaiLyDoChuyenVien INT NULL;

ALTER TABLE dbo.YeuCauKhamBenh
ADD ThoiDiemChuyenVien DATETIME NULL;

ALTER TABLE dbo.YeuCauKhamBenh
ADD PhuongTienChuyenVien NVARCHAR(2000) NULL;

ALTER TABLE dbo.YeuCauKhamBenh
ADD NhanVienHoTongChuyenVienId BIGINT NULL;

ALTER TABLE [dbo].[YeuCauKhamBenh]  WITH CHECK ADD  CONSTRAINT [FK_YeuCauKhamBenh_NhanVienHoTongChuyenVien] FOREIGN KEY(NhanVienHoTongChuyenVienId)
REFERENCES [dbo].[NhanVien] ([Id])
GO

Update CauHinh
Set [Value] = '0.4.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'
