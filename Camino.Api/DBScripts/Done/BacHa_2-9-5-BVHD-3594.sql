alter table YeuCauDieuChuyenDuocPham add HienThiCaThuocHetHan bit null;

GO
Update dbo.CauHinh
Set [Value] = '2.9.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'