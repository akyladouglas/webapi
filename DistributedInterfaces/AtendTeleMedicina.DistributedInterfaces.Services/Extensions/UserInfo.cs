using System;
using System.Collections.Generic;
using System.Security.Claims;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using Microsoft.AspNetCore.Http;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Extensions
{
    public class UserInfo : IUser
    {
        private readonly IHttpContextAccessor _accessor;

        public UserInfo(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public string Name => _accessor.HttpContext.User.Identity.Name;

        public string GetUserId()
        {
            return IsAuthenticated() ? _accessor.HttpContext.User.FindFirst("uid").Value : "";
        }

        public string GetUserEmail()
        {
            return IsAuthenticated() ? _accessor.HttpContext.User.GetUserEmail() : "";
        }

        public string GetUserOrigem()
        {
            return IsAuthenticated() ? _accessor.HttpContext.User.FindFirst("scope").Value : "N/A";
        }

        public string GetUserIp()
        {
            var remoteIp = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            if (string.IsNullOrEmpty(remoteIp)) return "N/A";
            return remoteIp;
        }

        public bool IsAuthenticated()
        {
            return _accessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public bool IsInRole(string role)
        {
            return _accessor.HttpContext.User.IsInRole(role);
        }

        public IEnumerable<Claim> GetClaimsIdentity()
        {
            return _accessor.HttpContext.User.Claims;
        }
    }

    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentException(nameof(principal));
            }

            var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
            return claim?.Value;
        }

        public static string GetUserEmail(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentException(nameof(principal));
            }

            var claim = principal.FindFirst(ClaimTypes.Email);
            return claim?.Value;
        }
    }
}

