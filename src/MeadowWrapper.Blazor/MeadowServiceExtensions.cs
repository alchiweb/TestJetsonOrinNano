using System;
using System.Collections.Generic;
using System.Text;

namespace Meadow.Blazor
{
    public static class MeadowServiceExtensions
    {
        //
        // Résumé :
        //     Registers and starts the Meadow application when the host application starts.
        //
        //
        // Paramètres :
        //   app:
        //     The Microsoft.AspNetCore.Builder.WebApplication instance.
        //
        // Paramètres de type :
        //   T:
        //     The Meadow application type that implements Meadow.IApp.
        public static void UseMeadow<T>(this WebApplication app) where T : IApp, new()
        {
            IHostApplicationLifetime requiredService = app.Services.GetRequiredService<IHostApplicationLifetime>();
            requiredService.ApplicationStarted.Register(async delegate
            {
                var app = new T();
                await app.Initialize();
                await app.Run();
            });
        }
    }
}
