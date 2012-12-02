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
        string[] split;
        string temp = "", type = "", tmp = "";
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
            temp = Format.ReplaceSpacesAndNewLines(text[funcs[i]]).Replace("private ", "").Replace("public ", "").Replace("static ", "");
            temp = temp.Substring(0, temp.IndexOf('('));
            index = temp.IndexOf(' ');
            Functions.Add(temp.Substring(0, index), temp.Substring(index + 1));
        }
        for (int i = 0; i < vLines; i++)
        {
            temp = Format.ReplaceSpacesAndNewLines(text[vars[i]]).Replace("private ", "").Replace("public ", "").Replace("static ", "");
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