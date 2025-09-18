SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[UserRoleID] [uniqueidentifier] NOT NULL,
	[RoleName] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserRoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 18/09/2025 9:34:34 pm ******/
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
	[UserRoleId] [uniqueidentifier] NOT NULL,
	[Latitude] [decimal](9, 6) NULL,
	[Longitude] [decimal](9, 6) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[UserRole] ([UserRoleID], [RoleName], [CreatedDate]) VALUES (N'c61e32ce-7775-491e-9859-3780f5dcdba5', N'Admin', CAST(N'2025-09-17T20:32:57.553' AS DateTime))
GO
INSERT [dbo].[UserRole] ([UserRoleID], [RoleName], [CreatedDate]) VALUES (N'2c21d99f-dec7-4876-8614-db8a6b1b1f38', N'Home Owner', CAST(N'2025-09-17T20:32:57.553' AS DateTime))
GO
INSERT [dbo].[UserRole] ([UserRoleID], [RoleName], [CreatedDate]) VALUES (N'f9b59208-5e27-4f5f-82b2-f4fdf744df13', N'Garbage Collector', CAST(N'2025-09-17T20:32:57.553' AS DateTime))
GO
INSERT [dbo].[Users] ([UserID], [FirstName], [LastName], [LotNumber], [BlockNumber], [StreetName], [ContactNumber], [MoveInDate], [Email], [Password], [CreatedDate], [UserRoleId], [Latitude], [Longitude]) VALUES (N'dfbb2211-1cf7-45c0-b322-1ae3afed2be8', N'Test', N'Test', N'2', N'6', N'Test', N'+639953637231', CAST(N'2025-09-18T00:00:00.000' AS DateTime), N'j@gmail.com', N'098F6BCD4621D373CADE4E832627B4F6', CAST(N'2025-09-18T21:31:25.053' AS DateTime), N'2c21d99f-dec7-4876-8614-db8a6b1b1f38', CAST(14.069444 AS Decimal(9, 6)), CAST(121.143039 AS Decimal(9, 6)))
GO
INSERT [dbo].[Users] ([UserID], [FirstName], [LastName], [LotNumber], [BlockNumber], [StreetName], [ContactNumber], [MoveInDate], [Email], [Password], [CreatedDate], [UserRoleId], [Latitude], [Longitude]) VALUES (N'396ff8e2-349d-49ff-b9ac-27e67b5bbc06', N'Jomiel', N'Enriquez', N'2', N'6', NULL, N'+639953637231', CAST(N'2025-09-16T00:00:00.000' AS DateTime), N'enriquez.jliquigan@gmail.com', N'21232F297A57A5A743894A0E4A801FC3', CAST(N'2025-09-16T22:05:27.060' AS DateTime), N'c61e32ce-7775-491e-9859-3780f5dcdba5', CAST(14.073443 AS Decimal(9, 6)), CAST(121.148041 AS Decimal(9, 6)))
GO
INSERT [dbo].[Users] ([UserID], [FirstName], [LastName], [LotNumber], [BlockNumber], [StreetName], [ContactNumber], [MoveInDate], [Email], [Password], [CreatedDate], [UserRoleId], [Latitude], [Longitude]) VALUES (N'11669577-0316-4c87-b23c-65e080806b93', N'Test', N'Test', N'3', N'7', N'Test', N'+639953637231', CAST(N'2025-09-18T00:00:00.000' AS DateTime), N'jom72056@gmail.com', N'21232F297A57A5A743894A0E4A801FC3', CAST(N'2025-09-18T20:51:22.457' AS DateTime), N'2c21d99f-dec7-4876-8614-db8a6b1b1f38', CAST(14.074375 AS Decimal(9, 6)), CAST(121.147202 AS Decimal(9, 6)))
GO
ALTER TABLE [dbo].[UserRole] ADD  DEFAULT (newid()) FOR [UserRoleID]
GO
ALTER TABLE [dbo].[UserRole] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (newid()) FOR [UserID]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_UserRoleID] FOREIGN KEY([UserRoleId])
REFERENCES [dbo].[UserRole] ([UserRoleID])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_UserRoleID]
GO
