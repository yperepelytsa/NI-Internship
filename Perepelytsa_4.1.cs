using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Reflection;

namespace MyParserv2
{
    //interface for objects to be written to xml by SaveToXml()

    //interface for class factories to get objects from xml
    [AttributeUsage(AttributeTargets.Class)]
    public class RegexStr : Attribute
    {
        string value;
        public RegexStr(string value)
        {
            this.value = value;
        }
        public virtual string Value
        {
            get { return value; }
        }
    }
    static class MyParser
    {
        static string xmlloc = "E:\\xml";
        private readonly static Dictionary<string, Type> serializerTypes = new Dictionary<string, Type>
        {
            ["Person"] = typeof(Person),
            ["FootballClub"]=typeof(FootballClub),
            ["Company"]=typeof(Company),
            ["Animal"]=typeof(Animal),
            ["Plant"]=typeof(Plant),
            ["Car"]=typeof(Car),
            ["Bicycle"]=typeof(Bicycle),
            ["Newspaper"]=typeof(Newspaper)
        };
        public static List<Tuple<string,string>> GetPropertiesTypes()
        {
            List<Tuple<string, string>> properties = new List<Tuple<string, string>>();
            foreach (Type t in serializerTypes.Values)
            {
                foreach (var prop in t.GetProperties())
                {
                    
                    string name = prop.Name;
                    string type = prop.PropertyType.Name;
                    if (!properties.Exists(e=>e.Item1==name&&e.Item2==type))
                    {                        
                        properties.Add(new Tuple<string,string>(name,type));
                    }
                }
            }
            return properties;
        }
        public static void ShowProperties(List<Tuple<string, string>> properties)
        {
            Dictionary<string, int> count= new Dictionary<string, int>();
            foreach(Tuple<string,string> val in properties)
            {
                if (!count.ContainsKey(val.Item2))
                    count[val.Item2] = 1;
                else count[val.Item2]++;
            }
            foreach (string str in count.Keys)
            {
                int val = 0;
                count.TryGetValue(str, out val);
                Console.WriteLine(str + ": " + val);
            }
        }
        //defines folders to which xml files are written
        public static void SaveToXml(IParsee input)
        {
            Directory.CreateDirectory(xmlloc+"\\"+ input.GetType().Name);          
            String path = String.Format("{0}\\{1}\\{2}{3}.xml", xmlloc, input.GetType().Name, input.FileName, "");

            int i = 1;
            while (File.Exists(path))
            {
                path = String.Format("{0}\\{1}\\{2}{3}.xml", xmlloc, input.GetType().Name, input.FileName, "(" + i + ")");
                i++;
            }
            var stream = new FileStream(path,FileMode.Create);
            XmlWriter xw = XmlWriter.Create(stream);
            input.WriteXml(xw);
            xw.Flush();
            stream.Dispose();
        }

