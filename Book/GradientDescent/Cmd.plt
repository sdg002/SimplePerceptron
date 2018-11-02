#remember to do a "cd 'NeuralNetworksCsharp\SimplePerceptronTraining'" before firing "load cmd.txt"
reset
clear
set size square #ensure aspect ratio is 1:1
set xzeroaxis
set yzeroaxis
set datafile separator "," #delimiter
set key autotitle columnhead #uses the column header as a legend
set xlabel 'X1' font ",20"
set ylabel 'X2' font ",20"
#set xrange [-5:5]
#set yrange [-5:5]
set xrange [-10:10]
set yrange [-10:10]
set grid
set xtics 1  font ",20"
set ytics 1  font ",20"
set key font ",20"
plot x*x
