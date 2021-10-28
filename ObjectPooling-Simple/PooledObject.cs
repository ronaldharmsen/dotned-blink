namespace DesignPattern.Objectpool
{
    // The PooledObject class is the type that is expensive or slow to instantiate,
    // or that has limited availability, so is to be held in the object pool.
    public class PooledObject
    {
        private DateTime _createdAt = DateTime.Now;
 
        public DateTime CreatedAt
        {
            get { return _createdAt; }
        }
 
        public string? TempData { get; set; }
    }
}