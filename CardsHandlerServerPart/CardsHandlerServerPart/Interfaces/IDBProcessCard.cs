using System.Data;

namespace CardsHandlerServerPart.Interfaces
{
    public interface IDBProcessCard
    {
        DataTable CreateCard(Card card);

        ResultOperations FindCardByPhone(out DataTable dataTable, string number);

        ResultOperations FindCardByCardNum(out DataTable dataTable, int number);

        ResultOperations AddBonus(out DataTable card, int cardNum, int summ);

        ResultOperations Charge(out DataTable card, int cardNum, int summ);

        DataTable GetAllCards();
    }
}
