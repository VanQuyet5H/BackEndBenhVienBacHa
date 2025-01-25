
alter table DuocPham add LaThuocHuongThanGayNghien bit null;

GO
Update CauHinh
Set [Value] = '1.7.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'