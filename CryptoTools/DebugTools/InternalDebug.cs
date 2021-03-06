﻿#if DEBUG

using System;
using System.Globalization;
using System.IO;

namespace FactaLogicaSoftware.CryptoTools.DebugTools
{
    public static class InternalDebug
    {
        public const string TempFilePath = @"CryptoTools\Debug";

        public static void WriteToDiagnosticsFile(params string[] items)
        {
            if (!Directory.Exists(TempFilePath))
            {
                Directory.CreateDirectory(TempFilePath);
            }

            if (!File.Exists(TempFilePath + "DiagnosticsAndDebug.data"))
            {
                File.Create(TempFilePath + "DiagnosticsAndDebug.data");
            }

            var handleToWrite = new FileStream(TempFilePath + "DiagnosticsAndDebug.data", FileMode.Append);
            using (var fWriter = new StreamWriter(handleToWrite))
            {
                fWriter.WriteLine('\n' + DateTime.Now.ToString(CultureInfo.CurrentCulture));
                foreach (string item in items)
                {
                    fWriter.WriteLine(item);
                }
            }
        }
    }
}

#endif