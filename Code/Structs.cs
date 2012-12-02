using System;
using System.Collections.Generic;
using System.Windows.Forms;

public class Struct
{
    public readonly string name;
    public readonly Dictionary<string, string> Functions = new Dictionary<string, string>();
    public readonly Dictionary<string, string> Variables = new Dictionary<string, string>();

    public Struct(string[] text, int start, int end)
    {
        bool isOperator = false;
        string[] split;
        string temp = "", type = "", tmp = "", name = "";
        int index = 0;
        int fLines = 0, vLines = 0;
        int[] funcs = new int[text.Length];
        int[] vars = new int[text.Length];
        for (int i = start; i <= end; i++)
        {
            temp = text[i].Trim();
            if (i == start)
                name = temp.Substring(7).Replace("{", "");
            if (Analyzer.IsFunction(temp))
                funcs[fLines++] = i;
            else if (Analyzer.IsVariable(temp))
                vars[vLines++] = i;
        }
        for (int i = 0; i < fLines; i++)
        {
            temp = Format.ReplaceSpacesAndNewLines(text[funcs[i]]);
            while ((index = temp.IndexOf(' ')) != -1)
            {
                tmp = temp.Substring(0, index);
                if (tmp == "private" || tmp == "public" ||
                    tmp == "static"  || tmp == "stub"   ||
                    tmp == "constant")
                {
                    //THEN
                    temp = temp.Substring(index + 1);
                }
                else
                    break;
            }
            if ((index = temp.IndexOf('(')) > -1)
            {
                temp = temp.Substring(0, index);
                isOperator = temp.IndexOf(" operator ") > -1;
                if (isOperator)
                    temp = temp.Replace("operator ", String.Empty);
                index = temp.IndexOf(' ');
                name = temp.Substring(index + 1);
                type = temp.Substring(0, index);
                if (isOperator)
                {
                    if (type != "nothing" && type != "void")
                        if (!Variables.ContainsKey(name))
                            Variables.Add(name, type);
                }
                else
                {
                    if (!Functions.ContainsKey(name))
                        Functions.Add(name, type);
                }
            }
            else
            {
                tmp = temp.Replace("method ", String.Empty);
                isOperator = tmp.Substring(0, tmp.IndexOf(' ')) == "operator";
                if (isOperator)
                    tmp = tmp.Replace("operator ", String.Empty);
                name = tmp.Substring(0, tmp.IndexOf(' '));
                type = tmp.Substring(tmp.LastIndexOf(' ') + 1);
                if (isOperator)
                {
                    if (type != "nothing" && type != "void")
                        if (!Variables.ContainsKey(name))
                            Variables.Add(name, type);
                } else {
                    if (!Functions.ContainsKey(name))
                        Functions.Add(name, type);
                }
            }
        }
        for (int i = 0; i < vLines; i++)
        {
            temp = Format.ReplaceSpacesAndNewLines(text[vars[i]]);
            while ((index = temp.IndexOf(' ')) != -1) {
                tmp = temp.Substring(0, index);
                if (tmp == "private" || tmp == "public"   ||
                    tmp == "static"  || tmp == "readonly" ||
                    tmp == "constant")
                    //THEN
                    temp = temp.Substring(index + 1);
                else
                    break;
            }
            if (temp.IndexOf(',') > 0)
            {
                split = temp.Split(',');
                type = temp.Substring(0, temp.IndexOf(' '));
                for (int j = 0; j < split.Length; j++)
                {
                    if (j > 0)
                        tmp = type + ' ' + split[j].Trim();
                    else
                        tmp = split[j].Trim();
                    index = tmp.IndexOf(' ');
                    Variables.Add(tmp.Substring(index + 1), tmp.Substring(0, index));
                }
            }
            else
            {
                index = temp.IndexOf('=');
                if (index > 0)
                    tmp = temp.Substring(0, index).Trim();
                else
                    tmp = temp;
                index = tmp.IndexOf(' ');
                Variables.Add(tmp.Substring(index + 1), tmp.Substring(0, index));
            }
        }
    }
}