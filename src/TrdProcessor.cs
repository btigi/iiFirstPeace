namespace ii.FirstPeace
{
    public class TrdProcessor
    {
        public class EntryInfo
        {
            public int Offset { get; set; }
            public int Id { get; set; }
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

        public void Process(string filename)
        {
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