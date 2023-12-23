from switch import Switch
from enum import Enum
import codecs

Rules = {'Класс': ['Цитрусовые', 'Косточковые', 'Ягоды'],
         'Тип': ['Друпа', 'Клубника', 'Апельсин', 'Банан'],
         'Структура листа': ['Составная', 'Парник', 'С зубчатым краем'],
         'Конфигурация': ['Альтернативная', 'Простая', 'Двудольная'],
         'Яркие цветки': ['Да', 'Нет'],
         'Форма листа': ['Эллиптическая', 'Овальная'],
         'Стебель': ['Нет', 'Короткий', 'Гладкий', 'Ветвистый'],
         'Положение': ['Вертикальное', 'Горизонтальное', 'Кластерное'],
         'Один ствол': ['Да', 'Нет'],
         'Семейство': ['Рутовые', 'Розовые', 'Ягодные']
         }

def switch_menu(arr, kkey):
    options = {0: "-"}  # Добавляем опцию с ключом 0 и значением "-"
    for i in range(len(arr)):
        options[i + 1] = arr[i]  # Используем i+1 в качестве ключа для удобства выбора пользователем
    while True:
        print(f"Выберите опцию для {kkey}:")
        for key, value in options.items():
            print(f"{key}) {value}")
        choice = input()
        if choice.isdigit():
            choice = int(choice)
            if choice in options:
                return options[choice]
        print("Некорректный выбор. Попробуйте снова.")

def rules(num):
    with Switch(num) as rule_num:
        if rule_num(0):
            a, b, c = signs.get("Класс"), signs.get("Структура листа"), signs.get("Конфигурация")
            if a == "Undefined":
                return "Undefined", 0, "Класс"
            if b == "Undefined":
                return "Undefined", 0, "Структура листа"
            if c == "Undefined":
                return "Undefined", 0, "Конфигурация"
            if a == "Цитрусовые" and b == "Составная" and c == "Простая":
                return "True", 0, "Рутовые"
        if rule_num(1):
            a, b, c = signs.get("Класс"), signs.get("Структура листа"), signs.get("Конфигурация")
            if a == "Undefined":
                return "Undefined", 1, "Класс"
            if b == "Undefined":
                return "Undefined", 1, "Структура листа"
            if c == "Undefined":
                return "Undefined", 1, "Конфигурация"
            if a == "Косточковые" and b == "Парник" and c == "Альтернативная":
                return "True", 1, "Розовые"
        if rule_num(2):
            a, b, c = signs.get("Класс"), signs.get("Структура листа"), signs.get("Конфигурация")
            if a == "Undefined":
                return "Undefined", 2, "Класс"
            if b == "Undefined":
                return "Undefined", 2, "Структура листа"
            if c == "Undefined":
                return "Undefined", 2, "Конфигурация"
            if a == "Ягоды" and b == "С зубчатым краем" and c == "Простая":
                return "True", 2, "Ягодные"
        if rule_num(3):
            a, b, c = signs.get("Тип"), signs.get("Форма листа"), signs.get("Яркие цветки")
            if a == "Undefined":
                return "Undefined", 3, "Тип"
            if b == "Undefined":
                return "Undefined", 3, "Форма листа"
            if c == "Undefined":
                return "Undefined", 3, "Яркие цветки"
            if a == "Клубника" and b == "С зубчатым краем" and c == "Нет":
                return "True", 3, "Ягоды"
        if rule_num(4):
            a, b = signs.get("Тип"), signs.get("Форма листа")
            if a == "Undefined":
                return "Undefined", 4, "Тип"
            if b == "Undefined":
                return "Undefined", 4, "Форма листа"
            if a == "Друпа" and b == "Овальная":
                return "True", 4, "Косточковые"
        if rule_num(5):
            a, b = signs.get("Тип"), signs.get("Форма листа")
            if a == "Undefined":
                return "Undefined", 5, "Тип"
            if b == "Undefined":
                return "Undefined", 5, "Форма листа"
            if a == "Апельсин" and b == "Эллиптическая":
                return "True", 5, "Цитрусовые"
        if rule_num(6):
            a = signs.get("Стебель")
            if a == "Undefined":
                return "Undefined", 6, "Стебель"
            if a == "Нет":
                return "True", 6, "Друпа"
        if rule_num(7):
            a, b = signs.get("Стебель"), signs.get("Положение")
            if a == "Undefined":
                return "Undefined", 7, "Стебель"
            if b == "Undefined":
                return "Undefined", 7, "Положение"
            if a == "Короткий" and b == "Вертикальное":
                return "True", 7, "Апельсин"
        if rule_num(8):
            a, b, c = signs.get("Стебель"), signs.get("Положение"), signs.get("Один ствол")
            if a == "Undefined":
                return "Undefined", 8, "Стебель"
            if b == "Undefined":
                return "Undefined", 8, "Положение"
            if c == "Undefined":
                return "Undefined", 8, "Один ствол"
            if a == "Гладкий" and b == "Горизонтальное" and c == "Да":
                return "True", 8, "Клубника"
        if rule_num(9):
            a, b, c = signs.get("Стебель"), signs.get("Положение"), signs.get("Один ствол")
            if a == "Undefined":
                return "Undefined", 9, "Стебель"
            if b == "Undefined":
                return "Undefined", 9, "Положение"
            if c == "Undefined":
                return "Undefined", 9, "Один ствол"
            if a == "Ветвистый" and b == "Кластерное" and c == "Нет":
                return "True", 9, "Банан"
    return "False", num, -1

