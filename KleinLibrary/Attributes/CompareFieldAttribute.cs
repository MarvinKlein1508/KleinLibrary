namespace KleinLibrary.Attributes
{
    /// <summary>
    /// This property is being used for extended mapping. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CompareFieldAttribute : Attribute
    {
        /// <summary>
        /// Ruft den zugeordneten Namen zur Datenbank ab.
        /// </summary>
        public string DatenbankFeld { get; }

        /// <summary>
        /// Erstellt ein neues CompareAttribut
        /// </summary>
        /// <param name="feld">Der Name des Feldes, wie es aus der Datenbank ausgelesen wird.</param>
        public CompareFieldAttribute(string feld)
        {
            this.DatenbankFeld = feld;
        }
    }
}
