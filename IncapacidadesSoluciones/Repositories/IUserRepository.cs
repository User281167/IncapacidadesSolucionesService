﻿using IncapacidadesSoluciones.Models;

namespace IncapacidadesSoluciones.Repositories
{
    public interface IUserRepository
    {
        Task<Boolean> UserExists(string email, string cedula);
        Task<User> GetUserByEmail(string email);
        Task<User> SignUp(string email, string pasword);
        Task<User> Update(User user);
    }
}