--WEB LISTING 1: Double Metaphone Script

-------------------------------------
IF OBJECT_ID('fnIsVowel') IS NOT NULL BEGIN DROP FUNCTION fnIsVowel END
GO;
CREATE FUNCTION fnIsVowel( @c char(1) )
RETURNS bit
AS
BEGIN
	IF (@c = 'A') OR (@c = 'E') OR (@c = 'I') OR (@c = 'O') OR (@c = 'U') OR (@c = 'Y') 
	BEGIN
		RETURN 1
	END
	--'ELSE' would worry SQL Server, it wants RETURN last in a scalar function
	RETURN 0
END
GO;
-----------------------------------------------
IF OBJECT_ID('fnSlavoGermanic') IS NOT NULL BEGIN DROP FUNCTION fnSlavoGermanic 
END
GO;
CREATE FUNCTION fnSlavoGermanic( @Word char(50) )
RETURNS bit
AS
BEGIN
	--Catch NULL also...
	IF (CHARINDEX('W',@Word) > 0) OR (CHARINDEX('K',@Word) > 0) OR 
(CHARINDEX('CZ',@Word) > 0)
	--'WITZ' test is in original Lawrence Philips C++ code, but appears to be a subset of the first test for 'W'
	-- OR (CHARINDEX('WITZ',@Word) > 0)
	BEGIN
		RETURN 1
	END
	--ELSE
		RETURN 0
END
GO;
---------------------------------------------------------------------------------------------------------------------------------
----------------------
--Lawrence Philips calls for a length argument, but this has two drawbacks:
--1. All target strings must be of the same length
--2. It presents an opportunity for subtle bugs, ie fnStringAt( 1, 7, 'Search me please', 'Search' ) returns 0 (no matter what is in the searched string)
--So I've eliminated the argument and fnStringAt checks the length of each target as it executes

--DEFAULTS suck with UDFs. Have to specify DEFAULT in caller - why bother?
IF OBJECT_ID('fnStringAtDef') IS NOT NULL BEGIN DROP FUNCTION fnStringAtDef END
GO;
CREATE FUNCTION fnStringAtDef( @Start int, @StringToSearch varchar(50), 
	@Target1 varchar(50), 
	@Target2 varchar(50) = NULL,
	@Target3 varchar(50) = NULL,
	@Target4 varchar(50) = NULL,
	@Target5 varchar(50) = NULL,
	@Target6 varchar(50) = NULL )
