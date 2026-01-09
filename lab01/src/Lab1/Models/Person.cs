using System.Text.Json.Serialization;

namespace Lab1.Models;

public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }

    public string FullName => $"{FirstName} {LastName}";
    public bool IsAdult => Age >= 18;

    [JsonIgnore]
    public string Password { get; set; }

    [JsonPropertyName("personId")]
    public string Id { get; set; }

    [JsonInclude]
    private DateTime _birthDate;
    public DateTime BirthDate
    {
        get => _birthDate;
        set => _birthDate = value;
    }

    private string _email;
    public string Email
    {
        get => _email;
        set
        {
            if (string.IsNullOrEmpty(value) || !value.Contains('@'))
                throw new ArgumentException("Email must contain '@'");
            _email = value;
        }
    }

    [JsonPropertyName("phone")]
    public string PhoneNumber { get; set; }

    public Person(string fname, string lname, int age, string pswd, string id, string phone, string email)
    {
        FirstName = fname;
        LastName = lname;
        Age = age;
        Password = pswd;
        Id = id;
        PhoneNumber = phone;
        Email = email;
    }

    public Person() { }
}