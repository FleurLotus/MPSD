namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;

    using Common.Library.Enums;

    using MagicPictureSetDownloader.Interface;

    public class CustomCardComparer : IComparer<ICardAllDbInfo>
    {
        private int CompareArtifact(ICardAllDbInfo x, ICardAllDbInfo y)
        {
            CardSubType xCardSubType = MultiPartCardManager.Instance.GetCardSubType(x);
            CardSubType yCardSubType = MultiPartCardManager.Instance.GetCardSubType(y);

            //for artifact
            //Rule 1:
            //  a) Vehicule last
            int? comp = CompareHasValue(xCardSubType, yCardSubType, CardSubType.Vehicle, false);
            if (comp.HasValue && comp.Value != 0)
            {
                return comp.Value;
            }
            //  a) creature before
            CardType xCardType = MultiPartCardManager.Instance.GetCardType(x);
            CardType yCardType = MultiPartCardManager.Instance.GetCardType(y);

            comp = CompareHasValue(xCardType, yCardType, CardType.Creature, false);
            if (comp.HasValue && comp.Value != 0)
            {
                return comp.Value;
            }
            //  c) equipement before, others first 
            comp = CompareHasValue(xCardSubType, yCardSubType, CardSubType.Equipment, false);
            if (comp.HasValue && comp.Value != 0)
            {
                return comp.Value;
            }
            //  d) others first 

            //Rule 2: By Color 
            comp = CompareColor(x, y, true);
            if (comp.HasValue && comp.Value != 0)
            {
                return comp.Value;
            }

            //Rule 3: By weighted CCM (Number of colored mana increasing), then by generic mana (X is biggest)
            comp = CompareCCM(x, y);
            if (comp.HasValue && comp.Value != 0)
            {
                return comp.Value;
            }

            //Rule 4: for Creature and Vehicule: by force increasing, then endurance increasing
            comp = ComparePTLoyaltyDefense(x, y);
            if (comp.HasValue && comp.Value != 0)
            {
                return comp.Value;
            }

            return 0;
        }
        public int Compare(ICardAllDbInfo x, ICardAllDbInfo y)
        {
            CardType xCardType = MultiPartCardManager.Instance.GetCardType(x);
            CardType yCardType = MultiPartCardManager.Instance.GetCardType(y);

            int? comp;
            //Rule 1:
            //  a) Special first : Plane, Schema, Conspiracy, Phenomenon, Contraption, Vanguard, Land
            foreach (CardType cardType in new[] { CardType.Plane, CardType.Scheme, CardType.Conspiracy, CardType.Phenomenon, CardType.Contraption, CardType.Vanguard, CardType.Land })
            {
                comp = CompareHasValue(xCardType, yCardType, cardType, true);
                if (comp.HasValue)
                {
                    return comp.Value;
                }
            }
            //  b) Artefact last (WhatEverColor)
            comp = CompareHasValue(xCardType, yCardType, CardType.Artifact, false);
            if (comp.HasValue)
            {
                if (comp.Value == 0)
                {
                    return CompareArtifact(x, y);
                }
                return comp.Value;
            }
            //  c) Others in between

            //Rule 2: By Color 
            int c = CompareColor(x, y, false);
            if (c != 0)
            {
                return c;
            }

            //Rule 3: By Type
            c = CompareType(x, y);
            if (c != 0)
            {
                return c;
            }

            //Rule 4: By weighted CCM
            c = CompareCCM(x, y);
            if (c != 0)
            {
                return c;
            }

            //Rule 5: By P/T, Loyalty, Defense for Creature, Vehicule: by force increasing, then endurance increasing
            c = ComparePTLoyaltyDefense(x, y);
            if (c != 0)
            {
                return c;
            }

            // then Name
            return 0;
        }
        private int CompareColor(ICardAllDbInfo x, ICardAllDbInfo y, bool colorLessFirst)
        {
            ShardColor xColor = MultiPartCardManager.Instance.GetColor(x);
            ShardColor yColor = MultiPartCardManager.Instance.GetColor(y);
            int xColorCount = ColorCount(xColor);
            int yColorCount = ColorCount(yColor);

            int? comp;

            //  a) by color order: W, U, B, R, G, MultiColor, Colorless  (For artifact, Colorless  is first)   
            if (colorLessFirst)
            {
                comp = CompareEquals(xColor, yColor, ShardColor.Colorless, true);
                if (comp.HasValue)
                {
                    return comp.Value;
                }
            }


            
            foreach (ShardColor color in new [] {ShardColor.White, ShardColor.Blue, ShardColor.Black, ShardColor.Red, ShardColor.Green})
            {
                comp = CompareEquals(xColor, yColor, color, true);
                if (comp.HasValue)
                {
                    return comp.Value;
                }
            }

            if (!colorLessFirst)
            {
                comp = CompareEquals(xColor, yColor, ShardColor.Colorless, false);
                if (comp.HasValue)
                {
                    return comp.Value;
                }
            }

            //  b) in MultiColor: * by Number of Color
            int c = xColorCount.CompareTo(yColorCount);
            if (c != 0)
            {
                return c;
            }

            //                    * then Contains W, Contains U, Contains B, Contains R 
            foreach (ShardColor color in new[] { ShardColor.White, ShardColor.Blue, ShardColor.Black, ShardColor.Red, ShardColor.Green })
            {
                comp = CompareHasValue(xColor, yColor, color, true);
                if (comp.HasValue && comp.Value != 0)
                {
                    return comp.Value;
                }
            }

            return 0;
        }
        private int CompareType(ICardAllDbInfo x, ICardAllDbInfo y)
        {
            CardType xCardType = MultiPartCardManager.Instance.GetCardType(x);
            CardType yCardType = MultiPartCardManager.Instance.GetCardType(y);

            //  a) By Type for all no Artifact: Instant, Sorcery, Enchantment, Creature, Planeswalker, Battle, others
            foreach (CardType cardType in new[] { CardType.Instant, CardType.Sorcery, CardType.Enchantment, CardType.Creature, CardType.Planeswalker, CardType.Battle })
            {
                int? comp = CompareHasValue(xCardType, yCardType, cardType, true);
                if (comp.HasValue)
                {
                    //  b) In Enchantement: Aura then No Aura
                    if (comp.Value == 0 && cardType == CardType.Enchantment)
                    {
                        CardSubType xCardSubType = MultiPartCardManager.Instance.GetCardSubType(x);
                        CardSubType yCardSubType = MultiPartCardManager.Instance.GetCardSubType(y);

                        comp = CompareHasValue(xCardSubType, yCardSubType, CardSubType.Aura, true);
                        if (comp.HasValue)
                        {
                            return comp.Value;
                        }
                        return 0;
                    }

                    return comp.Value;
                }
            }

            return 0;
        }

        private int CompareCCM(ICardAllDbInfo x, ICardAllDbInfo y)
        {
            (int xNoGenerics, int xXYZ, int xGenerics) = GetWeightedCCM(x);
            (int yNoGenerics, int yXYZ, int yGenerics) = GetWeightedCCM(y);
            
            //  a) by Number of colored mana increasing
            int comp = xNoGenerics.CompareTo(yNoGenerics);
            if (comp != 0)
            {
                return comp;
            }
            //  b) by generic mana (X is biggest)
            comp = xXYZ.CompareTo(yXYZ);
            if (comp != 0)
            {
                return comp;
            }
            return xGenerics.CompareTo(yGenerics);
        }

        private (int, int, int) GetWeightedCCM(ICardAllDbInfo cardAllDbInfo)
        {
            string castingCost = cardAllDbInfo.MainCardFace.CastingCost;

            /* ALERT TO BE REVIEWED
            if (MultiPartCardManager.Instance.HasMultiPart(cardAllDbInfo.Card) && cardAllDbInfo.CardPart2 != null)
            {
                castingCost += $" {cardAllDbInfo.CardPart2.CastingCost}";
            }
            */
            int noGenerics = 0;
            int generics = 0;
            int countXYZ = 0;

            foreach (Shard shard in Shard.GetShards(castingCost))
            {
                if (shard.IsXYZ)
                {
                    countXYZ++;
                }
                else if (shard.IsGeneric)
                {
                    generics += shard.ConvertedCastingCost;
                }
                else
                {
                    noGenerics++;
                }
            }

            return (noGenerics, countXYZ, generics);
        }

        private int GetToughness(ICardAllDbInfo cardAllDbInfo)
        {
            string toughness = cardAllDbInfo.MainCardFace.Toughness;
            /* ALERT TO BE REVIEWED
            if (string.IsNullOrWhiteSpace(toughness) && MultiPartCardManager.Instance.HasMultiPart(cardAllDbInfo.Card) && cardAllDbInfo.CardPart2 != null)
            {
                toughness = cardAllDbInfo.CardPart2.Toughness;
            }
            */
            if (int.TryParse(toughness, out int ret))
            {
                return ret;
            }

            return int.MaxValue;
        }
        private int GetPower(ICardAllDbInfo cardAllDbInfo)
        {
            string power = cardAllDbInfo.MainCardFace.Power;
            /* ALERT TO BE REVIEWED
            if (string.IsNullOrWhiteSpace(power) && MultiPartCardManager.Instance.HasMultiPart(cardAllDbInfo.Card) && cardAllDbInfo.CardPart2 != null)
            {
                power = cardAllDbInfo.CardPart2.Power;
            }
            */

            if (int.TryParse(power, out int ret))
            {
                return ret;
            }

            return int.MaxValue;
        }
        private int GetLoyalty(ICardAllDbInfo cardAllDbInfo)
        {
            string loyalty = cardAllDbInfo.MainCardFace.Loyalty;
            /* ALERT TO BE REVIEWED
            if (string.IsNullOrWhiteSpace(loyalty) && MultiPartCardManager.Instance.HasMultiPart(cardAllDbInfo.Card) && cardAllDbInfo.CardPart2 != null)
            {
                loyalty = cardAllDbInfo.CardPart2.Loyalty;
            }
            */
            if (int.TryParse(loyalty, out int ret))
            {
                return ret;
            }

            return int.MaxValue;
        }
        private int GetDefense(ICardAllDbInfo cardAllDbInfo)
        {
            string defense = cardAllDbInfo.MainCardFace.Defense;
            /* ALERT TO BE REVIEWED
            if (string.IsNullOrWhiteSpace(defense) && MultiPartCardManager.Instance.HasMultiPart(cardAllDbInfo.Card) && cardAllDbInfo.CardPart2 != null)
            {
                defense = cardAllDbInfo.CardPart2.Defense;
            }
            */
            if (int.TryParse(defense, out int ret))
            {
                return ret;
            }

            return int.MaxValue;
        }
        private int ComparePTLoyaltyDefense(ICardAllDbInfo x, ICardAllDbInfo y)
        {
            CardType xCardType = MultiPartCardManager.Instance.GetCardType(x);
            CardType yCardType = MultiPartCardManager.Instance.GetCardType(y);
            CardSubType xCardSubType = MultiPartCardManager.Instance.GetCardSubType(x);
            CardSubType yCardSubType = MultiPartCardManager.Instance.GetCardSubType(y);

            if ((Matcher<CardType>.HasValue(xCardType, CardType.Creature) && Matcher<CardType>.HasValue(yCardType, CardType.Creature)) ||
                (Matcher<CardSubType>.HasValue(xCardSubType, CardSubType.Vehicle) && Matcher<CardSubType>.HasValue(yCardSubType, CardSubType.Vehicle)))
            {
                int xPower = GetPower(x);
                int yPower = GetPower(y);

                int comp = xPower.CompareTo(yPower);
                if (comp != 0)
                {
                    return comp;
                }
                int xToughness = GetToughness(x);
                int yToughness = GetToughness(y);

                comp = xToughness.CompareTo(yToughness);
                if (comp != 0)
                {
                    return comp;
                }
            }

            if (Matcher<CardType>.HasValue(xCardType, CardType.Planeswalker) && Matcher<CardType>.HasValue(yCardType, CardType.Planeswalker))
            {
                int xLoyalty = GetLoyalty(x);
                int yLoyalty = GetLoyalty(y);

                int comp = xLoyalty.CompareTo(yLoyalty);
                if (comp != 0)
                {
                    return comp;
                }
            }

            if (Matcher<CardType>.HasValue(xCardType, CardType.Battle) && Matcher<CardType>.HasValue(yCardType, CardType.Battle))
            {
                int xDefense = GetDefense(x);
                int yDefense = GetDefense(y);

                int comp = xDefense.CompareTo(yDefense);
                if (comp != 0)
                {
                    return comp;
                }
            }


            return 0;
        }

        private static int? CompareHasValue<T>(T xValue, T yValue, T valueToCheck, bool first) where T : struct, IConvertible
        {
            int ret = first ? -1 : 1;

            if (Matcher<T>.HasValue(xValue, valueToCheck))
            {
                if (Matcher<T>.HasValue(yValue, valueToCheck))
                {
                    return 0;
                }

                return ret;
            }

            if (Matcher<T>.HasValue(yValue, valueToCheck))
            {
                return -ret;
            }

            return null;
        }
        private static int? CompareEquals<T>(T xValue, T yValue, T valueToCheck, bool first) where T : struct
        {
            int ret = first ? -1 : 1;

            if (xValue.Equals(valueToCheck))
            {
                if (yValue.Equals(valueToCheck))
                {
                    return 0;
                }

                return ret;
            }

            if (yValue.Equals(valueToCheck))
            {
                return -ret;
            }

            return null;
        }

        private int ColorCount(ShardColor shardColor)
        {
            int count = 0;
            foreach (ShardColor color in new[] { ShardColor.White, ShardColor.Blue, ShardColor.Black, ShardColor.Red, ShardColor.Green })
            {
                if (Matcher<ShardColor>.HasValue(shardColor, color))
                {
                    count++;
                }
            }
            return count;
        }

    }
}
