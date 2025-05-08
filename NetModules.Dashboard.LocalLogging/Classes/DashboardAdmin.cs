using NetModules.Dashboard.User.Events;
using NetModules.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModules.Dashboard.LocalLogging.Classes
{
    internal class DashboardAdmin
    {
        internal const string EnabledMetaKey = "dashboardLocalLoggingModuleEnabled";


        internal static void AddDashboardAdminSection(IModule module)
        {
            var adminSection = new AddUserAdminSectionEvent()
            {
                Input = new AddUserAdminSectionEventInput()
                {
                    Name = "dashboardLocalLogging",
                    Section = new Dashboard.User.Events.ProfileFields.ProfileSection()
                    {
                        Title = "Local Logging Module",
                        Description = "Allow this user to view log file reader.",
                        Fields = new Dictionary<string, Dashboard.User.Events.ProfileFields.ProfileField>()
                        {
                            {
                                EnabledMetaKey, new Dashboard.User.Events.ProfileFields.SimpleProfileField()
                                {
                                    Title = "Enable Log Reader Page",
                                    Type = "checkbox",
                                    Value = true,
                                }
                            },
                        }
                    }
                }
            };

            module.Host.Handle(adminSection);
        }
    }
}
