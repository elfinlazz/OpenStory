using OpenStory.Common.IO;
using OpenStory.Server.Processing;

namespace OpenStory.Server.Channel
{
    internal sealed class ChannelClient : ClientBase
    {
        private readonly IChannelServer server;

        public ChannelClient(IChannelServer server, IServerSession session)
            : base(server, session)
        {
            this.server = server;
        }

        protected override void ProcessPacket(PacketProcessingEventArgs args)
        {
            // TODO packet handling
        }
    }
}