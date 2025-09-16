SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[LotNumber] [nvarchar](50) NOT NULL,
	[BlockNumber] [nvarchar](50) NOT NULL,
	[StreetName] [nvarchar](50) NULL,
	[ContactNumber] [nvarchar](50) NOT NULL,
	[MoveInDate] [datetime] NOT NULL,
	[Email] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[Users] ([UserID], [FirstName], [LastName], [LotNumber], [BlockNumber], [StreetName], [ContactNumber], [MoveInDate], [Email], [Password], [CreatedDate]) VALUES (N'396ff8e2-349d-49ff-b9ac-27e67b5bbc06', N'Jomiel', N'Enriquez', N'2', N'6', NULL, N'09953637231', CAST(N'2025-09-16T22:05:27.060' AS DateTime), N'enriquez.jliquigan@gmail.com', N'test', CAST(N'2025-09-16T22:05:27.060' AS DateTime))
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (newid()) FOR [UserID]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
USE [master]
GO
ALTER DATABASE [SmartWasteDB] SET  READ_WRITE 
GO
