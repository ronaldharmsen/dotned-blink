var objectA = Pool.GetObject();

objectA.TempData = "Example";
Console.WriteLine(objectA.TempData);

Pool.ReleaseObject(objectA);