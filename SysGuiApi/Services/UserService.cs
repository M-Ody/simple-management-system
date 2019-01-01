using Microsoft.EntityFrameworkCore;
using SysGuiApi.Control;
using SysGuiApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SysGuiApi.Services
{
    public class UserService
    {
        public async Task<ServiceResponse> GetUser(int id)
        {
            var response = new ServiceResponse();
            using (var db = new BrillDbContext())
            {
                var dbUser = await db.Users
                    .Where(x => x.Id == id)
                    .Select(x => new { id = x.Id, username = x.Username, x.PermissionGroup.GroupName })
                    .FirstOrDefaultAsync();

                if (dbUser != null)
                {
                    response.Ok(dbUser);
                }
                else
                {
                    response.BadRequest("O usuário não existe.");
                }
            }

            return response;
        }

        public async Task<ServiceResponse> GetUsers()
        {
            var response = new ServiceResponse();
            using (var db = new BrillDbContext())
            {
                var dbUsers = await db.Users
                    .Select(x => new { id = x.Id, username = x.Username, x.PermissionGroup.GroupName })
                    .ToListAsync();
                response.Ok(dbUsers);
            }

            return response;
        }

        public async Task<ServiceResponse> GetGroups()
        {
            var response = new ServiceResponse();
            using (var db = new BrillDbContext())
            {
                var dbGroups = await db.PermissionGroup
                    .Select(x => new { id = x.Id, name = x.GroupName })
                    .ToListAsync();
                response.Ok(dbGroups);
            }

            return response;
        }

        public async Task<ServiceResponse> CreateUser(string username, string password, int permission)
        {
            var response = new ServiceResponse();
            using (var db = new BrillDbContext())
            {
                var dbUser = await db.Users.FirstOrDefaultAsync(x => x.Username == username);
                if (dbUser == null) // O usuário não está registrado
                {
                    var permissionGroup = await db.PermissionGroup.FirstOrDefaultAsync(x => x.Id == permission);
                    if (permissionGroup != null) // A permissão existe
                    {
                        User user = new User
                        {
                            Username = username,
                            PermissionGroup = permissionGroup,
                            Hash = AuthService.Md5Hash(username, password)
                        };

                        await db.Users.AddAsync(user);
                        await db.SaveChangesAsync();

                        response.Ok(new { Id = user.Id });
                    }
                    else
                    {
                        response.BadRequest("O grupo de usuários não existe.");
                    }
                }
                else
                {
                    response.BadRequest("Usuário já registrado.");
                }
            }

            return response;
        }

        public async Task<ServiceResponse> EditUser(int id, string username, string password, int permission)
        {
            var response = new ServiceResponse();
            using (var db = new BrillDbContext())
            {
                var dbUser = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (dbUser != null) // O usuário ESTÁ registrado
                {
                    var permissionGroup = await db.PermissionGroup.FirstOrDefaultAsync(x => x.Id == permission);
                    if (permissionGroup != null) // A permissão existe
                    {
                        dbUser.Username = username;
                        dbUser.PermissionGroup = permissionGroup;

                        if (!string.IsNullOrEmpty(password)) //Vai mudar a senha também?
                        {
                            dbUser.Hash = AuthService.Md5Hash(username, password);
                        }

                        db.Users.Attach(dbUser);
                        db.Entry(dbUser).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        response.Ok(new { Id = dbUser.Id });
                    }
                    else
                    {
                        response.BadRequest("O grupo de usuários não existe.");
                    }
                }
                else
                {
                    response.BadRequest("O usuário não existe.");
                }
            }

            return response;
        }

        public async Task<ServiceResponse> DeleteUser(int id)
        {
            var response = new ServiceResponse();
            using (var db = new BrillDbContext())
            {
                var dbUser = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (dbUser != null) // O usuário ESTÁ registrado
                {
                    db.Users.Remove(dbUser);
                    db.SaveChanges();
                    response.Ok("Ok");
                }
                else
                {
                    response.BadRequest("O usuário não existe.");
                }
            }

            return response;
        }
    }
}
