namespace RabbitMq.Library.QuickStart;

/// <summary>
///     Configuration options for RabbitMQ connection and reconnection behavior.
/// </summary>
public class RabbitMqOptions
{
    /// <summary>
    ///     Gets or sets the RabbitMQ server hostname. Defaults to "localhost".
    /// </summary>
    public string HostName { get; set; } = "localhost";

    /// <summary>
    ///     Gets or sets the RabbitMQ server port. Defaults to 5672.
    /// </summary>
    public int Port { get; set; } = 5672;

    /// <summary>
    ///     Gets or sets the username for RabbitMQ authentication. Defaults to "guest".
    /// </summary>
    public string UserName { get; set; } = "guest";

    /// <summary>
    ///     Gets or sets the password for RabbitMQ authentication. Defaults to "guest".
    /// </summary>
    public string Password { get; set; } = "guest";

    /// <summary>
    ///     Gets or sets the RabbitMQ virtual host. Defaults to "/".
    /// </summary>
    public string VirtualHost { get; set; } = "/";

    /// <summary>
    ///     Gets or sets the requested heartbeat interval for the connection.
    ///     A value of <see cref="TimeSpan.Zero"/> disables heartbeats. Defaults to 30 seconds.
    /// </summary>
    public TimeSpan RequestedHeartbeat { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    ///     Gets or sets a value indicating whether automatic connection recovery is enabled.
    ///     When enabled, the RabbitMQ client will attempt to recover connections and channels automatically.
    ///     Defaults to true.
    /// </summary>
    public bool AutomaticRecoveryEnabled { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether topology recovery is enabled.
    ///     When enabled, exchanges, queues, bindings, and consumers are recovered after a connection recovery.
    ///     Only effective when <see cref="AutomaticRecoveryEnabled"/> is true. Defaults to true.
    /// </summary>
    public bool TopologyRecoveryEnabled { get; set; } = true;

    /// <summary>
    ///     Gets or sets the interval between automatic network recovery attempts.
    ///     Only effective when <see cref="AutomaticRecoveryEnabled"/> is true. Defaults to 5 seconds.
    /// </summary>
    public TimeSpan NetworkRecoveryInterval { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    ///     Gets or sets the maximum number of manual reconnection attempts when automatic recovery is disabled
    ///     or has failed. Set to 0 or a negative value for unlimited retry attempts. Defaults to 10.
    /// </summary>
    public int MaxReconnectAttempts { get; set; } = 10;

    /// <summary>
    ///     Gets or sets the base delay in seconds for exponential backoff between reconnection attempts.
    ///     The actual delay is calculated as min(BackoffBaseSeconds ^ attempt, MaxBackoffDelaySeconds).
    ///     Defaults to 2 seconds.
    /// </summary>
    public int BackoffBaseSeconds { get; set; } = 2;

    /// <summary>
    ///     Gets or sets the maximum delay in seconds between reconnection attempts, capping the exponential backoff.
    ///     Defaults to 30 seconds.
    /// </summary>
    public int MaxBackoffDelaySeconds { get; set; } = 30;
}
