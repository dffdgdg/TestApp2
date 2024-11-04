using System;
using System.Collections.Generic;

namespace TestApp.Model;

public partial class User
{
    public int Id { get; set; }

    public string Surname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Midname { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateOnly Birthdate { get; set; }

    public int Usertype { get; set; }

    public virtual ICollection<Userresult> Userresults { get; set; } = new List<Userresult>();

    public virtual Usertype UsertypeNavigation { get; set; } = null!;
}
