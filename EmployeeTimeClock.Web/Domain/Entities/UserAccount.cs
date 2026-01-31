namespace EmployeeTimeClock.Web.Domain.Entities;

public class UserAccount
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Role { get; set; } = "Employee"; // Employee | Admin
}
