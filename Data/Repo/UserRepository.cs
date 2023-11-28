using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Data.Repo;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;

    public UserRepository(DataContext context)
    {
        _context = context;
    }


    public async Task<User> Authenticate(string userName, string passwordText)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == userName);

        if (user == null || user.PasswordKey == null)
            return null;

        if (!MatchPasswordHash(passwordText, user.Password, user.PasswordKey))
            return null;

        return user;
    }

    public void Register(string userName, string password)
    {
        using var hmac = new HMACSHA512();
        var passwordKey = hmac.Key;
        var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        var user = new User
        {
            Username = userName,
            Password = passwordHash,
            PasswordKey = passwordKey
        };

        _context.Users.Add(user);
    }

    public async Task<bool> UserAlreadyExists(string userName)
    {
        return await _context.Users.AnyAsync(x => x.Username == userName);
    }

    private bool MatchPasswordHash(string passwordText, byte[] password, byte[] passwordKey)
    {
        using var hmac = new HMACSHA512(passwordKey);
        var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(passwordText));

        for (int i=0; i<passwordHash.Length; i++)
        {
            if (passwordHash[i] != password[i])
                return false;
        }
        return true;
    }
}