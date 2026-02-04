using Bravia.Abstractions;

namespace Bravia;

public interface IAVContentRequestFactory
{
    BraviaHttpRequest GetPlayingContentInfoRequest();
    BraviaHttpRequest SetPlayContentRequest(SetPlayContentDto content);
}