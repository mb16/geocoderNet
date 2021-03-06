﻿if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'tiger_edges') drop table tiger_edges;
CREATE TABLE tiger_edges (
"statefp" nvarchar(2),
"countyfp" nvarchar(3),
"tlid" int,
"tfidl" int,
"tfidr" int,
"mtfcc" nvarchar(5),
"fullname" nvarchar(100),
"smid" nvarchar(22),
"lfromadd" nvarchar(12),
"ltoadd" nvarchar(12),
"rfromadd" nvarchar(12),
"rtoadd" nvarchar(12),
"zipl" nvarchar(5),
"zipr" nvarchar(5),
"featcat" nvarchar(1),
"hydroflg" nvarchar(1),
"railflg" nvarchar(1),
"roadflg" nvarchar(1),
"olfflg" nvarchar(1),
"passflg" nvarchar(1),
"divroad" nvarchar(1),
"exttyp" nvarchar(1),
"ttyp" nvarchar(1),
"deckedroad" nvarchar(1),
"artpath" nvarchar(1),
"persist" nvarchar(1),
"gcseflg" nvarchar(1),
"offsetl" nvarchar(1),
"offsetr" nvarchar(1),
"tnidf" int,
"tnidt" int,
"geom" varchar(MAX)
);

if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'tiger_featnames') drop table tiger_featnames;
CREATE TABLE tiger_featnames (
"tlid" int,
"fullname" nvarchar(100),
"name" nvarchar(100),
"predirabrv" nvarchar(15),
"pretypabrv" nvarchar(50),
"prequalabr" nvarchar(15),
"sufdirabrv" nvarchar(15),
"suftypabrv" nvarchar(50),
"sufqualabr" nvarchar(15),
"predir" nvarchar(2),
"pretyp" nvarchar(3),
"prequal" nvarchar(2),
"sufdir" nvarchar(2),
"suftyp" nvarchar(3),
"sufqual" nvarchar(2),
"linearid" nvarchar(22),
"mtfcc" nvarchar(5),
"paflag" nvarchar(1));
if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'tiger_addr') drop table tiger_addr;
CREATE TABLE tiger_addr (
"tlid" int,
"fromhn" nvarchar(12),
"tohn" nvarchar(12),
"side" nvarchar(1),
"zip" nvarchar(5),
"plus4" nvarchar(4),
"fromtyp" nvarchar(1),
"totyp" nvarchar(1),
"fromarmid" int,
"toarmid" int,
"arid" nvarchar(22),
"mtfcc" nvarchar(5));
