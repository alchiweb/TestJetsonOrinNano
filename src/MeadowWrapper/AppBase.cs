using Meadow;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meadow
{
    //
    // Résumé :
    //     Provides a base implementation for the Meadow App. Use this class for Meadow
    //     applications to get strongly-typed access to the current device information.
    public abstract class AppBase : IApp
    {
        /*
        private ExecutionContext executionContext;

        public CancellationToken CancellationToken { get; internal set; }

        public Dictionary<string, string> Settings { get; internal set; }

        //
        // Résumé :
        //     The app cancellation token
        public static CancellationToken Abort { get; protected set; }

        //
        // Résumé :
        //     Base constructor for the App class
        protected AppBase()
        {
            executionContext = Thread.CurrentThread.ExecutionContext;
            Abort = MeadowOS.AppAbort.Token;
            Resolver.Services.Add((IApp)this);
        }

        //
        // Résumé :
        //     Invokes an action in the context of the applications main thread
        //
        // Paramètres :
        //   action:
        //     The action to call
        //
        //   state:
        //     An optional state object to pass to the Action
        public virtual void InvokeOnMainThread(Action<object?> action, object? state = null)
        {
            action(state);
        }
        */
        //
        // Résumé :
        //     Called by MeadowOS when everything is ready for the App to run
        public virtual Task Run()
        {
            return Task.CompletedTask;
        }

        //
        // Résumé :
        //     Called by MeadowOS to initialize the App
        public virtual Task Initialize()
        {
            return Task.CompletedTask;
        }
        /*
        //
        // Résumé :
        //     Called when a request to shut down the App occurs
        //
        // Remarques :
        //     This is called by the Update Service before applying an update
        public virtual Task OnShutdown()
        {
            return Task.CompletedTask;
        }

        //
        // Résumé :
        //     Called when the MeadowOS encounters an error
        //
        // Paramètres :
        //   e:
        //     The exception from MeadowOS
        public virtual Task OnError(Exception e)
        {
            return Task.CompletedTask;
        }

        //
        // Résumé :
        //     Called when the application is about to update itself.
        public void OnUpdate(Version newVersion, out bool approveUpdate)
        {
            approveUpdate = true;
        }

        //
        // Résumé :
        //     Called when the application has updated itself.
        public void OnUpdateComplete(Version oldVersion, out bool rollbackUpdate)
        {
            rollbackUpdate = false;
        }

        //
        // Résumé :
        //     Virtual method provided for App implementations to clean up resources on Disposal
        public virtual ValueTask DisposeAsync()
        {
            return new ValueTask(Task.CompletedTask);
        }
        */
    }
}
