using SysGuiApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SysGuiApi.Control
{
    public class SignatureManager
    {
        private static SignatureManager _instance;

        public static SignatureManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SignatureManager();
                return _instance;
            }
        }

        private List<ValidSignature> validSignatures;

        public SignatureManager()
        {
            validSignatures = new List<ValidSignature>(10);
        }

        public void AddSignature(string signature, int userId, string username, List<int> permissions)
        {
            var previousSignature = validSignatures.FirstOrDefault(x => x.userId == userId);
            if (previousSignature != null)
            {
                previousSignature.signature = signature;
            }
            else
            {
                validSignatures.Add(new ValidSignature
                {
                    signature = signature,
                    userId = userId,
                    username = username,
                    permissions = permissions.ToList()
                });
            }
        }

        public void DeleteSignature(string signature)
        {
            int indexOf = validSignatures
                .IndexOf(validSignatures
                .FirstOrDefault(x => x.signature == signature));

            if (indexOf > 0)
                validSignatures.RemoveAt(indexOf);
        }

        public int GetUserId(string signature)
        {
            var user = validSignatures
                .FirstOrDefault(x => x.signature == signature);

            if (user != null)
                return user.userId;
            else
                return 0;
        }

        public string GetUsername(string signature)
        {
            var user = validSignatures
                .FirstOrDefault(x => x.signature == signature);

            if (user != null)
                return user.username;
            else
                return "UNKNOWN";
        }

        public bool HasSignature(string signature)
        {
            return validSignatures.Any(x => x.signature == signature);
        }

        public bool HasPermission(Permission permission, string signature)
        {
            return validSignatures.Any(x => x.signature == signature && x.permissions.Contains((int)permission));
        }

        public List<int> GetPermissions(string signature)
        {
            var user = validSignatures.FirstOrDefault(x => x.signature == signature);
            if (user != null)
                return user.permissions;
            else
            {
                return null;
            }
        }
    }

    public class ValidSignature
    {
        public string signature;
        public int userId;
        public string username;
        public List<int> permissions;
    }
}
