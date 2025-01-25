
IF (SELECT COUNT(1) FROM DuocPham WHERE SoDangKy = N'VD-33183-19') = 0
BEGIN
INSERT INTO [dbo].[DuocPham]
           ([Ten]
           ,[TenTiengAnh]
           ,[SoDangKy]
           ,[STTHoatChat]
           ,[MaHoatChat]
           ,[HoatChat]
           ,[LoaiThuocHoacHoatChat]
           ,[NhaSanXuat]
           ,[NuocSanXuat]
           ,[DuongDungId]
           ,[HamLuong]
           ,[QuyCach]
           ,[TieuChuan]
           ,[DangBaoChe]
           ,[DonViTinhId]
           ,[DonViTinhThamKhao]
           ,[HuongDan]
           ,[MoTa]
           ,[ChiDinh]
           ,[ChongChiDinh]
           ,[LieuLuongCachDung]
           ,[TacDungPhu]
           ,[ChuYDePhong]
           ,[IsDisabled]
           ,[CreatedById]
           ,[LastUserId]
           ,[LastTime]
           ,[CreatedOn]
           ,[LaThucPhamChucNang]
           ,[TheTich]
           ,[HeSoDinhMucDonViTinh]
           ,[LaThuocHuongThanGayNghien])
     VALUES
           (N'Fabamox 1000 DT',NULL ,N'VD-33183-19' ,NULL,N'40.154' ,N'Amoxicilin (dưới dạng Amoxcilin trihydrat) 1000 mg',1,N'Công ty cổ phần dược phẩm Trung ương I - Pharbaco',N'Việt Nam',1 ,N'1000 mg' ,N'Hộp 3 vỉ x 7 viên' ,N'TCCS',N'Viên nén phân tán',1,NULL,NULL,NULL ,NULL,NULL,NULL,NULL ,NULL,0 ,1,1,GETDATE(),GETDATE() ,0,NULL ,NULL,NULL)
END

GO
Update CauHinh
Set [Value] = '1.8.4' where [Name] = 'CauHinhHeThong.DatabaseVesion'