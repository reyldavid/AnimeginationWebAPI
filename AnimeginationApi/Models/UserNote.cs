namespace AnimeginationApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserNote
    {
        public int UserNoteId { get; set; }

        public int UserId { get; set; }

        public string CorrespondenceType { get; set; }

        public string Title { get; set; }

        public string Note { get; set; }

        public DateTime Created { get; set; }

        //public virtual UserProfile UserProfile { get; set; }
    }
}
