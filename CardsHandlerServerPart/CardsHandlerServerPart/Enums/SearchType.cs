///////////////////////////////
// Author : Tymoshchuk Maksym
// Created On : 26/01/202
// Last Modified On :
// Description: Типы поиска карты
// Project: CardsHandler
//////////////////////////////

namespace CardsHandlerServerPart.Enums
{
    public enum SearchType
    {
        /// <summary>
        /// Значение по умолчанию.
        /// </summary>
        None,

        /// <summary>
        /// Тип поиска по телефону.
        /// </summary>
        ByPhone,

        /// <summary>
        /// Тип поска по номеру карты.
        /// </summary>
        ByCard,
    }
}
