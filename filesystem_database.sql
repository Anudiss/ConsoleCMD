USE [FileSystem]
GO
/****** Object:  Table [dbo].[Directory]    Script Date: 06.01.2023 17:29:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Directory](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Parent] [int] NULL,
	[Icon_id] [int] NULL,
 CONSTRAINT [PK_Directory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[File]    Script Date: 06.01.2023 17:29:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[File](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Extension] [varchar](3) NOT NULL,
	[Parent] [int] NOT NULL,
	[Icon_id] [int] NULL,
 CONSTRAINT [PK_File] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Icon]    Script Date: 06.01.2023 17:29:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Icon](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Data] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_Icon] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Directory] ON 

INSERT [dbo].[Directory] ([ID], [Name], [Parent], [Icon_id]) VALUES (1, N'Root', NULL, NULL)
INSERT [dbo].[Directory] ([ID], [Name], [Parent], [Icon_id]) VALUES (2, N'Folder1', 1, NULL)
INSERT [dbo].[Directory] ([ID], [Name], [Parent], [Icon_id]) VALUES (3, N'Folder2', 2, NULL)
INSERT [dbo].[Directory] ([ID], [Name], [Parent], [Icon_id]) VALUES (4, N'Folder3', 3, NULL)
INSERT [dbo].[Directory] ([ID], [Name], [Parent], [Icon_id]) VALUES (5, N'Folder4', 4, NULL)
INSERT [dbo].[Directory] ([ID], [Name], [Parent], [Icon_id]) VALUES (6, N'Folder5', 5, NULL)
INSERT [dbo].[Directory] ([ID], [Name], [Parent], [Icon_id]) VALUES (7, N'Folder6', 6, NULL)
INSERT [dbo].[Directory] ([ID], [Name], [Parent], [Icon_id]) VALUES (8, N'Folder7', 7, NULL)
INSERT [dbo].[Directory] ([ID], [Name], [Parent], [Icon_id]) VALUES (9, N'Folder8', 1, NULL)
INSERT [dbo].[Directory] ([ID], [Name], [Parent], [Icon_id]) VALUES (10, N'Folder9', 9, NULL)
INSERT [dbo].[Directory] ([ID], [Name], [Parent], [Icon_id]) VALUES (11, N'Folder10', 5, NULL)
INSERT [dbo].[Directory] ([ID], [Name], [Parent], [Icon_id]) VALUES (12, N'Folder11', 11, NULL)
INSERT [dbo].[Directory] ([ID], [Name], [Parent], [Icon_id]) VALUES (13, N'Folder12', 11, NULL)
SET IDENTITY_INSERT [dbo].[Directory] OFF
GO
ALTER TABLE [dbo].[Directory]  WITH CHECK ADD  CONSTRAINT [FK_Directory_Directory] FOREIGN KEY([Parent])
REFERENCES [dbo].[Directory] ([ID])
GO
ALTER TABLE [dbo].[Directory] CHECK CONSTRAINT [FK_Directory_Directory]
GO
ALTER TABLE [dbo].[Directory]  WITH CHECK ADD  CONSTRAINT [FK_Directory_Icon] FOREIGN KEY([Icon_id])
REFERENCES [dbo].[Icon] ([ID])
GO
ALTER TABLE [dbo].[Directory] CHECK CONSTRAINT [FK_Directory_Icon]
GO
ALTER TABLE [dbo].[File]  WITH CHECK ADD  CONSTRAINT [FK_File_Directory] FOREIGN KEY([Parent])
REFERENCES [dbo].[Directory] ([ID])
GO
ALTER TABLE [dbo].[File] CHECK CONSTRAINT [FK_File_Directory]
GO
ALTER TABLE [dbo].[File]  WITH CHECK ADD  CONSTRAINT [FK_File_Icon] FOREIGN KEY([Icon_id])
REFERENCES [dbo].[Icon] ([ID])
GO
ALTER TABLE [dbo].[File] CHECK CONSTRAINT [FK_File_Icon]
GO
