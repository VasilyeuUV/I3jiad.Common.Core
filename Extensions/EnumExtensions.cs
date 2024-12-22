using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

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


        /// <summary>
        /// Возвращает список значений перечисления.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static IEnumerable<TEnum> GetValues<TEnum>(this TEnum enumValue)
            where TEnum : Enum
            => Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>();




        //###########################################################################################################################
        #region ПЕРЕЧИСЛЕНИЯ С [FLAG]

        /// <summary>
        /// Convert a complex enum (with flag) to a list of simple enums<br />
        /// (Преобразование сложного перечисления (с флагом) к списку простых перечислений).
        /// </summary>
        /// <typeparam name="TEnum">Enum type (Тип перечисления).</typeparam>
        /// <param name="enumValue">Enum value (Значение перечисления).</param>
        /// <returns>Enumerable of enum values (Список значений перечисления).</returns>
        public static IEnumerable<TEnum> ToEnumList<TEnum>(this TEnum enumValue)
             where TEnum : Enum
        {
            List<TEnum> enumList = enumValue.GetValues()
                .Where(en => enumValue.HasFlag(en))
                .ToList();

            return enumList is null ? Enumerable.Empty<TEnum>()
                : enumList.Count > 1 ? enumList
                                        .Where(en => !en.ToString().Equals(_none, StringComparison.OrdinalIgnoreCase)
                                                    && !en.ToString().Equals(_undefined, StringComparison.OrdinalIgnoreCase))
                : enumList;
        }


        /// <summary>
        /// Convert a enum with a flag to a list of enum value names.<br/>
        /// (Преобразование перечисления с флагом к списку названий значений перечислений).
        /// </summary>
        /// <typeparam name="TEnum">Enum type (Тип перечисления).</typeparam>
        /// <param name="enumValue">Enum value (Значение перечисления).</param>
        /// <returns>String list (Коллекцию строк).</returns>
        public static IEnumerable<string> ToEnumStringList<TEnum>(this TEnum enumValue)
            where TEnum : Enum
            => enumValue.ToEnumList()
                .Select(en => en.ToString());

        #endregion // ПЕРЕЧИСЛЕНИЯ С [FLAG]



        //###########################################################################################################################
        #region РАБОТА С АТТРИБУТАМИ

        /// <summary>
        /// Get the value of an enum attribute (Получить значение атрибута перечисления).
        /// </summary>
        /// <typeparam name="TAttribute">Attribute class (Класс атрибута).</typeparam>
        /// <param name="enumValue">Enum value (Значение перечисления).</param>
        /// <returns>Attribute value (Значеие атрибута).</returns>
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
            where TAttribute : Attribute
            => enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<TAttribute>()
            ?? null;


        /// <summary>
        /// Get Display attribute value (Получить значение атрибута Display).
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetDisplayAttr<TEnum>(this TEnum enumValue)
            where TEnum : Enum
            => enumValue.GetAttribute<DisplayAttribute>()?.Name
                ?? string.Empty;


        #endregion // РАБОТА С АТТРИБУТАМИ
    }
}