#pragma warning disable 0219

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


public class ExcelImporterMaker : EditorWindow
{
    private readonly static string PATH_PLUGIN = "Assets/01 Plugin/ReadExcel/Terasurware";
    private Vector2 curretScroll = Vector2.zero;

    void OnGUI()
    {
        GUILayout.Label("makeing importer", EditorStyles.boldLabel);
        className = EditorGUILayout.TextField("class name", className);
        //sepalateSheet = EditorGUILayout.Toggle("sepalate sheet", sepalateSheet);

        //EditorPrefs.SetBool(s_key_prefix + fileName + ".separateSheet", sepalateSheet);

        if (GUILayout.Button("create"))
        {
            EditorPrefs.SetString(s_key_prefix + fileName + ".className", className);
            ExportEntity();
            ExportImporter();

            AssetDatabase.ImportAsset(filePath);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            Close();
        }

        // selecting sheets

        EditorGUILayout.LabelField("sheet settings");
        EditorGUILayout.BeginVertical("box");
        foreach (ExcelSheetParameter sheet in sheetList)
        {
            GUILayout.BeginHorizontal();
            sheet.isEnable = EditorGUILayout.BeginToggleGroup("enable", sheet.isEnable);
            EditorGUILayout.LabelField(sheet.sheetName);
            EditorGUILayout.EndToggleGroup();
            EditorPrefs.SetBool(s_key_prefix + fileName + ".sheet." + sheet.sheetName, sheet.isEnable);
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        // selecting parameters
        EditorGUILayout.LabelField("parameter settings");
        curretScroll = EditorGUILayout.BeginScrollView(curretScroll);
        EditorGUILayout.BeginVertical("box");
        string lastCellName = string.Empty;
        foreach (ExcelRowParameter cell in typeList)
        {
            if (cell.isArray && lastCellName != null && cell.name.Equals(lastCellName))
            {
                continue;
            }

            cell.isEnable = EditorGUILayout.BeginToggleGroup("enable", cell.isEnable);
            if (cell.isArray)
            {
                EditorGUILayout.LabelField("---[array]---");
            }
            GUILayout.BeginHorizontal();
            cell.name = EditorGUILayout.TextField(cell.name);
            cell.type = (ValueType)EditorGUILayout.EnumPopup(cell.type, GUILayout.MaxWidth(100));
            EditorPrefs.SetInt(s_key_prefix + fileName + ".type." + cell.name, (int)cell.type);
            GUILayout.EndHorizontal();

            EditorGUILayout.EndToggleGroup();
            lastCellName = cell.name;
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();

    }

    private enum ValueType
    {
        BOOL = 0,
        STRING = 1,
        INT = 2,
        FLOAT = 3,
        DOUBLE = 4,
        ARAY_BOOL = 5,
        ARAY_STRING = 6,
        ARAY_INT = 7,
        ARAY_FLOAT = 8,
        ARAY_DOUBLE = 9,
        LIST_BOOL = 10,
        LIST_STRING = 11,
        LIST_INT = 12,
        LIST_FLOAT = 13,
        LIST_DOUBLE = 14,
    }

    private string filePath = string.Empty;
    private bool sepalateSheet = false;
    private List<ExcelRowParameter> typeList = new List<ExcelRowParameter>();
    private List<ExcelSheetParameter> sheetList = new List<ExcelSheetParameter>();
    private string className = string.Empty;
    private string fileName = string.Empty;
    private static string s_key_prefix = "terasurware.exel-importer-maker.";

    //  [MenuItem("Assets/XLS Import Settings...")]
    static void ExportExcelToAssetbundle()
    {
        foreach (Object obj in Selection.objects)
        {


            var window = ScriptableObject.CreateInstance<ExcelImporterMaker>();
            window.filePath = AssetDatabase.GetAssetPath(obj);
            window.fileName = Path.GetFileNameWithoutExtension(window.filePath);


            using (FileStream stream = File.Open(window.filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IWorkbook book = null;
                if (Path.GetExtension(window.filePath) == ".xlsx")
                {
                    book = new HSSFWorkbook(stream);
                }
                else
                {
                    book = new XSSFWorkbook(stream);
                }

                for (int i = 0; i < book.NumberOfSheets; ++i)
                {
                    ISheet s = book.GetSheetAt(i);
                    ExcelSheetParameter sht = new ExcelSheetParameter();
                    sht.sheetName = s.SheetName;
                    // sht.sheetName = sht.sheetName.Replace(" ", "_");
                    //sht.sheetName = Regex.Replace(sht.sheetName, @"[^0-9a-zA-Z]+", "_");
                    sht.isEnable = EditorPrefs.GetBool(s_key_prefix + window.fileName + ".sheet." + sht.sheetName, true);
                    window.sheetList.Add(sht);

                    Debug.Log(sht.sheetName);
                }

                ISheet sheet = book.GetSheetAt(0);

                window.className = EditorPrefs.GetString(s_key_prefix + window.fileName + ".className", "Entity_" + sheet.SheetName);

                //window.sepalateSheet = EditorPrefs.GetBool(s_key_prefix + window.fileName + ".separateSheet");
                window.sepalateSheet = true;
                IRow titleRow = sheet.GetRow(0);
                IRow dataRow = sheet.GetRow(1);
                for (int i = 0; i < titleRow.LastCellNum; i++)
                {
                    ExcelRowParameter lastParser = null;
                    ExcelRowParameter parser = new ExcelRowParameter();
                    Debug.Log("wwhat " + titleRow.GetCell(i));
                    if (titleRow.GetCell(i) == null)
                        continue;

                    parser.name = titleRow.GetCell(i).StringCellValue;
                    parser.name = parser.name.Replace(" ", "");
                    parser.name = Regex.Replace(parser.name, @"[^0-9a-zA-Z]+", "");
                    parser.rowSort = i;
                    // Debug.Log(parser.name);
                    parser.isArray = parser.name.Contains("[]");
                    if (parser.isArray)
                    {
                        parser.name = parser.name.Remove(parser.name.LastIndexOf("[]"));
                    }

                    ICell cell = dataRow.GetCell(i);

                    // array support
                    if (window.typeList.Count > 0)
                    {
                        lastParser = window.typeList[window.typeList.Count - 1];
                        if (lastParser.isArray && parser.isArray && lastParser.name.Equals(parser.name))
                        {
                            // trailing array items must be the same as the top type
                            parser.isEnable = lastParser.isEnable;
                            parser.type = lastParser.type;
                            lastParser.nextArrayItem = parser;
                            window.typeList.Add(parser);
                            continue;
                        }
                    }

                    if (cell.CellType != CellType.Unknown && cell.CellType != CellType.Blank)
                    {
                        parser.isEnable = true;

                        try
                        {
                            if (EditorPrefs.HasKey(s_key_prefix + window.fileName + ".type." + parser.name))
                            {
                                parser.type = (ValueType)EditorPrefs.GetInt(s_key_prefix + window.fileName + ".type." + parser.name);
                            }
                            else
                            {
                                string sampling = cell.StringCellValue;
                                parser.type = ValueType.STRING;
                            }
                        }
                        catch
                        {
                        }
                        try
                        {
                            if (EditorPrefs.HasKey(s_key_prefix + window.fileName + ".type." + parser.name))
                            {
                                parser.type = (ValueType)EditorPrefs.GetInt(s_key_prefix + window.fileName + ".type." + parser.name);
                            }
                            else
                            {
                                double sampling = cell.NumericCellValue;
                                parser.type = ValueType.DOUBLE;
                            }
                        }
                        catch
                        {
                        }
                        try
                        {
                            if (EditorPrefs.HasKey(s_key_prefix + window.fileName + ".type." + parser.name))
                            {
                                parser.type = (ValueType)EditorPrefs.GetInt(s_key_prefix + window.fileName + ".type." + parser.name);
                            }
                            else
                            {
                                bool sampling = cell.BooleanCellValue;
                                parser.type = ValueType.BOOL;
                            }
                        }
                        catch
                        {
                        }
                    }

                    window.typeList.Add(parser);
                }

                window.Show();
            }
        }
    }

    public static bool canImport;
    [MenuItem("Assets/Reimport Stats")]
    static void ReimportStats()
    {
        string pathRsc = "Assets/00 GragonSky/02 Data/DragonAge Data.xlsx";
        canImport = true;
        AssetDatabase.ImportAsset(pathRsc, ImportAssetOptions.ImportRecursive);
        canImport = false;
    }

    //Other import:
    [MenuItem("Assets/XLS Import Settings...")]
    static void ExportAllExcelToAssetbundle()
    {
        foreach (Object obj in Selection.objects)
        {


            var window = ScriptableObject.CreateInstance<ExcelImporterMaker>();
            window.filePath = AssetDatabase.GetAssetPath(obj);
            window.fileName = Path.GetFileNameWithoutExtension(window.filePath);


            using (FileStream stream = File.Open(window.filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IWorkbook book = null;
                if (Path.GetExtension(window.filePath) == ".xls")
                {
                    book = new HSSFWorkbook(stream);
                }
                else
                {
                    book = new XSSFWorkbook(stream);
                }

                for (int i = 0; i < book.NumberOfSheets; ++i)
                {
                    ISheet s = book.GetSheetAt(i);
                    //ExcelSheetParameter sht = new ExcelSheetParameter();
                    //sht.sheetName = s.SheetName;
                    //// sht.sheetName = sht.sheetName.Replace(" ", "_");
                    ////sht.sheetName = Regex.Replace(sht.sheetName, @"[^0-9a-zA-Z]+", "_");
                    //sht.isEnable = EditorPrefs.GetBool(s_key_prefix + window.fileName + ".sheet." + sht.sheetName, true);
                    //window.sheetList.Add(sht);
                    ExportSheet(s);
                    //     Debug.Log(sht.sheetName);
                }


            }
        }
    }


    public static void ExportSheet(ISheet sheet)
    {
        foreach (Object obj in Selection.objects)
        {


            var window = ScriptableObject.CreateInstance<ExcelImporterMaker>();
            window.filePath = AssetDatabase.GetAssetPath(obj);

            window.fileName = Path.GetFileNameWithoutExtension(window.filePath);


            //  using (FileStream stream = File.Open(window.filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                //IWorkbook book = null;
                //if (Path.GetExtension(window.filePath) == ".xls")
                //{
                //    book = new HSSFWorkbook(stream);
                //}
                //else
                //{
                //    book = new XSSFWorkbook(stream);
                //}

                // for (int i = 0; i < book.NumberOfSheets; ++i)
                {
                    ISheet s = sheet;
                    ExcelSheetParameter sht = new ExcelSheetParameter();
                    sht.sheetName = s.SheetName;
                    // sht.sheetName = sht.sheetName.Replace(" ", "_");
                    //sht.sheetName = Regex.Replace(sht.sheetName, @"[^0-9a-zA-Z]+", "_");
                    //  sht.isEnable = EditorPrefs.GetBool(s_key_prefix + window.fileName + ".sheet." + sht.sheetName, true);
                    sht.isEnable = true;
                    window.sheetList.Add(sht);

                    Debug.Log(sht.sheetName);
                }

                // ISheet sheet = book.GetSheetAt(0);

                // window.className = EditorPrefs.GetString(s_key_prefix + window.fileName + ".className", "Entity_" + sheet.SheetName);
                string _className = sheet.SheetName.Replace(" ", "");
                _className = Regex.Replace(_className, @"[^0-9a-zA-Z]+", "");
                //  window.filePath += _className;
                //  Debug.Log("yolo=" + window.filePath);
                window.className = _className;
                window.sepalateSheet = EditorPrefs.GetBool(s_key_prefix + window.fileName + ".separateSheet");

                IRow titleRow = sheet.GetRow(0);
                IRow dataRow = sheet.GetRow(1);
                for (int i = 0; i < titleRow.LastCellNum; i++)
                {
                    ExcelRowParameter lastParser = null;
                    ExcelRowParameter parser = new ExcelRowParameter();
                    // Debug.Log("wwhat " + titleRow.GetCell(i));
                    if (titleRow.GetCell(i) == null)
                        continue;

                    parser.name = titleRow.GetCell(i).StringCellValue;

                    if (!parser.name.StartsWith("(") && !(parser.name.Contains("(") && parser.name.Contains(")")))
                    {
                        continue;
                    }

                    parser.name = parser.name.Trim('(').Trim(')');
                    parser.name = parser.name.Trim();
                    parser.name = parser.name.Replace(" ", "_");
                    Debug.Log(parser.name);
                    parser.name = Regex.Replace(parser.name, @"[^0-9a-zA-Z]+", "_");
                    parser.rowSort = i;
                    // Debug.Log(parser.name);
                    parser.isArray = parser.name.Contains("[]");
                    if (parser.isArray)
                    {
                        parser.name = parser.name.Remove(parser.name.LastIndexOf("[]"));
                    }

                    ICell cell = dataRow.GetCell(i);

                    // array support
                    if (window.typeList.Count > 0)
                    {
                        lastParser = window.typeList[window.typeList.Count - 1];
                        if (lastParser.isArray && parser.isArray && lastParser.name.Equals(parser.name))
                        {
                            // trailing array items must be the same as the top type
                            parser.isEnable = lastParser.isEnable;
                            parser.type = lastParser.type;
                            lastParser.nextArrayItem = parser;
                            window.typeList.Add(parser);
                            continue;
                        }
                    }

                    if (cell != null && cell.CellType != CellType.Unknown && cell.CellType != CellType.Blank)
                    {
                        parser.isEnable = true;

                        try
                        {
                            if (EditorPrefs.HasKey(s_key_prefix + window.fileName + ".type." + parser.name))
                            {
                                parser.type = (ValueType)EditorPrefs.GetInt(s_key_prefix + window.fileName + ".type." + parser.name);
                            }
                            else
                            {
                                string sampling = cell.StringCellValue;
                                parser.type = ValueType.STRING;
                            }
                        }
                        catch
                        {
                        }
                        try
                        {
                            if (EditorPrefs.HasKey(s_key_prefix + window.fileName + ".type." + parser.name))
                            {
                                parser.type = (ValueType)EditorPrefs.GetInt(s_key_prefix + window.fileName + ".type." + parser.name);
                            }
                            else
                            {
                                double sampling = cell.NumericCellValue;
                                parser.type = ValueType.DOUBLE;
                            }
                        }
                        catch
                        {
                        }
                        try
                        {
                            if (EditorPrefs.HasKey(s_key_prefix + window.fileName + ".type." + parser.name))
                            {
                                parser.type = (ValueType)EditorPrefs.GetInt(s_key_prefix + window.fileName + ".type." + parser.name);
                            }
                            else
                            {
                                bool sampling = cell.BooleanCellValue;
                                parser.type = ValueType.BOOL;
                            }
                        }
                        catch
                        {
                        }
                    }

                    window.typeList.Add(parser);
                }

                window.Show();
            }
        }

    }

    //void OnTest()
    //{
    //    IWorkbook
    //}
    void ExportEntity()
    {
        //string templateFilePath = (sepalateSheet) ? "Assets/Terasurware/Editor/EntityTemplate2.txt" : "Assets/Terasurware/Editor/EntityTemplate.txt";
        string templateFilePath = PATH_PLUGIN + "/Editor/EntityTemplate2.txt";
        string entittyTemplate = File.ReadAllText(templateFilePath);
        StringBuilder builder = new StringBuilder();
        bool isInbetweenArray = false;
        foreach (ExcelRowParameter row in typeList)
        {
            if (row.isEnable)
            {
                if (!row.isArray)
                {
                    builder.AppendLine();
                    string str = "";
                    if ((int)row.type < 5)
                        str = row.type.ToString().ToLower();
                    else if((int)row.type > 9)
                        str = "List<" + (row.type - 10).ToString().ToLower() + ">";
                    else
                        str = (row.type - 5).ToString().ToLower() + "[]";
                    builder.AppendFormat("		public {0} {1};", str, row.name);
                }
                else
                {
                    if (!isInbetweenArray)
                    {
                        builder.AppendLine();
                        builder.AppendFormat("		public {0}[] {1};", row.type.ToString().ToLower(), row.name);
                    }
                    isInbetweenArray = (row.nextArrayItem != null);
                }
            }
        }

        entittyTemplate = entittyTemplate.Replace("$Types$", builder.ToString());
        entittyTemplate = entittyTemplate.Replace("$ExcelData$", className);

        Directory.CreateDirectory(PATH_PLUGIN + "/Classes/");
        File.WriteAllText(PATH_PLUGIN + "/Classes/" + className + ".cs", entittyTemplate);
    }

    void ExportImporter()
    {
        // string templateFilePath = (sepalateSheet) ? "Assets/Terasurware/Editor/ExportTemplate2.txt" : "Assets/Terasurware/Editor/ExportTemplate.txt";
        string templateFilePath = PATH_PLUGIN + "/Editor/ExportTemplate2.txt";
        string importerTemplate = File.ReadAllText(templateFilePath);

        StringBuilder builder = new StringBuilder();
        StringBuilder sheetListbuilder = new StringBuilder();
        int rowCount = 0;
        string tab = "					";
        bool isInbetweenArray = false;

        //public string[] sheetNames = {"hoge", "fuga"};
        //$SheetList$
        foreach (ExcelSheetParameter sht in sheetList)
        {
            if (sht.isEnable)
            {
                sheetListbuilder.Append("\"" + sht.sheetName + "\",");
            }
            /*
            if (sht != sheetList [sheetList.Count - 1])
            {
                sheetListbuilder.Append(",");
            }
            */
        }

        foreach (ExcelRowParameter row in typeList)
        {
            int rowCount1 = row.rowSort;
            if (row.isEnable)
            {
                if (!row.isArray)
                {
                    builder.AppendLine();
                    switch (row.type)
                    {
                        case ValueType.BOOL:
                            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = cell.TryGetCell<bool>();", row.name, rowCount1);
                            break;
                        case ValueType.DOUBLE:
                            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = cell.TryGetCell<double>();", row.name, rowCount1);
                            break;
                        case ValueType.INT:
                            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = cell.TryGetCell<int>();", row.name, rowCount1);
                            break;
                        case ValueType.FLOAT:
                            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = cell.TryGetCell<float>();", row.name, rowCount1);
                            break;
                        case ValueType.STRING:
                            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = (cell == null ? \"\" : cell.StringCellValue);", row.name, rowCount1);
                            //  builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = cell.TryGetCell<string>();", row.name, rowCount1);
                            break;
                        case ValueType.ARAY_BOOL:
                            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = Array.ConvertAll(cell.StringCellValue.Split(','), s => bool.Parse(s));", row.name, rowCount1);
                            break;
                        case ValueType.ARAY_DOUBLE:
                            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = Array.ConvertAll(cell.StringCellValue.Split(','), s => double.Parse(s));", row.name, rowCount1);
                            break;
                        case ValueType.ARAY_INT:
                            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = Array.ConvertAll(cell.StringCellValue.Split(','), s => int.Parse(s));", row.name, rowCount1);
                            break;
                        case ValueType.ARAY_FLOAT:
                            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = Array.ConvertAll(cell.StringCellValue.Split(','), s => float.Parse(s));", row.name, rowCount1);
                            break;
                        case ValueType.ARAY_STRING:
                            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = cell.StringCellValue.Split(',');", row.name, rowCount1);
                            break;
                        case ValueType.LIST_BOOL:
                            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = Array.ConvertAll(cell.StringCellValue.Split(','), s => bool.Parse(s)).ToList();", row.name, rowCount1);
                            break;
                        case ValueType.LIST_DOUBLE:
                            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = Array.ConvertAll(cell.StringCellValue.Split(','), s => double.Parse(s)).ToList();", row.name, rowCount1);
                            break;
                        case ValueType.LIST_INT:
                            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = Array.ConvertAll(cell.StringCellValue.Split(','), s => int.Parse(s)).ToList();", row.name, rowCount1);
                            break;
                        case ValueType.LIST_FLOAT:
                            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = Array.ConvertAll(cell.StringCellValue.Split(','), s => float.Parse(s)).ToList();", row.name, rowCount1);
                            break;
                        case ValueType.LIST_STRING:
                            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = cell.StringCellValue.Split(',').ToList();", row.name, rowCount1);
                            break;
                    }
                }
                else
                {
                    // only the head of array should generate code

                    if (!isInbetweenArray)
                    {
                        int arrayLength = 0;
                        for (ExcelRowParameter r = row; r != null; r = r.nextArrayItem, ++arrayLength)
                        {
                        }

                        builder.AppendLine();
                        switch (row.type)
                        {
                            case ValueType.BOOL:
                                builder.AppendFormat(tab + "p.{0} = new bool[{1}];", row.name, arrayLength);
                                break;
                            case ValueType.DOUBLE:
                                builder.AppendFormat(tab + "p.{0} = new double[{1}];", row.name, arrayLength);
                                break;
                            case ValueType.INT:
                                builder.AppendFormat(tab + "p.{0} = new int[{1}];", row.name, arrayLength);
                                break;
                            case ValueType.FLOAT:
                                builder.AppendFormat(tab + "p.{0} = new float[{1}];", row.name, arrayLength);
                                break;
                            case ValueType.STRING:
                                builder.AppendFormat(tab + "p.{0} = new string[{1}];", row.name, arrayLength);
                                break;
                        }

                        for (int i = 0; i < arrayLength; ++i)
                        {
                            builder.AppendLine();
                            switch (row.type)
                            {
                                case ValueType.BOOL:
                                    builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0}[{2}] = (cell == null ? false : cell.BooleanCellValue);", row.name, rowCount1 + i, i);
                                    break;
                                case ValueType.DOUBLE:
                                    builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0}[{2}] = (cell == null ? 0.0 : cell.NumericCellValue);", row.name, rowCount1 + i, i);
                                    break;
                                case ValueType.INT:
                                    builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0}[{2}] = (int)(cell == null ? 0 : cell.NumericCellValue);", row.name, rowCount1 + i, i);
                                    break;
                                case ValueType.FLOAT:
                                    builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0}[{2}] = (float)(cell == null ? 0.0 : cell.NumericCellValue);", row.name, rowCount1 + i, i);
                                    break;
                                case ValueType.STRING:
                                    builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0}[{2}] = (cell == null ? \"\" : cell.StringCellValue);", row.name, rowCount1 + i, i);
                                    break;
                            }
                        }
                    }
                    isInbetweenArray = (row.nextArrayItem != null);
                }
            }
            rowCount += 1;
        }


        importerTemplate = importerTemplate.Replace("$IMPORT_PATH$", filePath);
        if (!Directory.Exists(Path.GetDirectoryName(filePath) + "/" + className))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) + "/" + className);
        }
        //if (!File.Exists(Path.GetDirectoryName(filePath) + "/" + className))
        //    AssetDatabase.CreateFolder(Path.GetDirectoryName(filePath), className);
        importerTemplate = importerTemplate.Replace("$ExportAssetDirectry$", Path.GetDirectoryName(filePath).Replace("\\", "/") + "/" + className);
        string _exportPath = filePath.Replace(Path.GetFileName(filePath), className + ".asset");
        //importerTemplate = importerTemplate.Replace("$EXPORT_PATH$", Path.ChangeExtension(filePath, ".asset"));
        importerTemplate = importerTemplate.Replace("$EXPORT_PATH$", _exportPath);
        importerTemplate = importerTemplate.Replace("$ExcelData$", className);
        importerTemplate = importerTemplate.Replace("$SheetList$", sheetListbuilder.ToString());
        importerTemplate = importerTemplate.Replace("$EXPORT_DATA$", builder.ToString());
        importerTemplate = importerTemplate.Replace("$ExportTemplate$", fileName.Trim().Replace(' ', '_') + className + "_importer");

        Directory.CreateDirectory(PATH_PLUGIN + "/Classes/Editor/");
        // File.WriteAllText("Assets/Terasurware/Classes/Editor/" + fileName + "_importer.cs", importerTemplate);
        File.WriteAllText(PATH_PLUGIN + "/Classes/Editor/" + fileName.Trim().Replace(' ', '_') + className + "_importer.cs", importerTemplate);
    }

    private class ExcelSheetParameter
    {
        public string sheetName;
        public bool isEnable;
    }

    private class ExcelRowParameter
    {
        public ValueType type;
        public int rowSort;
        public string name;
        public bool isEnable;
        public bool isArray;
        public ExcelRowParameter nextArrayItem;
    }


}
