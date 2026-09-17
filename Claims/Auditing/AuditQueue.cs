using System.Threading.Channels;

namespace Claims.Auditing
{
    /// <summary>
    /// In-memory queue of pending audit writes. Registered as a singleton so
    /// <see cref="AuditService"/> (scoped, one per request) and <see cref="AuditBackgroundWorker"/>
    /// (a singleton that runs for the app's lifetime) share the same channel.
    /// </summary>
    public class AuditQueue
    {
        private readonly Channel<AuditEntry> _channel = Channel.CreateUnbounded<AuditEntry>();

        public ChannelReader<AuditEntry> Reader => _channel.Reader;

        public ValueTask EnqueueAsync(AuditEntry entry) => _channel.Writer.WriteAsync(entry);
    }
}
