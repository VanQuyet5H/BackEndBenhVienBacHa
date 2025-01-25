ALTER TABLE dbo.[YeuCauKhamBenh]
ADD [DonGiaBaoHiem] DECIMAL(15, 2) NULL;

ALTER TABLE dbo.[YeuCauKhamBenh]
ADD [MucHuongBaoHiem] INT NULL;

ALTER TABLE dbo.[YeuCauDichVuKyThuat]
ADD [DonGiaBaoHiem] DECIMAL(15, 2) NULL;

ALTER TABLE dbo.[YeuCauDichVuKyThuat]
ADD [MucHuongBaoHiem] INT NULL;

ALTER TABLE dbo.[YeuCauDuocPhamBenhVien]
ADD [DonGiaBaoHiem] DECIMAL(15, 2) NULL;

ALTER TABLE dbo.[YeuCauDuocPhamBenhVien]
ADD [MucHuongBaoHiem] INT NULL;

ALTER TABLE dbo.[YeuCauVatTuBenhVien]
ADD [DonGiaBaoHiem] DECIMAL(15, 2) NULL;

ALTER TABLE dbo.[YeuCauVatTuBenhVien]
ADD [MucHuongBaoHiem] INT NULL;

ALTER TABLE dbo.[YeuCauDichVuGiuongBenhVien]
ADD [DonGiaBaoHiem] DECIMAL(15, 2) NULL;

ALTER TABLE dbo.[YeuCauDichVuGiuongBenhVien]
ADD [MucHuongBaoHiem] INT NULL;

ALTER TABLE dbo.[DonThuocThanhToanChiTiet]
ADD [DonGiaBaoHiem] DECIMAL(15, 2) NULL;

ALTER TABLE dbo.[DonThuocThanhToanChiTiet]
ADD [MucHuongBaoHiem] INT NULL;

ALTER TABLE dbo.[DuyetBaoHiemChiTiet]
ADD [DonGiaBaoHiem] DECIMAL(15, 2) NULL;

ALTER TABLE dbo.[DuyetBaoHiemChiTiet]
ADD [MucHuongBaoHiem] INT NULL;

Update CauHinh
Set [Value] = '0.6.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'
