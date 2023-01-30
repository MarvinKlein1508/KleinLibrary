using System.Collections;
using System.Text;
using System.Xml.Linq;
#nullable disable

namespace KleinLibrary.DebugHelper
{
    public static class ExceptionHandler
    {
        public static string FormatException(Exception ex)
        {
            StringBuilder errorBuilder = new StringBuilder();
            if (ex != null)
            {
                XElement root = new XElement(ex.GetType().ToString());
                if (ex.Message != null)
                {
                    root.Add(new XElement("Message", ex.Message));
                }

                if (ex.StackTrace != null)
                {
                    root.Add(new XElement("StackTrace", ex.StackTrace));
                }

                if (ex.Data.Count > 0)
                {
                    root.Add
                    (
                        new XElement("Data",
                            from entry in ex.Data.Cast<DictionaryEntry>()
                            let key = entry.Key.ToString()
                            let value = (entry.Value == null) ?
                                            "null" : entry.Value.ToString()
                            select new XElement(key, value))
                    );
                }

                if (ex.InnerException != null)
                {
                    root.Add
                    (
                        new ExceptionXElement
                            (ex.InnerException, true)
                    );
                }


                errorBuilder.AppendLine(root.ToString());
            }
            return errorBuilder.ToString();
        }

        public static string FormatExceptionHTML(Exception ex)
        {
            string exception = FormatException(ex);
            exception = exception.Replace("<", "&lt;");
            exception = exception.Replace(">", "&gt;");
            exception = exception.Replace("\r\n", "<br />");
            return exception;
        }
    }
}
