using NetTools.Serialization.JsonSchemaAttributes;
using NetModules.Events;
using NetModules.Interfaces;
using NetModules.Logging.LocalLogging.Events.Enums;

namespace NetModules.Logging.LocalLogging.Events
{
    /// <summary>
    /// This is the event input object type for a Set Logging Level Event.
    /// </summary>
    [JsonSchemaTitle("Set Loggging Level Input")]
    [JsonSchemaDescription("This is the event input object type for a Set Logging Level Event.")]
    public struct SetLoggingLevelEventInput : IEventInput
    {
        /// <summary>
        /// Select a logger to apply a new logging level for.
        /// </summary>
        [JsonSchemaTitle("Logger")]
        [JsonSchemaDescription("Select a logger to apply a new logging level for.")]
        public Logger Logger { get; set; }


        /// <summary>
        /// Set the level of logging to output for the selected logger(s).
        /// </summary>
        [JsonSchemaTitle("Severity")]
        [JsonSchemaDefault(LoggingEvent.Severity.Error)]
        [JsonSchemaDescription("Set the level of logging to output for the selected logger(s).")]
        public LoggingEvent.Severity Severity { get; set; }
    }
}
