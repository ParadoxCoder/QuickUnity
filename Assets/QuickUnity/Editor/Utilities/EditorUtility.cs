﻿/*
 *	The MIT License (MIT)
 *
 *	Copyright (c) 2016 Jerry Lee
 *
 *	Permission is hereby granted, free of charge, to any person obtaining a copy
 *	of this software and associated documentation files (the "Software"), to deal
 *	in the Software without restriction, including without limitation the rights
 *	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *	copies of the Software, and to permit persons to whom the Software is
 *	furnished to do so, subject to the following conditions:
 *
 *	The above copyright notice and this permission notice shall be included in all
 *	copies or substantial portions of the Software.
 *
 *	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *	SOFTWARE.
 */

using QuickUnity.Core.Miscs;
using QuickUnity.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using QuickUnity.Extensions;

namespace QuickUnityEditor.Utilities
{
    /// <summary>
    /// Miscellaneous helper stuff for Editor.
    /// </summary>
    public static class EditorUtility
    {
        /// <summary>
        /// The extension of ScriptableObject asset.
        /// </summary>
        private static readonly string s_scriptableObjectAssetExtension = ".asset";

        /// <summary>
        /// Loads the asset of ScriptableObject.
        /// </summary>
        /// <typeparam name="T">The type definition of ScriptableObject.</typeparam>
        /// <param name="path">The path.</param>
        /// <param name="assetName">Name of the asset.</param>
        /// <returns>The ScriptableObject of type definition.</returns>
        public static T LoadScriptableObjectAsset<T>(string path, string assetName = null) where T : ScriptableObject
        {
            if (string.IsNullOrEmpty(assetName))
            {
                assetName = typeof(T).Name;
            }

            string assetPath = Path.Combine(path, assetName + s_scriptableObjectAssetExtension);
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }

        /// <summary>
        /// Creates the asset of ScriptableObject.
        /// </summary>
        /// <typeparam name="T">The type definition of ScriptableObject.</typeparam>
        /// <param name="path">The path.</param>
        /// <param name="assetName">Name of the asset.</param>
        /// <returns>The new instance of ScriptableObject of type definition.</returns>
        public static T CreateScriptableObjectAsset<T>(string path, string assetName = null) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            SaveScriptableObjectAsset(path, asset, assetName);
            return asset;
        }

        /// <summary>
        /// Saves the asset of ScriptableObject.
        /// </summary>
        /// <typeparam name="T">The type definition of ScriptableObject.</typeparam>
        /// <param name="path">The path.</param>
        /// <param name="scriptableObject">The object of ScriptableObject.</param>
        /// <param name="assetName">Name of the asset.</param>
        public static void SaveScriptableObjectAsset<T>(string path, T scriptableObject, string assetName = null) where T : ScriptableObject
        {
            if (!string.IsNullOrEmpty(path))
            {
                int index = path.IndexOf(QuickUnityEditorApplication.AssetsFolderName);

                if (index == 0)
                {
                    if (string.IsNullOrEmpty(assetName))
                    {
                        assetName = typeof(T).Name;
                    }

                    assetName += s_scriptableObjectAssetExtension;
                    string assetPath = Path.Combine(path, assetName);

                    T targetAsset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

                    if (targetAsset != null)
                    {
                        targetAsset = scriptableObject;
                        UnityEditor.EditorUtility.SetDirty(targetAsset);
                    }
                    else
                    {
                        AssetDatabase.CreateAsset(scriptableObject, assetPath);
                    }

                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                }
            }
        }

        /// <summary>
        /// Converts absolute path to asset path.
        /// </summary>
        /// <param name="absolutePath">The absolute path.</param>
        /// <returns>The asset path for project.</returns>
        public static string ConvertToAssetPath(string absolutePath)
        {
            string projectPath = Application.dataPath;

            if (!string.IsNullOrEmpty(absolutePath) && absolutePath.IndexOf(projectPath) != -1)
            {
                int index = absolutePath.IndexOf(QuickUnityEditorApplication.AssetsFolderName);

                if (index != -1)
                {
                    return absolutePath.Substring(index);
                }
            }

            return absolutePath;
        }

        /// <summary>
        /// Gets the asset paths.
        /// </summary>
        /// <param name="nameFilter">The name filter.</param>
        /// <param name="typeFilter">The type filter.</param>
        /// <param name="searchInFolders">The search in folders.</param>
        /// <returns>System.String[]. The paths about this asset name.</returns>
        public static string[] GetAssetPath(string nameFilter, string typeFilter = null, string[] searchInFolders = null)
        {
            if (string.IsNullOrEmpty(nameFilter))
                return null;

            string[] assetPaths = null;
            string fileter = nameFilter;

            if (!string.IsNullOrEmpty(typeFilter))
                fileter = string.Concat(fileter, string.Concat(" ", typeFilter));

            string[] guids = AssetDatabase.FindAssets(fileter, searchInFolders);

            if (guids != null && guids.Length > 0)
            {
                assetPaths = new string[guids.Length];

                for (int i = 0, length = guids.Length; i < length; ++i)
                {
                    string guid = guids[i];
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        assetPaths[i] = assetPath;
                    }
                }
            }

