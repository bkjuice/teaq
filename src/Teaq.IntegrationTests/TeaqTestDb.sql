﻿USE [master]
GO
/****** Object:  Database [TeaqTestDb]    Script Date: 5/23/2013 5:28:18 PM ******/
CREATE DATABASE [TeaqTestDb]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'TeaqTestDb', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\TeaqTestDb.mdf' , SIZE = 6072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'TeaqTestDb_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\TeaqTestDb_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [TeaqTestDb] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TeaqTestDb].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TeaqTestDb] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TeaqTestDb] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TeaqTestDb] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TeaqTestDb] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TeaqTestDb] SET ARITHABORT OFF 
GO
ALTER DATABASE [TeaqTestDb] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [TeaqTestDb] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [TeaqTestDb] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TeaqTestDb] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TeaqTestDb] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TeaqTestDb] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [TeaqTestDb] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TeaqTestDb] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TeaqTestDb] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TeaqTestDb] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TeaqTestDb] SET  DISABLE_BROKER 
GO
ALTER DATABASE [TeaqTestDb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TeaqTestDb] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TeaqTestDb] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TeaqTestDb] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [TeaqTestDb] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TeaqTestDb] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [TeaqTestDb] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TeaqTestDb] SET RECOVERY FULL 
GO
ALTER DATABASE [TeaqTestDb] SET  MULTI_USER 
GO
ALTER DATABASE [TeaqTestDb] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [TeaqTestDb] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TeaqTestDb] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [TeaqTestDb] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
EXEC sys.sp_db_vardecimal_storage_format N'TeaqTestDb', N'ON'
GO
USE [TeaqTestDb]
GO
/****** Object:  Schema [dbo2]    Script Date: 5/23/2013 5:28:18 PM ******/
CREATE SCHEMA [dbo2]
GO
/****** Object:  Table [dbo].[Address]    Script Date: 5/23/2013 5:28:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Address](
	[AddressId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[Change] [tinyint] NOT NULL,
	[AddressLine1] varchar(512)
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Customer]    Script Date: 5/23/2013 5:28:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Customer](
	[CustomerId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerKey] [char](20) NOT NULL,
	[Inception] [datetime] NOT NULL,
	[Modified] [datetimeoffset](7) NOT NULL,
	[Change] [tinyint] NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo2].[Address2]    Script Date: 5/23/2013 5:28:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo2].[Address2](
	[AddressId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[Change] [tinyint] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo2].[Customer2]    Script Date: 5/23/2013 5:28:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo2].[Customer2](
	[CustomerId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerKey] [char](20) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Index [IX_Address_1]    Script Date: 5/23/2013 5:28:18 PM ******/
CREATE CLUSTERED INDEX [IX_Address_1] ON [dbo].[Address]
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Customer_1]    Script Date: 5/23/2013 5:28:18 PM ******/
CREATE UNIQUE CLUSTERED INDEX [IX_Customer_1] ON [dbo].[Customer]
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Address2_1]    Script Date: 5/23/2013 5:28:18 PM ******/
CREATE CLUSTERED INDEX [IX_Address2_1] ON [dbo2].[Address2]
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Customer2]    Script Date: 5/23/2013 5:28:18 PM ******/
CREATE UNIQUE CLUSTERED INDEX [IX_Customer2] ON [dbo2].[Customer2]
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Address]    Script Date: 5/23/2013 5:28:18 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Address] ON [dbo].[Address]
(
	[AddressId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Customer]    Script Date: 5/23/2013 5:28:18 PM ******/
CREATE NONCLUSTERED INDEX [IX_Customer] ON [dbo].[Customer]
(
	[CustomerId] ASC,
	[CustomerKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Address2]    Script Date: 5/23/2013 5:28:18 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Address2] ON [dbo2].[Address2]
(
	[AddressId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Customer2_1]    Script Date: 5/23/2013 5:28:18 PM ******/
CREATE NONCLUSTERED INDEX [IX_Customer2_1] ON [dbo2].[Customer2]
(
	[CustomerId] ASC,
	[CustomerKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE SEQUENCE [dbo].[Int32Sequence] 
 AS [int]
 START WITH 1
 INCREMENT BY 1
 MINVALUE -2147483648
 MAXVALUE 2147483647
 CACHE  100 
GO

CREATE SEQUENCE [dbo].[Int64Sequence] 
 AS [bigint]
 START WITH -1
 INCREMENT BY 1
 MINVALUE -9223372036854775808
 MAXVALUE 9223372036854775807
 CACHE  100 
GO

create procedure select_address
@addressId int
as 
begin
	select * from [Address] where AddressId = @addressId
end
GO

USE [master]
GO
ALTER DATABASE [TeaqTestDb] SET  READ_WRITE 
GO
