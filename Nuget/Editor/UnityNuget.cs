using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace BoysheO.Nuget.Editor
{
    public static class UnityNuget
    {
        #region struct

        private struct PckInf
        {
            public string Id;
            public string Version;
        }

        private struct PckCached
        {
            public string Id;
            public string Ver;

            /// <summary>
            /// must exist when the PckInCache creat
            /// </summary>
            public string Folder;
        }

        private struct PckInstalled
        {
            public string Id;
            public string Ver;
            public string Folder;
        }

        #endregion

        private static readonly Regex _getIdAndVerFromFolderNameRegex =
            new Regex(@"^(?<Id>[a-zA-Z0-9.]+).(?<Ver>[0-9]+.[0-9]+.[0-9]+)$", RegexOptions.Compiled);

        /// <summary>
        /// tell UnityNuget select which net version package has
        /// </summary>
        private static readonly string[] NetVerOrder = new string[]
        {
            "netstandard2.0",
            "netstandard1.6",
            "netstandard1.5",
            "netstandard1.4",
            "netstandard1.3",
            "netstandard1.2",
            "netstandard1.1",
            "netstandard1.0",
            "netstandard2.1",
            "netstandard2.1",
        };

        private static string PackagesInstallationFolder => Application.dataPath + "/Plugins/Nuget/";

        private static XDocument LoadPackagesConfig()
        {
            var path = Application.dataPath + "/../packages.config";
            var xml = XDocument.Load(path);
            return xml;
        }

        private static IEnumerable<PckInf> AsPackageInfoEnumerable(this XDocument xmlDoc)
        {
            return xmlDoc.Root!.Elements()
                .Select(e => new PckInf()
                {
                    Id = e.Attribute("id").Value,
                    Version = e.Attribute("version").Value,
                });
        }

        private static IEnumerable<PckCached> ProjectPackages
        {
            get
            {
                string GetAppPath()
                {
                    const int assetsLength = 7;
                    var assets = Application.dataPath;
                    return assets.Remove(assets.Length - assetsLength, assetsLength);
                }

                var packageCache = LoadPackagesConfig()
                    .AsPackageInfoEnumerable()
                    .Select(v => new PckCached
                    {
                        Id = v.Id,
                        Ver = v.Version,
                        Folder = string.Format("{0}/Packages/{1}.{2}", GetAppPath(), v.Id, v.Version)
                    })
                    .Where(v => Directory.Exists(v.Folder));
                return packageCache;
            }
        }

        private static IEnumerable<PckInstalled> InstalledPackages
        {
            get
            {
                var installedPath = PackagesInstallationFolder;
                if (!Directory.Exists(installedPath)) return Enumerable.Empty<PckInstalled>();
                return Directory.GetDirectories(installedPath)
                    .Select(v =>
                        {
                            var dirName = Path.GetFileName(v);
                            if (dirName is null) return default;
                            try
                            {
                                var match = _getIdAndVerFromFolderNameRegex.Match(dirName);
                                return new PckInstalled()
                                {
                                    Id = match.Groups["Id"].Value,
                                    Ver = match.Groups["Ver"].Value,
                                    Folder = v,
                                };
                            }
                            catch
                            {
                                return default;
                            }
                        }
                    )
                    .Where(v => !string.IsNullOrWhiteSpace(v.Id));
            }
        }

        [MenuItem("Nuget/Restore")]
        private static void Restore()
        {
            Debug.Log("Nuget Restoring...");

            #region 检查是否打开了强制程序集版本

            if (PlayerSettings.assemblyVersionValidation)
            {
                Debug.Log("AssemblyVersionValidation detected.Script has already set it false.\nAssemblyVersionValidation is useless in Unity and crash usually( most of packages from nuget),about AssemblyVersionValidation you can learn more from google.com.");
                PlayerSettings.assemblyVersionValidation = false;
            }

            #endregion

            var slnPcks = ProjectPackages;
            var u3dPckFolder = PackagesInstallationFolder;
            if (!Directory.Exists(u3dPckFolder)) Directory.CreateDirectory(u3dPckFolder);
            var u3dPcks = InstalledPackages.ToArray();

            //清理没有引入的包
            var unusedPck = u3dPcks
                .Where(installed => !slnPcks.Any(cache => installed.Id == cache.Id && installed.Ver == cache.Ver));

            foreach (var pck in unusedPck)
            {
                // Debug.Log($"Del - {pck.Folder}");
                Directory.Delete(pck.Folder, true);
                File.Delete($"{pck.Folder}.meta");
            }

            foreach (var slnPck in slnPcks)
            {
                if (u3dPcks.FirstOrDefault(v => v.Id == slnPck.Id && v.Ver == slnPck.Ver).Id != null)
                {
                    // Debug.Log("already exist");
                    continue;
                }

                //确定要复制到unity中的文件夹，它一般是/Assets/../Packages/{Id}.{Ver}/netstandard2.0
                var folderReadyToCopy = NetVerOrder
                    .Select(netVer => new
                    {
                        slnPck.Id,
                        slnPck.Ver,
                        PckFolder = slnPck.Folder,
                        NetVer = netVer,
                        PckDirWithNetVer = slnPck.Folder + "/lib/" + netVer
                    })
                    .FirstOrDefault(v => Directory.Exists(v.PckDirWithNetVer));

                if (folderReadyToCopy is null)
                {
                    Debug.LogError(
                        string.Format("[{0}.{1}]Unable to find the appropriate net version to use in Unity!", slnPck.Id,
                            slnPck.Ver));
                    continue;
                }

                foreach (string files in Directory.GetFiles(folderReadyToCopy.PckDirWithNetVer))
                {
                    var fileName = Path.GetFileName(files);
                    var destFolder = PackagesInstallationFolder +
                                     string.Format("/{0}.{1}/lib/{2}", folderReadyToCopy.Id, folderReadyToCopy.Ver,
                                         folderReadyToCopy.NetVer);
                    if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);
                    var destFile = string.Format("{0}/{1}", destFolder, fileName);
                    // Debug.Log($"copy {files} to {destFile}");
                    File.Copy(files, destFile, true);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log("Nuget Restore done!");
        }
    }
}