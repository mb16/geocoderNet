geocoderNet
===========

Geocoder in C# for Sql Server. A Translation of the GeoCommons Geocoder Project.

https://github.com/geocommons/geocoder


Purpose
=======

This project is a direct translation of the GeoCommons Geocoder project written in Ruby and using the sqlite database.  This code is not idiomatic C#, but largely reflects the original Ruby code.  In almost all cases, the variable names, function names, and files have the same names.  Additionally, the web service is intended to work the same and return the same results.  These similarities have been preserved to aid debugging and to make the systems swappable. The project framework has been built using Visual Studio 2013 and MVC 5 with web API for handling service calls.  Data is stored in SQL Server, and PowerShell scripts have been provided to download the required data, and then move it into the database.

Please review the original project ReadMe to obtain information about how the geocoding process is intended to work.


Installation
============

1. Create a database in SQL Server named "geocoder".

2. In the build directory, open the tiger_download.ps1 script and edit the $year and $states variables as needed.  Then run the script in PowerShell to download the zipped data files.

3. In the build directory, edit the tiger_import.ps1 file.  Set the $dataSource, $user, and $pwd variables to enable access to your database.  The run the script in Power shell.  Note, this first unzips all the data files, and then performs inserts and data manipulation.  This can run for a long time depending on the number and size of states.  If the script needs to be rerun, comment the line "$destfolder.CopyHere($items);" to prevent unzipping the files again.

4. Start Visual Studio and open the geocoderNet.sln file in the mvc directory.  Open the web.config file and in the connectionString section, supply the database name, user name, and password for both connections.  If elmah is not used, simply uninstall the nuget package, and remove any lines of code referencing elmah.  Otherwise provide elmah to and from email addresses as well as a smtp ip address.  

Usage HTML: http://localhost:54704/home?q=YOUR_ADDRESS_HERE
Usage Web Service: http://localhost:54704/api/geocode?q=YOUR_ADDRESS_HERE


NOTES
=====

1. The feature_bin temporary table is not deleted, unlike the original project.  This holds an Identity column which needs to have its value preserved if additional data is to be imported after the original import.

2. The Units Test the in MVC project need to have the database name, user name and password supplied in the web.config to run.  Also, these are built to run against data from a specific county in Virginia, so this data needs to be present to run the Unit Testing.

3. The database stores the full geometry data, and it is only parsed when read from the data.  Laurent Dupuis kindly supplied code to parse WKB data. http://www.dupuis.me/node/28

4. The sqlite metaphone module is not available.  SQL Server supplied a soundex function which is known to be poor.  All data names are encoded using the soundex function.  Additionally all names are encoded using the double metaphone algorithm (http://sqlmag.com/t-sql/double-metaphone-sounds-great).  These sql proceedures have been included in the project.  The web.config holds a setting which determines whether the soundex or double metaphone algorithms are used when data is queried.

5. In database.cs, if a zip code is not present, we now try to find one based on the city, and then proceed to start the search with that zip code.

6. In database.cs the geocode function has a new parameter called use_find_candidates_new.  A new find_candidates function has been supplied which attempts to perform additional searches to match addresses.

Algorithm format:

a. break each city string, starting at end and build forward, testing for viable places.  Select the set with the best score.

b. if all the places have a poor score, try places_by_city.

c. build a list of streets, if a city matches the end of the string, remove it.

d. search for matching streets in zip code.

e. if candidates all have a poor street score, find all zip codes in the county of the original zip code and search for the street in all those zip codes.

f. if the result are still poor, fall back to the old method which tries all zip codes matching the first three digits (more_features...).

7.  In address.cs, and number Regex tries to handle street numbers which are hyphenated or comma delimited, such as 100-104 or 100,102,104.

8. In address.cs, the expand_parts builds a forward and backward list of street parts.

9. We use an unmodified version of shp2pgsql to process some data files.  This executable and associated libraries were acquired from http://download.osgeo.org/postgis/windows/pg93/ All files from the bin directory were included expect the GUI directory.  The zip file can be downloaded from this location, it comes in 32 and 64 bit versions.  If these files don't run on your computer, and consider downloading the latest package.


License
=======

This project is licensed under the MIT license.

The double metaphone code was freely available but did not have an explicit license.

The WellKnownBinary class was supplied by Laurent Dupuis under the LGPL license.

The shp2pgsql files are supplied under the GPL license. http://download.osgeo.org/postgis/windows/source/pg93/





