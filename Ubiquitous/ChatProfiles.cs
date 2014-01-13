using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml.Serialization;
using dotInterfaces;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.ComponentModel;

namespace Ubiquitous
{
    [Serializable]
    public class ChatProfiles : XmlSerializableBase<ChatProfiles>
    {
        TypeConverter fontConverter = TypeDescriptor.GetConverter(typeof(Font));
        TypeConverter colorConverter = TypeDescriptor.GetConverter(typeof(Color));
        TypeConverter locationConverter = TypeDescriptor.GetConverter(typeof(Point));
        TypeConverter sizeConverter = TypeDescriptor.GetConverter(typeof(Size));

        private const String newProfile = "New profile #";
        public ChatProfiles()
        {
            Profiles = new List<ChatProfile>();
        }
        [XmlElement("Profiles")]
        public List<ChatProfile> Profiles
        {
            get;
            set;
        }
        [XmlIgnore]
        private String[] ChatProperties = { 
                                                 "_ShortDescription",
                                                 "_LongDescription",
                                                 "_Game"
                                                 };
        public String CreateNew(ChatProfile template)
        {          
            int i = 1;
            while( Profiles.Any( p => p.Name.Equals(newProfile+i,StringComparison.CurrentCultureIgnoreCase))) 
                i++;
            var tmp = template.GetCopy();
            tmp.Name = newProfile + i;
            Profiles.Add(tmp);
            return tmp.Name;
        }
        public void WriteProfile(String name, object settings)
        {
            Profiles.RemoveAll(cp => cp.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            ChatProfile profile = new ChatProfile();
            profile.Name = name;           

            
            try
            {
                foreach (SettingsPropertyValue prop in ((Properties.Settings)settings).PropertyValues)
                {
                    if( prop != null && prop.Name != null )
                    {
                        if ( ChatProperties.Any(cp => prop.Name.EndsWith(cp, StringComparison.CurrentCultureIgnoreCase)) )
                        {
                        
                            profile.Strings.Add(new KeyValuePair<String, String>() {Key = prop.Name, Value = (String)prop.PropertyValue});
                        }
                        else if (prop.Property.PropertyType == typeof(Font))
                        {
                            profile.Fonts.Add(new KeyValuePair<String, String>() { Key = prop.Name, Value = fontConverter.ConvertToString(prop.PropertyValue) });
                        }
                        else if (prop.Property.PropertyType == typeof(Color))
                        {
                            profile.Colors.Add(new KeyValuePair<String, String>() { Key = prop.Name, Value = colorConverter.ConvertToString(prop.PropertyValue) });
                        }
                        else if (prop.Property.PropertyType == typeof(Size))
                        {
                            profile.Sizes.Add(new KeyValuePair<String, String>() { Key = prop.Name, Value = sizeConverter.ConvertToString(prop.PropertyValue) });
                        }
                        else if (prop.Property.PropertyType == typeof(Point))
                        {
                            profile.Locations.Add(new KeyValuePair<String, String>() { Key = prop.Name, Value = locationConverter.ConvertToString(prop.PropertyValue) });
                        }


                    }
                }

                Profiles.Add(profile);
            }
            catch (Exception e)
            {
                Debug.Print("WriteProfile: {0}", e.Message);
            }
        }

        public void ReadProfile(String name, ref object settings)
        {
            
            if (Profiles == null)
                return;

            ChatProfile profile = Profiles.FirstOrDefault(p => p.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));

            if( profile.Strings == null )
                return;
            try
            {
                foreach (var pair in profile.Strings)
                {
                    ((Properties.Settings)settings)[pair.Key] = pair.Value;
                }
                foreach (var pair in profile.Fonts)
                {
                    ((Properties.Settings)settings)[pair.Key] = (Font)fontConverter.ConvertFromString(pair.Value);
                }
                foreach (var pair in profile.Colors)
                {
                    ((Properties.Settings)settings)[pair.Key] = (Color)colorConverter.ConvertFromString(pair.Value);
                }
                foreach (var pair in profile.Sizes)
                {
                    ((Properties.Settings)settings)[pair.Key] = (Size)sizeConverter.ConvertFromString(pair.Value);
                }
                foreach (var pair in profile.Locations)
                {
                    ((Properties.Settings)settings)[pair.Key] = (Point)locationConverter.ConvertFromString(pair.Value);
                }
            }
            catch (Exception e)
            {
                Debug.Print("ReadProfile: {0}", e.Message);
            }

        }
    }
    [Serializable]
    public class ChatProfile :XmlSerializableBase<ChatProfile>
    {
        public ChatProfile()
        {
            Strings = new List<KeyValuePair<string, string>>();
            Fonts = new List<KeyValuePair<string, string>>();
            Colors = new List<KeyValuePair<string, string>>();
            Sizes = new List<KeyValuePair<string, string>>();
            Locations = new List<KeyValuePair<string, string>>();
        }
        [XmlAttribute("Name")]
        public String Name
        {
            get;
            set;
        }
        [XmlElement("Strings")]
        public List<KeyValuePair<String, String>> Strings
        {
            get;
            set;
        }
        [XmlElement("Fonts")]
        public List<KeyValuePair<String, String>> Fonts
        {
            get;
            set;
        }
        [XmlElement("Colors")]
        public List<KeyValuePair<String, String>> Colors
        {
            get;
            set;
        }
        [XmlElement("Sizes")]
        public List<KeyValuePair<String, String>> Sizes
        {
            get;
            set;
        }
        [XmlElement("Locations")]
        public List<KeyValuePair<String, String>> Locations
        {
            get;
            set;
        }
        
        public ChatProfile GetCopy()
        {
            return new ChatProfile()
            {
                Name = Name,
                Strings = this.Strings.ToList(),
                Fonts = this.Fonts.ToList(),
                Colors = this.Colors.ToList(),
                Sizes = this.Sizes.ToList(),
                Locations = this.Locations.ToList()
            };
        }
    }
    [Serializable]
    public struct KeyValuePair<K, V>
    {
        [XmlAttribute("Name")]
        public K Key { get; set; }
        [XmlAttribute("Value")]
        public V Value { get; set; }

    }
}
