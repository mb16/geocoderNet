


-- these proceedure creations must be at the top.

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GETHOUSENUMBER]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
    DROP FUNCTION dbo.GETHOUSENUMBER;
GO;

CREATE FUNCTION dbo.GETHOUSENUMBER(@HOUSENUMBER NVARCHAR(12))
RETURNS NVARCHAR(12)
AS
BEGIN

    -- this is ugly, how to improve it??

	-- check if starts with hyphen and remove it.
	IF  CHARINDEX('-', @HOUSENUMBER) = 1 
		SELECT @HOUSENUMBER = SUBSTRING(@HOUSENUMBER, 2, LEN(@HOUSENUMBER))

	-- check for hyphen in middle, get remainder of string after hyphen
	IF  CHARINDEX('-', @HOUSENUMBER) > 1 
		SELECT @HOUSENUMBER = SUBSTRING(@HOUSENUMBER, CHARINDEX('-', @HOUSENUMBER) + 1, LEN(@HOUSENUMBER) -  CHARINDEX('-', @HOUSENUMBER))

	-- check for "/" in middle, get remainder of string after slash
	IF  CHARINDEX('/', @HOUSENUMBER) > 1 
		SELECT @HOUSENUMBER = SUBSTRING(@HOUSENUMBER, CHARINDEX('/', @HOUSENUMBER) + 1, LEN(@HOUSENUMBER) -  CHARINDEX('/', @HOUSENUMBER))


    -- a few have a letter appended after a series of numbers, such as 11600W to 11648W, here we try to drop the W
    -- this is a postfix character, maybe should add a database field for this.
	IF  PATINDEX('%[a-zA-Z]', @HOUSENUMBER) >= 1 
		SELECT @HOUSENUMBER = SUBSTRING(@HOUSENUMBER, 1, LEN(@HOUSENUMBER) -  1)


	IF  PATINDEX('%[a-zA-Z][0-9][a-zA-Z]%', @HOUSENUMBER) > 1 
		SELECT @HOUSENUMBER = SUBSTRING(@HOUSENUMBER, PATINDEX('%[a-zA-Z][0-9][a-zA-Z]%', @HOUSENUMBER) + 3, LEN(@HOUSENUMBER) -  PATINDEX('%[a-zA-Z][0-9][a-zA-Z]%', @HOUSENUMBER) - 2)
		

	IF  PATINDEX('[a-zA-Z][0-9][a-zA-Z]%', @HOUSENUMBER) >= 1 
		SELECT @HOUSENUMBER = SUBSTRING(@HOUSENUMBER, PATINDEX('[a-zA-Z][0-9][a-zA-Z]%', @HOUSENUMBER) + 3, LEN(@HOUSENUMBER) -  PATINDEX('[a-zA-Z][0-9][a-zA-Z]%', @HOUSENUMBER) - 2)

	IF  PATINDEX('[a-zA-Z][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) >= 1 
		SELECT @HOUSENUMBER = SUBSTRING(@HOUSENUMBER, PATINDEX('[a-zA-Z][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) + 4, LEN(@HOUSENUMBER) -  PATINDEX('[a-zA-Z][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) - 3)

	IF  PATINDEX('[a-zA-Z][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) >= 1 
		SELECT @HOUSENUMBER = SUBSTRING(@HOUSENUMBER, PATINDEX('[a-zA-Z][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) + 5, LEN(@HOUSENUMBER) -  PATINDEX('[a-zA-Z][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) - 4)

	IF  PATINDEX('[a-zA-Z][0-9][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) >= 1 
		SELECT @HOUSENUMBER = SUBSTRING(@HOUSENUMBER, PATINDEX('[a-zA-Z][0-9][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) + 6, LEN(@HOUSENUMBER) -  PATINDEX('[a-zA-Z][0-9][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) - 5)

	IF  PATINDEX('[a-zA-Z][0-9][0-9][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) >= 1 
		SELECT @HOUSENUMBER = SUBSTRING(@HOUSENUMBER, PATINDEX('[a-zA-Z][0-9][0-9][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) + 7, LEN(@HOUSENUMBER) -  PATINDEX('[a-zA-Z][0-9][0-9][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) - 6)



	IF  PATINDEX('[a-zA-Z][a-zA-Z][0-9][a-zA-Z]%', @HOUSENUMBER) >= 1 
		SELECT @HOUSENUMBER = SUBSTRING(@HOUSENUMBER, PATINDEX('[a-zA-Z][a-zA-Z][0-9][a-zA-Z]%', @HOUSENUMBER) + 4, LEN(@HOUSENUMBER) -  PATINDEX('[a-zA-Z][a-zA-Z][0-9][a-zA-Z]%', @HOUSENUMBER) - 3)

	IF  PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) >= 1 
		SELECT @HOUSENUMBER = SUBSTRING(@HOUSENUMBER, PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) + 5, LEN(@HOUSENUMBER) -  PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) - 4)

	IF  PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) >= 1 
		SELECT @HOUSENUMBER = SUBSTRING(@HOUSENUMBER, PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) + 6, LEN(@HOUSENUMBER) -  PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) - 5)

	IF  PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) >= 1 
		SELECT @HOUSENUMBER = SUBSTRING(@HOUSENUMBER, PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) + 7, LEN(@HOUSENUMBER) -  PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) - 6)

	IF  PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) >= 1 
		SELECT @HOUSENUMBER = SUBSTRING(@HOUSENUMBER, PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) + 8, LEN(@HOUSENUMBER) -  PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) - 7)

		
	IF  PATINDEX('%[a-zA-Z][0-9][a-zA-Z]%', @HOUSENUMBER) > 1 
		SELECT @HOUSENUMBER = SUBSTRING(@HOUSENUMBER, PATINDEX('%[a-zA-Z][0-9][a-zA-Z]%', @HOUSENUMBER) + 3, LEN(@HOUSENUMBER) -  PATINDEX('%[a-zA-Z][0-9][a-zA-Z]%', @HOUSENUMBER) - 2)
		
	IF  PATINDEX('%[a-zA-Z][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) > 1 
		SELECT @HOUSENUMBER = SUBSTRING(@HOUSENUMBER, PATINDEX('%[a-zA-Z][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) + 3, LEN(@HOUSENUMBER) -  PATINDEX('%[a-zA-Z][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) - 2)

	IF  PATINDEX('[a-zA-Z][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) >= 1 
		SELECT @HOUSENUMBER = SUBSTRING(@HOUSENUMBER, PATINDEX('[a-zA-Z][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) + 3, LEN(@HOUSENUMBER) -  PATINDEX('[a-zA-Z][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) - 2)

	IF  PATINDEX('%[a-zA-Z][a-zA-Z]%', @HOUSENUMBER) > 1 
		SELECT @HOUSENUMBER = SUBSTRING(@HOUSENUMBER, PATINDEX('%[a-zA-Z][a-zA-Z]%', @HOUSENUMBER) + 2, LEN(@HOUSENUMBER) -  PATINDEX('%[a-zA-Z][a-zA-Z]%', @HOUSENUMBER) - 1)

	IF  PATINDEX('[a-zA-Z][a-zA-Z]%', @HOUSENUMBER) >= 1 
		SELECT @HOUSENUMBER = SUBSTRING(@HOUSENUMBER, PATINDEX('[a-zA-Z][a-zA-Z]%', @HOUSENUMBER) + 2, LEN(@HOUSENUMBER) -  PATINDEX('[a-zA-Z][a-zA-Z]%', @HOUSENUMBER) - 1)

	IF  PATINDEX('%[a-zA-Z]%', @HOUSENUMBER) > 1 
		SELECT @HOUSENUMBER = SUBSTRING(@HOUSENUMBER, PATINDEX('%[a-zA-Z]%', @HOUSENUMBER) + 1, LEN(@HOUSENUMBER) -  PATINDEX('%[a-zA-Z]%', @HOUSENUMBER))

	IF  PATINDEX('[a-zA-Z]%', @HOUSENUMBER) >= 1 
		SELECT @HOUSENUMBER = SUBSTRING(@HOUSENUMBER, PATINDEX('[a-zA-Z]%', @HOUSENUMBER) + 1, LEN(@HOUSENUMBER) -  PATINDEX('[a-zA-Z]%', @HOUSENUMBER))



        RETURN @HOUSENUMBER
END
GO;


IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GETPREFIX]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
    DROP FUNCTION dbo.GETPREFIX;
GO;

CREATE FUNCTION dbo.GETPREFIX(@HOUSENUMBER NVARCHAR(12))
RETURNS NVARCHAR(12)
AS
BEGIN
	DECLARE @PREFIX NVARCHAR(12) = ''


	-- check if starts with hyphen and remove it.
	IF  CHARINDEX('-', @HOUSENUMBER) = 1 
		SELECT @HOUSENUMBER = SUBSTRING(@HOUSENUMBER, 2, LEN(@HOUSENUMBER))

	-- check for hyphen in middle, get beginning of string before hyphen
	IF  CHARINDEX('-', @HOUSENUMBER) > 1 
		SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, CHARINDEX('-', @HOUSENUMBER))
    -- check for '/' in middle, get beginning of string before slash
	ELSE IF  CHARINDEX('/', @HOUSENUMBER) > 1 
		SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, CHARINDEX('/', @HOUSENUMBER))
	ELSE
		BEGIN

            -- this is ugly, how to improve it??


			IF  PATINDEX('%[0-9][0-9][a-zA-Z]%', @HOUSENUMBER) > 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('%[0-9][0-9][a-zA-Z]%', @HOUSENUMBER) + 3)
		
			ELSE IF  PATINDEX('[0-9][0-9][a-zA-Z]%', @HOUSENUMBER) >= 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('[0-9][0-9][a-zA-Z]%', @HOUSENUMBER) + 3)
				
			ELSE IF  PATINDEX('%[a-zA-Z][0-9][a-zA-Z]%', @HOUSENUMBER) > 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('%[a-zA-Z][0-9][a-zA-Z]%', @HOUSENUMBER) + 3)


			ELSE IF  PATINDEX('[a-zA-Z][0-9][a-zA-Z]%', @HOUSENUMBER) >= 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('[a-zA-Z][0-9][a-zA-Z]%', @HOUSENUMBER) + 3)

			ELSE IF  PATINDEX('[a-zA-Z][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) >= 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('[a-zA-Z][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) + 4)

			ELSE IF  PATINDEX('[a-zA-Z][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) >= 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('[a-zA-Z][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) + 5)

			ELSE IF  PATINDEX('[a-zA-Z][0-9][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) >= 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('[a-zA-Z][0-9][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) + 6)


			ELSE IF  PATINDEX('[a-zA-Z][a-zA-Z][0-9][a-zA-Z]%', @HOUSENUMBER) >= 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('[a-zA-Z][a-zA-Z][0-9][a-zA-Z]%', @HOUSENUMBER) + 4)

			ELSE IF  PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) >= 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) + 5)

			ELSE IF  PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) >= 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) + 6)

			ELSE IF  PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) >= 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][0-9][0-9][a-zA-Z]%', @HOUSENUMBER) + 7)



			ELSE IF  PATINDEX('[a-zA-Z][a-zA-Z][0-9][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) >= 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('[a-zA-Z][a-zA-Z][0-9][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) + 5)

			ELSE IF  PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) >= 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) + 6)

			ELSE IF  PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][0-9][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) >= 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][0-9][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) + 7)

			ELSE IF  PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][0-9][0-9][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) >= 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('[a-zA-Z][a-zA-Z][0-9][0-9][0-9][0-9][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) + 8)



			ELSE IF  PATINDEX('[a-zA-Z][a-zA-Z][0-9][a-zA-Z]%', @HOUSENUMBER) >= 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('[a-zA-Z][a-zA-Z][0-9][a-zA-Z]%', @HOUSENUMBER) + 4)


			ELSE IF  PATINDEX('[a-zA-Z][0-9][a-zA-Z]%', @HOUSENUMBER) >= 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('[a-zA-Z][0-9][a-zA-Z]%', @HOUSENUMBER) + 3)

			ELSE IF  PATINDEX('[a-zA-Z][0-9][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) >= 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('[a-zA-Z][0-9][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) + 4)


			ELSE IF  PATINDEX('%[a-zA-Z][a-zA-Z]%', @HOUSENUMBER) > 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('%[a-zA-Z][a-zA-Z]%', @HOUSENUMBER) + 2)

			ELSE IF  PATINDEX('%[a-zA-Z][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) > 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('%[a-zA-Z][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) + 3)

			ELSE IF  PATINDEX('%[a-zA-Z][a-zA-Z][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) > 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('%[a-zA-Z][a-zA-Z][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) + 4)


			ELSE IF  PATINDEX('[a-zA-Z][a-zA-Z]%', @HOUSENUMBER) >= 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('[a-zA-Z][a-zA-Z]%', @HOUSENUMBER) + 2)

			ELSE IF  PATINDEX('[a-zA-Z][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) >= 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('[a-zA-Z][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) + 3)

			ELSE IF  PATINDEX('[a-zA-Z][a-zA-Z][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) >= 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('[a-zA-Z][a-zA-Z][a-zA-Z][a-zA-Z]%', @HOUSENUMBER) + 4)


			ELSE IF  PATINDEX('%[a-zA-Z]%', @HOUSENUMBER) > 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('%[a-zA-Z]%', @HOUSENUMBER) + 1)

			ELSE IF  PATINDEX('[a-zA-Z]%', @HOUSENUMBER) >= 1 
				SELECT @PREFIX = SUBSTRING(@HOUSENUMBER, 0, PATINDEX('[a-zA-Z]%', @HOUSENUMBER) + 1)
				
		END
		
        RETURN @PREFIX
