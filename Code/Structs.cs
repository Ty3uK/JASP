using System;
using System.Collections.Generic;
using System.Windows.Forms;

public class Struct
{
    public string name;
    public string[] varName, varType;
    public string[] funcName, funcType;

    public void FindMethods(string[] text, int start, int end)
    {
        string temp = "";
        int index = 0, bIndex = 0;
        int lines = 0;
        int[] nums = new int[text.Length];
        for (int i = start; i <= end; i++)
        {
            temp = text[i].Trim();
            if (i == start)
                name = temp.Substring(7, temp.Length - 7).Replace("{", "");
            index = temp.IndexOf(' ');
            bIndex = temp.IndexOf('(');
            if (index != -1 && index < bIndex && temp.Substring(0, index) != "define" &&
                    temp.Substring(0, index) != "callback" && temp.Substring(0, index) != "lambda" &&
                    temp.Substring(0, index) != "library" && temp.Substring(0, index) != "scope" &&
                    temp.Substring(0, index) != "enum" && temp.Substring(0, index) != "struct" &&
                    temp.Substring(0, index) != "loop" && temp.Substring(0, index) != "for" &&
                    temp.Substring(0, index) != "while" && temp.Substring(0, index) != "whilenot" &&
                    temp.Substring(0, index) != "until" && temp.Substring(0, 2) != "if" &&
                    temp.Substring(0, index) != "else" && temp.Substring(0, index) != "elseif" &&
                    temp.Substring(0, index) != "call" && temp.Substring(0, index) != "set" &&
                    temp.Substring(0, index) != "return" && temp.Substring(0, index) != "exitwhen" &&
                    temp.Substring(0, index) != "local" && temp.Substring(0, index) != "void" &&
                    temp.Substring(0, index) != "nothing" && temp.IndexOf('=') == -1 &&
                    Analyzer.CountSymbolsInText("(", temp) == 1 && Analyzer.CountSymbolsInText(")", temp) == 1 &&
                    temp.IndexOf(';') == -1)
                // THEN
                nums[lines++] = i;
        }
        List<string> names = new List<string>(), types = new List<string>();
        for (int i = 0; i < lines; i++)
        {
            temp = Format.ReplaceSpacesAndNewLines(text[nums[i]].Trim()).Replace("private ", "").Replace("public ", "").Replace("static ", "");
            temp = temp.Substring(0, temp.IndexOf('('));
            index = temp.IndexOf(' ');
            types.Add(temp.Substring(0, index));
            names.Add(temp.Substring(index, temp.Length - index));
        }
        funcName = names.ToArray();
        funcType = types.ToArray();
    }

    public void FindVars(string[] text, int start, int end)
    {
        string temp = "";
        int index = 0;
        int lines = 0;
        int[] nums = new int[text.Length];
        int fails = 0;
        for (int i = start; i <= end; i++)
        {
            temp = text[i].Trim();
            if (i == start)
                name = temp.Substring(7, temp.Length - 7).Replace("{", "");
            index = temp.IndexOf(' ');
            if (temp.IndexOf('=') == -1 && index > 0 &&
                Analyzer.CountSymbolsInText("(", temp) == 0 &&
                Analyzer.CountSymbolsInText(")", temp) == 0 &&
                Analyzer.CountSymbolsInText("{", temp) == 0 &&
                Analyzer.CountSymbolsInText("}", temp) == 0 &&
                temp.Substring(0, index) != "define" && temp.Substring(0, index) != "local" &&
                temp.Substring(0, index) != "enum" && temp.Substring(0, index) != "struct" &&
                temp.Substring(0, index) != "loop" && temp.Substring(0, index) != "for" &&
                temp.Substring(0, index) != "while" && temp.Substring(0, index) != "whilenot" &&
                temp.Substring(0, index) != "until" && temp.Substring(0, 2) != "if" &&
                temp.Substring(0, index) != "else" && temp.Substring(0, index) != "elseif" &&
                temp.Substring(0, index) != "call" && temp.Substring(0, index) != "set" &&
                temp.Substring(0, index) != "return" && temp.Substring(0, index) != "exitwhen" &&
                temp.Substring(0, index) != "flush" && temp.Substring(0, index) != "delete")
                // THEN
                nums[lines++] = i;
            else if (temp.Length > 0)
                if (fails++ > 3)
                    break;
        }
        int commas = 0, sCount = 0;
        string aTemp = "";
        string startS = "";
        List<string> names = new List<string>(), types = new List<string>();
        for (int i = 0; i < lines; i++)
        {
            temp = Format.ReplaceSpacesAndNewLines(text[nums[i]].Trim()).Replace("private ", "").Replace("public ", "").Replace("static ", "");
            commas = Analyzer.CountSymbolsInText(",", temp);
            if (temp.IndexOf('=') == -1 && commas > 0)
            {
                aTemp = temp.Substring(0, temp.IndexOf(','));
                startS = "";
                sCount = Analyzer.CountSymbolsInText(" ", aTemp);
                if (sCount == 2)
                    startS = aTemp.Substring(0, aTemp.IndexOf(' ', 8));
                else if (sCount == 3)
                    startS = aTemp.Substring(0, aTemp.IndexOf(' ', 16));
                else
                    startS = aTemp.Substring(0, aTemp.IndexOf(' '));
                string[] tA = temp.Split(',');
                for (int j = 0; j < tA.Length; j++)
                {
                    aTemp = tA[j].Trim();
                    if (j > 0)
                        aTemp = startS + ' ' + aTemp;
                    index = aTemp.IndexOf(' ');
                    types.Add(aTemp.Substring(0, index));
                    names.Add(aTemp.Substring(index + 1, aTemp.Length - index - 1));
                }
            }
            else
            {
                index = temp.IndexOf(' ');
                types.Add(temp.Substring(0, index));
                names.Add(temp.Substring(index + 1, temp.Length - index - 1));
            }
        }
        varName = names.ToArray();
        varType = types.ToArray();
    }
}