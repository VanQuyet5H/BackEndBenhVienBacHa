ALTER TABLE NoiTruBenhAn ADD ThoiDiemTongHopYLenh datetime null;
ALTER TABLE NoiTruPhieuDieuTriChiTietYLenh DROP FK_NoiTruPhieuDieuTriChiTietYLenh_YeuCauDuocPhamBenhVien;
ALTER TABLE NoiTruPhieuDieuTriChiTietYLenh DROP COLUMN YeuCauDuocPhamBenhVienId;
ALTER TABLE NoiTruPhieuDieuTriChiTietYLenh ADD NoiTruChiDinhDuocPhamId bigint null;
ALTER TABLE NoiTruPhieuDieuTriChiTietYLenh ADD  CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietYLenh_NoiTruChiDinhDuocPham] FOREIGN KEY(NoiTruChiDinhDuocPhamId) REFERENCES [dbo].[NoiTruChiDinhDuocPham] ([Id])
GO
Update CauHinh
Set [Value] = '1.6.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'