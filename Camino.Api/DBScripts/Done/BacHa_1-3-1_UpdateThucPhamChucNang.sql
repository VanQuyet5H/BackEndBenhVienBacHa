alter table DuocPham add LaThucPhamChucNang bit NULL;

Update dbo.CauHinh
Set [Value] = '1.3.1' where [Name] = 'CauHinhHeThong.DatabaseVesion'