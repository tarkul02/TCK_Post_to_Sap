USE [Barcode1]
GO

/****** Object:  Table [dbo].[T_LogDatavalidate_GR_to_Sap]    Script Date: 11/15/2023 10:19:48 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[T_LogDatavalidate_GR_to_Sap](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[MatNo] [varchar](18) NULL,
	[CustID] [varchar](12) NULL,
	[FacNo] [varchar](2) NULL,
	[Plant] [varchar](4) NULL,
	[SLoc] [varchar](4) NULL,
	[MvmntType] [int] NULL,
	[PostDate] [varchar](8) NULL,
	[PostTime] [varchar](5) NULL,
	[QRQty] [int] NULL,
	[HeaderText] [varchar](8000) NULL,
	[Action] [int] NULL,
	[Type] [varchar](2) NULL,
	[CreateDate] [datetime] NULL,
	[ValidateMessage] [varchar](255) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

