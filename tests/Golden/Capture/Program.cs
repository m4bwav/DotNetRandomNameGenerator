#nullable disable

// Golden capture of the PUBLISHED RandomNameGeneratorLibrary 2.2.0 (package-modernize retrofit, Phase 0).
// Usage: dotnet run -c Release -f net48 -- <output.json>   (or -f net10.0)
// Writes UTF-8 without BOM and LF line endings. The header proves which DLL answered (SHA-256 of the loaded file,
// comparable with the lib/ folders of the nupkg on nuget.org). Never edit a recording; never regenerate it from new code.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using RandomNameGeneratorLibrary;

namespace GoldenCapture
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("usage: Capture <output.json>");
                return 2;
            }

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            var cases = Cases.Run();

            var old = typeof(PersonNameGenerator).Assembly;
            string sha;
            using (var h = SHA256.Create())
            using (var f = File.OpenRead(old.Location))
            {
                sha = string.Concat(h.ComputeHash(f).Select(b => b.ToString("x2", CultureInfo.InvariantCulture)));
            }

            var os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "windows" : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "macos" : "linux";

            var root = new JsonObject();
            root.Add("package", "RandomNameGeneratorLibrary@2.2.0");
            root.Add("assembly", old.GetName().Name + " " + old.GetName().Version);
            root.Add("assemblySha256", sha);
            root.Add("runtime", RuntimeInformation.FrameworkDescription);
            root.Add("os", os);
            root.Add("culture", "invariant");
            root.Add("captured", DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            root.Add("note", "Golden outputs of the published RandomNameGeneratorLibrary 2.2.0, recorded by tests/Golden/Capture. Never edit; never regenerate from new code.");
            root.Add("caseCount", cases.Count);
            var list = new List<object>();
            foreach (var c in cases)
            {
                var o = new JsonObject();
                o.Add("group", c.Group);
                o.Add("name", c.Name);
                o.Add("result", c.Result);
                list.Add(o);
            }

            root.Add("cases", list);

            File.WriteAllText(args[0], Json.Write(root), new UTF8Encoding(false));
            Console.Error.WriteLine("wrote " + cases.Count + " cases to " + args[0]);
            return 0;
        }
    }
}
