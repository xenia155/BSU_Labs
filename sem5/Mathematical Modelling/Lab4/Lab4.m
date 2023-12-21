% Одномерный интеграл

a= 3;
b = 3.5;
N = 1000;

x = unifrnd(a,b,N);

y = (x.^x).*(1+log(x)).*tan(x);

fprintf('calculating \n \n \n');
result =(sum(y(:,1))* (b -a))/N;

fprintf('our integral = \n');
disp (result);

syms f
func = @(f) (f.^f).*(1+log(f)).*tan(f);

realResult = integral(func, a,b);
fprintf('real integral = \n');

disp(realResult);

% Двумерный интеграл

N = [1 1000];

ksi1 = unifrnd(0, 1, N(2));
ksi2 = unifrnd(0, 1, N(2));

x1 = -2 + 4 * ksi1;
x2 = 0 + 4 * ksi2;

idx = x2 >= x1.^2;
x2 = x2(idx);
x1 = x1(idx);

size = length(x1);

ytemp = sqrt(x2 + sin(x1).^2);
expVector = exp(x1 .* x2);
y1 = ytemp .* expVector;

resultTemp = sum(y1);

result = (resultTemp * (32/3)) / size;

fprintf('Monte Carlo integral = %.4f\n', result);

% Точное значение интеграла
syms x y;
exactResult = int(int(sqrt(y + sin(x)^2) * exp(x * y), x, -2, 2), y, 0, 4);
fprintf('Exact integral = %.4f\n', double(exactResult));



