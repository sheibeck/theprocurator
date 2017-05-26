
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace theprocurator.Helpers
{
    public class AjaxHelpers
    {
        public class ValidateJSONAntiForgeryHeader : FilterAttribute, IAuthorizationFilter
        {
            public void OnAuthorization(AuthorizationContext filterContext)
            {
                string clientToken = filterContext.RequestContext.HttpContext.Request.Headers.Get(KEY_NAME);
                if (clientToken == null)
                {
                    throw new HttpAntiForgeryException(string.Format("Header does not contain {0}", KEY_NAME));
                }

                string serverToken = filterContext.HttpContext.Request.Cookies.Get(KEY_NAME).Value;
                if (serverToken == null)
                {
                    throw new HttpAntiForgeryException(string.Format("Cookies does not contain {0}", KEY_NAME));
                }

                System.Web.Helpers.AntiForgery.Validate(serverToken, clientToken);
            }

            private const string KEY_NAME = "__RequestVerificationToken";
        }
    }
}