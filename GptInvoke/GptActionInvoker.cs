using System.Collections.Concurrent;
using GptInvoke.Contracts;
using GptInvoke.Helper;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OneOf;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;

namespace GptInvoke
{
    internal sealed class GptActionInvoker: IGptActionInvoker
    {
        private OpenAIClient Api { get; }
        private readonly GptActionInvokeSettings _settings;
        private readonly IServiceProvider _serviceProvider;
        private readonly BlockingCollection<ChatPrompt> _history = new();

        private const string gtpExplain = @"""
I give you a user prompt and a List of services with information what they can handle and what parameter they need. 
If you think the prompt can handled by a service you need to ensure that you have all required parameters for this service filled out by the user. If parameters are missing ask the user until you have all required parameters.  Afterwards just answer with a json string in this format
{ ""Service"" : ""ServiceName"", ""Type"": ""ServiceType"", ""Parameters"": [{ ""Key"": ""ParameterName"", ""Value"": ""ParameterValue""}, {... }]}
If you think you need more values for parameters try to ask until you have all and then respond in described JSON.  Please never respond the Json util you have all required parameters filled out. And if you have all parameters only respond the json and nothing else.
Also, if you think that no service can handle the prompt, just answer as you would normally answer the prompt as an initial question. 
But never tell the user that you maybe could not find a service or anything about registered services. Also dont ask for the service to use. You need to find out. 
""";

        private const string gptPrompt = @$"{gtpExplain}" +
                                         "\r\nThis is the prompt: \"${prompt}\"" +
                                         "\r\nServices: ${services}";


        public GptActionInvoker(GptActionInvokeSettings settings, IServiceProvider serviceProvider)
        {
            _settings = settings;
            _serviceProvider = serviceProvider;
            Api = new OpenAIClient(settings.ApiKey);
        }
        
        public Task ClearHistoryAsync()
        {
            while(_history.TryTake(out _)){ }
            return Task.CompletedTask;
        }

        public async Task<OneOf<string, GptServiceResult>> PromptAsync(string userCommand)
        {
            var chatPrompts = ChatPrompts(userCommand);
            var chatRequest = new ChatRequest(chatPrompts, _settings.Model ?? Model.GPT3_5_Turbo);
            var response = await Api.ChatEndpoint.GetCompletionAsync(chatRequest);
            var responseString = response.FirstChoice.ToString();

            var result = await CheckResultAsync(responseString);
            if (result != null)
            {
                if (_settings.HistoryClearBehaviour == GptHistoryClearBehaviour.OnInvoke 
                    || (_settings.HistoryClearBehaviour == GptHistoryClearBehaviour.OnSuccessfulInvoke && result.Successful)
                    || (_settings.HistoryClearBehaviour == GptHistoryClearBehaviour.OnFailureInvoke && !result.Successful))
                    await ClearHistoryAsync();
                return result;
            }

            _history.Add(new ChatPrompt("assistant", responseString));
            if (_settings.HistoryClearBehaviour == GptHistoryClearBehaviour.OnEveryResponse)
                await ClearHistoryAsync();
            return responseString;
        }

        private async Task<GptServiceResult?> CheckResultAsync(string gptResponse)
        {
            if (Utils.TryParse<GptServiceResult>(gptResponse, out var completionResult) || Utils.TryParsePartial<GptServiceResult>(gptResponse, out completionResult))
            {
                var service = _serviceProvider.GetServices<IGptInvokableService>().FirstOrDefault(s => s.GetType().FullName == completionResult.Type);
                if (service != null)
                {
                    completionResult.UsedService = service;
                    completionResult.Successful = await service.ExecuteAsync(completionResult.Parameters.ToDictionary(p => p.Key, p => p.Value));
                }
                return completionResult;
            }
            return null;
        }

        private List<ChatPrompt> ChatPrompts(string userCommand)
        {
            var services = _serviceProvider.GetServices<IGptInvokableService>().Select(service => new
            {
                service.Name,
                service.Description,
                Type = service.GetType().FullName,
                service.Parameters
            }).ToArray();
            var json = JsonConvert.SerializeObject(services);
            var prompt = gptPrompt.Replace("${prompt}", userCommand).Replace("${services}", json);

            var chatPrompts = _history?.ToList() ?? new List<ChatPrompt>();
            //chatPrompts.Add(chatPrompts.Count <= 0
            //    ? new ChatPrompt("system", prompt)
            //    : new ChatPrompt("user", userCommand));

            chatPrompts.Add(new ChatPrompt("system", prompt));
            chatPrompts.Add(new ChatPrompt("user", userCommand));
            
            return chatPrompts;
        }
    }
}