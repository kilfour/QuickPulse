# Why QuickPulse Exists
*A.k.a. A deep dark forest, a looking glass, and a trail of dead generators.*

A little while back I was writing a test for a method that took some JSON as input.
I got my fuzzers out and went to work. And then... my fuzzers gave up.

So I added the following to **QuickMGenerate**:
```csharp
    var generator =
        from _ in MGen.For<Tree>().Depth(2, 5)
        from __ in MGen.For<Tree>().GenerateAsOneOf(typeof(Branch), typeof(Leaf))
        from ___ in MGen.For<Tree>().TreeLeaf<Leaf>()
        from tree in MGen.One<Tree>().Inspect()
        select tree;
```
Which can generate output like this:
```
    └── Node
        ├── Leaf(60)
        └── Node
            ├── Node
            │   ├── Node
            │   │   ├── Leaf(6)
            │   │   └── Node
            │   │       ├── Leaf(30)
            │   │       └── Leaf(21)
            │   └── Leaf(62)
            └── Leaf(97)
```
Neat. But this story isn't about the output, it's about the journey.  
Implementing this wasn't trivial. And I was, let's say, a muppet, more than once along the way.

Writing a unit test for a fixed depth like `(min:1, max:1)` or `(min:2, max:2)`? Not a problem.  
But when you're fuzzing with a range like `(min:2, max:5).` Yeah, ... good luck.

Debugging this kind of behavior was as much fun as writing an F# compiler in JavaScript.  
So I wrote a few diagnostic helpers: visualizers, inspectors, and composable tools
that could take a generated value and help me see why things were behaving oddly.

Eventually, I nailed the last bug and got tree generation working fine.

Then I looked at this little helper I'd written for combining stuff and thought: **"Now *that's* a nice-looking rabbit hole."**

One week and exactly nine combinators later, I had this surprisingly useful, lightweight little library.


