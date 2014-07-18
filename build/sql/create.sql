-- initialize the database tables.
-- 'place' contains the gazetteer of place names.
if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'place') drop table place; -- its OK to drop this one as it will get reloaded.
CREATE TABLE place(
  zip NCHAR(5),
  city NVARCHAR(100),
  state NCHAR(2),
  city_phone NVARCHAR(10),
  city_phone1 NVARCHAR(10),
  city_phone2 NVARCHAR(10),
  lat NUMERIC(9,6),
  lon NUMERIC(9,6),
  status CHAR(1),
  fips_class NCHAR(2),
  fips_place NCHAR(7),
  fips_county NCHAR(5),
  priority NCHAR(1));
-- 'edge' stores the line geometries and their IDs.
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'edge')
    CREATE TABLE edge (
      tlid int PRIMARY KEY,
      geometry VARCHAR(MAX));
-- 'feature' stores the name(s) and ZIP(s) of each edge.
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'feature')
    CREATE TABLE feature (
      fid INT PRIMARY KEY,
      street NVARCHAR(100),
      street_name NVARCHAR(100),
      street_phone NVARCHAR(10),
      street_phone1 NVARCHAR(10),
      paflag NVARCHAR(1),
      zip NCHAR(5));
-- 'feature_edge' links each edge to a feature.
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'feature_edge')
    CREATE TABLE feature_edge (
      fid INT,
      tlid INT);
-- 'range' stores the address range(s) for each edge.
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'range')
    CREATE TABLE range (
      tlid INT,
      fromhn INT,
      tohn INT,
      prenum NVARCHAR(12),
      zip NCHAR(5),
      side NCHAR(1));