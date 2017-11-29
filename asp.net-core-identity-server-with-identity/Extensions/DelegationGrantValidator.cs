using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp.net_core_identity_server_with_identity.Extensions
{
    public class DelegationGrantValidator : IExtensionGrantValidator
    {
        private readonly ITokenValidator _validator;

        public string GrantType => "delegation";

        public DelegationGrantValidator(ITokenValidator validator)
        {
            _validator = validator;
        }
        
        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            // It's the job of the extension grant validator to handle that request by validating the incoming token,
            // and returning a result that represents the new token
            var userToken = context.Request.Raw.Get("token");

            if (string.IsNullOrEmpty(userToken))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }

            var result = await _validator.ValidateAccessTokenAsync(userToken);
            if (result.IsError)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }

            // get user's identity
            var sub = result.Claims.FirstOrDefault(c => c.Type == "sub").Value;

            context.Result = new GrantValidationResult(sub, "delegation");
            return;
        }
    }
}
