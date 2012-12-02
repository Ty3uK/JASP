using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

public class Analyzer
{
    public static string[] globals;
    public static List<Struct> Structs = new List<Struct>();

    public static bool HaveString(List<string> input, string source)
    {
        return input.Exists(new System.Predicate<string>(x => x == source));
    }

    public static int FindBraces(string[] text, int start)
    {
        int OB = 0, CB = 0, end = 0;
        for (int i = start; i < text.Length; i++)
        {
            if (text[i].IndexOf("{") != -1) OB++;
            if (text[i].IndexOf("}") != -1)
            {
                CB++;
                if (CB == OB) { end = i; break; };

            }
        }
        return end;
    }

    public static int CountSymbolsInText(string symbol, string text)
    {
        int count = 0, m;
        while ((m = text.IndexOf(symbol)) != -1)
        {
            count++;
            text = text.Substring(m + 1);
        }
        return count;
    }

    public static string GetVariableType(string text, List<string> locals)
    {
        int index;
        if (text.IndexOf(".create") != -1)
            return text.Substring(0, text.IndexOf('.')).Trim();
        if (text.Substring(0, 1) == "'") return "integer";
        if (text.Substring(0, 1) == "\"") return "string";
        var reg = new Regex(@"\d");
        index = text.IndexOf(' ');
        string temp = "";
        if (index == -1) temp = text.Substring(0, text.Length);
        else temp = text.Substring(0, index);
        if (reg.IsMatch(temp))
            if (temp.IndexOf('.') == -1) return "integer";
            else return "real";
        if (text.Length >= 4 && text.Substring(0, 4).ToLower() == "true") return "boolean";
        if (text.Length >= 5 && text.Substring(0, 5).ToLower() == "false") return "boolean";
        if (text.Length >= 9 && text.Substring(0, 9) == "function ") return "code";
        index = text.IndexOf('(');
        if (locals != null)
            for (int i = 0; i < locals.Count; i++)
            {
                index = locals[i].IndexOf(' ');
                if (index != -1)
                {
                    string local = locals[i].Substring(index, locals[i].Length - index).Trim();
                    string first = "";
                    if (text.IndexOf(' ') != -1)
                        first = text.Substring(0, text.IndexOf(' '));
                    else
                        first = text;
                    if (first == local)
                        return locals[i].Substring(0, index);
                }
            }
        if (globals.Length > 0)
            for (int i = 0; i < globals.Length; i++)
            {
                index = globals[i].IndexOf(' ');
                if (index != -1)
                {
                    string global = "";
                    int nIndex = globals[i].IndexOf('=');
                    if (nIndex == -1)
                        global = globals[i].Substring(index, globals[i].Length - index).Trim();
                    else
                        global = globals[i].Substring(index, nIndex - index).Trim();
                    string first = "";
                    nIndex = text.IndexOf(' ');
                    if (nIndex != -1)
                        first = text.Substring(0, nIndex);
                    else
                        first = text;
                    if (first == global)
                        return globals[i].Substring(0, index);
                }
            }
        if (index != -1)
            return JData.Type(text.Substring(0, index).Trim());//Types.GetFuncType(text.Substring(0, index).Trim());
        return "undefined";
    }

    public static void GetGlobals(string[] text)
    {
        bool b = false;
        string temp = "";
        int lines = 0;
        int[] nums = new int[text.Length];
        for (int i = 0; i < text.Length; i++)
        {
            temp = text[i].Trim();
            if (temp == "globals")
                b = true;
            else if (temp == "endglobals")
                b = false;
            else if (b)
            {
                nums[lines++] = i;
            }
        }
        string[] output = new string[lines];
        for (int i = 0; i < lines; i++)
            output[i] = Format.ReplaceSpaces(text[nums[i]]);
        globals = output;
    }

