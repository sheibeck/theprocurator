using Microsoft.VisualStudio.TestTools.UnitTesting;
using theprocurator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace theprocurator.Helpers.Tests
{
    [TestClass()]
    public class EmailHelperTests
    {
        [TestMethod()]
        public void SendMailTest()
        {
            var sub = new Dictionary<string, string>();
                sub.Add("[%callbackUrl%]", "http://callback.url/");

            Task<SendGrid.Response> sendmail = EmailHelper.SendMail("sheibeck@gmail.com", ConfigurationManager.AppSettings["TPN_Account_Confirm"], "", sub);

            sendmail.Wait();

            Assert.AreEqual("OK", sendmail.Result.StatusCode.ToString());
        }
    }
}