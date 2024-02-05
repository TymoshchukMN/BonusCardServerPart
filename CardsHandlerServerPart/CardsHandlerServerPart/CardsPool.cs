using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsHandlerServerPart
{
    /// <summary>
    /// Класс для обработки пула карт.
    /// </summary>
    internal class CardsPool
    {
        #region FIELDS

        /// <summary>
        /// Размер пула.
        /// </summary>
        private const int PoolSixe = 1000;

        /// <summary>
        /// Минимальный размер пула карт.
        /// </summary>
        private const int MinPookSize = 100;

        /// <summary>
        /// Экземпляр класса CardsPool.
        /// </summary>
        private static CardsPool _instance;

        /// <summary>
        /// Пулл номеро карт для выдачи.
        /// </summary>
        private List<int> _poolCarsNumber = new List<int>() { 0 };

        /// <summary>
        /// Флаг свободен ли обработчик пула.
        /// </summary>
        private bool _isBusy;

        #endregion FIELDS

        private CardsPool()
        {
            _isBusy = false;
        }

        #region PROPERTIES

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            private set
            {
                _isBusy = value;
            }
        }

        #endregion PROPERTIES

        #region METHODS

        /// <summary>
        /// Получить экземпляр объекта CardsPool.
        /// </summary>
        /// <returns>CardsPool.</returns>
        public static CardsPool GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CardsPool();
            }

            return _instance;
        }

        /// <summary>
        /// Заполнить пулл доступных номеров.
        /// </summary>
        /// <param name="startVol">
        /// Первый доступный номер.
        /// </param>
        public void FillPool(int startVol)
        {
            for (int i = startVol; i < PoolSixe + startVol; i++)
            {
                _poolCarsNumber.Add(i);
            }
        }

        /// <summary>
        /// Получить номер карты.
        /// </summary>
        /// <returns>
        /// номер карты.
        /// </returns>
        public int GetCarNumber()
        {
            // указываем, что генератор карт занят.
            IsBusy = true;

            int cardNumber = _poolCarsNumber[0];
            _poolCarsNumber.Remove(cardNumber);

            if (_poolCarsNumber.Count <= MinPookSize)
            {
                int nextVol = _poolCarsNumber.Max() + 1;

                FillPool(nextVol);
            }

            IsBusy = false;

            return cardNumber;
        }

        #endregion METHODS
    }
}
