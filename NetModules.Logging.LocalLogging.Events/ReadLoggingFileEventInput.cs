using NetTools.Serialization.JsonSchemaAttributes;
using NetModules.Interfaces;
using NetModules.Logging.LocalLogging.Events.Enums;

namespace NetModules.Logging.LocalLogging.Events
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


        /// <summary>
        /// You can select whether to read log file from the start of the file (Head) or
        /// from the end of the file (Tail).
        /// </summary>
        [JsonSchemaTitle("Read Mode")]
        [JsonSchemaDescription("You can select whether to read log file from the start of the file (Head) or from the end of the file (Tail).")]
        public ReadMode ReadMode { get; set; }
    }
}
