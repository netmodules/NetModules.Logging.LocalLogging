using Modules.Logging.LocalLogging.Events;
using Modules.WebServer.DashboardModule;
using Modules.WebServer.DashboardModule.Classes;
using Modules.WebServer.DashboardModule.Classes.Plugins;
using Modules.WebServer.DashboardModule.Interfaces;
using Modules.UserManagement;
using Modules.Dashboard.User;
using Modules.UserManagement.Events;
using Modules.Dashboard.LocalLogging.Classes;
using NetModules;
using NetModules.Events;
using NetModules.Interfaces;
using NetTools.Web;
using NetTools.Serialization;
//using static Modules.WebServer.DashboardModule.DashboardModule;

namespace Modules.Dashboard.LocalLogging 
{
    [Module] 
    public class DashboardLoggingModule : DashboardModule 
    {  
        public override void OnLoaded()   
        {   
            base.OnLoaded();

            Log(LoggingEvent.Severity.Debug, "LocalLogging Dashboard module loaded.");

            AddAjaxCallback("local-logging-callback", (path, request, session, authorization) =>
            {
                if (UserHelpers.HasAccess(this, session, authorization)
                    && request.POST && !string.IsNullOrEmpty(request.RequestText))
                {
                    var json = request.RequestText.ToDictionary();

                    if (!json.TryGetValueAs<int>(out var lines, "num-lines"))
                    {
                        lines = 100;
                    }
                    else
                    {
                        lines = Math.Clamp(lines, 1, 10000);
                    }

                    if (!json.TryGetValueAs<int>(out var skip, "skip-lines"))
                    {
                        skip = 0;
                    }

                    if (!json.TryGetValueAs<string>(out var find, "log-find"))
                    {
                        find = string.Empty;
                    }

                    if (!string.IsNullOrWhiteSpace(find))
                    {
                        var logFileEvent = new SearchLoggingFileEvent()
                        {
                            Input = new SearchLoggingFileEventInput()
                            {
                                MaxLines = (ushort)lines,
                                Query = find
                            }
                        };

                        Host.Handle(logFileEvent);
                        request.ResponseText.Append(logFileEvent.Output.Log);
                        return;
                    }
                    else
                    {
                        var logFileEvent = new ReadLoggingFileEvent()
                        {
                            Input = new ReadLoggingFileEventInput()
                            {
                                Lines = (ushort)lines,
                                SkipLines = (ulong)skip
                            }
                        };

                        Host.Handle(logFileEvent);
                        request.ResponseText.Append(logFileEvent.Output.Log);
                        return;
                    }
                }

                request.ResponseStatus = System.Net.HttpStatusCode.Unauthorized;
            }); 
             
        }

        public override void OnAllModulesLoaded()
        {
            base.OnAllModulesLoaded();

            DashboardAdmin.AddDashboardAdminSection(this);
        } 
        

        public override List<FetchFooter> Footer(string session, string authorization)
        {
            return new List<FetchFooter>()
            {
                new FetchFooter()
                {
                    Position = WebServer.DashboardModule.Enums.PositionHorizontal.Left,
                    Dependencies = new()
                    {
                        new StylesheetDependency($"footer/footer-link{Version}.css")
                    }
                }
            };
            //throw new NotImplementedException();
        }


        public override byte[] GetResource(string[] path, string session, string authorization)
        {
            if (path.Length == 0 || string.IsNullOrWhiteSpace(path[0]))
            {
                return null;
            }

            string relativePath = path[0];

            try
            {
                // The following statement is evaluated for operating systems that require case-sensitive
                // file paths. This could be written into a switch/case rather than a nested if statement.
                if (path.Length > 1)
                {
                    var path0 = path[0].ToLowerInvariant();

                    if (path0 == "navbar")
                    {
                        path[0] = "Navbar";
                    }
                    else if (path0 == "sidebar")
                    {
                        path[0] = "Sidebar";
                    }
                    else if (path0 == "pages")
                    {
                        path[0] = "Pages";
                    }
                    else if (path0 == "footer")
                    {
                        path[0] = "Footer";
                    }
                    else if (path[0] == "locallogging")
                    {
                        path[0] = "LocalLogging";
                    }
                    else if (path0 == "plugins")
                    {
                        path[0] = "Plugins";
                    }  

                    relativePath = Path.Combine(path);  
                }
                 
                if (relativePath.EndsWith(Version + ".js")
                        || relativePath.EndsWith(Version + ".css"))
                { 
                    relativePath = relativePath.Remove(relativePath.LastIndexOf(Version), Version.Length); 
                }

                var file = Path.Combine(ResourcesPath, relativePath);

                if (File.Exists(file))
                {
                    return File.ReadAllBytes(file);
                }

                Log(LoggingEvent.Severity.Debug
                    , "This is raising a NetModules.Events.LoggingEvent from GetResource() method."
                    , new FileNotFoundException(file));
            }
            catch (Exception ex)
            {  
                Log(LoggingEvent.Severity.Error
                    , "This is raising a NetModules.Events.LoggingEvent from GetResource() method."
                    , ex);
            }

            // Potentially, an invalid file has been requested as a resource from this DashboardModule. This
            // could have been from a malicious script that has found a web link to your module's GetResource
            // path and in this case will be ignored. If your own resource is not found while developing your
            // DashboardModule you can drop a breakpoint and step through this method to help debugging and
            // ensure your paths are correct.
            return null; 
        }

        
        public override Dictionary<string, FetchPage> Pages(string session, string authorization)
        {
            var logFilePage = new FetchPage()
            {
                Dependencies = new List<IDashboardDependency>()
                {
                    new BootstrapCssDependency(),
                    new StylesheetDependency($"Pages/LocalLogging/local-logging{Version}.css"),
                    new JQueryDependency(),
                    new BootstrapJsDependency()
                }
            };  

            WebServer.DashboardModule.Classes.Plugins.CodeMirror.AddCodeMirrorPlugins(logFilePage.Dependencies
                , CodeMirror.CodeMirrorOptions.Hyperlinks
                , CodeMirror.Addons.Dialog, CodeMirror.Addons.ScrollAddons.AnnotateScrollbar
                , CodeMirror.Addons.SearchAddons.SearchCursor, CodeMirror.Addons.SearchAddons.Search
                , CodeMirror.Addons.SearchAddons.MatchesOnScrollbar, CodeMirror.Addons.SearchAddons.MatchHighlighter
                , CodeMirror.Addons.SearchAddons.JumpToLine);

            logFilePage.Dependencies.Add(new JavascriptDependency($"Pages/LocalLogging/local-logging{Version}.js"));
         
             
            return new Dictionary<string, FetchPage> 
            { 
                {
                    "local-logging/read-logs",
                    logFilePage        
                }
            };
        }


        public override void RenderFooter(DataRequestEventArgs request, string session, string authorization)
        {
            if (UserHelpers.HasAccess(this, session, authorization))
            {
                request.ResponseText.Append("<a href=\"/" + DashboardSlug + "local-logging/read-logs\">View System Log</a>");
            }
        }

        
        public override void RenderPage(DataRequestEventArgs request, string session, string authorization)
        {
            if (!UserHelpers.HasAccess(this, session, authorization)) 
            {
                return;
            }

            try
            {
                if (request.PathParts.Last() == "read-logs")
                {
                    var htmlFile = Path.Combine(ResourcesPath, "Pages", "LocalLogging", "local-logging.html");
                    request.ResponseText.Append(File.ReadAllText(htmlFile));
                    return;
                }
            }
            catch (Exception ex)
            {

                request.ResponseText.Append(ex);
            }
        }
    }
}