RETURNS bit
AS
BEGIN
	IF CHARINDEX(@Target1,@StringToSearch,@Start) > 0 RETURN 1
	--2 Styles, test each optional argument for NULL, nesting further tests
	--or just take advantage of CHARINDEX behavior with a NULL arg (unless 65 compatibility - code check before CREATE FUNCTION?
	--Style 1:
	--IF @Target2 IS NOT NULL
	--BEGIN
	--	IF CHARINDEX(@Target2,@StringToSearch,@Start) > 0 RETURN 1
	-- (etc.)
	--END
	--Style 2:
	IF CHARINDEX(@Target2,@StringToSearch,@Start) > 0 RETURN 1
	IF CHARINDEX(@Target3,@StringToSearch,@Start) > 0 RETURN 1
	IF CHARINDEX(@Target4,@StringToSearch,@Start) > 0 RETURN 1
	IF CHARINDEX(@Target5,@StringToSearch,@Start) > 0 RETURN 1
	IF CHARINDEX(@Target6,@StringToSearch,@Start) > 0 RETURN 1
	RETURN 0
END
GO;
-------------------------------------------------------------------------------------------------
IF OBJECT_ID('fnStringAt') IS NOT NULL BEGIN DROP FUNCTION fnStringAt END
GO;
CREATE FUNCTION fnStringAt( @Start int, @StringToSearch varchar(50), @TargetStrings 
varchar(2000) )
RETURNS bit
AS
BEGIN
	DECLARE @SingleTarget varchar(50)
	DECLARE @CurrentStart int
	DECLARE @CurrentLength int
	
	--Eliminate special cases
	--Trailing space is needed to check for end of word in some cases, so always append comma
	--loop tests should fairly quickly ignore ',,' termination
	SET @TargetStrings = @TargetStrings + ','
	
	SET @CurrentStart = 1
	--Include terminating comma so spaces don't get truncated
	SET @CurrentLength = (CHARINDEX(',',@TargetStrings,@CurrentStart) - 
@CurrentStart) + 1
	SET @SingleTarget = SUBSTRING(@TargetStrings,@CurrentStart,@CurrentLength)
	WHILE LEN(@SingleTarget) > 1
	BEGIN
		IF SUBSTRING(@StringToSearch,@Start,LEN(@SingleTarget)-1) = 
LEFT(@SingleTarget,LEN(@SingleTarget)-1)
		BEGIN
			RETURN 1
		END
		SET @CurrentStart = (@CurrentStart + @CurrentLength)
		SET @CurrentLength = (CHARINDEX(',',@TargetStrings,@CurrentStart) - 
@CurrentStart) + 1
		IF NOT @CurrentLength > 1 --getting trailing comma 
		BEGIN
			BREAK
		END
		SET @SingleTarget = 
SUBSTRING(@TargetStrings,@CurrentStart,@CurrentLength)
	END
	RETURN 0
END
GO;
------------------------------------------------------------------------
IF OBJECT_ID('fnDoubleMetaphoneTable') IS NOT NULL BEGIN DROP FUNCTION 
fnDoubleMetaphoneTable END
GO;
CREATE FUNCTION fnDoubleMetaphoneTable( @Word varchar(50) )
RETURNS @DMP TABLE ( Metaphone1 char(4), Metaphone2 char(4) )
AS
BEGIN
	DECLARE @MP1 varchar(4), @MP2 varchar(4)
	SET @MP1 = ''
	SET @MP2 = ''
	DECLARE @CurrentPosition int, @WordLength int, @CurrentChar char(1)
	SET @CurrentPosition = 1
	SET @WordLength = LEN(@Word)

	IF @WordLength < 1 
	BEGIN
		RETURN
	END
	
	--ensure case insensitivity
	SET @Word = UPPER(@Word)
	
	IF dbo.fnStringAt(1, @Word, 'GN,KN,PN,WR,PS') = 1 
	BEGIN
		SET @CurrentPosition = @CurrentPosition + 1
	END
	
	IF 'X' = LEFT(@Word,1)
	BEGIN
		SET @MP1 = @MP1 + 'S'
		SET @MP2 = @MP2 + 'S'
		SET @CurrentPosition = @CurrentPosition + 1
	END

	WHILE (4 > LEN(RTRIM(@MP1))) OR (4 > LEN(RTRIM(@MP2)))
	BEGIN
		IF @CurrentPosition > @WordLength 
		BEGIN
			BREAK
		END

		SET @CurrentChar = SUBSTRING(@Word,@CurrentPosition,1)

		IF @CurrentChar IN('A','E','I','O','U','Y')
		BEGIN
			IF @CurrentPosition = 1 
			BEGIN
				SET @MP1 = @MP1 + 'A'
				SET @MP2 = @MP2 + 'A'
			END
			SET @CurrentPosition = @CurrentPosition + 1
		END
		ELSE IF @CurrentChar = 'B'
		BEGIN
			SET @MP1 = @MP1 + 'P'
			SET @MP2 = @MP2 + 'P'
			IF 'B' = SUBSTRING(@Word,@CurrentPosition + 1,1) 
			BEGIN
				SET @CurrentPosition = @CurrentPosition + 2
			END
			ELSE 
			BEGIN
				SET @CurrentPosition = @CurrentPosition + 1
			END
		END
		ELSE IF @CurrentChar = 'Ç'
		BEGIN
			SET @MP1 = @MP1 + 'S'
			SET @MP2 = @MP2 + 'S'
			SET @CurrentPosition = @CurrentPosition + 1
		END
		ELSE IF @CurrentChar = 'C'
		BEGIN
			--various germanic
			IF (@CurrentPosition > 2) 
			   AND (dbo.fnIsVowel(SUBSTRING(@Word,@CurrentPosition-2,1))=0) 
			   AND (dbo.fnStringAt(@CurrentPosition-1,@Word,'ACH') = 1) 
			   AND ((SUBSTRING(@Word,@CurrentPosition+2,1) <> 'I') 
			   	AND ((SUBSTRING(@Word,@CurrentPosition+2,1) <> 'E') OR 
(dbo.fnStringAt(@CurrentPosition-2,@Word,'BACHER,MACHER')=1)))
			BEGIN
				SET @MP1 = @MP1 + 'K'
				SET @MP2 = @MP2 + 'K'
				SET @CurrentPosition = @CurrentPosition + 2
			END
			-- 'caesar'
			ELSE IF (@CurrentPosition = 1) AND 
(dbo.fnStringAt(@CurrentPosition,@Word,'CAESAR') = 1)
			BEGIN
				SET @MP1 = @MP1 + 'S'
				SET @MP2 = @MP2 + 'S'
				SET @CurrentPosition = @CurrentPosition + 2
			END
			-- 'chianti'
			ELSE IF dbo.fnStringAt(@CurrentPosition,@Word,'CHIA') = 1
			BEGIN
				SET @MP1 = @MP1 + 'K'
				SET @MP2 = @MP2 + 'K'
				SET @CurrentPosition = @CurrentPosition + 2
			END
			ELSE IF dbo.fnStringAt(@CurrentPosition,@Word,'CH') = 1
			BEGIN
				-- Find 'michael'
				IF (@CurrentPosition > 1) AND 
(dbo.fnStringAt(@CurrentPosition,@Word,'CHAE') = 1)
				BEGIN
					--First instance of alternate encoding
					SET @MP1 = @MP1 + 'K'
					SET @MP2 = @MP2 + 'X'
					SET @CurrentPosition = @CurrentPosition + 2
				END
				--greek roots e.g. 'chemistry', 'chorus'
				ELSE IF (@CurrentPosition = 1) AND (dbo.fnStringAt(2, @Word, 
'HARAC,HARIS,HOR,HYM,HIA,HEM') = 1) AND (dbo.fnStringAt(1,@Word,'CHORE') = 0)
				BEGIN
					SET @MP1 = @MP1 + 'K'
					SET @MP2 = @MP2 + 'K'
					SET @CurrentPosition = @CurrentPosition + 2
				END
				--germanic, greek, or otherwise 'ch' for 'kh' sound
				ELSE IF ((dbo.fnStringAt(1,@Word,'VAN ,VON ,SCH')=1) OR 
				   (dbo.fnStringAt(@CurrentPosition-
2,@Word,'ORCHES,ARCHIT,ORCHID')=1) OR 
				   (dbo.fnStringAt(@CurrentPosition+2,@Word,'T,S')=1) OR 
				   (((dbo.fnStringAt(@CurrentPosition-1,@Word,'A,O,U,E')=1) 
OR 
				   (@CurrentPosition = 1))  
				   AND 
(dbo.fnStringAt(@CurrentPosition+2,@Word,'L,R,N,M,B,H,F,V,W, ')=1)))
				BEGIN
					SET @MP1 = @MP1 + 'K'
					SET @MP2 = @MP2 + 'K'
					SET @CurrentPosition = @CurrentPosition + 2
				END
				ELSE
				BEGIN
                    --is this a given?
					IF (@CurrentPosition > 1)	
					BEGIN
						IF (dbo.fnStringAt(1,@Word,'MC') = 1)
						BEGIN
                            --eg McHugh
							SET @MP1 = @MP1 + 'K'
							SET @MP2 = @MP2 + 'K'
						END
						ELSE
						BEGIN
							--Alternate encoding
							SET @MP1 = @MP1 + 'X'
							SET @MP2 = @MP2 + 'K'
						END
					END
					ELSE
					BEGIN
						SET @MP1 = @MP1 + 'X'
						SET @MP2 = @MP2 + 'X'
					END
					SET @CurrentPosition = @CurrentPosition + 2
				END
			END
	                --e.g, 'czerny'
	                ELSE IF (dbo.fnStringAt(@CurrentPosition,@Word,'CZ')=1) AND 
(dbo.fnStringAt((@CurrentPosition - 2),@Word,'WICZ')=0)
	                BEGIN
				SET @MP1 = @MP1 + 'S'
				SET @MP2 = @MP2 + 'X'
	                        SET @CurrentPosition = @CurrentPosition + 2
	                END
	
	                --e.g., 'focaccia'
	                ELSE IF(dbo.fnStringAt((@CurrentPosition + 1),@Word,'CIA')=1)
	                BEGIN
				SET @MP1 = @MP1 + 'X'
				SET @MP2 = @MP2 + 'X'
	                        SET @CurrentPosition = @CurrentPosition + 3
	                END
	
	                --double 'C', but not if e.g. 'McClellan'
	                ELSE IF(dbo.fnStringAt(@CurrentPosition,@Word,'CC')=1) AND NOT 
((@CurrentPosition = 2) AND (LEFT(@Word,1) = 'M'))
	                        --'bellocchio' but not 'bacchus'
	                        IF (dbo.fnStringAt((@CurrentPosition + 2),@Word,'I,E,H')=1) AND 
(dbo.fnStringAt((@CurrentPosition + 2),@Word,'HU')=0)
	                        BEGIN
	                                --'accident', 'accede' 'succeed'
	                                IF (((@CurrentPosition = 2) AND 
(SUBSTRING(@Word,@CurrentPosition - 1,1) = 'A')) 
	                                                OR (dbo.fnStringAt((@CurrentPosition - 
1),@Word,'UCCEE,UCCES')=1))
					BEGIN
						SET @MP1 = @MP1 + 'KS'
						SET @MP2 = @MP2 + 'KS'
					END
	                                --'bacci', 'bertucci', other italian
	                                ELSE
					BEGIN
						SET @MP1 = @MP1 + 'X'
						SET @MP2 = @MP2 + 'X'
					END
		                        SET @CurrentPosition = @CurrentPosition + 3
				END
                            --Pierce's rule
	                        ELSE 
				BEGIN
					SET @MP1 = @MP1 + 'K'
					SET @MP2 = @MP2 + 'K'
		                        SET @CurrentPosition = @CurrentPosition + 2
	                        END
	
	                ELSE IF (dbo.fnStringAt(@CurrentPosition,@Word,'CK,CG,CQ')=1)
	                BEGIN
				SET @MP1 = @MP1 + 'K'
				SET @MP2 = @MP2 + 'K'
	                        SET @CurrentPosition = @CurrentPosition + 2
	                END
	
	                ELSE IF (dbo.fnStringAt(@CurrentPosition,@Word,'CI,CE,CY')=1)
	                BEGIN
	                        --italian vs. english
	                        IF (dbo.fnStringAt(@CurrentPosition,@Word,'CIO,CIE,CIA')=1)
				BEGIN
					SET @MP1 = @MP1 + 'S'
					SET @MP2 = @MP2 + 'X'
				END
	                        ELSE
				BEGIN
					SET @MP1 = @MP1 + 'S'
					SET @MP2 = @MP2 + 'S'
				END
	                        SET @CurrentPosition = @CurrentPosition + 2
	                END
	
	                ELSE
			BEGIN
				SET @MP1 = @MP1 + 'K'
				SET @MP2 = @MP2 + 'K'
	                
		                --name sent in 'mac caffrey', 'mac gregor
		                IF (dbo.fnStringAt((@CurrentPosition + 1),@Word,' C, Q, G')=1)
				BEGIN
		                        SET @CurrentPosition = @CurrentPosition + 3
				END
		                ELSE
				BEGIN
		                        IF (dbo.fnStringAt((@CurrentPosition + 1),@Word,'C,K,Q')=1)
		                                AND (dbo.fnStringAt((@CurrentPosition + 1), 2, 'CE,CI')=0)
					BEGIN
			                        SET @CurrentPosition = @CurrentPosition + 2
					END
		                        ELSE
					BEGIN
			                        SET @CurrentPosition = @CurrentPosition + 1
					END
		                END
			END
	
		END
		ELSE IF @CurrentChar = 'D'
		BEGIN
	                IF (dbo.fnStringAt(@CurrentPosition, @Word, 'DG')=1)
			BEGIN
	                        IF (dbo.fnStringAt((@CurrentPosition + 2),@Word,'I,E,Y')=1)
	                        BEGIN
	                                --e.g. 'edge'
					SET @MP1 = @MP1 + 'J'
					SET @MP2 = @MP2 + 'J'
		                        SET @CurrentPosition = @CurrentPosition + 3
	                        END
	                        ELSE
				BEGIN
	                                --e.g. 'edgar'
					SET @MP1 = @MP1 + 'TK'
					SET @MP2 = @MP2 + 'TK'
		                        SET @CurrentPosition = @CurrentPosition + 2
	                        END
			END
	                ELSE IF (dbo.fnStringAt(@CurrentPosition,@Word,'DT,DD')=1)
	                BEGIN
				SET @MP1 = @MP1 + 'T'
				SET @MP2 = @MP2 + 'T'
	                        SET @CurrentPosition = @CurrentPosition + 2
	                END
	                ELSE
			BEGIN
				SET @MP1 = @MP1 + 'T'
				SET @MP2 = @MP2 + 'T'
	                        SET @CurrentPosition = @CurrentPosition + 1
			END
		END
	
		ELSE IF @CurrentChar = 'F'
		BEGIN
	                IF (SUBSTRING(@Word,@CurrentPosition + 1,1) = 'F')
			BEGIN
	                        SET @CurrentPosition = @CurrentPosition + 2
			END
	                ELSE
			BEGIN
	                        SET @CurrentPosition = @CurrentPosition + 1
			END
			SET @MP1 = @MP1 + 'F'
			SET @MP2 = @MP2 + 'F'
		END
	
		ELSE IF @CurrentChar = 'G'
		BEGIN
	                IF (SUBSTRING(@Word,@CurrentPosition + 1,1) = 'H')
	                BEGIN
	                        IF (@CurrentPosition > 1) AND 
(dbo.fnIsVowel(SUBSTRING(@Word,@CurrentPosition - 1,1)) = 0)
	                        BEGIN
					SET @MP1 = @MP1 + 'K'
					SET @MP2 = @MP2 + 'K'
		                        SET @CurrentPosition = @CurrentPosition + 2
	                        END
                                --'ghislane', ghiradelli
                                ELSE IF (@CurrentPosition = 1)
                                BEGIN 
                                        IF (SUBSTRING(@Word,@CurrentPosition + 2,1) = 'I')
					BEGIN
						SET @MP1 = @MP1 + 'J'
						SET @MP2 = @MP2 + 'J'
					END
                                        ELSE
					BEGIN
						SET @MP1 = @MP1 + 'K'
						SET @MP2 = @MP2 + 'K'
					END
		                        SET @CurrentPosition = @CurrentPosition + 2
	                        END
	                        --Parker's rule (with some further refinements) - e.g., 'hugh'
	                        ELSE IF (((@CurrentPosition > 2) AND (dbo.fnStringAt((@CurrentPosition 
- 2),@Word,'B,H,D')=1) )
	                                --e.g., 'bough'
	                                OR ((@CurrentPosition > 3) AND (dbo.fnStringAt((@CurrentPosition 
- 3),@Word,'B,H,D')=1) )
	                                --e.g., 'broughton'
	                                OR ((@CurrentPosition > 4) AND (dbo.fnStringAt((@CurrentPosition 
- 4),@Word,'B,H')=1) ) )
	                        BEGIN
		                        SET @CurrentPosition = @CurrentPosition + 2
				END
	                        ELSE
				BEGIN
	                                --e.g., 'laugh', 'McLaughlin', 'cough', 'gough', 'rough', 'tough'
	                                IF ((@CurrentPosition > 3) 
	                                        AND (SUBSTRING(@Word,@CurrentPosition - 1,1) = 'U') 
	                                        AND (dbo.fnStringAt((@CurrentPosition - 
3),@Word,'C,G,L,R,T')=1) )
	                                BEGIN
						SET @MP1 = @MP1 + 'F'
						SET @MP2 = @MP2 + 'F'
					END
	                                ELSE
					BEGIN
	                                        IF ((@CurrentPosition > 1) AND 
SUBSTRING(@Word,@CurrentPosition - 1,1) <> 'I')
						BEGIN
							SET @MP1 = @MP1 + 'K'
							SET @MP2 = @MP2 + 'K'
						END
					END
	
		                        SET @CurrentPosition = @CurrentPosition + 2
	                        END
	                END
	
	                ELSE IF (SUBSTRING(@Word,@CurrentPosition + 1,1) = 'N')
	                BEGIN
	                        IF ((@CurrentPosition = 2) AND (dbo.fnIsVowel(LEFT(@Word,1))=1) AND 
(dbo.fnSlavoGermanic(@Word)=0))
	                        BEGIN
					SET @MP1 = @MP1 + 'KN'
					SET @MP2 = @MP2 + 'N'
				END
	                        ELSE
				BEGIN
	                                --not e.g. 'cagney'
	                                IF ((dbo.fnStringAt((@CurrentPosition + 2),@Word,'EY')=0) 
	                                                AND (SUBSTRING(@Word,@CurrentPosition + 1,1) <> 
'Y') AND (dbo.fnSlavoGermanic(@Word)=0))
	                                BEGIN
						SET @MP1 = @MP1 + 'N'
						SET @MP2 = @MP2 + 'KN'
					END
	                                ELSE
					BEGIN
						SET @MP1 = @MP1 + 'KN'
						SET @MP2 = @MP2 + 'KN'
					END
				END
	                        SET @CurrentPosition = @CurrentPosition + 2
	                END
	
	                --'tagliaro'
	                ELSE IF (dbo.fnStringAt((@CurrentPosition + 1),@Word,'LI')=1) AND 
(dbo.fnSlavoGermanic(@Word)=0)
	                BEGIN
				SET @MP1 = @MP1 + 'KL'
				SET @MP2 = @MP2 + 'L'
	                        SET @CurrentPosition = @CurrentPosition + 2
	                END
	
	                -- -ges-,-gep-,-gel-, -gie- at beginning
			-- This call to fnStringAt() is the 'worst case' in number of values passed. A UDF that used DEFAULT values instead of
                        -- a multi-valued argument would require ten DEFAULT arguments for EP, EB, EL, etc. (assuming the first was not defined with a DEFAULT).
	                ELSE IF ((@CurrentPosition = 1)
	                        AND ((SUBSTRING(@Word,@CurrentPosition + 1,1) = 'Y') 
	                                OR (dbo.fnStringAt((@CurrentPosition + 
1),@Word,'ES,EP,EB,EL,EY,IB,IL,IN,IE,EI,ER')=1)) )
	                BEGIN
				SET @MP1 = @MP1 + 'K'
				SET @MP2 = @MP2 + 'J'
	                        SET @CurrentPosition = @CurrentPosition + 2
	                END
	
	                -- -ger-,  -gy-
	                ELSE IF (((dbo.fnStringAt((@CurrentPosition + 1), @Word, 'ER')=1) OR 
(SUBSTRING(@Word,@CurrentPosition + 1,1) = 'Y'))
	                                AND (dbo.fnStringAt(1, @Word, 'DANGER,RANGER,MANGER')=0)
	                                        AND (dbo.fnStringAt((@CurrentPosition - 1), @Word, 
'E,I,RGY,OGY')=0) )
	                BEGIN
				SET @MP1 = @MP1 + 'K'
				SET @MP2 = @MP2 + 'J'
	                        SET @CurrentPosition = @CurrentPosition + 2
	                END
	
	                -- italian e.g, 'biaggi'
	                ELSE IF (dbo.fnStringAt((@CurrentPosition + 1),@Word,'E,I,Y')=1) OR 
(dbo.fnStringAt((@CurrentPosition - 1),@Word,'AGGI,OGGI')=1)
	                BEGIN
	                        --obvious germanic
	                        IF ((dbo.fnStringAt(1,@Word,'VAN ,VON ,SCH')=1)
	                                OR (dbo.fnStringAt((@CurrentPosition + 1),@Word,'ET')=1))
				BEGIN
					SET @MP1 = @MP1 + 'K'
					SET @MP2 = @MP2 + 'K'
				END
	                        ELSE
				BEGIN
	                                --always soft if french ending
	                                IF (dbo.fnStringAt((@CurrentPosition + 1),@Word,'IER ')=1)
					BEGIN
						SET @MP1 = @MP1 + 'J'
						SET @MP2 = @MP2 + 'J'
					END
	                                ELSE
					BEGIN
						SET @MP1 = @MP1 + 'J'
						SET @MP2 = @MP2 + 'K'
					END
				END
	                        SET @CurrentPosition = @CurrentPosition + 2
	                END

			ELSE
			BEGIN
		                IF (SUBSTRING(@Word,@CurrentPosition + 1,1) = 'G')
				BEGIN
		                        SET @CurrentPosition = @CurrentPosition + 2
				END
		                ELSE
				BEGIN
		                        SET @CurrentPosition = @CurrentPosition + 1
				END
				SET @MP1 = @MP1 + 'K'
				SET @MP2 = @MP2 + 'K'
			END
		END
	
		ELSE IF @CurrentChar = 'H'
		BEGIN
	                --only keep if first & before vowel or btw. 2 vowels
	                IF (((@CurrentPosition = 1) OR 
(dbo.fnIsVowel(SUBSTRING(@Word,@CurrentPosition - 1,1))=1)) 
	                        AND (dbo.fnIsVowel(SUBSTRING(@Word,@CurrentPosition + 1,1))=1))
	                BEGIN
				SET @MP1 = @MP1 + 'H'
				SET @MP2 = @MP2 + 'H'
	                        SET @CurrentPosition = @CurrentPosition + 2
			END
                    --also takes care of 'HH'
	                ELSE 
			BEGIN
	                        SET @CurrentPosition = @CurrentPosition + 1
			END
		END
	
		ELSE IF @CurrentChar = 'J'
		BEGIN
	                --obvious spanish, 'jose', 'san jacinto'
	                IF (dbo.fnStringAt(@CurrentPosition,@Word,'JOSE')=1) OR 
(dbo.fnStringAt(1,@Word,'SAN ')=1)
	                BEGIN
	                        IF (((@CurrentPosition = 1) AND (SUBSTRING(@Word,@CurrentPosition 
+ 4,1) = ' ')) OR (dbo.fnStringAt(1,@Word,'SAN ')=1) )
				BEGIN
					SET @MP1 = @MP1 + 'H'
					SET @MP2 = @MP2 + 'H'
				END
	                        ELSE
	                        BEGIN
					SET @MP1 = @MP1 + 'J'
					SET @MP2 = @MP2 + 'H'
	                        END
	                        SET @CurrentPosition = @CurrentPosition + 1
	                END
	
	                ELSE IF ((@CurrentPosition = 1) AND 
(dbo.fnStringAt(@CurrentPosition,@Word,'JOSE')=0))
			BEGIN
				SET @MP1 = @MP1 + 'J'
                --Yankelovich/Jankelowicz
				SET @MP2 = @MP2 + 'A' 
                        --it could happen!
		                IF (SUBSTRING(@Word,@CurrentPosition + 1,1) = 'J') 
				BEGIN
		                        SET @CurrentPosition = @CurrentPosition + 2
				END
		                ELSE
				BEGIN
		                        SET @CurrentPosition = @CurrentPosition + 1
				END
			END
	                ELSE
			BEGIN
	                        --spanish pron. of e.g. 'bajador'
	                        IF( (dbo.fnIsVowel(SUBSTRING(@Word,@CurrentPosition - 1,1))=1)
	                                AND (dbo.fnSlavoGermanic(@Word)=0)
	                                        AND ((SUBSTRING(@Word,@CurrentPosition + 1,1) = 'A') OR 
(SUBSTRING(@Word,@CurrentPosition + 1,1) = 'O')))
				BEGIN
					SET @MP1 = @MP1 + 'J'
					SET @MP2 = @MP2 + 'H'
				END
	                        ELSE
				BEGIN
	                                IF (@CurrentPosition = @WordLength)
					BEGIN
						SET @MP1 = @MP1 + 'J'
						SET @MP2 = @MP2 + ''
					END
	                                ELSE
					BEGIN
	                                        IF ((dbo.fnStringAt((@CurrentPosition + 1), @Word, 
'L,T,K,S,N,M,B,Z')=0) 
	                                                        AND (dbo.fnStringAt((@CurrentPosition - 1), @Word, 
'S,K,L')=0))
						BEGIN
							SET @MP1 = @MP1 + 'J'
							SET @MP2 = @MP2 + 'J'
						END
					END
				END
	                    --it could happen!
		                IF (SUBSTRING(@Word,@CurrentPosition + 1,1) = 'J') 
				BEGIN
		                        SET @CurrentPosition = @CurrentPosition + 2
				END
		                ELSE
				BEGIN
		                        SET @CurrentPosition = @CurrentPosition + 1
				END
			END
		END
	
		ELSE IF @CurrentChar = 'K'
		BEGIN
	                IF (SUBSTRING(@Word,@CurrentPosition + 1,1) = 'K')
			BEGIN
	                        SET @CurrentPosition = @CurrentPosition + 2
			END
	                ELSE
			BEGIN
	                        SET @CurrentPosition = @CurrentPosition + 1
			END
			SET @MP1 = @MP1 + 'K'
			SET @MP2 = @MP2 + 'K'
		END
	
		ELSE IF @CurrentChar = 'L'
		BEGIN
	                IF (SUBSTRING(@Word,@CurrentPosition + 1,1) = 'L')
	                BEGIN
	                        --spanish e.g. 'cabrillo', 'gallegos'
	                        IF (((@CurrentPosition = (@WordLength - 3)) 
	                                AND (dbo.fnStringAt((@CurrentPosition - 
1),@Word,'ILLO,ILLA,ALLE')=1))
	                                         OR (((dbo.fnStringAt((@WordLength - 1),@Word,'AS,OS')=1) 
OR (dbo.fnStringAt(@WordLength,@Word,'A,O')=1)) 
	                                                AND (dbo.fnStringAt((@CurrentPosition - 
1),@Word,'ALLE')=1)) )
	                        BEGIN
					SET @MP1 = @MP1 + 'L'
					SET @MP2 = @MP2 + ''
		                        SET @CurrentPosition = @CurrentPosition + 2
	                        END
				ELSE
				BEGIN
		                        SET @CurrentPosition = @CurrentPosition + 2
					SET @MP1 = @MP1 + 'L'
					SET @MP2 = @MP2 + 'L'
				END
			END
	                ELSE
			BEGIN
	                        SET @CurrentPosition = @CurrentPosition + 1
				SET @MP1 = @MP1 + 'L'
				SET @MP2 = @MP2 + 'L'
			END
		END
	
		ELSE IF @CurrentChar = 'M'
		BEGIN
                     --'dumb','thumb'
	                IF (((dbo.fnStringAt((@CurrentPosition - 1), @Word,'UMB')=1)
	                        AND (((@CurrentPosition + 1) = @WordLength) OR 
(dbo.fnStringAt((@CurrentPosition + 2),@Word,'ER')=1)))
	                               
	                                OR (SUBSTRING(@Word,@CurrentPosition + 1,1) = 'M') )
			BEGIN
	                        SET @CurrentPosition = @CurrentPosition + 2
			END
	                ELSE
			BEGIN
	                        SET @CurrentPosition = @CurrentPosition + 1
			END
			SET @MP1 = @MP1 + 'M'
			SET @MP2 = @MP2 + 'M'
		END
	
		ELSE IF @CurrentChar = 'N'
		BEGIN
	                IF (SUBSTRING(@Word,@CurrentPosition + 1,1) = 'N')
			BEGIN
	                        SET @CurrentPosition = @CurrentPosition + 2
			END
	                ELSE
			BEGIN
	                        SET @CurrentPosition = @CurrentPosition + 1
			END
			SET @MP1 = @MP1 + 'N'
			SET @MP2 = @MP2 + 'N'
		END
	
		ELSE IF @CurrentChar = 'Ñ'
		BEGIN
                        SET @CurrentPosition = @CurrentPosition + 1
			SET @MP1 = @MP1 + 'N'
			SET @MP2 = @MP2 + 'N'
		END
	
		ELSE IF @CurrentChar = 'P'
		BEGIN
                    --What about Michelle Pfeiffer, star of Grease 2? Price-Pfister?, Pfizer?
				    --Don't just look for an 'F' next, what about 'topflight', helpful, campfire, leapfrog, stepfather
				    --Sorry, Mark Knopfler, I don't know how to help you
	                IF (SUBSTRING(@Word,@CurrentPosition + 1,1) = 'H')
				
				OR ((@CurrentPosition = 1) AND 
(SUBSTRING(@Word,@CurrentPosition + 1,1) = 'F') AND 
(dbo.fnIsVowel(SUBSTRING(@Word,@CurrentPosition+2,1))=1))
	                BEGIN
				SET @MP1 = @MP1 + 'F'
				SET @MP2 = @MP2 + 'F'
	                        SET @CurrentPosition = @CurrentPosition + 2
	                END
	
	                --also account for "campbell", "raspberry"
	                ELSE 
			BEGIN
				IF (dbo.fnStringAt((@CurrentPosition + 1),@Word, 'P,B')=1)
				BEGIN
		                        SET @CurrentPosition = @CurrentPosition + 2
				END
		                ELSE
				BEGIN
		                        SET @CurrentPosition = @CurrentPosition + 1
				END
				SET @MP1 = @MP1 + 'P'
				SET @MP2 = @MP2 + 'P'
			END
		END
	
		ELSE IF @CurrentChar = 'Q'
		BEGIN
	                IF (SUBSTRING(@Word,@CurrentPosition + 1,1) = 'Q')
			BEGIN
	                        SET @CurrentPosition = @CurrentPosition + 2
			END
	                ELSE
			BEGIN
	                        SET @CurrentPosition = @CurrentPosition + 1
			END
			SET @MP1 = @MP1 + 'K'
			SET @MP2 = @MP2 + 'K'
		END
	
		ELSE IF @CurrentChar = 'R'
		BEGIN
			--QQ: Will SQL short circuit eval? Otherwise, I could try to read before string begins here...
	                --french e.g. 'rogier', but exclude 'hochmeier'
	                IF ((@CurrentPosition = @WordLength)
	                        AND (dbo.fnSlavoGermanic(@Word)=0)
	                                AND (dbo.fnStringAt((@CurrentPosition - 2), @Word, 'IE')=1) 
	                                        AND (dbo.fnStringAt((@CurrentPosition - 4), @Word, 
'ME,MA')=0))
			BEGIN
				SET @MP1 = @MP1 + ''
				SET @MP2 = @MP2 + 'R'
			END
	                ELSE
			BEGIN
				SET @MP1 = @MP1 + 'R'
				SET @MP2 = @MP2 + 'R'
			END
	
	                IF (SUBSTRING(@Word,@CurrentPosition + 1,1) = 'R')
			BEGIN
	                        SET @CurrentPosition = @CurrentPosition + 2
			END
	                ELSE
			BEGIN
	                        SET @CurrentPosition = @CurrentPosition + 1
			END
		END
	
		ELSE IF @CurrentChar = 'S'
		BEGIN
	                --special cases 'island', 'isle', 'carlisle', 'carlysle'
	                IF (dbo.fnStringAt((@CurrentPosition - 1), @Word, 'ISL,YSL')=1)
	                BEGIN
	                        SET @CurrentPosition = @CurrentPosition + 1
	                END
	
	                --special case 'sugar-'
	                ELSE IF ((@CurrentPosition = 1) AND (dbo.fnStringAt(@CurrentPosition, 
@Word, 'SUGAR')=1))
	                BEGIN
				SET @MP1 = @MP1 + 'X'
				SET @MP2 = @MP2 + 'S'
	                        SET @CurrentPosition = @CurrentPosition + 1
	                END
	
	                ELSE IF (dbo.fnStringAt(@CurrentPosition, @Word, 'SH')=1)
	                BEGIN
	                        --germanic
	                        IF (dbo.fnStringAt((@CurrentPosition + 1), @Word, 
'HEIM,HOEK,HOLM,HOLZ')=1)
				BEGIN
					SET @MP1 = @MP1 + 'S'
					SET @MP2 = @MP2 + 'S'
				END
	                        ELSE
				BEGIN
					SET @MP1 = @MP1 + 'X'
					SET @MP2 = @MP2 + 'X'
				END
	                        SET @CurrentPosition = @CurrentPosition + 2
	                END
	
	                --italian & armenian
	                ELSE IF (dbo.fnStringAt(@CurrentPosition, @Word, 'SIO,SIA')=1) OR 
(dbo.fnStringAt(@CurrentPosition, @Word, 'SIAN')=1)
	                BEGIN
	                        IF (dbo.fnSlavoGermanic(@Word)=0)
				BEGIN
					SET @MP1 = @MP1 + 'S'
					SET @MP2 = @MP2 + 'X'
				END
	                        ELSE
				BEGIN
					SET @MP1 = @MP1 + 'S'
					SET @MP2 = @MP2 + 'S'
				END
	                        SET @CurrentPosition = @CurrentPosition + 3
	                END
	
	                --german & anglicisations, e.g. 'smith' match 'schmidt', 'snider' match 'schneider'
	                --also, -sz- in slavic language altho in hungarian it is pronounced 's'
	                ELSE IF (((@CurrentPosition = 1) 
	                                AND (dbo.fnStringAt((@CurrentPosition + 1), @Word, 'M,N,L,W')=1))
	                                        OR (dbo.fnStringAt((@CurrentPosition + 1), @Word, 'Z')=1))
	                BEGIN
				SET @MP1 = @MP1 + 'S'
				SET @MP2 = @MP2 + 'X'
	                        IF (dbo.fnStringAt((@CurrentPosition + 1), @Word, 'Z')=1)
				BEGIN
		                        SET @CurrentPosition = @CurrentPosition + 2
				END
	                        ELSE
				BEGIN
		                        SET @CurrentPosition = @CurrentPosition + 1
	                        END
	                END
	
	                ELSE IF (dbo.fnStringAt(@CurrentPosition, @Word, 'SC')=1)
	                BEGIN
	                        --Schlesinger's rule
	                        IF (SUBSTRING(@Word,@CurrentPosition + 2,1) = 'H')
				BEGIN
	                                --dutch origin, e.g. 'school', 'schooner'
	                                IF (dbo.fnStringAt((@CurrentPosition + 3), @Word, 
'OO,ER,EN,UY,ED,EM')=1)
	                                BEGIN
	                                        --'schermerhorn', 'schenker'
	                                        IF (dbo.fnStringAt((@CurrentPosition + 3), @Word, 'ER,EN')=1)
	                                        BEGIN
							SET @MP1 = @MP1 + 'X'
							SET @MP2 = @MP2 + 'SK'
						END
	                                        ELSE
						BEGIN
							SET @MP1 = @MP1 + 'SK'
							SET @MP2 = @MP2 + 'SK'
						END
			                        SET @CurrentPosition = @CurrentPosition + 3
					END
	                                ELSE
					BEGIN
	                                        IF ((@CurrentPosition = 1) AND 
(dbo.fnIsVowel(SUBSTRING(@Word,3,1))=0) AND (SUBSTRING(@Word,3,1) <> 'W'))
						BEGIN
							SET @MP1 = @MP1 + 'X'
							SET @MP2 = @MP2 + 'S'
						END
	                                        ELSE
						BEGIN
							SET @MP1 = @MP1 + 'X'
							SET @MP2 = @MP2 + 'X'
						END
			                        SET @CurrentPosition = @CurrentPosition + 3
	                                END
				END
	
	                        ELSE IF (dbo.fnStringAt((@CurrentPosition + 2), @Word, 'I,E,Y')=1)
	                        BEGIN
					SET @MP1 = @MP1 + 'S'
					SET @MP2 = @MP2 + 'S'
		                        SET @CurrentPosition = @CurrentPosition + 3
	                        END
	                        ELSE
				BEGIN
					SET @MP1 = @MP1 + 'SK'
					SET @MP2 = @MP2 + 'SK'
		                        SET @CurrentPosition = @CurrentPosition + 3
	                        END
	                END
	
	                ELSE 
			BEGIN
		                --french e.g. 'resnais', 'artois'
				IF ((@CurrentPosition = @WordLength) AND 
(dbo.fnStringAt((@CurrentPosition - 2), @Word, 'AI,OI')=1))
				BEGIN
					SET @MP1 = @MP1 + ''
					SET @MP2 = @MP2 + 'S'
				END
		                ELSE
				BEGIN
					SET @MP1 = @MP1 + 'S'
					SET @MP2 = @MP2 + 'S'
				END
		
		                IF (dbo.fnStringAt((@CurrentPosition + 1), @Word, 'S,Z')=1)
				BEGIN
		                        SET @CurrentPosition = @CurrentPosition + 2
				END
		                ELSE
				BEGIN
		                        SET @CurrentPosition = @CurrentPosition + 1
				END
			END
		END
	
		ELSE IF @CurrentChar = 'T'
		BEGIN
	                IF (dbo.fnStringAt(@CurrentPosition, @Word, 'TION,TIA,TCH')=1)
	                BEGIN
				SET @MP1 = @MP1 + 'X'
				SET @MP2 = @MP2 + 'X'
	                        SET @CurrentPosition = @CurrentPosition + 3
			END
	
	                ELSE IF (dbo.fnStringAt(@CurrentPosition, @Word, 'TH,TTH')=1) 
	                BEGIN
	                        --special case 'thomas', 'thames' or germanic
	                        IF (dbo.fnStringAt((@CurrentPosition + 2), @Word, 'OM,AM')=1) 
	                                OR (dbo.fnStringAt(1, @Word, 'VAN ,VON ,SCH')=1) 
	                        BEGIN
					SET @MP1 = @MP1 + 'T'
					SET @MP2 = @MP2 + 'T'
				END
	                        ELSE	
				BEGIN
					SET @MP1 = @MP1 + '0'
					SET @MP2 = @MP2 + 'T'
	                        END
	                        SET @CurrentPosition = @CurrentPosition + 2
	                END
	
			ELSE
			BEGIN
		                IF (dbo.fnStringAt((@CurrentPosition + 1), @Word, 'T,D')=1)
				BEGIN
		                        SET @CurrentPosition = @CurrentPosition + 2
				END
		                ELSE
				BEGIN
		                        SET @CurrentPosition = @CurrentPosition + 1
				END
				SET @MP1 = @MP1 + 'T'
				SET @MP2 = @MP2 + 'T'
			END
		END
	
		ELSE IF @CurrentChar = 'V'
		BEGIN
	                IF (SUBSTRING(@Word,@CurrentPosition + 1,1) = 'V')
			BEGIN
	                        SET @CurrentPosition = @CurrentPosition + 2
			END
	                ELSE
			BEGIN
	                        SET @CurrentPosition = @CurrentPosition + 1
			END
			SET @MP1 = @MP1 + 'F'
			SET @MP2 = @MP2 + 'F'
		END
	
		ELSE IF @CurrentChar = 'W'
		BEGIN
	                --can also be in middle of word
	                IF (dbo.fnStringAt(@CurrentPosition, @Word, 'WR')=1)
	                BEGIN
				SET @MP1 = @MP1 + 'R'
				SET @MP2 = @MP2 + 'R'
	                        SET @CurrentPosition = @CurrentPosition + 2
	                END
	
	                ELSE IF ((@CurrentPosition = 1) 
	                        AND ((dbo.fnIsVowel(SUBSTRING(@Word,@CurrentPosition + 1,1))=1) 
OR (dbo.fnStringAt(@CurrentPosition, @Word, 'WH')=1)))
	                BEGIN
	                        --Wasserman should match Vasserman
	                        IF (dbo.fnIsVowel(SUBSTRING(@Word,@CurrentPosition + 1,1))=1)
				BEGIN
					SET @MP1 = @MP1 + 'A'
					SET @MP2 = @MP2 + 'F'
				END
	                        ELSE
				BEGIN
	                                --need Uomo to match Womo
					SET @MP1 = @MP1 + 'A'
					SET @MP2 = @MP2 + 'A'
				END
	                        SET @CurrentPosition = @CurrentPosition + 1
	                END
	
	                --Arnow should match Arnoff
	                ELSE IF (((@CurrentPosition = @WordLength) AND 
(dbo.fnIsVowel(SUBSTRING(@Word,@CurrentPosition - 1,1))=1)) 
	                        OR (dbo.fnStringAt((@CurrentPosition - 1), @Word, 
'EWSKI,EWSKY,OWSKI,OWSKY')=1) 
	                                        OR (dbo.fnStringAt(1, @Word, 'SCH')=1))
	                BEGIN
				SET @MP1 = @MP1 + ''
				SET @MP2 = @MP2 + 'F'
	                        SET @CurrentPosition = @CurrentPosition + 1
			END
	
	                --polish e.g. 'filipowicz'
	                ELSE IF (dbo.fnStringAt(@CurrentPosition, @Word, 'WICZ,WITZ')=1)
	                BEGIN
				SET @MP1 = @MP1 + 'TS'
				SET @MP2 = @MP2 + 'FX'
	                        SET @CurrentPosition = @CurrentPosition + 4
			END
	-- skip it
	                ELSE 
			BEGIN
	                        SET @CurrentPosition = @CurrentPosition + 1
			END
		END
	
		ELSE IF @CurrentChar = 'X'
		BEGIN
	                --french e.g. breaux
	                IF (NOT((@CurrentPosition = @WordLength) 
	                        AND ((dbo.fnStringAt((@CurrentPosition - 3), @Word, 'IAU,EAU')=1) 
	                                        OR (dbo.fnStringAt((@CurrentPosition - 2), @Word, 
'AU,OU')=1))) )
			BEGIN
				SET @MP1 = @MP1 + 'KS'
				SET @MP2 = @MP2 + 'KS'
			END
	
	                IF (dbo.fnStringAt((@CurrentPosition + 1), @Word, 'C,X')=1)
			BEGIN
	                        SET @CurrentPosition = @CurrentPosition + 2
			END
	                ELSE
			BEGIN
	                        SET @CurrentPosition = @CurrentPosition + 1
			END
		END
	
		ELSE IF @CurrentChar = 'Z'
		BEGIN
	                --chinese pinyin e.g. 'zhao'
	                IF (SUBSTRING(@Word,@CurrentPosition + 1,1) = 'H')
	                BEGIN
				SET @MP1 = @MP1 + 'J'
				SET @MP2 = @MP2 + 'J'
	                        SET @CurrentPosition = @CurrentPosition + 2
			END
	                ELSE
			BEGIN
	                        IF ((dbo.fnStringAt((@CurrentPosition + 1), @Word, 'ZO,ZI,ZA')=1) 
	                                OR ((dbo.fnSlavoGermanic(@Word)=1) AND ((@CurrentPosition > 
1) AND SUBSTRING(@Word,@CurrentPosition - 1,1) <> 'T')))
	                        BEGIN
					SET @MP1 = @MP1 + 'S'
					SET @MP2 = @MP2 + 'TS'
	                        END
	                        ELSE
				BEGIN
					SET @MP1 = @MP1 + 'S'
					SET @MP2 = @MP2 + 'S'
				END
		                IF (SUBSTRING(@Word,@CurrentPosition + 1,1) = 'Z')
				BEGIN
		                        SET @CurrentPosition = @CurrentPosition + 2
				END
		                ELSE
				BEGIN
		                        SET @CurrentPosition = @CurrentPosition + 1
		                END
			END
		END
	
	        ELSE
		BEGIN
                        SET @CurrentPosition = @CurrentPosition + 1
		END
	END
	
        --only give back 4 char metaphone
        IF (LEN(@MP1) > 4)
	BEGIN
		SET @MP1 = LEFT(@MP1, 4)
	END
        IF (LEN(@MP2) > 4)
	BEGIN
		SET @MP2 = LEFT(@MP2, 4)
	END
	IF @MP2 = @MP1
	BEGIN
		SET @MP2 = ''
	END

	INSERT @DMP(Metaphone1,Metaphone2) VALUES( @MP1, @MP2 )
	RETURN
END
GO;
------------------------------------------------------------------------
IF OBJECT_ID('fnDoubleMetaphoneScalar') IS NOT NULL BEGIN DROP FUNCTION 
fnDoubleMetaphoneScalar END
GO;
CREATE FUNCTION fnDoubleMetaphoneScalar( @MetaphoneType int, @Word varchar(50) )
RETURNS char(4)
AS
BEGIN
		RETURN (SELECT CASE @MetaphoneType WHEN 1 THEN Metaphone1 
WHEN 2 THEN Metaphone2 END FROM fnDoubleMetaphoneTable( @Word ))
END