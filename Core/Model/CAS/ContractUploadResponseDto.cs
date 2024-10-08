using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Model.CAS
{
    public class ContractUploadResponseDto
    {
        public CASContractDto Contract { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }
    }
}
