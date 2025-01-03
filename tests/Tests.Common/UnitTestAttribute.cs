using System;
using NUnit.Framework;

namespace Tests.Common;

/// <summary>
/// Marks the assembly, test fixture, or test as a unit test.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
public sealed class UnitTestAttribute : CategoryAttribute { }
