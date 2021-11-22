using System;
using System.Collections.Generic;
namespace TouhouMachineLearningSummary.Model
{
    [Serializable]
    public class CardDeck
    {
        public string DeckName { get; set; }
        public int LeaderId { get; set; }
        public List<int> CardIds { get; set; }
        public CardDeck(string DeckName, int LeaderId, List<int> CardIds)
        {
            this.DeckName = DeckName;
            this.LeaderId = LeaderId;
            this.CardIds = CardIds;
        }
    }
}
