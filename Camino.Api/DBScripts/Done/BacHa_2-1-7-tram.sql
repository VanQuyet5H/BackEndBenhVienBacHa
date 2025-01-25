alter table YeuCauNhapKhoDuocPhamChiTiet add ThanhTienTruocVat decimal(15,2) null;
alter table YeuCauNhapKhoDuocPhamChiTiet add ThanhTienSauVat decimal(15,2) null;
alter table YeuCauNhapKhoVatTuChiTiet add ThanhTienTruocVat decimal(15,2) null;
alter table YeuCauNhapKhoVatTuChiTiet add ThanhTienSauVat decimal(15,2) null;
update YeuCauNhapKhoDuocPhamChiTiet set ThanhTienTruocVat=SoLuongNhap*DonGiaNhap,ThanhTienSauVat=(SoLuongNhap*DonGiaNhap)*(isnull(VAT,0)/100.0);
update YeuCauNhapKhoVatTuChiTiet set ThanhTienTruocVat=SoLuongNhap*DonGiaNhap,ThanhTienSauVat=(SoLuongNhap*DonGiaNhap)*(isnull(VAT,0)/100.0);
alter table YeuCauNhapKhoDuocPhamChiTiet alter column ThanhTienTruocVat decimal(15,2) not null;
alter table YeuCauNhapKhoDuocPhamChiTiet alter column ThanhTienSauVat decimal(15,2) not null;
alter table YeuCauNhapKhoVatTuChiTiet alter column ThanhTienTruocVat decimal(15,2) not null;
alter table YeuCauNhapKhoVatTuChiTiet alter column ThanhTienSauVat decimal(15,2) not null;

alter table YeuCauTiepNhanTheBHYT alter column DiaChi nvarchar(500) not null;
alter table YeuCauTiepNhan alter column BHYTDiaChi nvarchar(500) null;
alter table YeuCauTiepNhan alter column BHYTThe2DiaChi nvarchar(500) null;
alter table BenhNhan alter column BHYTDiaChi nvarchar(500) null;
alter table YeuCauTiepNhanLichSuChuyenDoiTuong alter column DiaChi nvarchar(500) null;

GO
Update dbo.CauHinh
Set [Value] = '2.1.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'
