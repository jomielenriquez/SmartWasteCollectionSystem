IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Announcements')
BEGIN
	CREATE TABLE Announcements (
		AnnouncementId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), -- Primary key (GUID)
		Title NVARCHAR(200) NOT NULL,                             -- Short title
		Message NVARCHAR(MAX) NOT NULL,                           -- Full announcement content
		StartDate DATE NOT NULL,                                  -- When it becomes visible
		EndDate DATE NOT NULL,                                        -- Optional: when it expires
		IsActive BIT NOT NULL DEFAULT 1,                          -- Active/Inactive flag
		CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
	);
END

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Emails')
BEGIN
	CREATE TABLE Emails (
		EmailId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
		Title NVARCHAR(200) NOT NULL,                             
		Message NVARCHAR(MAX) NOT NULL,
		Recipients NVARCHAR(MAX) NOT NULL,
		IsSent BIT NOT NULL DEFAULT 0,
		SentDate DATETIME NULL,
		CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
	)
END