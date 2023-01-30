using System.Text.Json;
#nullable disable

namespace KleinLibrary.Serialization
{
    /// <summary>
    /// Ein Wrapper für Serialisierungen in das JSON Format
    /// </summary>
    public class JSONWriter
    {
        /// <summary>
        /// Schreibt eine JSON Datei aus einem Objekt
        /// </summary>
        /// <typeparam name="T">Der Typ des zu serialisierenden Objekt</typeparam>
        /// <param name="element">Das zu serialisierende Objekt</param>
        /// <param name="datei">Die Datei in die das serialisierte Objekt geschrieben werden soll</param>
        /// <returns>Gibt ein Tupel zurück. success gibt den Erfolg der Methode an, message gibt die Nachricht im Fehlerfall an</returns>
        public static (bool success, string message) Write<T>(T element, string datei) where T : new()
        {
            try
            {
                var (success, data, message) = Serialize<T>(element);

                if (success)
                {
                    File.WriteAllText(datei, data);
                    return (true, "Erfolgreich geschrieben");
                }
                else
                {
                    return (false, message);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Liest eine JSON Datei in ein Objekt ein
        /// </summary>
        /// <typeparam name="T">Der Typ des zu lesenden Objekt</typeparam>
        /// <param name="datei">Die Datei in der das serialisierte Objekt ist</param>
        /// <returns>Gibt ein Tupel zurück. success gibt den Erfolg der Methode an, element gibt das deserialisierte Objekt and, message gibt die Nachricht im Fehlerfall an </returns>
        public static (bool success, T element, string message) Read<T>(string datei) where T : new()
        {
            try
            {
                string data = File.ReadAllText(datei);
                if (String.IsNullOrWhiteSpace(data))
                {
                    return (false, default, "Der Inhalt der Datei war leer");
                }

                var (success, element, message) = Deserialize<T>(data);

                if (success)
                {
                    return (true, element, "Erfolgreich gelesen");
                }
                else
                {
                    return (false, default, message);
                }
            }
            catch (Exception ex)
            {
                return (false, default, ex.Message);
            }
        }

        /// <summary>
        /// Serialisiert ein Objekt zu einem JSON string
        /// </summary>
        /// <typeparam name="T">Der Typ des zu serialisierenden Objekt</typeparam>
        /// <param name="element">Das zu serialisierende Objekt</param>
        /// <returns>Gibt ein Tupel zurück. success gibt den Erfolg der Methode and, data gibt das serialisierte Objekt an, message gibt die Nachricht im Fehlerfall an</returns>
        public static (bool success, string data, string message) Serialize<T>(T element) where T : new()
        {
            try
            {
                return (true, JsonSerializer.Serialize<T>(element), "Erfolgreich serialisiert");
            }
            catch (Exception ex)
            {
                return (false, "", ex.Message);
            }
        }

        /// <summary>
        /// Deserialisiert einen JSON string in ein Objekt
        /// </summary>
        /// <typeparam name="T">Der Typ des zu deserialierenden Objekt</typeparam>
        /// <param name="data">Das Objekt als serialisierter JSON string</param>
        /// <returns>Gibt ein Tupel zurück. success gibt an ob die Methode Erfolg hatte, element gibt das deserialisierte Element an,message gibt die Nachricht im Fehlerfall and</returns>
        public static (bool success, T element, string message) Deserialize<T>(string data) where T : new()
        {
            try
            {
                return (true, JsonSerializer.Deserialize<T>(data), "Erfolgreich deserialisiert");
            }
            catch (Exception ex)
            {
                return (false, default, ex.Message);
            }
        }

        /// <summary>
        /// Schreibt eine Liste an Elementen in eine JSON Datei mit dem gegebenen Pfad
        /// </summary>
        /// <typeparam name="T">Der Typ der Listen elemente welche geschrieben werden sollen</typeparam>
        /// <param name="elemente">Eine Liste mit elementen welche als JSON Geschrieben werden sollen</param>
        /// <param name="pfad">Die Datei die geschrieben werden soll</param>
        /// <returns>Gibt ein Tupel zurück. success gibt an ob die Methode Erfolg hatte, message gibt eine Nachricht im Fehlerfall an</returns>
        public static (bool success, string message) WriteList<T>(List<T> elemente, string datei) where T : new()
        {
            return Write<List<T>>(elemente, datei);
        }

        /// <summary>
        /// List eine Liste an Elementen aus einer JSON Datei aus
        /// </summary>
        /// <typeparam name="T">Der Type der Elemente die gelesen werden sollen</typeparam>
        /// <param name="datei">Die Datei aus der die Elemente gelesen werden sollen</param>
        /// <returns>Gibt ein Tupel zurück. success gibt an ob die Methode Erfolg hatte, elements gibt das deserialiserte Listen Objekt and, message gibt die Nachricht im Fehlerfall an</returns>
        public static (bool success, List<T> elements, string message) ReadList<T>(string datei) where T : new()
        {
            var readRes = Read<List<T>>(datei);
            if (readRes.success)
            {
                if (readRes.element == null)
                {
                    readRes.element = new List<T>();
                }
            }

            return readRes;
        }

        /// <summary>
        /// Hängt ein Element an das Ende einer JSON Liste an
        /// </summary>
        /// <typeparam name="T">Der Typ der Elemente die in der JSON Liste enthalten sind</typeparam>
        /// <param name="element">Das amzuhängende Element</param>
        /// <param name="datei">Die Datei in der die JSON Liste gespeichert wird</param>
        /// <returns>Gibt ein Tupel zurück. succes gibt den Erfolg der Methode an, message gibt die Nachricht im Fehlerfall an</returns>
        public static (bool success, string message) ListAppend<T>(T element, string datei) where T : new()
        {
            var (success, elements, message) = ReadList<T>(datei);
            if (success)
            {
                elements.Add(element);
                return WriteList<T>(elements, datei);
            }
            else
            {
                return (false, message);
            }
        }

        /// <summary>
        /// Hängt ein Element am Anfang einer JSON Liste an
        /// </summary>
        /// <typeparam name="T">Der Typ der Elemente in der JSON Liste</typeparam>
        /// <param name="element">Das Vorzuhängende Element</param>
        /// <param name="datei">Die Datei an die das JSON Element vorgehängt werden soll</param>
        /// <returns>Gibt ein Tupel zurück. success gibt den Erfolg der Methode an, message gibt die Nachricht im Fehlerfall an</returns>
        public static (bool success, string message) ListPrepend<T>(T element, string datei) where T : new()
        {
            var (success, elements, message) = ReadList<T>(datei);
            if (success)
            {
                elements.Insert(0, element);
                return WriteList<T>(elements, datei);
            }
            else
            {
                return (false, message);
            }
        }

        /// <summary>
        /// Löscht ein Element aus einer JSON Liste
        /// </summary>
        /// <typeparam name="T">Der Typ der Elemente in der JSON Liste</typeparam>
        /// <param name="element">Das zu löschende Element</param>
        /// <param name="datei">Die Datei aus der das Element gelöscht werden soll</param>
        /// <returns>Gibt ein Tupel zurück. success gibt den Erfolg der Methode an, message gibt die Nachricht im Fehlerfall an</returns>
        public static (bool success, string message) ListDelete<T>(T element, string datei) where T : new()
        {
            (bool success, List<T> elements, string message) = ReadList<T>(datei);
            if (success)
            {
                while (elements.Contains(element))
                {
                    elements.Remove(element);
                }

                return WriteList<T>(elements, datei);
            }
            else
            {
                return (false, message);
            }


        }

        /// <summary>
        /// Löscht ein Element aus einer JSON Liste
        /// </summary>
        /// <typeparam name="T">Der Typ der Elemente in der JSON Liste</typeparam>
        /// <param name="removeCond">Die Kondition unter der ein Element entfernt werden soll</param>
        /// <param name="datei">Die Datei aus der das Element gelöscht werden soll</param>
        /// <returns>Gibt ein Tupel zurück. success gibt den Erfolg der Methode an, message gibt die Nachricht im Fehlerfall an</returns>
        public static (bool success, string message) ListDelete<T>(Predicate<T> removeCond, string datei) where T : new()
        {
            var (success, elements, message) = ReadList<T>(datei);
            if (success)
            {
                elements.RemoveAll(removeCond);
                return WriteList<T>(elements, datei);
            }
            else
            {
                return (false, message);
            }


        }
    }
}
