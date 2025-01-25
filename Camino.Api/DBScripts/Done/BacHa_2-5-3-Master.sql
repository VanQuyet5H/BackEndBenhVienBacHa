EXEC sp_RENAME 'NoiTruBenhAn.ThoiDiemTongHopYLenh', 'ThoiDiemTongHopYLenhDichVuKyThuat', 'COLUMN';
ALTER TABLE NoiTruBenhAn ADD ThoiDiemTongHopYLenhTruyenMau datetime null;
ALTER TABLE NoiTruBenhAn ADD ThoiDiemTongHopYLenhVatTu datetime null;
ALTER TABLE NoiTruBenhAn ADD ThoiDiemTongHopYLenhDuocPham datetime null;
GO
Update dbo.CauHinh
Set [Value] = '2.5.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'