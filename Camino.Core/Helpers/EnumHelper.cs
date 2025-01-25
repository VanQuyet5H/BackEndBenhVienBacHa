using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Camino.Core.Helpers
{
    public static class EnumHelper
    {
        public static string GetNameByValue<TEnum>(this int value) where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be an enum.");
            }
            return Enum.GetName(typeof(TEnum), value);
        }

        public static string GetDescription(this Enum value)
        {
            if (value == null) return null;

            var field = value.GetType().GetField(value.ToString());
            if (field == null)
                return "";
            var attribute
                = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                    as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static List<T> GetListEnum<T>()
        {
            var result = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            return result;
        }

        public class EnumModel
        {
            public int Value { get; set; }
            public string Name { get; set; }
        }

        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException("TEnum must be an enum.");
            FieldInfo[] fields = type.GetFields();
            var field = fields
                            .SelectMany(f => f.GetCustomAttributes(
                                typeof(DescriptionAttribute), false), (
                                    f, a) => new { Field = f, Att = a })
                            .Where(a => ((DescriptionAttribute)a.Att)
                                .Description == description).SingleOrDefault();
            return field == null ? default(T) : (T)field.Field.GetRawConstantValue();
        }
    }
}
