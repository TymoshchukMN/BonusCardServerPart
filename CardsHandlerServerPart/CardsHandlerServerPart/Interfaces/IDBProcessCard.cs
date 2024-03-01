using System.Data;

namespace CardsHandlerServerPart.Interfaces
{
    public interface IDBProcessCard
    {
        // bool CheckIfCardExist(int cardNumber);

        // bool CheckIfPhone(string phoneNumber);

        void CreateCard(ref Card card);

        ResultOperations FindCardByPhone(out Card card, string number);

        ResultOperations FindCardByCardNum(out Card card, int number);

        ResultOperations Charge(out Card card, int cardNum, int summ);

        ResultOperations AddBonus(out Card card, int cardNum, int summ);

        DataTable GetAllCards();

        ResultOperations GetExpiredCards(out DataTable dataTable);
    }
}