END

GO;

-- tranaction seems to slow things alot, and is largely a disk bound operation.
--BEGIN TRAN T1;



-- start by indexing the temporary tables created from the input data.

IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('dbo.tiger_featnames') AND NAME ='featnames_tlid_idx')
    DROP INDEX featnames_tlid_idx ON dbo.tiger_featnames;
CREATE INDEX featnames_tlid_idx ON tiger_featnames (tlid);

IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('dbo.tiger_addr') AND NAME ='addr_tlid_idx')
    DROP INDEX addr_tlid_idx ON dbo.tiger_addr;
CREATE INDEX addr_tlid_idx ON tiger_addr (tlid);

IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('dbo.tiger_edges') AND NAME ='edges_tlid_idx')
    DROP INDEX edges_tlid_idx ON dbo.tiger_edges;
CREATE INDEX edges_tlid_idx ON tiger_edges (tlid);

-- generate a summary table matching each edge to one or more ZIPs
--   for those edges that are streets and have a name


IF OBJECT_ID('linezip','U') is not null 
	TRUNCATE TABLE linezip;
ELSE 
    CREATE TABLE linezip (
    "tlid" int,
    "zip" nvarchar(5)
    );


INSERT INTO linezip
    SELECT DISTINCT tlid, zip FROM (
        SELECT tlid, zip FROM tiger_addr a
        UNION
        SELECT tlid, zipr AS zip FROM tiger_edges e
           WHERE e.mtfcc LIKE 'S%' AND zipr <> '' AND zipr IS NOT NULL
        UNION
        SELECT tlid, zipl AS zip FROM tiger_edges e
           WHERE e.mtfcc LIKE 'S%' AND zipl <> '' AND zipl IS NOT NULL
    ) AS whatever;


IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('dbo.linezip') AND NAME ='linezip_tlid_idx')
    DROP INDEX linezip_tlid_idx ON dbo.linezip;
CREATE INDEX linezip_tlid_idx ON linezip (tlid);


-- we don't remove this table, so that identity value is preserved for future inserts.
IF OBJECT_ID('feature_bin','U') is not null 
	TRUNCATE TABLE feature_bin;
ELSE 
    CREATE TABLE feature_bin (
      fid INTEGER PRIMARY KEY identity,
      street NVARCHAR(100),
      street_name NVARCHAR(100),
      street_phone NVARCHAR(10),
      street_phone1 NVARCHAR(10),     
      paflag nvarchar(1),
      zip NCHAR(5));


INSERT INTO feature_bin (street, street_name, street_phone, street_phone1, paflag, zip)
    --SELECT DISTINCT NULL, fullname, metaphone(name,5), paflag, zip
    SELECT DISTINCT fullname, name, SOUNDEX(name), dbo.fnDoubleMetaphoneScalar(1, name), paflag, zip 
    --SELECT DISTINCT fullname, name, SOUNDEX(name), '', paflag, zip
        FROM linezip l, tiger_featnames f
        WHERE l.tlid=f.tlid AND name <> '' AND name IS NOT NULL;


IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('dbo.feature_bin') AND NAME ='feature_bin_idx')
    DROP INDEX feature_bin_idx ON dbo.feature_bin;
CREATE INDEX feature_bin_idx ON feature_bin (street, zip);


INSERT INTO feature_edge
    SELECT DISTINCT fid, f.tlid
        FROM linezip l, tiger_featnames f, feature_bin b
        WHERE l.tlid=f.tlid AND l.zip=b.zip
          AND f.fullname=b.street AND f.paflag=b.paflag;


INSERT INTO feature
    SELECT * FROM feature_bin;


SELECT TOP 0 * INTO TempEdge FROM edge;
TRUNCATE TABLE TempEdge; -- remove any existing data.


INSERT  INTO TempEdge
    SELECT l.tlid, geom FROM
        (SELECT DISTINCT tlid FROM linezip) AS l, (SELECT DISTINCT tlid, fullname, geom FROM tiger_edges) AS e
        WHERE l.tlid=e.tlid AND fullname <> '' AND fullname IS NOT NULL;



INSERT INTO edge SELECT A.tlid, A.[geometry] FROM TempEdge A LEFT JOIN edge B ON A.tlid = B.tlid AND A.[geometry] = B.[geometry] WHERE B.tlid IS NULL AND B.[geometry] IS NULL;



