INSERT INTO [dbo].[CauHinh]([Name],[DataType],[Description],[Value],[CreatedById],[LastUserId],[LastTime],[CreatedOn])
     VALUES (N'CauHinhBaoCao.TinhHaNoi',2,N'Tỉnh Hà Nội Id',0,1,1,GETDATE(),GETDATE()),
			(N'CauHinhBaoCao.NhomKhamCapCuu',3,N'Nhóm Id các dịch vụ khám cấp cứu (cách nhau bằng dấu ;)',0,1,1,GETDATE(),GETDATE()),
			(N'CauHinhBaoCao.NhomKhamNoi',3,N'Nhóm Id các dịch vụ khám nội (cách nhau bằng dấu ;)',0,1,1,GETDATE(),GETDATE()),
			(N'CauHinhBaoCao.NhomKhamNhi',3,N'Nhóm Id các dịch vụ khám nhi (cách nhau bằng dấu ;)',0,1,1,GETDATE(),GETDATE()),
			(N'CauHinhBaoCao.NhomKhamTMH',3,N'Nhóm Id các dịch vụ khám TMH (cách nhau bằng dấu ;)',0,1,1,GETDATE(),GETDATE()),
			(N'CauHinhBaoCao.NhomKhamRHM',3,N'Nhóm Id các dịch vụ khám RHM (cách nhau bằng dấu ;)',0,1,1,GETDATE(),GETDATE()),
			(N'CauHinhBaoCao.NhomKhamMat',3,N'Nhóm Id các dịch vụ khám cấp cứu (cách nhau bằng dấu ;)',0,1,1,GETDATE(),GETDATE()),
			(N'CauHinhBaoCao.NhomKhamNgoai',3,N'Nhóm Id các dịch vụ khám ngoại (cách nhau bằng dấu ;)',0,1,1,GETDATE(),GETDATE()),
			(N'CauHinhBaoCao.NhomKhamDaLieu',3,N'Nhóm Id các dịch vụ khám da liễu (cách nhau bằng dấu ;)',0,1,1,GETDATE(),GETDATE()),
			(N'CauHinhBaoCao.NhomKhamPhuSan',3,N'Nhóm Id các dịch vụ khám phụ sản (cách nhau bằng dấu ;)',0,1,1,GETDATE(),GETDATE()),
			(N'CauHinhBaoCao.NhomKhamThamMy',3,N'Nhóm Id các dịch vụ khám thẩm mỹ (cách nhau bằng dấu ;)',0,1,1,GETDATE(),GETDATE())
GO


DECLARE @tinhHaNoiId BIGINT = NULL,
		@NhomKhamCapCuuId BIGINT = NULL,
		@NhomKhamNoiId BIGINT = NULL,
		@NhomKhamNhiId BIGINT = NULL,
		@NhomKhamTMHId BIGINT = NULL,
		@NhomKhamRHMId BIGINT = NULL,
		@NhomKhamMatId BIGINT = NULL,
		@NhomKhamNgoaiId BIGINT = NULL,
		@NhomKhamDaLieuId BIGINT = NULL,
		@NhomKhamPhuSanId BIGINT = NULL,
		@NhomKhamThamMyId BIGINT = NULL
SELECT TOP 1 @tinhHaNoiId = id FROM DonViHanhChinh WHERE Ma = N'01'
Update CauHinh SET Value = @tinhHaNoiId WHERE Name = N'CauHinhBaoCao.TinhHaNoi'
SELECT TOP 1 @NhomKhamCapCuuId = id FROM DichVuKhamBenhBenhVien WHERE Ten = N'Khám Cấp cứu'
Update CauHinh SET Value = CAST(@NhomKhamCapCuuId as NVARCHAR) + N';' WHERE Name = N'CauHinhBaoCao.NhomKhamCapCuu'
SELECT TOP 1 @NhomKhamNoiId = id FROM DichVuKhamBenhBenhVien WHERE Ten = N'Khám Nội'
Update CauHinh SET Value = CAST(@NhomKhamNoiId as NVARCHAR) + N';' WHERE Name = N'CauHinhBaoCao.NhomKhamNoi'
SELECT TOP 1 @NhomKhamNhiId = id FROM DichVuKhamBenhBenhVien WHERE Ten = N'Khám Nhi'
Update CauHinh SET Value = CAST(@NhomKhamNhiId as NVARCHAR) + N';' WHERE Name = N'CauHinhBaoCao.NhomKhamNhi'
SELECT TOP 1 @NhomKhamTMHId = id FROM DichVuKhamBenhBenhVien WHERE Ten = N'Khám Tai Mũi Họng'
Update CauHinh SET Value = CAST(@NhomKhamTMHId as NVARCHAR) + N';' WHERE Name = N'CauHinhBaoCao.NhomKhamTMH'
SELECT TOP 1 @NhomKhamRHMId = id FROM DichVuKhamBenhBenhVien WHERE Ten = N'Khám Răng Hàm Mặt'
Update CauHinh SET Value = CAST(@NhomKhamRHMId as NVARCHAR) + N';' WHERE Name = N'CauHinhBaoCao.NhomKhamRHM'
SELECT TOP 1 @NhomKhamMatId = id FROM DichVuKhamBenhBenhVien WHERE Ten = N'Khám Mắt'
Update CauHinh SET Value = CAST(@NhomKhamMatId as NVARCHAR) + N';' WHERE Name = N'CauHinhBaoCao.NhomKhamMat'
SELECT TOP 1 @NhomKhamNgoaiId = id FROM DichVuKhamBenhBenhVien WHERE Ten = N'Khám Ngoại'
Update CauHinh SET Value = CAST(@NhomKhamNgoaiId as NVARCHAR) + N';' WHERE Name = N'CauHinhBaoCao.NhomKhamNgoai'
SELECT TOP 1 @NhomKhamDaLieuId = id FROM DichVuKhamBenhBenhVien WHERE Ten = N'Khám Da liễu'
Update CauHinh SET Value = CAST(@NhomKhamDaLieuId as NVARCHAR) + N';' WHERE Name = N'CauHinhBaoCao.NhomKhamDaLieu'
SELECT TOP 1 @NhomKhamPhuSanId = id FROM DichVuKhamBenhBenhVien WHERE Ten = N'Khám Phụ Sản'
Update CauHinh SET Value = CAST(@NhomKhamPhuSanId as NVARCHAR) + N';' WHERE Name = N'CauHinhBaoCao.NhomKhamPhuSan'
SELECT TOP 1 @NhomKhamThamMyId = id FROM DichVuKhamBenhBenhVien WHERE Ten = N'Khám thẩm mỹ'
Update CauHinh SET Value = CAST(@NhomKhamThamMyId as NVARCHAR) + N';' WHERE Name = N'CauHinhBaoCao.NhomKhamThamMy'
Go

Update dbo.CauHinh
Set [Value] = '3.2.7' where [Name] = 'CauHinhHeThong.DatabaseVesion'