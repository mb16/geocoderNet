

Function GetFiles($path, $year, $state, $zipDirectory)
{

    $Request = [System.Net.WebRequest]::Create($path) 
    $Request.Method =  [System.Net.WebRequestMethods+Ftp]::ListDirectory 
 
    $Response = $Request.GetResponse() 
    $ResponseStream = $Response.GetResponseStream() 
 
    # Read and display the text in the file 
    $Reader = new-object System.Io.StreamReader $Responsestream 


    $lines = $Reader.ReadToEnd()   
    $lines = $lines.Split("`r`n")
    foreach ($line in $lines)
    {     

        if ( $line -match ("tl_" + $year + "_" + $state)){
            [System.Console]::Writeline("Download: " + $path + $line)
            [System.Console]::Writeline("Save: " + $zipDirectory + "\" + $line)

            $ftpclient = New-Object system.Net.WebClient
            $uri = New-Object System.Uri($path + $line)
            Try{
                $ftpclient.DownloadFile($uri,$zipDirectory + "\" + $line)
            }
            Catch
            {
                [System.Console]::Writeline($_.Exception.Message)
            }


         }
    }

    # Display Status 
    "Download Complete, status:" 
    $response.StatusDescription  
 
    # Close Reader and Response objects 
    $Reader.Close() 
    $Response.Close() 

}



# -----------------------------------------------------------------------------
# This is an automatic variable set to the current file's/module's directory
$PSScriptRoot

# url ftp://ftp2.census.gov/geo/tiger/
# Currently that latest files are in the TIGER2013 directory.  Update the $year
# variable if as newer datasets are released.  This specifies the directory from
# which TIGER data files are pulled.
$year = "2013"


# http://en.wikipedia.org/wiki/Federal_Information_Processing_Standard_state_code
# Add state codes to array for each state you want to download
# e.g. 11 is District of Columbia, and 10 is Deleware. Example: @("10", "11")
$states = @("51")

$zipDirectory = $PSScriptRoot + "\..\data"

foreach ($state in $states)
{
    GetFiles ("ftp://ftp2.census.gov/geo/tiger/TIGER" + $year + "/ADDR/") $year $state $zipDirectory
    GetFiles ("ftp://ftp2.census.gov/geo/tiger/TIGER" + $year + "/EDGES/") $year $state $zipDirectory
    GetFiles ("ftp://ftp2.census.gov/geo/tiger/TIGER" + $year + "/FEATNAMES/") $year $state $zipDirectory
}