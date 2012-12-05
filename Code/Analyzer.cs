using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

public class Analyzer
{
    public static string[] globals;

    public static bool HaveString(List<string> input, string source)
    {
        return input.Exists(new System.Predicate<string>(x => x == source));
    }

    public static int GetEndOfBlock(string[] text, int start, string blockStart, string bockEnd)
    {
        int OB = 0, CB = 0, end = 0;
        for (int i = start; i < text.Length; i++)
        {
            if (text[i].IndexOf(blockStart) > -1)
                OB++;
            if (text[i].IndexOf(bockEnd) > -1)
                if (++CB == OB) {
                    end = i;
                    break;
                }
        }
        return end;
    }

    public static int GetEndOfBlock(string[] text, int start, char blockStart, char bockEnd)
    {
        int OB = 0, CB = 0, end = 0;
        for (int i = start; i < text.Length; i++)
        {
            if (text[i].IndexOf(blockStart) > -1)
                OB++;
            if (text[i].IndexOf(bockEnd) > -1)
                if (++CB == OB)
                {
                    end = i;
                    break;
                }
        }
        return end;
    }

    public static int CountSymbolsInText(string symbols, string text)
    {
        int count = 0, m;
        while ((m = text.IndexOf(symbols)) != -1)
        {
            count++;
            text = text.Substring(m + 1);
        }
        return count;
    }

