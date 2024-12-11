using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


namespace MyApi.Services
{
    public static class myBooksTokenService
    {
        private static SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SXkSqsKyNUyvGbnHs7ke2NCq8zQzNLW7mPmHbnZZ"));
        private static string issuer = "http://localhost:5099";
        public static SecurityToken GetToken(List<Claim> claims) =>
            new JwtSecurityToken(
                issuer,
                issuer,
                claims,
                expires: DateTime.Now.AddDays(30.0),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

        public static bool IsTokenExpired(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true, // הגדרת כך תפעיל את בדיקת התוקן על פי זמן התפוגה
                ValidateAudience = false, // ניתן לכבות את בדיקת קהל היעד אם אינך משתמש בה
                ValidateIssuer = false, // ניתן לכבות את בדיקת הגורם המנפיק אםd אינך משתמש בה
                RequireExpirationTime = true, // מחייב את התפוגה של הטוקן
                ClockSkew = TimeSpan.Zero // משתמש באפס על מנת לוודא שאנו משווים את הזמנים בדיוק
            };

            try
            {
                // פענוח הטוקן ובדיקת האימות שלו
                tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return validatedToken.ValidTo <= DateTime.UtcNow;
            }
            catch (SecurityTokenExpiredException)
            {
                // טוקן פג תוקף
                return true;
            }
            catch (Exception)
            {
                // אירעה שגיאה אחרת
                return true; // או false, תלוי מה תרצה לעשות
            }
        }
        public static TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidIssuer = issuer,
                ValidAudience = issuer,
                IssuerSigningKey = key,
                //ClockSkew = TimeSpan.Zero, // remove delay of token when expire
                LifetimeValidator = LifetimeValidator //TimeSpan.FromDays(30)

            };
        }

        public static string WriteToken(SecurityToken token) =>
            new JwtSecurityTokenHandler().WriteToken(token);


        public static bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken token, TokenValidationParameters validationParameters)
        {
            // קביעת התוקף של הטוקן ל-30 ימים
            var expirationDate = notBefore?.AddDays(30.0);

            // בדיקה האם התוקף המוגדר הוא קטן מהתאריך הנוכחי
            if (expirationDate < DateTime.UtcNow)
            {
                // התוקף פג, מסר תוקף לא חוקי
                return false;
            }
            
            // התוקף עדיין תקף
            return true;
        }
    }
}