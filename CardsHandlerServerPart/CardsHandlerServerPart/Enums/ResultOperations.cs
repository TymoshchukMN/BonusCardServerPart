///////////////////////////////////////////////////
// Author : Tymoshchuk Maksym
// Created On : 26/01/202
// Last Modified On :
// Description: Перечисления с результатом операций.
// Project: CardsHandler
///////////////////////////////////////////////////

namespace CardsHandlerServerPart
{
    /// <summary>
    /// Ошибки.
    /// </summary>
    public enum ResultOperations : ushort
    {
        /// <summary>
        /// Значение по умолчанию.
        /// </summary>
        None = 0,

        /// <summary>
        /// Не верный телеон.
        /// </summary>
        WrongPhone = 1,

        /// <summary>
        /// Телефон не существует.
        /// </summary>
        PhoneDoesnEsixt = 2,

        /// <summary>
        /// не верный номер карты.
        /// </summary>
        WrongCard = 3,

        /// <summary>
        /// Карта не существует.
        /// </summary>
        CardDoesnExist = 4,

        /// <summary>
        /// Не верное имя.
        /// </summary>
        WrongName = 5,

        /// <summary>
        /// Данные не заполнены.
        /// </summary>
        EmptyField = 6,

        /// <summary>
        /// Не верная сумма.
        /// </summary>
        WrongSumm = 7,

        /// <summary>
        /// Ошибка списания.
        /// </summary>
        ChargeError = 8,

        /// <summary>
        /// Отрицательное число.
        /// </summary>
        NegativeDigit = 9,

        /// <summary>
        /// Срок действия карты истек.
        /// </summary>
        CardExpired = 10,

        /// <summary>
        /// Не выбрано что сделать.
        /// </summary>
        NotChangedWhatToDo = 11,

        /// <summary>
        /// Не удалось подключиться к БД.
        /// </summary>
        CannontConnectToDB = 12,
    }
}
