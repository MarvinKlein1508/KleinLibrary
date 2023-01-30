using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace KleinLibrary.Erweiterungen
{
    public static class StringErweiterungen
    {
        public static string OnlyDigits(string value)
        {
            Regex regex = new Regex(@"\D");
            return regex.Replace(value, string.Empty);
        }

        /// <summary>
        /// Führt einen Regex durch und überprüft ob ein String eine Email ist
        /// </summary>
        /// <param name="email">Der zu überprüfende String</param>
        /// <returns>Gibt einen <see cref="bool"/> Wert zurück der angibt ob der String eine Email ist</returns>
        public static bool IstEmail(string email) => Regex.IsMatch(email, @"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");

        /// <summary>
        /// Konvertiert einen String zwischen zwei Encodings
        /// </summary>
        /// <param name="input">Der zu konvertierende String</param>
        /// <param name="from">Das aktuelle Encoding des zu konvertierenden Strings</param>
        /// <param name="to">Das Encoding das der String nach der Konvertierung haben soll</param>
        /// <returns></returns>
        public static string ConvertEncoding(string? input, Encoding from, Encoding to)
        {
            if (input is null)
            {
                return String.Empty;
            }

            byte[] fromBytes = from.GetBytes(input);
            byte[] toBytes = Encoding.Convert(from, to, fromBytes);
            return to.GetString(toBytes);
        }

        public static string ParseEan(string ean)
        {
            if (ean == null)
            {
                return "";
            }

            if (string.IsNullOrWhiteSpace(ean))
            {
                return string.Empty;
            }

            if (ean.Length > 13)
            {
                return string.Empty;
            }

            while (ean.Length < 13)
            {
                ean = "0" + ean;
            }


            return ean;
        }

        /// <summary>
        /// Öffnet eine URL im default Webbrowser.
        /// <para>
        /// Diese Methode muss in .NET CORE verwendet werden, aufgrund eines Fehlers bei Process.Start();
        /// </para>
        /// </summary>
        /// <param name="url"></param>
        public static void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        public static string RemoveHTML(this string s)
        {
            if (String.IsNullOrWhiteSpace(s))
            {
                return String.Empty;
            }

            return Regex.Replace(s, "<.*?>", String.Empty);
        }

        public static string ParseUrl(string url)
        {
            url = url.Replace("https://", "");
            url = url.Replace("http://", "");
            url = url.Replace("www.", "");
            return $"//{url}";
        }
        public static string SafeSubstring(this string? text, int start, int length)
        {
            if (text is null)
            {
                return String.Empty;
            }

            if (start > text.Length)
            {
                return text;
            }
            else
            {
                int tmpLen = start + length > text.Length - 1 ? (text.Length - 1) - start : length;
                if (tmpLen <= 0)
                {
                    return text;
                }

                return text.Substring(start, tmpLen);
            }
        }

        public static bool IsValidVat(this string? text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                return true;
            }

            string pattern = @"(?xi)^(
(AT)?U[0-9]{8} |                              # Austria
(BE)?0[0-9]{9} |                              # Belgium
(BG)?[0-9]{9,10} |                            # Bulgaria
(HR)?[0-9]{11} |                              # Croatia
(CY)?[0-9]{8}[A-Z] |                          # Cyprus
(CZ)?[0-9]{8,10} |                            # Czech Republic
(DE)?[0-9]{9} |                               # Germany
(DK)?[0-9]{8} |                               # Denmark
(EE)?[0-9]{9} |                               # Estonia
(EL)?[0-9]{9} |                               # Greece
ES[A-Z][0-9]{7}(?:[0-9]|[A-Z]) |              # Spain
(FI)?[0-9]{8} |                               # Finland
(FR)?[0-9A-Z]{2}[0-9]{9} |                    # France
(GB)?([0-9]{9}([0-9]{3})?|[A-Z]{2}[0-9]{3}) | # United Kingdom
(HU)?[0-9]{8} |                               # Hungary
(IE)?[0-9]{7}[A-Z]{1,2}   |                   # Ireland
(IE)?[0-9][A-Z][0-9]{5}[A-Z] |                # Ireland (2)
(IT)?[0-9]{11} |                              # Italy
(LT)?([0-9]{9}|[0-9]{12}) |                   # Lithuania
(LU)?[0-9]{8} |                               # Luxembourg
(LV)?[0-9]{11} |                              # Latvia
(MT)?[0-9]{8} |                               # Malta
(NL)?[0-9]{9}B[0-9]{2} |                      # Netherlands
(PL)?[0-9]{10} |                              # Poland
(PT)?[0-9]{9} |                               # Portugal
(RO)?[0-9]{2,10} |                            # Romania
(SE)?[0-9]{12} |                              # Sweden
(SI)?[0-9]{8} |                               # Slovenia
(SK)?[0-9]{10}                                # Slovakia
)$";

            return new Regex(pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(text).Success;
        }

        public static bool IsValidEan(string code)
        {
            Regex _gtinRegex = new Regex("^(\\d{8}|\\d{12,14})$");
            if (!_gtinRegex.IsMatch(code)) return false;
            code = code.PadLeft(14, '0');
            int sum = code.Select((c, i) => (c - '0') * ((i % 2 == 0) ? 3 : 1)).Sum();
            return (sum % 10) == 0;
        }
        public static bool ValidateBIC(string bic)
        {
            bic = bic.ToUpper();
            Regex regex = new Regex(@"[A-Z]{6,6}[A-Z2-9][A-NP-Z0-9]([A-Z0-9]{3,3}){0,1}");
            return regex.Match(bic).Success;
        }
        public static bool ValidateIBAN(string iban)
        {
            iban = iban.ToUpper(); //IN ORDER TO COPE WITH THE REGEX BELOW
            if (String.IsNullOrEmpty(iban))
            {
                return false;
            }
            else if (System.Text.RegularExpressions.Regex.IsMatch(iban, "^[A-Z0-9]"))
            {
                iban = iban.Replace(" ", String.Empty);
                string bank =
                $"{iban[4..]}{iban.Substring(0, 4)}";
                int asciiShift = 55;
                StringBuilder sb = new StringBuilder();
                foreach (char c in bank)
                {
                    int v;
                    if (Char.IsLetter(c))
                    {
                        v = c - asciiShift;
                    }
                    else
                    {
                        v = int.Parse(c.ToString());
                    }

                    sb.Append(v);
                }
                string checkSumString = sb.ToString();
                int checksum = int.Parse(checkSumString.Substring(0, 1));
                for (int i = 1; i < checkSumString.Length; i++)
                {
                    int v = int.Parse(checkSumString.Substring(i, 1));
                    checksum *= 10;
                    checksum += v;
                    checksum %= 97;
                }
                return checksum == 1;
            }
            else
            {
                return false;
            }
        }

        public static string ToHumanReadableFileSize(decimal bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (bytes >= 1024 && order < sizes.Length - 1)
            {
                order++;
                bytes = bytes / 1024;
            }
            return String.Format("{0:0.##} {1}", bytes, sizes[order]);
        }
        /// <summary>
        /// Generiert einen zufälligen String mit der angegebenen Länge.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new string(stringChars);
            return finalString;
        }
    }
}

