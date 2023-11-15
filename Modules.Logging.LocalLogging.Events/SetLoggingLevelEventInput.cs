using System;
using NetModules.Events;
using NetModules.Interfaces;
using NetTools.Serialization.JsonSchemaAttributes;

namespace Modules.Logging.LocalLogging.Events
{
    /// <summary>
    /// This is the event input object type for a Set Logging Level Event.
    /// </summary>
    [JsonSchemaTitle("Set Loggging Level Input")]
    [JsonSchemaDescription("This is the event input object type for a Set Logging Level Event.")]
    public struct SetLoggingLevelEventInput : IEventInput
    {
        /// <summary>
        /// Set the level of logging to record in the local logging file.
        /// </summary>
        [JsonSchemaTitle("Severity")]
        [JsonSchemaDefault(100)]
        [JsonSchemaDescription("Set the level of logging to record in the local logging file.")]
        public LoggingEvent.Severity Severity { get; set; }
    }
}
