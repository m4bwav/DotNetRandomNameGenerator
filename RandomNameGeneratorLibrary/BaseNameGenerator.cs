using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace RandomNameGeneratorLibrary
{
    public abstract class BaseNameGenerator
    {
        private const string ResourcePathPrefix = "RandomNameGenerator.Resources.";
        protected readonly Random RandGen;

        protected BaseNameGenerator()
        {
            RandGen = new Random();
        }

        private static Stream ReadResourceStreamForFileName(string resourceFileName)
        {
            return
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream(ResourcePathPrefix + resourceFileName);
        }

        protected static string[] ReadResourceByLine(string resourceFileName)
        {
            var stream = ReadResourceStreamForFileName(resourceFileName);

            var list = new List<string>();

            var streamReader = new StreamReader(stream);
            string str;

            while ((str = streamReader.ReadLine()) != null)
            {
                if (str != string.Empty)
                    list.Add(str);
            }

            return list.ToArray();
        }
    }
}