        public delegate IParsee[] Extract(String input);
        // main methods: MyParser.SaveToXml(XmlWriteable), puts object into its directory in xml
        // PersonFactory/FootballClubFactory/CompanyFactory.GetFromXml(path), get objects from xml,
        //implement generic interface MyXmlReader (only writeable objects can be read from xml)
        //xmlwriteable objects need to provide xml representation and file name
        public static void Main()
        {
         //   [RegexStr(@"Car \D+ \D+ \((\D+,\D+)\)")]
      //  [RegexStr(@"Bicycle \D+ \D+ \((\D+)\)")]
        String s = " Car Ford GT (mechanic, Joe) 123 Bicycle HTX Pro (Lewis) abc Mr. John Smith was born on 2001/06/15 abc123 Company Microsoft (IT, USA) gdf Animal Zebra (horse, Africa) gdf Mrs. Jane Smith was born on 1999/11/03 32 asd" +
                "abc 123 FC Dynamo (Kyiv, Ukraine, est. 1927) gf Plant Appletree (fruit, Europe, 2000 sm) dgd Newspaper Times, USA (daily, general) asdad FC Dynamo (Kyiv, Ukraine, est. 1927)";
            Extract[] ex = { Person.Extract, FootballClub.Extract, Company.Extract ,Animal.Extract,Plant.Extract,Car.Extract,Bicycle.Extract,Newspaper.Extract};
            List<IParsee> res = MyParser.ParseAll(s, ex);

            foreach (IParsee t in res)
            {
                Console.WriteLine(t.ToString());
            }


            Console.WriteLine();
            SaveToXml((Person)res[0]);
            SaveToXml((FootballClub)res[2]);
            SaveToXml((Company)res[4]);
            SaveToXml((Animal)res[5]);
            SaveToXml((Plant)res[6]);
            SaveToXml((Car)res[7]);
            SaveToXml((Bicycle)res[8]);
            SaveToXml((Newspaper)res[9]);
            Console.WriteLine("Saved to xml");
            var folders = new DirectoryInfo(xmlloc).EnumerateDirectories();

            var deserializedEntities = new List<IParsee>();

            foreach (var fldr in folders)
            {
               
                var files = fldr.EnumerateFiles();

                if (!serializerTypes.ContainsKey(fldr.Name))
                    throw new Exception("Missing serializer or bad folder name");

                Type serializerType = serializerTypes[fldr.Name];
                

                var serializer = new XmlSerializer(serializerType);

                foreach (var fl in files)
                {
                    using (var stream = new FileStream(fl.FullName, FileMode.Open))
                    {
                        try
                        {
                            deserializedEntities.Add((IParsee)serializer.Deserialize(stream));
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine($"Cannot deserialize file {fl.Name} in {fldr.Name} folder: {exc}");
                        }
                    }
                }
            }

            foreach (var en in deserializedEntities)
            {
                Console.WriteLine(en.ToString());
            }

        }


        public static List<IParsee> RemoveDuplicates(this List<IParsee> list)
        {
            IParsee[] objects = list.ToArray();
            QuicksortByType(objects, 0, objects.Length - 1);
            IParsee[][] objectsByType = SplitByType(objects);
            List<IParsee> flattened = new List<IParsee>();
            for (int i = 0; i < objectsByType.Length; i++)
            {
                objectsByType[i] = RemoveDuplicatesOneType(objectsByType[i]);
                flattened.AddRange(objectsByType[i]);
            }
            return flattened;
        }

