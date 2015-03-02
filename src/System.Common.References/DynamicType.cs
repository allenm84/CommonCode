using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace System.Common.References
{
  /// <summary>
  /// 
  /// </summary>
  public sealed class DynamicType
  {
    /// <summary>
    /// 
    /// </summary>
    public interface IDynamicTypeProperty
    {
      /// <summary>
      /// The name of the property.
      /// </summary>
      string Name { get; }

      /// <summary>
      /// The type of the property
      /// </summary>
      Type Type { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="properties"></param>
    /// <returns></returns>
    public static DynamicType GenerateType(string typeName, IEnumerable<IDynamicTypeProperty> properties)
    {
      // create a dynamic assembly and module 
      AssemblyName assemblyName = new AssemblyName("tmpAssembly");
      AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
      ModuleBuilder module = assemblyBuilder.DefineDynamicModule("tmpModule.dll");

      // create a new type builder
      TypeBuilder typeBuilder = module.DefineType(typeName, TypeAttributes.Class | TypeAttributes.Public);
      typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

      // Loop over the attributes that will be used as the properties names in out new type
      foreach (var value in properties)
      {
        string propertyName = value.Name;
        Type propertyType = value.Type;

        // Generate a private field
        var field = typeBuilder.DefineField("m" + propertyName, propertyType, FieldAttributes.Private);

        // Generate a public property
        var property = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

        // The property set and property get methods require a special set of attributes:
        MethodAttributes GetSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

        // Define the "get" accessor method for current private field.
        var currGetPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName, GetSetAttr, propertyType, Type.EmptyTypes);

        // Intermediate Language stuff...
        var currGetIL = currGetPropMthdBldr.GetILGenerator();
        currGetIL.Emit(OpCodes.Ldarg_0);
        currGetIL.Emit(OpCodes.Ldfld, field);
        currGetIL.Emit(OpCodes.Ret);

        // Define the "set" accessor method for current private field.
        var currSetPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName, GetSetAttr, null, new Type[] { propertyType });

        // Again some Intermediate Language stuff...
        var currSetIL = currSetPropMthdBldr.GetILGenerator();
        currSetIL.Emit(OpCodes.Ldarg_0);
        currSetIL.Emit(OpCodes.Ldarg_1);
        currSetIL.Emit(OpCodes.Stfld, field);
        currSetIL.Emit(OpCodes.Ret);

        // Last, we must map the two methods created above to our PropertyBuilder to 
        // their corresponding behaviors, "get" and "set" respectively. 
        property.SetGetMethod(currGetPropMthdBldr);
        property.SetSetMethod(currSetPropMthdBldr);
      }

      // Generate our type
      return new DynamicType
      {
        Name = typeName,
        Type = typeBuilder.CreateType(),
      };
    }

    /// <summary>
    /// 
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public Type Type { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    private DynamicType() { }
  }
}
