namespace Interfaces.Notification;

/// <summary>
/// Generic interface for sending notifications
/// </summary>
/// <typeparam name="T">The type of message to send</typeparam>
public interface INotification<T> where T : class
{
    /// <summary>
    /// Sends a notification asynchronously
    /// </summary>
    /// <param name="message">The message to send</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task SendAsync(T message);
}