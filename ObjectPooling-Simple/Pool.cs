namespace DesignPattern.Objectpool
{

    // The Pool class controls access to the pooled objects. It maintains a list of available objects and a 
    // collection of objects that have been obtained from the pool and are in use. The pool ensures that released objects 
    // are returned to a suitable state, ready for reuse. 
    public static class Pool
    {
        private static List<PooledObject> _available = new List<PooledObject>();
        private static List<PooledObject> _inUse = new List<PooledObject>();
 
        public static PooledObject GetObject()
        {
            lock(_available)
            {
                if (_available.Count != 0)
                {
                    PooledObject po = _available[0];
                    _inUse.Add(po);
                    _available.RemoveAt(0);
                    return po;
                }
                else
                {
                    PooledObject po = new PooledObject();
                    _inUse.Add(po);
                    return po;
                }
            }
        }
 
        public static void ReleaseObject(PooledObject po)
        {
            CleanUp(po);
 
            lock (_available)
            {
                _available.Add(po);
                _inUse.Remove(po);
            }
        }
 
        private static void CleanUp(PooledObject po)
        {
            po.TempData = null;
        }
    }
}