using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(URLEvaluator.Startup))]
namespace URLEvaluator
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
