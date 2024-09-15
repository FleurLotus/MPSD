namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using MagicPictureSetDownloader.Interface;

    public class Shard
    {
        public const string Prefix = "{";
        public const string Suffix = "}";
        public const string Separator = "/";

        private static readonly Regex ManaRegex = new Regex("{(?<Mana>[^}]+)}", RegexOptions.Compiled);

        private const char White = 'W';
        private const char Blue = 'U';
        private const char Black = 'B';
        private const char Red = 'R';
        private const char Green = 'G';
        private const char Colorless = 'C';
        private const char Legendary = 'L';
        private const char Drop = 'D';

        public const string DisplayPrefix = "@";

        private const string Half = "H";
        private const string Phyrexian = "P";
        private const string TwoHybrid = "2";
        private const string Snow = "S";

        private static readonly string[] Generics = { "X", "Y", "Z" };

        private static readonly IDictionary<string, Shard> _shards = new Dictionary<string, Shard>(StringComparer.InvariantCultureIgnoreCase);

        private readonly string _toString;

        private Shard(string shardCastingCost, bool isGeneric, bool isSnow, bool isColorless, bool isXYZ, bool isLegendary, bool isDrop)
        {
            ShardCastingCost = shardCastingCost;
            Color = ShardColor.Colorless;
            IsGeneric = isGeneric;      // -> CCM = Value
            IsSnow = isSnow;            // -> CCM = 1
            IsColorless = isColorless;  // -> CCM = 1
            IsXYZ = isXYZ;              // -> CCM = 0
            IsLegendary = isLegendary;
            IsDrop = isDrop;

            if (IsXYZ)
            {
                ConvertedCastingCost = 0;
            }
            else if (IsGeneric)
            {
                ConvertedCastingCost = int.Parse(ShardCastingCost);
            }
            else
            {
                ConvertedCastingCost = 1;
            }

            _toString = string.Format("{0} => {1} CCM={2} {3}{4}{5}{6}{7}{8}", ShardCastingCost, Color, ConvertedCastingCost, IsGeneric ? "(IsGeneric)" : string.Empty,
                 IsSnow ? "(IsSnow)" : string.Empty, IsColorless ? "(IsColorless)" : string.Empty, IsXYZ ? "(IsXYZ)" : string.Empty, 
                 IsLegendary ? "(IsLegendary)" : string.Empty, IsDrop ? "(IsDrop)" : string.Empty);
        }
        private Shard(string shardCastingCost, ShardColor color, bool isPhyrexian, bool isHybrid, bool is2Hybrid, bool isHalf)
        {
            ShardCastingCost = shardCastingCost;
            Color = color;
            IsPhyrexian = isPhyrexian;  // -> CCM = 1
            IsHybrid = isHybrid;        // -> CCM = 1
            Is2Hybrid = is2Hybrid;      // -> CCM = 2
            IsHalf = isHalf;            // -> CCM = 0.5 -> 1

            ConvertedCastingCost = Is2Hybrid ? 2 : 1;

            _toString = string.Format("{0} => {1} CCM={2} {3}{4}{5}{6}", ShardCastingCost, Color, ConvertedCastingCost, IsPhyrexian ? "(IsPhyrexian)" : string.Empty,
                             IsHybrid ? "(IsHybrid)" : string.Empty, Is2Hybrid ? "(Is2Hybrid)" : string.Empty, IsHalf ? "(IsHalf)" : string.Empty);
        }

        public string ShardCastingCost { get; }
        public ShardColor Color { get; }
        
        public bool IsGeneric { get; }
        public bool IsSnow { get; }
        public bool IsColorless { get; }
        public bool IsXYZ { get; }
        public bool IsLegendary { get; }
        public bool IsDrop { get; }
        
        public bool IsPhyrexian { get; }
        public bool IsHybrid { get; }
        public bool Is2Hybrid { get; }
        public bool IsHalf { get; }

        public int ConvertedCastingCost { get; }

        public override string ToString()
        {
            return _toString;
        }

        internal static IEnumerable<Shard> GetShards(string castingCost)
        {
            if (string.IsNullOrWhiteSpace(castingCost))
            {
                return Array.Empty<Shard>();
            }

            return ManaRegex.Matches(castingCost.ToUpper()).Select(m => GetShard(m.Groups["Mana"].Value));
        }

        private static Shard GetShard(string shardCastingCost)
        {
            if (_shards.TryGetValue(shardCastingCost, out Shard shard))
            {
                return shard;
            }

            if (int.TryParse(shardCastingCost, out int _))
            {
                //IsGeneric
                shard = new Shard(shardCastingCost, true, false, false, false, false, false);
                _shards.Add(shardCastingCost, shard);
                return shard;
            }

            if (shardCastingCost == Snow)
            {
                //IsSnow
                shard = new Shard(shardCastingCost, false, true, false, false, false, false);
                _shards.Add(shardCastingCost, shard);
                return shard;
            }

            if (shardCastingCost == Colorless.ToString())
            {
                //IsColorless
                shard = new Shard(shardCastingCost, false, false, true, false, false, false);
                _shards.Add(shardCastingCost, shard);
                return shard;
            }

            if (shardCastingCost == Legendary.ToString())
            {
                //IsLegendary
                shard = new Shard(shardCastingCost, false, false, false, false, true, false);
                _shards.Add(shardCastingCost, shard);
                return shard;
            }
            
            if (shardCastingCost == Drop.ToString())
            {
                //IsDrop
                shard = new Shard(shardCastingCost, false, false, false, false, false, true);
                _shards.Add(shardCastingCost, shard);
                return shard;
            }

            if (Generics.Contains(shardCastingCost))
            {
                //IsXYZ => IsGeneric
                shard = new Shard(shardCastingCost, true, false, false, true, false, false);
                _shards.Add(shardCastingCost, shard);
                return shard;
            }

            string workingShardCastingCost = shardCastingCost;
            ShardColor color = ShardColor.Colorless;
            bool isPhyrexian = false;
            bool isHybrid = false;
            bool is2Hybrid = false;
            bool isHalf = false;

            if (workingShardCastingCost.StartsWith(Half))
            {
                isHalf = true;
                workingShardCastingCost = workingShardCastingCost[Half.Length..];
            }

            if (workingShardCastingCost.EndsWith(Phyrexian))
            {
                isPhyrexian = true;
                workingShardCastingCost = workingShardCastingCost[..^Phyrexian.Length];
            }

            if (workingShardCastingCost.StartsWith(TwoHybrid))
            {
                is2Hybrid = true;
                workingShardCastingCost = workingShardCastingCost[TwoHybrid.Length..];
            }

            if (workingShardCastingCost.Length == 0)
            {
                throw new Exception($"length of workingShardCastingCost is 0 after removing additional info : {shardCastingCost}");
            }

            isHybrid = workingShardCastingCost.ToCharArray().Length > 1;

            foreach (char c in workingShardCastingCost.Replace(Separator,string.Empty))
            {
                color |= c switch
                {
                    White => ShardColor.White,
                    Blue => ShardColor.Blue,
                    Black => ShardColor.Black,
                    Red => ShardColor.Red,
                    Green => ShardColor.Green,
                    Colorless => ShardColor.Colorless,
                    _ => throw new Exception($"Unknown element in shard cast cost {shardCastingCost}: {c}"),
                };
            }

            shard = new Shard(shardCastingCost, color, isPhyrexian, isHybrid, is2Hybrid, isHalf);
            _shards.Add(shardCastingCost, shard);
            return shard;
        }
    }
}
