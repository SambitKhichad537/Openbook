using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook .Data.Setting
{
    public class EmailSetting : IEntidadTenant
	{
		[Key]
		public int EmailSettingId { get; set; }
        [Required]
        public string MailHost { get; set; }
        [Required]
        public int MailPort { get; set; }
        [Required]
        public string MailAddress { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string MailFromname { get; set; }
        public string EncryptionName { get; set; }
		public string TenantId { get; set; } = null!;
	}
}
