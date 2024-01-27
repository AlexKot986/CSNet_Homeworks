﻿
namespace H5_EntityHomeWork.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Message> ToMessages { get; set;}
        public virtual ICollection<Message> FromMessages { get; set;}
    }
}
