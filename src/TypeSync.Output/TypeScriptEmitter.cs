﻿using System;
using System.IO;
using TypeSync.Common.Extensions;

namespace TypeSync.Output
{
    public class TypeScriptEmitter
    {
        public void Emit(string path, string className, string contents)
        {
            string fileType = "model";
            string extension = "ts";

            string fileName = String.Format("{0}.{1}.{2}", className.PascalToKebabCase(), fileType, extension);

            File.WriteAllText(Path.Combine(path, fileName), contents);
        }
    }
}