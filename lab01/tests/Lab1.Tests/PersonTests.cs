using Xunit;
using Lab1.Models;

namespace Lab1.Tests;

public class PersonTests
{
    private Person CreateSamplePerson()
    {
        return new Person("Ivan", "Ivanov", 25, "secret123", "p1", "+7-999-000-00-00", "ivan@example.com");
    }

    [Fact]
    public void FullName_ReturnsFirstNameAndLastName()
    {
        var person = CreateSamplePerson();
        Assert.Equal("Ivan Ivanov", person.FullName);
    }

    [Theory]
    [InlineData(18, true)]
    [InlineData(25, true)]
    [InlineData(17, false)]
    [InlineData(0, false)]
    public void IsAdult_ReturnsCorrectValue(int age, bool expected)
    {
        var person = new Person("Test", "User", age, "pwd", "id", "+7-000", "test@test.com");
        Assert.Equal(expected, person.IsAdult);
    }

    [Fact]
    public void Email_ValidEmail_SetsSuccessfully()
    {
        var person = CreateSamplePerson();
        person.Email = "new.email@domain.com";
        Assert.Equal("new.email@domain.com", person.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("no-at-symbol")]
    public void Email_InvalidEmail_ThrowsArgumentException(string invalidEmail)
    {
        var person = CreateSamplePerson();
        Assert.Throws<ArgumentException>(() => person.Email = invalidEmail);
    }

    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var person = new Person("Ivan", "Ivanov", 25, "secret", "p1", "+7-999", "ivan@test.com");

        Assert.Equal("Ivan", person.FirstName);
        Assert.Equal("Ivanov", person.LastName);
        Assert.Equal(25, person.Age);
        Assert.Equal("secret", person.Password);
    }
}
