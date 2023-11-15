using System;
using NetModules.Interfaces;
using NetTools.Serialization.JsonSchemaAttributes;

namespace Modules.Logging.LocalLogging.Events
{
    /// <summary>
    /// This is the event input object type for a Read Logging File Event.
    /// </summary>
    [JsonSchemaTitle("Read Logging File Input")]
    [JsonSchemaDescription("This is the event input object type for a Read Logging File Event.")]
    public struct ReadLoggingFileEventInput : IEventInput
    {
        /// <summary>
        /// Select the number of lines you wish to return from the local logging file.
        /// </summary>
        [JsonSchemaTitle("Lines")]
        [JsonSchemaDefault(100)]
        [JsonSchemaDescription("Select the number of lines you wish to return from the local logging file.")]
        public ushort Lines { get; set; }


        /// <summary>
        /// Select the number of lines you wish to skip from the local logging file.
        /// </summary>
        [JsonSchemaTitle("Skip Lines")]
        [JsonSchemaDefault(0)]
        [JsonSchemaDescription("Select the number of lines you wish to skip from the local logging file.")]
        public ulong SkipLines { get; set; }
    }
}
