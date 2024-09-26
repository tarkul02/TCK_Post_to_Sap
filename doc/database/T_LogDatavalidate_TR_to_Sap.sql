USE [Barcode1]
GO

/****** Object:  Table [dbo].[T_LogDatavalidate_TR_to_Sap]    Script Date: 11/15/2023 10:20:07 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[T_LogDatavalidate_TR_to_Sap](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SlipNo] [nvarchar](25) NULL,
	[Type] [varchar](2) NULL,
	[CreateDate] [datetime] NULL,
	[Datatype] [varchar](2) NULL,
	[ValidateMessage] [varchar](255) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

