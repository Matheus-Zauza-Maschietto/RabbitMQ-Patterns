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
            .RuleFor(u => u.Phone, f => f.Phone.PhoneNumberFormat())
            .RuleFor(u => u.ContactType, _ => GetRoutingKey().ToString());

      var users = userFaker.Generate(1);
      
      return users.First();
    }

    static ContactType GetRoutingKey(){
        var random = new Random();
        int randomNumber = random.Next(0, 3);
        if(randomNumber == 0)
            return ContactType.CLIENTE;
        else if(randomNumber == 1)
            return ContactType.CONTATO;
        else
            return ContactType.FORNECEDOR;
    }

}
