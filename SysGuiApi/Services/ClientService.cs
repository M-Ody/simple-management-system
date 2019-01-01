using Microsoft.EntityFrameworkCore;
using SysGuiApi.Control;
using SysGuiApi.Models;
using SysGuiApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SysGuiApi.Services
{
    public class ClientService
    {
        public async Task<ServiceResponse> GetClient(int id)
        {
            var response = new ServiceResponse();
            using (var db = new BrillDbContext())
            {
                var dbClient = await db.Client
                    .Where(x => x.Id == id)
                    .Select(x => new
                    {
                        x.Name,
                        x.Cpf,
                        x.Address,
                        x.Phone,
                        x.City
                    })
                    .FirstOrDefaultAsync();

                if (dbClient != null)
                {
                    response.Ok(dbClient);
                }
                else
                {
                    response.BadRequest("O cliente não existe.");
                }
            }

            return response;
        }

        public async Task<ServiceResponse> GetSearchResults(string str)
        {
            if (char.IsDigit(str[0]))
            {
                return await SearchForCpf(str);
            }
            else
            {
                return await SearchForName(str);
            }
        }

        public async Task<ServiceResponse> GetClientByName(string name)
        {
            var response = new ServiceResponse();
            using (var db = new BrillDbContext())
            {
                var dbClient = await db.Client
                    .Select(x => new
                    {
                        id = x.Id,
                        name = x.Name,
                        cpf = x.Cpf,
                        address = x.Address,
                        phone = x.Phone,
                        city = x.City.Name,
                    })
                    .FirstOrDefaultAsync(x => x.name == name);

                if (dbClient != null)
                    response.Ok(dbClient);
                else
                    response.BadRequest("Cliente não existe.");
            }
            return response;
        }

        public async Task<ServiceResponse> GetClientByCpf(string cpf)
        {
            var response = new ServiceResponse();
            using (var db = new BrillDbContext())
            {
                var dbClient = await db.Client
                    .Select(x => new
                    {
                        id = x.Id,
                        name = x.Name,
                        cpf = x.Cpf,
                        address = x.Address,
                        phone = x.Phone,
                        city = x.City.Name,
                    })
                    .FirstOrDefaultAsync(x => x.cpf == Client.UnmaskCpf(cpf));

                if (dbClient != null)
                {
                    response.Ok(dbClient);
                }
                else
                {
                    response.BadRequest("Cliente não existe.");
                }
            }
            return response;
        }

        public async Task<ServiceResponse> GetFullClientByName(string name)
        {
            var response = new ServiceResponse();
            using (var db = new BrillDbContext())
            {
                var dbOrder = await db.Order
                    .Where(x => x.Client.Name == name)
                    .Select(x => new
                    {
                        x.Id,
                        x.Description,
                        x.Factory,
                        date = x.DateCreation
                    })
                    .ToListAsync();

                var dbClient = await db.Client
                    .Select(x => new
                    {
                        id = x.Id,
                        name = x.Name,
                        cpf = x.Cpf,
                        address = x.Address,
                        phone = x.Phone,
                        city = x.City.Name,
                        orders = dbOrder
                    })
                    .FirstOrDefaultAsync(x => x.name == name);

                if (dbClient != null)
                    response.Ok(dbClient);
                else
                    response.BadRequest("Cliente não existe.");
            }
            return response;
        }

        public async Task<ServiceResponse> GetFullClientByCpf(string cpf)
        {
            var response = new ServiceResponse();
            using (var db = new BrillDbContext())
            {
                var dbClient = await db.Client
                    .Select(x => new
                    {
                        id = x.Id,
                        name = x.Name,
                        cpf = x.Cpf,
                        address = x.Address,
                        phone = x.Phone,
                        city = x.City.Name,
                    })
                    .FirstOrDefaultAsync(x => x.cpf == Client.UnmaskCpf(cpf));

                if (dbClient != null)
                {
                    response.Ok(dbClient);
                }
                else
                {
                    response.BadRequest("Cliente não existe.");
                }
            }
            return response;
        }

        public async Task<ServiceResponse> GetCities()
        {
            var response = new ServiceResponse();

            using (var db = new BrillDbContext())
            {
                var dbCities = await db.City
                    .ToListAsync();

                response.Ok(dbCities);
            }

            return response;
        }

        public async Task<ServiceResponse> CreateClient(string name, string cpf, int userId, 
            string address, int cityId, string phone, string userSignature)
        {
            var response = new ServiceResponse();
            using (var db = new BrillDbContext())
            {
                var dbClient = await db.Client.FirstOrDefaultAsync(x => x.Name == name);
                if (dbClient == null) // O cliente não está registrado
                {
                    Client client = new Client
                    {
                        Name = name,
                        Cpf = cpf,
                        User = db.Users.FirstOrDefaultAsync(x => x.Id == userId).Result,
                        Address = address,
                        City = db.City.FirstOrDefaultAsync(x => x.Id == cityId).Result,
                        Phone = phone,
                    };

                    await db.Client.AddAsync(client);
                    await db.SaveChangesAsync();

                    LogService.WriteLog(userSignature, "cadastrou o cliente " + name + " com ID " + client.Id);
                    response.Ok(new { Id = client.Id });
                }
                else
                {
                    response.BadRequest("Nome de cliente já registrado.");
                }
            }

            return response;
        }
        
        public async Task<ServiceResponse> EditClient(ClientViewModel model, string userSignature)
        {
            var response = new ServiceResponse();
            using (var db = new BrillDbContext())
            {
                var dbClient = await db.Client.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (dbClient != null) // O cliente ESTÁ registrado
                {
                    var city = await db.City.FirstOrDefaultAsync(x => x.Id == model.CityId);
                    if (city != null) // A cidade existe
                    {
                        dbClient.Name = model.Name;
                        dbClient.Cpf = model.Cpf;
                        dbClient.Address = model.Address;
                        dbClient.Phone = model.Phone;
                        dbClient.City = city;

                        db.Client.Attach(dbClient);
                        db.Entry(dbClient).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        LogService.WriteLog(userSignature, "editou o cliente " + model.Name + " com ID " + dbClient.Id);
                        response.Ok(new { Id = dbClient.Id });
                    }
                    else
                    {
                        response.BadRequest("A cidade não existe.");
                    }
                }
                else
                {
                    response.BadRequest("O cliente não existe.");
                }
            }

            return response;
        }

        private async Task<ServiceResponse> SearchForCpf(string cpf)
        {
            string unmaskedCpf = Client.UnmaskCpf(cpf);
            var response = new ServiceResponse();
            using (var db = new BrillDbContext())
            {
                var dbClient = await db.Client
                    .Where(x => x.Cpf.Contains(unmaskedCpf))
                    .Select(x => new { id = x.Id, name = x.Cpf })
                    .ToListAsync();

                response.Ok(dbClient);
            }
            return response;
        }

        private async Task<ServiceResponse> SearchForName(string name)
        {
            var response = new ServiceResponse();
            using (var db = new BrillDbContext())
            {
                var dbClient = await db.Client
                    .Where(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                    .Select(x => new { id = x.Id, name = x.Name })
                    .ToListAsync();

                response.Ok(dbClient);
            }
            return response;
        }
    }
}
