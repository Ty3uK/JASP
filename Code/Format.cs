using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;

class Format
{

    public static string StringsToString(string[] input)
    {
        return string.Join("\n", input);
    }

    public static string ReplaceEndOfLine(string text)
    {
        string output = "";
        if (text.Length > 3 && text.Substring(0, 3) != "for" && (text[3] != '(' || text[3] != ' ') && text[text.Length - 1] != ';')
            output = text.Replace(';', '\n');
        else
            output = text.Replace(';', ' ');
        return output;
    }

    public static string[] SplitEmptyLines(string[] input)
    {
        short lines = 0;
        short[] numbers = new short[input.Length];
        for (short i = 0; i < input.Length; i++)
            if (input[i].Trim().Length > 0) numbers[lines++] = i;
        if (lines == 0) return null;
        string[] output = new string[lines];
        for (short i = 0; i < lines; i++)
            output[i] = input[numbers[i]];
        return output;
    }

    public static string GetTextBetweenIndexes(string[] input, int a, int b)
    {
        string output = "";
        for (int i = a + 1; i < b; i++)
        {
            if (input[i].Trim().Length > 0) output += input[i] + '\n';
        }
        return output;
    }

    public static string ReplaceSpaces(string text)
    {
        bool space = true;
        string s = "";
        for (short i = 0; i < text.Length; i++)
        {
            if (text[i] == ' ' || text[i] == '\t')
            {
                if (space)
                    continue;
                space = true;
            }
            else if (space)
                space = false;
            s += text[i];
        }
        return s.Trim();
    }

    public static string ReplaceSpacesAndNewLines(string text)
    {
        string s = ReplaceSpaces(text);
        s = ReplaceEndOfLine(s);
        return s;
    }

    public static string[] SplitArray(string[] input)
    {
        return StringsToString(input).Split('\n');
    }

    public static string ParseAssign (string s)
	{
		string result = "";
		bool eq = false, ign = false;
		foreach (char c in s)
			if (ign) {
				result += c;
				ign = false;
			} else if (c == '=')
			if (!eq)
				eq = true;
			else {
				result += "==";
				eq = false;
			}
			else if (eq) {
				result += " = " + c.ToString ();
				eq = false;
			} else if (c == '!' || c == '<' || c == '>') {
				result += c;
				ign = true;
			} else
				result += c;
		return result;
	}

}