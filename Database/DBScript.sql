
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DayOfWeek](
	[DayOfWeekID] [uniqueidentifier] NOT NULL,
	[Day] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[DayOfWeekID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FrequencyType]    Script Date: 20/09/2025 8:18:32 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FrequencyType](
	[FrequencyTypeID] [uniqueidentifier] NOT NULL,
	[FrequencyName] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[FrequencyTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GarbageCollectionSchedule]    Script Date: 20/09/2025 8:18:32 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GarbageCollectionSchedule](
	[GarbageCollectionScheduleID] [uniqueidentifier] NOT NULL,
	[DayOfWeekID] [uniqueidentifier] NOT NULL,
	[FrequencyTypeID] [uniqueidentifier] NOT NULL,
	[CollectionTime] [time](7) NOT NULL,
	[EffectiveFrom] [date] NULL,
	[EffectiveTo] [date] NULL,
	[IsActive] [bit] NOT NULL,
	[Notes] [nvarchar](500) NULL,
	[CreatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[GarbageCollectionScheduleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MonthlyDues]    Script Date: 20/09/2025 8:18:32 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MonthlyDues](
	[MonthlyDueID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[Amount] [decimal](10, 2) NOT NULL,
	[DueDate] [datetime] NOT NULL,
	[IsPaid] [bit] NOT NULL,
	[PaidDate] [datetime] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[StarDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MonthlyDueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 20/09/2025 8:18:32 pm ******/
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
/****** Object:  Table [dbo].[Users]    Script Date: 20/09/2025 8:18:32 pm ******/
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
INSERT [dbo].[DayOfWeek] ([DayOfWeekID], [Day]) VALUES (N'0d52c731-5f63-4246-abd2-264264f83945', N'Tuesday')
GO
INSERT [dbo].[DayOfWeek] ([DayOfWeekID], [Day]) VALUES (N'b3a10a12-f96d-4fea-b31b-4c42f52f1337', N'Wednesday')
GO
INSERT [dbo].[DayOfWeek] ([DayOfWeekID], [Day]) VALUES (N'bb036136-5462-49de-a610-60a6f9daf58e', N'Sunday')
GO
INSERT [dbo].[DayOfWeek] ([DayOfWeekID], [Day]) VALUES (N'28fc4e2b-8c0d-462f-ac8a-b7261d7a57a3', N'Friday')
GO
INSERT [dbo].[DayOfWeek] ([DayOfWeekID], [Day]) VALUES (N'b0dafb73-5e4e-47af-a369-d0e6f76da5d4', N'Thursday')
GO
INSERT [dbo].[DayOfWeek] ([DayOfWeekID], [Day]) VALUES (N'a310db8c-b3cf-4b2a-b9ee-d7aaf6ed7841', N'Saturday')
GO
INSERT [dbo].[DayOfWeek] ([DayOfWeekID], [Day]) VALUES (N'a9ed7555-37f6-4c52-b239-eb8356a12500', N'Monday')
GO
INSERT [dbo].[FrequencyType] ([FrequencyTypeID], [FrequencyName]) VALUES (N'2d043b87-eea9-42a9-a9c8-7cd69e500026', N'Daily')
GO
INSERT [dbo].[FrequencyType] ([FrequencyTypeID], [FrequencyName]) VALUES (N'48ef4af4-d1d8-4b30-a9ca-8f17e02a5e78', N'Weekly')
GO
INSERT [dbo].[FrequencyType] ([FrequencyTypeID], [FrequencyName]) VALUES (N'8c598bb2-14ae-405a-b907-bcdd544af28e', N'Monthly')
GO
INSERT [dbo].[GarbageCollectionSchedule] ([GarbageCollectionScheduleID], [DayOfWeekID], [FrequencyTypeID], [CollectionTime], [EffectiveFrom], [EffectiveTo], [IsActive], [Notes], [CreatedDate]) VALUES (N'12a00426-25ab-45fd-93bc-466b0809eeef', N'0d52c731-5f63-4246-abd2-264264f83945', N'2d043b87-eea9-42a9-a9c8-7cd69e500026', CAST(N'00:00:00' AS Time), CAST(N'2025-09-20' AS Date), CAST(N'2025-09-20' AS Date), 0, N' Test', CAST(N'2025-09-20T12:55:33.890' AS DateTime))
GO
INSERT [dbo].[MonthlyDues] ([MonthlyDueID], [UserID], [Amount], [DueDate], [IsPaid], [PaidDate], [CreatedDate], [StarDate], [EndDate]) VALUES (N'f7eb258b-09a1-4e84-899f-dc61e0873820', N'dfbb2211-1cf7-45c0-b322-1ae3afed2be8', CAST(1299.00 AS Decimal(10, 2)), CAST(N'2025-09-30T00:00:00.000' AS DateTime), 1, NULL, CAST(N'2025-09-19T21:27:28.303' AS DateTime), CAST(N'2025-09-01T00:00:00.000' AS DateTime), CAST(N'2025-09-30T00:00:00.000' AS DateTime))
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
INSERT [dbo].[Users] ([UserID], [FirstName], [LastName], [LotNumber], [BlockNumber], [StreetName], [ContactNumber], [MoveInDate], [Email], [Password], [CreatedDate], [UserRoleId], [Latitude], [Longitude]) VALUES (N'11669577-0316-4c87-b23c-65e080806b93', N'Jomiel', N'Enriquez', N'3', N'7', N'Test', N'+639953637231', CAST(N'2025-09-18T00:00:00.000' AS DateTime), N'jom72056@gmail.com', N'21232F297A57A5A743894A0E4A801FC3', CAST(N'2025-09-18T20:51:22.457' AS DateTime), N'2c21d99f-dec7-4876-8614-db8a6b1b1f38', CAST(14.074375 AS Decimal(9, 6)), CAST(121.147202 AS Decimal(9, 6)))
GO
INSERT [dbo].[Users] ([UserID], [FirstName], [LastName], [LotNumber], [BlockNumber], [StreetName], [ContactNumber], [MoveInDate], [Email], [Password], [CreatedDate], [UserRoleId], [Latitude], [Longitude]) VALUES (N'96f0b7e6-3639-4890-b0f3-a04a1587edc6', N'System', N'Admin', N'0', N'0', NULL, N'+631231231231', CAST(N'2025-09-18T00:00:00.000' AS DateTime), N'smartbin422@gmail.com', N'21232F297A57A5A743894A0E4A801FC3', CAST(N'2025-09-18T22:33:16.390' AS DateTime), N'c61e32ce-7775-491e-9859-3780f5dcdba5', CAST(14.069444 AS Decimal(9, 6)), CAST(121.143039 AS Decimal(9, 6)))
GO
ALTER TABLE [dbo].[DayOfWeek] ADD  DEFAULT (newid()) FOR [DayOfWeekID]
GO
ALTER TABLE [dbo].[FrequencyType] ADD  DEFAULT (newid()) FOR [FrequencyTypeID]
GO
ALTER TABLE [dbo].[GarbageCollectionSchedule] ADD  DEFAULT (newid()) FOR [GarbageCollectionScheduleID]
GO
ALTER TABLE [dbo].[GarbageCollectionSchedule] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[MonthlyDues] ADD  DEFAULT (newid()) FOR [MonthlyDueID]
GO
ALTER TABLE [dbo].[MonthlyDues] ADD  DEFAULT ((0)) FOR [IsPaid]
GO
ALTER TABLE [dbo].[MonthlyDues] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[MonthlyDues] ADD  DEFAULT (getdate()) FOR [StarDate]
GO
ALTER TABLE [dbo].[MonthlyDues] ADD  DEFAULT (getdate()) FOR [EndDate]
GO
ALTER TABLE [dbo].[UserRole] ADD  DEFAULT (newid()) FOR [UserRoleID]
GO
ALTER TABLE [dbo].[UserRole] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (newid()) FOR [UserID]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[GarbageCollectionSchedule]  WITH CHECK ADD  CONSTRAINT [FK_GarbageCollectionSchedule_DayOfWeekId] FOREIGN KEY([DayOfWeekID])
REFERENCES [dbo].[DayOfWeek] ([DayOfWeekID])
GO
ALTER TABLE [dbo].[GarbageCollectionSchedule] CHECK CONSTRAINT [FK_GarbageCollectionSchedule_DayOfWeekId]
GO
ALTER TABLE [dbo].[GarbageCollectionSchedule]  WITH CHECK ADD  CONSTRAINT [FK_GarbageCollectionSchedule_FrequencyTypeID] FOREIGN KEY([FrequencyTypeID])
REFERENCES [dbo].[FrequencyType] ([FrequencyTypeID])
GO
ALTER TABLE [dbo].[GarbageCollectionSchedule] CHECK CONSTRAINT [FK_GarbageCollectionSchedule_FrequencyTypeID]
GO
ALTER TABLE [dbo].[MonthlyDues]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_UserRoleID] FOREIGN KEY([UserRoleId])
REFERENCES [dbo].[UserRole] ([UserRoleID])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_UserRoleID]

