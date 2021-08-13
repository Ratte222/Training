using BLL.Helpers;
using BLL.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace BLL.Services
{
    public class PythonLibService : IPythonLibService
    {
        private readonly AppSettings _appSettings;

        public PythonLibService(IOptions<AppSettings> options)
        {
            _appSettings = options.Value;
        }

        public int CalcArraySum(int[] arr)
        {
            using Process process = Process.Start(new ProcessStartInfo
            {
                FileName = _appSettings.ExternLibConfig["PathToPython"],
                Arguments = Path.Combine(Directory.GetCurrentDirectory(), "Python", 
                    _appSettings.ExternLibConfig["FileNameForCalcArraySum"]),
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow= false
            });

            

            using BinaryWriter writer = new BinaryWriter(process.StandardInput.BaseStream);
            Array.ForEach(arr, writer.Write);
            writer.Flush();
            using BinaryReader reader = new BinaryReader(process.StandardOutput.BaseStream);
            int result = reader.ReadInt32();
            return result;
        }
    }
}
