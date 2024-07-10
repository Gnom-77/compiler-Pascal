using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Компилятор
{
    class SyntaxAnalyzer
    {
        public static List<string> variables = new List<string>(); // Список для хранения имен переменных

        // Метод для проверки является ли тикущий символ подходящим по коду, иначе выводит ошибку по коду, которвый поступал в качестве параметра
        static void accept(byte symbolexpected)
        {
            if (LexicalAnalyzer.symbol == symbolexpected)
            {
                InputOutput.PrintSymbolCodeInFile();
                LexicalAnalyzer.NextSym();
            }
            else
                InputOutput.Error(symbolexpected, LexicalAnalyzer.token);
        }

        public static bool flag = false; // Булевая переменная для проверки оператора присваивания.


        // Оператор присваивания(идём по символам и елси они входят в список переменных то программа выполняется успешно)
        public static void ToVariables()
        {
            // Проверка следующего символа
            accept(LexicalAnalyzer.str);
            // Проверка, после названия переменной должно стоять ':='
            if(LexicalAnalyzer.symbol == LexicalAnalyzer.assign)
            {
                // Значению flag присваиваем false, для того, что бы не считывать каждую строку, как начало оператора присваивания, после чего проверяем конструкцию
                bool neytraz = false;
                flag = true;
                accept(LexicalAnalyzer.assign); // Переход к присваиванию
                // Пока не встретится символ ';' проверям оператор присвивания, на содержания в нём различных действий
                while(LexicalAnalyzer.symbol != LexicalAnalyzer.semicolon)
                {
                    neytraz = false; // Переменная нейтрализации ошибки, для проверки правильного оператора присваивания. Если было какое то математическое действие
                    // И следующий знак был ';', то присваивание выполнено не верно

                    // Ожидаем что текущий символ является строкой, то есть переменной. После переменной могут быть раличные математические действия
                    // Проверям соответствует ли следующий символ математической операции, и просто пропускаем математическое действие
                    if (LexicalAnalyzer.symbol == LexicalAnalyzer.str && variables.Contains(LexicalAnalyzer.name) == true)
                    {
                        neytraz = true;
                        accept(LexicalAnalyzer.str); 
                        if (LexicalAnalyzer.symbol == LexicalAnalyzer.plus || LexicalAnalyzer.symbol == LexicalAnalyzer.minus ||
                            LexicalAnalyzer.symbol == LexicalAnalyzer.star || LexicalAnalyzer.symbol == LexicalAnalyzer.slash ||
                            LexicalAnalyzer.symbol == LexicalAnalyzer.modsy || LexicalAnalyzer.symbol == LexicalAnalyzer.divsy)
                        {
                            InputOutput.PrintSymbolCodeInFile();
                            LexicalAnalyzer.NextSym();
                        }
                        
                    }
                    // Ожидаем что текущий символ является значением одного из стандартных типов данных. После переменной могут быть раличные математические действия
                    // Проверям соответствует ли следующий символ математической операции, и просто пропускаем математическое действие
                    if (LexicalAnalyzer.symbol == LexicalAnalyzer.floatc || LexicalAnalyzer.symbol == LexicalAnalyzer.intc 
                        || LexicalAnalyzer.symbol == LexicalAnalyzer.chars || LexicalAnalyzer.symbol == LexicalAnalyzer.boolc)
                    {
                        neytraz = true;
                        InputOutput.PrintSymbolCodeInFile();
                        LexicalAnalyzer.NextSym();
                        if (LexicalAnalyzer.symbol == LexicalAnalyzer.plus || LexicalAnalyzer.symbol == LexicalAnalyzer.minus ||
                            LexicalAnalyzer.symbol == LexicalAnalyzer.star || LexicalAnalyzer.symbol == LexicalAnalyzer.slash ||
                            LexicalAnalyzer.symbol == LexicalAnalyzer.modsy || LexicalAnalyzer.symbol == LexicalAnalyzer.divsy)
                        {
                            InputOutput.PrintSymbolCodeInFile();
                            LexicalAnalyzer.NextSym();
                        }
                    }
                    if(neytraz == false) // Если нейтрализация не изменилась в значении, то выводим ошибку. Оператор присваивания составлен не верно
                    {
                        InputOutput.PrintSymbolCodeInFile();
                        InputOutput.Error(1, LexicalAnalyzer.token);
                        LexicalAnalyzer.NextSym();
                    }
                    
                }
                accept(LexicalAnalyzer.semicolon);
                flag = false;
            }
        }

        public static Dictionary<string,int> set_of_procedures = new Dictionary<string, int>(); //Словарь для набора процедур, имя процедуры и сколько параметров в неё передаётся
        static int count; //счётчик для колличества парраметров в процедура

        // Оператор вызова процедуры
        public static void procedure_call()
        {
            int count_Variables = 0; // Счётчик передаваемых переменных
            int param = 0; // Переменная в которой будет хранится колличество параметров, вызываемой процедуры
            set_of_procedures.TryGetValue(LexicalAnalyzer.name, out param); // Обращаемся к словарю по имени процедуры, в качестве ключа и записываем в param колличество параметров
            accept(LexicalAnalyzer.str);
            if(LexicalAnalyzer.symbol == LexicalAnalyzer.semicolon) // Если вызываем процедуру без параметров, то после имени процедуры идёт знак ';' или конструкция '();' что проверяется дальше
            {
                if(param == 0)
                {
                    accept(LexicalAnalyzer.semicolon);
                }
                else
                {
                    InputOutput.Error(77, LexicalAnalyzer.token);
                }
            }
            if(LexicalAnalyzer.symbol == LexicalAnalyzer.leftpar)
            {
                accept(LexicalAnalyzer.leftpar);
                if(LexicalAnalyzer.symbol == LexicalAnalyzer.rightpar)
                {
                    // Проверка вызова процедуры, в которую не передаются значения, концтрукции "{имя процедуры}();"
                    if (param == 0)
                    {
                        accept(LexicalAnalyzer.rightpar);
                        accept(LexicalAnalyzer.semicolon);
                    }
                    else
                    {
                        InputOutput.Error(123, LexicalAnalyzer.token);
                    }
                }
                // Иначе считаем колличество значений по которым вызывается процедура и сравниваем их с param
                else
                {
                    count_Variables = 1; // Счётчик значений при вызове процедуры, если мы дошли до этого этапа, как минимум одна переменная уже передаётся
                    bool proced_bool = false;
                    // Счёт колличества значений в процедуре, если конструкция оператора вызова не соблюдается, выводится ошибка
                    while (LexicalAnalyzer.symbol != LexicalAnalyzer.rightpar && proced_bool == false)
                    {
                        InputOutput.PrintSymbolCodeInFile();
                        LexicalAnalyzer.NextSym();

                        if (LexicalAnalyzer.symbol == LexicalAnalyzer.comma || LexicalAnalyzer.symbol == LexicalAnalyzer.rightpar)
                        {
                            count_Variables++;
                        }
                        else if(LexicalAnalyzer.symbol != LexicalAnalyzer.comma || LexicalAnalyzer.symbol != LexicalAnalyzer.rightpar)
                        {
                            InputOutput.PrintSymbolCodeInFile();
                            InputOutput.Error(121, LexicalAnalyzer.token);
                            proced_bool = true;
                        }
                        if(LexicalAnalyzer.symbol != LexicalAnalyzer.rightpar)
                        {
                            InputOutput.PrintSymbolCodeInFile();
                            LexicalAnalyzer.NextSym();
                        }
                    }
                    // Сравниваем счёт параметров процедуры и колличество передаваемых переменных при вызове, если оно не совпадает, выводим ошибку
                    if (param == count_Variables)
                    {
                        accept(LexicalAnalyzer.rightpar);
                        accept(LexicalAnalyzer.semicolon);
                    }
                    else
                    {
                        InputOutput.Error(77, LexicalAnalyzer.token);
                    }   

                }
            }
        }

        //Описания переменных простых типов (real, char, integer, boolean), функция вызывается при встрече ключевого слова var
        public static void varpart()
        {
            // Переходим к следующему коду, после var
            accept(LexicalAnalyzer.varsy);
            do
            {
                variables.Add(LexicalAnalyzer.name); // Запись переменной в список, с переменными
                accept(LexicalAnalyzer.str);
                // Пока идёт перечесление переменных, пропускать их через ','
                // Если встречается ':' то идёт проверка на существующй тип данных, после чего идёт проверка дальше
                if (LexicalAnalyzer.symbol == LexicalAnalyzer.colon)
                {
                    accept(LexicalAnalyzer.colon);
                    if (LexicalAnalyzer.symbol == LexicalAnalyzer.realsy || LexicalAnalyzer.symbol == LexicalAnalyzer.integersy ||
                    LexicalAnalyzer.symbol == LexicalAnalyzer.boolsy || LexicalAnalyzer.symbol == LexicalAnalyzer.charsy)
                    {
                        InputOutput.PrintSymbolCodeInFile();
                        LexicalAnalyzer.NextSym();
                        accept(LexicalAnalyzer.semicolon);
                    }
                    else
                    {
                        InputOutput.Error(79, LexicalAnalyzer.token);
                        LexicalAnalyzer.NextSym();
                    }
                }
                else
                {
                    accept(LexicalAnalyzer.comma);
                }
            }
            while (LexicalAnalyzer.symbol != LexicalAnalyzer.procedurensy && LexicalAnalyzer.symbol != LexicalAnalyzer.functionsy &&
            LexicalAnalyzer.symbol != LexicalAnalyzer.beginsy && LexicalAnalyzer.symbol != LexicalAnalyzer.varsy);
            // Выход из цикла осуществляется если содержатся ключевые слова, которые могут находится после описания переменной
        }

        // Метод, что бы найти end, идём по всем словам, пока не встретим ключевое слово end, затем после него далжна находится ';' иначе выведется ошибка
        public static void BeginToEnd()
        {
            accept(LexicalAnalyzer.beginsy);
            while(LexicalAnalyzer.symbol != LexicalAnalyzer.endsy)
            {
                InputOutput.PrintSymbolCodeInFile();
                LexicalAnalyzer.NextSym();
            }
            accept(LexicalAnalyzer.endsy);
            if(LexicalAnalyzer.symbol == LexicalAnalyzer.semicolon )
                accept(LexicalAnalyzer.semicolon);

        }

        // Описание процедуры. Функция вызывается если в программе встречается ключевое слово "procedure"
        public static bool selector = false; // Переменная для проверки параметров процедуры. Если блок с параметрами пройдет, то присваиваем false и не заходим в последующие проверки
        // Процедура может быть нескольких типов 
        // procedure {имя процедуры}(); begin ... end;
        // procedure {имя процедуры}; begin ... end;
        // procedure {имя процедуры}(a,b,c:integer; var max: integer; var rr: real); begin ... end;
        // procedure {имя процедуры}(a,b,c:integer); begin ... end;
        public static void procedurepart()
        {
            count = 0; // Счётчик для колличества параметров в процедуре
            selector = true;
            accept(LexicalAnalyzer.procedurensy);
            string name_of_procedure = LexicalAnalyzer.name;
            accept(LexicalAnalyzer.str);
            // Проверка описания вида procedure {имя процедуры}; begin ... end;
            if (LexicalAnalyzer.symbol == LexicalAnalyzer.semicolon)
            {
                accept(LexicalAnalyzer.semicolon);
                selector = false;
                BeginToEnd();
            }
            if(LexicalAnalyzer.symbol == LexicalAnalyzer.leftpar)
            {
                // Проверка описания вида procedure {имя процедуры}(); begin ... end;
                InputOutput.PrintSymbolCodeInFile();
                LexicalAnalyzer.NextSym();
                if(LexicalAnalyzer.symbol == LexicalAnalyzer.rightpar)
                {
                    InputOutput.PrintSymbolCodeInFile();
                    LexicalAnalyzer.NextSym();
                    accept(LexicalAnalyzer.semicolon);
                    selector = false;
                    BeginToEnd();
                }
                // Проверяется описания процедуры вида
                // procedure {имя процедуры}(a,b,c:integer; var max: integer; var rr: real); begin ... end;
                // и
                // procedure {имя процедуры}(a,b,c:integer); begin ... end;
                // Каждый раз, если часть процедуры до блока проверена, вызывается функция BeginToEnd(); которая ищет выход из процедуры 
                else
                {
                    if (LexicalAnalyzer.symbol == LexicalAnalyzer.str)
                    {
                        do
                        {
                            variables.Add(LexicalAnalyzer.name);
                            accept(LexicalAnalyzer.str);
                            count++;
                            if (LexicalAnalyzer.symbol == LexicalAnalyzer.colon)
                            {
                                accept(LexicalAnalyzer.colon);
                                if (LexicalAnalyzer.symbol == LexicalAnalyzer.realsy || LexicalAnalyzer.symbol == LexicalAnalyzer.integersy ||
                                LexicalAnalyzer.symbol == LexicalAnalyzer.boolsy || LexicalAnalyzer.symbol == LexicalAnalyzer.charsy)
                                {
                                    InputOutput.PrintSymbolCodeInFile();
                                    LexicalAnalyzer.NextSym();
                                    if (LexicalAnalyzer.symbol != LexicalAnalyzer.rightpar)
                                        accept(LexicalAnalyzer.semicolon);
                                }
                                else
                                {
                                    InputOutput.Error(79, LexicalAnalyzer.token);
                                    LexicalAnalyzer.NextSym();
                                }
                            }
                            else
                            {
                                accept(LexicalAnalyzer.comma);
                            }
                        }
                        while (LexicalAnalyzer.symbol != LexicalAnalyzer.rightpar && LexicalAnalyzer.symbol != LexicalAnalyzer.varsy);

                        if(LexicalAnalyzer.symbol == LexicalAnalyzer.varsy)
                        {
                            accept(LexicalAnalyzer.varsy);
                            do
                            {
                                variables.Add(LexicalAnalyzer.name);
                                accept(LexicalAnalyzer.str);
                                count++;
                                if (LexicalAnalyzer.symbol == LexicalAnalyzer.colon)
                                {
                                    accept(LexicalAnalyzer.colon);
                                    if (LexicalAnalyzer.symbol == LexicalAnalyzer.realsy || LexicalAnalyzer.symbol == LexicalAnalyzer.integersy ||
                                    LexicalAnalyzer.symbol == LexicalAnalyzer.boolsy || LexicalAnalyzer.symbol == LexicalAnalyzer.charsy)
                                    {
                                        InputOutput.PrintSymbolCodeInFile();
                                        LexicalAnalyzer.NextSym();
                                        if(LexicalAnalyzer.symbol != LexicalAnalyzer.rightpar)
                                            accept(LexicalAnalyzer.semicolon);
                                    }
                                    else
                                    {
                                        InputOutput.Error(79, LexicalAnalyzer.token);
                                        LexicalAnalyzer.NextSym();
                                    }
                                }
                                else
                                {
                                    accept(LexicalAnalyzer.comma);
                                }
                                if(LexicalAnalyzer.symbol == LexicalAnalyzer.varsy)
                                    accept(LexicalAnalyzer.varsy);
                            }
                            while (LexicalAnalyzer.symbol != LexicalAnalyzer.rightpar);

                        }
                        if (LexicalAnalyzer.symbol == LexicalAnalyzer.rightpar)
                        {
                            accept(LexicalAnalyzer.rightpar);
                            accept(LexicalAnalyzer.semicolon);
                            selector = false;
                            BeginToEnd();
                        }
                    }
                }
            }
            if (LexicalAnalyzer.symbol == LexicalAnalyzer.semicolon)
            {
                selector = false;
                BeginToEnd();
            }
            set_of_procedures.Add(name_of_procedure, count); // Запись имени процедуры и колличества параметров в ней в словарь, где ключём является имя процедуры
        }


    }
}
