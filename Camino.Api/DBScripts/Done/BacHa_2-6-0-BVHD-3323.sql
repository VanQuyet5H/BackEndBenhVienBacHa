ALTER TABLE ChuongTrinhGoiDichVu
ADD 
	CongTyBaoHiemTuNhanId [bigint] NULL,
	CONSTRAINT [FK_ChuongTrinhGoiDichVu_CongTyBaoHiemTuNhan] FOREIGN KEY([CongTyBaoHiemTuNhanId]) REFERENCES [dbo].[CongTyBaoHiemTuNhan] ([Id])

GO
Update dbo.CauHinh
Set [Value] = '2.6.0' where [Name] = 'CauHinhHeThong.DatabaseVesion'