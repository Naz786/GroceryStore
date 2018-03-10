using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ValueVille.Startup))]
namespace ValueVille
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