    public static int CountSymbolsInText(char symbol, string text)
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
        string temp = string.Empty, type = string.Empty;
        int index = -1;
        for(int i = 0; i < text.Length; i++)
        {
            temp = Format.TrimModifiers(text[i]);
            if ((index = temp.IndexOf(' ')) > 0 && temp.Substring(0, temp.IndexOf(' ')) == "struct")
            {
                index = i;
                if (temp[temp.Length - 1] == '{')
                    i = Analyzer.GetEndOfBlock(text, i, '{', '}');
                else
                    i = Analyzer.GetEndOfBlock(text, i, "struct", "endstruct");
                JStruct.mStack.Add(new JStruct(text, index, i));
            }
            if (Analyzer.IsFunction(temp))
                if ((index = temp.IndexOf('(')) > 0)
                {
                    temp = temp.Substring(0, index);
                    index = temp.IndexOf(' ');
                    type = temp.Substring(0, index);
                    if (type != "nothing" && type != "void")
                        JData.Add(temp.Substring(index + 1), type);
                }
                else
                {
                    type = temp.Substring(temp.LastIndexOf(' ') + 1);
                    if (type != "nothing" && type != "void")
                        JData.Add(temp.Substring(9, temp.IndexOf(' ', 10) - 9), type);
                }
        }
        //MessageBox.Show(Struct.Stack[0].name);
    }

    private static string[] Keywords = new string[] {
        //For variables with '='
        "#define",
        "define",
        "enddefine",
        //For variables without '='
        "callback",
        "enum",
        "endenum",
        "library",
        "endlibrary",
        "library_once",
        "scope",
        "endscope",
        "struct",
        "endstruct",
        "type",
        "keyword",
        "delegate",
        "loop",
        "exitwhen",
        "endloop",
        "for",
        "endfor",
        "while",
        "endwhile",
        "whilenot",
        "endwhilenot",
        "if",
        "else",
        "elseif",
        "call",
        "set",
        "return",
        "flush",
        "delete",
        //For functions
        "local",
    };

    private static bool ContainsKeyword(string text, int max) {
        if (max > 34)
            max = 34;
        for (byte i = 0; i < max; i++)
            if (text == Keywords[i])
                return true;
        return false;
    }

    public static bool IsFunction(string text)
    {
        int sIndex = text.IndexOf(' ');
        int bIndex = text.IndexOf('(');
        string substr = "";
        text = Format.ReplaceSpacesAndNewLines(text);
        if (text.IndexOf('=') == -1 && sIndex > -1)
        {
            substr = text.Substring(0, sIndex).Trim();
            if (substr.Length > 2 && text.IndexOf(';') == -1 &&
                !ContainsKeyword(substr, 34))
                //THEN
                if ((bIndex > -1 && sIndex < bIndex &&
                    CountSymbolsInText("(", text) == 1 && CountSymbolsInText(")", text) == 1)
                    ||
                    (bIndex == -1 && text.IndexOf(" returns ") > -1))
                    return true;
        }
        return false;
    }

    public static bool IsVariable(string text)
    {
        int sIndex = text.IndexOf(' ');
        int bIndex = text.IndexOf('(');
        int eIndex = text.IndexOf('=');
        string substr = string.Empty;
        if (sIndex > 0)
        {
            substr = Format.ReplaceSpacesAndNewLines(text.Substring(0, sIndex));
            if (((eIndex > 0 && eIndex < bIndex && !ContainsKeyword(substr, 3)) // If we have '=' and '('
                    ||
                (eIndex == -1 && bIndex == -1 && !ContainsKeyword(substr, 33))) // Or if we haven't '=' and, of course, '('
                &&
                Analyzer.CountSymbolsInText("{", text) == 0 &&
                Analyzer.CountSymbolsInText("}", text) == 0)
                //THEN
                return true;
        }
        return false;
    }

    /// <summary>
    ///     Ищет ключевые слова в строке, определяя что она из мебя представляет - переменную или функцию.
    /// </summary>
    /// <param name="text">Строка, в которой происходит поиск.</param>
    /// <returns>Числовой параметр. 0 - переменная, 1 и более - функция и ее окончание</returns>
    public static int WhatKindOfStringWeHave(string input, string[] text, int start)
    {
        string substr = string.Empty;
        int sIndex = -1, bIndex = -1, abIndex = -1, eIndex = -1; //Indexes of SPACE, OPEN BRACE, OPEN  and EQUAL chars

        input = Format.TrimModifiers(Format.ReplaceSpaces(input));
        for (int i = 0; i < input.Length; i++)
        {
            if (sIndex == -1 && (input[i] == ' ' || input[i] == '\t'))
                sIndex = i;
            else if (bIndex == -1 && input[i] == '(')
                bIndex = i;
            else if (abIndex == -1 && input[i] == '{')
                abIndex = i;
            else if (eIndex == -1 && input[i] == '=')
                eIndex = i;
        }

        if (sIndex > 0)
        {
            substr = input.Substring(0, sIndex).Trim(); //Get first lexem of input string
            //MessageBox.Show(input + '\n' + substr + '\n' + substr.Length.ToString());
            if (eIndex == -1)
            {
                //If we have cJASS-styled function
                if (bIndex > 0 && sIndex < bIndex && !ContainsKeyword(substr, 34))
                    return GetEndOfBlock(text, start, '{', '}');
                //If we have classic JASS2-styled function
                else if (bIndex == -1 && input.IndexOf(" returns ") > 0)
                {
                    //Find end of detected method or function
                    for (int i = start; i < text.Length; i++)
                        if (text[i].Trim() == "end" + substr)
                            return i;
                }
                //Time to detect some kind of variables! :)
                else if (bIndex == -1 && !ContainsKeyword(substr, 33) &&
                          CountSymbolsInText('{', input) == 0 &&
                          CountSymbolsInText('}', input) == 0)
                    //THEN
                    return 0;
            }
            //Detection of another variables
            else if ((eIndex > 0 && (bIndex == -1 || eIndex < bIndex)) &&
                          !ContainsKeyword(substr, 3) &&
                          CountSymbolsInText('{', input) == 0 &&
                          CountSymbolsInText('}', input) == 0)
                //THEN
                return 0;
            else if (bIndex > 0 && eIndex > abIndex && !ContainsKeyword(substr, 34))
                return GetEndOfBlock(text, start, '{', '}');
        }
        return -1;
    }
}