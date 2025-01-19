USE [master]
GO
/****** Object:  Database [UneCont]    Script Date: 19/01/2025 09:56:15 ******/
CREATE DATABASE [UneCont]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'UneCont', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\UneCont.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'UneCont_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\UneCont_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [UneCont] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [UneCont].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [UneCont] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [UneCont] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [UneCont] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [UneCont] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [UneCont] SET ARITHABORT OFF 
GO
ALTER DATABASE [UneCont] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [UneCont] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [UneCont] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [UneCont] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [UneCont] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [UneCont] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [UneCont] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [UneCont] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [UneCont] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [UneCont] SET  ENABLE_BROKER 
GO
ALTER DATABASE [UneCont] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [UneCont] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [UneCont] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [UneCont] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [UneCont] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [UneCont] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [UneCont] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [UneCont] SET RECOVERY FULL 
GO
ALTER DATABASE [UneCont] SET  MULTI_USER 
GO
ALTER DATABASE [UneCont] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [UneCont] SET DB_CHAINING OFF 
GO
ALTER DATABASE [UneCont] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [UneCont] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [UneCont] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [UneCont] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'UneCont', N'ON'
GO
ALTER DATABASE [UneCont] SET QUERY_STORE = ON
GO
ALTER DATABASE [UneCont] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [UneCont]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 19/01/2025 09:56:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Log]    Script Date: 19/01/2025 09:56:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Log](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Url] [nvarchar](max) NULL,
	[DataDeInsercao] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LogAgora]    Script Date: 19/01/2025 09:56:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogAgora](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HttpMethod] [nvarchar](20) NOT NULL,
	[StatusCode] [nvarchar](20) NOT NULL,
	[UriPath] [nvarchar](100) NOT NULL,
	[TimeTaken] [int] NOT NULL,
	[ResponseSize] [nvarchar](max) NOT NULL,
	[CacheStatus] [nvarchar](50) NOT NULL,
	[LogId] [int] NOT NULL,
 CONSTRAINT [PK_LogAgora] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LogArquivo]    Script Date: 19/01/2025 09:56:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogArquivo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LogId] [int] NOT NULL,
	[TipoLog] [nvarchar](20) NOT NULL,
	[NomeArquivo] [nvarchar](100) NOT NULL,
	[CaminhoDoArquivo] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_LogArquivo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LogMinhaCdn]    Script Date: 19/01/2025 09:56:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogMinhaCdn](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResponseSize] [nvarchar](20) NOT NULL,
	[StatusCode] [nvarchar](10) NOT NULL,
	[CacheStatus] [nvarchar](30) NOT NULL,
	[Request] [nvarchar](50) NOT NULL,
	[TimeTaken] [nvarchar](max) NOT NULL,
	[LogId] [int] NOT NULL,
 CONSTRAINT [PK_LogMinhaCdn] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Index [IX_LogAgora_LogId]    Script Date: 19/01/2025 09:56:15 ******/
CREATE NONCLUSTERED INDEX [IX_LogAgora_LogId] ON [dbo].[LogAgora]
(
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_LogArquivo_LogId]    Script Date: 19/01/2025 09:56:15 ******/
CREATE NONCLUSTERED INDEX [IX_LogArquivo_LogId] ON [dbo].[LogArquivo]
(
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_LogMinhaCdn_LogId]    Script Date: 19/01/2025 09:56:15 ******/
CREATE NONCLUSTERED INDEX [IX_LogMinhaCdn_LogId] ON [dbo].[LogMinhaCdn]
(
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[LogArquivo] ADD  DEFAULT (N'') FOR [CaminhoDoArquivo]
GO
ALTER TABLE [dbo].[LogAgora]  WITH CHECK ADD  CONSTRAINT [FK_LogAgora_Log_LogId] FOREIGN KEY([LogId])
REFERENCES [dbo].[Log] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[LogAgora] CHECK CONSTRAINT [FK_LogAgora_Log_LogId]
GO
ALTER TABLE [dbo].[LogArquivo]  WITH CHECK ADD  CONSTRAINT [FK_LogArquivo_Log_LogId] FOREIGN KEY([LogId])
REFERENCES [dbo].[Log] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[LogArquivo] CHECK CONSTRAINT [FK_LogArquivo_Log_LogId]
GO
ALTER TABLE [dbo].[LogMinhaCdn]  WITH CHECK ADD  CONSTRAINT [FK_LogMinhaCdn_Log_LogId] FOREIGN KEY([LogId])
REFERENCES [dbo].[Log] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[LogMinhaCdn] CHECK CONSTRAINT [FK_LogMinhaCdn_Log_LogId]
GO
USE [master]
GO
ALTER DATABASE [UneCont] SET  READ_WRITE 
GO