    public static void FindUserFunctions(string[] text)
    {
        Struct tempS;
        string name = "", type = "";
        for (int i = 0; i < text.Length; i++)
        {
            string temp = text[i].Replace("private ", "").Replace("static ", "").Replace("public ", "").Replace("operator ", "").Trim();
            if (temp.Length >= 9 && temp.Substring(0, 9) == "function " && temp.IndexOf("takes") != -1 && temp.IndexOf("returns") != -1)
            {
                temp = temp.Substring(9, temp.Length - 9);
                type = temp.Substring(temp.IndexOf("returns") + 8, temp.Length - temp.IndexOf("returns") - 8);
                if (type != "nothing" && type != "void")
                {
                    name = temp.Substring(0, temp.IndexOf(' '));
                    JData.Add(name, type);
                    //Types.AddFunc(name, type);
                }
            }
            else if (temp.Length >= 7 && temp.Substring(0, 7) == "method " && temp.IndexOf("takes") != -1 && temp.IndexOf("returns") != -1)
            {
                temp = temp.Substring(7, temp.Length - 7);
                type = temp.Substring(temp.IndexOf("returns") + 8, temp.Length - temp.IndexOf("returns") - 8);
                if (type != "nothing" && type != "void")
                {
                    name = temp.Substring(0, temp.IndexOf(' ')).Trim();
                    JData.Add(name, type);
                    //Types.AddFunc(name, type);
                }
            }
            else if (temp.Length >= 7 && temp.Substring(0, 7) == "struct ") 
            {
                int close = i + 1;
                if (temp[temp.Length - 1] == '{' || text[i + 1].Trim() == "{")
                    close = Analyzer.FindBraces(text, i);
                else
                    for (int j = i; j < text.Length; j++)
                        if (text[j].Trim() == "endstruct")
                        {
                            close = j - 1;
                            break;
                        }
                tempS = new Struct(text, i, close);
                Structs.Add(tempS);
            }
            else
            {
                int index = temp.IndexOf(' ');
                int bIndex = temp.IndexOf('(');
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
                {
                    type = temp.Substring(0, index).Trim();
                    temp = temp.Replace(type, "");
                    JData.Add(temp.Substring(0, temp.IndexOf('(')).Trim(), type);
                    //Types.AddFunc(temp.Substring(0, temp.IndexOf('(')).Trim(), type);
                }
            }

        }
    }

    public static bool IsFunction(string text)
    {
        int sIndex = text.IndexOf(' ');
        int bIndex = text.IndexOf('(');
        string substr = "";
        if (text.IndexOf('=') == -1 && sIndex > -1 && sIndex < bIndex)
        {
            substr = text.Substring(0, sIndex);
            if (substr != "define" && substr != "callback" &&
                substr != "lambda" && substr != "library" &&
                substr != "scope" && substr != "enum" &&
                substr != "struct" && substr != "loop" &&
                substr != "for" && substr != "while" &&
                substr != "whilenot" && substr != "until" &&
                text.Substring(0, 2) != "if" && substr != "else" &&
                substr != "elseif" && substr != "call" &&
                substr != "set" && substr != "return" &&
                substr != "exitwhen" && substr != "local" && substr != "nothing" &&
                CountSymbolsInText("(", text) == 1 && CountSymbolsInText(")", text) == 1 &&
                text.IndexOf(';') == -1)
                //THEN
                return true;
        }
        return false;
    }

    public static bool IsVariable(string text)
    {
        int sIndex = text.IndexOf(' ');
        int bIndex = text.IndexOf('(');
        string substr = "";
        if (sIndex > 0)
        {
            substr = text.Substring(0, sIndex);
            if (Analyzer.CountSymbolsInText("{", text) == 0 &&
                Analyzer.CountSymbolsInText("}", text) == 0 &&
                substr != "define" && substr != "local" &&
                substr != "enum" && substr != "struct" &&
                substr != "loop" && substr != "for" &&
                substr != "while" && substr != "whilenot" &&
                substr != "until" && text.Substring(0, 2) != "if" &&
                substr != "else" && substr != "elseif" &&
                substr != "call" && substr != "set" &&
                substr != "return" && substr != "exitwhen" &&
                substr != "flush" && substr != "delete")
                //THEN
                return true;
        }
        return false;
    }
}