namespace ii.FirstPeace
{
    public class TrdProcessor
    {
        public class EntryType1
        {
            public string Name { get; set; }
            public int Offset { get; set; }
            public int Size { get; set; }
        }

        public class EntryType2
        {
            public int NameLength { get; set; }
            public string Name { get; set; }
            public short Unknown1 { get; set; }
            public short Unknown2 { get; set; }
            public short Unknown3 { get; set; }
            public short Unknown4 { get; set; }
            public short Unknown5 { get; set; }
            public short Unknown6 { get; set; }
            public short Unknown7 { get; set; }
        }

        public class EntryType3
        {
            public int NameLength { get; set; }
            public string Name { get; set; }
        }

        public void Process(string filename)
        {
            using var br = new BinaryReader(File.OpenRead(filename));

            // header ??
            // 2C0A0000 // file count?

            // entryType 0, 36 bytes
            // xx            xx
            // 346E 0100 3130333700BE6481C001560004942110A4007A00C0F26800588C211003000000 13422
            // 556E 0100 3130333800BE6481C001560004942110A4007A00C0F26800588C211003000000 21870
            // 756E 0100 3130333900BE6481C001560004942110A4007A00C0F26800588C211003000000 30062
            // 956E 0100 3130343000BE6481C001560004942110A4007A00C0F26800588C211003000000 38254
            // B56E 0100 3130343100BE6481C001560004942110A4007A00C0F26800588C211003000000 46446
            // D56E 0100 3130343200BE6481C001560004942110A4007A00C0F26800588C211003000000 54638
            // F56E 0100 3130343300BE6481C001560004942110A4007A00C0F26800588C211003000000 62830
            // 156F 0100 3130343400BE6481C001560004942110A4007A00C0F26800588C211003000000
            // 356F 0100 3130343500BE6481C001560004942110A4007A00C0F26800588C211003000000
            // 556F 0100 3130343600BE6481C001560004942110A4007A00C0F26800588C211003000000
            // 756F 0100 3130343700BE6481C001560004942110A4007A00C0F26800588C211003000000
            // 956F 0100 3130343800BE6481C001560004942110A4007A00C0F26800588C211003000000
            // B66F 0100 3130343900BE6481C001560004942110A4007A00C0F26800588C211003000000
            // D66F 0100 3130353000BE6481C001560004942110A4007A00C0F26800588C211003000000
            // F66F 0100 3130353100BE6481C001560004942110A4007A00C0F26800588C211003000000
            // 1670 0100 3130353200BE6481C001560004942110A4007A00C0F26800588C211003000000
            // 3670 0100 3130353300BE6481C001560004942110A4007A00C0F26800588C211003000000
            // 5670 0100 3130353400BE6481C001560004942110A4007A00C0F26800588C211003000000
            // 7670 0100 3130353500BE6481C001560004942110A4007A00C0F26800588C211003000000
            // 9670 0100 3130353600BE6481C001560004942110A4007A00C0F26800588C211003000000
            // B670 0100 3130353700BE6481C001560004942110A4007A00C0F26800588C211003000000
            // D670 0100 3130353800BE6481C001560004942110A4007A00C0F26800588C211003000000
            // F670 0100 3130380000BE6481C001560004942110A4007A00C0F26800588C211003000000
            // 2271 0100 3130390000BE6481C001560004942110A4007A00C0F26800588C211003000000
            // 4D71 0100 3131300000BE6481C001560004942110A4007A00C0F26800588C211003000000
            // 7871 0100 3131303500BE6481C001560004942110A4007A00C0F26800588C211003000000
            // 9A71 0100 3131303600BE6481C001560004942110A4007A00C0F26800588C211003000000
            // BB71 0100 3131303700BE6481C001560004942110A4007A00C0F26800588C211003000000

            br.BaseStream.Seek(4, SeekOrigin.Begin);
            for (var i = 0; i < 576; i++)
            {
                var entryNameBytes = br.ReadBytes(36);
                Console.WriteLine(Convert.ToHexString(entryNameBytes));
            }


            br.BaseStream.Seek(20740, SeekOrigin.Begin);

            var entryType1Count = 2028;

            var entryType1s = new List<EntryType1>();
            for (int i = 0; i < entryType1Count; i++)
            {
                var offset = br.ReadInt32();
                var entryNameBytes = br.ReadBytes(32);
                var nullIndex = Array.IndexOf(entryNameBytes, (byte)0);
                var result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nullIndex >= 0 ? nullIndex : entryNameBytes.Length);
                var entryType1 = new EntryType1
                {
                    Name = result,
                    Offset = offset,
                    Size = 0 // Size will be calculated later
                };
                entryType1s.Add(entryType1);
            }

            for (var i = 0; i < entryType1s.Count - 1; i++)
            {
                entryType1s[i].Size = entryType1s[i + 1].Offset - entryType1s[i].Offset;
            }

            var entryType2Count = 176;
            var entryType2s = new List<EntryType2>();
            for (int i = 0; i < entryType2Count; i++)
            {
                var nameLength = br.ReadInt32();
                var entryNameBytes = br.ReadBytes(nameLength);
                var result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                var unknown1 = br.ReadInt16();
                var unknown2 = br.ReadInt16(); // 2
                var unknown3 = br.ReadInt16(); // 0
                var unknown4 = br.ReadInt16(); // 2
                var unknown5 = br.ReadInt16(); // 0
                var unknown6 = br.ReadInt16(); // 0
                var unknown7 = br.ReadInt16(); // 128

                var entryType2 = new EntryType2
                {
                    NameLength = nameLength,
                    Name = result,
                    Unknown1 = unknown1,
                    Unknown2 = unknown2,
                    Unknown3 = unknown3,
                    Unknown4 = unknown4,
                    Unknown5 = unknown5,
                    Unknown6 = unknown6,
                    Unknown7 = unknown7,
                };
                entryType2s.Add(entryType2);
            }



            var entryType3s = new List<EntryType3>();
            {
                // on-map image
                // build-up image 0 (generic)
                // build-up image 1
                // build-up image 2
                // destroyed image 1
                // destroyed image 2
                // portrait image
                // 'option' image

                // sell
                // building
                // attack / destroyed
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



            foreach (var eo in entryType1s)
            {
                br.BaseStream.Seek(eo.Offset, SeekOrigin.Begin);
                var bytes = br.ReadBytes(eo.Size);
                var output = $@"D:\data\finalconflict\{eo.Name}";
                var d = Path.GetDirectoryName(output);
                Directory.CreateDirectory(d);
                File.WriteAllBytes(output, bytes);
            }

            entryType1Count = 2027;

        }
    }
}