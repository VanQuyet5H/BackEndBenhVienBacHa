ALTER TABLE DuocPhamBenhVienPhanNhom
ADD KyHieuPhanNhom NVARCHAR(1) NULL
GO

UPDATE DuocPhamBenhVienPhanNhom
SET KyHieuPhanNhom = CASE Ten WHen N'Tân dược' THEN N'T'
							  WHen N'Thuốc từ dược liệu' THEN N'T'
							  WHen N'Thuốc đông y' THEN N'T'
							  WHen N'Thực phẩm chức năng' THEN N'P'
							  WHen N'Thiết bị y tế' THEN N'Y'
							  WHen N'Vật tư y tế' THEN N'V'
							  WHen N'hóa chất' THEN N'H'
							  WHen N'sinh phẩm' THEN N'S'
							  WHen N'Vaccine' THEN N'A'
							  WHen N'Vaccin' THEN N'A'
							  WHen N'Mỹ phẩm' THEN N'M'
							  WHen N'sinh phẩm chẩn đoán' THEN N'C'
							  ELSE NULL END

Go
Update dbo.CauHinh
Set [Value] = '2.7.3' where [Name] = 'CauHinhHeThong.DatabaseVesion'
