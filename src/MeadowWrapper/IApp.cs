using System;
using System.Collections.Generic;
using System.Text;

namespace Meadow
{
    //
    // Résumé :
    //     Contract for Meadow applications. Provides a way for the Meadow OS to communicate
    //     with Meadow applications when system events are happening.
    public interface IApp
    {/*
        //
        // Résumé :
        //     The application's version number
        static Version Version { get; } = new Version("1.0.0");


        //
        // Résumé :
        //     Settings parsed from the app.config.yaml at startup
        Dictionary<string, string> Settings { get; }

        //
        // Résumé :
        //     A cancellation token that is canceled when the application is signaled to shut
        //     down
        CancellationToken CancellationToken { get; }

        //
        // Résumé :
        //     Use this method to invoke actions on the application's startup thread
        //
        // Paramètres :
        //   action:
        //     The action to invoke
        //
        //   state:
        //     Optional state data to pass to the Action
        void InvokeOnMainThread(Action<object?> action, object? state = null);
        */
        //
        // Résumé :
        //     Called when the application is being brought up.
        Task Initialize();

        //
        // Résumé :
        //     The core of the app's work and logic
        Task Run();
        /*
        //
        // Résumé :
        //     Called if the app is being brought down.
        Task OnShutdown();

        //
        // Résumé :
        //     Called if a failure occurred while running the app
        Task OnError(Exception e);

        //
        // Résumé :
        //     Called when the application is about to update itself.
        void OnUpdate(Version newVersion, out bool approveUpdate);

        //
        // Résumé :
        //     Called when the application has updated itself.
        void OnUpdateComplete(Version oldVersion, out bool rollbackUpdate);
        */
    }
}
