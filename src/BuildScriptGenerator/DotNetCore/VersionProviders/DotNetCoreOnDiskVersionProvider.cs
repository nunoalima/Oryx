﻿// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Microsoft.Oryx.BuildScriptGenerator.DotNetCore
{
    public class DotNetCoreOnDiskVersionProvider : IDotNetCoreVersionProvider
    {
        private readonly ILogger<DotNetCoreOnDiskVersionProvider> logger;

        public DotNetCoreOnDiskVersionProvider(ILogger<DotNetCoreOnDiskVersionProvider> logger)
        {
            this.logger = logger;
        }

        public string GetDefaultRuntimeVersion()
        {
            var defaultRuntimeVersion = DotNetCoreRunTimeVersions.NetCoreApp31;

            this.logger.LogDebug("Default runtime version is {defaultRuntimeVersion}", defaultRuntimeVersion);

            return defaultRuntimeVersion;
        }

        public Dictionary<string, string> GetSupportedVersions()
        {
            var versionMap = new Dictionary<string, string>();

            this.logger.LogDebug(
                "Getting list of supported runtime and their sdk versions from {installationDir}",
                DotNetCoreConstants.DefaultDotNetCoreSdkVersionsInstallDir);

            // Example:
            // /opt/dotnet/
            //      1.1.100
            //      2.1.810
            //      all
            var dotNetCoreVersionDirs = Directory.GetDirectories(
                DotNetCoreConstants.DefaultDotNetCoreSdkVersionsInstallDir);
            foreach (var sdkVersionDirPath in dotNetCoreVersionDirs)
            {
                var sdkVersionDir = new DirectoryInfo(sdkVersionDirPath);

                try
                {
                    var version = new SemanticVersioning.Version(sdkVersionDir.Name);
                }
                catch
                {
                    // Ignore directory names like 'all', 'lts' etc.
                    continue;
                }

                var netCoreAppDirPath = Path.Combine(sdkVersionDirPath, "shared", "Microsoft.NETCore.App");

                Console.Write($"Path : {netCoreAppDirPath}");
                if (Directory.Exists(netCoreAppDirPath))
                {
                    Console.WriteLine($"Entered here in {netCoreAppDirPath}");
                    var runtimeVersionDirNames = Directory.GetDirectories(netCoreAppDirPath);
                    foreach (var runtimeVersionDirPath in runtimeVersionDirNames)
                    {
                        Console.WriteLine($" Entered for loop for dotnetcore : {runtimeVersionDirPath}");
                        var runtimeVersionDir = new DirectoryInfo(runtimeVersionDirPath);
                        Console.WriteLine($"Name of runtime version dir : {runtimeVersionDir.Name}");
                        versionMap[runtimeVersionDir.Name] = sdkVersionDir.Name;
                    }
                }
            }

            foreach (var map in versionMap)
            {
                Console.WriteLine("Entered the foreach loop for dotnet");
                Console.WriteLine($" Key : {map.Key} and Value : {map.Value}");
            }

            return versionMap;
        }
    }
}
