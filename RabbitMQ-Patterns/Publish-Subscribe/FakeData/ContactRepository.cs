using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;

namespace FakeData;

public static class ContactRepository
{
    private static int ContactId = 0;
    public static Contact GetContact()
    {
      var userFaker = new Faker<Contact>()
            .RuleFor(u => u.Id, _ => ContactId++)
            .RuleFor(u => u.Name, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Country, f => f.Address.Country())
            .RuleFor(u => u.Phone, f => f.Phone.PhoneNumberFormat());

      var users = userFaker.Generate(1);
      return users.First();
    }
}
