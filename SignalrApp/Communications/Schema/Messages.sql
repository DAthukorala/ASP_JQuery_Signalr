CREATE TABLE [dbo].[Messages](
	[UserId] [nvarchar](50) NULL,
	[OrganizationId] [nvarchar](50) NULL,
	[Title] [nvarchar](150) NULL,
	[Message] [nvarchar](max) NOT NULL,
	[CommunicationType] [int] NOT NULL,
	[IsPersist] [bit] NOT NULL,
	[IsRead] [bit] NOT NULL,
	[TimeStamp] [datetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Messages] ADD  CONSTRAINT [Default_Message_Timestamp]  DEFAULT (getdate()) FOR [TimeStamp]
GO

