using NetTools.Serialization.JsonSchemaAttributes;

namespace NetModules.Logging.LocalLogging.Events
{
    /// <summary>
    /// This event is handled by the Modules.Logging.LocalLogging.LoggingModule and allows you to read lines
    /// from the local logging file.
    /// </summary>
    [JsonSchemaTitle("Read Logging File Event")]
    [JsonSchemaDescription("This event is handled by the Modules.Logging.LocalLogging.LoggingModule and allows you to read lines from the local logging file.")]
    public class ReadLoggingFileEvent : Event<ReadLoggingFileEventInput, ReadLoggingFileEventOutput>
    {
        /// <inheritdoc/>
        public override EventName Name => "Logging.LocalLogging.ReadLoggingFile";
    }
}
