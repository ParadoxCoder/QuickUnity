﻿/*
 *	The MIT License (MIT)
 *
 *	Copyright (c) 2017 Jerry Lee
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

using Excel;
using QuickUnity;
using QuickUnity.Core.Miscs;
using QuickUnity.Data;
using QuickUnity.Extensions;
using QuickUnity.Utilities;
using QuickUnityEditor.Attributes;
using QuickUnityEditor.Data.Parsers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace QuickUnityEditor.Data
{
    /// <summary>
    /// Handle the process of data tables import.
    /// </summary>
    [InitializeOnEditorStartup]
    public static class DataImport
    {
        /// <summary>
        /// Struct DataTableRowInfo.
        /// </summary>
        private struct DataTableRowInfo
        {
            /// <summary>
            /// The field name.
            /// </summary>
            public string fieldName;

            /// <summary>
            /// The type string.
            /// </summary>
            public string type;

            /// <summary>
            /// The comments.
            /// </summary>
            public string comments;

            /// <summary>
            /// Initializes a new instance of the <see cref="DataTableRowInfo"/> struct.
            /// </summary>
            /// <param name="fieldName">Name of the field.</param>
            /// <param name="dataType">Type of the data.</param>
            /// <param name="comments">The comments.</param>
            public DataTableRowInfo(string fieldName, string type, string comments)
            {
                this.fieldName = fieldName;
                this.type = type;
                this.comments = comments;
            }
        }

        /// <summary>
        /// The specifiers collection of script template.
        /// </summary>
        private static class ScriptTemplateSpecifiers
        {
            /// <summary>
            /// The specifier of namespace.
            /// </summary>
            public const string NamespaceSpecifier = "#NAMESPACE#";

            /// <summary>
            /// The specifier of script name.
            /// </summary>
            public const string ScriptNameSpecifier = "#SCRIPTNAME#";

            /// <summary>
            /// The specifier of fields.
            /// </summary>
            public const string FieldsSpecifier = "#FIELDS#";
        }

        /// <summary>
        /// The messages collection of dialog.
        /// </summary>
        private static class DialogMessages
        {
            /// <summary>
            /// The message content of dataTableRowScriptsStorageLocation is null or empty.
            /// </summary>
            public const string DataTableRowScriptsStorageLocationNullMessage = "Please set the storage location of DataTableRow scripts first: [QuickUnity/DataTable/Preferences...]";

            /// <summary>
            /// The message of the content of template is null or empty.
            /// </summary>
            public const string CanNotFindTplFileMessage = "Can not find the template for DataTableRow scripts. Please make sure you already import it into the project!";

            /// <summary>
            /// The message of data import done.
            /// </summary>
            public const string DataImportDoneMessage = "Data import done!";

            /// <summary>
            /// The message of data import abort.
            /// </summary>
            public const string DataImportAbortMessage = "Data import abort!";
        }

        /// <summary>
        /// The title of import progress bar.
        /// </summary>
        private const string ImportProgressBarTitle = "Data Import Progress";

        /// <summary>
        /// The generating scripts progress bar information
        /// </summary>
        private const string GeneratingScriptsProgressBarInfo = "Generating Script file: {0}.cs... {1}/{2}";

        /// <summary>
        /// The saving data progress bar information
        /// </summary>
        private const string SavingDataProgressBarInfo = "Saving Data of {0}.. {1}/{2}";

        /// <summary>
        /// The file name of DataTableRow script template.
        /// </summary>
        private const string DataTableRowScriptTemplateFileName = "NewDataTableRowScript";

        /// <summary>
        /// The extension of excel file.
        /// </summary>
        private const string ExcelFileExtension = ".xls";

        /// <summary>
        /// The extension of script file.
        /// </summary>
        private const string ScriptFileExtension = ".cs";

        /// <summary>
        /// The extension of box database file.
        /// </summary>
        public const string BoxDbFileExtension = ".box";

        /// <summary>
        /// The extension of database configuration file.
        /// </summary>
        private const string DbConfigFileExtension = ".swp";

        /// <summary>
        /// The map of data tables location.
        /// </summary>
        public static readonly Dictionary<DataTableStorageLocation, string> dataTablesLocationMap = new Dictionary<DataTableStorageLocation, string>()
        {
            { DataTableStorageLocation.PersistentDataPath, Path.Combine(Application.persistentDataPath, DataTableManager.DataTablesStorageFolderName) },
            { DataTableStorageLocation.ResourcesPath, Path.Combine(Path.Combine(Application.dataPath, QuickUnityEditorApplication.ResourcesFolderName), DataTableManager.DataTablesStorageFolderName) },
            { DataTableStorageLocation.StreamingAssetsPath, Path.Combine(Application.streamingAssetsPath, DataTableManager.DataTablesStorageFolderName) }
        };

        /// <summary>
        /// The search patterns of excel files.
        /// </summary>
        private static readonly string s_excelFileSearchPatterns = string.Format("*{0}", ExcelFileExtension);

        /// <summary>
        /// The preference key of scriptsGenerated.
        /// </summary>
        private static readonly string s_scriptsGeneratedPrefKey = string.Format("{0}_scriptsGenerated", PlayerSettings.productGUID.ToString());

        /// <summary>
        /// The preference key of excelFilesFolderPath.
        /// </summary>
        private static readonly string s_excelFilesFolderPathPrefKey = string.Format("{0}_excelFilesFolderPath", PlayerSettings.productGUID.ToString());

        /// <summary>
        /// The map of cached type parsers.
        /// </summary>
        private static Dictionary<Type, ITypeParser> s_cachedTypeParsersMap;

        /// <summary>
        /// The scripts generated.
        /// </summary>
        /// <value><c>true</c> if [scripts generated]; otherwise, <c>false</c>.</value>
        public static bool scriptsGenerated
        {
            get
            {
                return EditorPrefs.GetBool(s_scriptsGeneratedPrefKey, false);
            }

            set
            {
                EditorPrefs.SetBool(s_scriptsGeneratedPrefKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the excel files folder path.
        /// </summary>
        /// <value>The excel files folder path.</value>
        public static string excelFilesFolderPath
        {
            get
            {
                return EditorPrefs.GetString(s_excelFilesFolderPathPrefKey, null);
            }

            set
            {
                EditorPrefs.SetString(s_excelFilesFolderPathPrefKey, value);
            }
        }

        /// <summary>
        /// Initializes static members of the <see cref="DataImport"/> class.
        /// </summary>
        static DataImport()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        /// <summary>
        /// Delegate for generic updates.
        /// </summary>
        private static void OnEditorUpdate()
        {
            if (scriptsGenerated && !EditorApplication.isCompiling)
            {
                GenerateDBFiles();
                scriptsGenerated = false;
            }
        }

        /// <summary>
        /// Imports data.
        /// </summary>
        [MenuItem("Tools/QuickUnity/DataTable/Import Data...", false, 100)]
        [MenuItem("Assets/Import Data...", false, 20)]
        public static void Import()
        {
            if (CheckPreferencesData())
            {
                string filesFolderPath = EditorUtility.OpenFolderPanel("Import Data...", "", "");

                if (!string.IsNullOrEmpty(filesFolderPath))
                {
                    Utilities.EditorUtility.ClearConsole();
                    excelFilesFolderPath = filesFolderPath;
                    GenerateDataTableRowScripts(filesFolderPath);
                }
                else
                {
                    QuickUnityEditorApplication.DisplaySimpleDialog("", DialogMessages.DataImportAbortMessage);
                }
            }
        }

        /// <summary>
        /// Checks the preferences data.
        /// </summary>
        /// <returns><c>true</c> if preferences data is ready, <c>false</c> otherwise.</returns>
        private static bool CheckPreferencesData()
        {
            bool success = true;

            // Load preferences data or create default preferences data.
            DataTablePreferences preferencesData = DataTablePreferencesWindow.LoadPreferencesData();

            if (!preferencesData)
            {
                preferencesData = DataTablePreferencesWindow.CreateDefaultPreferencesData();
                DataTablePreferencesWindow.SavePreferenceData(preferencesData);
            }

            // Check preferences data is safe.
            if (string.IsNullOrEmpty(preferencesData.dataTableRowScriptsStorageLocation))
            {
                QuickUnityEditorApplication.DisplaySimpleDialog("", DialogMessages.DataTableRowScriptsStorageLocationNullMessage, () =>
                {
                    success = false;
                    DataTablePreferencesWindow.ShowEditorWindow();
                });
            }

            return success;
        }

        /// <summary>
        /// Gets the text content of template.
        /// </summary>
        /// <returns>The text content of template.</returns>
        private static string GetTplText()
        {
            string[] assetPaths = Utilities.EditorUtility.GetAssetPath(DataTableRowScriptTemplateFileName, "t:TextAsset");
            string tplPath = null;
            string tplText = null;

            if (assetPaths != null && assetPaths.Length > 0)
            {
                tplPath = assetPaths[0];
            }

            if (!string.IsNullOrEmpty(tplPath))
            {
                TextAsset tplAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(tplPath);

                if (tplAsset)
                {
                    tplText = tplAsset.text;
                }
            }
            else
            {
                QuickUnityEditorApplication.DisplaySimpleDialog("", DialogMessages.CanNotFindTplFileMessage);
            }

            return tplText;
        }

        /// <summary>
        /// Gets the namespace.
        /// </summary>
        /// <returns>The namespace of scripts.</returns>
        private static string GetNamespace()
        {
            string namespaceString = string.Empty;

            DataTablePreferences preferencesData = DataTablePreferencesWindow.LoadPreferencesData();

            if (preferencesData)
            {
                if (preferencesData.autoGenerateScriptsNamespace)
                {
                    // Generate namespace automatically.
                    namespaceString = GenerateNamespace(preferencesData);
                }
                else
                {
                    namespaceString = preferencesData.dataTableRowScriptsNamespace;
                }
            }

            // Handle empty namespace.
            if (string.IsNullOrEmpty(namespaceString))
            {
                namespaceString = DataTablePreferences.DefaultNamespace;
            }

            return namespaceString;
        }

        /// <summary>
        /// Generates the namespace.
        /// </summary>
        /// <param name="preferencesData">The preferences data.</param>
        /// <returns>The generated namespace string.</returns>
        private static string GenerateNamespace(DataTablePreferences preferencesData)
        {
            string namespaceString = string.Empty;

            if (preferencesData)
            {
                string path = preferencesData.dataTableRowScriptsStorageLocation;
                path = path.Replace(Path.AltDirectorySeparatorChar, '.');
                int index = path.IndexOf(QuickUnityEditorApplication.ScriptsFolderName);

                if (index != -1)
                {
                    if (index + QuickUnityEditorApplication.ScriptsFolderName.Length + 1 <= path.Length)
                    {
                        namespaceString = path.Substring(index + QuickUnityEditorApplication.ScriptsFolderName.Length + 1);
                    }
                }
                else
                {
                    index = path.IndexOf(QuickUnityEditorApplication.AssetsFolderName);

                    if (index != -1)
                    {
                        if (index + QuickUnityEditorApplication.AssetsFolderName.Length + 1 <= path.Length)
                        {
                            namespaceString = path.Substring(index + QuickUnityEditorApplication.AssetsFolderName.Length + 1);
                        }
                    }
                    else
                    {
                        namespaceString = path.Replace(":", "");
                    }
                }
            }

            return namespaceString;
        }

        /// <summary>
        /// Generates the list of DataTableRowInfo.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        /// <returns>The list of DataTableRowInfo.</returns>
        private static List<DataTableRowInfo> GenerateDataTableRowInfos(DataTable dataTable)
        {
            DataColumnCollection columns = dataTable.Columns;
            DataRowCollection rows = dataTable.Rows;
            int columnCount = columns.Count;
            List<DataTableRowInfo> infos = new List<DataTableRowInfo>();

            for (int i = 0; i < columnCount; i++)
            {
                string fieldName = rows[0][i].ToString().Trim();
                string type = rows[1][i].ToString().Trim();
                string comments = rows[2][i].ToString().Trim();

                if (!string.IsNullOrEmpty(fieldName) && !string.IsNullOrEmpty(type))
                {
                    DataTableRowInfo info = new DataTableRowInfo(fieldName, type, FormatCommentsString(comments));
                    infos.Add(info);
                }
            }

            return infos;
        }

        /// <summary>
        /// Generates the scripts of DataTableRows.
        /// </summary>
        /// <param name="excelFilesFolderPath">The folder path of excel files.</param>
        private static void GenerateDataTableRowScripts(string excelFilesFolderPath)
        {
            string tplText = GetTplText();
            string namespaceString = GetNamespace();

            ForEachExcelFile(excelFilesFolderPath, (DataTable table, string fileName, int index, int length) =>
            {
                EditorUtility.DisplayProgressBar(ImportProgressBarTitle,
                    string.Format(GeneratingScriptsProgressBarInfo, fileName, index + 1, length), (float)(index + 1) / length);
                List<DataTableRowInfo> rowInfos = GenerateDataTableRowInfos(table);
                GenerateDataTableRowScript((string)tplText.Clone(), namespaceString, fileName, rowInfos);
            });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            scriptsGenerated = true;
        }

        /// <summary>
        /// Generates the script of DataTableRow.
        /// </summary>
        /// <param name="tplText">The script template text.</param>
        /// <param name="namesapceString">The string of namesapce.</param>
        /// <param name="scriptName">Name of the script.</param>
        /// <param name="rowInfos">The list of DataTableRowInfo.</param>
        private static void GenerateDataTableRowScript(string tplText, string namesapceString, string scriptName, List<DataTableRowInfo> rowInfos)
        {
            tplText = tplText.Replace(ScriptTemplateSpecifiers.NamespaceSpecifier, namesapceString);
            tplText = tplText.Replace(ScriptTemplateSpecifiers.ScriptNameSpecifier, scriptName);
            tplText = tplText.Replace(ScriptTemplateSpecifiers.FieldsSpecifier, GenerateScriptFieldsString(rowInfos));

            DataTablePreferences preferencesData = DataTablePreferencesWindow.LoadPreferencesData();

            if (preferencesData)
            {
                if (!Directory.Exists(preferencesData.dataTableRowScriptsStorageLocation))
                {
                    Directory.CreateDirectory(preferencesData.dataTableRowScriptsStorageLocation);
                }

                string scriptFilePath = Path.Combine(preferencesData.dataTableRowScriptsStorageLocation, scriptName + ScriptFileExtension);
                UnityEngine.Object scriptAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(scriptFilePath);

                if (scriptAsset)
                {
                    EditorUtility.SetDirty(scriptAsset);
                }

                FileStream fileStream = null;
                StreamWriter writer = null;

                try
                {
                    fileStream = File.Open(scriptFilePath, FileMode.Create, FileAccess.Write);
                    writer = new StreamWriter(fileStream, new UTF8Encoding(true));
                    writer.Write(tplText);
                }
                catch (Exception exception)
                {
                    DebugLogger.LogException(exception);
                }
                finally
                {
                    if (writer != null)
                    {
                        writer.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Generates the database files.
        /// </summary>
        private static void GenerateDBFiles()
        {
            string path = excelFilesFolderPath;
            string namespaceString = GetNamespace();

            DeleteOldDBFiles();

            ForEachExcelFile(path, (DataTable table, string fileName, int index, int length) =>
            {
                EditorUtility.DisplayProgressBar(ImportProgressBarTitle,
                    string.Format(SavingDataProgressBarInfo, fileName, index + 1, length), (float)(index + 1) / length);
                List<DataTableRowInfo> rowInfos = GenerateDataTableRowInfos(table);
                List<DataTableRow> collection = GenerateDataTableRowCollection(table, namespaceString, fileName, rowInfos);

                string classFullName = string.Format("{0}.{1}", namespaceString, fileName);
                Type type = ProjectAssemblies.GetType(classFullName);

                if (type != null)
                {
                    ReflectionUtility.InvokeStaticGenericMethod(typeof(DataImport),
                            "SaveData",
                            type,
                            new object[] { fileName, rowInfos, collection, index });
                }
                else
                {
                    DebugLogger.LogErrorFormat(null, "Can not find the type: {0}", classFullName);
                }
            });

            DeleteDBConfigFiles();
            RenameDBFiles();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
            QuickUnityEditorApplication.DisplaySimpleDialog("", DialogMessages.DataImportDoneMessage);
        }

        /// <summary>
        /// Generates the data table row collection.
        /// </summary>
        /// <param name="table">The data table.</param>
        /// <param name="className">Name of the class.</param>
        /// <param name="rowInfos">The list of DataTableRow.</param>
        /// <returns>The data collection of data table row.</returns>
        private static List<DataTableRow> GenerateDataTableRowCollection(DataTable table, string namespaceString, string className, List<DataTableRowInfo> rowInfos)
        {
            List<DataTableRow> dataCollection = new List<DataTableRow>();
            string classFullName = className;

            if (!string.IsNullOrEmpty(namespaceString))
            {
                classFullName = string.Format("{0}.{1}", namespaceString, classFullName);
            }

            DataTablePreferences preferencesData = DataTablePreferencesWindow.LoadPreferencesData();

            if (preferencesData)
            {
                int rowCount = table.Rows.Count;

                for (int i = preferencesData.dataRowsStartRow - 1; i < rowCount; ++i)
                {
                    DataTableRow rowData = (DataTableRow)ReflectionUtility.CreateClassInstance(classFullName);

                    for (int j = 0, fieldsCount = rowInfos.Count; j < fieldsCount; ++j)
                    {
                        string cellValue = table.Rows[i][j].ToString().Trim();

                        if (!string.IsNullOrEmpty(cellValue))
                        {
                            DataTableRowInfo rowInfo = rowInfos[j];
                            ITypeParser typeParser = GetTypeParser(rowInfo.type);

                            if (typeParser != null)
                            {
                                object value = typeParser.Parse(cellValue);
                                ReflectionUtility.SetObjectFieldValue(rowData, rowInfo.fieldName, value);
                            }
                            else
                            {
                                Debug.LogWarningFormat("Type '{0}' is not supported!", rowInfo.type);
                            }
                        }
                    }

                    dataCollection.Add(rowData);
                }
            }

            return dataCollection;
        }

        /// <summary>
        /// Generates the script fields string.
        /// </summary>
        /// <param name="rowInfos">The row infos.</param>
        /// <returns>The fields string.</returns>
        private static string GenerateScriptFieldsString(List<DataTableRowInfo> rowInfos)
        {
            string fieldsString = string.Empty;

            if (rowInfos != null && rowInfos.Count > 0)
            {
                for (int i = 0, length = rowInfos.Count; i < length; ++i)
                {
                    DataTableRowInfo rowInfo = rowInfos[i];
                    fieldsString += string.Format("\t\t/// <summary>{0}\t\t/// {1}{2}\t\t/// </summary>{3}\t\tpublic {4} {5};",
                        Environment.NewLine,
                        rowInfo.comments,
                        Environment.NewLine,
                        Environment.NewLine,
                        rowInfo.type,
                        rowInfo.fieldName);

                    if (i < length - 1)
                        fieldsString += Environment.NewLine + Environment.NewLine;
                }
            }

            return fieldsString;
        }

        /// <summary>
        /// Format the comments string.
        /// </summary>
        /// <param name="comments">The comments.</param>
        /// <returns>The formatted comments string.</returns>
        private static string FormatCommentsString(string comments)
        {
            if (!string.IsNullOrEmpty(comments))
            {
                const string pattern = @"\r*\n";
                Regex rgx = new Regex(pattern);
                return rgx.Replace(comments, Environment.NewLine + "\t\t/// ");
            }

            return comments;
        }

        /// <summary>
        /// Gets the type parser.
        /// </summary>
        /// <param name="typeKeyword">The type keyword.</param>
        /// <returns>The type parser.</returns>
        private static ITypeParser GetTypeParser(string typeKeyword)
        {
            if (!string.IsNullOrEmpty(typeKeyword))
            {
                if (s_cachedTypeParsersMap == null)
                {
                    s_cachedTypeParsersMap = new Dictionary<Type, ITypeParser>();
                }

                Type type = TypeParserFactory.GetTypeParserType(typeKeyword);
                ITypeParser typeParser = null;

                if (type != null)
                {
                    if (s_cachedTypeParsersMap.ContainsKey(type))
                    {
                        typeParser = s_cachedTypeParsersMap[type];

                        if (typeParser == null)
                        {
                            typeParser = TypeParserFactory.CreateTypeParser(typeKeyword);
                            s_cachedTypeParsersMap[type] = typeParser;
                        }
                    }
                    else
                    {
                        typeParser = TypeParserFactory.CreateTypeParser(typeKeyword);
                        s_cachedTypeParsersMap.Add(type, typeParser);
                    }

                    return typeParser;
                }
            }

            return null;
        }

        /// <summary>
        /// Performs the specified action on each excel file under the folder path.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="action">
        /// The Action&lt;DataTable, string, int, int&gt; delegate to perform on each excel file.
        /// </param>
        private static void ForEachExcelFile(string folderPath, Action<DataTable, string, int, int> action = null)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return;
            }

            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
            FileInfo[] fileInfos = dirInfo.GetFiles(s_excelFileSearchPatterns, SearchOption.AllDirectories);

            for (int i = 0, length = fileInfos.Length; i < length; ++i)
            {
                FileInfo fileInfo = fileInfos[i];

                if (fileInfo != null)
                {
                    string filePath = fileInfo.FullName;
                    string fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    string fileExtension = Path.GetExtension(fileInfo.Name).ToLower();
                    IExcelDataReader excelReader = null;
                    FileStream fileStream = null;

                    try
                    {
                        fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read);

                        if (fileExtension == ExcelFileExtension)
                        {
                            // '97-2003 format; *.xls
                            excelReader = ExcelReaderFactory.CreateBinaryReader(fileStream);
                        }
                        else
                        {
                            // 2007 format; *.xlsx
                            excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                        }

                        if (excelReader != null)
                        {
                            DataSet result = excelReader.AsDataSet();
                            DataTable table = result.Tables[0];

                            if (action != null)
                            {
                                action.Invoke(table, fileName, i, length);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        DebugLogger.LogException(exception);
                    }
                    finally
                    {
                        if (excelReader != null)
                        {
                            excelReader.Close();
                        }

                        if (fileStream != null)
                        {
                            fileStream.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Deletes old database files.
        /// </summary>
        private static void DeleteOldDBFiles()
        {
            DataTablePreferences preferencesData = DataTablePreferencesWindow.LoadPreferencesData();

            if (preferencesData)
            {
                string path = dataTablesLocationMap[preferencesData.dataTablesStorageLocation];

                // If directory doesn't exist, create it.
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                DirectoryInfo dirInfo = new DirectoryInfo(path);
                FileInfo[] fileInfos = dirInfo.GetFiles();

                for (int i = 0, length = fileInfos.Length; i < length; ++i)
                {
                    FileInfo fileInfo = fileInfos[i];

                    try
                    {
                        fileInfo.Delete();
                    }
                    catch (Exception exception)
                    {
                        DebugLogger.LogException(exception);
                    }
                }
            }
        }

        /// <summary>
        /// Saves the data.
        /// </summary>
        /// <typeparam name="T">The type definition of data.</typeparam>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="rowInfos">The row infomation list.</param>
        /// <param name="collection">The data collection.</param>
        /// <param name="index">The index for database.</param>
        private static void SaveData<T>(string tableName, List<DataTableRowInfo> rowInfos, List<DataTableRow> collection, int index) where T : DataTableRow
        {
            string primayFieldName = rowInfos[0].fieldName;
            BoxDBAdapter addressMapDbAdapter = GetAddressMapDBAdapter();
            DataTableAddressMap addressMap = new DataTableAddressMap();
            addressMap.type = tableName;
            addressMap.primaryFieldName = primayFieldName;
            addressMap.localAddress = index + 2;
            bool success = addressMapDbAdapter.Insert(typeof(DataTableAddressMap).Name, addressMap);

            if (success)
            {
                string dbPath = GetDBFilesPath();
                BoxDBAdapter adpater = new BoxDBAdapter(dbPath, addressMap.localAddress);
                adpater.EnsureTable<T>(tableName, primayFieldName);
                adpater.Open();

                for (int i = 0, length = collection.Count; i < length; ++i)
                {
                    T data = (T)collection[i];

                    if (data != null)
                    {
                        success = adpater.Insert(tableName, data);
                    }
                }

                adpater.Dispose();
            }

            addressMapDbAdapter.Dispose();
        }

        /// <summary>
        /// Gets the database files path.
        /// </summary>
        /// <returns>The database files path.</returns>
        private static string GetDBFilesPath()
        {
            string path = dataTablesLocationMap[DataTableStorageLocation.PersistentDataPath];

            DataTablePreferences preferencesData = DataTablePreferencesWindow.LoadPreferencesData();

            if (preferencesData)
            {
                path = dataTablesLocationMap[preferencesData.dataTablesStorageLocation];
            }

            return path;
        }

        /// <summary>
        /// Gets the database adapter of address map database.
        /// </summary>
        /// <returns>The adapter of address map database.</returns>
        private static BoxDBAdapter GetAddressMapDBAdapter()
        {
            string path = GetDBFilesPath();
            BoxDBAdapter adapter = new BoxDBAdapter(path);
            adapter.EnsureTable<DataTableAddressMap>(typeof(DataTableAddressMap).Name, DataTableAddressMap.PrimaryKey);
            adapter.Open();
            return adapter;
        }

        /// <summary>
        /// Deletes the database configuration files.
        /// </summary>
        private static void DeleteDBConfigFiles()
        {
            string path = GetDBFilesPath();
            string[] filePaths = Directory.GetFiles(path);

            if (filePaths != null && filePaths.Length > 0)
            {
                for (int i = 0, length = filePaths.Length; i < length; ++i)
                {
                    string filePath = filePaths[i];
                    FileInfo fileInfo = new FileInfo(filePath);

                    if (fileInfo.Extension == DbConfigFileExtension)
                    {
                        try
                        {
                            fileInfo.Delete();
                            Utilities.EditorUtility.DeleteMetaFile(filePath);
                        }
                        catch (Exception exception)
                        {
                            DebugLogger.LogException(exception);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// If necessary, Renames the database files.
        /// </summary>
        private static void RenameDBFiles()
        {
            DataTablePreferences preferencesData = DataTablePreferencesWindow.LoadPreferencesData();

            if (preferencesData && preferencesData.dataTablesStorageLocation == DataTableStorageLocation.ResourcesPath)
            {
                string path = dataTablesLocationMap[preferencesData.dataTablesStorageLocation];
                string[] filePaths = Directory.GetFiles(path);

                for (int i = 0, length = filePaths.Length; i < length; ++i)
                {
                    string filePath = filePaths[i];
                    FileInfo fileInfo = new FileInfo(filePath);

                    if (fileInfo.Extension == BoxDbFileExtension)
                    {
                        string newFileName = fileInfo.GetFileNameWithoutExtension() + QuickUnityEditorApplication.BytesAssetFileExtension;
                        fileInfo.Rename(newFileName);
                    }
                }
            }
        }
    }
}