using CommonLibrary.Util.Interface;
using Domain.Models.Export;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommonLibrary.Util
{
    public class FileUtility : IFileUtility
    {
        public ExportFile ExportFile<T>(IEnumerable<T> records, string fileName)
        {
            var ms = new MemoryStream();
            using (var writer = new StreamWriter(ms, Encoding.UTF8))
            {
                // 寫入列標題
                WriteCsvHeader(writer, typeof(T));

                // 每行資料
                foreach (var record in records)
                {
                    WriteCsvRow(writer, record);
                }
            }
            byte[] bytes = ms.ToArray();
            return new ExportFile { File = bytes, FileName = fileName, FileType = "text/csv" };
        }

        private void WriteCsvHeader(TextWriter writer, Type type)
        {
            var properties = type.GetProperties();
            var header = string.Join(",", properties.Select(prop => GetCsvColumnName(prop)));
            writer.WriteLine(header);
        }

        private void WriteCsvRow<T>(TextWriter writer, T record)
        {
            var properties = typeof(T).GetProperties();
            var line = string.Join(",", properties.Select(prop => EscapeCsvValue(prop.GetValue(record)?.ToString())));
            writer.WriteLine(line);
        }

        private string GetCsvColumnName(System.Reflection.PropertyInfo prop)
        {
            // 自定義列名
            // 例如: var attr = prop.GetCustomAttribute<YourCustomAttribute>();
            // 如果没有，只使用屬性名
            var attr = prop.GetCustomAttribute<CsvColumnNameAttribute>();
            if (attr != null)
            {
                return attr.Name; //使用自定義
            }
            return prop.Name; // 如果没有自定義使用屬性名
        }

        private string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
                value = $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CsvColumnNameAttribute : Attribute
    {
        public string Name { get; private set; }

        public CsvColumnNameAttribute(string name)
        {
            Name = name;
        }
    }
}
