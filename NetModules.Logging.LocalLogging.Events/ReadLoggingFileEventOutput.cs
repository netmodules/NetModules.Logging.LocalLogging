using NetTools.Serialization.JsonSchemaAttributes;
using NetTools.Serialization.JsonSchemaEnums;
using NetModules.Interfaces;

namespace NetModules.Logging.LocalLogging.Events
{
    /// <summary>
    /// This is the event output object type that is returned by a Read Logging File Event.
    /// </summary>
    [JsonSchemaTitle("Read Logging File Output")]
    [JsonSchemaDescription("This is the event output object type that is returned by a Read Logging File Event.")]
    public class ReadLoggingFileEventOutput : IEventOutput
    {
        /// <summary>
        /// Contains the log text from the local logging file for the number of lines requested.
        /// </summary>
        [JsonSchemaTitle("Log")]
        [JsonSchemaFormat(StringFormat.Multiline)]
        [JsonSchemaDescription("Contains the log text from the local logging file for the number of lines requested.")]
        public string Log { get; set; }
    }
}
