
ALTER TABLE YeuCauKhamBenh
ADD ICDChinhNghiHuongBHYT [bigint] NULL
GO

ALTER TABLE YeuCauKhamBenh
ADD TenICDChinhNghiHuongBHYT NVARCHAR(2000) NULL
GO

ALTER TABLE YeuCauKhamBenh
ADD PhuongPhapDieuTriNghiHuongBHYT  NVARCHAR(2000) NULL
GO

INSERT INTO [dbo].[CauHinh]([Name],[DataType],[Description],[Value],[CreatedById],[LastUserId],[LastTime],[CreatedOn])
     VALUES (N'CauHinhKhamBenh.NghiHuongBHYT',2,N'Ngày Nghỉ Hưởng Bảo Hiểm Xã Hội Bệnh Báo vượt 180 Ngày',
	 N'[
	 {"ICDId":"11454","Ten":"A15"},
	 {"ICDId":"11455","Ten":"A15.0"},
	 {"ICDId":"11456","Ten":"A15.1"},
	 {"ICDId":"11457","Ten":"A15.2"},
	 {"ICDId":"11458","Ten":"A15.3"},
	 {"ICDId":"11459","Ten":"A15.4"},
	 {"ICDId":"11460","Ten":"A15.5"},
	 {"ICDId":"11461","Ten":"A15.6"},
	 {"ICDId":"11462","Ten":"A15.7"},
     {"ICDId":"11463","Ten":"A15.8"},
	 {"ICDId":"11464","Ten":"A15.9"},

	 {"ICDId":"11465","Ten":"A16"},
	 {"ICDId":"11466","Ten":"A16.0"},
	 {"ICDId":"11467","Ten":"A16.1"},
	 {"ICDId":"11468","Ten":"A16.2"},
	 {"ICDId":"11469","Ten":"A16.3"},
	 {"ICDId":"11470","Ten":"A16.4"},
	 {"ICDId":"11471","Ten":"A16.5"},
	 {"ICDId":"11472","Ten":"A16.7"},
	 {"ICDId":"11473","Ten":"A16.8"},
     {"ICDId":"11474","Ten":"A16.9"},
	 
	 {"ICDId":"11474","Ten":"A17†"},
	 {"ICDId":"11475","Ten":"A17.0†"},
	 {"ICDId":"11476","Ten":"A17.1†"},
	 {"ICDId":"11477","Ten":"A17.8†"},
	 {"ICDId":"11478","Ten":"A16.3"},
	 {"ICDId":"11479","Ten":"A17.9†"},

	 {"ICDId":"11480","Ten":"A18"},
	 {"ICDId":"11481","Ten":"A18.0†"},
	 {"ICDId":"11482","Ten":"A18.1"},
	 {"ICDId":"11483","Ten":"A18.2"},
	 {"ICDId":"11484","Ten":"A18.3"},
	 {"ICDId":"11485","Ten":"A18.4"},
	 {"ICDId":"11486","Ten":"A18.5"},
	 {"ICDId":"11487","Ten":"A18.6"},
	 {"ICDId":"11488","Ten":"A18.7"},
     {"ICDId":"11489","Ten":"A18.8"},

	 {"ICDId":"11490","Ten":"A19"},
	 {"ICDId":"11491","Ten":"A19.0"},
	 {"ICDId":"11492","Ten":"A19.1"},
	 {"ICDId":"11493","Ten":"A19.2"},
	 {"ICDId":"11494","Ten":"A19.8"},
	 {"ICDId":"11495","Ten":"A19.9"},
	 ]',1,1,GETDATE(),GETDATE())
GO


Update dbo.CauHinh
Set [Value] = '3.4.5' where [Name] = 'CauHinhHeThong.DatabaseVesion'
