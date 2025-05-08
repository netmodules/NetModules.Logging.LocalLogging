using NetModules.Events;
using NetTools.Serialization.JsonSchemaAttributes;

namespace NetModules.Logging.LocalLogging.Events
{
    /// <summary>
    /// This event is handled by the Modules.Logging.LocalLogging.LoggingModule and allows you to read lines
    /// from the local logging file.
    /// </summary>
    [JsonSchemaTitle("Last Line Event")]
    [JsonSchemaDescription("This event is handled by the Modules.Logging.LocalLogging.LoggingModule and returns the last line to be logged.")]
    public class LastLineEvent : Event<EmptyEventInput, ReadLoggingFileEventOutput>
    {
        /// <inheritdoc/>
        public override EventName Name => "Logging.LocalLogging.LastLine";
    }
}
