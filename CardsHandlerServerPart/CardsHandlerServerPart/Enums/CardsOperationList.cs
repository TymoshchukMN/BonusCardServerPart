namespace CardsHandlerServerPart.Enums
{
    public enum CardsOperationList
    {
        /// <summary>
        /// Значение по умолчанию.
        /// </summary>
        None,

        /// <summary>
        /// Операция создания карты.
        /// </summary>
        Create,

        /// <summary>
        /// Поиск карты.
        /// </summary>
        Find,

        /// <summary>
        /// Списание с карты.
        /// </summary>
        Change,

        /// <summary>
        /// Просмотр баланса на карте.
        /// </summary>
        SeeBalance,

        /// <summary>
        /// Получить все карты в базе.
        /// </summary>
        GetAllCards,
    }
}
