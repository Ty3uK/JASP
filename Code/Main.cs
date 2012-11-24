using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using JASP.Forms;

public class MainClass
{
    public static Progress parsing;
    public static Map map;

    [STAThread]
    static void Main(string[] args)
    {
        Application.EnableVisualStyles();

        if (args.Length == 0) {
            Application.Run(new About());
        } else if (args.Length > 1 && args[0] == "--map") {
            if (File.Exists(SFMpq.DLL))
            {
                parsing = new Progress();
                parsing = new Progress();
                parsing.Visible = true;
                parsing.SetMapPath(args[1]);
                map = new Map(Application.StartupPath, args[1]);
                parsing.SetProgress(20, "comments and empty lines...");
                Preprocessor.SplitComments(map.script);
                map.script = Format.SplitEmptyLines(map.script);
                parsing.SetProgress(20, "comments and empty lines... DONE!");
                parsing.SetProgress(40, "user functions...");
                Analyzer.FindUserFunctions(map.script);
                Analyzer.GetGlobals(map.script);
                parsing.SetProgress(40, "user functions and structs... DONE!");
                parsing.SetProgress(60, "\"//! jasp\" blocks...");
                Preprocessor.ParseJaspBlock(map.script);
                parsing.SetProgress(60, "\"//! jasp\" blocks... DONE!");
                parsing.SetProgress(80, "Rebuilding parsed script...");
                map.script = Format.StringsToString(map.script).Split('\n');
                parsing.SetProgress(80, "Rebuilding parsed script... DONE!");
                parsing.SetProgress(100, "errors...");
                Preprocessor.SearchErrors(map.script);
            } else
                MessageBox.Show("File \"SFMpq.dll\" isn't exists.\nSorry :(", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

}