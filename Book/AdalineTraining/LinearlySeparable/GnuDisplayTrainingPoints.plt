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
#replot 0.883651885108247*x  +1.26974171708758 
#replot 0.0482654191402316*x +2.80713292849143
#replot 0.259939232680499*x+2.58020010663298
#replot 0.39054038313778*x+2.35450952852227
#replot -0.832642234363627*x  + 46.500092368246
replot 0.335903632754328*x + 2.42960287579037 title "8"
replot 0.91021092581998*x + 1.27400682337308 title "50"
replot 0.963578573055459*x + 1.12165835721823 title "100"
replot 0.999873253207872*x + 1.00042693888217 title "500"



