using System;
using System.Collections.Generic;

namespace TestApp.Model;

public partial class Usertype
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
