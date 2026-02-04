using Bravia.Abstractions;

namespace Bravia;

internal class AVContentRequestFactory : IAVContentRequestFactory
{
    public BraviaHttpRequest GetPlayingContentInfoRequest()
    {
        return new BraviaHttpRequest
        {
            Id = 103,
            Version = "1.0",
            Method = "getPlayingContentInfo",
            Params = new List<Dictionary<string, object>>()
        };
    }

    public BraviaHttpRequest SetPlayContentRequest(SetPlayContentDto content)
    {
        ArgumentNullException.ThrowIfNull(content, nameof(content));
        
        return new BraviaHttpRequest
        {
            Id = 101,
            Version = "1.0",
            Method = "setPlayContent",
            Params = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object> { { "uri", content.Uri } }
            }
        };
    }
}