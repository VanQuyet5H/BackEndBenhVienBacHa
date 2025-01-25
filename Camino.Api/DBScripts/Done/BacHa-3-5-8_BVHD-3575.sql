ALTER TABLE YeuCauKhamBenh
ADD LaChiDinhTuNoiTru BIT NULL
GO
ALTER TABLE NoiTruBenhAn ADD ThoiDiemTongHopYLenhKhamBenh datetime null;
GO
ALTER TABLE NoiTruPhieuDieuTriChiTietYLenh ADD YeuCauKhamBenhId BIGINT NULL;
ALTER TABLE [dbo].[NoiTruPhieuDieuTriChiTietYLenh]  WITH CHECK ADD  CONSTRAINT [FK_NoiTruPhieuDieuTriChiTietYLenh_YeuCauKhamBenh] FOREIGN KEY(YeuCauKhamBenhId) REFERENCES [dbo].[YeuCauKhamBenh] ([Id]);

Go
Update dbo.CauHinh
Set [Value] = '3.5.8' where [Name] = 'CauHinhHeThong.DatabaseVesion'