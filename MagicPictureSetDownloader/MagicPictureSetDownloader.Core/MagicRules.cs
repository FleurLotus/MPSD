namespace MagicPictureSetDownloader.Core
{
    using System.Linq;

    using Common.Library.Enums;

    using MagicPictureSetDownloader.Interface;
    
    public enum DisplayCardType
    {
        Instant,
        Sorcery,
        Enchantment,
        Creature,
        Planeswalker,
        Artifact,
        Land,
        Plane,
        Scheme,
        Conspiracy,
        Phenomenon,
        Contraption,
        Vanguard,
        Token,
        Stickers,
    }

    public enum DisplayColor
    {
        Colorless,
        White,
        Blue,
        Black,
        Red,
        Green,
        Multicolor,
        Land,
        Special,
    }

    internal static class MagicRules
    {
        public static DisplayColor GetDisplayColor(string castingCost)
        {
            ShardColor color = GetColor(castingCost);
            if (color == ShardColor.Colorless)
            {
                return DisplayColor.Colorless;
            }

            if (color == ShardColor.White)
            {
                return DisplayColor.White;
            }

            if (color == ShardColor.Blue)
            {
                return DisplayColor.Blue;
            }

            if (color == ShardColor.Black)
            {
                return DisplayColor.Black;
            }

            if (color == ShardColor.Red)
            {
                return DisplayColor.Red;
            }

            if (color == ShardColor.Green)
            {
                return DisplayColor.Green;
            }

            return DisplayColor.Multicolor;
        }
        public static int GetConvertedCastCost(string castingCost)
        {
            return Shard.GetShards(castingCost).Sum(shard => shard.ConvertedCastingCost);
        }
        public static DisplayCardType GetDisplayCardType(string type, string castingCost)
        {
            CardType card = GetCardType(type, castingCost);

            if (Matcher<CardType>.HasValue(card, CardType.Land))
            {
                return DisplayCardType.Land;
            }

            if (Matcher<CardType>.HasValue(card, CardType.Artifact))
            {
                return DisplayCardType.Artifact;
            }

            if (Matcher<CardType>.HasValue(card, CardType.Planeswalker))
            {
                return DisplayCardType.Planeswalker;
            }

            if (Matcher<CardType>.HasValue(card, CardType.Instant))
            {
                return DisplayCardType.Instant;
            }

            if (Matcher<CardType>.HasValue(card, CardType.Sorcery))
            {
                return DisplayCardType.Sorcery;
            }

            if (Matcher<CardType>.HasValue(card, CardType.Creature))
            {
                return DisplayCardType.Creature;
            }

            if (Matcher<CardType>.HasValue(card, CardType.Enchantment))
            {
                return DisplayCardType.Enchantment;
            }

            if (Matcher<CardType>.HasValue(card, CardType.Plane))
            {
                return DisplayCardType.Plane;
            }

            if (Matcher<CardType>.HasValue(card, CardType.Phenomenon))
            {
                return DisplayCardType.Phenomenon;
            }

            if (Matcher<CardType>.HasValue(card, CardType.Conspiracy))
            {
                return DisplayCardType.Conspiracy;
            }

            if (Matcher<CardType>.HasValue(card, CardType.Scheme))
            {
                return DisplayCardType.Scheme;
            }

            if (Matcher<CardType>.HasValue(card, CardType.Contraption))
            {
                return DisplayCardType.Contraption;
            }

            if (Matcher<CardType>.HasValue(card, CardType.Vanguard))
            {
                return DisplayCardType.Vanguard;
            }

            if (Matcher<CardType>.HasValue(card, CardType.Stickers))
            {
                return DisplayCardType.Stickers;
            }

            return DisplayCardType.Token;
        }
        //For search 
        public static CardType GetCardType(string type, string castingCost)
        {
            CardType cardType = CardType.Token;
            
            if (IsToken(type) || string.IsNullOrEmpty(castingCost) && IsCreature(type))
            {
                return CardType.Token;
            }

            if (IsLand(type))
            {
                cardType |= CardType.Land;
            }

            if (IsInstant(type))
            {
                cardType |= CardType.Instant;
            }

            if (IsSorcery(type))
            {
                cardType |= CardType.Sorcery;
            }

            if (IsEnchantment(type))
            {
                cardType |= CardType.Enchantment;
            }

            if (IsCreature(type))
            {
                cardType |= CardType.Creature;
            }

            if (IsArtifact(type))
            {
                cardType |= CardType.Artifact;
            }

            if (IsPlaneswalker(type))
            {
                cardType |= CardType.Planeswalker;
            }


            if (IsPhenomenon(type))
            {
                cardType |= CardType.Phenomenon;
            }

            if (IsPlane(type))
            {
                cardType |= CardType.Plane;
            }

            if (IsConspiracy(type))
            {
                cardType |= CardType.Conspiracy;
            }

            if (IsScheme(type))
            {
                cardType |= CardType.Scheme;
            }

            if (IsContraption(type))
            {
                cardType |= CardType.Contraption;
            }

            if (IsVanguard(type))
            {
                cardType |= CardType.Vanguard;
            }

            if (IsStickers(type))
            {
                cardType |= CardType.Stickers;
            }

            return cardType;
        }
        public static CardSubType GetCardSubType(string type)
        {
            CardSubType cardSubType = CardSubType.None;
            if (IsVehicle(type))
            {
                cardSubType |= CardSubType.Vehicle;
            }

            if (IsHost(type))
            {
                cardSubType |= CardSubType.Host;
            }

            if (IsAura(type))
            {
                cardSubType |= CardSubType.Aura;
            }

            if (IsSnow(type))
            {
                cardSubType |= CardSubType.Snow;
            }

            if (IsLegendary(type))
            {
                cardSubType |= CardSubType.Legendary;
            }

            if (IsCurse(type))
            {
                cardSubType |= CardSubType.Curse;
            }

            if (IsTrap(type))
            {
                cardSubType |= CardSubType.Trap;
            }

            if (IsArcane(type))
            {
                cardSubType |= CardSubType.Arcane;
            }

            if (IsTribal(type))
            {
                cardSubType |= CardSubType.Tribal;
            }

            if (IsSaga(type))
            {
                cardSubType |= CardSubType.Saga;
            }

            if (IsAdventure(type))
            {
                cardSubType |= CardSubType.Adventure;
            }

            if (IsEquipment(type))
            {
                cardSubType |= CardSubType.Equipment;
            }
            return cardSubType;
        }
        public static ShardColor GetColor(string castingCost)
        {
            return Shard.GetShards(castingCost).Select(shard => shard.Color)
                                               .Aggregate(ShardColor.Colorless, (current, newColor) => current | newColor);
        }
        public static bool IsSorcery(string type)
        {
            return type.ToLowerInvariant().Contains("sorcery");
        }
        public static bool IsInstant(string type)
        {
            return type.ToLowerInvariant().Contains("instant") || type.ToLowerInvariant().Contains("interrupt");
        }
        public static bool IsEnchantment(string type)
        {
            return type.ToLowerInvariant().Contains("enchant");
        }
        public static bool IsLand(string type)
        {
            return type.ToLowerInvariant().Contains("land");
        }
        public static bool IsPlaneswalker(string type)
        {
            return type.ToLowerInvariant().Contains("planeswalker");
        }
        public static bool IsArtifact(string type)
        {
            return type.ToLowerInvariant().Contains("artifact");
        }
        public static bool IsCreature(string type)
        {
            return type.ToLowerInvariant().Contains("summon") || type.ToLowerInvariant().Contains("eaturecray") || 
                  (type.ToLowerInvariant().Contains("creature") && !type.ToLowerInvariant().Contains("enchant creature"));
        }
        public static bool IsTribal(string type)
        {
            return type.ToLowerInvariant().Contains("tribal");
        }
        public static bool IsArcane(string type)
        {
            return type.ToLowerInvariant().Contains("arcane");
        }
        public static bool IsLegendary(string type)
        {
            return type.ToLowerInvariant().Contains("legend");
        }
        public static bool IsSpecial(string type)
        {
            return IsPhenomenon(type) || IsConspiracy(type) || IsScheme(type) || IsPlane(type) || IsContraption(type) || IsVanguard(type) || IsStickers(type) ;
        }
        public static bool IsContraption(string type)
        {
            return type.ToLowerInvariant().Contains("contraption");
        }
        public static bool IsPhenomenon(string type)
        {
            return type.ToLowerInvariant().Contains("phenomenon");
        }
        public static bool IsConspiracy(string type)
        {
            return type.ToLowerInvariant().Contains("conspiracy");
        }
        public static bool IsScheme(string type)
        {
            return type.ToLowerInvariant().Contains("scheme");
        }
        public static bool IsPlane(string type)
        {
            return type.ToLowerInvariant().Contains("plane") && !IsPlaneswalker(type);
        }
        public static bool IsSaga(string type)
        {
            return type.ToLowerInvariant().Contains("saga");
        }
        public static bool IsAdventure(string type)
        {
            return type.ToLowerInvariant().Contains("adventure");
        }
        public static bool IsVehicle(string type)
        {
            return type.ToLowerInvariant().Contains("vehicle");
        } 
        public static bool IsHost(string type)
        {
            return type.ToLowerInvariant().Contains("host");
        }
        public static bool IsAura(string type)
        {
            return type.ToLowerInvariant().Contains("aura");
        }
        public static bool IsSnow(string type)
        {
            return type.ToLowerInvariant().Contains("snow");
        }
        public static bool IsCurse(string type)
        {
            return type.ToLowerInvariant().Contains("curse");
        }
        public static bool IsTrap(string type)
        {
            return type.ToLowerInvariant().Contains("trap");
        }
        public static bool IsEquipment(string type)
        {
            return type.ToLowerInvariant().Contains("equipment");
        }
        public static bool IsVanguard(string type)
        {
            return type.ToLowerInvariant().Contains("vanguard");
        }
        public static bool IsStickers(string type)
        {
            return type.ToLowerInvariant().Contains("stickers");
        }
        public static bool IsToken(string type)
        {
            return type.ToLowerInvariant().Contains("token");
        }
    }
}
