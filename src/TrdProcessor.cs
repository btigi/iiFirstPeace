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



            {
                var entryType3s = new List<EntryType3>();

                //br.BaseStream.Seek(0x1A0, SeekOrigin.Begin);
                var nameLength = br.ReadInt32();
                var entryNameBytes = br.ReadBytes(nameLength);
                var result = System.Text.Encoding.ASCII.GetString(entryNameBytes, 0, nameLength);
                var entryType3 = new EntryType3
                {
                    NameLength = nameLength,
                    Name = result
                };
                entryType3s.Add(entryType3);

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