﻿using Microsoft.AspNetCore.Components.Web;

namespace AzRefArc.AspNetBlazorUnited.Client
{
    public static class CustomRenderingMode
    {
        /// <summary>
        /// Gets an <see cref="IComponentRenderMode"/> that represents rendering interactively on the server via Blazor Server hosting
        /// with server-side prerendering.
        /// </summary>
        public static InteractiveServerRenderMode InteractiveServer { get; } = new();

        /// <summary>
        /// Gets an <see cref="IComponentRenderMode"/> that represents rendering interactively on the client via Blazor WebAssembly hosting
        /// with server-side prerendering.
        /// </summary>
        public static InteractiveWebAssemblyRenderMode InteractiveWebAssembly { get; } = new();

        /// <summary>
        /// Gets an <see cref="IComponentRenderMode"/> that means the render mode will be determined automatically based on a policy.
        /// </summary>
        public static InteractiveAutoRenderMode InteractiveAuto { get; } = new();

        /// <summary>
        /// Gets an <see cref="IComponentRenderMode"/> that represents rendering interactively on the server via Blazor Server hosting
        /// without server-side prerendering.
        /// </summary>
        public static InteractiveServerRenderMode InteractiveServerWithoutPrerendering { get; } = new(prerender: false);
        /// <summary>
        /// Gets an <see cref="IComponentRenderMode"/> that represents rendering interactively on the client via Blazor WebAssembly hosting
        /// without server-side prerendering.
        /// </summary>
        public static InteractiveWebAssemblyRenderMode InteractiveWebAssemblyWithoutPrerendering { get; } = new(prerender: false);

        /// <summary>
        /// Gets an <see cref="IComponentRenderMode"/> that means the render mode will be determined automatically based on a policy without server-side prerendering.
        /// </summary>
        public static InteractiveAutoRenderMode InteractiveAutoWithoutPrerendering { get; } = new(prerender: false);

    }
}
