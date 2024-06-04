using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace NUS_Orbital.Models
{
    public class Tag
    {
        public int tagId { get; set; }
        public String tag { get; set; }
        public bool filtered { get; set; }

        public Tag(int tagId, string tag, bool filtered)
        {
            this.tagId = tagId;
            this.tag = tag;
            this.filtered = filtered;
        }
    }
}
