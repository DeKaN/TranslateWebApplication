using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TranslateWebApplication.Startup))]
namespace TranslateWebApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
