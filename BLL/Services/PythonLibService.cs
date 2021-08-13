using BLL.Helpers;
using BLL.Interfaces;
using IronPython.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Scripting.Hosting;
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

        public int CalcArraySum(int[] arr, bool useIronPython = true)
        {
            if(useIronPython)
            {
                ScriptEngine engine = Python.CreateEngine();
                ScriptScope scope = engine.CreateScope();
                //string arg = 
                engine.ExecuteFile(Path.Combine(Directory.GetCurrentDirectory(), "Python",
                    _appSettings.ExternLibConfig["FileNameForCalcArraySumIP"]), scope);
                dynamic function = scope.GetVariable("calc_arr_sum");
                // exec func and recive answer
                dynamic result = function(arr);
                return result;
            }
            else
            {
                using Process process = Process.Start(new ProcessStartInfo
                {
                    FileName = _appSettings.ExternLibConfig["PathToPython"],
                    Arguments = Path.Combine(Directory.GetCurrentDirectory(), "Python",
                    _appSettings.ExternLibConfig["FileNameForCalcArraySum"]),
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = false
                });



                using BinaryWriter writer = new BinaryWriter(process.StandardInput.BaseStream);
                Array.ForEach(arr, writer.Write);
                writer.Flush();
                using BinaryReader reader = new BinaryReader(process.StandardOutput.BaseStream);
                return reader.ReadInt32();
            }            
        }

    }
}
