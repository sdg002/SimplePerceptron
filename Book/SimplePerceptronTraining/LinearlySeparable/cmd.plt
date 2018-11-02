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
plot   'Class1.txt' with points title 'Class 1'  pt 7 ps 1
replot 'Class2.txt' with points title 'Class 2'  pt 3 ps 1
#set label "Untrained classifier" at 0.5,0.5 right font ",20"
#replot  +1-x linewidth 2

#set title "Epoch 4" font ",20"
replot 0.714285714285714*x +1.14285714285714



