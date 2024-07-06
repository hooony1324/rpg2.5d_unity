using Codice.CM.WorkspaceServer.Tree;
using Data;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class DataTransformer : EditorWindow
{
#if UNITY_EDITOR

    [MenuItem("Tools/ParseExcelToJson")]
    public static void ParseExcelDataToJson()
    {
        ParseExcelDataToJson<TextDataLoader, TextData>("Text_Name");
        ParseExcelDataToJson<TextDataLoader, TextData>("Text_Description");
        ParseExcelDataToJson<TextDataLoader, TextData>("Text_Message");

        ParseExcelDataToJson<MonsterDataLoader, MonsterData>("Monster");
        ParseExcelDataToJson<HeroDataLoader, HeroData>("Hero");
        ParseExcelDataToJson<SkillDataLoader, SkillData>("Skill");
        ParseExcelDataToJson<EffectDataLoader, EffectData>("Effect");

        ParseExcelDataToJson<DropTableDataLoader, DropTableData>("DropTable");
        ParseExcelDataToJson<ItemDataLoader<EquipmentData>, EquipmentData>("Item_Equipment");
        ParseExcelDataToJson<ItemDataLoader<ConsumableData>, ConsumableData>("Item_Consumable");
        ParseExcelDataToJson<ItemDataLoader<CurrencyData>, CurrencyData>("Item_Currency");

        ParseExcelDataToJson<EquipmentOptionDataLoader, EquipmentOptionData>("EquipmentOption");
        ParseExcelDataToJson<HeroLevelDataLoader, HeroLevelData>("HeroLevel");
        ParseExcelDataToJson<PlayerLevelDataLoader, PlayerLevelData>("PlayerLevel");
    }

    private static void ParseExcelDataToJson<Loader, LoaderData>(string filename) where Loader : new() where LoaderData : new()
    {
        Loader loader = new Loader();
        FieldInfo field = loader.GetType().GetFields()[0];
        field.SetValue(loader, ParseExcelDataToList<LoaderData>(filename));

        string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
        File.WriteAllText($"{Application.dataPath}/@Resources/Data/JsonData/{filename}Data.json", jsonStr);
        AssetDatabase.Refresh();
    }

    private static List<LoaderData> ParseExcelDataToList<LoaderData>(string filename)where LoaderData : new()
    {
        List<LoaderData> loaderDatas = new List<LoaderData>();

        string[] lines = File.ReadAllText($"{Application.dataPath}/@Resources/Data/ExcelData/{filename}Data.csv").Trim().Split("\n");

        List<string[]> rows = new List<string[]>();
        int innerFieldCount = 0;
        for (int l = 1; l < lines.Length; l++)
        {
            string[] row = lines[l].Replace("\r", "").Split(',');
            rows.Add(row);
        }

        for (int r = 0; r < rows.Count; r++)
        {
            if (rows[r].Length == 0)
                continue;
            if (string.IsNullOrEmpty(rows[r][0]))
                continue;
            innerFieldCount = 0;
            
            LoaderData loaderData = new LoaderData();
            Type loaderDataType = typeof(LoaderData);
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var fields = GetFieldsInBase(loaderDataType, bindingFlags);

            int nextIndex;
            for (nextIndex = r + 1; nextIndex < rows.Count; nextIndex++)
            {
                if (string.IsNullOrEmpty(rows[nextIndex][0]) == false)
                    break;
            }


            for (int f = 0; f < fields.Count; f++)
            {
                FieldInfo field = loaderData.GetType().GetField(fields[f].Name);
                Type type = field.FieldType;

                if (type.IsGenericType)
                {
                    Type valueType = type.GetGenericArguments()[0];
                    Type genericListType = typeof(List<>).MakeGenericType(valueType);
                    var genericList = Activator.CreateInstance(genericListType) as IList;

                    for (int i = r; i < nextIndex; i++)
                    {
                        if (string.IsNullOrEmpty(rows[i][f + innerFieldCount]))
                            continue;
                        Debug.Log($"filename = {filename} ,  {field} -> {rows[i][f]}");
                        {
                            bool isCustomClass = valueType.IsClass && !valueType.IsPrimitive && valueType != typeof(string);

                            if (isCustomClass)
                            {
                                object fieldInstance = Activator.CreateInstance(valueType);

                                Type fieldType = fieldInstance.GetType();
                                FieldInfo[] fieldInfos = fieldType.GetFields(BindingFlags.Public | BindingFlags.Instance);

                                for (int k = 0; k < fieldInfos.Length; k++)
                                {
                                    FieldInfo innerField = valueType.GetFields()[k];
                                    string str = rows[i][f + innerFieldCount + k];
                                    object convertedValue = ConvertValue(str, innerField.FieldType);
                                    if (convertedValue != null)
                                    {
                                        innerField.SetValue(fieldInstance, convertedValue);
                                    }
                                }

                                string nextStr = null;
                                if (i + 1 < rows.Count)
                                {
                                    if (f + innerFieldCount < rows[i + 1].Length)
                                    {
                                        //DataId가 null이면 리스트가 아직 끝난게 아님
                                        if (string.IsNullOrEmpty(rows[i + 1][0]))
                                            nextStr = rows[i + 1][f + innerFieldCount];
                                    }
                                }
                                if (string.IsNullOrEmpty(nextStr))
                                {
                                    innerFieldCount = fieldInfos.Length - 1;
                                }
                                else if (i + 1 == nextIndex)
                                    innerFieldCount = fieldInfos.Length - 1;

                                genericList.Add(fieldInstance);

                                // field.SetValue(loaderData, fieldInstance);
                            }
                            else
                            {
                                object value = ConvertValue(rows[i][f], valueType);
                                genericList.Add(value);
                            }
                        }
                    }

                    if (genericList != null)
                    {
                        field.SetValue(loaderData, genericList);
                    }
                }
                else
                {
                    Debug.Log($"filename = {filename} ,  {field} -> {rows[r][f]}");
                    if (rows[r][f].Contains("780"))
                    {
                        Debug.Log($"filename = {filename} ,  {field} -> {rows[r][f]}");
                    }

                    bool isCustomClass = field.FieldType.IsClass && !field.FieldType.IsPrimitive && field.FieldType != typeof(string);
                    if (isCustomClass)
                    {
                        object fieldInstance = Activator.CreateInstance(field.FieldType);

                        Type fieldType = fieldInstance.GetType();
                        FieldInfo[] fieldInfos = fieldType.GetFields(BindingFlags.Public | BindingFlags.Instance);

                        for (int i = 0; i < fieldInfos.Length; i++)
                        {
                            FieldInfo innerField = field.FieldType.GetFields()[i];
                            string value = rows[r][f + innerFieldCount + i];
                            object convertedValue = ConvertValue(value, innerField.FieldType);
                            if (convertedValue != null)
                            {
                                innerField.SetValue(fieldInstance, convertedValue);
                            }

                        }
                        innerFieldCount = fieldInfos.Length - 1;
                        field.SetValue(loaderData, fieldInstance);
                    }
                    else
                    {
                        //기타필드 처리
                        object value = ConvertValue(rows[r][f], field.FieldType);
                        if (value != null)
                        {
                            field.SetValue(loaderData, value);
                        }
                    }
                }
            }
            loaderDatas.Add(loaderData);
        }

        return loaderDatas;
    }
    private static object ConvertValue(string value, Type type)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        TypeConverter converter = TypeDescriptor.GetConverter(type);
        return converter.ConvertFromString(value);
    }

    public static List<FieldInfo> GetFieldsInBase(Type type, BindingFlags bindingFlags)
    {//Data.Contents.cs에 정의된 클래스로 멤버Serialize하여 파싱 준비
        List<FieldInfo> fields = new List<FieldInfo>();
        HashSet<string> fieldNames = new HashSet<string>(); //중복방지
        Stack<Type> stack = new Stack<Type>();              //Fields순서 : 자식->부모 >> 부모->자식 

        while (type != null && type != typeof(object))
        {
            stack.Push(type);
            type = type.BaseType;
        }

        while (stack.Count > 0)
        {
            Type currentType = stack.Pop();
            foreach (var field in currentType.GetFields(bindingFlags))
            {
                if (fieldNames.Add(field.Name))
                {
                    fields.Add(field);
                }
            }
        }

        return fields;
    }
#endif
}
