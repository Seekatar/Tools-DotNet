using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Seekatar.Tools
{

    /// <summary>
    /// Extensions for HostBuilder
    /// </summary>
    public static class HostBuilderExtensions
    {
        const string SharedDevSettingsName = "shared.appsettings.Development.json";
        const string SharedDevSettingsConfigName = "sharedDevSettingsPath";

        private static string GetPath(string fileName, IConfiguration? config = null)
        {
            if (config != null && config[SharedDevSettingsConfigName] != null)
            {
                return config[SharedDevSettingsConfigName]!;
            }
            else
            {
                var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Directory.GetCurrentDirectory();
                while (dir != null)
                {
                    var path = Path.Join(dir, fileName);
                    if (File.Exists(path))
                    {
                        return path;
                    }

                    dir = Path.GetDirectoryName(dir);
                }

                Console.WriteLine($"{nameof(InsertSharedDevSettings)} called, but couldn't find '{fileName}'");
                Debug.WriteLine($"{nameof(InsertSharedDevSettings)} called, but couldn't find '{fileName}'");

                return "";
            }
        }

        /// <summary>
        /// Get the path of a shared settings file by looking up until it finds it
        /// </summary>
        /// <param name="fileName">Name of file to find</param>
        /// <returns>fully qualified name or null</returns>
        public static string GetSharedSettingsPath(string fileName = SharedDevSettingsName)
        {
            return GetPath(fileName);
        }

        /// <summary>
        /// Add a shared dev settings file for dev builds for console apps
        /// </summary>
        /// <param name="builder">ConfigurationBuilder</param>
        /// <param name="reloadOnChange">switch to set for reload on change</param>
        /// <param name="fileName">override default name of shared_appsettings.Development.json</param>
        /// <example>
        /// <code>
        /// var configuration = new ConfigurationBuilder()
        ///                        .AddSharedDevSettings()
        ///                        .AddJsonFile("appsettings.json", true, true)
        ///                        .AddEnvironmentVariables()
        ///                        .Build();
        /// </code>
        /// </example>
        /// <returns>builder</returns>
        public static IConfigurationBuilder AddSharedDevSettings(this IConfigurationBuilder builder,
                                                                 bool reloadOnChange = false,
                                                                 string? fileName = null)
        {
            if (String.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase) ||
                String.Equals(Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase) ||
                String.Equals(Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(fileName)) { fileName = SharedDevSettingsName; }

                var path = GetPath(fileName);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    builder.AddJsonFile(path, optional: false, reloadOnChange);
                }
            }
            return builder;
        }

        /// <summary>
        /// Add a shared dev settings file for dev builds for non-minimal web apps
        /// </summary>
        /// <param name="hostBuilder">host builder</param>
        /// <param name="reloadOnChange">switch to set for reload on change</param>
        /// <param name="fileName">override default name of shared_appsettings.Development.json</param>
        /// <remarks>
        /// Call in program.cs to build IConfiguration with this. Add if first to be lowest priority.
        /// </remarks>
        /// <returns>builder</returns>
        public static IHostBuilder InsertSharedDevSettings(this IHostBuilder hostBuilder, bool reloadOnChange = false, string? fileName = null)
        {
            if (hostBuilder == null) { throw new ArgumentNullException(nameof(hostBuilder)); }

            hostBuilder.ConfigureAppConfiguration((hostingContext, builder) =>
            {
                InsertSharedDevSettings(builder, reloadOnChange, fileName);
            });
            return hostBuilder;
        }

        /// <summary>
        /// Add a shared dev settings file for dev builds for console or minimal API apps
        /// </summary>
        /// <param name="builder">ConfigurationBuilder</param>
        /// <param name="reloadOnChange">switch to set for reload on change</param>
        /// <param name="fileName">override default name of shared_appsettings.Development.json</param>
        /// <example>
        /// <code>
        /// var configuration = new ConfigurationBuilder()
        ///                        .AddSharedDevSettings()
        ///                        .AddJsonFile("appsettings.json", true, true)
        ///                        .AddEnvironmentVariables()
        ///                        .Build();
        /// </code>
        /// </example>
        /// <returns>builder</returns>
        public static IConfigurationBuilder InsertSharedDevSettings(this IConfigurationBuilder builder, bool reloadOnChange = false, string? fileName = null)
        {
            if (string.IsNullOrWhiteSpace(fileName)) { fileName = SharedDevSettingsName; }

            var config = builder.Build();

            if (String.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase) ||
                String.Equals(Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase) ||
                String.Equals(Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase))
            {
                // set as low priority
                // very roughly based on https://jumpforjoysoftware.com/2018/09/aspnet-core-shared-settings/
                // ResolveFileProvider was the trick

                var path = GetPath(fileName, config);

                if (File.Exists(path))
                {
                    var sharedConfig = new JsonConfigurationSource
                    {
                        Path = path,
                        Optional = true,
                        ReloadOnChange = reloadOnChange
                    };
                    sharedConfig.ResolveFileProvider();
                    builder.Sources.Insert(0, sharedConfig);
                }
            }
            return builder;
        }
    }

}