%%
%input data
a_01 = 55360495;
a_02 = 98167543;
c_1 = 399880125;
c_2 = 427192983;
k = 192;
M = 2^31;
n = 1000;

%%
%а) Датчики БСВ
a = zeros(n+1, 1);
a_star = zeros(n+1, 1);
a_star(1) = a_01;
beta = max(c_1, M - c_1);
for i=2:n+1
    z = floor((beta * a_star(i-1))/M);
    a_star(i) = beta * a_star(i-1) - M * z;
    a(i-1) = a_star(i)/M;
end
%%
%б) Метод Макларена-Марсальи
alpha_1 = zeros(n+1, 1);
alpha_2 = zeros(n+1, 1);
alpha_1_star = zeros(n+1, 1);
alpha_2_star = zeros(n+1, 1);
alpha_1_star(1) = a_01;
alpha_2_star(1) = a_02;
alpha_1_beta = max(c_1, M-c_1);
alpha_2_beta = max(c_2, M-c_2);
for t = 2:n+1
  alpha_1_star(t) = mod(alpha_1_beta * alpha_1_star(t-1) - M*floor(alpha_1_beta*alpha_1_star(t-1)/M), M);
  alpha_1(t) = alpha_1_star(t) / M;
  
  alpha_2_star(t) = mod(alpha_2_beta * alpha_2_star(t-1) - M*floor(alpha_2_beta*alpha_1_star(t-1)/M), M);
  alpha_2(t) = alpha_2_star(t) / M;
end
new_a = zeros(n+1, 1);
V = alpha_1(1:k);
for t = 2:n+1 
  s = floor(alpha_2(t) * k)+1;
  new_a(t) = V(s);
  V(s) = alpha_1(mod(t+k, n)+1);
end

eps = 0.05;

%%
%1) Tест «совпадения моментов»
m = 1./n * sum(new_a);
eps1 = m-1/2;
s_sq = 0;
for i = 1:n+1
  s_sq = s_sq + (new_a(i)-m)^2;
end 
s_sq = s_sq * 1./(n-1);
eps2 = s_sq - 1./12; 
c1_n = sqrt(12*n);
c2_n = (n-1)/n * (0.0056*n^(-1) + 0.0028*n^(-2) - 0.0083*n^(-3))^(-1./2);
Lambda = 1.96; %%Порог критериев
if c1_n * abs(eps1) < Lambda
  disp('Tест «совпадения моментов» выполнен (мат ожидание)');
else 
    disp('Tест «совпадения моментов» не выполнен (мат ожидание)');
end
if c2_n * abs(eps2) < Lambda
  disp('Tест «совпадения моментов» выполнен (дисперсия)');
else 
    disp('Tест «совпадения моментов» не выполнен (дисперсия)');
end
%%
%4) Гистограммa
figure(1);
histogram(a);
figure(2);
histogram(new_a);
%%
%5) Теста «равномерность двумерного распределения»
eps2 = 0.05;
a = new_a(2:n+1);
m = n/2;
r = (0:0.05:0.5)';
k=11;
p = zeros(k, 1);
a_pair = zeros(m, 2);
for i = 1:m
    a_pair(i, 1) = a(2*(i-1)+1);
    a_pair(i, 2) = a(2*i);
end
for i = 1:k
    if i==k
        p(i) = 1-pi*r(i)^2;
    else
        p(i) = pi*(r(i+1)^2 - r(i)^2);
    end
end
temp = zeros(k, 1);
for i = 1:k
    if i == 1
        x2 = 0.5 + r(2)*cosd(0:360);
        y2 = 0.5 + r(2)*sind(0:360); 
    elseif i == k
        x1 = 0.5 + r(i)*cosd(0:360);
        y1 = 0.5 + r(i)*sind(0:360);
        xv = [0,0,1,1,0];
        yv = [0,1,1,0,0]';
    else
        x1 = 0.5 + r(i)*cosd(0:360);
        y1 = 0.5 + r(i)*sind(0:360);
        x2 = 0.5 + r(i+1)*cosd(0:360);
        y2 = 0.5 + r(i+1)*sind(0:360);
    end 
    for j=1:m
        if i == 1
            L1 = false;
            L2 = inpolygon(a_pair(j,1), a_pair(j,2), x2, y2);
        elseif i == k
            L1 = inpolygon(a_pair(j,1), a_pair(j,2), x1, y1);
            L2 = inpolygon(a_pair(j,1), a_pair(j,2), xv, yv);
        else
            L1 = inpolygon(a_pair(j,1), a_pair(j,2), x1, y1);
            L2 = inpolygon(a_pair(j,1), a_pair(j,2), x2, y2);
        end 
        if ~L1 && L2
            temp(i) = temp(i) + 1;
        end
    end
end
ver = temp / m;
nev = ver - p;
result = true;
for i = 1:k
    if nev > eps2
        result = false;
    end
end

if result
   disp('Тест «равномерность двумерного распределения» выполнен'); 
end

%%
%6) (Критерий Колмогорова-Смирнова используется для проверки гипотезы H_0: "случайная величина X имеет распределение F(x)")
a_sort = sort(new_a);
Dn = 0;
for i=1:n
    tmp = abs((i+1)/n - a_sort(i));
    if tmp > Dn
         Dn = tmp;
    end
end

isExact = true;
if sqrt(n)*Dn >1.63
    isExact = false;
end

if isExact
   disp('Тест Колмогорова-Смирнова выполнен'); 
end
%%
%Graphic arts
figure(3);
hold on;
for i = 1:m
    plot(a_pair(i, 1), a_pair(i, 2), '.');
end
for i = 1:k
    plot(0.5 + r(i)*cosd(0:360), 0.5 + r(i)*sind(0:360), 'black');
    if i == k
        text(0.1, 0.9, string(p(i)));
    else
        text(0.475, r(i)+0.525, string(p(i)));
    end
end
x = [0 0 1 1];
y = [0 1 1 0];
x(end+1) = x(1); 
y(end+1) = y(1); 
plot(x,y),xlim([-0.2 1.2]);
