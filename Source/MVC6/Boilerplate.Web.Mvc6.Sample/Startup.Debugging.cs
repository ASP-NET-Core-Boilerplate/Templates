﻿namespace MvcBoilerplate
{
    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.Diagnostics;
    using Microsoft.AspNet.Hosting;
    using Microsoft.Framework.Logging;

    public partial class Startup
    {
        /// <summary>
        /// Configure tools used to help with debugging the application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="environment">The environment the application is running under. This can be Development, 
        /// Staging or Production by default.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        private static void ConfigureDebugging(
            IApplicationBuilder application,
            IHostingEnvironment environment,
            ILoggerFactory loggerFactory)
        {
            // Add the following to the request pipeline only in development environment.
            if (environment.IsDevelopment())
            {
                // Add the console logger, which logs events to the Console, including errors and trace information.
                loggerFactory.MinimumLevel = LogLevel.Information;
                loggerFactory.AddConsole();

                // Browse to /runtimeinfo to see information about the runtime that is being used and the packages that 
                // are included in the application. See http://docs.asp.net/en/latest/fundamentals/diagnostics.html
                application.UseRuntimeInfoPage();

                // Allow updates to your files in Visual Studio to be shown in the browser. You can use the Refresh 
                // browser link button in the Visual Studio toolbar or Ctrl+Alt+Enter to refresh the browser.
                // TODO: Browser Link does not work in ASP.NET 5 Beta 5. Add it back in the next version.
                application.UseBrowserLink();
            }
        }
    }
}