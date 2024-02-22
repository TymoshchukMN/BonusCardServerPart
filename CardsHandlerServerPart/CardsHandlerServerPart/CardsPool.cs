using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsHandlerServerPart
{
    /// <summary>
    /// Класс для обработки пула карт.
    /// </summary>
    public class CardsPool
    {
        #region FIELDS

        /// <summary>
        /// Размер пула.
        /// </summary>
        private const int PoolSize = 1000;

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
        private ConcurrentQueue<int> _poolCard = new ConcurrentQueue<int>();

        /// <summary>
        /// Флаг свободен ли обработчик пула.
        /// </summary>
        private bool _isBusy;

        #endregion FIELDS

        private CardsPool()
        {
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

        public ConcurrentQueue<int> GetPoolCard
        {
            get
            {
                return _poolCard;
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

        public void FillPool(int startVol)
        {
            if (startVol == 0)
            {
                startVol = PoolSize;
            }
            else
            {
                for (int i = startVol + 1; i < PoolSize + startVol; i++)
                {
                    _poolCard.Enqueue(i);
                }
            }
        }

        /// <summary>
        /// Проверка размера пула. Если пулл мельньше минимального,
        /// вызываем метод расширения.
        /// </returns>
        public async void CheckSizePool()
        {
            if (_poolCard.Count <= MinPookSize)
            {
                await Expandpool(_poolCard);
            }
        }

        /// <summary>
        /// Расширение размера пула.
        /// </summary>
        /// <param name="poolCard">
        /// Пул карт для расширения.
        /// </param>
        /// <returns>Task.</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private static async Task Expandpool(ConcurrentQueue<int> poolCard)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            int firstVolToAdd = poolCard.Last() + 1;
            int currentQueueSize = poolCard.Count;
            int targetQueueSize = poolCard.Count * 2;

            for (int i = currentQueueSize; i < targetQueueSize; i++)
            {
                poolCard.Enqueue(firstVolToAdd);
                ++firstVolToAdd;
            }
        }

        #endregion METHODS
    }
}
