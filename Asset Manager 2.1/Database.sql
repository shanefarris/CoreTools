USE [master]
GO
/****** Object:  Database [AssetManager]    Script Date: 9/25/2014 1:44:06 PM ******/
CREATE DATABASE [AssetManager] ON  PRIMARY 
( NAME = N'AssetManager', FILENAME = N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\AssetManager.mdf' , SIZE = 852992KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'AssetManager_log', FILENAME = N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\AssetManager_log.ldf' , SIZE = 47616KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [AssetManager] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [AssetManager].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [AssetManager] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [AssetManager] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [AssetManager] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [AssetManager] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [AssetManager] SET ARITHABORT OFF 
GO
ALTER DATABASE [AssetManager] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [AssetManager] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [AssetManager] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [AssetManager] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [AssetManager] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [AssetManager] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [AssetManager] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [AssetManager] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [AssetManager] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [AssetManager] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [AssetManager] SET  DISABLE_BROKER 
GO
ALTER DATABASE [AssetManager] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [AssetManager] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [AssetManager] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [AssetManager] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [AssetManager] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [AssetManager] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [AssetManager] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [AssetManager] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [AssetManager] SET  MULTI_USER 
GO
ALTER DATABASE [AssetManager] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [AssetManager] SET DB_CHAINING OFF 
GO
USE [AssetManager]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [DeleteAssetFromProfile]
	@AssetId int,
	@ProfileId int
AS
BEGIN
	SET NOCOUNT ON;
	
	DELETE AssetList 
	WHERE AssetListSet = (SELECT AssetListSet FROM Profile WHERE ProfileId = @ProfileId) AND
	AssetId = @AssetId
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [DeleteProfile]
	@ProfileId int
AS
BEGIN
	SET NOCOUNT ON;
	
	DELETE AssetList WHERE AssetListSet = (SELECT AssetListSet FROM Profile WHERE ProfileId = @ProfileId)
	DELETE Profile WHERE ProfileId = @ProfileId
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [GetRootAssets]
AS
BEGIN
	SET NOCOUNT ON;

    SELECT     *
	FROM       asset AS a
	WHERE      NOT EXISTS (SELECT * FROM AssetDependency AS ad WHERE ad.ChildAssetId = a.AssetId)
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [AssetManager].[dbo].[Asset](
	[AssetId] [int] IDENTITY(1,1) NOT NULL,
	[AssetTypeId] [int] NOT NULL,
	[FileName] [varchar](255) NOT NULL,
	[Data] [image] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[MeshTypeId] [int] NULL,
 CONSTRAINT [PK_Asset] PRIMARY KEY CLUSTERED 
(
	[AssetId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [AssetManager].[dbo].[AssetDependency](
	[AssetDependencyId] [int] IDENTITY(1,1) NOT NULL,
	[ParentAssetId] [int] NOT NULL,
	[ChildAssetId] [int] NOT NULL,
 CONSTRAINT [PK_AssetList] PRIMARY KEY CLUSTERED 
(
	[AssetDependencyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [AssetManager].[dbo].[AssetList](
	[AssetListId] [int] IDENTITY(1,1) NOT NULL,
	[AssetListSet] [nvarchar](50) NOT NULL,
	[AssetId] [int] NOT NULL,
 CONSTRAINT [PK_AssetList_1] PRIMARY KEY CLUSTERED 
(
	[AssetListId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [AssetManager].[dbo].[AssetType](
	[AssetTypeId] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
	[Extension] [nvarchar](20) NOT NULL,
	[IsText] [bit] NOT NULL,
 CONSTRAINT [PK_AssetType] PRIMARY KEY CLUSTERED 
(
	[AssetTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [AssetManager].[dbo].[Category](
	[CategoryId] [int] IDENTITY(1,1) NOT NULL,
	[ParentCategoryId] [int] NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [AssetManager].[dbo].[MeshType](
	[MeshTypeId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_MeshType] PRIMARY KEY CLUSTERED 
(
	[MeshTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [AssetManager].[dbo].[Profile](
	[ProfileId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[AssetListSet] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Profile] PRIMARY KEY CLUSTERED 
(
	[ProfileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [AssetManager].[dbo].[Asset]  WITH CHECK ADD  CONSTRAINT [FK_Asset_AssetType] FOREIGN KEY([AssetTypeId])
REFERENCES [AssetManager].[dbo].[AssetType] ([AssetTypeId])
GO
ALTER TABLE [AssetManager].[dbo].[Asset] CHECK CONSTRAINT [FK_Asset_AssetType]
GO
ALTER TABLE [AssetManager].[dbo].[Asset]  WITH CHECK ADD  CONSTRAINT [FK_Asset_Category] FOREIGN KEY([CategoryId])
REFERENCES [AssetManager].[dbo].[Category] ([CategoryId])
GO
ALTER TABLE [AssetManager].[dbo].[Asset] CHECK CONSTRAINT [FK_Asset_Category]
GO
ALTER TABLE [AssetManager].[dbo].[Asset]  WITH CHECK ADD  CONSTRAINT [FK_Asset_MeshType] FOREIGN KEY([MeshTypeId])
REFERENCES [AssetManager].[dbo].[MeshType] ([MeshTypeId])
GO
ALTER TABLE [AssetManager].[dbo].[Asset] CHECK CONSTRAINT [FK_Asset_MeshType]
GO
ALTER TABLE [AssetManager].[dbo].[AssetDependency]  WITH CHECK ADD  CONSTRAINT [FK_AssetDependency_Asset] FOREIGN KEY([ChildAssetId])
REFERENCES [AssetManager].[dbo].[Asset] ([AssetId])
GO
ALTER TABLE [AssetManager].[dbo].[AssetDependency] CHECK CONSTRAINT [FK_AssetDependency_Asset]
GO
ALTER TABLE [AssetManager].[dbo].[AssetDependency]  WITH CHECK ADD  CONSTRAINT [FK_AssetDependency_Asset1] FOREIGN KEY([ParentAssetId])
REFERENCES [AssetManager].[dbo].[Asset] ([AssetId])
GO
ALTER TABLE [AssetManager].[dbo].[AssetDependency] CHECK CONSTRAINT [FK_AssetDependency_Asset1]
GO
ALTER TABLE [AssetManager].[dbo].[AssetList]  WITH CHECK ADD  CONSTRAINT [FK_AssetList_Asset] FOREIGN KEY([AssetId])
REFERENCES [AssetManager].[dbo].[Asset] ([AssetId])
GO
ALTER TABLE [AssetManager].[dbo].[AssetList] CHECK CONSTRAINT [FK_AssetList_Asset]
GO

-- AssetType
SET IDENTITY_INSERT [AssetManager].[dbo].[AssetType] ON 
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (1, N'Material Script', N'material', 1)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (2, N'Mesh 3D Model', N'mesh', 0)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (3, N'Overlay Script', N'overlay', 1)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (4, N'Particle Script', N'particle', 1)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (5, N'Skeleton Animaiton file', N'skeleton', 0)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (6, N'GPU Script File', N'program', 1)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (7, N'TGA Image', N'tga', 0)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (8, N'PNG Image', N'png', 0)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (9, N'BitMap Image', N'bmp', 0)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (10, N'JPG Image', N'jpg', 0)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (11, N'TIF Image', N'tif', 0)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (12, N'PSD Image', N'psd', 0)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (13, N'DDS Image', N'dds', 0)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (14, N'CG Script', N'cg', 1)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (15, N'HLSL Script', N'hlsl', 1)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (16, N'Source Script', N'source', 1)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (17, N'Zip Compressed File', N'zip', 0)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (18, N'OGG Audio File', N'ogg', 0)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (19, N'Vertex File', N'vert', 0)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (20, N'Fragment File', N'frag', 0)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (21, N'Custom Text File', N'text', 1)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (22, N'Custom Binary File', N'bin', 0)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (23, N'Ode Ragdoll File', N'ogreode', 1)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (24, N'Font Script', N'fontdef', 1)
INSERT [AssetManager].[dbo].[AssetType] ([AssetTypeId], [Description], [Extension], [IsText]) VALUES (25, N'True Type Font', N'ttf', 0)
SET IDENTITY_INSERT [AssetManager].[dbo].[AssetType] OFF

-- MeshType
SET IDENTITY_INSERT [AssetManager].[dbo].[MeshType] ON 
INSERT [AssetManager].[dbo].[MeshType] ([MeshTypeId], [Name]) VALUES (1, N'Game Object')
INSERT [AssetManager].[dbo].[MeshType] ([MeshTypeId], [Name]) VALUES (2, N'Character')
INSERT [AssetManager].[dbo].[MeshType] ([MeshTypeId], [Name]) VALUES (3, N'Building')
SET IDENTITY_INSERT [AssetManager].[dbo].[MeshType] OFF

-- Category
INSERT INTO [AssetManager].[dbo].[Category]([ParentCategoryId], [Name]) VALUES (NULL ,N'[Default]'>)
INSERT INTO [AssetManager].[dbo].[Category]([ParentCategoryId], [Name]) VALUES (NULL ,N'Billboard')
INSERT INTO [AssetManager].[dbo].[Category]([ParentCategoryId], [Name]) VALUES (NULL ,N'Effects')
INSERT INTO [AssetManager].[dbo].[Category]([ParentCategoryId], [Name]) VALUES (NULL ,N'Environment')
INSERT INTO [AssetManager].[dbo].[Category]([ParentCategoryId], [Name]) VALUES (NULL ,N'Font')
INSERT INTO [AssetManager].[dbo].[Category]([ParentCategoryId], [Name]) VALUES (NULL ,N'GUI')
INSERT INTO [AssetManager].[dbo].[Category]([ParentCategoryId], [Name]) VALUES (NULL ,N'Model Building')
INSERT INTO [AssetManager].[dbo].[Category]([ParentCategoryId], [Name]) VALUES (NULL ,N'Model Character')
INSERT INTO [AssetManager].[dbo].[Category]([ParentCategoryId], [Name]) VALUES (NULL ,N'Model Enviroment')
INSERT INTO [AssetManager].[dbo].[Category]([ParentCategoryId], [Name]) VALUES (NULL ,N'Model Other')
INSERT INTO [AssetManager].[dbo].[Category]([ParentCategoryId], [Name]) VALUES (NULL ,N'Model Vehicle')
INSERT INTO [AssetManager].[dbo].[Category]([ParentCategoryId], [Name]) VALUES (NULL ,N'Model Weapon')
INSERT INTO [AssetManager].[dbo].[Category]([ParentCategoryId], [Name]) VALUES (NULL ,N'Pack')
INSERT INTO [AssetManager].[dbo].[Category]([ParentCategoryId], [Name]) VALUES (NULL ,N'Script')
INSERT INTO [AssetManager].[dbo].[Category]([ParentCategoryId], [Name]) VALUES (NULL ,N'Sound')

-- Create user
USE [master]
GO
CREATE LOGIN [AssetManager] WITH PASSWORD=N't00lb0x', DEFAULT_DATABASE=[AssetManager], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO