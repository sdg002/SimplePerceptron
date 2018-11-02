$ErrorActionPreference="Stop"
cls
Set-StrictMode -Version "2.0"
#
#Read the data points for both classes
#
#$global:subfolder="Not-LinearlySeparable";
$global:subfolder="LinearlySeparable";
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

[double]$global:learningrate=0.2;
[double]$global:NORMALIZATIONMAX=10.0;
#
#We will expect points belonging to Class1 to give us an output of -1 and Class2 to +1
#

#
#This will compute the output of w0 + w1*x1 + w2*x2
#
function ComputeHyperplaneFunction
{
    param([double]$x1,[double]$x2)
    [double]$f=$global:w0*1.0 + $global:w1*$x1 + $global:w2*$x2;
    $f;
}
#
#Passes the output of the hyperplane equation through the -1 or +1 gates
#
function ComputeActivation
{
    param([double]$x1,[double]$x2)
    $f=ComputeHyperplaneFunction -x1 $x1 -x2 $x2
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

#
#Takes all training points and does a small adjustment to the weights
#
function TrainPointsUsingWidroffLearning
{
    param([array]$points)
    [double]$delta0=0.0;
    [double]$delta1=0.0;
    [double]$delta2=0.0;
    [double]$N=$points.Count;
    foreach($pt in $points)
    {
        [double]$actual=ComputeHyperplaneFunction -x1 $pt.x -x2 $pt.y
        [double]$delta=($pt.expected-$actual);
        $global:w0=$global:w0 + $global:learningrate*$delta*1.0;
        $global:w1=$global:w1 + $global:learningrate*$delta*$pt.x;
        $global:w2=$global:w2 + $global:learningrate*$delta*$pt.y;
        <#
        Write-Host ("actual={0}" -f $actual)
        Write-Host ("delta={0}" -f $delta)
        Write-Host ("w0={0}" -f $global:w0)
        Write-Host ("w1={0}" -f $global:w1)
        Write-Host ("w2={0}" -f $global:w2)
        #>
    }
    <#
    #The input value for the attribute w0 is a fixed 1.0
    $global:w0=$global:w0 - (2.0/$N) * $global:learningrate * $delta0;
    #We treat the x axis as x1
    $global:w1=$global:w1 - (2.0/$N) * $global:learningrate* $delta1;
    #We treat the y axis as x2
    $global:w2=$global:w2 - (2.0/$N) * $global:learningrate* $delta2;
    #>
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
    param([double]$x,[double]$y,[double]$expected)
    $properties=@{
            "x"=[double]$x;
            "y"=[double]$y;
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
        $ptNew=CreateNewTrainingPoint -x $pt.x -y $pt.y -expected -1.0
        $arrMerged.Add($ptNew) | Out-Null;
    }
    foreach($pt in $class2)
    {
        $ptNew=CreateNewTrainingPoint -x $pt.x -y $pt.y -expected +1.0
        $arrMerged.Add($ptNew) | Out-Null;
    }
    $arrMerged;
}
#
#This function will use the weights and display the equation in y=mx+x form
#
function DisplayEquationOfClassifier
{
    $xintercept=-$global:w0/$global:w1 * $global:NORMALIZATIONMAX;
    $yintercept=-$global:w0/$global:w0 * $global:NORMALIZATIONMAX;

    $m=-$global:w1/$global:w2;
    $c=-$global:w0/$global:w2* $global:NORMALIZATIONMAX;
    $sign="-";
    if ($c -gt 0) 
    {
        $sign="+";
    }
    $message= "EQUATION: {0}*x {1} {2}" -f $m,$sign,$c
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
function CreateNormalizedPoints
{
    param([array]$points)
    $arrNormalized=New-Object -TypeName System.Collections.ArrayList;
    foreach($pt in $points)
    {
        $ptNew=CreateNewTrainingPoint -x ($pt.x/$global:NORMALIZATIONMAX) -y ($pt.y/$global:NORMALIZATIONMAX) -expected $pt.expected
        $arrNormalized.Add($ptNew) | Out-Null;
    }
    $arrNormalized;
}
[int]$global:MAXITERATIONS=Read-Host -Prompt "Enter maximum number of epochs (E.g. 10)"
[double]$global:learningrate=Read-Host -Prompt "Enter learning rate (E.g. 0.02)"
$merged=MergeTwoClasses -class1 $ptsClass1 -class2 $ptsClass2
DeleteTrainingLogFile
$minErrors=[int]::MaxValue;
<#
#
#Gradient descent with normalized points
#
$ptsNormalized=CreateNormalizedPoints -points $merged
for($epochs=0;$epochs -lt $global:MAXITERATIONS;$epochs++)
{
    "EPOCH:{0} ...BEGIN" -f $epochs
    TrainPointsUsingWidroffLearning -points $ptsNormalized
    $misclassifications=EvaluateNetwork -points $ptsNormalized
    "Total misclassifications in this iteration:{0}    Total points:{1}" -f $misclassifications,$ptsNormalized.Length
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
#>


#
#testing with unnormalized points
#
$global:learningrate=0.02;
$global:NORMALIZATIONMAX=1.0;
for($epochs=0;$epochs -lt $global:MAXITERATIONS;$epochs++)
{
    "EPOCH:{0} ...BEGIN" -f $epochs
    $equation=DisplayEquationOfClassifier
    $misclassifications=EvaluateNetwork -points $merged
    LogTrainingProgress -iterations $epochs -errors $misclassifications -equation $equation
    "Total misclassifications in this iteration:{0}    Total points:{1}" -f $misclassifications,$merged.Length


    TrainPointsUsingWidroffLearning -points $merged
    "EPOCH:{0} ...END" -f $epochs
    if ($misclassifications -lt $minErrors)
    {
        $minErrors=$misclassifications;
    }
    "----------------------------------------------------------------------------------------"    
}
Write-Host ("Min errors {0}" -f $minErrors)
<#
#>
