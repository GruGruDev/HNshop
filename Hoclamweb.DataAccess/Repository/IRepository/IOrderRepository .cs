using Suitshop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Suitshop.DataAccess.Repository.IRepository
{
	public interface IOrderRepository : IRepository<Order>
	{
		void Update(Order order);
		void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
		void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId);
	}
}
