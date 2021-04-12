using System;
using System.Xml.Serialization;

namespace PasswordManagerClient.Models
{
   
    public abstract class Credential
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Category { get; set; }
        public string Notes { get; set; }
        public bool Favourite { get; set; }

        [XmlAttribute]
        public Guid credGUID { get; set; }

    }
}
