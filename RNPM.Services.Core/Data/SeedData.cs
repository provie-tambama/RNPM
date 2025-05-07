using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RNPM.Common.Constants;
using RNPM.Common.Data;
using RNPM.Common.Models;
using Serilog;

namespace RNPM.API.Data
{
    public static class SeedData
    {
        //Roles Seed
        public static void SeedRoles(RnpmDbContext dbContext, RoleManager<ApplicationRole> roleManager)
        {
            var roles = ApplicationRoles.GetApplicationRoles();

            foreach (var role in roles)
            {
                if (role.Name == null) continue;
                
                var r = roleManager.FindByNameAsync(role.Name).Result;

                if (r == null)
                {
                    var result = roleManager.CreateAsync(new ApplicationRole
                        { Name = role.Name, Description = role.Description, RoleClass = role.RoleClass }).Result;

                    if (!result.Succeeded) throw new Exception($"Failed to Create Role {role.Description}");

                    r = roleManager.FindByNameAsync(role.Name).Result;

                    if (role.RoleClaims
                        .Select(claim =>
                            roleManager.AddClaimAsync(r, new Claim(claim.ClaimType, claim.ClaimValue)).Result)
                        .Any(rResult => !rResult.Succeeded))
                    {
                        throw new Exception("Failed to add Role Claim");
                    }
                }
                else
                {
                    var claims = dbContext.RoleClaims.Where(c => c.RoleId == r.Id).ToList();
                    var totalClaims = claims.Count;

                    if (totalClaims == role.RoleClaims.Count) continue;

                    if (totalClaims != 0)
                    {
                        dbContext.RemoveRange(claims);
                        dbContext.SaveChanges();
                    }

                    if (role.RoleClaims
                        .Select(claim =>
                            roleManager.AddClaimAsync(r, new Claim(claim.ClaimType, claim.ClaimValue)).Result)
                        .Any(rResult => !rResult.Succeeded))
                    {
                        throw new Exception("Failed to add Role Claim");
                    }
                }
            }
        }
        
        //Users Seed
        public static void SeedUsers(RnpmDbContext dbContext, RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            var admin = dbContext.Users.Any(u => u.UserName == "provietambama@gmail.com");

            if (!admin)
            {
                var user = new ApplicationUser()
                {
                    Email = "provietambama@gmail.com",
                    Fullname = "System Admin",
                    PhoneNumber = "0776130508",
                    UserName = "provietambama@gmail.com",
                    IsAdmin = true,
                };

                var result = userManager.CreateAsync(user, "Password+1").Result;

                if (!result.Succeeded) throw new Exception($"Failed to create user {user.UserName}");

                var role = roleManager.FindByNameAsync(ApiRoles.SystemAdministrator).Result;

                if (role == null)
                    if (role != null)
                        throw new Exception($"Role {role.Description} does not exist.");

                if (role != null)
                {
                    var roleUser = userManager.AddToRoleAsync(user, role.Name).Result;

                    if (!roleUser.Succeeded) throw new Exception($"Failed to add System Administrator to Role");
                }

                Log.Information("User created. {user}", user.UserName);
            }
        }

    }
}