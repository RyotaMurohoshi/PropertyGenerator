# PropertyGenerator

`PropertyGenerator` is C# source generator is for Simple Property.

This project is under the develop🚧.

## Install

Under the develop.

## Usage

```csharp
using System;
using PropertyGenerator;

public partial class Product
{
    [GetterProperty(PropertyName = "Identifier")]
    private readonly int id;

    [GetterProperty] private readonly string name;

    public Product(string name, int id)
    {
        this.name = name;
        this.id = id;
    }
}
```

With `GetterProperty` attribute, below code is generated.

```csharp
public partial class Product
{
    public int Identifier => this.id;
    public string Name => this.name;
}
```

## Motivation

In the Unity Game Engine, field name is so important to serialize data correctly.
So we can not auto Auto-Implemented Properties with Serialized field in Unity. Auto-Implemented Properties has some prefix for its backing field.

```csharp
using System;
using UnityEngine;

public partial class Launcher: MonoBehaviour
{
    // This implementation's backing field is not good.
    // Serialized name is not good.
    [field:SerializeField]
    public float Speed { get; }

    [field:SerializeField]
    public GameObject Bullet { get; }

    public void Start() {
        /* ... */
    }
}
```

To avoid Auto-Implemented Properties usage, we need to write redundant field and property code.

```csharp
using System;
using UnityEngine;

public partial class Launcher: MonoBehaviour
{
    [SerializeField]
    private float speed;
    public float Speed => speed;

    [SerializeField]
    private GameObject bullet;
    public GameObject Bullet => bullet;

    public void Start() {
        /* ... */
    }
}
```

`PropertyGenerator.GetterProperty` generate property only adding attribute to field. In the future, this library will provide Property with serialized backing field in Unity Game Engine.

```csharp
using System;
using UnityEngine;
using PropertyGenerator;

public partial class Launcher: MonoBehaviour
{
    [SerializeField, GetterProperty]
    private float speed;

    [SerializeField, GetterProperty]
    private GameObject bullet;

    public void Start() {
        /* ... */
    }
}
```

## Plans

* Setter support
* both Getter and Setter support

## Author

Ryota Murohoshi is game Programmer in Japan.

* Posts:http://qiita.com/RyotaMurohoshi (JPN)
* Twitter:https://twitter.com/RyotaMurohoshi (JPN)

## License

This library is under MIT License.
