using OpenAI.Models;

namespace GptInvoke.Contracts;

public class GptActionInvokeSettings
{

    /// <summary>
    /// Model to use. If null GPT-3.5 turbo will be used.
    /// Strongly recommended is GPT-4 to prevent errors or miss understanding. But because GPT-4 is not Available for all API users the default is 3.5
    /// </summary>
    public Model? Model { get; set; }

    /// <summary>
    /// Your open ai api key 
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// Set this to false If you prefer to call possible service manually
    /// </summary>
    public bool AutoInvokeFoundService { get; set; } = true;

    public HistoryAddBehaviour HistoryAddBehaviour { get; set; } = HistoryAddBehaviour.Default;

    /// <summary>
    /// Set up when history should automatically cleared. Default is after a successful invocation
    /// </summary>
    public GptHistoryClearBehaviour HistoryClearBehaviour { get; set; } = GptHistoryClearBehaviour.OnSuccessfulInvoke;
}

public enum GptHistoryClearBehaviour
{
    Never,
    OnInvoke,
    OnSuccessfulInvoke,
    OnFailureInvoke,
    OnEveryResponse,
}

public enum HistoryAddBehaviour
{
    Default,
    UserAlways,
}