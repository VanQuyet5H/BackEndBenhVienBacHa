
-- update data phòng bệnh viện cho những khoa phòng chưa có phòng
SELECT Id, Ten, Ma INTO #tblKhoaPhong
FROM KhoaPhong WHERE Id NOT IN (SELECT KhoaPhongId FROM PhongBenhVien)

DECLARE @total INT = NULL,
		@KhoaPhongId BIGINT = NULL,
		@TenKhoaPhong NVARCHAR(250) = NULL,
		@MaKhoaPhong NVARCHAR(50) = NULL,

		@IdCurrent BIGINT = NULL,
		@CreatedById int = 1,
		@LastUserId int = 1,
		@LastTime datetime = GETDATE(),
		@CreatedOn datetime = GETDATE()

SELECT @total = COUNT(1) FROM #tblKhoaPhong

SELECT TOP 1 @IdCurrent = Id FROM PhongBenhVien ORDER BY Id DESC

DBCC CHECKIDENT ('PhongBenhVien', RESEED, @IdCurrent)

WHILE(@total > 0)
BEGIN
	SELECT TOP 1 
		@KhoaPhongId = Id,
		@TenKhoaPhong = Ten,
		@MaKhoaPhong = Ma
	FROM #tblKhoaPhong

	INSERT INTO PhongBenhVien(KhoaPhongId, Ten, Ma, CreatedById, LastUserId, LastTime, CreatedOn)
	VALUES(@KhoaPhongId, @TenKhoaPhong, @MaKhoaPhong, @CreatedById, @LastUserId, @LastTime, @CreatedOn)

	DELETE TOP (1) FROM #tblKhoaPhong 
	SELECT @total = @total - 1,
		   @KhoaPhongId = NULL,
		   @TenKhoaPhong = NULL
END
DROP TABLE #tblKhoaPhong

-- update khoa phòng nhân viên cho nhân viên thuộc ban lãnh đạo

SELECT NhanVienId INTO #tblNhanVien 
FROM KhoaPhongNhanVien WHERE KhoaPhongId = (SELECT Id FROM KhoaPhong WHERE Ma = N'BLD')

SELECT Id INTO #tblKhoaPhongKhac 
FROM KhoaPhong WHERE  Ma <> N'BLD'

DECLARE @totalKhoaPhong INT = NULL,
		@totalNhanVien INT = NULL,
		@NhanVienId BIGINT = NULL

SELECT @totalKhoaPhong = COUNT(1) FROM #tblKhoaPhongKhac
SELECT @totalNhanVien = COUNT(1) FROM #tblNhanVien
SELECT TOP 1 @IdCurrent = Id FROM KhoaPhongNhanVien ORDER BY Id DESC
DBCC CHECKIDENT ('KhoaPhongNhanVien', RESEED, @IdCurrent)

WHILE(@totalNhanVien > 0)
BEGIN
	SELECT TOP 1
		@NhanVienId = NhanVienId
		FROM #tblNhanVien
	
	WHILE (@totalKhoaPhong > 0)
	BEGIN
		SELECT TOP 1
			@KhoaPhongId = Id
		FROM #tblKhoaPhongKhac

		IF (SELECT COUNT(1) FROM KhoaPhongNhanVien WHERE NhanVienId = @NhanVienId AND KhoaPhongId = @KhoaPhongId) = 0
		BEGIN
			INSERT INTO KhoaPhongNhanVien(KhoaPhongId, NhanVienId, CreatedById, LastUserId, LastTime, CreatedOn)
			VALUES(@KhoaPhongId, @NhanVienId, @CreatedById, @LastUserId, @LastTime, @CreatedOn)
		END

		DELETE TOP (1) FROM #tblKhoaPhongKhac 
		SELECT @totalKhoaPhong = @totalKhoaPhong - 1,
			   @KhoaPhongId = NULL
	END

	DELETE TOP (1) FROM #tblNhanVien 
	SELECT @totalNhanVien = @totalNhanVien - 1,
		   @NhanVienId = NULL
END

DROP TABLE #tblNhanVien
DROP TABLE #tblKhoaPhongKhac