with (codecs.open("log_plants.txt", "w", "utf-16")) as file:
    aim_stack_num = [-1]
    context_stack = []
    rules_list = ['Семейство','Семейство','Семейство','Класс','Класс','Класс','Тип','Тип','Тип','Тип']
    unused_rules = [0] * len(rules_list)
    signs = {key: "Undefined" for key in Rules.keys()}

    flag = False

    test = ""
    print("Что вы желаете определить?")
    print("1) Класс")
    print("2) Тип")
    print("3) Семейство")
    n = int(input())
    with Switch(n) as case:
        if case(1):
            test = "Класс"
        if case(2):
            test = "Тип"
        if case(3):
            test = "Семейство"
    aim_stack = [test]
    counter = 0
    while not flag:
        file.write(str(counter) + "\n")
        rule_id = -1
        for i in range(len(rules_list)):
            if unused_rules[i] == 0 and rules_list[i] == aim_stack[len(aim_stack) - 1]:
                rule_id = i
                break
        if rule_id != -1:
            result = rules(rule_id)
            with Switch(result[0]) as case:
                if case("True"):
                    context_stack.append((rules_list[rule_id], result[1], result[2]))
                    signs[rules_list[rule_id]] = result[2]
                    aim_stack.pop()
                    aim_stack_num.pop()
                    r = str(rule_id + 1)
                    file.write("Правило номер " + r + " принято\n")
                    file.write("В контекстны стек занесено " + rules_list[rule_id] + ": " + result[2] + "\n")
                    if len(aim_stack) == 0:
                        flag = True
                if case("False"):
                    unused_rules[rule_id] = 1
                    r = str(rule_id + 1)
                    file.write("Правило номер " + r + " отброшено\n")
                if case("Undefined"):
                    aim_stack_num.append(result[1])
                    aim_stack.append(result[2])
                    r = str(rule_id + 1)
                    file.write("Правило номер " + r + ": неизвестный параметр - " + result[2] + "\n")
        else:
            if aim_stack[len(aim_stack) - 1] == test:
                flag = True
            else:
                print("Дополните информацию(Если она не важна, выберите символ '-')")
                in_str = switch_menu(Rules.get(aim_stack[len(aim_stack) - 1]), aim_stack[len(aim_stack) - 1])
                if in_str != '-':
                    context_stack.append((aim_stack[len(aim_stack) - 1], -1, in_str))
                    signs[aim_stack[len(aim_stack) - 1]] = in_str
                    file.write("Признак " + aim_stack[len(aim_stack) - 1] + " = " + in_str + " получен из ответа пользователя\n")
                    aim_stack.pop()
                    aim_stack_num.pop()
                else:
                    signs[aim_stack[len(aim_stack) - 1]] = "-"
                    file.write(aim_stack[len(aim_stack) - 1] + " - не важно для конечного ответа\n")
                    aim_stack.pop()
                    aim_stack_num.pop()
                if len(aim_stack) == 0:
                    flag = True
        file.write("☺" * 50 + "\n")
        counter += 1
    res = context_stack[len(context_stack) - 1]
    if res[0] == test and res[1] != -1:
        print("Ответ получен!")
        print(context_stack[len(context_stack) - 1][2])
        file.write("Ответ получен!\n")
        file.write(context_stack[len(context_stack) - 1][0] + ": " + context_stack[len(context_stack) - 1][2] + "\n")
    else:
        print("Ответ не может быть получен!")
        file.write("Ответ не может быть получен!\n")
