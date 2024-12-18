using System;
using System.Collections.Generic;
using System.Linq;

namespace I3jiad.Common.Core.Extensions
{
    /// <summary>
    /// Extensions for enums (Расширения для перечислений).
    /// </summary>
    public static class EnumExtensions
    {
        private const string _undefined = "Undefined";
        private const string _none = "None";


        /// <summary>
        /// Проверка значения Enum на значение по умолчанию.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static bool IsDefault<TEnum>(this TEnum enumValue)
            where TEnum : Enum
            => enumValue != null
                && EqualityComparer<TEnum>.Default.Equals(enumValue, default);


        /// <summary>
        /// Проверить существование перечисления с указанными значениями.
        /// </summary>
        /// <typeparam name="TEnum">Тип перечисления, в котором проверяется наличие значения.</typeparam>
        /// <param name="value">Проеряемое на существовании в перечислении значение.</param>
        /// <param name="isIgnoreCase">При определении существования перечисления игнорировать регистр проверяемого значения.</param>
        /// <returns>True, если значение существует в перечислении.</returns>
        public static bool HasEnumValue<TEnum>(this object value, bool isIgnoreCase = false)
            where TEnum : struct, IComparable, IConvertible
            => value != null
                && (isIgnoreCase && value is string strValue
                        ? Enum.GetNames(typeof(TEnum))
                            .Any(name => name.Equals(strValue.Trim(), StringComparison.OrdinalIgnoreCase))
                        : Enum.IsDefined(typeof(TEnum), value));


        /// <summary>
        /// Convert string to enum (Преобразует текст к перечислению).
        /// </summary>
        /// <typeparam name="TEnum">Enum type (Тип перечисления).</typeparam>
        /// <param name="value">String for convert (Строка для преобразования).</param>
        /// <param name="defaultValue">Default enumeration value in case of conversion error
        /// (Значение перечисления по умолчанию в случае ошибки преобразования).</param>
        /// <param name="isIgnoreCase">Ignore case of value (Игнорировать регистр значения).</param>
        /// <returns>Enum value (Значение перечисления).</returns>
        /// <exception cref="ArgumentException"></exception>
        public static TEnum ToEnum<TEnum>(this object value, TEnum? defaultValue = null, bool isIgnoreCase = false)
            where TEnum : struct, IComparable, IConvertible
            => value != null && value.HasEnumValue<TEnum>(isIgnoreCase) ? (TEnum)Enum.Parse(typeof(TEnum), value.ToString(), isIgnoreCase)
             : Enum.TryParse(value?.ToString(), isIgnoreCase, out TEnum enumValue) && enumValue.HasEnumValue<TEnum>(isIgnoreCase) ? enumValue
             : defaultValue != null ? defaultValue.Value
             : throw new ArgumentException($"Can't convert {nameof(value)} to Enum {typeof(TEnum)}", nameof(value));
    }



    //###########################################################################################################################
    #region РАБОТА С АТТРИБУТАМИ



    #endregion // РАБОТА С АТТРИБУТАМИ

}
