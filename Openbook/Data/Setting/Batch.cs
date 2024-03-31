using Openbook.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Openbook.Data.Setting
{
    public class Batch : IEntidadTenant
	{
		[Key]
		public int BatchId { get; set; }
        public string BatchNo { get; set; }
        public int ProductId { get; set; }
        public string Barcode { get; set; }
        public string batchNo { get; set; }
		public string TenantId { get; set; } = null!;
	}
}
