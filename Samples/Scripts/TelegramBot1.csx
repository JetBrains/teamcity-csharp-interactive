#r "nuget: csinetstandard11, 1.0.0"

using System.Collections.Generic;
using System.Linq;

var list = new List<int>{1, 2};
var list2 = list.Where(i => i == 1).ToList();
WriteLine(list2.Count);

new Abc.Class1();