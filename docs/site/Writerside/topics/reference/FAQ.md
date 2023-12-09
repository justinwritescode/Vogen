# FAQ

## What tests are there
There are unit tests for Vogen itself, and there are snapshot tests to test that the source code that is generated
matches what is expected.

To run the unit tests for Vogen itself, you can either run them in your IDE, or via `Build.ps1`.
To run the snapshot tests, run `RunSnapshots.ps1`

## I want to add a test, where is the best place for it?

Most tests involve checking two, maybe three, things:
* checking that source generated is as expected (so the snapshot tests in the main solution)
* checking that behavior of the change works as expected (so a test in the `consumers` solution (`tests/consumers.sln`))
* (maybe) - if you added/changed behavior of the code that generates the source code (rather than just changing a template), then a unit test in the main solution

## I've changed the source that is generated, and I now have snapshot failures, what should I do?

There are a **lot** of snapshot tests. A lot of permutations of the following are run:
* framework
* class/struct/record class/record struct
* internal/public/sealed/readonly
* locales
* conversions, such as EF Core, JSON, etc. 

When the tests are run, it uses snapshot tests to compare the current output to the expected output.
If your feature/fix changes the output, the snapshot tests will bring up your configured code diff tool, for instance,
Beyond Compare, and show you the differences. 
If your change modifies what is generated, then it is likely that a lot of `verified` files will need update to match
the new source code that is generated.

To do this, run `RunSnapshots.ps1 -reset`. This will delete all the snapshots and treat what is generated
as the correct version. Needless to say, only do this if you're sure that the newly generated code is
correct.



## How do I identify types that are generated by Vogen?
_I'd like to be able to identify types that are generated by Vogen so that I can integrate them in things like EFCore._

**A: ** You can use this information on [this page](How-to-identify-a-type-that-is-generated-by-Vogen.md)