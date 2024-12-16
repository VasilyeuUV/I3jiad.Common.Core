using System.Linq;

namespace I3jiad.Common.Core.Extensions
{
    /// <summary>
    /// String extensions (Расширения для строк).
    /// </summary>
    public static class StringExtensions
    {
        //#######################################################################################################
        #region ВЫБОР ИЗ СТРОКИ НЕ ЦИФР

        /// <summary>
        /// Get all characters from a string except numbers (Получить из строки все символы, кроме цифр).
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveDigits(this string str)
            => string.IsNullOrEmpty(str)
                ? str
                : string.Join("", str.Where(ch => !char.IsDigit(ch)));

        #endregion // ВЫБОР ИЗ СТРОКИ НЕ ЦИФР
    }
}
