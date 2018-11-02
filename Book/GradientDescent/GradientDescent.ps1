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
function DeleteTrainingLogFile
{
    $path=GetPathToGradientDescentLogFile()
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
#
function LogTrainingProgress
{
    param([int]$iterations,[double]$x,[double]$y)
    $path=GetPathToGradientDescentLogFile()
    $csv="{0},{1},{2}" -f $iterations,$x.ToString(".###"),$y.ToString(".###")
    $csv | out-file -Append -FilePath $path -Encoding ascii
    Write-Host $csv
}

[double]$global:xinitial=7.0
[int]$global:MAXITERATIONS=Read-Host -Prompt "Enter maximum number of epochs"
[double]$global:learningrate=Read-Host -Prompt "Enter learning rate (e.g. 0.2)"
$x=$global:xinitial;
DeleteTrainingLogFile


for($epochs=0;$epochs -lt $global:MAXITERATIONS;$epochs++)
{
    $y=CalculateFunctionValue -x $x;
    LogTrainingProgress -iterations $epochs -x $x -y $y
    $gradient=CalculateGradientOfFunction -x $x
    $delta=$global:learningrate * $gradient;
    $xNew=$x - $delta;
    Write-Host ("GRADIENT={0}   DELTA={1}" -f $gradient.ToString(".###"),$delta.ToString(".###"))
    $x=$xNew;
}
