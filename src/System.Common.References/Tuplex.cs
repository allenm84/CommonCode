using System;

namespace System.Common.References
{
  /// <summary>
  /// A class used to stored multiple values.
  /// </summary>
  public class Tuplex<T1>
  {
    /// <summary>
    /// Gets or sets the first value.
    /// </summary>
    public T1 Value1 { get; set; }

    /// <summary>
    /// Constructs an empty tuple
    /// </summary>
    public Tuplex()
    {

    }

    /// <summary>
    /// Constructs a new Tuple with the specified value.
    /// </summary>
    /// <param name="value1">The value to store inside the tuple.</param>
    public Tuplex(T1 value1)
    {
      Value1 = value1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static implicit operator T1(Tuplex<T1> result)
    {
      return result.Value1;
    }
  }

  /// <summary>
  /// A class used to stored multiple values.
  /// </summary>
  public class Tuplex<T1, T2> : Tuplex<T1>
  {
    /// <summary>
    /// Gets or sets the second value.
    /// </summary>
    public T2 Value2 { get; set; }

    /// <summary>
    /// Constructs an empty tuple
    /// </summary>
    public Tuplex()
    {

    }

    /// <summary>
    /// Constructs a new Tuple with the specified value.
    /// </summary>
    /// <param name="value1">The value of the first object.</param>
    /// <param name="value2">The value of the second object.</param>
    public Tuplex(T1 value1, T2 value2)
      : base(value1)
    {
      Value2 = value2;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static implicit operator T2(Tuplex<T1, T2> result)
    {
      return result.Value2;
    }
  }

  /// <summary>
  /// A class used to stored multiple values.
  /// </summary>
  public class Tuplex<T1, T2, T3> : Tuplex<T1, T2>
  {
    /// <summary>
    /// Gets or sets the third value.
    /// </summary>
    public T3 Value3 { get; set; }

    /// <summary>
    /// Constructs an empty tuple
    /// </summary>
    public Tuplex()
    {

    }

    /// <summary>
    /// Constructs a new Tuple with the specified value.
    /// </summary>
    /// <param name="value1">The value of the first object.</param>
    /// <param name="value2">The value of the second object.</param>
    /// <param name="value3">The value of the third object.</param>
    public Tuplex(T1 value1, T2 value2, T3 value3)
      : base(value1, value2)
    {
      Value3 = value3;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static implicit operator T3(Tuplex<T1, T2, T3> result)
    {
      return result.Value3;
    }
  }

  /// <summary>
  /// A class used to stored multiple values.
  /// </summary>
  public class Tuplex<T1, T2, T3, T4> : Tuplex<T1, T2, T3>
  {
    /// <summary>
    /// Gets or sets the fourth value.
    /// </summary>
    public T4 Value4 { get; set; }

    /// <summary>
    /// Constructs an empty tuple
    /// </summary>
    public Tuplex()
    {

    }

    /// <summary>
    /// Constructs a new Tuple with the specified value.
    /// </summary>
    /// <param name="value1">The value of the first object.</param>
    /// <param name="value2">The value of the second object.</param>
    /// <param name="value3">The value of the third object.</param>
    /// <param name="value4">The value of the fourth object.</param>
    public Tuplex(T1 value1, T2 value2, T3 value3, T4 value4)
      : base(value1, value2, value3)
    {
      Value4 = value4;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static implicit operator T4(Tuplex<T1, T2, T3, T4> result)
    {
      return result.Value4;
    }
  }

  /// <summary>
  /// A class used to stored multiple values.
  /// </summary>
  public class Tuplex<T1, T2, T3, T4, T5> : Tuplex<T1, T2, T3, T4>
  {
    /// <summary>
    /// Gets or sets the fifth value.
    /// </summary>
    public T5 Value5 { get; set; }

    /// <summary>
    /// Constructs an empty tuple
    /// </summary>
    public Tuplex()
    {

    }

