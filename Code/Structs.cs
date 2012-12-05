using System.Collections.Generic;

public class JStruct
{
    /// <summary>
    ///     Название структуры.
    /// </summary>
    public readonly string name;
    private Dictionary<string, string> Functions = new Dictionary<string, string>();
    private Dictionary<string, string> Variables = new Dictionary<string, string>();
    /// <summary>
    ///     Стек структур.
    /// </summary>
    public static readonly List<JStruct> mStack = new List<JStruct>();

    /// <summary>
    ///     Возвращает тип функции, если она была определена при создании объекта.
    /// </summary>
    /// <param name="fName">Название функции</param>
    /// <returns>Тип функции</returns>
    public string GetFunctionType(string fName)
    {
        if (Functions.ContainsKey(fName))
            return Functions[fName];
        return string.Empty;
    }

    /// <summary>
    ///     Возвращает тип переменной, если она была определена при создании объекта.
    /// </summary>
    /// <param name="vName">Имя переменной</param>
    /// <returns>Тип переменной</returns>
    public string GetVariableType(string vName)
    {
        if (Variables.ContainsKey(vName))
            return Variables[vName];
        return string.Empty;
    }

    /// <summary>
    ///     Создает экземпляр структуры, содержащей все методы, функции и переменные.
    /// </summary>
    /// <param name="text">Массив, содержащий скрипт.</param>
    /// <param name="start">Индекс начала.</param>
    /// <param name="end">Индекс конца.</param>
    public JStruct(string[] text, int start, int end)
    {
        bool isOperator = false;
        string[] split;
        string temp = string.Empty, type = string.Empty, tmp = string.Empty, name = string.Empty;
        int index = 0;
        int fLines = 0, vLines = 0;
        int[] funcs = new int[text.Length];
        int[] vars = new int[text.Length];
        for (int i = start; i <= end; i++)
        {
            temp = text[i].Trim();
            if (i == start)
                name = temp.Substring(7).Replace('{', '\0');
            if ((index = Analyzer.WhatKindOfStringWeHave(temp, text, i)) > 0)
            {
                funcs[fLines++] = i;
                if (index > i)
                    i = index;
            }
            else if (index == 0)
                vars[vLines++] = i;
        }
        for (int i = 0; i < fLines; i++)
        {
            temp = Format.TrimModifiers(Format.ReplaceSpacesAndNewLines(text[funcs[i]]));
            if ((index = temp.IndexOf('(')) > -1)
            {
                temp = temp.Substring(0, index);
                isOperator = temp.IndexOf(" operator ") > -1;
                if (isOperator)
                    temp = temp.Replace("operator ", string.Empty);
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
                tmp = temp.Replace("method ", string.Empty);
                isOperator = tmp.Substring(0, tmp.IndexOf(' ')) == "operator";
                if (isOperator)
                    tmp = tmp.Replace("operator ", string.Empty);
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
            temp = Format.TrimModifiers(Format.ReplaceSpacesAndNewLines(text[vars[i]]));
            if (temp.IndexOf(',') > 0)
            {
                split = temp.Split(',');
                if ((index = temp.IndexOf(' ')) > 0)
                    type = temp.Substring(0, index);
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