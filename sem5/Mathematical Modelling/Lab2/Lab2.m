clear;

%%
%Датчики БСВ

a_01 = 564853681;
c_1 = 790941697;
M = 2^31;
n0 = 10000;
a = zeros(n0, 1);
a_star = zeros(n0+1, 1);
a_star(1) = a_01;

beta = max(c_1, M - c_1);
for i=2:n0+1
    z = floor((beta * a_star(i-1))/M);
    a_star(i) = beta * a_star(i-1) - M * z;
    a(i-1) = a_star(i)/M;
end
%%
%Моделирование геометрическое №1
n1 = 5000;
p1 = 0.25;
q1 = 1 - p1;
s1 = zeros(n1, 1);

for i=1:n1
    k = 0;
    sumT = p1;
    prod = p1;
    while a(i) > sumT
        prod = prod * q1;
        sumT = sumT + prod;
        k = k + 1;
    end
    s1(i) = k;
end

disp('-----------------------------------------------------------------------------------------');
disp('Геометрическое №1: мат ожидание  = ' + string(mean(s1)) + ' ( ' + string(q1/p1) + ' )');
disp('Геометрическое №1: дисперсия  = ' + string(var(s1)) + ' ( ' + string(q1/(p1^2)) + ' )');
disp('Геометрическое №1: коэффициент асимметрии  = ' + string(skewness(s1)) + ' ( ' + string((2-p1)/(sqrt(1-p1))) + ' )');
disp('Геометрическое №1: коэффициент эксцесса  = ' + string(kurtosis(s1)) + ' ( ' + string(6+(p1^2)/(1-p1)) + ' )');
%%
%Моделирование геометрическое №2
s2 = zeros(n1, 1);

for i=1:n1
    s2(i) = random('Geometric', p1);
end

disp('-----------------------------------------------------------------------------------------');
disp('Геометрическое №2: мат ожидание  = ' + string(mean(s2)) + ' ( ' + string(q1/p1) + ' )');
disp('Геометрическое №2: дисперсия  = ' + string(var(s2)) + ' ( ' + string(q1/(p1^2)) + ' )');
disp('Геометрическое №2: коэффициент асимметрии  = ' + string(skewness(s2)) + ' ( ' + string((2-p1)/(sqrt(1-p1))) + ' )');
disp('Геометрическое №2: коэффициент эксцесса  = ' + string(kurtosis(s2)) + ' ( ' + string(6+(p1^2)/(1-p1)) + ' )');
%%
%Моделирование отрицательное биномиальное №1
n2 = 1000;
p2 = 0.6;
q2 = 1 - p2;
r = 5;
ob1 = zeros(n2, 1);
for i=1:n2
    temp = 0;
    for j = 1:r
        temp = temp + random('Geometric', p2);
    end
    ob1(i) = temp;
end

disp('-----------------------------------------------------------------------------------------');
disp('Отрицательное биномиальное №1: мат ожидание  = ' + string(mean(ob1)) + ' ( ' + string(r * q2/p2) + ' )');
disp('Отрицательное биномиальное №1: дисперсия  = ' + string(var(ob1)) + ' ( ' + string(r * q2/(p2^2)) + ' )');
disp('Отрицательное биномиальное №1: коэффициент асимметрии  = ' + string(skewness(ob1)) + ' ( ' + string((2-p2)/(sqrt(r*q2))) + ' )');
disp('Отрицательное биномиальное №1: коэффициент эксцесса  = ' + string(kurtosis(ob1)) + ' ( ' + string(6/r+(p2^2)/(q2^2)) + ' )');
%%
%Моделирование отрицательное биномиальное №2
ob2 = zeros(n2, 1);

for i=1:n2
    ob2(i) = random('Poisson', random('Gamma', r, q2 /p2));
end

disp('-----------------------------------------------------------------------------------------');
disp('Отрицательное биномиальное №2: мат ожидание  = ' + string(mean(ob2)) + ' ( ' + string(r * q2/p2) + ' )');
disp('Отрицательное биномиальное №2: дисперсия  = ' + string(var(ob2)) + ' ( ' + string(r * q2/(p2^2)) + ' )');
disp('Отрицательное биномиальное №1: коэффициент асимметрии  = ' + string(skewness(ob2)) + ' ( ' + string((2-p2)/(sqrt(r*q2))) + ' )');
disp('Отрицательное биномиальное №1: коэффициент эксцесса  = ' + string(kurtosis(ob2)) + ' ( ' + string(6/r+(p2^2)/(q2^2)) + ' )');
%%
%Моделирование отрицательное биномиальное №3
ob3 = zeros(n2, 1);

