namespace TesSaveLocationTracker.Skyrim
{
    /// <summary>
    /// Save game RefID type.
    /// </summary>
    public enum RefIDType
    {
        /// <summary>
        /// FormID is stored inside savefile's form ID array.
        /// </summary>
        FormID = 0, // 00        
        /// <summary>
        /// This is default FormID.
        /// </summary>
        Skyrim = 1, // 01        
        /// <summary>
        /// FormID source is from another plugin.
        /// </summary>
        Custom = 2, // 10        
        /// <summary>
        /// Unknown RefID flag.
        /// </summary>
        Unknown = 3 // 11
    }
}
