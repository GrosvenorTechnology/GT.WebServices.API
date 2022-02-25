using System.Collections.Generic;
using System.Xml.Serialization;

namespace GT.WebServices.API.Domain
{
    [XmlRoot("dataCollection")]
    public class DataCollection
    {
        [XmlElement("dataCollectionFlow")]
        public List<DataCollectionFlow> Flows { get; set; }

    }

    public class DataCollectionFlow
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlElement("config")]
        public Config Config { get; set; }

        [XmlArray("levels")]
        [XmlArrayItem("level")]
        public List<Level> Levels { get; set; }
    }

    public class Config
    {
        [XmlElement("button")]
        public Button Button { get; set; }
    }

    public class Button
    {
        [XmlElement("label")]
        public string Label { get; set; }
        [XmlElement("group")]
        public string Group { get; set; }
        [XmlElement("reqRole")]
        public string ReqRole { get; set; }
    }

    public class Level
    {
        [XmlElement("title")]
        public string Title { get; set; }
        [XmlArray("items")]
        [XmlArrayItem("item")]
        public List<LevelItem> Items { get; set; }
    }

    public class LevelItem
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlElement("label")]
        public string Label { get; set; }

        //Levels can be nested, but can nest only a single level
        [XmlElement("levels")]
        public SubLevels Levels { get; set; } 
    }

    //If anyone knows a better way to express this, please let us know!
    public class SubLevels
    {
        [XmlElement("level")]
        public SubLevel Level { get; set; } = new ();
    }

    public class SubLevel
    {
        [XmlArray("items")]
        [XmlArrayItem("item")]
        public List<LevelItemBase> Items { get; set; }
    }





}


