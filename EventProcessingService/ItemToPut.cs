namespace EventProcessingService {

	public class ItemToPut {

		public int Id { get; set; }
		
		public override string ToString() {
			return (this.GetType().Name + " #" + this.Id);
		}

	}

}