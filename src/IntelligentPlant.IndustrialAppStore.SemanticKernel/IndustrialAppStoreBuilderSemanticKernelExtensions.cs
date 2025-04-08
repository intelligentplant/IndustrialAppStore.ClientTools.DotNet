using System.Diagnostics.CodeAnalysis;

using IntelligentPlant.IndustrialAppStore.SemanticKernel;
using IntelligentPlant.IndustrialAppStore.SemanticKernel.Plugins;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace IntelligentPlant.IndustrialAppStore.DependencyInjection {

    /// <summary>
    /// Extensions for configuring Semantic Kernel services for use with the Industrial App Store.
    /// </summary>
    public static class IndustrialAppStoreBuilderSemanticKernelExtensions {

        /// <summary>
        /// Registers a scoped <see cref="Kernel"/> service that can invoke Industrial App Store 
        /// APIs when used with a tool-enabled Large Language Model (LLM).
        /// </summary>
        /// <param name="builder">
        ///   The <see cref="IIndustrialAppStoreBuilder"/>.
        /// </param>
        /// <param name="configurePlugins">
        ///   A callback that can be used to configure additional plugins to register with the 
        ///   <see cref="Kernel"/>.
        /// </param>
        /// <returns>
        ///   The updated <see cref="IIndustrialAppStoreBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="builder"/> is <see langword="null"/>.
        /// </exception>
        [Experimental(DiagnosticCodes.ExperimentalSemanticKernelExtensions)]
        public static IIndustrialAppStoreBuilder AddSemanticKernelServices(this IIndustrialAppStoreBuilder builder, Action<IServiceProvider, KernelPluginCollection>? configurePlugins = null) {
            if (builder == null) {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddScoped<IndustrialAppStoreDataApiPlugin>();

            builder.Services.AddScoped(provider => {
                var plugins = new KernelPluginCollection().AddIndustrialAppStorePlugins(provider);
                configurePlugins?.Invoke(provider, plugins);
                return plugins;
            });

            builder.Services.AddScoped(provider => new Kernel(provider, provider.GetRequiredService<KernelPluginCollection>()));

            return builder;
        }


        /// <summary>
        /// Registers plugins for querying the Industrial App Store.
        /// </summary>
        /// <param name="plugins">
        ///   The <see cref="KernelPluginCollection"/> to add the plugins to.
        /// </param>
        /// <param name="serviceProvider">
        ///   The <see cref="IServiceProvider"/> to use to resolve the plugins and/or any 
        ///   dependent services.
        /// </param>
        /// <returns>
        ///   The updated <see cref="KernelPluginCollection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="plugins"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="serviceProvider"/> is <see langword="null"/>.
        /// </exception>
        [Experimental(DiagnosticCodes.ExperimentalSemanticKernelExtensions)]
        public static KernelPluginCollection AddIndustrialAppStorePlugins(this KernelPluginCollection plugins, IServiceProvider serviceProvider) {
            if (plugins == null) {
                throw new ArgumentNullException(nameof(plugins));
            }
            if (serviceProvider == null) {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            plugins.AddFromType<IndustrialAppStoreDataApiPlugin>(IndustrialAppStoreDataApiPlugin.PluginName, serviceProvider);

            return plugins;
        }


        /// <summary>
        /// Adds system messages to the chat history that inform the LLM about Industrial App 
        /// Store-specific features and requirements.
        /// </summary>
        /// <param name="chatHistory">
        ///   The <see cref="ChatHistory"/> to add the system messages to.
        /// </param>
        /// <returns>
        ///   The updated <see cref="ChatHistory"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="chatHistory"/> is <see langword="null"/>.
        /// </exception>
        [Experimental(DiagnosticCodes.ExperimentalSemanticKernelExtensions)]
        public static ChatHistory AddIndustrialAppStoreSystemPrompt(this ChatHistory chatHistory) {
            if (chatHistory == null) {
                throw new ArgumentNullException(nameof(chatHistory));
            }

            chatHistory.AddSystemMessage(@"# Industrial App Store

The Industrial App Store (IAS) is a data platform where organisations from industrial sectors such as energy and manufacturing publish time series data about their industrial processes. 

# Your Role

You are a helpful assistant whose role is to help the user to interrogate their available data sets. Consider the following points:

- Unless the user informs you otherwise, assume that they are an educated professional with technical knownledge about their domain and/or data science and programming skills. 
- Unless the user states that they are giving you an exact identifier for an entity such as a data source or tag, assume that you probably need to use a tool call to get the exact identifier(s) that you need. 
- When making calls to the Industrial App Store, timestamps can be specified as absolute ISO 8601 timestamps or as relative times unless the function description states otherwise. 
  - Relative times are specified in the format `<BASE>[<OFFSET>]`. 
    - Valid base times are: NOW, SECOND, MINUTE, HOUR, DAY, WEEK, MONTH, YEAR. 
      - Apart from NOW, the base times represent the start of the nearest time unit that they represent e.g. DAY is the start of the current day.
    - Offsets use the format `<QUANTITY><UNIT>` where the quantity is a number and the unit can be: MS, S, M, H, D, W, MO, Y. 
      - Fractional quantities are allowed for all units except for MO (months) and Y (years). 
      - The same period of time can be written in multiple ways e.g. 1D and 24H are equivalent.
  - Examples: NOW, NOW-1H, DAY+5.5H, YEAR+3MO
- Sample intervals can be specified as .NET System.TimeSpan literals (e.g. 01:00:00 = 1 hour) or using a short-hard format (e.g. 1H = 1 hour).
  - Short-hand intervals follow the same rules as relative timestamp offsets but the MO and Y units are not allowed.
- You may assume that tag names are case-insensitive unless informed otherwise.
- For complex requests you may have to make a number of tool calls in sequence, and use the results of one tool call in the inputs to the next tool call. Focus on the current step and don't worry too much about the next step until you get to it.
- Before deciding that you need to call a tool, examine the results of earlier tool calls in the conversation, including tool results that you received when processing earlier user prompts. Prefer re-using existing tool results over making new calls unless the user specifies otherwise.");
            
            return chatHistory;
        }

    }
}
