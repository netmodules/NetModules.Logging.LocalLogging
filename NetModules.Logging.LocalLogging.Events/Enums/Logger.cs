using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModules.Logging.LocalLogging.Events.Enums
{
    /// <summary>
    /// Enumeration of available loggers.
    /// </summary>
    public enum Logger
    {
        /// <summary>
        /// All loggers.
        /// </summary>
        All,

        /// <summary>
        /// The console logger.
        /// </summary>
        Console,

        /// <summary>
        /// The file logger.
        /// </summary>
        File,
    }
}
