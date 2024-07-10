using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Компилятор
{

    // Словарь словарей, в котором хронятся ключевые слова для компилятора Pascal
    class Keywords
    {
        Dictionary<byte, Dictionary<string, byte>> kw = new Dictionary<byte, Dictionary<string, byte>>();
        public Dictionary<byte, Dictionary<string, byte>> Kw
        {
            get { return kw; }
        }
        public Keywords()
        {
            Dictionary<string, byte> tmp = new Dictionary<string, byte>();
            tmp["do"] = LexicalAnalyzer.dosy;
            tmp["if"] = LexicalAnalyzer.ifsy;
            tmp["in"] = LexicalAnalyzer.insy;
            tmp["of"] = LexicalAnalyzer.ofsy;
            tmp["or"] = LexicalAnalyzer.orsy;
            tmp["to"] = LexicalAnalyzer.tosy;
            kw[2] = tmp;
            tmp = new Dictionary<string, byte>();
            tmp["end"] = LexicalAnalyzer.endsy;
            tmp["var"] = LexicalAnalyzer.varsy;
            tmp["div"] = LexicalAnalyzer.divsy;
            tmp["and"] = LexicalAnalyzer.andsy;
            tmp["not"] = LexicalAnalyzer.notsy;
            tmp["for"] = LexicalAnalyzer.forsy;
            tmp["mod"] = LexicalAnalyzer.modsy;
            tmp["nil"] = LexicalAnalyzer.nilsy;
            tmp["set"] = LexicalAnalyzer.setsy;
            kw[3] = tmp;
            tmp = new Dictionary<string, byte>();
            tmp["then"] = LexicalAnalyzer.thensy;
            tmp["else"] = LexicalAnalyzer.elsesy;
            tmp["case"] = LexicalAnalyzer.casesy;
            tmp["file"] = LexicalAnalyzer.filesy;
            tmp["goto"] = LexicalAnalyzer.gotosy;
            tmp["type"] = LexicalAnalyzer.typesy;
            tmp["with"] = LexicalAnalyzer.withsy;
            tmp["char"] = LexicalAnalyzer.charsy;
            tmp["true"] = LexicalAnalyzer.Trueis;
            tmp["real"] = LexicalAnalyzer.realsy;
            kw[4] = tmp;
            tmp = new Dictionary<string, byte>();
            tmp["begin"] = LexicalAnalyzer.beginsy;
            tmp["while"] = LexicalAnalyzer.whilesy;
            tmp["array"] = LexicalAnalyzer.arraysy;
            tmp["const"] = LexicalAnalyzer.constsy;
            tmp["label"] = LexicalAnalyzer.labelsy;
            tmp["until"] = LexicalAnalyzer.untilsy;
            tmp["false"] = LexicalAnalyzer.Falseis;
            kw[5] = tmp;
            tmp = new Dictionary<string, byte>();
            tmp["downto"] = LexicalAnalyzer.downtosy;
            tmp["packed"] = LexicalAnalyzer.packedsy;
            tmp["record"] = LexicalAnalyzer.recordsy;
            tmp["repeat"] = LexicalAnalyzer.repeatsy;
            tmp["string"] = LexicalAnalyzer.stringsy;
            kw[6] = tmp;
            tmp = new Dictionary<string, byte>();
            tmp["program"] = LexicalAnalyzer.programsy;
            tmp["integer"] = LexicalAnalyzer.integersy;
            tmp["boolean"] = LexicalAnalyzer.boolsy;
            kw[7] = tmp;
            tmp = new Dictionary<string, byte>();
            tmp["function"] = LexicalAnalyzer.functionsy;
            kw[8] = tmp;
            tmp = new Dictionary<string, byte>();
            tmp["procedure"] = LexicalAnalyzer.procedurensy;
            kw[9] = tmp;
        }

        // Метод для поиска ключевого слова, если оно найдено, присваивается код, иначе 0.
        public byte code_search(string name)
        {
            byte symb = 0;
            Dictionary<string, byte> keyValuePairs = new Dictionary<string, byte>();
            kw.TryGetValue(Convert.ToByte(name.Length), out keyValuePairs);
            foreach (var item1 in keyValuePairs.Keys)
            {
                if (item1 == name)
                {
                    keyValuePairs.TryGetValue(item1, out symb);
                    break;
                }   
                else
                    symb = 0;
            }
            return symb; 
        }
    }
}
