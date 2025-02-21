namespace Domain.Primitives;

/// <summary>
///     Статус тикета
/// </summary>
public enum RequestStatus
{
    /// <summary>
    ///     Открыт
    /// </summary>
    Open = 0,

    /// <summary>
    ///     В процессе
    /// </summary>
    InProgress = 1,

    /// <summary>
    ///     Закрыт
    /// </summary>
    Closed = 2
}