using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AtendTeleMedicina.Domain.Services.Helpers
{
    /// <summary>
    ///     Classe EnumService.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class EnumService<T>
    {
        /// <summary>
        ///     Obtém uma lista com todos os valores do Enum
        /// </summary>
        /// <returns>IList&lt;T&gt;.</returns>
        public static IList<T> GetValues()
        {
            var enumValues =
                Enum.GetValues(typeof(T)).Cast<T>().ToList();
            return enumValues;
        }

        public static IList<T> ParseList(IEnumerable<string> value)
        {
            var tipos = new List<T>();
            if (value == null) return tipos;
            foreach (var item in value)
            {
                if (string.IsNullOrEmpty(item))
                    continue;
                var values = item.Split(',');
                tipos.AddRange(values.Where(x => !string.IsNullOrWhiteSpace(x.ToString())).Select(Parse));
            }
            return tipos;
        }


        /// <summary>
        ///     Obtém uma lista com todos os valores do Enum específicado
        /// </summary>
        /// <param name="value">A classe Enum que gerará a lista de seus valores.</param>
        /// <returns>IList&lt;T&gt;.</returns>
        public static IList<T> GetValues(Enum value)
        {
            var enumValues = new List<T>();

            foreach (var fi in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumValues.Add((T)Enum.Parse(value.GetType(), fi.Name, false));
            }
            return enumValues;
        }

        /////// <summary>
        /////// Obtém a lista de valores de um Enum ogranizados no padrão "Text" e "Value".
        /////// </summary>
        /////// <returns>IEnumerable.</returns>
        //public static IEnumerable GetAsListEditItems()
        //{
        //    var yy = GetValues().Select(x => new
        //    {
        //        Text = GetDisplayValue(x),
        //        Value = x
        //    }).OrderBy(x => x.Text);
        //    return yy;
        //}

        /// <summary>
        ///     Converte o valor específicado no tipo Enum.
        /// </summary>
        /// <param name="value">O valor.</param>
        /// <returns>T.</returns>
        public static T Parse(string value)
        {
            //if (value != null)
            //    return null;
            var enumValue = (T)Enum.Parse(typeof(T), value, true);
            return enumValue;
        }

        public static List<string> ParseInt(IEnumerable<T> value)
        {
            var tipos = new List<string>();
            foreach (var item in value)
            {
                if (item == null)
                    continue;
                var enumValue = (int)Enum.Parse(typeof(T), item.ToString());
                tipos.Add(enumValue.ToString());
            }

            return tipos;
        }

        /// <summary>
        ///     Obtém uma lista com todos os nomes do Enum.
        /// </summary>
        /// <param name="value">O valor Enum.</param>
        /// <returns>IList&lt;System.String&gt;.</returns>
        public static IList<string> GetNames(Enum value)
        {
            var enumNames =
                value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
            return enumNames;
        }

        /// <summary>
        ///     Obtém o nome do valor do Enum em forma de string.
        /// </summary>
        /// <param name="value">O valor.</param>
        /// <returns>System.String.</returns>
        public static string GetName(object value)
        {
            return Enum.GetName(typeof(T), value);
        }

        /// <summary>
        ///     Obtém uma lista com os valores de exibição do Enum.
        /// </summary>
        /// <param name="value">O valor Enum.</param>
        /// <returns>IList&lt;System.String&gt;.</returns>
        public static IList<string> GetDisplayValues(Enum value)
        {
            var displayValues =
                GetNames(value).Select(obj => GetDisplayValue(Parse(obj))).ToList();
            return displayValues;
        }


        /// <summary>
        ///     Obtém o valor de exibição do Enum a partir de seu valor.
        /// </summary>
        /// <param name="value">O valor Enum.</param>
        /// <returns>System.String.</returns>
        public static string GetDisplayValue(T value)
        {
            if (value == null) return string.Empty;

            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo?.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes == null) return string.Empty;

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Name : value.ToString();
        }

        public static string GetDisplayValue(int value)
        {
            var type = (T)(object)value;
            var fieldInfo = type.GetType().GetField(type.ToString());
            var descriptionAttributes = fieldInfo?.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes == null) return string.Empty;

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Name : type.ToString();
        }

        /// <summary>
        ///     Obtém o valor de exibição do Enum a partir de uma string.
        /// </summary>
        /// <param name="value">A string representando o valor Enum.</param>
        /// <returns>System.String.</returns>
        public static string GetDisplayValue(string value)
        {
            return GetDisplayValue(Parse(value));
        }
    }
}
