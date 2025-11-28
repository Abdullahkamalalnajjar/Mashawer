namespace Mashawer.Data.Entities.ClasssOfOrder
{
    public class RepresentitiveCancelOrder
    {
        public int Id { get; set; }

        public string Reason { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
//1- order status = pending , ordertaks = pending
//2- make endpoint marktaskorderiscomplete => ordertask status = compeleted

//3- make endpoint AddDeliveredTimeToOrderTask => ordertask deliveredat (represetitive) {
//"orderTaskId": 0,
//  "deliveredAt": "2025-11-23T23:36:30.092Z"
//}