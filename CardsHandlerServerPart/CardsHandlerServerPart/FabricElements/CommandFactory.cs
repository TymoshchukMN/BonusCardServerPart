using CardsHandlerServerPart.Enums;
using CardsHandlerServerPart.Interfaces;

namespace CardsHandlerServerPart.FabricElements
{
    /// <summary>
    /// Класс для возвражения объекта в соостветствии с типом операции.
    /// </summary>
    internal class CommandFactory
    {
        public static IProcessCard GetCommand(CardsOperationList operation)
        {
            switch (operation)
            {
                case CardsOperationList.Create:

                    return new CreateCard();

                case CardsOperationList.Find:

                    return new FindCard();

                case CardsOperationList.Change:

                    return new ChangeCard();

                case CardsOperationList.SeeBalance:

                    return new GetBalanceCard();

                case CardsOperationList.GetAllCards:

                    return new GetAllCards();
            }

            return null;
        }
    }
}
