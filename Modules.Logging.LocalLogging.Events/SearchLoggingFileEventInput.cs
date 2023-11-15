using System;
using NetModules.Interfaces;
using NetTools.Serialization.JsonSchemaAttributes;

namespace Modules.Logging.LocalLogging.Events
{
    /// <summary>
    /// This is the event input object type for a Search Logging File Event.
    /// </summary>
    [JsonSchemaTitle("Search Logging File Input")]
    [JsonSchemaDescription("This is the event input object type for a Search Logging File Event.")]
    public struct SearchLoggingFileEventInput : IEventInput
    {
        /// <summary>
        /// Enter a query to search from the local logging file.
        /// </summary>
        [JsonSchemaTitle("Query")]
        [JsonSchemaDescription("Enter a query to search from the local logging file.")]
        public string Query { get; set; }


        /// <summary>
        /// Select the number of lines you wish to return from the local logging file.
        /// </summary>
        [JsonSchemaTitle("Max Lines")]
        [JsonSchemaDescription("Select the number of lines you wish to return from the local logging file.")]
        public ushort MaxLines { get; set; }
    }
}