            return assetPaths;
        }

        /// <summary>
        /// Gets the object assets.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String[] The paths of object assets.</returns>
        public static string[] GetObjectAssets(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            List<string> targetAssetPaths = new List<string>();

            if (Directory.Exists(path))
            {
                string[] guids = AssetDatabase.FindAssets("", new string[1] { path });

                for (int i = 0, length = guids.Length; i < length; ++i)
                {
                    string guid = guids[i];
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                    if (!Directory.Exists(assetPath))
                    {
                        targetAssetPaths.AddUnique(assetPath);
                    }
                }
            }
            else
            {
                targetAssetPaths.AddUnique(path);
            }

            return targetAssetPaths.ToArray();
        }

        /// <summary>
        /// Gets the dependencies map.
        /// </summary>
        /// <param name="targetAssets">The target assets.</param>
        /// <param name="recursive">
        /// If <c>false</c>, return only assets which are direct dependencies of the input; if
        /// <c>true</c>, include all indirect dependencies of the input. Defaults to true.
        /// </param>
        /// <returns>Dictionary&lt;System.String, System.String[]&gt; The dependencies map.</returns>
        public static Dictionary<string, string[]> GetDependenciesMap(List<string> targetAssets, bool recursive = true)
        {
            if (targetAssets == null)
            {
                return null;
            }

            Dictionary<string, string[]> referencesMap = new Dictionary<string, string[]>();

            for (int i = 0, length = targetAssets.Count; i < length; ++i)
            {
                string targetAsset = targetAssets[i];

                if (!string.IsNullOrEmpty(targetAsset))
                {
                    List<string> dependenciesList = new List<string>(AssetDatabase.GetDependencies(targetAsset, recursive));

                    if (dependenciesList.Contains(targetAsset))
                    {
                        dependenciesList.Remove(targetAsset);
                    }

                    if (dependenciesList != null)
                    {
                        string[] dependencies = dependenciesList.ToArray();

                        if (dependencies != null && dependencies.Length > 0)
                        {
                            referencesMap.AddUnique(targetAsset, dependenciesList.ToArray());
                        }
                    }
                }
            }

            return referencesMap;
        }

        /// <summary>
        /// Deletes the meta file by asset file path.
        /// </summary>
        /// <param name="assetFilePath">The asset file path.</param>
        public static void DeleteMetaFile(string assetFilePath)
        {
            string metaFilePath = assetFilePath + QuickUnityEditorApplication.MetaFileExtension;

            if (File.Exists(metaFilePath))
            {
                try
                {
                    File.Delete(metaFilePath);
                }
                catch (Exception exception)
                {
                    DebugLogger.LogException(exception);
                }
            }
        }

        /// <summary>
        /// Gets the size of the asset runtime memory.
        /// </summary>
        /// <param name="assetPath">The asset path.</param>
        /// <returns>The runtime memory size of this asset object.</returns>
        public static int GetAssetRuntimeMemorySize(string assetPath)
        {
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
            return GetAssetRuntimeMemorySize(asset);
        }

        /// <summary>
        /// Gets the size of the asset runtime memory.
        /// </summary>
        /// <param name="asset">The asset object.</param>
        /// <returns>The runtime memory size of this asset object.</returns>
        public static int GetAssetRuntimeMemorySize(UnityEngine.Object asset)
        {
            return Profiler.GetRuntimeMemorySize(asset);
        }

        /// <summary>
        /// Gets the size of the asset storage memory.
        /// </summary>
        /// <param name="assetPath">The asset path.</param>
        /// <returns>The storage memory size of this asset object.</returns>
        public static long GetAssetStorageMemorySize(string assetPath)
        {
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
            return GetAssetStorageMemorySize(asset);
        }

        /// <summary>
        /// Gets the size of the asset storage memory.
        /// </summary>
        /// <param name="asset">The asset object.</param>
        /// <returns>The storage memory size of this asset object.</returns>
        public static long GetAssetStorageMemorySize(UnityEngine.Object asset)
        {
            long size = 0;

            if (asset is Texture)
            {
                size = (int)ReflectionUtility.InvokeStaticMethod("UnityEditor.TextureUtil", "GetStorageMemorySize", new object[] { asset });
            }
            else
            {
                string path = AssetDatabase.GetAssetPath(asset);

                if (!string.IsNullOrEmpty(path))
                {
                    FileInfo fileInfo = new FileInfo(path);

                    if (fileInfo != null)
                    {
                        size = fileInfo.Length;
                    }
                }
            }

            return size;
        }

        /// <summary>
        /// Clears messages of console.
        /// </summary>
        public static void ClearConsole()
        {
            ReflectionUtility.InvokeStaticMethod("UnityEditorInternal.LogEntries", "Clear");
        }
    }
}