//Option 1
// var provider = new DefaultObjectPoolProvider();
// var policy = new StringBuilderPooledObjectPolicy();
// var pool = provider.Create(policy);

//Option 2
var policy = new DefaultPooledObjectPolicy<StringBuilder>();
var pool = new DefaultObjectPool<StringBuilder>(policy);

var myStringBuilder = pool.Get();
try
{
     myStringBuilder.Append("Hello");     
     myStringBuilder.Append(" ");
     myStringBuilder.Append("World");

     Console.WriteLine(myStringBuilder.ToString());
}
finally
{
    pool.Return(myStringBuilder);
}

