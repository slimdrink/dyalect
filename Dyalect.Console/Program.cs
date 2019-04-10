﻿using Dyalect.Command;
using Dyalect.Compiler;
using Dyalect.Linker;
using Dyalect.Parser;
using Dyalect.Runtime;
using System;
using System.IO;

namespace Dyalect
{
    public static class Program
    {
        private static OptionBag options;

        static void Main(string[] args)
        {
            options = OptionDispatcher.Dispatch<OptionBag>();

            var buildOptions = new BuilderOptions
            {
                Debug = options.Debug,
                NoLangModule = options.NoLang
            };

            var lookup = FileLookup.Create(Path.GetDirectoryName(options.StartupPath), options.Paths);
            var linker = new DyLinker(lookup, buildOptions);

            var made = linker.Make(SourceBuffer.FromFile(options.DefaultArgument));

            if (!made.Success)
                foreach (var m in made.Messages)
                    Console.WriteLine(m);

            var dym = new DyMachine(made.Value);
            var res = dym.Execute();
            Console.WriteLine(res.Value);
        }
    }
}