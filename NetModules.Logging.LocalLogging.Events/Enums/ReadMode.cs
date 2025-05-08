namespace NetModules.Logging.LocalLogging.Events.Enums
{
    /// <summary>
    /// Enumeration of available read modes for the logging file.
    /// </summary>
    public enum ReadMode
    {
        /// <summary>
        /// Read from the end of the file.
        /// </summary>
        Tail,
        /// <summary>
        /// Read from the start of the file.
        /// </summary>
        Head
    }
}
