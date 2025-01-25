ALTER TABLE [NoiTruPhieuDieuTriChiTietYLenh] DROP CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietYLenh_NoiTruChiDinhDuocPham];
ALTER TABLE [NoiTruPhieuDieuTriChiTietYLenh] DROP CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietYLenh_YeuCauDichVuKyThuat];
ALTER TABLE [NoiTruPhieuDieuTriChiTietYLenh] DROP CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietYLenh_YeuCauTruyenMau];
ALTER TABLE [NoiTruPhieuDieuTriChiTietYLenh] DROP CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietYLenh_YeuCauVatTuBenhVien];

ALTER TABLE [NoiTruPhieuDieuTriChiTietYLenh]
ADD CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietYLenh_NoiTruChiDinhDuocPham] FOREIGN KEY ([NoiTruChiDinhDuocPhamId]) REFERENCES [dbo].[NoiTruChiDinhDuocPham] ([Id]) ON DELETE CASCADE;

ALTER TABLE [NoiTruPhieuDieuTriChiTietYLenh]
ADD CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietYLenh_YeuCauDichVuKyThuat] FOREIGN KEY ([YeuCauDichVuKyThuatId]) REFERENCES [dbo].[YeuCauDichVuKyThuat] ([Id]) ON DELETE CASCADE;

ALTER TABLE [NoiTruPhieuDieuTriChiTietYLenh]
ADD CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietYLenh_YeuCauTruyenMau] FOREIGN KEY ([YeuCauTruyenMauId]) REFERENCES [dbo].[YeuCauTruyenMau] ([Id]) ON DELETE CASCADE;

ALTER TABLE [NoiTruPhieuDieuTriChiTietYLenh]
ADD CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietYLenh_YeuCauVatTuBenhVien] FOREIGN KEY ([YeuCauVatTuBenhVienId]) REFERENCES [dbo].[YeuCauVatTuBenhVien] ([Id]) ON DELETE CASCADE;
GO
UPDATE CauHinh
Set [Value] = '1.9.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'