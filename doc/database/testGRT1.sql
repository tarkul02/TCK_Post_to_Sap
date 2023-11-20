USE [Barcode1]
GO

INSERT INTO [dbo].[testGRTR]
           ([MatNo]
           ,[CustID]
           ,[FacNo]
           ,[Plant]
           ,[SLoc]
           ,[MvmntType]
           ,[PostDate]
           ,[PostTime]
           ,[QRQty]
           ,[HeaderText]
           ,[Action])
     VALUES
           (<MatNo, varchar(18),>
           ,<CustID, varchar(12),>
           ,<FacNo, varchar(2),>
           ,<Plant, varchar(4),>
           ,<SLoc, varchar(4),>
           ,<MvmntType, int,>
           ,<PostDate, varchar(8),>
           ,<PostTime, varchar(5),>
           ,<QRQty, int,>
           ,<HeaderText, varchar(8000),>
           ,<Action, int,>)
GO

