using Microsoft.EntityFrameworkCore;
using SysGuiApi.Control;
using SysGuiApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SysGuiApi.Services
{
    public class AuthService
    {
        public async Task<ServiceResponse> Login(string username, string password)
        {
            var response = new ServiceResponse();
            User user;
            using (var db = new BrillDbContext())
            {
                user = await db.Users.FirstOrDefaultAsync(x => x.Username == username);
            }

            if (user != null)
            {
                var inputHash = Md5Hash(username, password);

                if (user.Hash == inputHash)
                {
                    List<int> permissions;
                    using (var db = new BrillDbContext())
                    {
                        permissions = db.Users
                            .Where(x => x.Id == user.Id)
                            .Select(x => x.PermissionGroup.Permissions)
                            .First()
                            .ToList();
                    }

                    var signature = ComputeSecuritySignature(user.Id, username, permissions);
                    response.Ok(new { securitySignature = signature, username = username });
                }
                else
                {
                    response.BadRequest("Senha incorreta");
                }
            }
            else
            {
                response.BadRequest("Usuário não encontrado");
            }

            return response;
        }

        public async Task<ServiceResponse> Logout(string signature)
        {
            var response = new ServiceResponse();

            SignatureManager.Instance.DeleteSignature(signature);

            response.Ok(new { status = "Ok" });

            return response;
        }

        public bool HasPermission(Permission permission, string signature)
        {
            return SignatureManager.Instance.HasPermission(permission, signature);
        }

        public static string Md5Hash(string username, string password)
        {
            var md5 = MD5.Create();
            var byteHash = md5.ComputeHash(Encoding.ASCII.GetBytes(username + ":" + password));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < byteHash.Length; i++)
            {
                sb.Append(byteHash[i].ToString("X2"));
            }

            return sb.ToString();
        }

        private string ComputeSecuritySignature(int userId, string username, List<int> permissions)
        {
            string signature = Guid.NewGuid().ToString();
            SignatureManager.Instance.AddSignature(signature, userId, username, permissions);

            return signature;
        }
    }
}
