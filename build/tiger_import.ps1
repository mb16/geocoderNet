

function getSQL($sqlDir, $sqlFile)
{
    return (Get-Content ($sqlDir + $sqlFile))
}


function getCleanSQL($sql)
{
    return ($sql | Where { $_ -notmatch "--" } | Where { $_ -notmatch "SET " } | ForEach-Object { $_ -replace "COMMIT;" , "COMMIT TRAN T1;" }| ForEach-Object { $_ -replace "BEGIN;" , "BEGIN TRAN T1;" })

}

function getCleanSQLSimple($sql)
{
    return ($sql | Where { $_ -notmatch "--" })

}

# -----------------------------------------------------------------------------

<# Hey, if any sql commands in this script don't complete properly, leaving the database locked,
use this command to release all locks. (**** Warning, use at your own risk. ****).

DECLARE @cmd NVARCHAR(MAX) ;
SELECT @cmd = ISNULL(@cmd + '; ', '') + 'KILL ' + LTRIM(request_session_id)
FROM (
SELECT DISTINCT request_session_id
FROM sys.dm_tran_locks
WHERE request_session_id <> @@SPID
) s
EXEC(@cmd)

#>


# ************ These should be configured for your database. ******************
$dataSource = "your_database"
$user = "database_usename"
$pwd = "database_password"
$database = "geocoder"


# This is an automatic variable set to the current file's/module's directory
$PSScriptRoot

Clear-Host

$AddrList = @()
$EdgesList = @()
$FeatnameList = @()


# path to directory with all file downloaded from us census,  TIGER files needed: addr, edges, featnames
$zipDirectory = (get-item "$PSScriptRoot\..\data")
[System.Console]::Writeline("zipDirectory: " + $zipDirectory)
$fc = New-Object -com Scripting.FileSystemObject
$folder = $fc.GetFolder($zipDirectory.FullName)
$shell = New-Object -com Shell.Application


Foreach ($i in $folder.Files) {
    if($i.name.EndsWith(".zip","CurrentCultureIgnoreCase")){

        $zipfile = $zipDirectory.FullName + "\" + $i.name
        [System.Console]::Writeline("Unzipping: " + $zipfile)

        $srcfolder = $shell.NameSpace($zipfile)
        $destfolder = $shell.NameSpace($zipDirectory.FullName)

        $items = $srcfolder.Items()
        $destfolder.CopyHere($items);   # comment this line to not unzip files if they have already been unzipped.


        if($i.name.EndsWith("_addr.zip","CurrentCultureIgnoreCase")){
            $AddrList += $i.name
        }
        if($i.name.EndsWith("_edges.zip","CurrentCultureIgnoreCase")){
            $EdgesList += $i.name
        }
        if($i.name.EndsWith("_featnames.zip","CurrentCultureIgnoreCase")){
            $FeatnameList += $i.name
        }
    }
}




$sqlDir = (get-item "$PSScriptRoot\sql\").FullName
$cmd = (get-item "$PSScriptRoot\..\src\").FullName + "shp2pgsql.exe"

 


$connectionString = "Server=$dataSource;uid=$user; pwd=$pwd;Database=$database;Integrated Security=False;"


$connection = New-Object System.Data.SqlClient.SqlConnection
$connection.ConnectionString = $connectionString

$connection.Open()
$command = $connection.CreateCommand()
  
  
function processSQL($command, $inserts, $maxInserts = 10000){

    $Matches = [regex]::Matches($inserts, ";")

    #Note, for very large sets of inserts, (>100000) it seems sql server runs out of memory.
    #Therefore we run single inserts in that case.  There doesn't seem to be an obvious speed
    #penalty, and this might prove faster in most cases.  Decrease the number below if memory 
    #errors occur.
    if ( $Matches.count -gt $maxInserts){

        foreach ($insert in $inserts.Split(";"))
        {
            if (-Not [string]::IsNullOrEmpty($insert))
            {
                $command.CommandText = $insert + ";"
                $command.CommandTimeout = 30 
                $result = $command.ExecuteNonQuery()
                if ($result -eq -1) # print for errors.
                {
                    $command.CommandText
                    $result
                }
            }
        }
        $Matches.count
    }
    else{

        $command.CommandText = $inserts

        #Note, for some very large files, and with insufficient memory, this can become a disk bound problem.
        #Hence, this timeout may need to be increased if the connection closes before the inserts complete.
        $command.CommandTimeout = 3600 
        $result = $command.ExecuteNonQuery()
        $result
    }

}  
  
 


$command.CommandText = (getCleanSQL (getSQL $sqlDir "create.sql"))
$command.CommandText
$command.CommandTimeout = 300 
$result = $command.ExecuteNonQuery()
$result



$a = getCleanSQLSimple ((getSQL $sqlDir "doubleMetaphone.sql") -replace "\n", "\r\n")
$a


$commandList = [regex]::split($a, 'go;', [System.Text.RegularExpressions.RegexOptions]::IgnoreCase -bor [System.Text.RegularExpressions.RegexOptions]::Multiline ) # each statement separated by GO; must run as a unique execution.
foreach ($myCmd in $commandList) {
    $command.CommandText = $myCmd
    $command.CommandText
    $command.CommandTimeout = 60
    $result = $command.ExecuteNonQuery()
    $result
}




$inserts = (getSQL $sqlDir "place.sql")
processSQL $command $inserts 100



$command.CommandText = (getSQL $sqlDir "setup.sql")
$command.CommandText
$command.CommandTimeout = 300 
$result = $command.ExecuteNonQuery()
$result



foreach ($file in $EdgesList) {

    $output = & $cmd "-a" "-s" "-n" "-g" "geom" ($zipDirectory.FullName + "\" + $file) "tiger_edges"
    $inserts = (getCleanSQL $output)
    processSQL $command $inserts
    ($zipDirectory.FullName + "\" + $file)
}



foreach ($file in $AddrList) {
    $output = & $cmd "-a" "-n" ($zipDirectory.FullName + "\" + $file) "tiger_addr"
    $inserts = (getCleanSQL $output)
    processSQL $command $inserts
    ($zipDirectory.FullName + "\" + $file)
}


foreach ($file in $FeatnameList) {
    $output = & $cmd "-a" "-n" ($zipDirectory.FullName + "\" + $file) "tiger_featnames"
    $inserts = (getCleanSQL $output)
    processSQL $command $inserts
    ($zipDirectory.FullName + "\" + $file)
}




$a = getCleanSQL ((getSQL $sqlDir "convert.sql") -replace "\n", "\r\n")
$commandList = [regex]::split($a, 'go;', [System.Text.RegularExpressions.RegexOptions]::IgnoreCase -bor [System.Text.RegularExpressions.RegexOptions]::Multiline ) # each statement separated by GO; must run as a unique execution.
foreach ($cmd in $commandList) {
    $command.CommandText = $cmd
    $command.CommandText
    $command.CommandTimeout = 7200 # 86400 # 1 day timeout limit. Not normally safe, but its no knowable how long these statements will run.
    $result = $command.ExecuteNonQuery()
    $result
}



$connection.Close()

