using System;
using System.Collections.Generic;

namespace TestApp.Model;

public partial class UserDetail
{
    public int? Id { get; set; }

    public string? Surname { get; set; }

    public string? Name { get; set; }

    public string? Midname { get; set; }

    public string? Login { get; set; }

    public DateOnly? Birthdate { get; set; }

    public string? UserType { get; set; }
}
