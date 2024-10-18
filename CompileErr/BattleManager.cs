
namespace BackYard
{
    public class BattleManager //: IBattleManager
    {
    }
    public class CardPile : ICardPile
    {
        public List<ICard> CardList { get; private set; } = new List<ICard>();
        public ICardPile Copy()
        {
            CardPile result = new CardPile();
            foreach (ICard card in CardList)
            {
                result.CardList.Add(card.CopyCard());
            }
            return result;
        }
    }
}
