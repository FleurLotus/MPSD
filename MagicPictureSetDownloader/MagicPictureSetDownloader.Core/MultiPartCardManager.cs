namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;
    using MagicPictureSetDownloader.Interface;
    
    public class MultiPartCardManager: IMultiPartCardManager
    {
        private static readonly Lazy<IMultiPartCardManager> _lazy = new Lazy<IMultiPartCardManager>(() => new MultiPartCardManager());
        private readonly Dictionary<string, string> _reverseSideOfFlipLand;

        public static IMultiPartCardManager Instance
        {
            get { return _lazy.Value; }
        }

        private MultiPartCardManager()
        {
            _reverseSideOfFlipLand = new Dictionary<string, string> {
                {"Agadeem, the Undercrypt","Agadeem's Awakening"},
                {"Akoum Teeth","Akoum Warrior"},
                {"Bala Ged Sanctuary","Bala Ged Recovery"},
                {"Beyeen Coast","Beyeen Veil"},
                {"Blackbloom Bog","Blackbloom Rogue"},
                {"Boulderloft Pathway","Branchloft Pathway"},
                {"Emeria, Shattered Skyclave","Emeria's Call"},
                {"Glasspool Shore","Glasspool Mimic"},
                {"Grimclimb Pathway","Brightclimb Pathway"},
                {"Hagra Broodpit","Hagra Mauling"},
                {"Jwari Ruins","Jwari Disruption"},
                {"Kabira Plateau","Kabira Takedown"},
                {"Kazandu Valley","Kazandu Mammoth"},
                {"Kazuul's Cliffs","Kazuul's Fury"},
                {"Khalni Territory","Khalni Ambush"},
                {"Lavaglide Pathway","Riverglide Pathway"},
                {"Makindi Mesas","Makindi Stampede"},
                {"Malakir Mire","Malakir Rebirth"},
                {"Murkwater Pathway","Clearwater Pathway"},
                {"Ondu Skyruins","Ondu Inversion"},
                {"Pelakka Caverns","Pelakka Predation"},
                {"Pillarverge Pathway","Needleverge Pathway"},
                {"Sea Gate, Reborn","Sea Gate Restoration"},
                {"Sejiri Glacier","Sejiri Shelter"},
                {"Shatterskull, the Hammer Pass","Shatterskull Smashing"},
                {"Silundi Isle","Silundi Vision"},
                {"Skyclave Basilica","Skyclave Cleric"},
                {"Song-Mad Ruins","Song-Mad Treachery"},
                {"Spikefield Cave","Spikefield Hazard"},
                {"Tangled Vale","Tangled Florahedron"},
                {"Timbercrown Pathway","Cragcrown Pathway"},
                {"Turntimber, Serpentine Wood","Turntimber Symbiosis"},
                {"Umara Skyfalls","Umara Wizard"},
                {"Vastwood Thicket","Vastwood Fortification"},
                {"Valakut Stoneforge","Valakut Awakening"},
                {"Zof Bloodbog","Zof Consumption"},
            };
        }

        public bool HasMultiPart(ICard card)
        {
            return card.OtherPartName != null;
        }

        public bool IsDownSide(ICard card)
        {
            return HasMultiPart(card) &&
                  !IsSplitted(card) && !IsReverseSide(card) && !IsReverseSideOfFlipLand(card) && !IsMultiCard(card) && !IsSameDisplay(card) ;
        }

        //Adventure
        private bool IsSameDisplay(ICard card)
        {
             return HasMultiPart(card) && card.Type.EndsWith("Adventure"); 
        }

        //Recto-Verso from Zendikar Rising
        private bool IsReverseSideOfFlipLand(ICard card)
        {
            //There is no way to know the recto from verso on Zendikar Rising Flip Land so we keep a hard coded list
            return _reverseSideOfFlipLand.ContainsKey(card.Name);
        }

        //Recto-Verso
        private bool IsReverseSide(ICard card)
        {
            //Exclude land because of Westvale Abbey and Hanweir Battlements
            //But keep transformed to land
            return HasMultiPart(card) && card.CastingCost == null &&
              (!card.Type.ToLowerInvariant().Contains("land") || (card.Text != null && card.Text.StartsWith("(Transforms from")));
        }
        //Multiple part on the same side (it is not the case of Up/Down)
        private bool IsSplitted(ICard card)
        {
            return HasMultiPart(card) && card.PartName != card.Name && card.OtherPartName != card.Name; 
        }

        //B.F.M. (Big Furry Monster)
        private bool IsMultiCard(ICard card)
        {
            return HasMultiPart(card) && card.PartName == card.Name && card.OtherPartName == card.Name && card.CastingCost != null;
        }

        //Aftermath
        public bool Is90DegreeSide(ICard card)
        {
            return IsSplitted(card) && card.Text != null && card.Text.StartsWith("Aftermath");
        }

        public bool ShouldIgnore(ICard card)
        {
            //Ignore the reverse side of a recto-verso card
            return IsReverseSide(card) || IsReverseSideOfFlipLand(card);
        }

        public bool IsSecondPartOfSplitted(ICard card)
        {
            return IsSplitted(card) && !card.Name.StartsWith(card.PartName);
        }

        public ICard GetOtherPartCard(ICard card, Func<string, string, ICard> getCard)
        {
            if (!HasMultiPart(card))
            {
                return null;
            }

            return IsSplitted(card) ? getCard(card.Name, card.OtherPartName) : getCard(card.OtherPartName, null);
        }

        public ShardColor GetColor(ICardAllDbInfo cai)
        {
            ShardColor color = MagicRules.GetColor(cai.Card.CastingCost);
            if (IsSplitted(cai.Card))
            {
                color |= MagicRules.GetColor(cai.CardPart2.CastingCost);
            }

            return color;
        }

        public CardType GetCardType(ICardAllDbInfo cai)
        {
            CardType type = MagicRules.GetCardType(cai.Card.Type, cai.Card.CastingCost);
            if (IsSplitted(cai.Card))
            {
                type |= MagicRules.GetCardType(cai.CardPart2.Type, cai.CardPart2.CastingCost);
            }
            return type;
        }

        public CardSubType GetCardSubType(ICardAllDbInfo cai)
        {
            CardSubType subType = MagicRules.GetCardSubType(cai.Card.Type);
            if (IsSplitted(cai.Card))
            {
                subType |= MagicRules.GetCardSubType(cai.CardPart2.Type);
            }

            return subType;

        }
    }
}
