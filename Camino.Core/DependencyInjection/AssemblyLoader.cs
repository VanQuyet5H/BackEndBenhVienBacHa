using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.DependencyModel;

namespace Camino.Core.DependencyInjection
{
    public class AssemblyLoader : AssemblyLoadContext
    {
        public readonly string FolderFullPath;

        public AssemblyLoader(string folderFullPath = null)
        {
            FolderFullPath = folderFullPath;

            // Update List Loaded Assembly
            var runtimeId = RuntimeEnvironment.GetRuntimeIdentifier();
            var listLoadedAssemblyName = DependencyContext.Default.GetRuntimeAssemblyNames(runtimeId).Where(p => p.Name.Contains("Camino"));
            //var listLoadedAssemblyNameTest = DependencyContext.Default.GetRuntimeAssemblyNames(runtimeId);

            foreach (var assemblyName in listLoadedAssemblyName)
                ListLoadedAssemblyName.Add(assemblyName);
        }

        public List<AssemblyName> ListLoadedAssemblyName { get; } = new List<AssemblyName>();

        /// <inheritdoc />
        /// <summary>
        ///     Load an assembly, if the assembly already loaded then return null 
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        protected override Assembly Load(AssemblyName assemblyName)
        {
            // Check if assembly already added by Dependency (Reference)
            if (ListLoadedAssemblyName.Any(x => string.Equals(x.Name, assemblyName.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return null;
            }

            // Load Assembly not yet load
            var assemblyFileInfo = new FileInfo($"{FolderFullPath}{Path.DirectorySeparatorChar}{assemblyName.Name}.dll");

            var assembly = File.Exists(assemblyFileInfo.FullName) ? LoadFromAssemblyPath(assemblyFileInfo.FullName) : Assembly.Load(assemblyName);

            // Add to loaded
            ListLoadedAssemblyName.Add(assembly.GetName());

            return assembly;
        }
    }
}