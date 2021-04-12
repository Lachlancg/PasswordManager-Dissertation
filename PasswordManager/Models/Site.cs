using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;


namespace PasswordManagerClient.Models
{
    public class Site:Credential
    {
	    public Site()
        {
            base.credGUID = Guid.NewGuid();
            this.Domain = null;
            base.Name = null;
            base.Username = null;
            base.Password = null;
            base.Favourite = false;
        }
        public Site(string domain, string name, string username, string password, bool favourite)
        {
            this.Domain = domain;
            base.Name = name;
            base.Username = username;
            base.Password = password;
            base.Favourite = favourite;
            base.credGUID = Guid.Empty;
        }
        public Site(string domain, string name, string username, string password, bool favourite, Guid credguid)
        {
            this.Domain = domain;
            base.Name = name;
            base.Username = username;
            base.Password = password;
            base.Favourite = favourite;
            base.credGUID = credguid;
        }
        public Site(Guid credguid, string domain, string name, string username, string password, string category , string notes, bool favourite)
        {
            this.Domain = domain;
            base.Name = name;
            base.Username = username;
            base.Password = password;
            base.Favourite = favourite;
            base.Category = category;
            base.Notes = notes;
            base.credGUID = credguid;
        }
        

        public string Domain { get; set; }

    }
}
