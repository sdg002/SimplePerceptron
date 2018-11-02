reset
clear
set size square #ensure aspect ratio is 1:1
set xzeroaxis
set yzeroaxis
set datafile separator "," #delimiter
set key autotitle columnhead #uses the column header as a legend
set xlabel 'X' font ",20"
set ylabel 'Y' font ",20"
set xrange [-0:10]
set yrange [-0:10]
set grid
set xtics 1  font ",20"
set ytics 1  font ",20"
set key font ",20"

set object 1 circle at 5,1 size 0.05 fillcolor rgb "black" fillstyle solid
set label "minima" at 5,0.8
set object 2 circle at 7,5 size 0.05 fillcolor rgb "black" fillstyle solid
set label "start" at 7.1,5
plot (x-5)*(x-5) +1 
replot 'LearningCurve.log' using 2:3 with linespoints title 'learning curve' ps 3