        //only for arrays of single type
        private static IParsee[] RemoveDuplicatesOneType(IParsee[] source)
        {

            IParsee[] newsource = new IParsee[source.Length];
            Quicksort(source, 0, source.Length - 1);
            int i = 0, j = 0, count = 0;
            while (i < source.Length - 1)
            {
                if (source[i].CompareTo(source[i + 1]) == 0)
                {
                    newsource[i - count] = source[i];
                    j = i;
                    i++;
                    while (i < source.Length && source[j].CompareTo(source[i]) == 0)
                    {
                        i++;
                        count++;
                    }
                }
                else
                {
                    newsource[i - count] = source[i];
                    i++;
                }
            }
            if (i == source.Length - 1)
            {
                newsource[i - count] = source[i];
            }
            IParsee[] resArray = new IParsee[source.Length - count];
            Array.Copy(newsource, 0, resArray, 0, source.Length - count);
            return resArray;
        }
        //only for sorted arrays (by type)
        private static IParsee[][] SplitByType(IParsee[] source)
        {
            int count = 0;
            Type t = null;
            foreach (IParsee obj in source)
            {
                if (obj.GetType() != t)
                {
                    count++;
                    t = obj.GetType();
                }
            }

            IParsee[][] res = new IParsee[count][];
            int currTypeStart = 0, currTypeNumb = 0;
            t = source[0].GetType();
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i].GetType() != t)
                {
                    res[currTypeNumb] = new IParsee[i - currTypeStart];
                    Array.Copy(source, currTypeStart, res[currTypeNumb], 0, i - currTypeStart);
                    currTypeStart = i;
                    currTypeNumb++;
                    t = source[i].GetType();
                }
            }
            res[currTypeNumb] = new IParsee[source.Length - currTypeStart];
            Array.Copy(source, currTypeStart, res[currTypeNumb], 0, source.Length - currTypeStart);
            return res;
        }
        //sort array by type
        private static void QuicksortByType(IParsee[] source, int left, int right)
        {
            int i = left, j = right;
            IParsee pivot = source[(left + right) / 2];
            while (i <= j)
            {

                while (source[i].GetType().FullName.CompareTo(pivot.GetType().FullName) < 0)
                {
                    i++;
                }
                while (source[j].GetType().FullName.CompareTo(pivot.GetType().FullName) > 0)
                {
                    j--;
                }
                if (i <= j)
                {
                    IParsee tmp = source[i];
                    source[i] = source[j];
                    source[j] = tmp;

                    i++;
                    j--;
                }
            }

            if (left < j)
            {
                QuicksortByType(source, left, j);
            }
            if (i < right)
            {
                QuicksortByType(source, i, right);
            }
        }

        private static void Quicksort(IParsee[] source, int left, int right)
        {
            int i = left, j = right;
            IParsee pivot = source[(left + right) / 2];

            while (i <= j)
            {
                while (source[i].CompareTo(pivot) < 0)
                {

                    i++;
                }
                while (source[j].CompareTo(pivot) > 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    IParsee tmp = source[i];
                    source[i] = source[j];
                    source[j] = tmp;
                    i++;
                    j--;
                }
            }

            if (left < j)
            {
                Quicksort(source, left, j);
            }
            if (i < right)
            {
                Quicksort(source, i, right);
            }
        }

        public static List<IParsee> ParseAll(String input, Extract[] parsers)
        {
            int count = 0;
            IParsee[][] alloc = new IParsee[parsers.Length][];
            for (int i = 0; i < parsers.Length; i++)
            {
                if (parsers[i](input) != null)
                {
                    alloc[i] = parsers[i](input);
                    count += alloc[i].Length;
                }
            }

            List<IParsee> res = new List<IParsee>();
            for (int i = 0; i < parsers.Length; i++)
            {
                if (alloc[i] != null)
                {
                    res.AddRange(alloc[i]);
                    count -= alloc[i].Length;
                }
            }
            return res;
        }

        public static List<String> GetEntries(String input, String regexStr)
        {
            var regex = new Regex(regexStr);
            MatchCollection matches = regex.Matches(input);
            List<String> result = new List<String>();
            foreach (Match mat in matches)
            {
                result.Add(mat.Value);
            }
            return result;
        }
    }
    public interface IParsee : IComparable<IParsee>, IXmlExportable { }
    public interface IXmlExportable : IXmlSerializable
    {
        string FileName { get; }
    }
    [RegexStr(@"FC\D+\((\D+\d{4})\)")]
    public class FootballClub : IParsee
    {
        public String Name { get; private set; }
        public String Country { get; private set; }
        public String City { get; private set; }
        public DateTime Established { get; private set; }
    public virtual string FileName => $"{Name}({City})";

        public FootballClub() { }
    public FootballClub(String name, String country, String city, DateTime established)
        {
            this.Name = name;
            this.Country = country;
            this.City = city;
            this.Established = established;
        }
        override public String ToString()
        {
            return Name + " (" + Country + ", " + City + ", est." + Established.Year + ")";
        }

        public static FootballClub[] Extract(String input)
        {
            var type = typeof(FootballClub);
            RegexStr reg = (RegexStr)type.GetTypeInfo().GetCustomAttribute(typeof(RegexStr));
           
           List<String> clubs = MyParser.GetEntries(input, reg.Value);
            FootballClub[] result = new FootballClub[clubs.Count];
            int i = 0;

            foreach (String s in clubs)
            {
                String name;
                String country;
                String city;
                DateTime established;
                String[] parts = s.Split(' ');

                name = parts[1].Substring(0, parts[1].Length);
                city = parts[2].Substring(1, parts[2].Length - 2);
                country = parts[3].Substring(0, parts[3].Length - 1);

                established = DateTime.ParseExact(parts[5].Substring(0, 4), "yyyy", System.Globalization.CultureInfo.InvariantCulture);
                FootballClub res = new FootballClub(name, country, city, established);
                result[i++] = res;
            }
            return result;
        }

        public int CompareTo(IParsee obj)
        {
            if (obj == null) return 1;

            FootballClub other = obj as FootballClub;

            if (this.Country.CompareTo(other.Country) > 0)
                return 1;
            else if (this.Country.CompareTo(other.Country) < 0)
                return -1;
            else
            {
                if (this.City.CompareTo(other.City) > 0)
                {
                    return 1;

                }
                else if (this.City.CompareTo(other.City) < 0)
                {
                    return -1;
                }
                else
                {
                    if (this.Name.CompareTo(other.Name) == 0)
                    {
                        return DateTime.Compare(this.Established, other.Established);
                    }
                    else return this.Name.CompareTo(other.Name);
                }
            }
        }

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        reader.MoveToFirstAttribute();
        string est = reader.Value;
        reader.ReadToFollowing("Name");
        Name = reader.ReadElementContentAsString();
        Country = reader.ReadElementContentAsString();
        City = reader.ReadElementContentAsString();
        Established = DateTime.ParseExact(est, "yyyy", System.Globalization.CultureInfo.InvariantCulture);
    }

    public void WriteXml(XmlWriter writer)
    {
            writer.WriteStartDocument();
            writer.WriteStartElement("FootballClub");
            writer.WriteAttributeString("Established", String.Format("{0:yyyy}", Established));
        writer.WriteElementString("Name", Name.ToString());
        writer.WriteElementString("Country", Country.ToString());
        writer.WriteElementString("City", City.ToString());
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
        }
}

    public enum Gender { m, f };

    [RegexStr(@"(Mr\.|Mrs\.|Ms\.)\s(\w+)\s(\w+)\swas\sborn\son\s(\d{4}/\d\d/\d\d)")]
    public class Person : IParsee
    {

        public Gender Gender { get; private set; }
        public String FirstName { get; private set; }
        public String LastName { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public virtual string FileName => $"{FirstName}{LastName}";
        public Person() { }
        public Person(Gender gender, String firstName, String lastName, DateTime birth)
        {
            this.Gender = gender;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.DateOfBirth = birth;
        }
        override public String ToString()
        {
            return LastName + ", " + FirstName + " (" + Gender + ", " + GetAge(DateOfBirth) + ")";
        }

        public int GetAge(DateTime DateOfBirth)
        {
            var today = DateTime.Today;

            var a = (today.Year * 100 + today.Month) * 100 + today.Day;
            var b = (DateOfBirth.Year * 100 + DateOfBirth.Month) * 100 + DateOfBirth.Day;

            return (a - b) / 10000;
        }
        public int CompareTo(IParsee obj)
        {
            if (obj == null) return 1;

            Person other = obj as Person;

            if (this.Gender > other.Gender)
                return 1;
            else if (this.Gender < other.Gender)
                return -1;
            else
            {
                if (this.LastName.CompareTo(other.LastName) > 0)
                {
                    return 1;

                }
                else if (this.LastName.CompareTo(other.LastName) < 0)
                {
                    return -1;
                }
                else
                {
                    if (this.FirstName.CompareTo(other.FirstName) == 0)
                    {
                        return DateTime.Compare(this.DateOfBirth, other.DateOfBirth);
                    }
                    else return this.FirstName.CompareTo(other.FirstName);
                }
            }
        }






        public static Person[] Extract(String input)
        {
            var type = typeof(Person);
            RegexStr reg = (RegexStr)type.GetTypeInfo().GetCustomAttribute(typeof(RegexStr));
            List<String> people = MyParser.GetEntries(input, reg.Value);
            //Mr. John Smith was born on 2001/06/15
            Person[] result = new Person[people.Count];
            int i = 0;
            foreach (String s in people)
            {
                Gender Gender = Gender.m;
                String FirstName;
                String LastName;
                DateTime DateOfBirth;
                String[] parts = s.Split(' ');
                if (parts[0].Equals("Mrs.")) Gender = Gender.f;
                FirstName = parts[1];
                LastName = parts[2];
                DateOfBirth = DateTime.ParseExact(parts[6], "yyyy/MM/dd", System.Globalization.CultureInfo.InvariantCulture);
                Person res = new Person(Gender, FirstName, LastName, DateOfBirth);
                result[i++] = res;
            }
            return result;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {

                reader.MoveToFirstAttribute();
                string gender = reader.Value;
                reader.ReadToFollowing("FirstName");
                FirstName = reader.ReadElementContentAsString();
                LastName = reader.ReadElementContentAsString();
                string BirthDate = reader.ReadElementContentAsString();
                if (gender.ToCharArray()[0] == 'm')
                    Gender = Gender.m;
                else Gender = Gender.f;            
                DateOfBirth=DateTime.ParseExact(BirthDate, "MM.dd.yyyy", System.Globalization.CultureInfo.InvariantCulture);


        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("Person");
            writer.WriteAttributeString("gender", Gender.ToString());
            writer.WriteElementString("FirstName", FirstName.ToString());
            writer.WriteElementString("LastName",LastName.ToString());
            writer.WriteElementString("BirthDate", String.Format("{0:MM/dd/yyyy}", DateOfBirth));
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
        }
    }
    [RegexStr(@"Company \D+ \((\D+,\D+)\)")]
    public class Company : IParsee
    {

        public String Name { get; private set; }
        public String Segment { get; private set; }
        public String Location { get; private set; }

        public virtual string FileName => $"{Name}({Location})";
        public Company() { }
        public Company(String name, String segment, String location)
        {
            this.Name = name;
            this.Segment = segment;
            this.Location = location;
        }
        override public String ToString()
        {
            return Name + " (" + Segment + ", " + Location + ")";
        }
        public int CompareTo(IParsee obj)
        {
            if (obj == null) return 1;

            Company other = obj as Company;

            if (this.Segment.CompareTo(other.Segment) > 0)
            {
                return 1;

            }
            else if (this.Segment.CompareTo(other.Segment) < 0)
            {
                return -1;
            }
            else
            {
                if (this.Location.CompareTo(other.Location) > 0)
                {
                    return 1;

                }
                else if (this.Location.CompareTo(other.Location) < 0)
                {
                    return -1;
                }
                else
                {
                    return this.Name.CompareTo(other.Name);
                }
            }
        }

        public static Company[] Extract(String input)
        {
            var type = typeof(Company);
            RegexStr reg = (RegexStr)type.GetTypeInfo().GetCustomAttribute(typeof(RegexStr));
            List<String> companies = MyParser.GetEntries(input, reg.Value);
            //Company Microsoft (IT, USA)
            Company[] result = new Company[companies.Count];
            int i = 0;
            foreach (String s in companies)
            {
                String Name;
                String Segment;
                String Location;
                String[] parts = s.Split(' ');
                Name = parts[1];
                Segment = parts[2].Substring(1, parts[2].Length - 2);
                Location = parts[3].Substring(0, parts[3].Length - 1);
                Company res = new Company(Name, Segment, Location);
                result[i++] = res;
            }
            return result;
        }

        
    public XmlSchema GetSchema()
    {
        return null;
    }

    public virtual void ReadXml(XmlReader reader)
    {
                reader.MoveToElement();
                reader.MoveToFirstAttribute();
                Location = reader.Value;
                reader.ReadToFollowing("Name");
                Name = reader.ReadElementContentAsString();
                Segment = reader.ReadElementContentAsString();


        }

    public virtual void WriteXml(XmlWriter writer)
    {
            writer.WriteStartDocument();
            writer.WriteStartElement("Company");
            writer.WriteAttributeString("Location", Location.ToString());
            writer.WriteElementString("Name", Name.ToString());
            writer.WriteElementString("Segment", Segment.ToString());
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
        }

    }

    [RegexStr(@"Animal \D+ \((\D+,\D+)\)")]
    public class Animal : IParsee
    {

        public String Name { get; private set; }
        public String Species { get; private set; }
        public String Location { get; private set; }

        public virtual string FileName => $"{Name}({Location})";
        public Animal() { }
        public Animal(String name, String species, String location)
        {
            this.Name = name;
            this.Species = species;
            this.Location = location;
        }
        override public String ToString()
        {
            return Name + " (" + Species + ", " + Location + ")";
        }
        public int CompareTo(IParsee obj)
        {
            if (obj == null) return 1;

            Animal other = obj as Animal;

            if (this.Species.CompareTo(other.Species) > 0)
            {
                return 1;

            }
            else if (this.Species.CompareTo(other.Species) < 0)
            {
                return -1;
            }
            else
            {
                if (this.Location.CompareTo(other.Location) > 0)
                {
                    return 1;

                }
                else if (this.Location.CompareTo(other.Location) < 0)
                {
                    return -1;
                }
                else
                {
                    return this.Name.CompareTo(other.Name);
                }
            }
        }

        public static Animal[] Extract(String input)
        {
            var type = typeof(Animal);
            RegexStr reg = (RegexStr)type.GetTypeInfo().GetCustomAttribute(typeof(RegexStr));
            List<String> animals = MyParser.GetEntries(input, reg.Value);
            //Animal Zebra (equids, Africa)
            Animal[] result = new Animal[animals.Count];
            int i = 0;
            foreach (String s in animals)
            {
                String Name;
                String Species;
                String Location;
                String[] parts = s.Split(' ');
                Name = parts[1];
                Species = parts[2].Substring(1, parts[2].Length - 2);
                Location = parts[3].Substring(0, parts[3].Length - 1);
                Animal res = new Animal(Name, Species, Location);
                result[i++] = res;
            }
            return result;
        }


        public XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            reader.MoveToElement();
            reader.MoveToFirstAttribute();
            Location = reader.Value;
            reader.ReadToFollowing("Name");
            Name = reader.ReadElementContentAsString();
            Species = reader.ReadElementContentAsString();


        }

        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("Animal");
            writer.WriteAttributeString("Location", Location.ToString());
            writer.WriteElementString("Name", Name.ToString());
            writer.WriteElementString("Species", Species.ToString());
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
        }

    }

    [RegexStr(@"Plant \D+ \((\D+,\D+,.+ sm)\)")]
    public class Plant : IParsee
    {

        public String Name { get; private set; }
        public String Type { get; private set; }
        public int Height { get; private set; }
        public String Location { get; private set; }

        public virtual string FileName => $"{Name}({Type})";
        public Plant() { }
        public Plant(String name, String type, int height,String location)
        {
            this.Name = name;
            this.Type = type;
            this.Height = height;
            this.Location = location;
        }
        override public String ToString()
        {
            return Name + " (" + Type + ", " + Location + ", "+Height+" sm)";
        }
        public int CompareTo(IParsee obj)
        {
            if (obj == null) return 1;

            Plant other = obj as Plant;

            if (this.Name.CompareTo(other.Name) > 0)
            {
                return 1;

            }
            else if (this.Name.CompareTo(other.Name) < 0)
            {
                return -1;
            }
            else
            {
                if (this.Type.CompareTo(other.Type) > 0)
                {
                    return 1;

                }
                else if (this.Type.CompareTo(other.Type) < 0)
                {
                    return -1;
                }
                else
                {

                    if (this.Location.CompareTo(other.Location) > 0)
                    {
                        return 1;

                    }
                    else if (this.Location.CompareTo(other.Location) < 0)
                    {
                        return -1;
                    }
                    else
                    {
                        return this.Height.CompareTo(other.Height);
                    }
                }
            }
        }

        public static Plant[] Extract(String input)
        {
            var type = typeof(Plant);
            RegexStr reg = (RegexStr)type.GetTypeInfo().GetCustomAttribute(typeof(RegexStr));
            List<String> plants = MyParser.GetEntries(input, reg.Value);
            
            Plant[] result = new Plant[plants.Count];
            int i = 0;
            foreach (String s in plants)
            {
               
                String Name;
                String Type;
                String Location;
                int height;
                String[] parts = s.Split(' ');
                Name = parts[1];
                Type = parts[2].Substring(1, parts[2].Length - 2);
                Location = parts[3].Substring(0, parts[3].Length - 1);
                height = Int32.Parse(parts[4].Substring(0, parts[4].Length - 1));
                

                Plant res = new Plant(Name, Type,height, Location);
                result[i++] = res;
            }
            return result;
        }


        public XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            reader.MoveToElement();
            reader.MoveToFirstAttribute();
            Height = Int32.Parse(reader.Value);
            reader.ReadToFollowing("Name");
            Name = reader.ReadElementContentAsString();
           Type = reader.ReadElementContentAsString();
            Location = reader.ReadElementContentAsString();

        }

        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("Plant");
            writer.WriteAttributeString("height", Height.ToString());
            writer.WriteElementString("Name", Name);
            writer.WriteElementString("Type", Type);
            writer.WriteElementString("Location", Location);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
        }

    }

    [RegexStr(@"Car \D+ \D+ \((\D+,\D+)\)")]
    public class Car : IParsee
    {

        public String Model { get; private set; }
        public String Manufacturer { get; private set; }
        public String Transmission { get; private set; }
        public String Owner { get; private set; }

        public virtual string FileName => $"{Manufacturer} {Model}";
        public Car() { }
        public Car(String model, String manufacturer, String transmission, String owner)
        {
            this.Model = model;
            this.Manufacturer = manufacturer;
            this.Transmission = transmission;
            this.Owner = owner;
        }
        override public String ToString()
        {
            return Manufacturer+" "+ Model + " (" + Transmission + ", "  + Owner + ")";
        }
        public int CompareTo(IParsee obj)
        {
            if (obj == null) return 1;

            Car other = obj as Car;

            if (this.Manufacturer.CompareTo(other.Manufacturer) > 0)
            {
                return 1;

            }
            else if (this.Manufacturer.CompareTo(other.Manufacturer) < 0)
            {
                return -1;
            }
            else
            {
                if (this.Model.CompareTo(other.Model) > 0)
                {
                    return 1;

                }
                else if (this.Model.CompareTo(other.Model) < 0)
                {
                    return -1;
                }
                else
                {

                    if (this.Transmission.CompareTo(other.Transmission) > 0)
                    {
                        return 1;

                    }
                    else if (this.Transmission.CompareTo(other.Transmission) < 0)
                    {
                        return -1;
                    }
                    else
                    {
                        return this.Owner.CompareTo(other.Owner);
                    }
                }
            }
        }

        public static Car[] Extract(String input)
        {
            var type = typeof(Car);
            RegexStr reg = (RegexStr)type.GetTypeInfo().GetCustomAttribute(typeof(RegexStr));
            List<String> cars = MyParser.GetEntries(input, reg.Value);

            Car[] result = new Car[cars.Count];
            int i = 0;
            foreach (String s in cars)
            {

                String Model;
                String Manufacturer;
                String Transmission;
                String Owner;
                String[] parts = s.Split(' ');
                Manufacturer = parts[1];
                Model = parts[2];
                Transmission = parts[3].Substring(1, parts[3].Length - 2);
                Owner = parts[4].Substring(0, parts[4].Length - 1);


                Car res = new Car(Model, Manufacturer, Transmission, Owner);
                result[i++] = res;
            }
            return result;
        }


        public XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            reader.MoveToElement();
            reader.MoveToFirstAttribute();
            Owner = reader.Value;
            reader.ReadToFollowing("Model");
            Model = reader.ReadElementContentAsString();
            Manufacturer = reader.ReadElementContentAsString();
            Transmission = reader.ReadElementContentAsString();


        }

        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("Car");
            writer.WriteAttributeString("owner", Owner);
            writer.WriteElementString("Model", Model);
            writer.WriteElementString("Manufacturer", Manufacturer);
            writer.WriteElementString("Transmission", Transmission);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
        }

    }

    [RegexStr(@"Bicycle \D+ \D+ \((\D+)\)")]
    public class Bicycle : IParsee
    {

        public String Model { get; private set; }
        public String Manufacturer { get; private set; }
        public String Owner { get; private set; }

        public virtual string FileName => $"{Manufacturer} {Model}";
        public Bicycle() { }
        public Bicycle(String Model, String Manufacturer, String Owner)
        {
            this.Model = Model;
            this.Manufacturer = Manufacturer;
            this.Owner = Owner;
        }
        override public String ToString()
        {
            return Manufacturer + " " + Model + " (" + Owner + ")";
        }
        public int CompareTo(IParsee obj)
        {
            if (obj == null) return 1;

            Bicycle other = obj as Bicycle;

            if (this.Manufacturer.CompareTo(other.Manufacturer) > 0)
            {
                return 1;

            }
            else if (this.Manufacturer.CompareTo(other.Manufacturer) < 0)
            {
                return -1;
            }
            else
            {
                if (this.Model.CompareTo(other.Model) > 0)
                {
                    return 1;

                }
                else if (this.Model.CompareTo(other.Model) < 0)
                {
                    return -1;
                }
                else
                {
                    return this.Owner.CompareTo(other.Owner);                  
                }
            }
        }

        public static Bicycle[] Extract(String input)
        {
            var type = typeof(Bicycle);
            RegexStr reg = (RegexStr)type.GetTypeInfo().GetCustomAttribute(typeof(RegexStr));
            List<String> bicycles = MyParser.GetEntries(input, reg.Value);            
            Bicycle[] result = new Bicycle[bicycles.Count];
            int i = 0;
            foreach (String s in bicycles)
            {
                String Model;
                String Manufacturer;
                String Owner;
                String[] parts = s.Split(' ');
                Manufacturer = parts[1];
                Model = parts[2];
                Owner = parts[3].Substring(1, parts[3].Length - 2);

                Bicycle res = new Bicycle(Model, Manufacturer, Owner);
                result[i++] = res;
            }
            return result;
        }


        public XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            reader.MoveToElement();
            reader.MoveToFirstAttribute();
            Owner = reader.Value;
            reader.ReadToFollowing("Model");
            Model = reader.ReadElementContentAsString();
            Manufacturer = reader.ReadElementContentAsString();


        }

        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("Bicycle");
            writer.WriteAttributeString("owner", Owner);
            writer.WriteElementString("Model", Model);
            writer.WriteElementString("Manufacturer", Manufacturer);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
        }

    }

    [RegexStr(@"Newspaper \D+, \D+ \((\D+,\D+)\)")]
    public class Newspaper : IParsee
    {

        public String Name { get; private set; }
        public String Country { get; private set; }
        public String Frequency { get; private set; }
        public String Subject { get; private set; }

        public virtual string FileName => $"{Name}, {Country}";
        public Newspaper() { }
        public Newspaper(String Name, String Country, String Frequency, String Subject)
        {
            this.Name = Name;
            this.Country = Country;
            this.Frequency = Frequency;
            this.Subject = Subject;
        }
        override public String ToString()
        {
            return Name + ", " + Country + " (" + Frequency + ", " + Subject + ")";
        }
        public int CompareTo(IParsee obj)
        {
            if (obj == null) return 1;

            Newspaper other = obj as Newspaper;

            if (this.Name.CompareTo(other.Name) > 0)
            {
                return 1;

            }
            else if (this.Name.CompareTo(other.Name) < 0)
            {
                return -1;
            }
            else
            {
                if (this.Country.CompareTo(other.Country) > 0)
                {
                    return 1;

                }
                else if (this.Country.CompareTo(other.Country) < 0)
                {
                    return -1;
                }
                else
                {

                    if (this.Frequency.CompareTo(other.Frequency) > 0)
                    {
                        return 1;

                    }
                    else if (this.Frequency.CompareTo(other.Frequency) < 0)
                    {
                        return -1;
                    }
                    else
                    {
                        return this.Subject.CompareTo(other.Subject);
                    }
                }
            }
        }

        public static Newspaper[] Extract(String input)
        {
            var type = typeof(Newspaper);
            RegexStr reg = (RegexStr)type.GetTypeInfo().GetCustomAttribute(typeof(RegexStr));
            List<String> newspapers = MyParser.GetEntries(input, reg.Value);
            Newspaper[] result = new Newspaper[newspapers.Count];
            int i = 0;
            foreach (String s in newspapers)
            {
                String Name;
                String Country;
                String Frequency;
                String Subject;
                 String[] parts = s.Split(' ');
                Name = parts[1].Substring(0, parts[1].Length - 1);
                Country = parts[2];
                Frequency = parts[3].Substring(1, parts[3].Length - 2);
                Subject = parts[4].Substring(0, parts[4].Length - 1);
                Newspaper res = new Newspaper(Name, Country, Frequency, Subject);
                result[i++] = res;
            }
            return result;
        }


        public XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            reader.MoveToElement();
            reader.MoveToFirstAttribute();
            Subject = reader.Value;
            reader.ReadToFollowing("Name");
            Name = reader.ReadElementContentAsString();
            Country = reader.ReadElementContentAsString();
            Frequency = reader.ReadElementContentAsString();
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("Newspaper");
            writer.WriteAttributeString("subject", Subject);
            writer.WriteElementString("Name", Name);
            writer.WriteElementString("Country", Country);
            writer.WriteElementString("Frequency", Frequency);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
        }

    }
}