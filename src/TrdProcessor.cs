using System.Data;
using System.Data.OleDb;
using System.Reflection.Emit;
using static ii.FirstPeace.TrdProcessor;

namespace ii.FirstPeace
{
    public class TrdProcessor
    {
        public class EntryInfo
        {
            public int Offset { get; set; }
            public int Id { get; set; }  // matches the ID from MDB.Character
            public byte[] Unknown { get; set; }
        }

        public class GraphicInfo
        {
            public string Name { get; set; }
            public int Offset { get; set; }
            public int Size { get; set; }
        }

        public class Entry
        {
            public int NameLength { get; set; }
            public string Name { get; set; }
        }

        public class EntryType3
        {
            public int NameLength { get; set; }
            public string Name { get; set; }
        }


        public class Character
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public int Speed { get; set; }
            public int CreateTime { get; set; }
            public int ClassID { get; set; }
            public int WpnRange { get; set; }
            public int Monetary { get; set; }
            public int Materials { get; set; }
            public int Energy { get; set; }
            public int BaseTiles { get; set; }
            public int Parent { get; set; }
            public int WpnStrength { get; set; }
            public int StripID { get; set; }
            public int Health { get; set; }
            public int Armor { get; set; }
            public int Sight { get; set; }
            public int BasicOptions { get; set; }
            public int UpgradeTo { get; set; }
            public int WpnID { get; set; }
            public int WpnRate { get; set; }
            public int WpnSpeed { get; set; }
            public int Alignment { get; set; }
            public int DependentOn1 { get; set; }
            public int OppositeRace { get; set; }
            public int BuildOn { get; set; }
            public int Styles { get; set; }
            public int DependentOn2 { get; set; }
            public int Level { get; set; }
            public int WpnFrameInc { get; set; }
        }

        public class CharacterBitmap
        {
            public int Id { get; set; }
            public string Filename { get; set; }
        }

        public class CharacterCanEnter
        {
            public int Id { get; set; }
            public int CanEnterId { get; set; }
            public int ReturnValue { get; set; }
            public int ReturnToId { get; set; }
            public int Parent { get; set; }
            public int ProductionValue { get; set; }
        }

        public class CharacterCategory
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class CharacterFrame
        {
            public int Id { get; set; }
            public int BitmapId { get; set; }
            public int Parent { get; set; }
            public int XPos { get; set; }
            public int YPos { get; set; }
            public int Frame { get; set; }
        }

        public class CharacterOption
        {
            public int Id { get; set; }
            public int CharacterId { get; set; }
            public int Parent { get; set; }
            public int SetId { get; set; }
            public int Pos { get; set; }
        }

        public class CharacterSound
        {
            public int Id { get; set; }
            public int SoundTypeId { get; set; }
            public int SoundId { get; set; }
            public int Parent { get; set; }
        }

        public class CharacterStrip
        {
            public int Direction { get; set; }
            public int XSize { get; set; }
            public int YSize { get; set; }
            public int Id { get; set; }
            public int Parent { get; set; }
            public int Type { get; set; }
            public int NumFrames { get; set; }
        }

        public class Identifier
        {
            public int Id { get; set; }
        }

