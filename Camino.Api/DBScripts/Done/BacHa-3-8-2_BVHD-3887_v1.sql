update CauHinh set [Value] ='[{"DichVuKyThuatBenhVienId":1459,"CauHinhIn":{"InLogo":false,"InBarcode":false,"InTieuDe":true,"InHoVaTen":true,"InNamSinh":true,"InGioiTinh":true,"InDiaChi":true,"InBSChiDinh":false,"InNgayChiDinh":false,"InNoiChiDinh":false,"InSoBenhAn":false,"InChuanDoan":false,"InDienGiai":false,"InChiDinh":false,"InThanhNgang":false,"InKyThuat":false},"NhomDichVuBenhVienIds":[{"Id":230},{"Id":226},{"Id":227},{"Id":231}]}]'
where [Name]='CauHinhCDHA.CauHinhIn'

Update dbo.CauHinh
Set [Value] = '3.8.2' where [Name] = 'CauHinhHeThong.DatabaseVesion'