    /// <summary>
    /// Constructs a new Tuple with the specified value.
    /// </summary>
    /// <param name="first">The value of the first object.</param>
    /// <param name="second">The value of the second object.</param>
    /// <param name="third">The value of the third object.</param>
    /// <param name="fourth">The value of the fourth object.</param>
    /// <param name="fifth">The value of the fifth object.</param>
    public Tuplex(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
      : base(value1, value2, value3, value4)
    {
      Value5 = value5;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static implicit operator T5(Tuplex<T1, T2, T3, T4, T5> result)
    {
      return result.Value5;
    }
  }

  /// <summary>
  /// A class used to stored multiple values.
  /// </summary>
  public class Tuplex<T1, T2, T3, T4, T5, T6> : Tuplex<T1, T2, T3, T4, T5>
  {
    /// <summary>
    /// Gets or sets the sixth value.
    /// </summary>
    public T6 Value6 { get; set; }

    /// <summary>
    /// Constructs an empty tuple
    /// </summary>
    public Tuplex()
    {

    }

    /// <summary>
    /// Constructs a new Tuple with the specified value.
    /// </summary>
    /// <param name="value1">The value of the first object.</param>
    /// <param name="value2">The value of the second object.</param>
    /// <param name="value3">The value of the third object.</param>
    /// <param name="value4">The value of the fourth object.</param>
    /// <param name="value5">The value of the fifth object.</param>
    /// <param name="value6">The value of the sixth object.</param>
    public Tuplex(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
      : base(value1, value2, value3, value4, value5)
    {
      Value6 = value6;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static implicit operator T6(Tuplex<T1, T2, T3, T4, T5, T6> result)
    {
      return result.Value6;
    }
  }

  /// <summary>
  /// A class used to stored multiple values.
  /// </summary>
  public class Tuplex<T1, T2, T3, T4, T5, T6, T7> : Tuplex<T1, T2, T3, T4, T5, T6>
  {
    /// <summary>
    /// Gets or sets the seventh value.
    /// </summary>
    public T7 Value7 { get; set; }

    /// <summary>
    /// Constructs an empty tuple
    /// </summary>
    public Tuplex()
    {

    }

    /// <summary>
    /// Constructs a new Tuple with the specified value.
    /// </summary>
    /// <param name="value1">The value of the first object.</param>
    /// <param name="value2">The value of the second object.</param>
    /// <param name="value3">The value of the third object.</param>
    /// <param name="value4">The value of the fourth object.</param>
    /// <param name="value5">The value of the fifth object.</param>
    /// <param name="value6">The value of the sixth object.</param>
    /// <param name="value7">The value of the seventh object</param>
    public Tuplex(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
      : base(value1, value2, value3, value4, value5, value6)
    {
      Value7 = value7;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static implicit operator T7(Tuplex<T1, T2, T3, T4, T5, T6, T7> result)
    {
      return result.Value7;
    }
  }

  /// <summary>
  /// A class used to stored multiple values.
  /// </summary>
  public class Tuplex<T1, T2, T3, T4, T5, T6, T7, T8> : Tuplex<T1, T2, T3, T4, T5, T6, T7>
  {
    /// <summary>
    /// Gets or sets the eigth value.
    /// </summary>
    public T8 Value8 { get; set; }

    /// <summary>
    /// Constructs an empty tuple
    /// </summary>
    public Tuplex()
    {

    }

    /// <summary>
    /// Constructs a new Tuple with the specified value.
    /// </summary>
    /// <param name="value1">The value of the first object.</param>
    /// <param name="value2">The value of the second object.</param>
    /// <param name="value3">The value of the third object.</param>
    /// <param name="value4">The value of the fourth object.</param>
    /// <param name="value5">The value of the fifth object.</param>
    /// <param name="value6">The value of the sixth object.</param>
    /// <param name="value7">The value of the seventh object</param>
    /// <param name="value8">The value of the eigth object</param>
    public Tuplex(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
      : base(value1, value2, value3, value4, value5, value6, value7)
    {
      Value8 = value8;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static implicit operator T8(Tuplex<T1, T2, T3, T4, T5, T6, T7, T8> result)
    {
      return result.Value8;
    }
  }

  /// <summary>
  /// A class used to stored multiple values.
  /// </summary>
  public class Tuplex<T1, T2, T3, T4, T5, T6, T7, T8, T9> : Tuplex<T1, T2, T3, T4, T5, T6, T7, T8>
  {
    /// <summary>
    /// Gets or sets the ninth value.
    /// </summary>
    public T9 Value9 { get; set; }

    /// <summary>
    /// Constructs an empty tuple
    /// </summary>
    public Tuplex()
    {

    }

    /// <summary>
    /// Constructs a new Tuple with the specified value.
    /// </summary>
    /// <param name="value1">The value of the first object.</param>
    /// <param name="value2">The value of the second object.</param>
    /// <param name="value3">The value of the third object.</param>
    /// <param name="value4">The value of the fourth object.</param>
    /// <param name="value5">The value of the fifth object.</param>
    /// <param name="value6">The value of the sixth object.</param>
    /// <param name="value7">The value of the seventh object</param>
    /// <param name="value8">The value of the eigth object</param>
    /// <param name="value9">The value of the ninth object</param>
    public Tuplex(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9)
      : base(value1, value2, value3, value4, value5, value6, value7, value8)
    {
      Value9 = value9;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static implicit operator T9(Tuplex<T1, T2, T3, T4, T5, T6, T7, T8, T9> result)
    {
      return result.Value9;
    }
  }
}