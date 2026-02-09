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