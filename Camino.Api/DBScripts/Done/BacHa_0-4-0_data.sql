INSERT [User]([Avatar],[Password],[HoTen],[SoDienThoai],[Email],[SoChungMinhThu],[DiaChi],[NgaySinh],[GioiTinh],[IsActive],[PassCode],[ExpiredCodeDate],[IsDefault],[Region],[CreatedById],[LastUserId],[LastTime],[CreatedOn]) 
	SELECT A.[Avatar],A.[Password],A.[HoTen],A.[SoDienThoai],A.[Email],A.[SoChungMinhThu],A.[DiaChi],A.[NgaySinh],A.[GioiTinh],A.[IsActive],A.[PassCode],A.[ExpiredCodeDate],A.[IsDefault],A.[Region],A.[CreatedById],A.[LastUserId],A.[LastTime],A.[CreatedOn]
	FROM [User] A
	WHERE A.Id = 1

INSERT [NhanVien]([Id],[QuyenHan],[GhiChu],[CreatedById],[LastUserId],[LastTime],[CreatedOn],[HocHamHocViId],[PhamViHanhNgheId],[ChucDanhId],[MaChungChiHanhNghe],[VanBangChuyenMonId],[NgayCapChungChiHanhNghe],[NoiCapChungChiHanhNghe],[NgayKyHopDong],[NgayHetHopDong]) 
	SELECT (SELECT top 1 Id FROM [USER] order by Id desc),
			A.[QuyenHan],A.[GhiChu],A.[CreatedById],A.[LastUserId],A.[LastTime],A.[CreatedOn],A.[HocHamHocViId],A.[PhamViHanhNgheId],A.[ChucDanhId],A.[MaChungChiHanhNghe],A.[VanBangChuyenMonId],A.[NgayCapChungChiHanhNghe],A.[NoiCapChungChiHanhNghe],A.[NgayKyHopDong],A.[NgayHetHopDong]
	FROM [NhanVien] A
	WHERE A.Id = 1

INSERT [NhanVienRole]([NhanVienId],[RoleId],[CreatedById],[LastUserId],[LastTime],[CreatedOn])
	SELECT (SELECT top 1 Id FROM [USER] order by Id desc),
			A.[RoleId],A.[CreatedById],A.[LastUserId],A.[LastTime],A.[CreatedOn]
	FROM [NhanVienRole] A
	WHERE A.NhanVienId = 1

INSERT [KhoaPhongNhanVien]([NhanVienId],[KhoaPhongId],[CreatedById],[LastUserId],[LastTime],[CreatedOn])
	SELECT (SELECT top 1 Id FROM [USER] order by Id desc),
			A.[KhoaPhongId],A.[CreatedById],A.[LastUserId],A.[LastTime],A.[CreatedOn]
	FROM [KhoaPhongNhanVien] A
	WHERE A.NhanVienId = 1

UPDATE [User]
SET Avatar = NULL, HoTen = N'Hệ Thống', SoDienThoai = '0000000000', Email = NULL, SoChungMinhThu = NULL, DiaChi = NULL, IsDefault = 1
WHERE Id = 1

DELETE FROM [KhoaPhongNhanVien] WHERE NhanVienId = 1 AND KhoaPhongId != 14