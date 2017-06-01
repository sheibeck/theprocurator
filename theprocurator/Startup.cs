using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(theprocurator.Startup))]
namespace theprocurator
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