INSERT INTO range
    SELECT tlid, 
        dbo.GETHOUSENUMBER([fromhn]), 
        dbo.GETHOUSENUMBER([tohn]),
        dbo.GETPREFIX([tohn]), 
        zip, side
    FROM tiger_addr



IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('dbo.place') AND NAME ='place_city_phone_state_idx')
    DROP INDEX place_city_phone_state_idx ON dbo.place;
CREATE INDEX place_city_phone_state_idx ON place (city_phone, state);

IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('dbo.place') AND NAME ='place_city_phone1_state_idx')
    DROP INDEX place_city_phone1_state_idx ON dbo.place;
CREATE INDEX place_city_phone1_state_idx ON place (city_phone1, state);

IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('dbo.place') AND NAME ='place_city_phone2_state_idx')
    DROP INDEX place_city_phone2_state_idx ON dbo.place;
CREATE INDEX place_city_phone2_state_idx ON place (city_phone2, state);

IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('dbo.place') AND NAME ='place_state_zip_idx')
    DROP INDEX place_state_zip_idx ON dbo.place;
CREATE INDEX place_state_zip_idx ON place (state, zip);

IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('dbo.place') AND NAME ='place_fips_county_idx')
    DROP INDEX place_fips_county_idx ON dbo.place;
CREATE INDEX place_fips_county_idx ON place (fips_county);


IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('dbo.place') AND NAME ='place_zip_priority_idx')
    DROP INDEX place_zip_priority_idx ON dbo.tiger_edges;
CREATE INDEX place_zip_priority_idx ON place (zip, priority);



IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('dbo.feature') AND NAME ='feature_street_phone_zip_idx')
    DROP INDEX feature_street_phone_zip_idx ON dbo.feature;
CREATE INDEX feature_street_phone_zip_idx ON feature (zip, street_phone) INCLUDE (fid, street, street_phone1, paflag);

IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('dbo.feature') AND NAME ='feature_street_phone1_zip_idx')
    DROP INDEX feature_street_phone1_zip_idx ON dbo.feature;
CREATE INDEX feature_street_phone1_zip_idx ON feature (zip, street_phone1) INCLUDE (fid, street, street_phone, paflag);



IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('dbo.feature_edge') AND NAME ='feature_edge_fid_idx')
    DROP INDEX feature_edge_fid_idx ON dbo.feature_edge;
CREATE INDEX feature_edge_fid_idx ON feature_edge (fid);

IF EXISTS(SELECT * FROM sys.indexes WHERE object_id = object_id('dbo.range') AND NAME ='range_tlid_idx')
    DROP INDEX range_tlid_idx ON dbo.feature_edge;
CREATE INDEX range_tlid_idx ON range (tlid);



DROP TABLE TempEdge;

--DROP TABLE feature_bin; -- we don't drop this to preserve identity value for future inserts.
DROP TABLE linezip;
DROP TABLE tiger_addr;
DROP TABLE tiger_featnames;
DROP TABLE tiger_edges;

--COMMIT TRAN T1;
