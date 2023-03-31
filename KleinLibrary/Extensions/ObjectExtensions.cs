using KleinLibrary.Attributes;
using KleinLibrary.Core;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace KleinLibrary.Erweiterungen
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Weißt alle Werte der Columns von der DataRow den entsprechend Attribut des Übergebenes Objects zu.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T DataRowZuObjekt<T>(T obj, DataRow row)
        {
            if (row == null)
            {
                throw new ArgumentNullException($"{nameof(row)} darf nicht null sein");
            }

            if (obj == null)
            {
                throw new ArgumentNullException($"{nameof(obj)} darf nicht null sein");
            }

            Type objectType = obj.GetType();
            List<PropertyInfo> properties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).ToList();

            foreach (DataColumn col in row.Table.Columns)
            {
                string colName = Encoding.UTF8.GetString(Encoding.Default.GetBytes(col.ColumnName));

                IEnumerable<PropertyInfo> matchingProps = properties.Where(x => x.Name.Equals(colName, StringComparison.OrdinalIgnoreCase));

                PropertyInfo? prop = null;
                if (matchingProps.Any())
                {
                    prop = matchingProps.First();
                }
                else
                {
                    foreach (PropertyInfo tmp in properties)
                    {
                        CompareFieldAttribute? attribute = tmp.GetCustomAttribute<CompareFieldAttribute>();
                        if (attribute != null)
                        {
                            if (attribute.DatenbankFeld.Equals(colName, StringComparison.OrdinalIgnoreCase))
                            {
                                prop = tmp;
                                break;
                            }
                        }
                    }
                }

                if (prop == null)
                {
                    continue;
                }
                else
                {

                    object? converted = ConvertRowToProp(prop, row, colName);

                    prop.SetValue(obj, converted, null);
                }
            }
            return obj;
        }
        private static object? ConvertRowToProp(PropertyInfo prop, DataRow row, string colName)
        {
            Type compareType = prop.PropertyType;
            var t = Nullable.GetUnderlyingType(prop.PropertyType);
            if (t is not null)
                compareType = t;


            if (compareType == typeof(string))
            {
                if (String.IsNullOrEmpty(row[colName].ToString()))
                {
                    return null;
                }
                else
                {
                    return row[colName].ToString();
                }
            }
            else if (compareType == typeof(int))
            {
                if (!Int32.TryParse(row[colName].ToString(), out int result))
                {
                    return null;
                }
                else
                {
                    return result;
                }
            }
            else if (compareType == typeof(decimal))
            {
                if (!Decimal.TryParse(row[colName].ToString(), out decimal result))
                {
                    return null;
                }
                else
                {
                    return result;
                }
            }
            else if (compareType == typeof(bool))
            {
                switch (row[colName].ToString())
                {
                    case "Y":
                        return true;
                    case "N":
                        return false;
                    case "1":
                        return true;
                    case "0":
                        return false;
                    default:
                        {
                            if (Boolean.TryParse(row[colName].ToString(), out bool outBool))
                            {
                                return outBool;
                            }
                            else
                            {
                                return false;
                            }
                        }
                }
            }
            else if (compareType == typeof(DateTime))
            {
                if (!DateTime.TryParse(row[colName].ToString(), out DateTime result))
                {
                    return null;
                }
                else
                {
                    return result;
                }
            }
            else if (compareType == typeof(byte[]))
            {
                var tmp = row.Field<byte[]>(colName);
                return tmp;
            }
            else if (compareType.IsEnum)
            {
                if (!row.IsNull(colName))
                {
                    string rowVal = row[colName].ToString()!;
                    try
                    {
                        if (Int32.TryParse(rowVal, out int tmpInt))
                        {
                            return Enum.ToObject(prop.PropertyType, tmpInt);
                        }
                        else if (long.TryParse(rowVal, out long tmpLong))
                        {
                            return Enum.ToObject(prop.PropertyType, tmpLong);
                        }
                        else if (byte.TryParse(rowVal, out byte tmpByte))
                        {
                            return Enum.ToObject(prop.PropertyType, tmpByte);
                        }

                        if (Enum.TryParse(prop.PropertyType, rowVal, out object? tmpEnum))
                        {
                            if (tmpEnum is not null)
                            {
                                return tmpEnum;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            return null;
        }
        private static bool HasEnumerableChanged(object original, object vergleich)
        {
            IEnumerable? originalEnumerable = original as IEnumerable;
            IEnumerable? vergleichEnumerable = vergleich as IEnumerable;


            if (originalEnumerable is null && vergleichEnumerable is null)
            {
                return false;
            }

            if (originalEnumerable is null || vergleichEnumerable is null)
            {
                return true;
            }

            int originalCount = 0;
            int vergleichCount = 0;
            foreach (var item in originalEnumerable)
            {
                originalCount++;
            }

            foreach (var item in vergleichEnumerable)
            {
                vergleichCount++;
            }

            if (originalCount != vergleichCount)
            {
                return true;
            }

            foreach (var item in originalEnumerable)
            {
                bool foundOneMatching = false;
                foreach (var modItem in vergleichEnumerable)
                {
                    // Check for different types to support base abstraction
                    if (modItem.GetType() != item.GetType())
                    {
                        continue;
                    }

                    if (!item.HasBeenModified(modItem, true))
                    {
                        foundOneMatching = true;
                        break;
                    }
                }
                if (!foundOneMatching)
                {
                    return true;
                }
            }

            return false;
        }
        public static bool HasBeenModified<T>(this T original, T vergleich, bool innerLoop = false)
        {
            if (original is null && vergleich is null)
            {
                return false;
            }

            if (original is null || vergleich is null)
            {
                return true;
            }

            // String Implementiert IEnumerable<char> daher prüfen wir string davor, damit wir nicht jedes Zeichen im Loop durchgehen müssen
            if (original is string && vergleich is string)
            {
                if (!original.Equals(vergleich))
                {
                    return true;
                }
            }
            else if (original is IEnumerable && vergleich is IEnumerable)
            {
                if (HasEnumerableChanged(original, vergleich))
                {
                    return true;
                }
            }
            else
            {
                foreach (PropertyInfo prop in original.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var attribute = prop.GetCustomAttribute<IgnoreModificationCheckAttribute>();
                    if (attribute != null)
                    {
                        continue;
                    }

                    object? originalValue = prop.GetValue(original);
                    object? vergleichValue = prop.GetValue(vergleich);

                    if (originalValue is string)
                    {
                        if (String.IsNullOrWhiteSpace(originalValue as string) && String.IsNullOrWhiteSpace(vergleichValue as string))
                        {
                            continue;
                        }
                        else
                        {
                            if (!originalValue.Equals(vergleichValue))
                            {
                                return true;
                            }
                        }
                    }

                    if (originalValue is null && vergleichValue is null)
                    {
                        continue;
                    }

                    if (originalValue is null || vergleichValue is null)
                    {
                        return true;
                    }

                    if (originalValue is IEnumerable && vergleichValue is IEnumerable)
                    {
                        if (HasEnumerableChanged(originalValue, vergleichValue))
                        {
                            return true;
                        }
                    }
                    else if (originalValue is IEnumerable || vergleichValue is IEnumerable)
                    {
                        return true;
                    }
                    else if (originalValue.GetType().IsEnum && vergleichValue.GetType().IsEnum)
                    {
                        if (!Enum.Equals(originalValue, vergleichValue))
                        {
                            return true;
                        }
                    }
                    else if (originalValue.GetType().IsEnum || vergleichValue.GetType().IsEnum)
                    {
                        return true;
                    }
                    else
                    {
                        MethodInfo? setter = prop.GetSetMethod(/*nonPublic*/ true);
                        if (setter is null)
                        {
                            continue;
                        }

                        if (prop.PropertyType.IsPrimitive || originalValue is string or decimal or float or double)
                        {
                            if (!originalValue.Equals(vergleichValue))
                            {
                                return true;
                            }
                        }
                        else if (originalValue is DateTime)
                        {
                            DateTime? originalDT = originalValue as DateTime?;
                            DateTime? vergleichDT = vergleichValue as DateTime?;
                            if (!DateTime.Equals(originalDT, vergleichDT))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (prop.PropertyType == original.GetType())
                            {
                                continue;
                            }

                            if (originalValue.HasBeenModified(vergleichValue, true))
                            {
                                return true;
                            }
                        }
                    }

                }
            }
            return false;
        }
        public static T DeepCopy<T>(this T originalValue) => originalValue.DeepCopyByExpressionTree<T>();
    }
}
