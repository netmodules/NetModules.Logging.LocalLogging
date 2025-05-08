using NetTools.Serialization.JsonSchemaAttributes;
using NetModules.Events;

namespace NetModules.Logging.LocalLogging.Events
{
    /// <summary>
    /// This event is handled by the NetModules.Logging.LocalLogging.LoggingModule and allows you to set the logging
    /// level for a selected logger at runtime.
    /// </summary>
    [JsonSchemaTitle("Set Logging Level Event")]
    [JsonSchemaDescription("This event is handled by the NetModules.Logging.LocalLogging.LoggingModule and allows you to set the level for a selected logger at runtime.")]
    public class SetLoggingLevelEvent : Event<SetLoggingLevelEventInput, EmptyEventOutput>
    {
        /// <inheritdoc/>
        public override EventName Name => "Logging.LocalLogging.SetLoggingLevel";
    }
}
