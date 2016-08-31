﻿using System.Collections.Generic;
using System.Reflection;

namespace CommandRunner
{
    public class CommandMethodReflector
    {
        public static IEnumerable<MethodInfo> GetCommandMethods(IConfigureScanning scanningConfiguration)
        {
            if (scanningConfiguration.ScanAllAssemblies)
            {
                return Reflection.ReflectAllAssemblies();
            }
            else if (scanningConfiguration.SpecificAssembliesToScan != null && scanningConfiguration.SpecificAssembliesToScan.Count > 0)
            {
                return Reflection.ReflectAssemblies(scanningConfiguration.SpecificAssembliesToScan);
            }
            else if (scanningConfiguration.SpecificTypesToScan != null &&
                     scanningConfiguration.SpecificTypesToScan.Count > 0)
            {
                return Reflection.ReflectTypes(scanningConfiguration.SpecificTypesToScan);
            }
            return null;
        }
    }
}