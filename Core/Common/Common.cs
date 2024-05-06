using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;

namespace WCDS.WebFuncions.Core.Common
{
    public class Common
    {
        public static string[] filteredRateTypes = {
            "Accommodation",
            "Airport Fee",
            "Basing",
            "Basing Non-Core",
            "Basing Penalty",
            "Charter Minimums",
            "Crew Exp - Breakfast",
            "Crew Exp - Lunch",
            "Crew Exp - Dinner",
            "Crew Expenses",
            "Double Crew",
            "Flat",
            "Fuel",
            "Landing Fee",
            "Nav Canada",
            "Passenger Fee",
            "Standby",
            "Vehicle Rental",};

        public static string[] filteredRateUnits = {
            "Day",
            "Each",
            "Hour",
            "Invoice",
            "Kilometre",
            "Litre",
            "Mile",
            "Nautical Mile",}; 

        public bool ParseToken(IHeaderDictionary header, string key, out string response)
        {
            bool result = false;
            StringValues headerValue;
            if(header.TryGetValue(key, out headerValue))
            {
                var parts = headerValue.ToString().Split(" ");
                if (parts.Length != 2)
                {
                    response = "Malformed Authorization Header";
                }
                else
                {
                    var token = DecodeJwtToken(parts[1]);
                    if ( token.Payload != null  && token.Payload.ContainsKey("name"))
                    {
                        var part1 = token.Payload["name"];
                        if (part1 is string && string.IsNullOrEmpty((string)part1))
                        {
                            response = "No Name found in token";
                        }
                        else
                        {
                            response = (string)part1;
                            result = true;
                        }
                    }
                    else 
                    {
                        response = "B2B";
                        result = true;
                    }
                }
            }
            else
            {
                response = "System";
                result = true;
            }

            return result;
        }

        private JwtSecurityToken DecodeJwtToken(string encodedToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(encodedToken);
            return token;
        }
    }
}
