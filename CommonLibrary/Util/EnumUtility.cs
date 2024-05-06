using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace CommonLibrary.Util
{
    public static class EnumUtility
    {
        public static string GetDescription(this Enum value)
        {
            string desc = string.Empty;
            var fi = value.GetType().GetField(value.ToString());
            if (fi != null)
            {
                var attrs = fi.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false) as IEnumerable<System.ComponentModel.DescriptionAttribute>;

                if (!IsNullOrEmpty(attrs))
                {
                    desc = attrs.First().Description;
                }
            }
            return desc;
        }

        public static string GetDisplayName(this Enum value)
        {
            string output = null;
            var type = value.GetType();
            var fi = type.GetField(value.ToString());
            if (fi != null)
            {
                var attrs = fi.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];
                if (attrs != null && attrs.Any() && !string.IsNullOrWhiteSpace(attrs[0].Name))
                {
                    output = attrs[0].Name;
                }
            }
            return output;
        }


        public static T GetValueFromDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException("Not found.", nameof(description));
            // Or return default(T);
        }

        public static string GetDisplayFromDescription<T>(string description) where T : Enum
        {
            string output = null;
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                    {
                        var attrs = field.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];
                        if (attrs != null && attrs.Any() && !string.IsNullOrWhiteSpace(attrs[0].Name))
                        {
                            output = attrs[0].Name;
                        }
                    }
                }
            }
            return output;
            // Or return default(T);
        }

        public static string GetDescripFromName<T>(string name) where T : Enum
        {
            string output = null;
            foreach (var field in typeof(T).GetFields())
            {
                if (field.Name.Equals(name))
                {
                    var attrs = field.GetCustomAttributes(typeof(DescriptionAttribute), false) as IEnumerable<DescriptionAttribute>;

                    if (!IsNullOrEmpty(attrs))
                    {
                        output = attrs.First().Description;
                    }
                }
            }
            return output;
        }


        private static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> source)
            => source is null || !source.Any();


    }

}
