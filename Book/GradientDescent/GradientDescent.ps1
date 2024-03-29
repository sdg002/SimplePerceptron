$ErrorActionPreference="Stop"
cls
$global:subfolder=$PSScriptRoot;

function CalculateFunctionValue
{
    param([double]$x)
    $value=($x-5)*($x-5) + 1;
    $value;
}
function CalculateGradientOfFunction
{
    param([double]$x)
    $gradient=2*($x-5);
    $gradient;
}
function GetPathToGradientDescentLogFile()
{
    $path="$PSScriptRoot\LearningCurve.log"
    $path;
}
#
#Delete the training curve log file if it exists. 
#When to call this function? Every time the script runs.
#
function DeleteGradientDescentLogFile
{
    $path=GetPathToGradientDescentLogFile
    if (Test-Path -Path $path)
    {
        del $path
    }
    $header="{0},{1},{2}" -f "N","X","Y"
    $header| out-file -Append -FilePath $path -Encoding ascii
}
#
#This function will log the current value of the function ($y) at the specified iterations give by $iterations
#Why do we need this? This will help us visualize how we gradually moved towards the minima
#The output file will be used by GNUPLOT so it has to be CSV
#
function LogTrainingProgress
{
    param([int]$iterations,[double]$x,[double]$y)
    $path=GetPathToGradientDescentLogFile
    $display="Iteration={0},X={1},Y={2}" -f $iterations,$x.ToString("F"),$y.ToString("F")
    $csv="{0},{1},{2}" -f $iterations,$x.ToString("F"),$y.ToString("F")
    $csv | out-file -Append -FilePath $path -Encoding ascii
    Write-Host $csv
}

[double]$global:xinitial=7.0
[int]$global:MAXITERATIONS=Read-Host -Prompt "Enter maximum number of epochs. (E.g: 10)"
[double]$global:learningrate=Read-Host -Prompt "Enter learning rate (E.g. 0.2)"
$x=$global:xinitial;
DeleteGradientDescentLogFile


for($epochs=0;$epochs -lt $global:MAXITERATIONS;$epochs++)
{
    $y=CalculateFunctionValue -x $x;
    LogTrainingProgress -iterations $epochs -x $x -y $y
    $gradient=CalculateGradientOfFunction -x $x
    $delta=$global:learningrate * $gradient;
    $xNew=$x - $delta;
    Write-Host "---------------------------------------------------------------------" -ForegroundColor Green
    Write-Host ("GRADIENT={0}   DELTA={1}" -f $gradient.ToString("F"),$delta.ToString("F"))
    $x=$xNew;
}
