using System;
using System.Linq;

namespace I3jiad.Common.Core.Extensions
{
    /// <summary>
    /// Расширения для обработки bool.
    /// </summary>
    public static class BoolExtensions
    {
        /// <summary>
        /// Cтроки, трактуемые как false.
        /// </summary>
        private readonly static string[] _falseStrings
            = new string[] { "0", "false", "nan", "no", "none", "undefine", "undefined", "zero", "ложь", "нет", "ноль" };


        /// <summary>
        /// Приведение строки в bool.
        /// </summary>
        /// <param name="str">Входная строка.</param>
        /// <returns>Конвертированное значение.</returns>
        public static bool ToBool(this string str)
            => !(str.IsEmpty()
                || _falseStrings.Any(s => s.Equals(str.Trim(), StringComparison.OrdinalIgnoreCase)));


        /// <summary>
        /// Приведение целочисленных значений к bool
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToBool<T>(this T value)
            where T : struct, IComparable, IConvertible, IFormattable
        {
            switch (value)
            {
                case char tChar:
                    return !(tChar.Equals('0')
                            || char.IsSeparator(tChar)
                            || char.IsWhiteSpace(tChar)
                            || char.IsControl(tChar));
                case Enum tEnum:
                    return !tEnum.ToString().Equals("none", StringComparison.OrdinalIgnoreCase)
                        && !tEnum.ToString().Equals("undefined", StringComparison.OrdinalIgnoreCase);
                default:
                    return !value.Equals(default(T));
            };
        }


        /// <summary>
        /// Приведение nullable целочисленных значений к bool
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToBool<T>(this T? value)
            where T : struct, IComparable, IConvertible, IFormattable
            => value.HasValue
                && value.Value.ToBool();
    }
}
