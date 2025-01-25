CREATE FULLTEXT CATALOG InputStringStored;

  CREATE UNIQUE INDEX inputStringStored ON InputStringStored([ID]); 

  CREATE FULLTEXT INDEX ON dbo.InputStringStored
(  
	[Value]  
        Language 1066
)  
KEY INDEX inputStringStored ON InputStringStored --Unique index  
GO 


Update dbo.CauHinh
Set [Value] = '0.5.6' where [Name] = 'CauHinhHeThong.DatabaseVesion'