        public class Level
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int XSize { get; set; }
            public int YSize { get; set; }
            public string FileName { get; set; }
        }

        public class Sound
        {
            public int Id { get; set; }
            public string FileName { get; set; }
        }

        public class Structure
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int BitmapId { get; set; }
            public int Parent { get; set; }
            public int BaseCategory { get; set; }
        }

        public class StructureCategory
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class Tile
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Parent { get; set; }
            public int Category { get; set; }
            public int MapColor { get; set; }
        }

        public class TileBitmap
        {
            public int Id { get; set; }
            public string FileName { get; set; }
        }

        public class TileCategory
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class TileSection
        {
            public int Id { get; set; }
            public int Parent { get; set; }
            public int BitmapId { get; set; }
            public int Pos { get; set; }
            public int SetId { get; set; }
        }

        public class Transition
        {
            public int Id { get; set; }
            public int TileId { get; set; }
            public int Parent { get; set; }
            public int TransitionId { get; set; }
        }

        public void Process(string filename)
        {
            var connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=F:\final\dom.mdb";
            var sql = "SELECT * FROM [Character]";

            var characters = new List<Character>();
            var characterBitmaps = new List<CharacterBitmap>();
            var characterCanEnters = new List<CharacterCanEnter>();
            var characterCategories = new List<CharacterCategory>();
            var characterFrames = new List<CharacterFrame>();
            var characterOptions = new List<CharacterOption>();
            var characterSounds = new List<CharacterSound>();
            var characterStrips = new List<CharacterStrip>();
            var identifiers = new List<Identifier>();
            var levels = new List<Level>();
            var sounds = new List<Sound>();
            var structures = new List<Structure>();
            var structureCategories = new List<StructureCategory>();
            var tiles = new List<Tile>();
            var tileBitmaps = new List<TileBitmap>();
            var tileCategories = new List<TileCategory>();
            var tileSections = new List<TileSection>();
            var transitions = new List<Transition>();

            using (var connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                try
                {
                    var command = new OleDbCommand(sql, connection);
                    using OleDbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var character = new Character();
                        character.ID = Convert.ToInt32(reader["ID"]);
                        character.Name = Convert.ToString(reader["Name"]);
                        character.Speed = Convert.ToInt32(reader["Speed"]);
                        character.CreateTime = Convert.ToInt32(reader["CreateTime"]);
                        character.ClassID = Convert.ToInt32(reader["ClassID"]);
                        character.WpnRange = Convert.ToInt32(reader["WpnRange"]);
                        character.Monetary = Convert.ToInt32(reader["Monetary"]);
                        character.Materials = Convert.ToInt32(reader["Materials"]);
                        character.Energy = Convert.ToInt32(reader["Energy"]);
                        character.BaseTiles = Convert.ToInt32(reader["BaseTiles"]);
                        character.Parent = Convert.ToInt32(reader["Parent"]);
                        character.WpnStrength = Convert.ToInt32(reader["WpnStrength"]);
                        character.StripID = Convert.ToInt32(reader["StripID"]);
                        character.Health = Convert.ToInt32(reader["Health"]);
                        character.Armor = Convert.ToInt32(reader["Armor"]);
                        character.Sight = Convert.ToInt32(reader["Sight"]);
                        character.BasicOptions = Convert.ToInt32(reader["BasicOptions"]);
                        character.UpgradeTo = Convert.ToInt32(reader["UpgradeTo"]);
                        character.WpnID = Convert.ToInt32(reader["WpnID"]);
                        character.WpnRate = Convert.ToInt32(reader["WpnRate"]);
                        character.WpnSpeed = Convert.ToInt32(reader["WpnSpeed"]);
                        character.Alignment = Convert.ToInt32(reader["Alignment"]);
                        character.DependentOn1 = Convert.ToInt32(reader["DependentOn1"]);
                        character.OppositeRace = Convert.ToInt32(reader["OppositeRace"]);
                        character.BuildOn = Convert.ToInt32(reader["BuildOn"]);
                        character.Styles = Convert.ToInt32(reader["Styles"]);
                        character.DependentOn2 = Convert.ToInt32(reader["DependentOn2"]);
                        character.Level = Convert.ToInt32(reader["Level"]);
                        character.WpnFrameInc = Convert.ToInt32(reader["WpnFrameInc"]);
                        characters.Add(character);
                    }
                    reader.Close();


                    command.CommandText = "SELECT * FROM [CharacterBitmaps]";
                    using OleDbDataReader reader2 = command.ExecuteReader();
                    while (reader2.Read())
                    {
                        var characterBitmap = new CharacterBitmap();
                        characterBitmap.Id = Convert.ToInt32(reader2["ID"]);
                        characterBitmap.Filename = Convert.ToString(reader2["FileName"]);
                        characterBitmaps.Add(characterBitmap);
                    }
                    reader2.Close();

                    command.CommandText = "SELECT * FROM [CharacterCanEnter]";
                    using OleDbDataReader reader3 = command.ExecuteReader();
                    while (reader3.Read())
                    {
                        var characterCanEnter = new CharacterCanEnter();
                        characterCanEnter.Id = Convert.ToInt32(reader3["ID"]);
                        characterCanEnter.CanEnterId = Convert.ToInt32(reader3["CanEnterId"]);
                        characterCanEnter.ReturnValue = Convert.ToInt32(reader3["ReturnValue"]);
                        characterCanEnter.ReturnToId = Convert.ToInt32(reader3["ReturnToId"]);
                        characterCanEnter.Parent = Convert.ToInt32(reader3["Parent"]);
                        characterCanEnter.ProductionValue = Convert.ToInt32(reader3["ProductionValue"]);
                        characterCanEnters.Add(characterCanEnter);
                    }
                    reader3.Close();

                    command.CommandText = "SELECT * FROM [CharacterCategory]";
                    using OleDbDataReader reader4 = command.ExecuteReader();
                    while (reader4.Read())
                    {
                        var characterCategory = new CharacterCategory();
                        characterCategory.Id = Convert.ToInt32(reader4["ID"]);
                        characterCategory.Name = Convert.ToString(reader4["Name"]);
                        characterCategories.Add(characterCategory);
                    }
                    reader4.Close();

                    command.CommandText = "SELECT * FROM [CharacterFrame]";
                    using OleDbDataReader reader5 = command.ExecuteReader();
                    while (reader5.Read())
                    {
                        var characterFrame = new CharacterFrame();
                        characterFrame.Id = Convert.ToInt32(reader5["ID"]);
                        characterFrame.BitmapId = Convert.ToInt32(reader5["BitmapId"]);
                        characterFrame.Parent = Convert.ToInt32(reader5["Parent"]);
                        characterFrame.XPos = Convert.ToInt32(reader5["XPos"]);
                        characterFrame.YPos = Convert.ToInt32(reader5["YPos"]);
                        characterFrame.Frame = Convert.ToInt32(reader5["Frame"]);
                        characterFrames.Add(characterFrame);
                    }
                    reader5.Close();

                    command.CommandText = "SELECT * FROM [CharacterOptions]";
                    using OleDbDataReader reader6 = command.ExecuteReader();
                    while (reader6.Read())
                    {
                        var characterOption = new CharacterOption();
                        characterOption.Id = Convert.ToInt32(reader6["ID"]);
                        characterOption.CharacterId = Convert.ToInt32(reader6["CharacterId"]);
                        characterOption.Parent = Convert.ToInt32(reader6["Parent"]);
                        characterOption.SetId = Convert.ToInt32(reader6["SetId"]);
                        characterOption.Pos = Convert.ToInt32(reader6["Pos"]);
                        characterOptions.Add(characterOption);
                    }
                    reader6.Close();

                    command.CommandText = "SELECT * FROM [CharacterSounds]";
                    using OleDbDataReader reader7 = command.ExecuteReader();
                    while (reader7.Read())
                    {
                        var characterSound = new CharacterSound();
                        characterSound.Id = Convert.ToInt32(reader7["ID"]);
                        characterSound.SoundTypeId = Convert.ToInt32(reader7["SoundTypeId"]);
                        characterSound.SoundId = Convert.ToInt32(reader7["SoundId"]);
                        characterSound.Parent = Convert.ToInt32(reader7["Parent"]);
                        characterSounds.Add(characterSound);
                    }
                    reader7.Close();

                    command.CommandText = "SELECT * FROM [CharacterStrip]";
                    using OleDbDataReader reader8 = command.ExecuteReader();
                    while (reader8.Read())
                    {
                        var characterStrip = new CharacterStrip();
                        characterStrip.Direction = Convert.ToInt32(reader8["Direction"]);
                        characterStrip.XSize = Convert.ToInt32(reader8["XSize"]);
                        characterStrip.YSize = Convert.ToInt32(reader8["YSize"]);
                        characterStrip.Id = Convert.ToInt32(reader8["Id"]);
                        characterStrip.Parent = Convert.ToInt32(reader8["Parent"]);
                        characterStrip.Type = Convert.ToInt32(reader8["Type"]);
                        characterStrip.NumFrames = Convert.ToInt32(reader8["NumFrames"]);
                        characterStrips.Add(characterStrip);
                    }
                    reader8.Close();

                    command.CommandText = "SELECT * FROM [Id]";
                    using OleDbDataReader reader9 = command.ExecuteReader();
                    while (reader9.Read())
                    {
                        var identifier = new Identifier();
                        identifier.Id = Convert.ToInt32(reader9["Id"]);
                        identifiers.Add(identifier);
                    }
                    reader9.Close();

                    command.CommandText = "SELECT * FROM [Levels]";
                    using OleDbDataReader reader10 = command.ExecuteReader();
                    while (reader10.Read())
                    {
                        var level = new Level();
                        level.Id = Convert.ToInt32(reader10["Id"]);
                        level.Name = Convert.ToString(reader10["Name"]);
                        level.XSize = Convert.ToInt32(reader10["XSize"]);
                        level.YSize = Convert.ToInt32(reader10["YSize"]);
                        level.FileName = Convert.ToString(reader10["FileName"]);
                        levels.Add(level);
                    }
                    reader10.Close();

                    command.CommandText = "SELECT * FROM [Sounds]";
                    using OleDbDataReader reader11 = command.ExecuteReader();
                    while (reader11.Read())
                    {
                        var sound = new Sound();
                        sound.Id = Convert.ToInt32(reader11["Id"]);
                        sound.FileName = Convert.ToString(reader11["FileName"]);
                        sounds.Add(sound);
                    }
                    reader11.Close();

                    command.CommandText = "SELECT * FROM [Structure]";
                    using OleDbDataReader reader12 = command.ExecuteReader();
                    while (reader12.Read())
                    {
                        var structure = new Structure();
                        structure.Id = Convert.ToInt32(reader12["Id"]);
                        structure.Name = Convert.ToString(reader12["Name"]);
                        structure.BitmapId = Convert.ToInt32(reader12["BitmapId"]);
                        structure.Parent = Convert.ToInt32(reader12["Parent"]);
                        structure.BaseCategory = Convert.ToInt32(reader12["BaseCategory"]);
                        structures.Add(structure);
                    }
                    reader12.Close();

                    command.CommandText = "SELECT * FROM [StructureCategory]";
                    using OleDbDataReader reader13 = command.ExecuteReader();
                    while (reader13.Read())
                    {
                        var structureCategory = new StructureCategory();
                        structureCategory.Id = Convert.ToInt32(reader13["Id"]);
                        structureCategory.Name = Convert.ToString(reader13["Name"]);
                        structureCategories.Add(structureCategory);
                    }
                    reader13.Close();

                    command.CommandText = "SELECT * FROM [Tile]";
                    using OleDbDataReader reader14 = command.ExecuteReader();
                    while (reader14.Read())
                    {
                        var tile = new Tile();
                        tile.Id = Convert.ToInt32(reader14["Id"]);
                        tile.Name = Convert.ToString(reader14["Name"]);
                        tile.Parent = Convert.ToInt32(reader14["Parent"]);
                        tile.Category = Convert.ToInt32(reader14["Category"]);
                        tile.MapColor = Convert.ToInt32(reader14["MapColor"]);
                        tiles.Add(tile);
                    }
                    reader14.Close();

                    command.CommandText = "SELECT * FROM [TileBitmaps]";
                    using OleDbDataReader reader15 = command.ExecuteReader();
                    while (reader15.Read())
                    {
                        var tileBitmap = new TileBitmap();
                        tileBitmap.Id = Convert.ToInt32(reader15["Id"]);
                        tileBitmap.FileName = Convert.ToString(reader15["FileName"]);
                        tileBitmaps.Add(tileBitmap);
                    }
                    reader15.Close();

                    command.CommandText = "SELECT * FROM [TileCategory]";
                    using OleDbDataReader reader16 = command.ExecuteReader();
                    while (reader16.Read())
                    {
                        var tileCategory = new TileCategory();
                        tileCategory.Id = Convert.ToInt32(reader16["Id"]);
                        tileCategory.Name = Convert.ToString(reader16["Name"]);
                        tileCategories.Add(tileCategory);
                    }
                    reader16.Close();

                    command.CommandText = "SELECT * FROM [TileSection]";
                    using OleDbDataReader reader17 = command.ExecuteReader();
                    while (reader17.Read())
                    {
                        var tileSection = new TileSection();
                        tileSection.Id = Convert.ToInt32(reader17["Id"]);
                        tileSection.Parent = Convert.ToInt32(reader17["Parent"]);
                        tileSection.BitmapId = Convert.ToInt32(reader17["BitmapId"]);
                        tileSection.Pos = Convert.ToInt32(reader17["Pos"]);
                        tileSection.SetId = Convert.ToInt32(reader17["SetId"]);
                        tileSections.Add(tileSection);
                    }
                    reader17.Close();

                    command.CommandText = "SELECT * FROM [Transition]";
                    using OleDbDataReader reader18 = command.ExecuteReader();
                    while (reader18.Read())
                    {
                        var transition = new Transition();
                        transition.Id = Convert.ToInt32(reader18["Id"]);
                        transition.TileId = Convert.ToInt32(reader18["TileId"]);
                        transition.Parent = Convert.ToInt32(reader18["Parent"]);
                        transition.TransitionId = Convert.ToInt32(reader18["TransitionId"]);
                        transitions.Add(transition);
                    }
                    reader18.Close();

                    //Transition


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }






            using var br = new BinaryReader(File.OpenRead(filename));

            // header, 0x2C0A0000
            br.BaseStream.Seek(4, SeekOrigin.Begin);


            // Read EntryInfos, the id and offset to the entry (and some unknown identical bytes)
            // we're now at 0x04
            const int EntryInfoCount = 576;
            var entryInfos = new List<EntryInfo>();
            for (var i = 0; i < EntryInfoCount; i++)
            {
                var offsetToEntry = br.ReadInt32();
                var idBytes = br.ReadBytes(4);
                var nullIndex = Array.IndexOf(idBytes, (byte)0);
                var id = System.Text.Encoding.ASCII.GetString(idBytes, 0, nullIndex >= 0 ? nullIndex : idBytes.Length);
                var entryNameBytes = br.ReadBytes(28);
                //Console.WriteLine($"[{i}] 0x{Convert.ToHexString(entryNameBytes)}"); // These seem to be identical in every entry
                entryInfos.Add(new EntryInfo
                {
                    Offset = offsetToEntry,
                    Id = Convert.ToInt32(id),
                    Unknown = entryNameBytes
                });
            }


            // Read GraphicInfos, the name and offset to the bitmaps themselves
            // we're now at 0x5104
            const int GraphicInfoCount = 2028;
            var graphicInfos = new List<GraphicInfo>();
            for (int i = 0; i < GraphicInfoCount; i++)
            {
                var offset = br.ReadInt32();
                var entryNameBytes = br.ReadBytes(32);
                var nullIndex = Array.IndexOf(entryNameBytes, (byte)0);
                var result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nullIndex >= 0 ? nullIndex : entryNameBytes.Length);
                graphicInfos.Add(new GraphicInfo
                {
                    Name = result,
                    Offset = offset,
                    Size = 0 // Size is not a stored field and is calculated later
                });
            }

            // Calculate the size
            for (var i = 0; i < graphicInfos.Count - 1; i++)
            {
                graphicInfos[i].Size = graphicInfos[i + 1].Offset - graphicInfos[i].Offset;
            }


            // Read the entry entries
            // we're now at 0x16E34
            var entryType2s = new List<Entry>();
            for (int i = 0; i < entryInfos.Count; i++)
            {
                br.BaseStream.Seek(entryInfos[i].Offset, SeekOrigin.Begin);

                var nameLength = br.ReadInt32();
                var entryNameBytes = br.ReadBytes(nameLength);
                var result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                var u1 = br.ReadInt16();

                /*
                // The number of bytes in this 'entry' entry varies - for a plain entry is another 14 bytes, but for a unit/building it's an 'EntryType3' entry
                // The format of entryType3 itself is reasonable except:
                //   There is a variable number of opening bytes (usually 0x62, 0x72 etc.)
                //   Not all supporting files are present in every entry
                var data = br.ReadBytes(14);
                var additionalBytesInThisEntry = 14;
                if (result == "Power Plant")
                {
                    additionalBytesInThisEntry = 0x62;
                }
                if (result == "Defensive Outpost")
                {
                    additionalBytesInThisEntry = 0x72;
                }
                if (result == "Marine")
                {
                    additionalBytesInThisEntry = 0xaa;
                }
                if (result == "Main Base")
                {
                    additionalBytesInThisEntry = 0x72;
                }
                if (result == "Diamond Mine")
                {
                    additionalBytesInThisEntry = 0x62;
                }
                if (result == "Mine Shaft")
                {
                    additionalBytesInThisEntry = 0x62;
                }
                if (result == "Metal Deposit")
                {
                    additionalBytesInThisEntry = 0x62;
                }

                if (additionalBytesInThisEntry != 14 || i > 170 && i < 190)
                {
                    var l = 0;
                    if (i + 1 < entryInfos.Count)
                    {
                        var o = entryInfos[i + 1].Offset;
                        var length = o - entryInfos[i].Offset;
                        l = length;
                    }

                    Console.WriteLine($"[{i}] 0x{Convert.ToHexString(data)} - name: " + result + " additional bytes: " + additionalBytesInThisEntry + " -> " + (l-result.Length));
                }
                */

                // for sounds u1 is the sound id

                Console.WriteLine($"[{i}] {result} {u1} (id {entryInfos[i].Id})");

                var entry = new Entry
                {
                    NameLength = nameLength,
                    Name = result,
                };
                entryType2s.Add(entry);
            }

            var entryType3s = new List<EntryType3>();
            {
                // on-map image                  -> followed by 0x16 bytes
                // build-up image 0 (generic)    -> followed by 0x8 bytes
                // build-up image 1              -> followed by 0x8 bytes
                // build-up image 2              -> followed by 0x16 bytes
                // destroyed image 1             -> followed by 0x8 bytes
                // destroyed image 2             -> followed by 0x16 bytes
                // portrait image                -> followed by 0x8 bytes
                // 'option' image                -> followed by 0x14 bytes

                // sell                          -> followed by 0x8 bytes
                // building                      -> followed by 0x8 bytes
                // attack / destroyed            -> followed by 0x8 bytes
                // explode


                // def outpost      90010000050000000A0000000000010000001800000002003A0C00000A000A00D00700000D00000003000000F4010000900100000000000000000000000000003F0D000003000200000004000000000000000200000001000000000000000000040000000800010000006000000060000000
                // main base        E80300000000000003000000000001000000180000000100000000000000000000000000000000000300000020030000E8030000000000000000000000000000571300000300000002000100A60B0000000001004B0B0000010000000000000004000000080001000000A0000000A0000000
                // diamond mine     0000000000000000010006000000010000001800000001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000030000000000000000000000030000000800010000006000000060000000
                // mine shaft       C8000000000000000300000000000000000018000000010000000000000000000000000000000000030000006400000064000000000000000000000000000000310A0000030000000000000000000000040000000800010000006000000060000000
                // metal deposit    0000000000000000010006000000010000001800000001000000000000000000000000000000000003000000000000000000000000000000000000000000000000000000030000000000000000000000020000000800010000006000000060000000                


                br.BaseStream.Seek(0x1858f, SeekOrigin.Begin);
                // powerplant 0x62, 98
                var nameLength = br.ReadInt32();
                var entryNameBytes = br.ReadBytes(nameLength);
                var result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                var entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // defensive outpost 0x72, 114
                br.BaseStream.Seek(0x187A4, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // marine 0xaa, 170
                br.BaseStream.Seek(0x189CF, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // harvest robot 0x10a
                br.BaseStream.Seek(0x19580, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // main base 0x72, 114
                br.BaseStream.Seek(0x19fc4, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // builder 0x11a, 282
                br.BaseStream.Seek(0x1a1c1, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // power plant 0x62, 98
                br.BaseStream.Seek(0x1af01, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // defensive outpost 0x72, 114
                br.BaseStream.Seek(0x1b0f3, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // main base 0x72, 114
                br.BaseStream.Seek(0x1b4c6, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // diamond mine 0x62, 98
                br.BaseStream.Seek(0x1b6ca, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // mine shaft 0x62, 98
                br.BaseStream.Seek(0x1b7e7, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // metal deposit 0x62, 98
                br.BaseStream.Seek(0x1bbcc, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // barracks 0x72, 114
                br.BaseStream.Seek(0x1be82, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // engineering 0x72, 114
                br.BaseStream.Seek(0x1c08e, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // engineering 0x72, 114
                br.BaseStream.Seek(0x1c288, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // space port 0x62, 98
                br.BaseStream.Seek(0x1c48a, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // warrior
                br.BaseStream.Seek(0x1c872, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // excavator
                br.BaseStream.Seek(0x1d190, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // laborer
                br.BaseStream.Seek(0x1daa3, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // pulse rifle
                br.BaseStream.Seek(0x1e8a3, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // plasma cannon
                br.BaseStream.Seek(0x1eac7, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // plated cretin
                br.BaseStream.Seek(0x1eced, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // bio-centre
                br.BaseStream.Seek(0x1f18d, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // hyperlink
                br.BaseStream.Seek(0x1f60c, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // hyperlink
                br.BaseStream.Seek(0x1f849, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // defense radar post
                br.BaseStream.Seek(0x1fa90, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // defense radar post
                br.BaseStream.Seek(0x1fcb4, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // defense perimeter
                br.BaseStream.Seek(0x1feb5, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

                // defense perimeter
                br.BaseStream.Seek(0x200b5, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);


                // etc.


                // mutated snowman
                br.BaseStream.Seek(0x2ac2b, SeekOrigin.Begin);
                nameLength = br.ReadInt32();
                entryNameBytes = br.ReadBytes(nameLength);
                result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);
            }


            {
                br.BaseStream.Seek(0x271C1, SeekOrigin.Begin);
                // entryType 4 - levels?
            }



            // Then read bitmaps, from 177,741 (0x2b64d)

            foreach (var eo in graphicInfos)
            {
                br.BaseStream.Seek(eo.Offset, SeekOrigin.Begin);
                var bytes = br.ReadBytes(eo.Size);
                var output = $@"D:\data\finalconflict\{eo.Name}";
                var d = Path.GetDirectoryName(output);
                Directory.CreateDirectory(d);
                File.WriteAllBytes(output, bytes);
            }
        }
    }
}