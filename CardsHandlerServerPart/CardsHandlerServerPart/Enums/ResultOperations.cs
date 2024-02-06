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
    internal enum ResultOperations : ushort
    {
        /// <summary>
        /// Значение по умолчанию.
        /// </summary>
        None,

        /// <summary>
        /// Не верный телеон.
        /// </summary>
        WrongPhone,

        /// <summary>
        /// Телефон не существует.
        /// </summary>
        PhoneDoesnEsixt,

        /// <summary>
        /// не верный номер карты.
        /// </summary>
        WrongCard,

        /// <summary>
        /// Карта не существует.
        /// </summary>
        CardDoesnExist,

        /// <summary>
        /// Не верное имя.
        /// </summary>
        WrongName,

        /// <summary>
        /// Данные не заполнены.
        /// </summary>
        EmptyField,

        /// <summary>
        /// Не верная сумма.
        /// </summary>
        WrongSumm,

        /// <summary>
        /// Ошибка списания.
        /// </summary>
        ChargeError,

        /// <summary>
        /// Отрицательное число.
        /// </summary>
        NegativeDigit,

        /// <summary>
        /// Срок действия карты истек.
        /// </summary>
        CardExpired,

        /// <summary>
        /// Не выбрано что сделать.
        /// </summary>
        NotChangedWhatToDo,

        /// <summary>
        /// Не удалось подключиться к БД.
        /// </summary>
        CannontConnectToDB,
    }
}
