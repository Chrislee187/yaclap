using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace YACLAP.Extensions
{
    public static class StringConversionExtensions
    {
        public static int ToInt(this string value, int dflt = 0)
        {
            int result;
            return int.TryParse(value, out result) ? result : dflt;
        }

        public static float ToFloat(this string value, float dflt = 0f)
        {
            float result;
            return float.TryParse(value, out result) ? result : dflt;
        }

        public static double ToDouble(this string value, double dflt = 0d)
        {
            double result;
            return double.TryParse(value, out result) ? result : dflt;
        }

        public static decimal ToDecimal(this string value, decimal dflt = 0m)
        {
            decimal result;
            return decimal.TryParse(value, out result) ? result : dflt;
        }


        public static bool ToBool(this string value, bool dflt = false)
        {
            if (value.Length == 0) return false;

            var c1 = value.ToLower();
            return new[] { "yes", "true", "1" }.Any(c => c == c1);
        }
        public static DateTime? ToDateTime(this string value, DateTime? dflt = null)
        {
            DateTime result;

            return DateTime.TryParse(value, out result)
                ? result
                : dflt ?? DateTime.MinValue;
        }

        public static Guid ToGuid(this string value, Guid? dflt = null)
        {
            Guid result;

            return Guid.TryParse(value, out result)
                ? result
                : dflt ?? Guid.Empty;
        }

        public static MemoryStream ToMemoryStream(this string value)
        {
            byte[] testBytes = value.ToBytes();

            MemoryStream memStream = new MemoryStream(testBytes.Length);
            memStream.Write(testBytes, 0, testBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            return memStream;
        }

        public static byte[] ToBytes(this string value)
        {
            byte[] bytes = new byte[value.Length * sizeof(char)];
            Buffer.BlockCopy(value.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Split strings using vertical bars, used to produce simulated args[] arrays received in main()
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string[] ToArgsArray(this string value) 
            => value.Split('|');

        /// <summary>
        /// Return default value is index out of bounds
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T DefaultingIndex<T>(this T[] value, int index, T defaultValue = default(T)) =>
            index + 1 > value.Length 
                ? defaultValue 
                : value[index];

        /// <summary>
        /// Searchs for the default static TryParse(string, out T) that exists on many .NET types to
        /// dynamically convert a string to any type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ToTypeInstance(this string value, Type type)
        {
            if (type == typeof(string)) return value;

            var tryParseParamsFilter = new[]{ typeof(string), type.MakeByRefType() };
            var tryParseMethod = type.GetMethod("TryParse", tryParseParamsFilter);

            if (tryParseMethod == null)
            {
                throw new SystemException($"No static TryParse(string, out {type.Name}) found on {type.Name}");
            }
            var parameters = new object[] {value, null};
            var result = (bool) tryParseMethod.Invoke(null, parameters);

            if (result)
            {
                return parameters[1];
            }

            throw new SystemException($"Attempt to parse '{value}' using {type}.TryParse() failed");
        }

    }
}