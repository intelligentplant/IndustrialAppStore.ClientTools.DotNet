# IntelligentPlant.IndustrialAppStore.SemanticKernel

This package contains extensions for the .NET version of [Semantic Kernel](https://learn.microsoft.com/en-us/semantic-kernel/overview/) to allow tool-trained AI models to query the Industrial App Store.

> [!WARNING]
> This package is experimental and is intended for building proof of concept AI applications. Features may change or be removed in future releases.


# Getting Started

You can register core Semantic Kernel services via the `IIndustrialAppStoreBuilder` type:

```csharp
// You must explicitly suppress the experimental warning to acknowledge 
// that you understand the limits of using this package.
#pragma warning disable IASSK0001

builder.Services.AddIndustrialAppStoreApiServices()
    .AddSemanticKernelServices();
```

This will register a scoped `Kernel` service that can be used to interact with AI pipelines. The kernel is pre-configured with plugins that allow AI tools such as tool-trained large language models (LLMs) to query the Industrial App Store Data API.

> [!NOTE]
> You must register Semantic Kernel AI services such as `IChatCompletionService` yourself. This package does not include any specific AI services, as the choice of AI service is dependent on your application and the models you wish to use.


# Using a Chat Service

Refer to the [Microsoft documentation](https://learn.microsoft.com/en-us/semantic-kernel/concepts/ai-services/chat-completion) about how to register a chat service with the Semantic Kernel.

Once you have registered your chat service and have configured the Industrial App Store API services so that the client can obtain a valid access token, you can use your `Kernel` service to start a chat session. For example:

```csharp
var kernel = scope.ServiceProvider.GetRequiredService<Kernel>();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

#pragma warning disable IASSK0001

// Create a new chat history and add a system prompt that helps the AI 
// understand how to use the Industrial App Store plugins.
var chatHistory = new ChatHistory()
    .AddIndustrialAppStoreSystemPrompt();

chatHistory.AddUserMessage("List my Industrial App Store data sources.");

var response = chatCompletionService.GetStreamingChatMessageContentsAsync(
    chatHistory: chatHistory,
    kernel: kernel,
    cancellationToken: cancellationToken);

await foreach (var chunk in response) {
    Console.Write(chunk.Content ?? Environment.NewLine);
}
```


## Managing Chat History

When you use a `ChatHistory` object with an `IChatCompletionService` it is **your responsibility** to add the messages returned by the assistant to the history, so that they can be included in the context for subsequent requests. You must also ensure that the context does not grow too large. Semantic Kernel provides services that can perform [chat history reduction](https://learn.microsoft.com/en-us/semantic-kernel/concepts/ai-services/chat-completion/chat-history#chat-history-reduction) to truncate or summarise earlier messages in the chat history to ensure that the context remains manageable.


## Working with Reasoning Models

Tool-trained reasoning models such as [QwQ](https://huggingface.co/Qwen/QwQ-32B) are capable of planning and executing a sequence of steps to achieve a goal. For example, you can ask the model questions such as *"What is the current temperature of Sensor-1 on my Edge Historian?"* and it will use multiple tool calls to retrieve available data sources, find the correct tag name, and then request the current value of the tag.

> [!TIP]
> When maintaining a `ChatHistory` when working with a reasoning model, you usually need to filter out the part of the response that contains the reasoning steps to ensure that the context for subsequent messages does not become unnecessarily large. Identifying reasoning steps may vary depending on the model that you are using. In the case of QwQ, the reasoning steps are usually encloded in `<think>...</think>` tags, so any part of the response inside these tags should be removed prior to adding the assistant response to the `ChatHistory`.


# Additional Notes

When registering Industrial App Store-related services, most services are registered with a scoped lifetime (for example, to allow the IAS API client to use an access token associated with the calling user in an ASP.NET Core application). As a result, some of the Semantic Kernel-related services registered by this package (including the `Kernel` service itself) are also registered with a scoped lifetime.

When using the `Kernel` service, ensure that you create a new scope for the session if one has not already been created. This is typically only required in non-ASP.NET Core applications, such as console applications or background services.
