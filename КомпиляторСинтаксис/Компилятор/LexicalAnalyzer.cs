using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Компилятор
{
    class LexicalAnalyzer
    {
        public const byte
        quotes = 2, // "
        quo = 3, // '
        star = 21, // *
        slash = 60, // /
        equal = 16, // =
        comma = 20, // ,
        semicolon = 14, // ;
        colon = 5, // :
        point = 61, // .
        arrow = 62, // ^
        leftpar = 9,    // (
        rightpar = 4,   // )
        lbracket = 11,  // [
        rbracket = 12,  // ]
        flpar = 63, // {
        frpar = 64, // }
        later = 65, // <
        greater = 66,   // >
        laterequal = 67,    //  <=
        greaterequal = 68,  //  >=
        latergreater = 69,  //  <>
        plus = 70,  // +
        minus = 71, // –
        lcomment = 72,  //  (*
        rcomment = 73,  //  *)
        assign = 51,    //  :=
        twopoints = 74, //  ..
        ident = 2,  // идентификатор
        floatc = 82,    // вещественная константа
        intc = 15,  // целая константа
        boolc = 16,
        chars = 83, // символьная константа
        str = 84, // текст
        forbidden = 6, // Код запрещённого символа
        integersy = 200,
        stringsy = 201,
        charsy = 203,
        boolsy = 204,
        realsy = 205,
        Trueis = 190,
        Falseis = 191,
        casesy = 31,
        elsesy = 32,
        filesy = 57,
        gotosy = 33,
        thensy = 52,
        typesy = 34,
        untilsy = 53,
        dosy = 54,
        withsy = 37,
        ifsy = 56,
        insy = 100,
        ofsy = 101,
        orsy = 102,
        tosy = 103,
        endsy = 104,
        varsy = 105,
        divsy = 106,
        andsy = 107,
        notsy = 108,
        forsy = 109,
        modsy = 110,
        nilsy = 111,
        setsy = 112,
        beginsy = 113,
        whilesy = 114,
        arraysy = 115,
        constsy = 116,
        labelsy = 117,
        downtosy = 118,
        packedsy = 119,
        recordsy = 120,
        repeatsy = 121,
        programsy = 122,
        functionsy = 123,
        procedurensy = 124;

        public static byte symbol; // код символа
        public static TextPosition token; // позиция символа
        static int nmb_int; // значение целой константы
        float nmb_float; // значение вещественной константы
        char one_symbol; // значение символьной константы
        public static string name;

        // Статический метод, который берёт первый символ в строке
        // И передвигается по ней, присваивая кода символам,
        // Если это слово, то он проверяет его в словаре и присваивает
        // Соответствующий код.
        // Так же метод обнаруживает ошибку переполнения int 
        // И символы которые компилятор не может прочитать
        public static byte NextSym()
        {
            symbol = 0;
            while (InputOutput.Ch == ' ') InputOutput.NextCh();
            token.lineNumber = InputOutput.positionNow.lineNumber;
            token.charNumber = InputOutput.positionNow.charNumber;
            switch (InputOutput.Ch)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    // Проверяет число в строке и определяет привышает ли число предел
                    byte digit;
                    Int16 maxint = Int16.MaxValue;
                    nmb_int = 0;
                    while (InputOutput.Ch >= '0' && InputOutput.Ch <= '9')
                    {
                        digit = (byte)(InputOutput.Ch - '0');
                        if (nmb_int < maxint / 10 ||
                        (nmb_int == maxint / 10 &&
                        digit <= maxint % 10))
                            nmb_int = 10 * nmb_int + digit;
                        else
                        {
                            // константа превышает предел
                            InputOutput.Error(203, InputOutput.positionNow);
                            nmb_int = 0;
                            while (InputOutput.Ch >= '0' && InputOutput.Ch <= '9') InputOutput.NextCh();
                        }
                        InputOutput.NextCh();
                    }
                    // С плавающей запятой число или целое
                    if (InputOutput.Ch == '.')
                    {
                        InputOutput.NextCh();
                        while (InputOutput.Ch >= '0' && InputOutput.Ch <= '9')
                        {
                            InputOutput.NextCh();
                        }
                        symbol = floatc;
                        break;
                    }
                    else
                        symbol = intc;
                    break;
                case '<':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '=')
                    {
                        symbol = laterequal; InputOutput.NextCh();
                    }
                    else
                     if (InputOutput.Ch == '>')
                    {
                        symbol = latergreater; InputOutput.NextCh();
                    }
                    else
                        symbol = later;
                    break;
                case '>':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '=')
                    {
                        symbol = greaterequal; InputOutput.NextCh();
                    }
                    else
                        symbol = greater;
                    break;
                case ':':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '=')
                    {
                        symbol = assign; InputOutput.NextCh();
                    }
                    else
                        symbol = colon;
                    break;
                case ';':
                    symbol = semicolon; InputOutput.NextCh();
                    break;
                case '"':
                    InputOutput.NextCh();
                    symbol = quotes;
                    if (InputOutput.Ch != '"')
                    {
                        while (InputOutput.Ch != '"')
                        {
                            InputOutput.NextCh();
                        }
                        InputOutput._quotes();
                        InputOutput.NextCh();
                        break;
                    }
                    else
                    {
                        InputOutput._quotes2();
                        InputOutput.NextCh();
                        break;
                    }
                case '\u0027':
                    InputOutput.NextCh();
                    symbol = quo;
                    if (InputOutput.Ch != '\u0027')
                    {
                        InputOutput.NextCh();
                        if (InputOutput.Ch == '\u0027')
                        {
                            InputOutput._quo();
                            InputOutput.NextCh();
                            break;
                        }
                        else
                        {
                            InputOutput.Error(201, InputOutput.positionNow);
                            InputOutput.NextCh();
                            break;
                        }
                    }
                    else
                    {
                        InputOutput.Error(201, InputOutput.positionNow);
                        InputOutput._quo2();
                        InputOutput.NextCh();
                        break;
                    }
                    
                    
                case '.':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '.')
                    {
                        symbol = twopoints; InputOutput.NextCh();
                    }
                    else symbol = point;
                    break;
                case '*':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == ')')
                    {
                        symbol = rcomment;
                    }
                    else symbol = star;
                    InputOutput.NextCh();
                    break;
                case '/':
                    symbol = slash;
                    InputOutput.NextCh();
                    break;
                case '=':
                    symbol = equal;
                    InputOutput.NextCh();
                    break;
                case ',':
                    symbol = comma;
                    InputOutput.NextCh();
                    break;
                case '^':
                    symbol = arrow;
                    InputOutput.NextCh();
                    break;
                case '(':
                    if(InputOutput.Ch == '*')
                    {
                        symbol = lcomment;
                    }
                    else symbol = leftpar;
                    InputOutput.NextCh();
                    break;
                case ')':
                    symbol = rightpar;
                    InputOutput.NextCh();
                    break;
                case '[':
                    symbol = lbracket;
                    InputOutput.NextCh();
                    break;
                case ']':
                    symbol = rbracket;
                    InputOutput.NextCh();
                    break;
                case '{':
                    symbol = flpar;
                    while(InputOutput.Ch != '}')
                    {
                        InputOutput.NextCh();
                    }
                    break;
                case '}':
                    symbol = frpar;
                    InputOutput.NextCh();
                    break;
                case '+':
                    symbol = plus;
                    InputOutput.NextCh();
                    break;
                case '-':
                    symbol = minus;
                    InputOutput.NextCh();
                    break;
                // default используется, если символ не подошёл ни к одному символу из списка
                // поэтому это может быть строка или ключевое слово, в default происходит проверка и определяется код
                default:
                    // Считываем набор символов не разделённый пробелом 
                    // И присваиваем наше слово к переменной name.
                    name = "";
                    while ((InputOutput.Ch >= 'a' && InputOutput.Ch <= 'z') ||
                            (InputOutput.Ch >= 'A' && InputOutput.Ch <= 'Z') ||
                            (InputOutput.Ch >= '0' && InputOutput.Ch <= '9'))
                    {
                        name += InputOutput.Ch;
                        InputOutput.NextCh();
                    }
                    // Приводим слово к нижнему регистру, для удобной обработки слова и определения кода.
                    name = name.ToLower();
                    // Console.WriteLine("///" + name + "///"); Это просто для проверки, аля Debug, не убирать -_- 
                    // Создаём класс Keywords, в котором хронится словарь словарей О_О ключевых слов.
                    Keywords keywords = new Keywords(); 
                    // Массив символов, которые может читать компилятор.
                    char[] check_symbol = { '1', '2', '3', '4', '5', '6', '7', '8', '9','0',
                    'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o','p', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l','z',
                    'x', 'c', 'v', 'b', 'n', 'm', ',', '.', '/',']', '[', '}', '{', '(', ')', '*', '^', ':', ';','<'
                    ,'>', '+', '-', '=',  ' ', '\u0027', '"'};
                    bool check = false;
                    // Проверка на то, является ли слово ключевым, или просто слово.
                    // Если содержатся нечитаемые символы, выводится ошибка.
                    if (keywords.Kw.ContainsKey(Convert.ToByte(name.Length)))
                    {
                        if (keywords.code_search(name) != 0)
                        {
                            symbol = keywords.code_search(name);
                            // Вызов описания переменных
                            if (symbol == varsy && SyntaxAnalyzer.selector == false)
                            {
                                SyntaxAnalyzer.varpart();
                            }
                            // Вызов описания процедуры
                            if (symbol == procedurensy)
                            {
                                SyntaxAnalyzer.procedurepart();
                            }


                            break;
                        }
                        else
                        {
                            for (int i = 0; i < name.Length; i++)
                            {
                                for (int j = 0; j < check_symbol.Length; j++)
                                {
                                    if (name[i] == check_symbol[j])
                                        check = true;
                                }
                            }
                            if (check == true)
                            {
                                symbol = str;
                                if (SyntaxAnalyzer.variables.Contains(name) && SyntaxAnalyzer.flag == false)
                                {
                                    SyntaxAnalyzer.ToVariables();
                                }
                                else
                                {
                                    if(SyntaxAnalyzer.set_of_procedures.ContainsKey(name))
                                    {
                                        SyntaxAnalyzer.procedure_call();
                                    }
                                }
                                break;
                            }
                            // Вывод ошибки
                            else
                            {
                                symbol = forbidden;
                                InputOutput.Error(204, InputOutput.positionNow);
                                InputOutput.NextCh();
                                break;
                            }
                        }
                        
                    }
                    // Если в слове больше символов чем значение ключей словаря, то определяется, 
                    // Содержатся там запрещённые символы или нет.
                    else
                    {
                        for (int i = 0; i < name.Length; i++)
                        {
                            for (int j = 0; j < check_symbol.Length; j++)
                            {
                                if (name[i] == check_symbol[j])
                                    check = true;
                            }
                        }
                        if(check == true)
                        {
                            symbol = str;
                            if (SyntaxAnalyzer.variables.Contains(name) && SyntaxAnalyzer.flag == false)
                            {
                                SyntaxAnalyzer.ToVariables();
                            }
                            else
                            {
                                if (SyntaxAnalyzer.set_of_procedures.ContainsKey(name))
                                {
                                    SyntaxAnalyzer.procedure_call();
                                }
                            }
                            break;
                        }
                        else
                        {
                            symbol = forbidden;
                            InputOutput.Error(204, InputOutput.positionNow);
                            InputOutput.NextCh();
                            break;
                        }
                    }
            }
            // Подсчёт ключевых слов end и begin для составного оператора
            if(symbol == beginsy && InputOutput.door_for_begin == true && InputOutput.permission == true)
            {
                InputOutput.begin_score++;
                InputOutput.door_for_begin = false;
            }
            if(symbol != beginsy)
            {
                InputOutput.door_for_begin = true;
            }
            if (symbol == endsy && InputOutput.door_for_end == true && InputOutput.permission == true)
            {
                InputOutput.end_score++;
                InputOutput.door_for_end = false;
            }
            if (symbol != endsy)
            {
                InputOutput.door_for_end = true;
            }
            return symbol;
        }

    }
}


