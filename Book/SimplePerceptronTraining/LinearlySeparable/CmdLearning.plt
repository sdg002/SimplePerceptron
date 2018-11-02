#This file contains the commands for plotting the learning curve
reset
clear
set datafile separator "," #delimiter
#set size square #ensure aspect ratio is 1:1
#set key autotitle columnhead
set ylabel 'Misclassifications' font ",20"
set xlabel 'Epochs' font ",20"
set xrange [0:20]
set yrange [0:15]
set grid
set xtics 1  font ",12"
set ytics 1  font ",15"
set key font ",20"
plot  'LearningCurve.log'  with linespoints
set obj circle at 2,0 size 0.5




