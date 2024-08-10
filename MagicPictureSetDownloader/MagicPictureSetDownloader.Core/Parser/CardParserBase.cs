namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    internal class CardParserBase
    {
        private const string Start = @"<div class=""contentcontainer"">";
        private const string End = @"<div class=""clear""></div>";
        private const string CardStartBlock = @"<td id=""ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_cardComponent";
        private const string CardStartData = @"<table class=""cardDetails";
        private const string LastCardEndData = @"</td>";
        public const string ImageKey = "Image";
        public const string NameKey = "Name";
        public const string ManaCostKey = "ManaCost";
        public const string CmcKey = "CMC";
        public const string TypeKey = "Type";
        public const string TextKey = "Text";
        public const string FlavorKey = "Flavor";
        public const string PTKey = "PT";
        public const string SetKey = "Set";
        public const string RarityKey = "Rarity";
        public const string NumberKey = "Number";
        public const string ArtistKey = "Artist";
        public const string VariationsKey = "Variations";

        private static readonly IDictionary<string, IDictionary<string, string>> _missingPropertyValue = new Dictionary<string, IDictionary<string, string>>
        {
            { "Rarity:", new Dictionary<string, string>
                {
                    //Warhammer 40,000 Commander
                    {"Fabricate", "Rare" },
                    //Magic The Gathering—Fallout
                    {"War Room", "Rare" }
                }
            },
        };

        private static readonly IDictionary<string, string> _missingLoyalty = new Dictionary<string, string>
        {
            //Innistrad: Midnight Hunt Alchemy
            {@"Garruk, Wrath of the Wilds","3"},
            {@"Tibalt, Wicked Tormentor","3"},
            //Lorwyn
            {@"Ajani Goldmane","4"},
            {@"Chandra Nalaar","6"},
            {@"Garruk Wildspeaker","3"},
            {@"Jace Beleren","3"},
            {@"Liliana Vess","5"},
            //Shards of Alara
            {@"Ajani Vengeant","3"},
            {@"Elspeth, Knight-Errant","4"},
            {@"Sarkhan Vol","4"},
            {@"Tezzeret the Seeker","4"},
            //Conflux
            {@"Nicol Bolas, Planeswalker","5"},
            //Zendikar
            {@"Chandra Ablaze","5"},
            {@"Nissa Revane","2"},
            {@"Sorin Markov","4"},
            //Worldwake
            {@"Jace, the Mind Sculptor","3"},
            //Rise of the Eldrazi
            {@"Gideon Jura","6"},
            {@"Sarkhan the Mad","7"},
            //Scars of Mirrodin
            {@"Elspeth Tirel","4"},
            {@"Koth of the Hammer","3"},
            {@"Venser, the Sojourner","3"},
            //Mirrodin Besieged
            {@"Tezzeret, Agent of Bolas","3"},
            //New Phyrexia
            {@"Karn Liberated","6"},
            //Magic 2012
            {@"Chandra, the Firebrand","3"},
            {@"Garruk, Primal Hunter","3"},
            {@"Jace, Memory Adept","4"},
            //Innistrad
            {@"Garruk Relentless","3"},
            {@"Liliana of the Veil","3"},
            //Dark Ascension
            {@"Sorin, Lord of Innistrad","3"},
            //Avacyn Restored
            {@"Tamiyo, the Moon Sage","4"},
            {@"Tibalt, the Fiend-Blooded","2"},
            //Magic 2013
            {@"Ajani, Caller of the Pride","4"},
            {@"Liliana of the Dark Realms","3"},
            //Return to Ravnica
            {@"Jace, Architect of Thought","4"},
            {@"Vraska the Unseen","5"},
            //Gatecrash
            {@"Domri Rade","3"},
            {@"Gideon, Champion of Justice","4"},
            //Dragon's Maze
            {@"Ral Zarek","4"},
            //Magic 2014
            {@"Chandra, Pyromaster","4"},
            {@"Garruk, Caller of Beasts","4"},
            //Theros
            {@"Ashiok, Nightmare Weaver","3"},
            {@"Elspeth, Sun's Champion","4"},
            {@"Xenagos, the Reveler","3"},
            //Born of the Gods
            {@"Kiora, the Crashing Wave","2"},
            //Journey into Nyx
            {@"Ajani, Mentor of Heroes","4"},
            //Conspiracy
            {@"Dack Fayden","3"},
            //Magic 2015
            {@"Ajani Steadfast","4"},
            {@"Garruk, Apex Predator","5"},
            {@"Jace, the Living Guildpact","5"},
            {@"Nissa, Worldwaker","3"},
            //Khans of Tarkir
            {@"Sarkhan, the Dragonspeaker","4"},
            {@"Sorin, Solemn Visitor","4"},
            //Commander 2014
            {@"Daretti, Scrap Savant","3"},
            {@"Freyalise, Llanowar's Fury","3"},
            {@"Nahiri, the Lithomancer","3"},
            {@"Ob Nixilis of the Black Oath","3"},
            {@"Teferi, Temporal Archmage","5"},
            //Fate Reforged
            {@"Ugin, the Spirit Dragon","7"},
            //Dragons of Tarkir
            {@"Narset Transcendent","6"},
            {@"Sarkhan Unbroken","4"},
            //Magic Origins
            {@"Chandra, Roaring Flame","4"},
            {@"Gideon, Battle-Forged","3"},
            {@"Jace, Telepath Unbound","5"},
            {@"Liliana, Defiant Necromancer","3"},
            {@"Nissa, Sage Animist","3"},
            //Battle for Zendikar
            {@"Gideon, Ally of Zendikar","4"},
            {@"Kiora, Master of the Depths","4"},
            {@"Ob Nixilis Reignited","5"},
            //Oath of the Gatewatch
            {@"Chandra, Flamecaller","4"},
            {@"Nissa, Voice of Zendikar","3"},
            //Shadows over Innistrad
            {@"Arlinn Kord","3"},
            {@"Jace, Unraveler of Secrets","5"},
            {@"Nahiri, the Harbinger","4"},
            {@"Sorin, Grim Nemesis","6"},
            //Eldritch Moon
            {@"Liliana, the Last Hope","3"},
            {@"Tamiyo, Field Researcher","4"},
            //Conspiracy: Take the Crown
            {@"Daretti, Ingenious Iconoclast","3"},
            {@"Kaya, Ghost Assassin","5"},
            //Kaladesh
            {@"Chandra, Pyrogenius","5"},
            {@"Chandra, Torch of Defiance","4"},
            {@"Dovin Baan","3"},
            {@"Nissa, Nature's Artisan","5"},
            {@"Nissa, Vital Force","5"},
            {@"Saheeli Rai","3"},
            //Aether Revolt
            {@"Ajani Unyielding","4"},
            {@"Ajani, Valiant Protector","4"},
            {@"Tezzeret the Schemer","5"},
            {@"Tezzeret, Master of Metal","5"},
            //Amonkhet
            {@"Gideon of the Trials","3"},
            {@"Gideon, Martial Paragon","5"},
            {@"Liliana, Death Wielder","5"},
            {@"Liliana, Death's Majesty","5"},
            {@"Nissa, Steward of Elements","X"},
            //Hour of Devastation
            {@"Nicol Bolas, God-Pharaoh","7"},
            {@"Nicol Bolas, the Deceiver","5"},
            {@"Nissa, Genesis Mage","5"},
            {@"Samut, the Tested","4"},
            //Ixalan
            {@"Huatli, Dinosaur Knight","4"},
            {@"Huatli, Warrior Poet","3"},
            {@"Jace, Cunning Castaway","3"},
            {@"Jace, Ingenious Mind-Mage","5"},
            {@"Vraska, Relic Seeker","6"},
            //Unstable
            {@"Urza, Academy Headmaster","4"},
            //Rivals of Ixalan
            {@"Angrath, Minotaur Pirate","5"},
            {@"Angrath, the Flame-Chained","4"},
            {@"Huatli, Radiant Champion","3"},
            {@"Vraska, Scheming Gorgon","5"},
            //Dominaria
            {@"Chandra, Bold Pyromancer","5"},
            {@"Jaya Ballard","5"},
            {@"Karn, Scion of Urza","5"},
            {@"Teferi, Hero of Dominaria","4"},
            {@"Teferi, Timebender","5"},
            //Battlebond
            {@"Rowan Kenrith","4"},
            {@"Will Kenrith","4"},
            //Jiang Yanggu and Mu Yanling
            {@"Jiang Yanggu","4"},
            {@"Mu Yanling","5"},
            //Core Set 2019
            {@"Ajani, Adversary of Tyrants","4"},
            {@"Ajani, Wise Counselor","5"},
            {@"Liliana, Untouched by Death","4"},
            {@"Liliana, the Necromancer","4"},
            {@"Nicol Bolas, the Arisen","7"},
            {@"Sarkhan, Dragonsoul","5"},
            {@"Sarkhan, Fireblood","3"},
            {@"Tezzeret, Artifice Master","5"},
            {@"Tezzeret, Cruel Machinist","4"},
            {@"Vivien Reid","5"},
            {@"Vivien of the Arkbow","5"},
            //Commander 2018
            {@"Aminatou, the Fateshifter","3"},
            {@"Estrid, the Masked","3"},
            {@"Lord Windgrace","5"},
            {@"Saheeli, the Gifted","4"},
            //Guilds of Ravnica
            {@"Ral, Caller of Storms","4"},
            {@"Ral, Izzet Viceroy","5"},
            {@"Vraska, Golgari Queen","4"},
            {@"Vraska, Regal Gorgon","5"},
            //Ravnica Allegiance
            {@"Domri, Chaos Bringer","5"},
            {@"Domri, City Smasher","4"},
            {@"Dovin, Architect of Law","5"},
            {@"Dovin, Grand Arbiter","3"},
            {@"Kaya, Orzhov Usurper","3"},
            //War of the Spark
            {@"Ajani, the Greathearted","5"},
            {@"Angrath, Captain of Chaos","5"},
            {@"Arlinn, Voice of the Pack","7"},
            {@"Ashiok, Dream Render","5"},
            {@"Chandra, Fire Artisan","4"},
            {@"Davriel, Rogue Shadowmage","3"},
            {@"Domri, Anarch of Bolas","3"},
            {@"Dovin, Hand of Control","5"},
            {@"Gideon Blackblade","4"},
            {@"Gideon, the Oathsworn","4"},
            {@"Huatli, the Sun's Heart","7"},
            {@"Jace, Arcane Strategist","4"},
            {@"Jace, Wielder of Mysteries","4"},
            {@"Jaya, Venerated Firemage","5"},
            {@"Jiang Yanggu, Wildcrafter","3"},
            {@"Karn, the Great Creator","5"},
            {@"Kasmina, Enigmatic Mentor","5"},
            {@"Kaya, Bane of the Dead","7"},
            {@"Kiora, Behemoth Beckoner","7"},
            {@"Liliana, Dreadhorde General","6"},
            {@"Nahiri, Storm of Stone","6"},
            {@"Narset, Parter of Veils","5"},
            {@"Nicol Bolas, Dragon-God","4"},
            {@"Nissa, Who Shakes the World","5"},
            {@"Ob Nixilis, the Hate-Twisted","5"},
            {@"Ral, Storm Conduit","4"},
            {@"Saheeli, Sublime Artificer","5"},
            {@"Samut, Tyrant Smasher","5"},
            {@"Sarkhan the Masterless","5"},
            {@"Sorin, Vengeful Bloodlord","4"},
            {@"Tamiyo, Collector of Tales","5"},
            {@"Teferi, Time Raveler","4"},
            {@"Teyo, the Shieldmage","5"},
            {@"Tezzeret, Master of the Bridge","5"},
            {@"The Wanderer","5"},
            {@"Tibalt, Rakish Instigator","5"},
            {@"Ugin, the Ineffable","4"},
            {@"Vivien, Champion of the Wilds","4"},
            {@"Vraska, Swarm's Eminence","5"},
            //Modern Horizons
            {@"Serra the Benevolent","4"},
            {@"Wrenn and Six","3"},
            //Core Set 2020
            {@"Ajani, Inspiring Leader","5"},
            {@"Ajani, Strength of the Pride","5"},
            {@"Chandra, Acolyte of Flame","4"},
            {@"Chandra, Awakened Inferno","6"},
            {@"Chandra, Flame's Fury","4"},
            {@"Chandra, Novice Pyromancer","5"},
            {@"Mu Yanling, Celestial Wind","5"},
            {@"Mu Yanling, Sky Dancer","2"},
            {@"Sorin, Imperious Bloodlord","4"},
            {@"Sorin, Vampire Lord","4"},
            {@"Vivien, Arkbow Ranger","4"},
            {@"Vivien, Nature's Avenger","3"},
            //Throne of Eldraine
            {@"Garruk, Cursed Huntsman","5"},
            {@"Oko, Thief of Crowns","4"},
            {@"Oko, the Trickster","4"},
            {@"Rowan, Fearless Sparkmage","5"},
            {@"The Royal Scions","5"},
            //Theros Beyond Death
            {@"Ashiok, Nightmare Muse","5"},
            {@"Ashiok, Sculptor of Fears","4"},
            {@"Calix, Destiny's Hand","4"},
            {@"Elspeth, Sun's Nemesis","5"},
            {@"Elspeth, Undaunted Hero","5"},
            //Unsanctioned
            {@"B.O.B. (Bevy of Beebles)","*"},
            //Ikoria: Lair of Behemoths
            {@"Lukka, Coppercoat Outcast","5"},
            {@"Narset of the Ancient Way","4"},
            {@"Vivien, Monsters' Advocate","3"},
            //Core Set 2021
            {@"Basri Ket","3"},
            {@"Basri, Devoted Paladin","4"},
            {@"Chandra, Flame's Catalyst","5"},
            {@"Chandra, Heart of Fire","5"},
            {@"Garruk, Savage Herald","5"},
            {@"Garruk, Unleashed","4"},
            {@"Liliana, Death Mage","4"},
            {@"Liliana, Waker of the Dead","4"},
            {@"Teferi, Master of Time","3"},
            {@"Teferi, Timeless Voyager","4"},
            //Zendikar Rising
            {@"Jace, Mirror Mage","4"},
            {@"Nahiri, Heir of the Ancients","4"},
            {@"Nissa of Shadowed Boughs","4"},
            //Commander Legends
            {@"Jeska, Thrice Reborn","0"},
            {@"Tevesh Szat, Doom of Fools","4"},
            //Kaldheim
            {@"Kaya the Inexorable","5"},
            {@"Niko Aris","3"},
            {@"Tibalt, Cosmic Impostor","5"},
            {@"Tyvar Kell","3"},
            //Strixhaven: School of Mages
            {@"Kasmina, Enigma Sage","2"},
            {@"Lukka, Wayward Bonder","5"},
            {@"Professor Onyx","5"},
            {@"Rowan, Scholar of Sparks","2"},
            {@"Will, Scholar of Frost","4"},
            //Modern Horizons 2
            {@"Dakkon, Shadow Slayer","0"},
            {@"Geyadrone Dihada","4"},
            {@"Grist, the Hunger Tide","3"},
            //Adventures in the Forgotten Realms
            {@"Ellywick Tumblestrum","4"},
            {@"Grand Master of Flowers","3"},
            {@"Lolth, Spider Queen","4"},
            {@"Mordenkainen","5"},
            {@"Zariel, Archduke of Avernus","4"},
            //Jumpstart: Historic Horizons
            {@"Davriel, Soul Broker","4"},
            {@"Freyalise, Skyshroud Partisan","4"},
            {@"Kiora, the Tide's Fury","4"},
            {@"Sarkhan, Wanderer to Shiv","4"},
            {@"Teyo, Aegis Adept","4"},
            //Innistrad: Midnight Hunt
            {@"Arlinn, the Moon's Fury","4"},
            {@"Arlinn, the Pack's Hope","4"},
            {@"Teferi, Who Slows the Sunset","4"},
            {@"Wrenn and Seven","5"},
            //Innistrad: Crimson Vow
            {@"Chandra, Dressed to Kill","3"},
            {@"Kaya, Geist Hunter","3"},
            {@"Sorin the Mirthless","4"},
            //Kamigawa: Neon Dynasty
            {@"Kaito Shizuki","3"},
            {@"Tamiyo, Compleated Sage","5"},
            {@"Tezzeret, Betrayer of Flesh","4"},
            {@"The Wandering Emperor","3"},
            //Streets of New Capenna
            {@"Elspeth Resplendent","5"},
            {@"Ob Nixilis, the Adversary","3"},
            {@"Vivien on the Hunt","4"},
            //Commander Legends: Battle for Baldur's Gate
            {@"Elminster","5"},
            {@"Minsc & Boo, Timeless Heroes","3"},
            {@"Tasha, the Witch Queen","4"},
            //Alchemy Horizons: Baldur's Gate
            {@"Tasha, Unholy Archmage","4"},
            //Dominaria United
            {@"Ajani, Sleeper Agent","4"},
            {@"Jaya, Fiery Negotiator","4"},
            {@"Karn, Living Legacy","4"},
            //Dominaria United Commander
            {@"Dihada, Binder of Wills","5"},
            {@"Jared Carthalion","5"},
            //Unfinity
            {@"Comet, Stellar Pup","5"},
            {@"Space Beleren","3"},
            //The Brothers' War
            {@"Saheeli, Filigree Master","3"},
            {@"Teferi, Temporal Pilgrim","4"},
            {@"Urza, Planeswalker","7"},
            //Phyrexia: All Will Be One
            {@"Jace, the Perfected Mind","5"},
            {@"Kaito, Dancing Shadow","3"},
            {@"Kaya, Intangible Slayer","6"},
            {@"Koth, Fire of Resistance","4"},
            {@"Lukka, Bound to Ruin","5"},
            {@"Nahiri, the Unforgiving","5"},
            {@"Nissa, Ascended Animist","7"},
            {@"The Eternal Wanderer","5"},
            {@"Tyvar, Jubilant Brawler","3"},
            {@"Vraska, Betrayal's Sting","6"},
            // March of the Machine
            {@"Archangel Elspeth", "4" },
            {@"Chandra, Hope's Beacon", "5" },
            {@"Wrenn and Realmbreaker", "4" },
            {@"Teferi Akosa of Zhalfir", "4" },
        };


        protected CardParserBase()
        {
        }

        protected string[] ExtractCardText(string text)
        {
            List<string> cardsText = new List<string>();

            //No decode because we need to do a special correction for XmlTextReader to be able to read HTML with javascript
            //To end because of multi part card
            //This a way to get rid for header and footer of HTML file
            string cutText = Parser.ExtractContent(text, Start, End, false, true);
            
            string[] tokens = cutText.Split(new[] { CardStartBlock }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 1)
            {
                throw new ParserException("Wrong parsing of card block extraction! Check HTML source");
            }
            //tokens[0] before card ignored
            //tokens[1+] real cards
            //last token contains also after card data
            for (int i = 1; i < tokens.Length; i++)
            {
                string tmpString = CardStartBlock + tokens[i];
                int index = tmpString.LastIndexOf(CardStartData, StringComparison.InvariantCulture);
                if (index < 0)
                {
                    continue;
                }

                if (i == tokens.Length - 1)
                {
                    int endpos = tmpString.LastIndexOf(LastCardEndData, StringComparison.InvariantCulture);
                    if (endpos >= 0)
                    {
                        tmpString = tmpString[..(endpos + LastCardEndData.Length)];
                    }
                }

                cardsText.Add(tmpString);
            }

            return cardsText.ToArray();
        }

        protected string SpecialXMLCorrection(string text)
        {
            StringBuilder sb = new StringBuilder();

            //Ensure unique root element because of fragment file
            sb.Append("<root>").Append(text).Append("</root>");

            //For XmlTextReader which doesn't support & caracter
            sb.Replace("&nbsp;", " ");
            sb.Replace("&lt;i&gt;", "<i>");
            sb.Replace("&lt;/i&gt;", "</i>");
            sb.Replace("&amp;", "&");
            sb.Replace("&", "&amp;");
            sb.Replace("<</i>", "&lt;</i>");
            sb.Replace("<</div>", "&lt;</div>");

            //Known XML issues in file
            sb.Replace("<div>", "<tr>");
            sb.Replace("</div\r\n", "</div>");
            sb.Replace("<i>", "");
            sb.Replace("</i>", "");
            sb.Replace("--->", "-->");
            
            return CardSpecificCorrection(sb.ToString());
        }

        private string CardSpecificCorrection(string text)
        {
            foreach (var kv in _missingLoyalty)
            {
                string regKey = $@"<div class=""value"">\s*{kv.Key}</div>";
                if (Regex.IsMatch(text, regKey))
                {
                    string start = "Loyalty:</div>";
                    const string end = @"</div>";
                    const string middle = @"<div class=""value"">";

                    int index = text.IndexOf(start);
                    if (index >= 0)
                    {
                        // Property is present but empty
                        int endIndex = text.IndexOf(end, index + start.Length);
                        if (endIndex >= 0)
                        {
                            string toreplace = text.Substring(index, endIndex - index + end.Length);
                            int subIndex = toreplace.IndexOf(middle);
                            if (subIndex >= 0)
                            {
                                string tocheck = toreplace.Substring(subIndex + middle.Length, toreplace.Length - subIndex - middle.Length - end.Length);
                                if (string.IsNullOrWhiteSpace(tocheck))
                                {
                                    //Confirm no property
                                    return text.Replace(toreplace, toreplace[..^end.Length] + kv.Value + end);
                                }
                            }
                            //string tocheck = text.Substring(index + 14, endIndex - index - 14);
                        }
                    }
                    else
                    {
                        // Property is not even present
                        start = @"<div id=""ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_rarityRow""";
                        string missingProperty = @"<div id=""ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_ptRow"" class=""row""><div class=""label"">Loyalty:</div><div class=""value"">{0}</div></div>";
                        index = text.IndexOf(start);
                        if (index >= 0)
                        {
                            return text.Replace(start, string.Format(missingProperty, kv.Value) + start);
                        }
                    }
                }
            }

            foreach (string property in _missingPropertyValue.Keys)
            {
                foreach (var kv in _missingPropertyValue[property])
                {
                    string key = kv.Key + " </div>";
                    string key2 = kv.Key + "</div>";
                    if (text.Contains(key) || text.Contains(key2))
                    {
                        //Missing the property
                        string start = property + "</div>";
                        const string end = @"</div>";
                        const string middle = @"<div class=""value"">";

                        int index = text.IndexOf(start);
                        if (index >= 0)
                        {
                            int endIndex = text.IndexOf(end, index + start.Length);
                            if (endIndex >= 0)
                            {
                                string toreplace = text.Substring(index, endIndex - index + end.Length);
                                int subIndex = toreplace.IndexOf(middle);
                                if (subIndex >= 0)
                                {
                                    string tocheck = toreplace.Substring(subIndex + middle.Length, toreplace.Length - subIndex - middle.Length - end.Length);
                                    if (string.IsNullOrWhiteSpace(tocheck))
                                    {
                                        //Confirm no property
                                        return text.Replace(toreplace, toreplace[..^end.Length] + kv.Value + end);
                                    }
                                }
                                //string tocheck = text.Substring(index + 14, endIndex - index - 14);
                            }
                        }
                    }
                }
            }

            return text;
        }
    }
}
