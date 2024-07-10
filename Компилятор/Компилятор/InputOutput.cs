using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Компилятор
{
    
    struct TextPosition
    {
        public uint lineNumber; // номер строки
        public byte charNumber; // номер позиции в строке

        // Позиция символа, по строке и номеру позиции в строке
        public TextPosition(uint ln = 0, byte c = 0)
        {
            lineNumber = ln;
            charNumber = c;
        }
    }

    // Если найдена ошибка, положения символа, в котором вызвана ошибка и код ошибки.
    struct Err
    {
        public TextPosition errorPosition;
        public byte errorCode;

        public Err(TextPosition errorPosition, byte errorCode)
        {
            this.errorPosition = errorPosition;
            this.errorCode = errorCode;
        }
    }

    class InputOutput
    {
        public static int count_quotes = 0;
        const byte ERRMAX = 9;
        static StreamReader File { get; set; }
        public static TextPosition positionNow = new TextPosition();
        static string line;
        static byte lastInLine = 0;
        static bool permission = true;
        public static List<Err> err;
        static uint errCount = 0;
        static StreamWriter export = new StreamWriter("Export.txt", false); // Создание файла для записи проанализированного кода
        static StreamWriter character_codes = new StreamWriter("Codes.txt", false); // Файл в который записываем последовательность кодов
        public static char Ch { get; set; }

        //Методы для обработки символа '
        public static void _quo()
        {
            character_codes.Write(LexicalAnalyzer.quo + " " + LexicalAnalyzer.chars + " ");
        }
        public static void _quo2()
        {
            character_codes.Write(LexicalAnalyzer.quo + " ");
        }

        //Методы для обработки символа "
        public static void _quotes2()
        {
            character_codes.Write(LexicalAnalyzer.quotes + " ");
        }
        public static void _quotes()
        {
            character_codes.Write(LexicalAnalyzer.quotes + " " + LexicalAnalyzer.str + " ");
        }
        // Метод который вызывается при запуске программы,
        // Происходит считка из файла и запись в два других файла
        // Так же идёт проверка, на коментарии в программе, файл правда 2 раза открывается и это ужасно((
        public static void analysis()
        {
            string fpar = "";
            string fpar_2 = "";
            int count = 0;
            
            StreamReader File_beta;
            try
            {
                // Файл с которого считываем
                File_beta = new StreamReader("Input.txt");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            while(!File_beta.EndOfStream)
            {
                fpar = File_beta.ReadLine();
                for (int i = 0; i < fpar.Length; i++)
                {
                    if (fpar[i] == '{' || fpar[i] == '}')
                    {
                        fpar_2 += fpar[i];
                    }
                    if (fpar[i] == '"')
                    {
                        count_quotes += 1;
                    }
                }
            }
            for (int i = 0; i < fpar_2.Length - 1; i++)
            {
                if (fpar_2[i] == '{' && fpar_2[i + 1] == '}')
                {
                    count += 1;
                }
            }
            File_beta.Close();
            if ((fpar_2.Length == count * 2) && (count_quotes % 2 == 0))
            {
                try
                {
                    // Файл с которого считываем
                    File = new StreamReader("Input.txt");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
                positionNow = new TextPosition();
                ReadNextLine();


                Ch = line[0];
                // Запись кода в файл
                while (permission)
                {
                    byte print_code_symbol = LexicalAnalyzer.NextSym();
                    if (permission && (print_code_symbol != 0))
                    {
                        character_codes.Write(print_code_symbol + " ");
                    }
                }
                export.Close();
                character_codes.Close();
                File.Close();
            }
            else
            {
                if (fpar_2.Length != count * 2)
                {
                    Console.WriteLine("Ошибка компиляции, не верно введён коментарий");
                    export.WriteLine("Ошибка компиляции, не верно введён коментарий");
                    
                }
                if (count_quotes % 2 != 0)
                {
                    Console.WriteLine("Ошибка компиляции, не верно введено значение типа string");
                    export.WriteLine("Ошибка компиляции, не верно введено значение типа string");
                }
                export.Close();
            }


           
        }

        // Переход к следующему символу.
        static public void NextCh()
        {
            
            if (positionNow.charNumber == lastInLine - 1)
            {
                ListThisLine();
                if (err.Count > 0)
                    ListErrors();
                ReadNextLine();
                positionNow.lineNumber = positionNow.lineNumber + 1;
                positionNow.charNumber = 0;
            }
            else ++positionNow.charNumber;
            Ch = line[positionNow.charNumber];
        }

        // Вывод строки в консоль и в файл с исправленным кодом
        private static void ListThisLine()
        {
            Console.WriteLine((positionNow.lineNumber + 1) + line);
            export.WriteLine((positionNow.lineNumber + 1) + line);
        }

        // Ну тут всё понятно, считываем следующую строчку в программу
        private static void ReadNextLine()
        {
            if (!File.EndOfStream)
            {
                line = "        " + File.ReadLine() + ' ';
                lastInLine = Convert.ToByte(line.Length);
                err = new List<Err>();
            }
            else
            {
                End();
                permission = false;
            }
                
        }

        // Вывод в консоль и в файл последней строки, как завершена программа
        static void End()
        {
            if (errCount == 0)
            {
                Console.WriteLine("Компиляция завершена успешно");
                export.WriteLine("Компиляция завершена успешно");
            }
            else
            {
                if (errCount < 10)
                {
                    Console.WriteLine($"Компиляция завершена: : ошибок — {errCount}!");
                    export.WriteLine($"Компиляция завершена: : ошибок — {errCount}!");
                }
                
                else
                {
                    Console.WriteLine($"Компиляция завершена: : ошибок — {errCount}! Подумайте хоть немного");
                    export.WriteLine($"Компиляция завершена: : ошибок — {errCount}! Подумайте хоть немного");
                }
            }
            
        }

        // Метод, который определяет ошибку и записывает её в файл, с указанием на неудовлетворяющий условию символ
        static void ListErrors()
        {
            int pos = 6 - $"{positionNow.lineNumber} ".Length;
            string s;
            foreach (Err item in err)
            {
                ++errCount;
                s = "**";
                if (errCount < 10)
                {
                    s += "0";
                    s += $"{errCount}**";
                    while (s.Length - 1 < item.errorPosition.charNumber) s += " ";
                    s += $"^ ошибка код {item.errorCode}";
                    switch (item.errorCode)
                    {
                        case 201:
                            s += ": ошибка в символьной константе";
                            break;
                        case 203:
                            s += ": Переполнение целочисленного типа данных";
                            break;
                        case 204:
                            s += ": Неизвестный символ";
                            break;
                        default:
                            s += ": Ошибка";
                            break;
                    }
                    export.WriteLine(s);
                    Console.WriteLine(s);
                }

            }
        }

        static public void Error(byte errorCode, TextPosition position)
        {
            Err e;
            if (err.Count <= ERRMAX)
            {
                e = new Err(position, errorCode);
                err.Add(e);
            }
        }

    }
}
