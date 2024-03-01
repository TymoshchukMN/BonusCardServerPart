//////////////////////////////////////////////////
// Author : Tymoshchuk Maksym
// Created On : 26/01/202
// Last Modified On :
// Description: Класс карты
// Project: CardsHandler
//////////////////////////////////////////////////

using System;

namespace CardsHandlerServerPart
{
    public class Card
    {
        private const int DefaultSumm = 1000;

        /// <summary>
        /// Номер карты.
        /// </summary>
        private int _number;

        /// <summary>
        /// Номер телефона.
        /// </summary>
        private string _phoneNumber;

        /// <summary>
        /// Срок действия карты.
        /// </summary>
        private DateTime _expirationDate;

        /// <summary>
        /// Баланс на карте.
        /// </summary>
        private int _ballance;

        /// <summary>
        /// Имя владельца карты.
        /// </summary>
        private string _ownerFirstName;

        /// <summary>
        /// Отчество владельца карты.
        /// </summary>
        private string _ownerMiddleName;

        /// <summary>
        /// Фамилия владельца карты.
        /// </summary>
        private string _ownerLastName;

        private bool _isActive;

        #region CTORs

        /// <summary>
        /// Initializes a new instance of the <see cref="Card"/> class.
        /// Def ctor.
        /// </summary>
        public Card()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Card"/> class.
        /// Конструктор, для получения объекта из БД.
        /// </summary>
        /// <param name="number">номер карты.</param>
        /// <param name="phone">номер телефона.</param>
        /// <param name="date">Дата истечения срока карты.</param>
        /// <param name="firstName">Имя.</param>
        /// <param name="middleName">отчество.</param>
        /// <param name="lasName">Фамилия.</param>
        /// <param name="date">Дата истечения срока.</param>
        /// <param name="balance">Баланс.</param>
        public Card(
             int number,
             string phone,
             string firstName,
             string middleName,
             string lasName,
             DateTime date,
             int balance)
        {
            _number = number;
            _phoneNumber = phone;
            _expirationDate = date;
            _ballance = DefaultSumm;
            _ownerFirstName = firstName;
            _ownerMiddleName = middleName;
            _ownerLastName = lasName;
            _ballance = balance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Card"/> class.
        /// Конструктор, для создания карыт и помещения в БД.
        /// </summary>
        /// <param name="number">номер карты.</param>
        /// <param name="phone">номер телефона.</param>
        /// <param name="firstName">имя клиента.</param>
        /// <param name="middleName">отчетство клиента.</param>
        /// <param name="lasName">фамилия клиента.</param>
        public Card(
            int number,
            string phone,
            string firstName,
            string middleName,
            string lasName,
            bool isActive)
        {
            _number = number;
            _phoneNumber = phone;
            _expirationDate = DateTime.Today;
            _ballance = DefaultSumm;
            _ownerFirstName = firstName;
            _ownerMiddleName = middleName;
            _ownerLastName = lasName;
            _isActive = isActive;
        }

        public Card(
            int cardnumber,
            DateTime expirationDate,
            int ballance,
            string firstName,
            string middleName,
            string lastName,
            string phoneNumber)
        {
            _number = cardnumber;
            _expirationDate = expirationDate;
            _ballance = ballance;
            _ownerFirstName = firstName;
            _ownerMiddleName = middleName;
            _ownerLastName = lastName;
            _phoneNumber = phoneNumber;
        }

        public Card(
            int cardnumber,
            DateTime expirationDate,
            int ballance,
            string firstName,
            string middleName,
            string lastName,
            string phoneNumber,
            bool isActive)
        {
            _number = cardnumber;
            _expirationDate = expirationDate;
            _ballance = ballance;
            _ownerFirstName = firstName;
            _ownerMiddleName = middleName;
            _ownerLastName = lastName;
            _phoneNumber = phoneNumber;
            _isActive = isActive;
        }

        #endregion CTORs
        public int Cardnumber
        {
            get { return _number; }
            set { _number = value; }
        }

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value; }
        }

        public DateTime ExpirationDate
        {
            get { return _expirationDate; }
            set { _expirationDate = value; }
        }

        public int Ballance
        {
            get { return _ballance; }
            set { _ballance = value; }
        }

        public string FirstName
        {
            get { return _ownerFirstName; }
            set { _ownerFirstName = value; }
        }

        public string MiddleName
        {
            get { return _ownerMiddleName; }
            set { _ownerMiddleName = value; }
        }

        public string LastName
        {
            get { return _ownerLastName; }
            set { _ownerLastName = value; }
        }

        public bool ActivityFlag
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

    }
}
