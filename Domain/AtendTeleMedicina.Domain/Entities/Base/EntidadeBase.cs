using System;
using System.Text.RegularExpressions;

namespace AtendTeleMedicina.Domain.Entities.Base
{
  public class EntidadeBase
  {
        public string Id { get; set; }

        private string Filters { get; set; }

        public string ValidateText(string text, int maxLength)
        {

            if (string.IsNullOrEmpty(text))
                return string.Empty;
            text = text.Trim();
            if (text.Length > maxLength)
                text = text.Substring(0, maxLength);
            text = Regex.Replace(text, "[\\s]{2,}", " ");   //two or more spaces
            text = Regex.Replace(text, "(<[b|B][r|R]/*>)+|(<[p|P](.|\\n)*?>)", "\n");   //<br>
            text = Regex.Replace(text, "(\\s*&[n|N][b|B][s|S][p|P];\\s*)+", " ");   //&nbsp;
            text = Regex.Replace(text, "<(.|\\n)*?>", string.Empty);    //any other tags
            text = text.Replace("'", "''");
            return text;
        }

    }
}
