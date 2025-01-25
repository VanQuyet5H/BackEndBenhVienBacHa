ALTER TABLE [TaiKhoanBenhNhanThu]
ADD 
	[LoaiNoiThu] int NULL;
GO

UPDATE A
SET
    A.LoaiNoiThu =	case when e.LoaiDonThuoc is not null AND e.LoaiDonThuoc = 2 then 2 else 1 end
FROM
    [TaiKhoanBenhNhanThu] AS A
    
	LEFT JOIN [TaiKhoanBenhNhanChi] AS C ON A.Id = C.TaiKhoanBenhNhanThuId
	LEFT JOIN [DonThuocThanhToanChiTiet] AS D ON D.Id = C.DonThuocThanhToanChiTietId
	LEFT JOIN [DonThuocThanhToan] AS E ON E.Id = D.DonThuocThanhToanId

GO

ALTER TABLE [TaiKhoanBenhNhanThu]
ALTER COLUMN [LoaiNoiThu] int NOT NULL;
GO

Update CauHinh
Set [Value] = '0.1.9' where [Name] = 'CauHinhHeThong.DatabaseVesion'