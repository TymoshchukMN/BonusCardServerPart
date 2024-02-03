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
        /// <summary>
        /// Размер пула.
        /// </summary>
        private const int PoolSixe = 1000;

        /// <summary>
        /// Минимальный размер пула карт.
        /// </summary>
        private const int MinPookSize = 100;

        private static CardsPool _instance;

        /// <summary>
        /// Пулл номеро карт для выдачи.
        /// </summary>
        private List<int> _poolCarsNumber = new List<int>() { 0 };

        /// <summary>
        /// Флаг свободен ли обработчик пула.
        /// </summary>
        private bool _isBusy = false;

        private CardsPool()
        {
        }

        public bool IsBusy
        {
            get { return _isBusy = false; }
            private set { _isBusy = false; }
        }

        public List<int> PoolCards
        {
            get { return _poolCarsNumber; }
            private set { _poolCarsNumber = value; }
        }

        public static CardsPool GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CardsPool();
            }

            return _instance;
        }

        public void FillPool(int startVol)
        {
            for (int i = startVol; i < PoolSixe + startVol; i++)
            {
                _poolCarsNumber.Add(startVol);
            }
        }

        public int GetCarNumberAsync()
        {
            // указываем, что генератор карт занят.
            _isBusy = true;

            int cardNumber = _poolCarsNumber[0];
            _poolCarsNumber.Remove(cardNumber);

            if (_poolCarsNumber.Count <= MinPookSize)
            {
                int nextVol = _poolCarsNumber.Max() + 1;

                FillPool(nextVol);
            }

            _isBusy = false;

            return cardNumber;
        }
    }
}
