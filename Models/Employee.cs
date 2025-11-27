using System;
using System.Collections.Generic;

namespace PublishingHouseApp.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string Firstname { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Position { get; set; } = null!;

    public DateTime HireDate { get; set; }
}
