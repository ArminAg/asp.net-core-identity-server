using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace asp.net_core_identity_server_with_identity.Models.ManageViewModels
{
    public class GenerateRecoveryCodesViewModel
    {
        public string[] RecoveryCodes { get; set; }
    }
}
