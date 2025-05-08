using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using NetModules;
using NetTools.Serialization.JsonSchemaAttributes;
using NetTools.Serialization.JsonSchemaEnums;

namespace Modules.Logging.LocalLogging.Events
{
    /// <summary>
    /// This event is handled by the Modules.Logging.LocalLogging.LoggingModule and allows you to search lines
    /// from the local logging file.
    /// </summary>
    [JsonSchemaTitle("Search Logging File Event")]
    [JsonSchemaDescription("This event is handled by the Modules.Logging.LocalLogging.LoggingModule and allows you to search lines from the local logging file.")]
    public class SearchLoggingFileEvent : Event<SearchLoggingFileEventInput, ReadLoggingFileEventOutput>
    {
        /// <inheritdoc/>
        public override EventName Name => "Logging.LocalLogging.SearchLoggingFile";
    }
}
