using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebViewer.Startup))]
namespace WebViewer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
