$ErrorActionPreference="Stop"
cls
#
#Read the data points for both classes
#
$choice=Read-Host -Prompt "Do you want to test 1)Lineary separable 2)Linearyly inseparable  (Type 1 or 2)"
if ($choice -eq "1")
{
    $global:subfolder="LinearlySeparable";    
}
elseif ($choice -eq "2")
{
    $global:subfolder="Not-LinearlySeparable";
}
else
{
    Write-Host "Invalid choice.Quitting"
    return;
}
write-host ("You selected {0}" -f $global:subfolder)
$ptsClass1=Import-Csv -Path "$PSScriptRoot\$global:subfolder\Class1.txt" -Delimiter ","
"{0} points were read from Class1.txt" -f $ptsClass1.Length
$ptsClass2=Import-Csv -Path "$PSScriptRoot\$global:subfolder\Class2.txt" -Delimiter ","
"{0} points were read from Class2.txt" -f $ptsClass2.Length

#
#Initialize the weights
#
[double]$global:w0=-1.0;
[double]$global:w1=+1.0;
[double]$global:w2=+1.0;
[double]$global:learningrate=0.5;
#
#We will expect points belonging to Class1 to give us an output of -1 and Class2 to +1
#

function ComputeActivation
{
    param([double]$x1,[double]$x2)
    $f=$global:w0*1.0 + $global:w1*$x1 + $global:w2*$x2;
    if ($f -gt 0)
    {
        [double]+1.0;
    }
    elseif ($f -lt 0)
    {
        [double]-1.0;
    }
    else
    {
        [double]0.0;
    }
}


function TrainPoints
{
    param([array]$points)
    $misclassifications=0; #how many points were not classified correctly
    $totalpoints=0;
    foreach($pt in $points)
    {
        Write-Host "*****"
        $totalpoints++;
        [double]$expected=$pt.expected;
        $activation=ComputeActivation -x1 $pt.x -x2 $pt.y
        $message="x0={0}    x1={1}    Actual={2}    Expected={3}" -f $pt.x,$pt.y,$activation,$expected
        Write-Host $message 
        $message="W0={0}    W1={1}    W2={2}" -f $global:w0,$global:w1,$global:w2
        Write-Host $message 
        if ($activation -eq $expected)
        {
            #do nothing if the expected value matches the actual activation value
            Write-Host "ACTION:DO NOTHING"
            continue;
        }
        else
        {
            Write-Host "ACTION:UPDATE WEIGHTS"
        }
        $misclassifications++;
        #The input value for the attribute w0 is a fixed 1.0
        $global:w0=$global:w0 + $global:learningrate * 1.0 * $expected;
        #We treat the x axis as x1
        $global:w1=$global:w1 + $global:learningrate * $pt.x * $expected;
        #We treat the y axis as x2
        $global:w2=$global:w2 + $global:learningrate * $pt.y * $expected;
    }
    Write-Host  "------------------------------------------------------------------------------------"
    $misclassifications;#pass this back to the caller so the results can be plotted
}
function EvaluateNetwork
{
    param([array]$points)
    $misclassifications=0; #how many points were not classified correctly
    #
    #Fires all the training points on the current state of the network
    #and makes a note of total missclassifications. No weights are updated
    #
    foreach($pt in $points)
    {
        [double]$expected=$pt.expected;
        $activation=ComputeActivation -x1 $pt.x -x2 $pt.y
        if ($activation -eq $expected)
        {
            #this data point was a success
        }
        else
        {
            $misclassifications++;
        }
    }
    $misclassifications;
}
#
#Takes in a training point with 
#    properties x1,x2 and returns a new training point with 
#    properties x1,x2,expected
#
function CreateNewTrainingPoint
{
    param($point,[double]$expected)
    $properties=@{
            "x"=[double]$point.x;
            "y"=[double]$point.y;
            "expected"=[double]$expected}
    $custom=New-Object -TypeName psobject -Property $properties
    $custom;
}
#
#This functioni will create a single array of training points by merging the training points 
#in class1 and class2
#
function MergeTwoClasses
{
    param([array]$class1,[array]$class2)
    $arrMerged=New-Object -TypeName System.Collections.ArrayList;
    foreach($pt in $class1)
    {
        $ptNew=CreateNewTrainingPoint -point $pt -expected -1.0
        $arrMerged.Add($ptNew) | Out-Null;
    }
    foreach($pt in $class2)
    {
        $ptNew=CreateNewTrainingPoint -point $pt -expected +1.0
        $arrMerged.Add($ptNew) | Out-Null;
    }
    $arrMerged;
}
#
#This function will use the weights and display the equation in y=mx+x form
#
function DisplayEquationOfClassifier
{
    $m=-$global:w1/$global:w2
    $c=-$global:w0/$global:w2
    $message= "EQUATION: {0}*x  {1}" -f $m,$c
    Write-Host  $message;
    $message
}
#
#This function will log the total no of misclassifications ($errors) at the specified iterations give by $iterations
#Why do we need this? This will help us visualize the learning curve and arrive at the point where training is best
#
function LogTrainingProgress
{
    param([int]$iterations,[int]$errors,[string]$equation)
    $path="$PSScriptRoot\$global:subfolder\LearningCurve.log"
    $csv="{0},{1},{2}" -f $iterations,$errors,$equation
    $csv | out-file -Append -FilePath $path -Encoding ascii
}
#
#Delete the training curve log file if it exists. 
#When to call this function? Every time the script runs.
#
function DeleteTrainingLogFile
{
    $path="$PSScriptRoot\$global:subfolder\LearningCurve.log"
    if (Test-Path -Path $path)
    {
        del $path
    }
}
[int]$global:MAXITERATIONS=Read-Host -Prompt "Enter maximum number of epochs. E.g 10"
$merged=MergeTwoClasses -class1 $ptsClass1 -class2 $ptsClass2
DeleteTrainingLogFile
$minErrors=[int]::MaxValue;
for($epochs=1;$epochs -lt $global:MAXITERATIONS;$epochs++)
{
    "EPOCH:{0} ...BEGIN" -f $epochs
    TrainPoints -points $merged -expected -1.0
    $misclassifications=EvaluateNetwork -points $merged
    "Total misclassifications in this iteration:{0}    Total points:{1}" -f $misclassifications,$merged.Length
    $equation=DisplayEquationOfClassifier

    LogTrainingProgress -iterations $epochs -errors $misclassifications -equation $equation
    "EPOCH:{0} ...END" -f $epochs
    if ($misclassifications -lt $minErrors)
    {
        $minErrors=$misclassifications;
    }
    "----------------------------------------------------------------------------------------"    
}
Write-Host ("Min errors {0}" -f $minErrors)
