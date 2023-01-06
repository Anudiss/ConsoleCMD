USE [FileSystem]
GO
/****** Object:  Table [dbo].[Directory]    Script Date: 06.01.2023 16:58:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Directory](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Parent] [int] NULL,
 CONSTRAINT [PK_Directory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[File]    Script Date: 06.01.2023 16:58:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[File](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Extension] [varchar](3) NOT NULL,
	[Parent] [int] NOT NULL,
 CONSTRAINT [PK_File] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Directory] ON 

INSERT [dbo].[Directory] ([ID], [Name], [Parent]) VALUES (1, N'Root', NULL)
INSERT [dbo].[Directory] ([ID], [Name], [Parent]) VALUES (2, N'Folder1', 1)
INSERT [dbo].[Directory] ([ID], [Name], [Parent]) VALUES (3, N'Folder2', 2)
INSERT [dbo].[Directory] ([ID], [Name], [Parent]) VALUES (4, N'Folder3', 3)
INSERT [dbo].[Directory] ([ID], [Name], [Parent]) VALUES (5, N'Folder4', 4)
INSERT [dbo].[Directory] ([ID], [Name], [Parent]) VALUES (6, N'Folder5', 5)
INSERT [dbo].[Directory] ([ID], [Name], [Parent]) VALUES (7, N'Folder6', 6)
INSERT [dbo].[Directory] ([ID], [Name], [Parent]) VALUES (8, N'Folder7', 7)
INSERT [dbo].[Directory] ([ID], [Name], [Parent]) VALUES (9, N'Folder8', 1)
INSERT [dbo].[Directory] ([ID], [Name], [Parent]) VALUES (10, N'Folder9', 9)
INSERT [dbo].[Directory] ([ID], [Name], [Parent]) VALUES (11, N'Folder10', 5)
INSERT [dbo].[Directory] ([ID], [Name], [Parent]) VALUES (12, N'Folder11', 11)
INSERT [dbo].[Directory] ([ID], [Name], [Parent]) VALUES (13, N'Folder12', 11)
SET IDENTITY_INSERT [dbo].[Directory] OFF
GO
ALTER TABLE [dbo].[Directory]  WITH CHECK ADD  CONSTRAINT [FK_Directory_Directory] FOREIGN KEY([Parent])
REFERENCES [dbo].[Directory] ([ID])
GO
ALTER TABLE [dbo].[Directory] CHECK CONSTRAINT [FK_Directory_Directory]
GO
ALTER TABLE [dbo].[File]  WITH CHECK ADD  CONSTRAINT [FK_File_Directory] FOREIGN KEY([Parent])
REFERENCES [dbo].[Directory] ([ID])
GO
ALTER TABLE [dbo].[File] CHECK CONSTRAINT [FK_File_Directory]
GO
