using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace I3jiad.Common.Core.Extensions
{
    /// <summary>
    /// String extensions (Расширения для строк).
    /// </summary>
    public static class StringExtensions
    {
        private const string _ruSimilarLetters = "аАВсСеЕНкКМоОрРТхХуУ";       // - русские символы, похожие на английские
        private const string _enSimilarLetters = "aABcCeEHkKMoOpPTxXyY";       // - английские символы, похожие на русские 


        /// <summary>
        /// Проверка строки на null, string.Empty и WhiteSpase
        /// </summary>
        /// <param name="str">Исходная строка.</param>
        /// <returns>True, если строка не содержит значащих и/или видимых символов или равна null</returns>
        public static bool IsEmpty(this string str)
            => string.IsNullOrWhiteSpace(str);


        /// <summary>
        /// Переписать строку строчными буквами с первой Заглавной.
        /// </summary>
        /// <param name="str">Исходная строка</param>
        /// <returns></returns>
        public static string ToLowerWithTitleCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }
            string strLower = str.Trim().ToLower();
            string strLowerWithTitleCase = char.ToUpper(strLower[0]) + strLower.Substring(1);
            return str.ToLower().Replace(strLower, strLowerWithTitleCase);
        }


        //###########################################################################################################################
        #region ВЫБОР ИЗ СТРОКИ НЕ ЦИФР

        /// <summary>
        /// Get all characters from a string except numbers (Получить из строки все символы, кроме цифр).
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveDigits(this string str)
            => str.IsEmpty()
                ? str
                : string.Join(string.Empty, str.Where(ch => !char.IsDigit(ch)));

        #endregion // ВЫБОР ИЗ СТРОКИ НЕ ЦИФР



        //###########################################################################################################################
        #region ВЫБОР ИЗ СТРОКИ ЦИФР

        /// <summary>
        /// Получить из строки все цифры как одно число (например, из строки "а28о-с53р" получить "2853") без учета знака "минус"
        /// </summary>
        /// <param name="str">Исходная строка</param>
        /// <param name="separator">Разделитель между числами.</param>
        /// <returns>Все цифры из строки</returns>
        public static string ToPositiveDigits(this string str, string separator = "")
            => str.IsEmpty()
                ? str
                : string.Join(separator, str.Where(char.IsDigit));


        /// <summary>
        /// Получить из строки все группы подряд идущих цифр (например, из строки "а28о-с53р" получить ["28", "53"])  без учета знака "минус"
        /// </summary>
        /// <param name="str">Исходная строка.</param>
        /// <returns>Список цифровых строк.</returns>
        public static IEnumerable<string> ToPositiveDigitsList(this string str)
            => str.IsEmpty()
                ? Enumerable.Empty<string>()
                : Regex.Matches(str, @"\d+")
                    .Cast<Match>()
                    .Select(match => match.Value);


        #endregion // ВЫБОР ИЗ СТРОКИ ЦИФР



        //###########################################################################################################################
        #region РУССКИЙ VS АНГЛИЙСКИЙ

        /// <summary>
        /// Проверка строки на содержание не латинских символов.
        /// </summary>
        /// <param name="str">Исходная строка.</param>
        /// <returns>True, если в строке есть не латинские символы.</returns>
        public static bool HasNonLatinChars(this string str)
            => !str.IsEmpty()
                && str.Any(ch => ch > 127);


        /// <summary>
        /// Проверка на начало строки с заданного значения без учета русского и английского шрифта.
        /// </summary>
        /// <param name="str">Исходная строка.</param>
        /// <param name="prefix">Префикс, с которого должна начинаться исходная строка.</param>
        /// <param name="ignoreCase">Флаг игнорирования прописных и заглавных букв.</param>
        /// <returns>True, если исходная строка начинается с префикса без учета английских и русских букв.</returns>
        public static bool StartWithRuEn(this string str, string prefix = "", bool ignoreCase = false)
        {
            if (string.IsNullOrEmpty(str)
                || string.IsNullOrEmpty(prefix)
                || str.Length < prefix.Length)
            {
                return false;
            }

            // - Нормализация строк, чтобы обеспечить корректное сравнение с учетом возможных комбинированных символов
            str = str.Normalize(NormalizationForm.FormC);
            prefix = prefix.Normalize(NormalizationForm.FormC);

            return prefix
                .Select((ch, i) => CheckEqual(ignoreCase, ch, str[i])
                                || (_ruSimilarLetters.Contains(ch)
                                    ? CheckEqual(ignoreCase, _enSimilarLetters[_ruSimilarLetters.IndexOf(ch)], str[i])
                                    : _enSimilarLetters.Contains(ch)
                                        && CheckEqual(ignoreCase, _ruSimilarLetters[_enSimilarLetters.IndexOf(ch)], str[i])))
                .All(x => x == true);
        }


        /// <summary>
        /// Заменить русские символы на похожие латинские.
        /// </summary>
        /// <param name="str">Исходная строка.</param>
        /// <returns>Исходная строка с замененным русскими буквами на похожие латинские буквы.</returns>
        public static string ToSimilarLatinLetters(this string str)
            => str.IsEmpty()
                ? str
                : string.Concat(str.Select(ch => _ruSimilarLetters.Contains(ch)
                                                ? _enSimilarLetters[_ruSimilarLetters.IndexOf(ch)]
                                                : ch));


        /// <summary>
        /// Заменить латинские символы на похожие русские.
        /// </summary>
        /// <param name="str">Исходная строка.</param>
        /// <returns>Исходная строка с замененным латинскими буквами на похожие русские буквы.</returns>
        public static string ToSimilarRuLetters(this string str)
            => str.IsEmpty()
                ? str
                : string.Concat(str.Select(ch => _enSimilarLetters.Contains(ch)
                                                ? _ruSimilarLetters[_enSimilarLetters.IndexOf(ch)]
                                                : ch));

        #endregion // РУССКИЙ VS АНГЛИЙСКИЙ


        /// <summary>
        /// Сравнение символов с учетом значения регистра.
        /// </summary>
        /// <param name="ignoreCase">Флаг учёта региста.</param>
        /// <param name="ch1">Первый сравниваемый символ.</param>
        /// <param name="ch2">Второй сравниваемый символ.</param>
        /// <returns></returns>
        private static bool CheckEqual(bool ignoreCase, char ch1, char ch2)
            => ignoreCase
                ? char.ToUpper(ch1) == char.ToUpper(ch2)
                : ch1 == ch2;
    }
}