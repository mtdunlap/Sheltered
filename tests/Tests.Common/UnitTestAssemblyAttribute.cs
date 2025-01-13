using System;
using NUnit.Framework;

namespace Tests.Common;

/// <summary>
/// Marks the assembly, test fixture, or test as a unit test.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class UnitTestAssemblyAttribute : CategoryAttribute { }
