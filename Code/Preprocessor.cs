using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using JASP.Forms;

class Preprocessor
{

    private static int bCount = 0;
    private static List<int> bStart = new List<int>();
    private static List<int> bEnd = new List<int>();

    private static void ParseStrings(string[] text, List<Match> strings, int start, int end, short stage)
    {
        Regex reg;
        MatchCollection matches;
        if (stage == 1)
        {
            for (int i = start; i < end; i++)
            {
                reg = new Regex("\".*?\"", RegexOptions.Singleline);
                matches = reg.Matches(text[i]);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        text[i] = reg.Replace(text[i], "_JASP_STRING_" + match.GetHashCode(), 1);
                        strings.Add(match);
                    }
                }
            }
        }
        else
        {
            for (int i = start; i < end; i++)
                foreach (Match match in strings)
                    text[i] = text[i].Replace("_JASP_STRING_" + match.GetHashCode(), match.Value);
        }
    }

    public static void SplitComments(string[] input)
    {
        int index, oIndex, cIndex, OC = 0, CC = 0, Start = 0, End = 0;
        for (int i = 0; i < input.Length; i++)
        {
            index = input[i].IndexOf("//");
            if (index != -1 && input[i][index+2] != '!') input[i] = input[i].Substring(0, index);
            oIndex = input[i].IndexOf("/*");
            cIndex = input[i].IndexOf("*/");
            if (oIndex != -1)
            {
                if (OC == 0)
                    Start = i;
                OC++;
            }
            if (cIndex != -1)
            {
                CC++;
                if (OC == CC)
                {
                    End = i;
                    OC = 0;
                    CC = 0;
                    if (Start == End)
                    {
                        input[i] = input[i].Remove(oIndex, cIndex - oIndex + 2);
                    }
                    else
                    {
                        for (int j = Start; j <= End; j++)
                        {
                            if (j == Start) input[j] = input[j].Substring(0, input[j].IndexOf("/*"));
                            else input[j] = "";
                        }
                    }
                }
            }
        }
    }

    private static List<string> GetLocals(string[] input, int start, int end)
    {
        List<string> output = new List<string>();
        for (int i = start; i < end; i++)
        {
            string temp = input[i].Trim();
            string prefix = "";
            string[] split;
            int index = temp.IndexOf(' ');
            if (index != -1)
                prefix = temp.Substring(0, index);
                if (prefix != "" && prefix != "call" && prefix != "set" && prefix != "return" &&
                    prefix != "if" && prefix != "else" && prefix != "elseif" && prefix != "endif" &&
                    prefix != "var" && prefix != "flush" && prefix != "delete" && prefix != "exitwhen" &&
                    prefix.IndexOf('(') == -1 && prefix.IndexOf("bj_") == -1 &&
                    prefix != "for" && prefix != "while" && prefix != "whilenot" && prefix != "function" &&
                    prefix != "method" && prefix != "struct" && prefix != "define" && prefix != "enum")
                {
                    split = temp.Split('\n');
                    foreach (string tmp in split)
                    {
                        temp = tmp.Substring(6);
                        index = temp.IndexOf('=');
                        if (index != -1)
                            temp = temp.Substring(0, index).Trim();
                        else
                            temp = temp.Trim();
                        if (!Analyzer.HaveString(output, temp))
                        {
                            output.Add(temp);
                        }
                    }
                }
        }
        return output;
    }

    private static void ParseSingleLine(string[] text, int start, int end)
    {
        for (int i = start; i < end; i++)
        {
            string temp = text[i].Trim();
            if (temp.Length >= 4 && temp.Substring(0, 4) == "new ")
                text[i] = text[i].Replace(",", "\nnew "+temp.Substring(4, temp.IndexOf(' ', 5) - 4));
            if (temp.Length >= 6 && temp.Substring(0, 6) == "flush ") text[i] = text[i].Replace(",", "\nflush");
            if (temp.Length >= 7 && temp.Substring(0, 7) == "delete " && temp.IndexOf('(') == -1) text[i] = text[i].Replace(",", "\ndelete");
            text[i] = Format.ReplaceSpacesAndNewLines(text[i]);
        }
    }

    private static void ParseFlush(string[] text, int start, int end)
    {
        string temp = "";
        for (int i = start; i < end; i++)
        {
            if (text[i].Length > 6 && text[i].Substring(0, 6) == "flush ")
            {
                temp = text[i].Replace('\n', ' ').Substring(6, text[i].Length - 6);
                if (temp != "locals")
                {
                    text[i] = "set " + temp + " = null";
                }
            }
        }
    }

    private static void ParseNew(string[] text, int start, int end)
    {
        string temp = "", output = "", func = "", arg = "", s = "";
        int index = -1, count = 1;
        for (int i = start; i < end; i++)
        {
            s = text[i].Trim();
            if (s.Length > 3 && s.Substring(0, 4) == "new ")
            {
                string[] split = s.Split('\n');
                foreach (string tmp in split)
                {
                    func = JData.New(tmp.Substring(4, tmp.IndexOf(' ', 5) - 4));
                    temp = tmp.Substring(4);
                    index = temp.IndexOf('(');
                    if (index != -1)
                    {
                        arg = temp.Substring(index, temp.Length - index);
                        temp = temp.Replace(arg, "");
                    }
                    else arg = "()";
                    if (func == ".create")
                        temp += " = " + temp.Substring(0, temp.IndexOf(' '));
                    else
                        temp += " = ";
                    output += "local " + temp + func + arg;
                    if (++count <= split.Length)
                        output += '\n';
                }
                text[i] = output;
            }
        }
    }

    private static string[] ParseDelete(string[] text, int start, int end)
    {
        List<string> locals = Preprocessor.GetLocals(text, start, end);
        string temp = "", type = "", output = "", del = "";
        string[] split;
        int index = -1, count = 1;
        for (int i = start; i < end; i++)
        {
            temp = text[i].Trim();
            if (temp.Length >= 7 && temp.Substring(0, 7) == "delete ")
            {
                split = temp.Split('\n');
                foreach(string tmp in split)
                {
                    type = "";
                    temp = tmp.Substring(7);
                    index = temp.IndexOf('(');
                    if (index != -1)
                        type = JData.Type(temp.Substring(0, index));
                    else
                    {
                        foreach (string local in locals)
                        {
                            index = local.IndexOf(' ');
                            if (temp == local.Substring(index + 1))
                            {
                                type = local.Substring(0, index);
                                break;
                            }
                        }
                        if (type == "")
                            foreach (string global in Analyzer.globals)
                            {
                                index = global.IndexOf(' ');
                                if (temp == global.Substring(index + 1))
                                {
                                    type = global.Substring(0, index);
                                    break;
                                }
                            }
                    }
                    del = JData.Delete(type);
                    if (del != "undefined")
                        output += "call " + del + '(' + temp + ')';
                    else
                        output += "call " + temp + ".destroy()";
                    if (++count <= split.Length)
                        output += '\n';
                }
                text[i] = output;
            }
        }
        return text;
    }

    private static string[] ParseDynamicVariables(string[] text, int start, int end)
    {
        var Vars = new List<string>();
        var locals = Preprocessor.GetLocals(text, start, end);
        string type = "";
        string expression = "";
        for (int i = start; i < end; i++)
        {
            string temp = Format.ParseAssign(text[i].Trim());
            if (temp.Length > 3 && temp.Substring(0, 4) == "var ")
            {
                temp = temp.Substring(4, temp.Length - 4).Trim();
                int index = temp.IndexOf('=');
                if (index != -1 && temp.Substring(0, index).Trim().Length > 0)
                {
                    int newIndex = temp.IndexOf(" new ");
                    int argsIndex = temp.IndexOf('(');
                    if (newIndex != -1)
                    {
                        if (argsIndex != -1)
                            temp = temp.Replace(temp.Substring(argsIndex, temp.Length - argsIndex), "");
                        expression = temp.Substring(newIndex, temp.Length - newIndex);
                        type = temp.Substring(newIndex + 5, temp.Length - newIndex - 5);
                    }
                    else
                        //MessageBox.Show(temp.Substring(index + 1, temp.Length - index - 1).Trim());
                        type = Analyzer.GetVariableType(temp.Substring(index + 1, temp.Length - index - 1).Trim(), locals);

                    if (newIndex != -1)
                    {
                        string func = JData.New(expression + ' ');//Preprocessor.GetNewFunc(expression + ' ');
                        if (func == ".create")
                            text[i] = text[i].Replace("var ", "local " + type + " ").Replace(expression, ' ' + type + func);
                        else
                            text[i] = text[i].Replace("var ", "local " + type + " ").Replace(expression, ' ' + func);
                        if (argsIndex == -1)
                            text[i] += "()";
                    }
                    else if (type != "undefined")
                        text[i] = text[i].Replace("var ", "local " + type + " ");

                    string local = type + ' ' + temp.Substring(0, temp.IndexOf(' ')).Trim();
                    if (!Analyzer.HaveString(locals, local))
                        locals.Add(local);
                }
                else Vars.Add(temp);
            }
        }
        for (int i = start; i < end; i++)
        {
            string temp = Format.ParseAssign(text[i].Trim() + ' ');
            if (temp.Length > 3 && temp.Substring(0, 4) == "var ")
            {
                temp = temp.Substring(4, temp.Length - 4).Trim();
                int index = temp.IndexOf('=');
                if (index != -1 && temp.Substring(0, index).Trim().Length > 0)
                {
                    type = Analyzer.GetVariableType(temp.Substring(index + 1, temp.Length - index - 1).Trim(), locals);
                    text[i] = text[i].Replace("var ", "local " + type + " ");
                }
            }
        }
        return text;
    }

    private static void ParseFunctions(string[] text, int start, int end)
    {
        for (short i = 0; i < text.Length; i++)
        {
            string temp = text[i].Replace("private", "").Replace("static", "").Replace("public", "").Trim();
            if (temp.Length >= 9 && temp.Substring(0, 9) == "function ")
                start = i;
            if (temp.Length >= 11 && temp.Substring(0, 11) == "endfunction")
            {
                text = ParseDynamicVariables(text, start, i);
                text = ParseDelete(text, start, i);
            }
        }

        for (short i = 0; i < text.Length; i++)
        {
            string temp = text[i].Replace("private", "").Replace("static", "").Replace("public", "").Trim();
            if (temp.Length >= 7 && temp.Substring(0, 7) == "method ")
                start = i;
            if (temp.Length >= 9 && temp.Substring(0, 9) == "endmethod")
            {
                text = ParseDynamicVariables(text, start, i);
                text = ParseDelete(text, start, i);
            }
        }

        start = 0;
        do
        {
            if (text[start].IndexOf("{") != -1 && text[start].IndexOf("library") == -1 &&
                text[start].IndexOf("scope") == -1 && text[start].IndexOf("struct") == -1 &&
                text[start].IndexOf("define") == -1 && text[start].IndexOf("enum") == -1 &&
                text[start - 1].IndexOf("library") == -1 && text[start - 1].IndexOf("scope") == -1 &&
                text[start - 1].IndexOf("struct") == -1 && text[start - 1].IndexOf("define") == -1 &&
                text[start - 1].IndexOf("enum") == -1)
            {
                end = Analyzer.FindBraces(text, start);
                text = ParseDynamicVariables(text, start, end);
                text = ParseDelete(text, start, end);
                start = end;
            }
            start++;
        } while (start < text.Length);
    }

    public static void ParseJaspBlock(string[] text)
    {
        int start = 0, end = 0;
        string temp = "";
        List<Match> strings = new List<Match>();
        for (int i = 0; i < text.Length; i++)
        {
            temp = text[i].Trim();
            if (temp.Length > 7 && temp.Substring(0, 8) == "//! jasp")
                start = i;
            if (temp.Length > 10 && temp.Substring(0, 11) == "//! endjasp")
            {
                end = i;
                bStart.Add(start);
                bEnd.Add(end);
                bCount++;
                ParseStrings(text, strings, start, end, 1);
                ParseSingleLine(text, start, end);
                ParseNew(text, start, end);
                ParseFlush(text, start, end);
                ParseFunctions(text, start, end);
                ParseStrings(text, strings, start, end, 2);
            }
        }
    }

    public static void SearchErrors(string[] text)
    {
        Errors ERR = new Errors();
        int firstline = -1;
        for (int a = 0; a < bCount; a++)
        {
            for (int i = bStart[a]; i < bEnd[a]; i++)
            {
                if (text[i].Length > 4 && text[i].Substring(0, 4) == "var ")
                {
                    if (text[i].IndexOf('=') == -1)
                        if (text[i].IndexOf(',') != -1)
                            ERR.AddListItem("Line " + i + ": multiply definition not suported.");
                        else
                            ERR.AddListItem("Line " + i + ": where is variable's value?");
                    else if (text[i].Substring(4, text[i].IndexOf('=') - 4).Trim().Length == 0)
                        ERR.AddListItem("Line " + i + ": where is variable's label?");
                    else
                        ERR.AddListItem("Line " + i + ": cannot parse dynamic-type variable outside of function or method.");
                    if (firstline == -1)
                        firstline = i;
                }
                else if (text[i].Length > 7 && text[i].Substring(0, 4) == "delete ")
                {
                    ERR.AddListItem("Line " + i + ": cannot parse delete...");
                    if (firstline == -1)
                        firstline = i;
                }
            }
        }
        if (firstline > 0)
        {
            MainClass.parsing.SetProgress(100, "errors... Have errors!");
            ERR.SetText(text);
            ERR.SetFirstLine(firstline);
            MainClass.parsing.Close();
            System.Environment.ExitCode = 1;
            ERR.SelectFirstLine();
        }
        else
        {
            MainClass.parsing.SetProgress(100, "errors... DONE!");
            File.WriteAllLines(MainClass.map.PATH + "\\war3map_parsed.j", text);
            System.Environment.ExitCode = 0;
        }
    }

}