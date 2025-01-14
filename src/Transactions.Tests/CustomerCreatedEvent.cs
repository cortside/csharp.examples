namespace CSharpExamples {
    public class CustomerCreatedEvent {
        public CustomerCreatedEvent(int customerId) {
            CustomerId = customerId;
        }

        public int CustomerId { get; }
    }
}
