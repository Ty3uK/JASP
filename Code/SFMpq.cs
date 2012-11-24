/*

            SFMpq.dll API for C#
       Max Karelov aka Ty3uK (c) 2012
 
*/

using System;
using System.IO;
using System.Runtime.InteropServices;

    public class SFMpq
    {

        public const string DLL = "JASP\\SFmpq.dll";

        public const uint MPQ_ERROR_MPQ_INVALID = 0x85200065;
        public const uint MPQ_ERROR_FILE_NOT_FOUND = 0x85200066;
        public const uint MPQ_ERROR_DISK_FULL = 0x85200068;
        public const uint MPQ_ERROR_HASH_TABLE_FULL = 0x85200069;
        public const uint MPQ_ERROR_ALREADY_EXISTS = 0x8520006A;
        public const uint MPQ_ERROR_BAD_OPEN_MODE = 0x8520006C;

        public const uint MPQ_ERROR_COMPACT_ERROR = 0x85300001;

        public const uint MOAU_CREATE_NEW = 0x00;
        public const uint MOAU_CREATE_ALWAYS = 0x08;
        public const uint MOAU_OPEN_EXISTING = 0x04;
        public const uint MOAU_OPEN_ALWAYS = 0x20;
        public const uint MOAU_READ_ONLY = 0x10;
        public const uint MOAU_MAINTAIN_LISTFILE = 0x01;

        public const uint MAFA_EXISTS = 0x80000000;
        public const uint MAFA_UNKNOWN = 0x40000000;
        public const uint MAFA_MODCRYPTKEY = 0x00020000;
        public const uint MAFA_ENCRYPT = 0x00010000;
        public const uint MAFA_COMPRESS = 0x00000200;
        public const uint MAFA_COMPRESS2 = 0x00000100;
        public const uint MAFA_REPLACE_EXISTING = 0x00000001;

        public const uint MAFA_COMPRESS_STANDARD = 0x08;
        public const uint MAFA_COMPRESS_DEFLATE = 0x02;
        public const uint MAFA_COMPRESS_WAVE = 0x81;
        public const uint MAFA_COMPRESS_WAVE2 = 0x41;

        public const uint MAFA_COMPRESS_WAVECOMP1 = 0x80;
        public const uint MAFA_COMPRESS_WAVECOMP2 = 0x40;
        public const uint MAFA_COMPRESS_WAVECOMP3 = 0x01;

        public const uint Z_NO_COMPRESSION = 0;
        public const uint Z_BEST_SPEED = 1;
        public const uint Z_BEST_COMPRESSION = 9;
        public const int Z_DEFAULT_COMPRESSION = (-1);

        public const uint MAWA_QUALITY_HIGH = 1;
        public const uint MAWA_QUALITY_MEDIUM = 0;
        public const uint MAWA_QUALITY_LOW = 2;

        public const uint SFILE_INFO_BLOCK_SIZE = 0x01;
        public const uint SFILE_INFO_HASH_TABLE_SIZE = 0x02;
        public const uint SFILE_INFO_NUM_FILES = 0x03;
        public const uint SFILE_INFO_TYPE = 0x04;
        public const uint SFILE_INFO_SIZE = 0x05;
        public const uint SFILE_INFO_COMPRESSED_SIZE = 0x06;
        public const uint SFILE_INFO_FLAGS = 0x07;
        public const uint SFILE_INFO_PARENT = 0x08;
        public const uint SFILE_INFO_POSITION = 0x09;
        public const uint SFILE_INFO_LOCALEID = 0x0A;
        public const uint SFILE_INFO_PRIORITY = 0x0B;
        public const uint SFILE_INFO_HASH_INDEX = 0x0C;

        public const uint SFILE_LIST_MEMORY_LIST = 0x01;
        public const uint SFILE_LIST_ONLY_KNOWN = 0x02;
        public const uint SFILE_LIST_ONLY_UNKNOWN = 0x04;

        public const uint SFILE_TYPE_MPQ = 0x01;
        public const uint SFILE_TYPE_FILE = 0x02;

        public const uint SFILE_OPEN_HARD_DISK_FILE = 0x0000;
        public const uint SFILE_OPEN_CD_ROM_FILE = 0x0001;
        public const uint SFILE_OPEN_ALLOW_WRITE = 0x8000;

        public const uint SFILE_SEARCH_CURRENT_ONLY = 0x00;
        public const uint SFILE_SEARCH_ALL_OPEN = 0x01;

        // Storm functions implemented by this library
        [DllImport(DLL, EntryPoint = "SFileOpenArchive")]
        public static extern bool SFileOpenArchive(string fileName, int mPQID, int p3, ref int hMPQ);
        [DllImport(DLL, EntryPoint = "SFileCloseArchive")]
        public static extern bool SFileCloseArchive(int hMPQ);
        [DllImport(DLL, EntryPoint = "SFileGetArchiveName")]
        public static extern bool SFileGetArchiveName(int hMPQ, string lpBuffer, uint dwBufferLength);
        [DllImport(DLL, EntryPoint = "SFileOpenFile")]
        public static extern bool SFileOpenFile(string fileName, ref int hFile);
        [DllImport(DLL, EntryPoint = "SFileOpenFileEx")]
        public static extern bool SFileOpenFileEx(int hMPQ, string fileName, uint dwSearchScope, ref int hFile);
        [DllImport(DLL, EntryPoint = "SFileCloseFile")]
        public static extern bool SFileCloseFile(int hFile);
        [DllImport(DLL, EntryPoint = "SFileGetFileSize")]
        public static extern uint SFileGetFileSize(int hFile, ref uint highPartOfFileSize);
        [DllImport(DLL, EntryPoint = "SFileGetFileArchive")]
        public static extern bool SFileGetFileArchive(int hFile, ref int hMPQ);
        [DllImport(DLL, EntryPoint = "SFileGetFileName")]
        public static extern bool SFileGetFileName(int hFile, string lpBuffer, uint dwBufferLength);
        [DllImport(DLL, EntryPoint = "SFileSetFilePointer")]
        public static extern uint SFileSetFilePointer(int hFile, long lDistanceToMove, ulong lplDistanceToMoveHigh, uint dwMoveMethod);
        [DllImport(DLL, EntryPoint = "SFileReadFile")]
        public static extern bool SFileReadFile(int hFile, byte[] buffer, uint numberOfBytesToRead, ref uint numberOfBytesRead, int overlapped);
        [DllImport(DLL, EntryPoint = "SFileSetLocale")]
        public static extern uint SFileSetLocale(uint nNewLocale);
        [DllImport(DLL, EntryPoint = "SFileGetBasePath")]
        public static extern bool SFileGetBasePath(string lpBuffer, uint dwBufferLength);
        [DllImport(DLL, EntryPoint = "SFileSetBasePath")]
        public static extern bool SFileSetBasePath(string lpNewBasePath);

        // Extra storm-related functions
        [DllImport(DLL, EntryPoint = "SFileGetFileInfo")]
        public static extern uint SFileGetFileInfo(int hFile, uint dwInfoType);
        [DllImport(DLL, EntryPoint = "SFileSetArchivePriority")]
        public static extern bool SFileSetArchivePriority(int hFile, uint dwPriority);
        [DllImport(DLL, EntryPoint = "SFileFindMpqHeader")]
        public static extern uint SFileFindMpqHeader(int hFile);

        // Archive editing functions implemented by this library
        [DllImport(DLL, EntryPoint = "MpqOpenArchiveForUpdate")]
        public static extern int MpqOpenArchiveForUpdate(string lpFileName, uint dwFlags, uint dwMaximumFilesInArchive);
        [DllImport(DLL, EntryPoint = "MpqCloseUpdatedArchive")]
        public static extern uint MpqCloseUpdatedArchive(int hMPQ, uint dwUnknown2);
        [DllImport(DLL, EntryPoint = "MpqAddFileToArchive")]
        public static extern bool MpqAddFileToArchive(int hMPQ, string lpSourceFileName, string lpDestFileName, uint dwFlags);
        [DllImport(DLL, EntryPoint = "MpqAddWaveToArchive")]
        public static extern bool MpqAddWaveToArchive(int hMPQ, string lpSourceFileName, string lpDestFileName, uint dwFlags, uint dwQuality);
        [DllImport(DLL, EntryPoint = "MpqRenameFile")]
        public static extern bool MpqRenameFile(int hMPQ, string lpcOldFileName, string lpcNewFileName);
        [DllImport(DLL, EntryPoint = "MpqDeleteFile")]
        public static extern bool MpqDeleteFile(int hMPQ, string lpFileName);
        [DllImport(DLL, EntryPoint = "MpqCompactArchive")]
        public static extern bool MpqCompactArchive(int hMPQ);

        // Extra archive editing functions
        [DllImport(DLL, EntryPoint = "MpqAddFileToArchiveEx")]
        public static extern bool MpqAddFileToArchiveEx(int hMPQ, string source, string dest, uint dwFlags, uint dwCompressionType, uint dwCompressLevel);
        /*[DllImport(DLL, EntryPoint = "MpqAddFileFromBufferEx")]
        public static extern bool MpqAddFileFromBufferEx(int hMPQ, ref void * lpBuffer, uint dwLength, string lpFileName, uint dwFlags, uint dwCompressionType, uint dwCompressLevel);
        [DllImport(DLL, EntryPoint = "MpqAddFileFromBuffer")]
        public static extern bool MpqAddFileFromBuffer(int hMPQ, ref void* lpBuffer, uint dwLength, string lpFileName, uint dwFlags);
        [DllImport(DLL, EntryPoint = "MpqAddWaveFromBuffer")]
        public static extern bool MpqAddWaveFromBuffer(int hMPQ, ref void* lpBuffer, uint dwLength, string lpFileName, uint dwFlags, uint dwQuality);
        */[DllImport(DLL, EntryPoint = "MpqSetFileLocale")]
        public static extern bool MpqSetFileLocale(uint hMPQ, uint lpFileName, uint nOldLocale, uint nNewLocale);
        [DllImport(DLL, EntryPoint = "SFileDestroy")]
        public static extern bool SFileDestroy();
        [DllImport(DLL, EntryPoint = "StormDestroy")]
        public static extern void StormDestroy();

        public static void MpqExtractFileTo(int hMPQ, string fileName, string output)
        {
            int hFile = -1;
            if (SFileOpenFile(fileName, ref hFile))
            {
                uint fileSizeHigh = 0;
                uint fileSize = SFileGetFileSize(hFile, ref fileSizeHigh);
                if ((fileSizeHigh == 0) && (fileSize > 0))
                {
                    byte[] bs = new byte[fileSize];
                    uint countRead = 0;
                    SFileReadFile(hFile, bs, fileSize, ref countRead, 0);
                    FileStream F = new FileStream(output, FileMode.Create, FileAccess.ReadWrite);
                    F.Write(bs, 0, bs.Length);
                    F.Close();
                    SFileCloseFile(hFile);
                }
            }
        }

    }