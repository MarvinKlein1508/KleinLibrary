namespace KleinLibrary.Emails
{
    /// <summary>
    /// Die Struktur die für einen Email Anhang vorgegeben ist.
    /// </summary>
    public struct EmailAnhang
    {
        public string Pfad { get; set; }
        public string Name { get; set; }

        public EmailAnhang(string pfad, string name)
        {
            Pfad = pfad;
            Name = name;
        }

        /// <summary>
        /// Prüft ob ein Beleg Anhang Valide ist. Die Prüfung wird auf Basis des Dateipfad und des Namen ausgeführt.
        /// </summary>
        /// <returns>Gibt einen <see cref="bool"/> zurück der angibt ob der Dateianhang valide ist oder nicht</returns>
        public bool IsValid()
        {
            return File.Exists(Pfad) && !string.IsNullOrWhiteSpace(Name);
        }
    }
}
