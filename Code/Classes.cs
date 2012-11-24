using System.IO;
using System.Collections.Generic;

public class Map
{
    public readonly int MPQ;
    public string[] script;
    public readonly string PATH;

    public Map(string mapPath, string aName)
    {
        PATH = mapPath;
        MPQ = SFMpq.MpqOpenArchiveForUpdate(aName, SFMpq.MOAU_OPEN_EXISTING + SFMpq.MOAU_MAINTAIN_LISTFILE, 0);
        SFMpq.MpqExtractFileTo(MPQ, "war3map.j", PATH + "\\war3map.j");
        script = File.ReadAllLines(PATH + "\\war3map.j");
    }

   /* ~Map()
    {
        SFMpq.MpqAddFileToArchiveEx(MPQ, PATH + "\\war3map_parsed.j", "war3map.j", SFMpq.MAFA_REPLACE_EXISTING + SFMpq.MAFA_COMPRESS, SFMpq.MAFA_COMPRESS_DEFLATE, SFMpq.Z_BEST_COMPRESSION);
        SFMpq.MpqCompactArchive(MPQ);
        SFMpq.MpqCloseUpdatedArchive(MPQ, 0);
    }*/
}

public class Script
{
    public List<string> globals = new List<string>();
}