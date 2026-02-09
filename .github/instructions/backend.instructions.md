---
applyTo: "src/**"
---

# Backend Instructions

Use 'using' single line statement instead of block statement.
```csharp
// Bad
using (var stream = new FileStream("file.txt", FileMode.Open))
{
  // code
}

// Good
using var stream = new FileStream("file.txt", FileMode.Open);
```

Plase each class/interface/record etc to it's own file.

Generate unit test files for each service/repository/handler etc. and place them in the tests folder, following the same structure as the src folder.