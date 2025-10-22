using Bogus;
using GymManagement_Members_Microservice.Context;
using GymManagement_Members_Microservice.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_Members_Microservice.Data
{
    public class MemberSeeder(int _numberOfRows = 100, int _randomSeed = 42)
    {
        public async Task SeedAsync(ApplicationDbContext db)
        {
            //The memberships database wont be seeded with the memberships of these members...
            //Il leave it like this so i didnt do this for nothing

            await db.Database.MigrateAsync();

            int existingCount = await db.Member.CountAsync();
            int toCreate = _numberOfRows - existingCount;
            if (toCreate <= 0) return;

            HashSet<string> existingEmails = await db.Member.Select(m => m.Email).ToHashSetAsync();
            HashSet<string> existingIbans = await db.Member.Select(m => m.IBAN).ToHashSetAsync();

            Randomizer.Seed = new Random(_randomSeed);
            var faker = new Faker("pt_PT");

            var members = new List<Member>(toCreate);
            int guard = 0;

            while (members.Count < toCreate && guard < toCreate * 10)
            {
                guard++;

                string first = faker.Name.FirstName();
                string last = faker.Name.LastName();
                string email = $"{first}.{last}{faker.Random.Int(1, 9999)}@example.com".ToLowerInvariant();
                string phone = $"+3519{faker.Random.Number(10000000, 99999999)}";
                string iban = GeneratePortugueseIban(faker);

                if (existingEmails.Contains(email) || members.Any(m => m.Email == email)) continue;
                if (existingIbans.Contains(iban) || members.Any(m => m.IBAN == iban)) continue;

                members.Add(new Member
                {
                    Name = $"{first} {last}",
                    Phone = phone,
                    Email = email,
                    IBAN = iban,
                    RegisterDay = DateOnly.FromDateTime(DateTime.UtcNow),
                    ActiveUntilDay = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1))
                });
            }

            if (members.Count > 0)
            {
                await db.Member.AddRangeAsync(members);
                await db.SaveChangesAsync();
            }
        }

        private string GeneratePortugueseIban(Faker faker)
        {
            int check = faker.Random.Int(10, 99);
            string tail = string.Concat(Enumerable.Range(0, 21).Select(_ => faker.Random.Number(0, 9).ToString()));
            return $"PT{check}{tail}";
        }

    }
}
