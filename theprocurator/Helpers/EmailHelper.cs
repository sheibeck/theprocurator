using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace theprocurator.Helpers
{
    public class EmailHelper
    {
        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="bodyHtml">The body HTML.</param>
        /// <param name="bodyPlain">The body plain.</param>
        /// <param name="mailTo">The mail to.</param>
        /// <param name="mailFrom">The mail from.</param>
        public static async Task<Response> SendMail(string mailTo, string templateId, string subject, Dictionary<string, string> substitutions)
        {
            var mailFrom = ConfigurationManager.AppSettings["TPN_Email_From"];

            // get SendGrid info
            var apiKey = ConfigurationManager.AppSettings["SendGridApiKey"];
            var client = new SendGridClient(apiKey);

            // set mail variables            
            var from = new EmailAddress(mailFrom);
            var to = new EmailAddress(mailTo);

            // create an email
            string bodyHtml = " ";
            string bodyPlain = " ";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, bodyPlain, bodyHtml);
            msg.SetTemplateId(templateId);
            msg.AddSubstitutions(substitutions);

            // setup mail settings
            msg.MailSettings = new MailSettings();

            // turn on sandbox mode by default;
            var enableSandbox = false;
#if DEBUG
            enableSandbox = true;
#endif
            msg.MailSettings.SandboxMode = new SandboxMode() { Enable = enableSandbox };
        

            // sendemail
            return await client.SendEmailAsync(msg);
        }

        /// <summary>
        /// Determines whether [is mail sent] [the specified mail].
        /// </summary>
        /// <param name="mail">The mail.</param>
        /// <returns>
        ///   <c>true</c> if [is mail sent] [the specified mail]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMailSent(Response mail)
        {
            switch (mail.StatusCode.ToString().ToLower())
            {
                case "ok":
                case "accepted":
                    return true;
                default:
                    return false;
            }
        }
    }
}