ALTER TABLE dbo.[YeuCauKhamBenh]
ADD [TiLeBaoHiemThanhToan] INT NULL;

ALTER TABLE dbo.[YeuCauDichVuKyThuat]
ADD [TiLeBaoHiemThanhToan] INT NULL;

ALTER TABLE dbo.[YeuCauDuocPhamBenhVien]
ADD [TiLeBaoHiemThanhToan] INT NULL;

ALTER TABLE dbo.[YeuCauVatTuBenhVien]
ADD [TiLeBaoHiemThanhToan] INT NULL;

ALTER TABLE dbo.[YeuCauDichVuGiuongBenhVien]
ADD [TiLeBaoHiemThanhToan] INT NULL;

ALTER TABLE dbo.[DonThuocThanhToanChiTiet]
ADD [TiLeBaoHiemThanhToan] INT NULL;

ALTER TABLE dbo.[DuyetBaoHiemChiTiet]
ADD [TiLeBaoHiemThanhToan] INT NULL;

Update CauHinh
Set [Value] = '0.6.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'
