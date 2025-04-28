using System;
using System.Collections.Generic;
using System.Text;
using NetModules;
using NetModules.Interfaces;
using Modules.UserManagement.Events;

namespace Modules.Dashboard.LocalLogging.Classes
{
    internal static class UserHelpers
    {
        internal static UserManagement.Events.Classes.User GetUser(IModule module, string session, string authorization)
        {
            if (string.IsNullOrEmpty(session) || string.IsNullOrEmpty(authorization))
            {
                return null;
            }

            var getUserEvent = new GetUserEvent()
            {
                Input = new GetUserEventInput()
                {
                    Authorization = authorization,
                    Session = session,
                }
            };

            module.Host.Handle(getUserEvent);

            if (getUserEvent.Handled)
            {
                return getUserEvent.Output.User;
            }

            return null;
        }


        internal static bool HasAccess(IModule module, string session, string authorization)
        {
            var user = GetUser(module, session, authorization);
            return HasAccess(module, user, session, authorization);
        }


        internal static bool HasAccess(IModule module, UserManagement.Events.Classes.User user, string session, string authorization, string userMetaKey = null)
        {
            // This method is used in various method within the Modules.FrontendStarter module to check that a
            // valid user is available. If the user is null while debugging, you may still wish to make your
            // content available so you can test its functionality. The following condition enables this:
            if (user == null)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    return true;
                }

                return false;
            }

            // If the current user is a super admin we return true indefinitely, this allows super admins to
            // access all content in the Modules.WebServer Web App. This may be disabled if required by
            // removing the following condition:
            if (user.UserLevel == UserManagement.Events.Enums.UserLevel.SuperAdmin)
            {
                return true;
            }

            // Raise a GetUserMetadataEvent with the metadata key to see if the user has this permission...
            var meta = new GetUserMetadataEvent()
            {
                Input = new GetUserMetadataEventInput()
                {
                    Authorization = authorization,
                    Session = session,
                    UserId = user.UserId,
                    MetaKey = string.IsNullOrEmpty(userMetaKey) ? DashboardAdmin.EnabledMetaKey : userMetaKey
                }
            };

            module.Host.Handle(meta);

            if (meta.Handled)
            {
                if (meta.Output != null && meta.Output.MetaValue != null)
                {
                    var metaString = meta.Output.MetaValue.ToString();
                    return !string.IsNullOrEmpty(metaString) && bool.TryParse(metaString, out var enabled) && enabled;
                }
            }

            return false;
        }
    }
}
