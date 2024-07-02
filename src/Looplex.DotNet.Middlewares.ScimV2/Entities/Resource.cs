using System.ComponentModel.DataAnnotations;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities
{
    public class Resource
    {
        #region Common

        [Required(ErrorMessage = "Id is required.")]
        public string Id { get; set; }

        public string ExternalId { get; set; }

        public Meta Meta { get; set; }

        #endregion
    }
}
