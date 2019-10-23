using System;
using System.Collections.Generic;

namespace AnimeginationApi.Models
{
    public class UserNoteModel
    {
        public int userNoteId { get; set; }
        public string userid { get; set; }
        public string correspondenceType { get; set; }
        public string title { get; set; }
        public string note { get; set; }
        public DateTime created { get; set; }
    }
}
