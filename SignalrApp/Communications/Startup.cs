using Microsoft.Owin;  
using Owin;

[assembly: OwinStartupAttribute(typeof(SignalrApp.Communications.Startup))] 
namespace SignalrApp.Communications
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)  
        {  
            app.MapSignalR();  
        }  
    }
}