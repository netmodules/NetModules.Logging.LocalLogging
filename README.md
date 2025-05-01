# NetModules.Logging.LocalLogging

**NetModules.Logging.LocalLogging** is a [NetModules](https://github.com/netmodules/NetModules) module that handles the built in [LoggingEvent](https://github.com/netmodules/NetModules/blob/main/NetModules/Events/LoggingEvent.cs) and outputs to Console 8f available, as well as logging to local files.

Every loaded module in a loaded ModuleHost can raise a `LoggingEvent` either mannually by instantiating and sending it to Host.Handle, or by using to `this.Log` wrapper method. A module doesn't need to worry about how logging is handled unless it's a logging module!

## NetModules.Logging.LocalLogging.Events

The **NetModules.Logging.LocalLogging** module handles and exposes its own events for interacting with the local log files, these events can be referenced and raised by other modules to read log data.

### These events are as follows:

- **LastLine**: Returns the last `LoggingEvent` record in the event otput.
- **ReadLogFile**: Returns n lines from the local log file in the event output.
- **SearchLogFile**: Returns n lines from the local log file that match a query in the event output.
- **SetLoggingLevel**: Allows you to dynamically override the logging level that is written to local files. This may be useful if you need to read a level urgently.

## Getting Started

### Installation

To use **NetModules.Logging.LocalLogging**, add the library to a project where you instantiate a [ModuleHost](https://github.com/netmodules/NetModules/tree/main?tab=readme-ov-file#creating-and-loading-a-module-host) via NuGet Package Manager:
```bash
Install-Package NetModules.Logging.LocalLogging
```
Logging will be automatically enabled when you load the module into your ModuleHost. 


To use events from **NetModules.Logging.LocalLogging.Events**, add the events library to a module project via NuGet Package Manager:
```bash
Install-Package NetModules.Logging.LocalLogging.Events
```
You can then instantiate an event and raise it to ModuleHost via your module's `this.Host.Handle` method for a handling module to process. 


## Contributing

We welcome contributions! To get involved:
1. Fork [NetModules](https://github.com/netmodules/NetTools.Logging), make improvements, and submit a pull request.
2. Code will be reviewed upon submission.
3. Join discussions via the [issues board](https://github.com/netmodules/NetTools.Logging/issues).

This project must always strictly adhere to the [NetModules](https://github.com/netmodules/NetModules) architecture and design pattern!

## License

NetModules.Logging.LocalLogging is licensed under the [MIT License](https://tldrlegal.com/license/mit-license), allowing unrestricted use, modification, and distribution. If you use NetModules.Logging.LocalLogging in your own project, we’d love to hear about your experience, and possibly feature you on our website!

Full documentation coming soon!

[NetModules Foundation](https://netmodules.net/)
