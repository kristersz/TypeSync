namespace TypeSync.Models.Enums
{
    /// <summary>
    /// https://www.typescriptlang.org/docs/handbook/basic-types.html
    /// </summary>
    public enum TypeScriptBasicType
    {
        /// <summary>
        /// Can represent any type
        /// </summary>
        Any = 0,

        /// <summary>
        /// True/False
        /// </summary>
        Boolean = 1,

        /// <summary>
        /// Floating point number
        /// </summary>
        Number = 2,

        /// <summary>
        /// Textual data
        /// </summary>
        String = 3,

        /// <summary>
        /// Array of values
        /// </summary>
        Array = 4,

        /// <summary>
        /// Tuple type
        /// </summary>
        Tuple = 5,

        /// <summary>
        /// Set of numeric values with friendly names
        /// </summary>
        Enum = 6,

        /// <summary>
        /// The absene of any type
        /// </summary>
        Void = 7,

        /// <summary>
        /// Subtype of other types
        /// </summary>
        Null = 8,

        /// <summary>
        /// Subtype of other types
        /// </summary>
        Undefined = 9,

        /// <summary>
        /// Value that never occurs
        /// </summary>
        Never = 10,

        Date = 11
    }
}
