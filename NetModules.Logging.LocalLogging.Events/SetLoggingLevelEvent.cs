using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using NetModules;
using NetModules.Events;
using NetTools.Serialization.JsonSchemaAttributes;
using NetTools.Serialization.JsonSchemaEnums;

namespace Modules.Logging.LocalLogging.Events
{
    /// <summary>
    /// This event is handled by the Modules.Logging.LocalLogging.LoggingModule and allows you to set the logging
    /// level for a selected logger at runtime.
    /// </summary>
    [JsonSchemaTitle("Set Logging Level Event")]
    [JsonSchemaDescription("This event is handled by the Modules.Logging.LocalLogging.LoggingModule and allows you to set the level for a selected logger at runtime.")]
    public class SetLoggingLevelEvent : Event<SetLoggingLevelEventInput, EmptyEventOutput>
    {
        /// <inheritdoc/>
        public override EventName Name => "Logging.LocalLogging.SetLoggingLevel";
    }
}