for i=1:n2
    ob3(i) = random('Negative Binomial', r, p2);
end

disp('-----------------------------------------------------------------------------------------');
disp('Отрицательное биномиальное №3: мат ожидание  = ' + string(mean(ob3)) + ' ( ' + string(r * q2/p2) + ' )');
disp('Отрицательное биномиальное №3: дисперсия  = ' + string(var(ob3)) + ' ( ' + string(r * q2/(p2^2)) + ' )');
disp('Отрицательное биномиальное №1: коэффициент асимметрии  = ' + string(skewness(ob3)) + ' ( ' + string((2-p2)/(sqrt(r*q2))) + ' )');
disp('Отрицательное биномиальное №1: коэффициент эксцесса  = ' + string(kurtosis(ob3)) + ' ( ' + string(6/r+(p2^2)/(q2^2)) + ' )');
%%
%Функция распределения 

f1 = @(y) fun(ob1, y);
f2 = @(y) fun(ob2, y);
f3 = @(y) fun(ob3, y);

f4 = @(y) fun(s1, y);
f5 = @(y) fun(s2, y);

fraspgeo = @(n) 1 - q1^(n+1);

geo = @(x) geopdf(x, p1);

x = 0:0.001:15;
x2 = 0:1:20;

y1 = zeros(length(x),1);
y2 = zeros(length(x),1);
y3 = zeros(length(x),1);
y8 = zeros(length(x),1);

y4 = zeros(length(x),1);
y5 = zeros(length(x),1);
y6 = zeros(length(x),1);

y7 = zeros(length(x2),1);
x7 = linspace(0, 20, 21);



for i = 1:length(x)
    y1(i) = f1(x(i));
    y2(i) = f2(x(i));
    y3(i) = f3(x(i));
    y4(i) = f4(x(i));
    y5(i) = f5(x(i));
    y6(i) = geocdf(x(i), p1);
    y8(i) = nbincdf(x(i), r, p2);
end
for i = 1:length(x2)
    y7(i) = geo(x2(i)) * n1;
end

%%
%Критерий хи-квадрат Пирсона
pd1 = fitdist(ob1,'NegativeBinomial');
testResult1 = chi2gof(ob1,'CDF', pd1);
pd2 = fitdist(ob1,'NegativeBinomial');
testResult2 = chi2gof(ob2,'CDF', pd2);
pd3 = fitdist(ob1,'NegativeBinomial');
testResult3 = chi2gof(ob3,'CDF', pd3);
disp('-----------------------------------------------------------------------------------------');
if (testResult1 == 0)
    disp('Критерий хи-квадрат Пирсона: ob1 принадлежит к NegativeBinomial');
else
    disp('Критерий хи-квадрат Пирсона: ob1 не принадлежит к NegativeBinomial');
end

if (testResult2 == 0)
    disp('Критерий хи-квадрат Пирсона: ob2 принадлежит к NegativeBinomial');
else
    disp('Критерий хи-квадрат Пирсона: ob2 не принадлежит к NegativeBinomial');
end

if (testResult3 == 0)
    disp('Критерий хи-квадрат Пирсона: ob3 принадлежит к NegativeBinomial');
else
    disp('Критерий хи-квадрат Пирсона: ob3 не принадлежит к NegativeBinomial');
end

%%
%Графики

figure(1);
subplot(2,3,1);
histogram(s1);
hold on
plot(x7, y7);
grid on;
title('Геометрическое №1');

subplot(2,3,2);
histogram(s2);
hold on
plot(x7, y7);
grid on;
title('Геометрическое №2');

subplot(2,3,4);
hold on;
histfit(ob1,14,'nbin')
grid on;
title('Отрицательное биномиальное №1');

subplot(2,3,5);
hold on;
histfit(ob2,14,'nbin')
grid on;
title('Отрицательное биномиальное №2');

subplot(2,3,6);
hold on;
histfit(ob3,14,'nbin')
grid on;
title('Отрицательное биномиальное №3');

figure(2);

subplot(1,2,1);
hold on;
plot(y4, '-k');
plot(y5, '-k');
plot(y6, '-b');
grid on;

subplot(1,2,2);
hold on;
plot(y1, '-k');
plot(y2, '-k');
plot(y3, '-k');
p2 = plot(y8, '-b');

grid on;


grid on;
