using System.Collections.Concurrent;
using GptInvoke.Contracts;
using GptInvoke.Helper;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OneOf;

namespace GptInvoke
{
    internal sealed class AIActionInvoker: IAIActionInvoker
    {
        private readonly IAIHandler? _iaiHandler;
        private readonly AIActionInvokeSettings _settings;
        private readonly IServiceProvider _serviceProvider;
        private BlockingCollection<AIMessage> _history = new();
        private const string gtpExplain = @"""
I give you a user prompt and a List of services with information what they can handle and what parameter they need. 
If you think the prompt can handled by a service you need to ensure that you have all required parameters for this service filled out by the user. If parameters are missing and required ask the user until you have all required parameters. And please try to convert the Parameter to the required type. Afterwards just answer with a json string in this format
{ ""Service"" : ""ServiceName"", ""Type"": ""ServiceType"", ""Parameters"": [{ ""Key"": ""ParameterName"", ""Value"": ""ParameterValue""}, {... }]}
If you think you need more values for parameters try to ask until you have all and then respond in described JSON. Please never respond the Json util you have all required parameters filled out. And if you have all parameters only respond the json and nothing else.
Also, if you think that no service can handle the prompt, just answer as you would normally answer the prompt as an initial question. And please answer in the same language as the prompt.
But never tell the user that you maybe could not find a service or anything about registered services. Also dont ask for the service to use. You need to find out. 
""";

        private const string gptPrompt = @$"{gtpExplain}" +
                                         "\r\nThis is the prompt: \"${prompt}\"" +
                                         "\r\nServices: ${services}";

        public AIActionInvoker(
            AIActionInvokeSettings settings,
            IServiceProvider serviceProvider
        )
        {
            _settings = settings;
            _serviceProvider = serviceProvider;
            _iaiHandler = _serviceProvider.GetService<IAIHandler>();
        }

        public Task ClearHistoryAsync()
        {
            while (_history.TryTake(out _)) { }
            return Task.CompletedTask;
        }

        public IEnumerable<AIMessage> History => _history;

        public void SetHistory(IEnumerable<AIMessage> history)
        {
            var messages = history.ToArray();
            bool containsSystemRole = messages.Any(m => m.Role == "system");

            ConcurrentQueue<AIMessage> newHistoryQueue = new ConcurrentQueue<AIMessage>();

            if (!containsSystemRole)
            {
                var systemPrompt = CreateSystemPrompt(messages.FirstOrDefault()?.Content);
                if (systemPrompt != null)
                    newHistoryQueue.Enqueue(systemPrompt);
            }

            foreach (var msg in messages)
            {
                newHistoryQueue.Enqueue(msg);
            }

            _history = new BlockingCollection<AIMessage>(newHistoryQueue);
        }

        public async Task<OneOf<string, AIInvokeServiceResult>> PromptAsync(string userCommand,
            Action<string>? resultHandler = null, CancellationToken cancellationToken = default)
        {
            var messages = ChatPrompts(userCommand);
            var request = new AIRequest
            {
                Messages = messages.ToList()
            };

            string responseString = string.Empty;

            if (resultHandler != null)
            {
                await _iaiHandler.StreamCompletionAsync(request, part =>
                {
                    responseString += part;
                    resultHandler(part);
                }, cancellationToken);
            }
            else
            {
                responseString = await _iaiHandler.GetCompletionAsync(request, cancellationToken);
            }

            var result = await CheckResultAsync(responseString);
            if (result != null)
            {
                if (_settings.HistoryClearBehaviour == ChatHistoryClearBehaviour.OnInvoke
                    || (_settings.HistoryClearBehaviour == ChatHistoryClearBehaviour.OnSuccessfulInvoke && result.Successful)
                    || (_settings.HistoryClearBehaviour == ChatHistoryClearBehaviour.OnFailureInvoke && !result.Successful))
                    await ClearHistoryAsync();
                return result;
            }

            _history.Add(new AIMessage { Role = "assistant", Content = responseString }, cancellationToken);
            if (_settings.HistoryClearBehaviour == ChatHistoryClearBehaviour.OnEveryResponse)
                await ClearHistoryAsync();
            return responseString;
        }

        private async Task<AIInvokeServiceResult?> CheckResultAsync(string gptResponse)
        {
            if (Utils.TryParse<AIInvokeServiceResult>(gptResponse, out var completionResult) || Utils.TryParsePartial<AIInvokeServiceResult>(gptResponse, out completionResult))
            {
                var service = _serviceProvider.GetServices<IAIInvokableService>()
                    .FirstOrDefault(s => s.GetType().FullName == completionResult.Type);
                if (service != null)
                {
                    completionResult.UsedService = service;
                    completionResult.Successful = await service.ExecuteAsync(
                        completionResult.Parameters.ToDictionary(p => p.Key, p => p.Value)
                    );
                }
                return completionResult;
            }
            return null;
        }

        private AIMessage? CreateSystemPrompt(string userCommand)
        {
            var services = _serviceProvider.GetServices<IAIInvokableService>().Select(service => new
            {
                service.Name,
                service.Description,
                Type = service.GetType().FullName,
                service.Parameters
            }).ToArray();
            if (!services.Any() || string.IsNullOrEmpty(userCommand))
                return null;

            var json = JsonConvert.SerializeObject(services);
            var prompt = gptPrompt.Replace("${prompt}", userCommand).Replace("${services}", json);
            return new AIMessage { Role = "system", Content = prompt };
        }

        private List<AIMessage> ChatPrompts(string userCommand)
        {
            var systemPrompt = CreateSystemPrompt(userCommand);

            if (_settings.HistoryAddBehaviour == HistoryAddBehaviour.UserAlways)
            {
                if (_history.Count <= 0 && systemPrompt != null)
                    _history.Add(systemPrompt);
                _history.Add(new AIMessage { Role = "user", Content = userCommand });
            }
            else
            {
                if (_history.Count <= 0 && systemPrompt != null)
                    _history.Add(systemPrompt);
                else
                    _history.Add(new AIMessage { Role = "user", Content = userCommand });
            }

            return _history.ToList();
        }
    }
}
